using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public enum SFXType
{
    Fall,
    Jump,
    Success_Normal,
    Success_Perfect,
    BtnClick,
}

public enum BGMType
{
    Stop,
    Title,
    Lobby,
    InGame,
}

[RequireComponent(typeof(AudioSource))]
public class AudioManager : Singleton<AudioManager>
{
    public bool IsBgmMute { get; private set; }
    public bool IsSfxMute { get; private set; }
    public float BGMVolume => bgmVolume;
    public float SFXVolume => sfxVolume;

    private AudioSource bgmAudioSource;
    private List<Sfx> playingList = new List<Sfx>();
    private const float FADE_TIME = 1f;
    private float bgmVolume;
    private float sfxVolume;


    public void Initialize()
    {
        bgmAudioSource = GetComponent<AudioSource>();
        bgmAudioSource.loop = true;
        playingList.Clear();

        bgmVolume = Preferences.GetBGMVolume();
        sfxVolume = Preferences.GetSFXVolume();

        IsBgmMute = Preferences.GetBGMSettings() == Preferences.EAudioSettings.Mute;
        IsSfxMute = Preferences.GetSFXSettings() == Preferences.EAudioSettings.Mute;
        bgmAudioSource.mute = IsBgmMute;
    }

    public void SetBGMSettings(Preferences.EAudioSettings setting)
    {
        IsBgmMute = setting == Preferences.EAudioSettings.Mute;
        bgmAudioSource.mute = IsBgmMute;
        Preferences.SaveBGMSettings(setting);
    }

    public void SetSFXSettings(Preferences.EAudioSettings setting)
    {
        IsSfxMute = setting == Preferences.EAudioSettings.Mute;
        Preferences.SaveSFXSettings(setting);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        Preferences.SaveSFXVolume(volume);
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        if (!IsBgmMute)
            bgmAudioSource.volume = bgmVolume;

        Preferences.SaveBGMVolume(volume);
    }

    public void SFXPlay(SFXType type, Action endCallback = null, bool isLoop = false, bool isOverlap = false)
    {
        if (IsPlaying(type) && !isOverlap)
            return;

        if (IsSfxMute)
            return;

        string extraNum = string.Empty;
        if (type == SFXType.Jump)
        {
            int randomNum = UnityEngine.Random.Range(1, 3);
            extraNum = randomNum.ToString();
        }

        string sfxName = $"Sfx_{type}{extraNum}";
        Sfx sfx = PoolingManager.Instance.Create<Sfx>(EPoolingType.Sound, sfxName, null, isLoop, sfxVolume,
            endCallback);
        playingList.Add(sfx);
    }

    private bool IsPlaying(SFXType type)
    {
        return playingList.Select(x => x.SFXType == type).FirstOrDefault();
    }

    public void RemovePlayList(Sfx sfx)
    {
        if (playingList.Contains(sfx))
            playingList.Remove(sfx);
    }

    public void AllSFXStop()
    {
        foreach (var sfx in playingList)
            sfx.Stop(false);

        playingList.Clear();
    }

    public void BGMPlay(BGMType type, Action endCallback = null, bool isStopBefore = true)
    {
        AudioClip bgm = LoadBGM(type);
        bgmAudioSource.clip = bgm;

        if (isStopBefore)
            FadeOut(type, endCallback);
        else
            FadeIn(type, endCallback);
    }

    public void BGMStop(Action endCallback = null)
    {
        FadeOut(BGMType.Stop, endCallback);
    }

    public void FadeOut(BGMType type, Action endCallback)
    {
        bgmAudioSource.DOFade(0f, FADE_TIME).OnComplete(() =>
        {
            bgmAudioSource.Stop();
            if (type == BGMType.Stop)
                endCallback?.Invoke();
            else
                FadeIn(type, endCallback);
        });
    }

    public void FadeIn(BGMType type, Action endCallback)
    {
        AudioClip bgm = LoadBGM(type);
        bgmAudioSource.clip = bgm;
        bgmAudioSource.loop = true;
        bgmAudioSource.Play();
        bgmAudioSource.DOFade(bgmVolume, FADE_TIME).OnComplete(() => { endCallback?.Invoke(); });
    }

    private AudioClip LoadBGM(BGMType type)
    {
        string path = $"Sound/Bgm/Bgm_{type}";
        return ResourceManager.Instance.Load<AudioClip>(path);
    }
}