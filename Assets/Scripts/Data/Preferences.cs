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
    private const string BGM_VOLUME_KEY = "BGM_Volume_Key";
    private const string SFX_VOLUME_KEY = "SFX_Volume_Key";
    
    private const float DEFAULT_VOLUME = 1f;

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
    
    public static void SaveBGMVolume(float value)
    {
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, value);
        PlayerPrefs.Save();
    }

    public static float GetBGMVolume()
    {
        return PlayerPrefs.HasKey(BGM_VOLUME_KEY) ? PlayerPrefs.GetFloat(BGM_VOLUME_KEY) : DEFAULT_VOLUME;
    }

    public static void SaveSFXVolume(float value)
    {
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, value);
        PlayerPrefs.Save();
    }

    public static float GetSFXVolume()
    {
        return PlayerPrefs.HasKey(SFX_VOLUME_KEY) ? PlayerPrefs.GetFloat(SFX_VOLUME_KEY) : DEFAULT_VOLUME;
    }
}