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


    private ECharacterType characterType;
    private CharacterInvenUI InvenUI;

    public void SetData(CharacterInvenUI ui, ECharacterType inType)
    {
        InvenUI = ui;
        characterType = inType;

        bool isSelected = characterType == InvenUI.CurrentCharacterType;
        img_select.gameObject.SetActive(isSelected);
        tmp_name.text = characterType.ToString().ToUpper();

        string iconPath = $"Image/Icon/{characterType}";
        Sprite iconSprite = ResourceManager.Instance.Load<Sprite>(iconPath);
        img_character.sprite = iconSprite;
    }

    public void OnClick_Select()
    {
        InvenUI.ResetSelectedItem();
        ActiveSelect(true);
        InvenUI.SetCurrentCharacter(characterType);
    }

    public void ActiveSelect(bool isSelect)
    {
        img_select.gameObject.SetActive(isSelect);
    }

}
