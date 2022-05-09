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

    public enum EAudioSettings
    {
        Play,
        Mute,
    }


    private const string LANGUAGE_SETTINGS_KEY = "Language_Settings_Key";
    private const string SFX_SETTINGS_KEY = "SFX_Settings_Key";
    private const string BGM_SETTINGS_KEY = "BGM_Settings_Key";

    private const ELanguageSettings DEFAULT_LANGUAGE = ELanguageSettings.Korean;
    private const EAudioSettings DEFAULT_AUDIO = EAudioSettings.Play;

    public static void SaveLanguageSettings(ELanguageSettings value)
    {
        PlayerPrefs.SetInt(LANGUAGE_SETTINGS_KEY, (int)value);
        PlayerPrefs.Save();
    }

    public static ELanguageSettings GetLanguageSettings()
    {
        if (!PlayerPrefs.HasKey(LANGUAGE_SETTINGS_KEY))
            return DEFAULT_LANGUAGE;

        return (ELanguageSettings)PlayerPrefs.GetInt(LANGUAGE_SETTINGS_KEY);
    }

    public static void SaveSFXSettings(EAudioSettings value)
    {
        PlayerPrefs.SetInt(SFX_SETTINGS_KEY, (int)value);
        PlayerPrefs.Save();
    }

    public static EAudioSettings GetSFXSettings()
    {
        if (!PlayerPrefs.HasKey(SFX_SETTINGS_KEY))
            return DEFAULT_AUDIO;

        return (EAudioSettings)PlayerPrefs.GetInt(SFX_SETTINGS_KEY);
    }

    public static void SaveBGMSettings(EAudioSettings value)
    {
        PlayerPrefs.SetInt(BGM_SETTINGS_KEY, (int)value);
        PlayerPrefs.Save();
    }

    public static EAudioSettings GetBGMSettings()
    {
        if (!PlayerPrefs.HasKey(BGM_SETTINGS_KEY))
            return DEFAULT_AUDIO;

        return (EAudioSettings)PlayerPrefs.GetInt(BGM_SETTINGS_KEY);
    }
}