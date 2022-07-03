using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;


[CanEditMultipleObjects]
public class ButtonClickSfxPlayer : MonoBehaviour, IPointerDownHandler {
    [SerializeField] private SFXType sfxType = SFXType.BtnClick;

    public void OnPointerDown(PointerEventData eventData) {
        AudioManager.Instance.SFXPlay(sfxType);
    }
}
