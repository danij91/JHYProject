using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : Singleton<AudioManager> {
    private AudioSource bgmAudioSource;
    public bool IsBgmMute => bgmAudioSource.mute;
    public bool IsSfxMute { get; private set; }

    public void Initialize() {
        bgmAudioSource = GetComponent<AudioSource>();

        int bgmMuteAsNumber = PlayerPrefs.GetInt("isBgmMute", 1);
        int sfxMuteAsNumber = PlayerPrefs.GetInt("isSfxMute", 1);

        bgmAudioSource.mute = bgmMuteAsNumber == 1;
        IsSfxMute = sfxMuteAsNumber == 1;
    }

    public void SetBgmMute(bool isMute) {
        bgmAudioSource.mute = isMute;
        int bgmMuteAsNumber = isMute ? 1 : 0;
        PlayerPrefs.SetInt("isBgmMute", bgmMuteAsNumber);
    }

    public void SetSfxMute(bool isMute) {
        IsSfxMute = isMute;
        int sfxMuteAsNumber = isMute ? 1 : 0;
        PlayerPrefs.SetInt("isSfxMute", sfxMuteAsNumber);
    }
}
