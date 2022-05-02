using UnityEngine;

public static class Preferences
{
    public enum ELanguageSettings
    {
        Korean,
        English,
        Japanese,
        Chinese,
    }

    private const ELanguageSettings DEFAULT_LANGUAGE = ELanguageSettings.Korean;

    public static void SaveLanguageSettings(ELanguageSettings value)
    {
        PlayerPrefs.SetInt(nameof(ELanguageSettings), (int)value);
        PlayerPrefs.Save();
    }

    public static ELanguageSettings GetLanguageSettings()
    {
        if (!PlayerPrefs.HasKey(nameof(ELanguageSettings)))
            return DEFAULT_LANGUAGE;

        return (ELanguageSettings)PlayerPrefs.GetInt(nameof(ELanguageSettings));
    }
}