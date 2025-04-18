// CharacterInvenUI.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterInvenUI : UIBase {
    [SerializeField] private Button btn_select;
    [SerializeField] private Button btn_purchase;
    [SerializeField] private Button btn_back;
    [SerializeField] private TextMeshProUGUI tmp_select;
    [SerializeField] private Transform contentTr;
    [SerializeField] private CharacterInvenItem itemTemplate;
    [SerializeField] private Transform viewerTr;

    private CharacterData CurrentCharacterData;
    private GameObject currentModel;
    private Animator animator;

    private CharacterInventory Inven => CharacterInventory.Instance;
    private List<CharacterInvenItem> itemList = new List<CharacterInvenItem>();
    private List<CharacterData> allCharacters => Inven.GetAllCharacters();

    private string select;
    private string selected;
    private string purchaseTitle;
    private string purchaseMessage;

    protected override void PrevOpen(params object[] args) {
        CurrentCharacterData = Inven.GetSelectedCharacterData();
        CreateCharacterItems();
        RefreshCharacterViewer();
        RefreshButton();

        select = LocalizationManager.Instance.GetLocalizedText("characterInven_select");
        selected = LocalizationManager.Instance.GetLocalizedText("characterInven_selected");
        purchaseTitle = LocalizationManager.Instance.GetLocalizedText("characterInven_purchaseTitle");
        purchaseMessage = LocalizationManager.Instance.GetLocalizedText("characterInven_purchaseMessage");
        tmp_select.text = selected;
    }

    private void CreateCharacterItems() {
        itemList.Clear();

        for (int i = 0; i < allCharacters.Count; i++) {
            CharacterData data = allCharacters[i];

            CharacterInvenItem item = i < contentTr.childCount
                ? contentTr.GetChild(i).GetComponent<CharacterInvenItem>()
                : Instantiate(itemTemplate, contentTr);

            item.SetData(this, data);
            item.gameObject.SetActive(true);

            itemList.Add(item);
        }
    }

    private void RefreshCharacterViewer() {
        if (currentModel != null)
            DestroyImmediate(currentModel);
        
        if (CurrentCharacterData == null) return;

        currentModel = Instantiate(CurrentCharacterData.modelPrefab, viewerTr);
        currentModel.transform.localPosition = Vector3.zero;
        currentModel.transform.localRotation = Quaternion.Euler(0, 0, 0);
        currentModel.transform.localScale = Vector3.one * 400f;
        
        animator = currentModel.GetComponent<Animator>();

        if (animator != null && animator.runtimeAnimatorController != null) {
            animator.Play("Idle_A");
        }
    }

    public void SetCurrentCharacter(CharacterData data) {
        CurrentCharacterData = data;
        RefreshCharacterViewer();
        RefreshButton();
    }

    public CharacterData GetCurrentCharacterData() {
        return CurrentCharacterData;
    }

    public void ResetSelectedItem() {
        foreach (var item in itemList) {
            item.ActiveSelect(false);
        }
    }

    private void RefreshButton() {
        bool isValid = Inven.IsValid(CurrentCharacterData.characterId);
        btn_purchase.gameObject.SetActive(!isValid);
        btn_select.gameObject.SetActive(isValid);
        tmp_select.text = CurrentCharacterData.characterId == Inven.MainCharacterId ? selected : select;
    }

    private void SelectCharacter() {
        if (!Inven.IsValid(CurrentCharacterData.characterId)) return;

        Inven.SelectCharacter(CurrentCharacterData.characterId);
        RefreshButton();

        foreach (var item in itemList) {
            item.CheckMainCharacter();
        }
    }

    private void PurchaseCharacter() {
        if (Inven.IsValid(CurrentCharacterData.characterId)) return;

        UIManager.Instance.Show<MessageBoxUI>(ui => {
            viewerTr.gameObject.SetActive(false);
            ui.SetMessage(purchaseMessage, purchaseTitle, () => {
                Inven.Add(CurrentCharacterData.characterId);
                RefreshButton();
                itemList.Find(x => x.CharacterId == CurrentCharacterData.characterId)?.SetGrayScale();
            }, null);
        }, ui =>
        {
            viewerTr.gameObject.SetActive(true);
            animator.Play("Idle_A");
        });
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