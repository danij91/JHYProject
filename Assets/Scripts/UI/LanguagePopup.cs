using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguagePopup : UIBase {
    [SerializeField] private Transform tr_grid;
    [SerializeField] private LanguageItem itemTemplate;
    [SerializeField] private Button btn_back;

    private int langCount;
    private List<LanguageItem> items = new List<LanguageItem>();
    private bool isLanguageItemInitialized;

    protected override void PrevOpen(params object[] args) {
        langCount = LocalizationManager.Instance.GetLanguageCount();
        CreateLanguageItems();
    }

    private void CreateLanguageItems() {
        if (isLanguageItemInitialized) {
            return;
        }

        isLanguageItemInitialized = true;
        
        for (int i = 0; i < langCount; i++) {
            LanguageItem newItem = Instantiate(itemTemplate, tr_grid);
            int curIndex = i;
            EConfig.ELanguage langEnum = (EConfig.ELanguage) i;
            newItem.SetOnClicked(() => OnClickLanguageButton(curIndex));
            string languageName = LocalizationManager.Instance.GetLocalizedText("language_" + langEnum);
            newItem.SetLanguageText(languageName);

            if (curIndex == LocalizationManager.Instance.GetCurrentLanguage()) {
                newItem.SetSelected();
            } else {
                newItem.SetDeselected();
            }

            items.Add(newItem);
        }
    }

    private void RefreshLanguages() {
        for (int i = 0; i < langCount; i++) {
            if (i == LocalizationManager.Instance.GetCurrentLanguage()) {
                items[i].SetSelected();
            } else {
                items[i].SetDeselected();
            }
        }
    }

    private void OnClickLanguageButton(int index) {
        LocalizationManager.Instance.ChangeLanguage(index);
        RefreshLanguages();
    }

    public override void OnButtonEvent(Button inButton) {
        switch (inButton.name) {
            case nameof(btn_back):
                Close();
                break;
        }
    }
}
