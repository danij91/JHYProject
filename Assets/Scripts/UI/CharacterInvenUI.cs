using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInvenUI : UIBase
{
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

    protected override void PrevOpen(params object[] args)
    {
        CurrentCharacterType = Inven.MainCharacter;
        RefreshButton();
        CreateCharacterItems();
        ActiveSelectedCharacter();
    }

    protected override void PrevClose()
    {
    }

    public void ResetSelectedItem()
    {
        foreach (var item in itemList)
            item.ActiveSelect(false);
    }

    public void SetCurrentCharacter(ECharacterType type)
    {
        CurrentCharacterType = type;
        RefreshButton();
        ActiveSelectedCharacter();
    }

    public void RefreshButton()
    {
        bool isVaild = Inven.IsVaild(CurrentCharacterType);
        btn_purchase.gameObject.SetActive(!isVaild);
        btn_select.gameObject.SetActive(isVaild);
        tmp_select.text = CurrentCharacterType == Inven.MainCharacter ? "SELECTED" : "TOSELECT";
    }

    private void CreateViewerCharacter()
    {
        characterList.Clear();
        for (int i = 0; i < CharacterCount; i++)
        {
            int type = 1 << i;
            Player character = Instantiate(ResourceManager.Instance.Load<Player>($"Prefabs/Character/Player_{(ECharacterType)type}"), viewerTr);
            character.transform.GetChild(0).GetChild(0).gameObject.layer = LayerMask.NameToLayer("UI");
            character.GetComponent<Rigidbody>().isKinematic = true;
            character.gauge.gameObject.SetActive(false);
            character.transform.localPosition = Vector3.zero;
            character.transform.localRotation = Quaternion.identity;
            character.transform.localScale = Vector3.one * 150f;
            characterList.Add((ECharacterType)type, character);
        }
    }

    private void CreateCharacterItems()
    {
        itemList.Clear();
        for (int i =0; i < CharacterCount; i++)
        {
            CharacterInvenItem item = i < contentTr.childCount
                ? contentTr.GetChild(i).GetComponent<CharacterInvenItem>()
                : Instantiate(itemTemplate, contentTr);

            int type = 1 << i;
            item.SetData(this, (ECharacterType)type);
            item.gameObject.SetActive(true);

            itemList.Add(item);
        }
    }

    private void ActiveSelectedCharacter()
    {
        if (characterList.Count != CharacterCount || characterList.Count == 0)
            CreateViewerCharacter();

        foreach (var pair in characterList)
            pair.Value.gameObject.SetActive(false);

        if (characterList.ContainsKey(CurrentCharacterType))
            characterList[CurrentCharacterType].gameObject.SetActive(true);
    }

    private void SelectCharacter()
    {
        if (!Inven.IsVaild(CurrentCharacterType))
            return;

        Inven.SelectCharacter(CurrentCharacterType);
        RefreshButton();
    }

    private void PurchaseCharacter()
    {
        // ToDo 재화로 구매할 때 재화충분한지 체크 추가
        if (Inven.IsVaild(CurrentCharacterType))
            return;

        Inven.Add(CurrentCharacterType);
        RefreshButton();
    }


    public override void OnButtonEvent(Button inButton)
    {
        switch (inButton.name)
        {
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
