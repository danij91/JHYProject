using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Sfx : PoolingObject {
    private AudioSource audioSource;

    internal override void OnInitialize(params object[] parameters) {
        audioSource = GetComponent<AudioSource>();
    }

    protected override void OnUse() { }

    protected override void OnRestore() { }

    public void Play() {
        if (AudioManager.Instance.IsSfxMute) {
            return;
        }
        audioSource.Play();
    }
}
