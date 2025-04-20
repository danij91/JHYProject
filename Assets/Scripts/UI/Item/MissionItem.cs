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

        titleText.text = data.title;
        descText.text = data.description;
        progressText.text = $"{currentProgress} / {data.requiredValue}";
        rewardText.text =
            $"{(data.rewardType == CurrencyType.Soft ? LocalizationManager.Instance.GetLocalizedText("currency_coin") : LocalizationManager.Instance.GetLocalizedText("currency_gem"))} x{data.rewardAmount}";

        bool isCompleted = currentProgress >= data.requiredValue;

        claimButton.interactable = isCompleted && !claimed;
        if (isCompleted && !claimed)
            claimButton.GetComponent<Image>().color = Color.yellow;
        claimButton.onClick.RemoveAllListeners();
        claimButton.onClick.AddListener(() =>
        {
            MissionManager.Instance.ClaimReward(missionId);
            missionCallback?.Invoke();
            // 이후 UI 갱신 필요
            claimButton.GetComponent<Image>().color = Color.gray;
            claimButton.interactable = false;
        });
    }
}