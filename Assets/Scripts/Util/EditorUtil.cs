#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Google;
using UnityEditor;
using UnityEngine;

public class DeletePlayerPrefsScript : EditorWindow {
    [MenuItem("Window/Delete PlayerPrefs (All)")]
    static void DeleteAllPlayerPrefs() {
        FirebaseAuth.DefaultInstance.SignOut();
        PlayerPrefs.DeleteAll();
    }
}
#endif
