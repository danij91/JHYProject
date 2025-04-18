using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterInvenItem : MonoBehaviour {
    [SerializeField] private Image img_character;
    [SerializeField] private Image img_select;
    [SerializeField] private Image img_checkmark;
    [SerializeField] private TextMeshProUGUI tmp_name;

    public CharacterData CharacterData { get; private set; }
    public string CharacterId => CharacterData.characterId;

    private CharacterInvenUI InvenUI;

    public void SetData(CharacterInvenUI ui, CharacterData data) {
        InvenUI = ui;
        CharacterData = data;

        bool isSelected = data == InvenUI.GetCurrentCharacterData();
        img_select.gameObject.SetActive(isSelected);

        string characterName = LocalizationManager.Instance.GetLocalizedText( data.characterName);
        tmp_name.text = characterName;

        // 썸네일 연결 (없을 경우 fallback 처리 가능)
        if (data.thumbnail != null) {
            img_character.sprite = data.thumbnail;
        } else {
            img_character.sprite = ResourceManager.Instance.Load<Sprite>($"Image/Icon/{data.characterId}");
        }

        SetGrayScale();
        CheckMainCharacter();
    }

    public void SetGrayScale() {
        Material tempMat = Instantiate(img_character.material);
        img_character.material = tempMat;
        img_character.GrayScale(!CharacterInventory.Instance.IsValid(CharacterId));
    }

    public void CheckMainCharacter() {
        bool isMain = CharacterInventory.Instance.MainCharacterId == CharacterId;
        img_checkmark.gameObject.SetActive(isMain);
    }

    public void OnClick_Select() {
        InvenUI.ResetSelectedItem();
        ActiveSelect(true);
        InvenUI.SetCurrentCharacter(CharacterData);
    }

    public void ActiveSelect(bool isSelect) {
        img_select.gameObject.SetActive(isSelect);
    }
}