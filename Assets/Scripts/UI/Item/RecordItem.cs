using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecordItem : MonoBehaviour {
    [SerializeField] private TMP_Text txt_rank;
    [SerializeField] private TMP_Text txt_nickname;
    [SerializeField] private TMP_Text txt_score;

    public void SetRank(int ranking) {
        txt_rank.text = ranking + ".";
    }

    public void SetRankEmpty() {
        txt_rank.text = "";
    }

    public void SetNickname(string nickname) {
        txt_nickname.text = nickname;
    }

    public void SetScore(int score) {
        txt_score.text = score.ToString();
    }
}
