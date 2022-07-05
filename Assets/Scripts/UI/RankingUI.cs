using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class RankingUI : UIBase {
    [SerializeField] private Button btn_back;
    [SerializeField] private RecordItem itemTemplate;
    [SerializeField] private Transform contentTr;
    [SerializeField] private RecordItem myRecordItem;

    private int MAX_RANKING_COUNT = 50;

    protected override void PrevOpen(params object[] args) {
        UserManager.Instance.LoadUserRecords().Forget();

        CreateRankingRecords().Forget();
        SetMyRecord().Forget();
    }

    private async UniTaskVoid CreateRankingRecords() {
        await UniTask.WaitUntil(() => UserManager.Instance.isRecordLoaded);
        var userRecords = UserManager.Instance.UserRecords;
        int count = 0;
        foreach (var userRecord in userRecords) {
            RecordItem item = count < contentTr.childCount
                ? contentTr.GetChild(count).GetComponent<RecordItem>()
                : Instantiate(itemTemplate, contentTr);

            if (count >= MAX_RANKING_COUNT) {
                return;
            }

            item.SetRank(count + 1);
            item.SetNickname(userRecord.nickname);
            item.SetScore(userRecord.score);
            count++;
        }
    }

    private async UniTaskVoid SetMyRecord() {
        await UniTask.WaitUntil(() => UserManager.Instance.isRecordLoaded);
        if (UserManager.Instance.myRecordIndex == -1) {
            return;
        }
        var myRecord = UserManager.Instance.UserRecords[UserManager.Instance.myRecordIndex];
        myRecordItem.SetRank(UserManager.Instance.myRecordIndex + 1);
        myRecordItem.SetNickname(myRecord.nickname);
        myRecordItem.SetScore(myRecord.score);
    }


    public override void OnButtonEvent(Button inButton) {
        switch (inButton.name) {
            case nameof(btn_back):
                Close();
                break;
        }
    }
}
