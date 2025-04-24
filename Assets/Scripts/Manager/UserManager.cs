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

public class UserManager : Singleton<UserManager>
{
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

    private void CheckFirebaseDependencies()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                if (task.Result == DependencyStatus.Available)
                    auth = FirebaseAuth.DefaultInstance;
                else
                    AddToInformation("Could not resolve all Firebase dependencies: " + task.Result.ToString());
            }
            else
            {
                AddToInformation("Dependency check was not completed. Error : " + task.Exception.Message);
            }
        });
    }

    public async void SignInWithGoogle(Action onNewUser = null, Action onOldUser = null)
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        AddToInformation("Calling SignIn");

        var task = GoogleSignIn.DefaultInstance.SignIn();
        try
        {
            var idToken = await OnAuthenticationFinished(task);
            await SignInWithGoogleOnFirebase(idToken);

            if (await IsNewUser())
            {
                onNewUser?.Invoke();
            }
            else
            {
                onOldUser?.Invoke();
            }

            await LoadUserData();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void OnDisconnect()
    {
        GoogleSignIn.DefaultInstance.Disconnect();
    }

    private async UniTask<string> OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        await task;
        if (task.IsFaulted)
        {
            using (IEnumerator<Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                    AddToInformation("Got Error: " + error.Status + " " + error.Message);
                    throw error;
                }

                throw task.Exception;
            }
        }

        if (task.IsCanceled)
        {
            throw new Exception("OnAuthenticationFinished canceled");
        }

        AddToInformation("Welcome: " + task.Result.DisplayName + "!");
        AddToInformation("Email = " + task.Result.Email);
        AddToInformation("Google ID Token = " + task.Result.IdToken);
        AddToInformation("Email = " + task.Result.Email);
        return task.Result.IdToken;
    }

    private async UniTask<FirebaseUser> SignInWithGoogleOnFirebase(string idToken)
    {
        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);
        var task = auth.SignInWithCredentialAsync(credential);
        await task;

        AggregateException ex = task.Exception;
        if (ex != null)
        {
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

    private void AddToInformation(string str)
    {
        Debug.Log(str);
    }

    public void Initialize()
    {
        firestore = FirebaseFirestore.DefaultInstance;
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestEmail = true,
            RequestIdToken = true
        };
        CheckFirebaseDependencies();
    }

    public void UpdateScore(int score)
    {
        if (CurrentUserRecord == null)
        {
            return;
        }

        CurrentUserRecord.score = score;
        firestore.Collection("scores").Document(GetCurrentUserId()).SetAsync(CurrentUserRecord);
    }


    public void SignUp()
    {
        auth.CreateUserWithEmailAndPasswordAsync("testEmail@test.com", "testpw1000").ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }

            if (task.IsFaulted)
            {
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

    public async void SignInWithApple(Action onSuccess = null, Action onFailed = null)
    {
    }

    public async void SignInWithEmail(Action onSuccess = null, Action onFailed = null)
    {
    }

    public async void SignInAnonymously(Action onSuccess = null, Action onFailed = null)
    {
        var signInAnonymouslyTask = auth.SignInAnonymouslyAsync();
        await signInAnonymouslyTask;

        if (signInAnonymouslyTask.IsCanceled)
        {
            Debug.LogError("SignInAnonymouslyAsync was canceled.");
            onFailed?.Invoke();
            return;
        }

        if (signInAnonymouslyTask.IsFaulted)
        {
            Debug.LogError("SignInAnonymouslyAsync encountered an error: " + signInAnonymouslyTask.Exception);
            onFailed?.Invoke();
            return;
        }

        FirebaseUser newUser = signInAnonymouslyTask.Result.User;
        currentUser = newUser;
        
        await LoadUserData();
        Debug.LogFormat("User signed in successfully: {0} ({1})",
            newUser.DisplayName, newUser.UserId);
        onSuccess?.Invoke();
    }

    public async UniTask LoadUserData()
    {
        currentUser = auth.CurrentUser;

        var userDoc = await firestore.Collection("users").Document(GetCurrentUserId()).GetSnapshotAsync();
        if (userDoc.Exists)
        {
            CurrentUserData = userDoc.ConvertTo<UserData>();
        }
        else
        {
            CurrentUserData = new UserData { nickname = "", characters = new List<string>() };
            LocalDataHelper.SaveMainCharacterId(EConfig.Character.INITIAL_CHARACTER_ID);
        }

        CharacterInventory.Instance.SetValidCharacters(CurrentUserData.characters);

        var scoreDoc = await firestore.Collection("scores").Document(GetCurrentUserId()).GetSnapshotAsync();
        if (scoreDoc.Exists)
        {
            CurrentUserRecord = scoreDoc.ConvertTo<UserRecord>();
        }
        else
        {
            CurrentUserRecord = new UserRecord { nickname = CurrentUserData.nickname, score = 0 };
        }

        RefreshEnergy();
        CheckAndResetDailyMissions();
    }


    public void SignOut()
    {
        auth.SignOut();
        CharacterInventory.Instance.ResetCharacter();
    }

    public void SignOutFromGoogle()
    {
        GoogleSignIn.DefaultInstance.SignOut();
        auth.SignOut();
    }

    public bool IsSignedIn()
    {
        return auth.CurrentUser != null;
    }

    public bool IsAnonymous()
    {
        return currentUser.IsAnonymous;
    }

    public string GetCurrentUserId()
    {
#if UNITY_EDITOR
        return currentUser == null ? "editor_test" : currentUser.UserId;
#else
        return currentUser.UserId;
#endif
    }

    public async UniTaskVoid LoadUserRecords()
    {
        isRecordLoaded = false;
        myRecordIndex = -1;
        UserRecords.Clear();

        var query = firestore.Collection("scores").OrderByDescending("score");
        var snapshot = await query.GetSnapshotAsync();

        int index = 0;
        foreach (var doc in snapshot.Documents)
        {
            var record = doc.ConvertTo<UserRecord>();
            UserRecords.Add(record);

            if (doc.Id == GetCurrentUserId())
            {
                myRecordIndex = index;
            }

            index++;
        }

        isRecordLoaded = true;
    }

    public void SetUserNickname(string nickname)
    {
        CurrentUserData = new UserData
            { nickname = nickname, characters = new List<string> { EConfig.Character.INITIAL_CHARACTER_ID } };
        firestore.Collection("users").Document(GetCurrentUserId()).SetAsync(CurrentUserData);

        CurrentUserRecord = new UserRecord { nickname = nickname, score = 0 };
        firestore.Collection("scores").Document(GetCurrentUserId()).SetAsync(CurrentUserRecord);

        LocalDataHelper.SaveMainCharacterId(EConfig.Character.INITIAL_CHARACTER_ID);
        CharacterInventory.Instance.SetValidCharacters(CurrentUserData.characters);
    }

    public async UniTask<bool> IsNewUser()
    {
        var doc = await firestore.Collection("users").Document(GetCurrentUserId()).GetSnapshotAsync();
        return !doc.Exists;
    }


    public void UpdateUserData()
    {
        firestore.Collection("users").Document(GetCurrentUserId()).SetAsync(CurrentUserData);
    }

    public async UniTask UpdateUserDataAsync()
    {
        await firestore.Collection("users")
            .Document(GetCurrentUserId())
            .SetAsync(CurrentUserData);
    }
    
    private void CheckAndResetDailyMissions()
    {
        string today = DateTime.UtcNow.ToString("yyyy-MM-dd");

        if (CurrentUserData.lastDailyResetDate != today)
        {
            Debug.Log($"🗓 Daily reset triggered (prev: {CurrentUserData.lastDailyResetDate}, now: {today})");
            MissionManager.Instance.ResetDailyMissions();
            CurrentUserData.lastDailyResetDate = today;
            UpdateUserData();
        }
    }
    
    public void RefreshEnergy()
    {
        var user = CurrentUserData;

        if (user.energy >= EConfig.System.MAX_ENERGY_COUNT)
            return;

        DateTime last = user.energyLastUpdated.ToDateTime();
        TimeSpan passed = DateTime.UtcNow - last;

        int recoverable = (int)(passed.TotalMinutes / EConfig.System.ENERGY_RECOVER_INTERVAL_MINUTES);
        if (recoverable <= 0)
            return;

        int newEnergy = Mathf.Min(user.energy + recoverable, EConfig.System.MAX_ENERGY_COUNT);
        int usedRecovery = newEnergy - user.energy;

        if (usedRecovery > 0)
        {
            user.energy = newEnergy;
            user.energyLastUpdated = Timestamp.FromDateTime(last.AddMinutes(usedRecovery * EConfig.System.ENERGY_RECOVER_INTERVAL_MINUTES));
            UpdateUserData(); // 인스턴스 메서드 호출 가능
        }
    }
    
    public bool TryConsumeEnergy()
    {
        RefreshEnergy();

        if (CurrentUserData.energy <= 0)
            return false;

        CurrentUserData.energy--;
        UpdateUserData();
        return true;
    }
}


[FirestoreData]
public class UserData
{
    //무료화폐
    [FirestoreProperty] public int coin { get; set; }
    //유료화폐
    [FirestoreProperty] public int gem { get; set; }
    [FirestoreProperty] public string nickname { get; set; }
    [FirestoreProperty] public List<string> characters { get; set; }
    [FirestoreProperty] public Dictionary<string, int> missionProgress { get; set; } = new();
    [FirestoreProperty] public List<string> claimedMissions { get; set; } = new();
    [FirestoreProperty] public int totalPlayCount { get; set; }
    [FirestoreProperty] public int maxJump { get; set; }
    [FirestoreProperty] public int maxCombo { get; set; }
    [FirestoreProperty] public int maxScore { get; set; }
    [FirestoreProperty] public int totalJump { get; set; }
    [FirestoreProperty] public int totalCombo { get; set; }
    [FirestoreProperty] public int totalScore { get; set; }
    [FirestoreProperty] public int adWatchedCount { get; set; }
    [FirestoreProperty] public string lastDailyResetDate { get; set; }
    [FirestoreProperty] public int energy { get; set; } = 10;
    [FirestoreProperty] public Timestamp energyLastUpdated { get; set; } = Timestamp.GetCurrentTimestamp();
}

[FirestoreData]
public class UserRecord
{
    [FirestoreProperty] public string nickname { get; set; }

    [FirestoreProperty] public int score { get; set; }
}