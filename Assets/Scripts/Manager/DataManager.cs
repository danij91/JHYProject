using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;

public class DataManager : Singleton<DataManager> {
    private DatabaseReference reference;

    public void Initialize() {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void UpdateScore(int score) {
        //임시
        reference.Child("score").Child("userId").SetValueAsync(score);
    }
}
