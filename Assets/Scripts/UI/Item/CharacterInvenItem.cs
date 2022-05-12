using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInvenItem : MonoBehaviour
{
    [SerializeField] private Image img_character;
    [SerializeField] private Image img_select;
    [SerializeField] private TextMeshProUGUI tmp_name;


    public ECharacterType CharacterType { get; private set; }
    private CharacterInvenUI InvenUI;

    public void SetData(CharacterInvenUI ui, ECharacterType inType)
    {
        InvenUI = ui;
        CharacterType = inType;

        bool isSelected = CharacterType == InvenUI.CurrentCharacterType;
        img_select.gameObject.SetActive(isSelected);
        tmp_name.text = CharacterType.ToString().ToUpper();
        string iconPath = $"Image/Icon/{CharacterType}";
        img_character.sprite = ResourceManager.Instance.Load<Sprite>(iconPath);
        SetGrayScale();
    }

    public void SetGrayScale()
    {
        Material tempMat = Instantiate(img_character.material);
        img_character.material = tempMat;
        img_character.GrayScale(!CharacterInventory.Instance.IsVaild(CharacterType));
    }

    public void OnClick_Select()
    {
        InvenUI.ResetSelectedItem();
        ActiveSelect(true);
        InvenUI.SetCurrentCharacter(CharacterType);
    }

    public void ActiveSelect(bool isSelect)
    {
        img_select.gameObject.SetActive(isSelect);
    }

}
