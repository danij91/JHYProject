using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class CharacterInvenUI : UIBase {
    [SerializeField] private Button btn_select;
    [SerializeField] private Button btn_purchase;
    [SerializeField] private Button btn_back;
    [SerializeField] private TextMeshProUGUI tmp_select;
    [SerializeField] private Transform contentTr;
    [SerializeField] private CharacterInvenItem itemTemplate;
    [SerializeField] private Transform viewerTr;

    public ECharacterType CurrentCharacterType { get; private set; }
    private CharacterInventory Inven => CharacterInventory.Instance;
    private int CharacterCount => Enum.GetValues(typeof(ECharacterType)).Length - 1;

    private List<CharacterInvenItem> itemList = new List<CharacterInvenItem>();
    private Dictionary<ECharacterType, Player> characterList = new Dictionary<ECharacterType, Player>();
    private string select;
    private string selected;
    private string purchaseTitle;
    private string purchaseMessage;

    protected override void PrevOpen(params object[] args) {
        CurrentCharacterType = Inven.MainCharacter;
        RefreshButton();
        CreateCharacterItems();
        RefreshCharacterViewer();
        select = LocalizationManager.Instance.GetLocalizedText("characterInven_select");
        selected = LocalizationManager.Instance.GetLocalizedText("characterInven_selected");
        purchaseTitle = LocalizationManager.Instance.GetLocalizedText("characterInven_purchaseTitle");
        purchaseMessage = LocalizationManager.Instance.GetLocalizedText("characterInven_purchaseMessage");
        tmp_select.text = CurrentCharacterType == Inven.MainCharacter ? selected : select;
    }
    

    protected override void PrevClose() { }

    public void ResetSelectedItem() {
        foreach (var item in itemList)
            item.ActiveSelect(false);
    }

    public void SetCurrentCharacter(ECharacterType type) {
        CurrentCharacterType = type;
        RefreshButton();
        RefreshCharacterViewer();
    }

    public void RefreshButton() {
        bool isValid = Inven.IsValid(CurrentCharacterType);
        btn_purchase.gameObject.SetActive(!isValid);
        btn_select.gameObject.SetActive(isValid);
        tmp_select.text = CurrentCharacterType == Inven.MainCharacter ? selected : select;
    }

    private void CreateCharacterViewer() {
        characterList.Clear();
        for (int i = 0; i < CharacterCount; i++) {
            Player character =
                Instantiate(ResourceManager.Instance.Load<Player>($"Prefabs/Character/Player_{(ECharacterType) i}"),
                    viewerTr);
            character.transform.GetChild(0).GetChild(0).gameObject.layer = LayerMask.NameToLayer("UI");
            character.GetComponent<Rigidbody>().isKinematic = true;
            character.gauge.gameObject.SetActive(false);
            character.transform.localPosition = Vector3.zero;
            character.transform.localRotation = Quaternion.identity;
            character.transform.localScale = Vector3.one * 150f;
            characterList.Add((ECharacterType) i, character);
        }
    }

    private void CreateCharacterItems() {
        itemList.Clear();
        for (int i = 0; i < CharacterCount; i++) {
            CharacterInvenItem item = i < contentTr.childCount
                ? contentTr.GetChild(i).GetComponent<CharacterInvenItem>()
                : Instantiate(itemTemplate, contentTr);

            item.SetData(this, (ECharacterType) i);
            item.gameObject.SetActive(true);

            itemList.Add(item);
        }
    }

    private void RefreshCharacterViewer() {
        if (characterList.Count != CharacterCount || characterList.Count == 0)
            CreateCharacterViewer();

        foreach (var pair in characterList)
            pair.Value.gameObject.SetActive(false);

        if (characterList.ContainsKey(CurrentCharacterType))
            characterList[CurrentCharacterType].gameObject.SetActive(true);
    }

    private void SelectCharacter() {
        if (!Inven.IsValid(CurrentCharacterType))
            return;

        Inven.SelectCharacter(CurrentCharacterType);
        RefreshButton();

        foreach (var item in itemList)
            item.CheckMainCharacter();
    }

    private void PurchaseCharacter() {
        // ToDo 재화로 구매할 때 재화충분한지 체크 추가
        if (Inven.IsValid(CurrentCharacterType))
            return;

        UIManager.Instance.Show<MessageBoxUI>(ui => {
            viewerTr.gameObject.SetActive(false);
            ui.SetMessage(purchaseMessage, purchaseTitle, () => {
                Inven.Add(CurrentCharacterType);
                RefreshButton();
                CharacterInvenItem item = itemList.Find(x => x.CharacterType == CurrentCharacterType);
                item.SetGrayScale();
            }, null);
        }, ui => { viewerTr.gameObject.SetActive(true); });
    }

    public override void OnButtonEvent(Button inButton) {
        switch (inButton.name) {
            case nameof(btn_back):
                Close();
                break;
            case nameof(btn_select):
                SelectCharacter();
                break;
            case nameof(btn_purchase):
                PurchaseCharacter();
                break;
        }
    }
}
