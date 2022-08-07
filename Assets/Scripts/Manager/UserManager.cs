using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Google;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class UserManager : Singleton<UserManager> {
    private DatabaseReference reference;
    private FirebaseAuth auth;
    private FirebaseUser currentUser;
    public List<UserRecord> UserRecords { get; private set; } = new List<UserRecord>();
    public bool isRecordLoaded { get; private set; }
    public int myRecordIndex { get; private set; }
    public UserData CurrentUserData { get; private set; }
    public UserRecord CurrentUserRecord { get; set; }
    public Text infoText;
    public string webClientId = "148045014698-mq8qk0fnopli9oto1e9klalbd3atm118.apps.googleusercontent.com";

    private GoogleSignInConfiguration configuration;

    private void CheckFirebaseDependencies() {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            if (task.IsCompleted) {
                if (task.Result == DependencyStatus.Available)
                    auth = FirebaseAuth.DefaultInstance;
                else
                    AddToInformation("Could not resolve all Firebase dependencies: " + task.Result.ToString());
            } else {
                AddToInformation("Dependency check was not completed. Error : " + task.Exception.Message);
            }
        });
    }

    public async void SignInWithGoogle(Action onNewUser = null, Action onOldUser = null) {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        AddToInformation("Calling SignIn");

        var task = GoogleSignIn.DefaultInstance.SignIn();
        try {
            var idToken = await OnAuthenticationFinished(task);
            await SignInWithGoogleOnFirebase(idToken);

            if (await IsNewUser()) {
                
                onNewUser?.Invoke();
            } else {
                onOldUser?.Invoke();
            }
        }
        catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public void OnDisconnect() {
        GoogleSignIn.DefaultInstance.Disconnect();
    }

    private async UniTask<string> OnAuthenticationFinished(Task<GoogleSignInUser> task) {
        await task;
        if (task.IsFaulted) {
            using (IEnumerator<Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator()) {
                if (enumerator.MoveNext()) {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException) enumerator.Current;
                    AddToInformation("Got Error: " + error.Status + " " + error.Message);
                    throw error;
                }

                throw task.Exception;
            }
        }

        if (task.IsCanceled) {
            throw new Exception("OnAuthenticationFinished canceled");
        }

        AddToInformation("Welcome: " + task.Result.DisplayName + "!");
        AddToInformation("Email = " + task.Result.Email);
        AddToInformation("Google ID Token = " + task.Result.IdToken);
        AddToInformation("Email = " + task.Result.Email);
        return task.Result.IdToken;
    }

    private async UniTask<FirebaseUser> SignInWithGoogleOnFirebase(string idToken) {
        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);
        var task = auth.SignInWithCredentialAsync(credential);
        await task;

        AggregateException ex = task.Exception;
        if (ex != null) {
            if (ex.InnerExceptions[0] is FirebaseException inner && (inner.ErrorCode != 0))
                AddToInformation("\nError code = " + inner.ErrorCode + " Message = " + inner.Message);
            throw ex;
        }

        currentUser = task.Result;
        AddToInformation("Sign In Successful.");
        return task.Result;
    }

    // public void OnSignInSilently() {
    //     GoogleSignIn.Configuration = configuration;
    //     GoogleSignIn.Configuration.UseGameSignIn = false;
    //     GoogleSignIn.Configuration.RequestIdToken = true;
    //     AddToInformation("Calling SignIn Silently");
    //
    //     GoogleSignIn.DefaultInstance.SignInSilently().ContinueWith(OnAuthenticationFinished);
    // }
    //
    // public void OnGamesSignIn() {
    //     GoogleSignIn.Configuration = configuration;
    //     GoogleSignIn.Configuration.UseGameSignIn = true;
    //     GoogleSignIn.Configuration.RequestIdToken = false;
    //
    //     AddToInformation("Calling Games SignIn");
    //
    //     GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    // }

    private void AddToInformation(string str) {
        Debug.Log(str);
    }

    public void Initialize() {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        configuration = new GoogleSignInConfiguration
            {WebClientId = webClientId, RequestEmail = true, RequestIdToken = true};
        CheckFirebaseDependencies();
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

    // public async void SignInWithGoogle(Action onSuccess = null, Action onFailed = null) {
    //     // Firebase.Auth.Credential credential =
    //     //     Firebase.Auth.GoogleAuthProvider.GetCredential(googleIdToken, googleAccessToken);
    //     // auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
    //     //     if (task.IsCanceled) {
    //     //         Debug.LogError("SignInWithCredentialAsync was canceled.");
    //     //         return;
    //     //     }
    //     //     if (task.IsFaulted) {
    //     //         Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
    //     //         return;
    //     //     }
    //     //
    //     //     Firebase.Auth.FirebaseUser newUser = task.Result;
    //     //     Debug.LogFormat("User signed in successfully: {0} ({1})",
    //     //         newUser.DisplayName, newUser.UserId);
    //     // });
    // }

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
    
    public void SignOutFromGoogle() {
        GoogleSignIn.DefaultInstance.SignOut();
        auth.SignOut();
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

    public async UniTask<bool> IsNewUser() {
        var task = reference.Child("users").Child(GetCurrentUserId()).GetValueAsync();
        await task;

        if (task.IsFaulted) {
            throw task.Exception;
        }

        if (task.IsCanceled) {
            throw new Exception("IsNewUser canceled");
        }

        return !task.Result.Exists;
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
