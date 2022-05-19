using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;

public class DataManager : Singleton<DataManager> {
    private DatabaseReference reference;
    private FirebaseAuth auth;
    private FirebaseUser currentUser;

    public void Initialize() {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
    }

    public void UpdateScore(int score) {
        reference.Child("score").Child(GetCurrentUserId()).SetValueAsync(score);
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

    public async void SignInAnonymously(Action onSuccess, Action onFailed = null) {
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
}
