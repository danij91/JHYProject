using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private Button claimButton;

    private string missionId;
    private Action missionCallback;

    public void SetCallback(Action callback)
    {
        missionCallback = callback;
    }

    public void Set(MissionData data, int currentProgress, bool claimed)
    {
        missionId = data.id;

        titleText.text = LocalizationManager.Instance.GetLocalizedText(data.title);
        descText.text = LocalizationManager.Instance.GetLocalizedText(data.description);
        progressText.text = $"{currentProgress} / {data.requiredValue}";
        rewardText.text =
            $"{(data.rewardType == CurrencyType.Soft ? LocalizationManager.Instance.GetLocalizedText("currency_coin") : LocalizationManager.Instance.GetLocalizedText("currency_gem"))} x{data.rewardAmount}";

        bool isCompleted = MissionManager.Instance.IsMissionCompleted(data);

        claimButton.interactable = isCompleted && !claimed;
        claimButton.GetComponent<Image>().color = isCompleted && !claimed ? Color.yellow : Color.gray;

        claimButton.onClick.RemoveAllListeners();
        claimButton.onClick.AddListener(() =>
        {
            MissionManager.Instance.ClaimReward(missionId);
            missionCallback?.Invoke();
            Set(data, MissionManager.Instance.GetCurrentProgress(data), true); // 상태 갱신
        });
    }
}