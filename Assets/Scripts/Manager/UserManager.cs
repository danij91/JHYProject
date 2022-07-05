using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class UserManager : Singleton<UserManager> {
    private DatabaseReference reference;
    private FirebaseAuth auth;
    private FirebaseUser currentUser;
    public List<UserRecord> UserRecords { get; private set; } = new List<UserRecord>();
    public bool isRecordLoaded { get; private set; }
    public int myRecordIndex { get; private set; }
    public UserData CurrentUserData { get; private set; }
    public UserRecord CurrentUserRecord { get; set; }

    public void Initialize() {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
    }

    public void UpdateScore(int score) {
        CurrentUserRecord.score = score;
        string json = JsonUtility.ToJson(CurrentUserRecord);

        reference.Child("scores").Child(GetCurrentUserId()).SetValueAsync(json);
    }

    public void SignUp() {
        auth.CreateUserWithEmailAndPasswordAsync("testEmail@test.com", "testpw1000").ContinueWith(task => {
            if (task.IsCanceled) {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }

            if (task.IsFaulted) {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            // Firebase user has been created.
            FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
    }

    public async void SignInWithGoogle(Action onSuccess = null, Action onFailed = null) {
        // Firebase.Auth.Credential credential =
        //     Firebase.Auth.GoogleAuthProvider.GetCredential(googleIdToken, googleAccessToken);
        // auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
        //     if (task.IsCanceled) {
        //         Debug.LogError("SignInWithCredentialAsync was canceled.");
        //         return;
        //     }
        //     if (task.IsFaulted) {
        //         Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
        //         return;
        //     }
        //
        //     Firebase.Auth.FirebaseUser newUser = task.Result;
        //     Debug.LogFormat("User signed in successfully: {0} ({1})",
        //         newUser.DisplayName, newUser.UserId);
        // });
    }

    public async void SignInWithApple(Action onSuccess = null, Action onFailed = null) { }
    public async void SignInWithEmail(Action onSuccess = null, Action onFailed = null) { }

    public async void SignInAnonymously(Action onSuccess = null, Action onFailed = null) {
        var signInAnonymouslyTask = auth.SignInAnonymouslyAsync();
        await signInAnonymouslyTask;

        if (signInAnonymouslyTask.IsCanceled) {
            Debug.LogError("SignInAnonymouslyAsync was canceled.");
            onFailed?.Invoke();
            return;
        }

        if (signInAnonymouslyTask.IsFaulted) {
            Debug.LogError("SignInAnonymouslyAsync encountered an error: " + signInAnonymouslyTask.Exception);
            onFailed?.Invoke();
            return;
        }

        FirebaseUser newUser = signInAnonymouslyTask.Result;
        currentUser = newUser;
        Debug.LogFormat("User signed in successfully: {0} ({1})",
            newUser.DisplayName, newUser.UserId);
        onSuccess?.Invoke();
    }

    public async UniTask LoadUserData() {
        currentUser = auth.CurrentUser;
        var loadUserDataTask = reference.Child("users").Child(GetCurrentUserId()).GetValueAsync();
        var loadUserDataResult = await loadUserDataTask;

        if (loadUserDataTask.IsCompleted && loadUserDataResult.Exists) {
            CurrentUserData = JsonUtility.FromJson<UserData>(loadUserDataResult.Value.ToString());
        } else {
            CurrentUserData = new UserData {nickname = "", characters = new List<int>()};
            LocalDataHelper.SaveMainCharacter((int) EConfig.Character.INITIAL_CHARACTER);
        }

        CharacterInventory.Instance.SetValidCharacters(CurrentUserData.characters);

        var loadUserRecordTask = reference.Child("scores").Child(GetCurrentUserId()).GetValueAsync();
        var loadUserRecordResult = await loadUserRecordTask;

        if (loadUserRecordTask.IsCompleted && loadUserRecordResult.Exists) {
            CurrentUserRecord = JsonUtility.FromJson<UserRecord>(loadUserRecordResult.Value.ToString());
        } else {
            CurrentUserRecord = new UserRecord {nickname = CurrentUserData.nickname, score = 0};
        }
    }

    public void SignOut() {
        auth.SignOut();
        CharacterInventory.Instance.ResetCharacter();
    }

    public bool IsSignedIn() {
        return auth.CurrentUser != null;
    }

    public bool IsAnonymous() {
        return currentUser.IsAnonymous;
    }

    public string GetCurrentUserId() {
#if UNITY_EDITOR
        return currentUser == null ? "editor_test" : currentUser.UserId;
#endif
        return currentUser.UserId;
    }

    public async UniTaskVoid LoadUserRecords() {
        isRecordLoaded = false;
        myRecordIndex = -1;
        UserRecords.Clear();
        var task = reference.Child("scores").OrderByChild("score").GetValueAsync();
        var result = await task;

        if (task.IsCompleted) {
            foreach (var r in result.Children) {
                var userRecord = JsonUtility.FromJson<UserRecord>(r.Value.ToString());
                UserRecords.Add(userRecord);
                if (r.Key == GetCurrentUserId()) {
                    myRecordIndex = UserRecords.Count - 1;
                }
            }

            isRecordLoaded = true;
        }
    }

    public void SetUserNickname(string nickname) {
        CurrentUserData = new UserData {nickname = nickname, characters = new List<int>()};
        CurrentUserData.characters.Add((int) EConfig.Character.INITIAL_CHARACTER);
        var json = JsonUtility.ToJson(CurrentUserData);
        reference.Child("users").Child(GetCurrentUserId()).SetValueAsync(json);
        LocalDataHelper.SaveMainCharacter((int) EConfig.Character.INITIAL_CHARACTER);
        CharacterInventory.Instance.SetValidCharacters(CurrentUserData.characters);
        CurrentUserRecord = new UserRecord {nickname = nickname, score = 0};
    }

    public void UpdateUserData() {
        var json = JsonUtility.ToJson(CurrentUserData);
        reference.Child("users").Child(GetCurrentUserId()).SetValueAsync(json);
    }
}

public class UserRecord {
    public string nickname;
    public int score;
}

public class UserData {
    public string nickname;
    public List<int> characters;
}
