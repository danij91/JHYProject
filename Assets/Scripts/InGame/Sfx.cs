using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Sfx : PoolingObject {
    [SerializeField] private SFXType sfxType;

    public SFXType SFXType => sfxType;

    private AudioSource audioSource;
    private Action endCallback = null;
    private float volume = 1f;
    private bool isLoop = false;

    internal override void OnInitialize(params object[] parameters) {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        audioSource.mute = AudioManager.Instance.IsSfxMute;
        isLoop = (bool) parameters[0];
        volume = (float) parameters[1];

        if (parameters.Length > 2)
            endCallback = (Action) parameters[2];

        audioSource.loop = isLoop;
        audioSource.volume = volume;
        StartCoroutine(Co_Play());
    }

    protected override void OnUse() { }

    protected override void OnRestore() { }

    private IEnumerator Co_Play() {
        audioSource.Play();
        if (!isLoop) {
            yield return new WaitForSeconds(audioSource.clip.length);
            OnPlayEnd();
        }
    }

    public void Stop(bool isRemove = true) {
        StopCoroutine(Co_Play());
        audioSource.Stop();
        OnPlayEnd(isRemove);
    }

    public void OnPlayEnd(bool isRemove = true) {
        endCallback?.Invoke();
        if (isRemove)
            AudioManager.Instance.RemovePlayList(this);
        Restore();
    }
}
