using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class DataManager : Singleton<DataManager> {
    private DatabaseReference reference;
    private FirebaseAuth auth;
    private FirebaseUser currentUser;
    public List<UserRecord> UserRecords { get; private set; } = new List<UserRecord>();
    public bool isRecordLoaded { get; private set; }
    public int myRecordIndex { get; private set; }
    public UserData CurrentUserData { get; private set; }
    public UserRecord CurrentUserRecord { get;  set; }

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

    public void SignOut() {
        auth.SignOut();
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
