using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SocialPlatforms;

public class LocalizationManager : Singleton<LocalizationManager> {
    public void Initialize() { }

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
}
