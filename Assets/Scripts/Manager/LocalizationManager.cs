using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SocialPlatforms;

public class LocalizationManager : Singleton<LocalizationManager> {
    public void Initialize() {
        int savedLang = LocalDataHelper.GetLanguage();
        if (GetCurrentLanguage() != savedLang) {
            LocalizationSettings.SelectedLocale =
                LocalizationSettings.AvailableLocales.Locales[savedLang];
        }
    }

    public string GetLocalizedText(string key) {
        LocalizedString localizeString = new LocalizedString()
            {TableReference = "LanguageTable", TableEntryReference = key};
        var stringOperation = localizeString.GetLocalizedStringAsync();

        if (stringOperation.IsDone && stringOperation.Status == AsyncOperationStatus.Succeeded) {
            return stringOperation.Result;
        } else {
            return null;
        }
    }

    public void ChangeLanguage(int index) {
        LocalizationSettings.SelectedLocale =
            LocalizationSettings.AvailableLocales.Locales[index];
        LocalDataHelper.SetLanguage();
    }

    public int GetLanguageCount() {
        return LocalizationSettings.AvailableLocales.Locales.Count;
    }

    public int GetCurrentLanguage() {
        return LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
    }
}
