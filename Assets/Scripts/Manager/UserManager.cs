using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Google;
using UnityEngine;
using UnityEngine.UI;

public class UserManager : Singleton<UserManager> {
    private FirebaseFirestore firestore;
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
            
            LoadUserData().Forget();
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
        firestore = FirebaseFirestore.DefaultInstance;
        configuration = new GoogleSignInConfiguration {
            WebClientId = webClientId,
            RequestEmail = true,
            RequestIdToken = true
        };
        CheckFirebaseDependencies();
    }

    public void UpdateScore(int score) {
        if (CurrentUserRecord == null)
        {
            return;
        }
        CurrentUserRecord.score = score;
        firestore.Collection("scores").Document(GetCurrentUserId()).SetAsync(CurrentUserRecord);
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

            FirebaseUser newUser = task.Result.User;
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

        FirebaseUser newUser = signInAnonymouslyTask.Result.User;
        currentUser = newUser;
        Debug.LogFormat("User signed in successfully: {0} ({1})",
            newUser.DisplayName, newUser.UserId);
        onSuccess?.Invoke();
    }

    public async UniTask LoadUserData() {
        currentUser = auth.CurrentUser;

        var userDoc = await firestore.Collection("users").Document(GetCurrentUserId()).GetSnapshotAsync();
        if (userDoc.Exists) {
            CurrentUserData = userDoc.ConvertTo<UserData>();
        } else {
            CurrentUserData = new UserData {nickname = "", characters = new List<int>()};
            LocalDataHelper.SaveMainCharacter((int)EConfig.Character.INITIAL_CHARACTER);
        }

        CharacterInventory.Instance.SetValidCharacters(CurrentUserData.characters);

        var scoreDoc = await firestore.Collection("scores").Document(GetCurrentUserId()).GetSnapshotAsync();
        if (scoreDoc.Exists) {
            CurrentUserRecord = scoreDoc.ConvertTo<UserRecord>();
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

        var query = firestore.Collection("scores").OrderByDescending("score");
        var snapshot = await query.GetSnapshotAsync();

        int index = 0;
        foreach (var doc in snapshot.Documents) {
            var record = doc.ConvertTo<UserRecord>();
            UserRecords.Add(record);

            if (doc.Id == GetCurrentUserId()) {
                myRecordIndex = index;
            }
            index++;
        }

        isRecordLoaded = true;
    }

    public void SetUserNickname(string nickname) {
        CurrentUserData = new UserData {nickname = nickname, characters = new List<int> { (int)EConfig.Character.INITIAL_CHARACTER }};
        firestore.Collection("users").Document(GetCurrentUserId()).SetAsync(CurrentUserData);
    
        CurrentUserRecord = new UserRecord {nickname = nickname, score = 0};
        firestore.Collection("scores").Document(GetCurrentUserId()).SetAsync(CurrentUserRecord);

        LocalDataHelper.SaveMainCharacter((int)EConfig.Character.INITIAL_CHARACTER);
        CharacterInventory.Instance.SetValidCharacters(CurrentUserData.characters);
    }

    public async UniTask<bool> IsNewUser() {
        var doc = await firestore.Collection("users").Document(GetCurrentUserId()).GetSnapshotAsync();
        return !doc.Exists;
    }


    public void UpdateUserData() {
        firestore.Collection("users").Document(GetCurrentUserId()).SetAsync(CurrentUserData);
    }

}


[FirestoreData]
public class UserData
{
    [FirestoreProperty]
    public string nickname { get; set; }

    [FirestoreProperty]
    public List<int> characters { get; set; }
}

[FirestoreData]
public class UserRecord
{
    [FirestoreProperty]
    public string nickname { get; set; }

    [FirestoreProperty]
    public int score { get; set; }
}