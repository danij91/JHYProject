// CharacterInvenUI.cs

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UI.ProceduralImage;

public class CharacterInvenUI : UIBase
{
    [SerializeField] private Button btn_select;
    [SerializeField] private Button btn_purchase;
    [SerializeField] private Button btn_back;
    [SerializeField] private Button btn_getCoin;
    [SerializeField] private Button btn_getGem;
    
    [SerializeField] private TMP_Text txt_gem;
    [SerializeField] private TMP_Text txt_coin;
    [SerializeField] private TMP_Text txt_price;
    [SerializeField] private TextMeshProUGUI tmp_select;
    [SerializeField] private Transform contentTr;
    [SerializeField] private CharacterInvenItem itemTemplate;
    [SerializeField] private Transform viewerTr;
    [SerializeField] private Image img_currency;
    [SerializeField] private Sprite sprite_coin;
    [SerializeField] private Sprite sprite_gem;
    
    private CharacterData CurrentCharacterData;
    private GameObject currentModel;
    private Animator animator;
    private ProceduralImage purchaseBtnBg;

    private CharacterInventory Inven => CharacterInventory.Instance;
    private List<CharacterInvenItem> itemList = new List<CharacterInvenItem>();
    private List<CharacterData> allCharacters => Inven.GetAllCharacters();

    private Color currentPurchaseBtnBgColor = new Color32(244, 209, 155, 255);
    private Color inactivePurchaseBtnBgColor = Color.gray;

    private string select;
    private string selected;
    private string purchaseTitle;
    private string purchaseMessage;

    protected override void PrevOpen(params object[] args)
    {
        CurrentCharacterData = Inven.GetSelectedCharacterData();
        CreateCharacterItems();
        RefreshCharacterViewer();
        RefreshButton();

        btn_back.AddOnClickListener(()=>Close());
        btn_select.AddOnClickListener(SelectCharacter);
        btn_purchase.AddOnClickListener(PurchaseCharacter);
        btn_getCoin.AddOnClickListener(()=>Debug.Log("getCoin called"));
        btn_getGem.AddOnClickListener(()=>Debug.Log("getGem called"));
        
        select = LocalizationManager.Instance.GetLocalizedText("characterInven_select");
        selected = LocalizationManager.Instance.GetLocalizedText("characterInven_selected");
        purchaseTitle = LocalizationManager.Instance.GetLocalizedText("characterInven_purchaseTitle");
        purchaseMessage = LocalizationManager.Instance.GetLocalizedText("characterInven_purchaseMessage");
        tmp_select.text = selected;
        
        purchaseBtnBg = btn_purchase.GetComponent<ProceduralImage>();
        purchaseBtnBg.color = currentPurchaseBtnBgColor;
        ResetUserCurrency();
    }

    private bool IsPurchasable()
    {
        if (CurrentCharacterData.coinPrice != 1)
        {
            return UserManager.Instance.CurrentUserData.coin >= CurrentCharacterData.coinPrice;
        }

        return UserManager.Instance.CurrentUserData.gem >= CurrentCharacterData.gemPrice;
    }

    private void ResetUserCurrency()
    {
        txt_gem.text = UserManager.Instance.CurrentUserData.gem.ToString();
        txt_coin.text = UserManager.Instance.CurrentUserData.coin.ToString();
    }

    private void CreateCharacterItems()
    {
        itemList.Clear();

        for (int i = 0; i < allCharacters.Count; i++)
        {
            CharacterData data = allCharacters[i];

            CharacterInvenItem item = i < contentTr.childCount
                ? contentTr.GetChild(i).GetComponent<CharacterInvenItem>()
                : Instantiate(itemTemplate, contentTr);

            item.SetData(this, data);
            item.gameObject.SetActive(true);

            itemList.Add(item);
        }
    }

    private void RefreshCharacterViewer()
    {
        if (currentModel != null)
            DestroyImmediate(currentModel);

        if (CurrentCharacterData == null) return;

        currentModel = Instantiate(CurrentCharacterData.modelPrefab, viewerTr);
        currentModel.transform.localPosition = Vector3.zero;
        currentModel.transform.localRotation = Quaternion.Euler(0, 0, 0);
        currentModel.transform.localScale = Vector3.one * 400f;

        animator = currentModel.GetComponent<Animator>();

        if (animator != null && animator.runtimeAnimatorController != null)
        {
            animator.Play("Idle_A");
        }

        bool isPurchased = Inven.IsValid(CurrentCharacterData.characterId);

        txt_price.gameObject.SetActive(!isPurchased);
        img_currency.gameObject.SetActive(!isPurchased);
        
        if (isPurchased)
        {
            return;
        }

        if (CurrentCharacterData.coinPrice != -1)
        {
            img_currency.sprite = sprite_coin;
            txt_price.text = CurrentCharacterData.coinPrice.ToString();
            txt_price.color = UIColors.TEXT_COIN;
        }
        else
        {
            img_currency.sprite = sprite_gem;
            txt_price.text = CurrentCharacterData.gemPrice.ToString();
            txt_price.color = UIColors.TEXT_GEM;
        }
    }

    public void SetCurrentCharacter(CharacterData data)
    {
        CurrentCharacterData = data;
        RefreshCharacterViewer();
        RefreshButton();
    }

    public CharacterData GetCurrentCharacterData()
    {
        return CurrentCharacterData;
    }

    public void ResetSelectedItem()
    {
        foreach (var item in itemList)
        {
            item.ActiveSelect(false);
        }
    }

    private void RefreshButton()
    {
        bool isValid = Inven.IsValid(CurrentCharacterData.characterId);
        btn_purchase.gameObject.SetActive(!isValid);
        if (!isValid)
        {
            RefreshPurchaseButton();
        }

        btn_select.gameObject.SetActive(isValid);
        tmp_select.text = CurrentCharacterData.characterId == Inven.MainCharacterId ? selected : select;
    }

    private void RefreshPurchaseButton()
    {
        var isPurchasable = this.IsPurchasable();
        btn_purchase.interactable = isPurchasable;
        purchaseBtnBg.color = isPurchasable ? currentPurchaseBtnBgColor : inactivePurchaseBtnBgColor;
    }

    private void SelectCharacter()
    {
        if (!Inven.IsValid(CurrentCharacterData.characterId)) return;

        Inven.SelectCharacter(CurrentCharacterData.characterId);
        RefreshButton();
        RefreshButton();

        foreach (var item in itemList)
        {
            item.CheckMainCharacter();
        }
    }

    private void PurchaseCharacter()
    {
        if (Inven.IsValid(CurrentCharacterData.characterId)) return;

        UIManager.Instance.Show<MessageBoxUI>(ui =>
        {
            viewerTr.gameObject.SetActive(false);
            ui.SetMessage(purchaseMessage, purchaseTitle, () =>
            {
                if (CurrentCharacterData.coinPrice != -1)
                {
                    if (UserManager.Instance.CurrentUserData.coin >= CurrentCharacterData.coinPrice)
                    {
                        UserManager.Instance.CurrentUserData.coin -= CurrentCharacterData.coinPrice;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    if (UserManager.Instance.CurrentUserData.gem >= CurrentCharacterData.gemPrice)
                    {
                        UserManager.Instance.CurrentUserData.gem -= CurrentCharacterData.gemPrice;
                    }
                    else
                    {
                        return;
                    }
                }

                ResetUserCurrency();
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
}