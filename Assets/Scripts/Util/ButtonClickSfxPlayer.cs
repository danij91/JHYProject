using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
[CanEditMultipleObjects]
#endif
public class ButtonClickSfxPlayer : MonoBehaviour, IPointerDownHandler {
    [SerializeField] private SFXType sfxType = SFXType.BtnClick;

    public void OnPointerDown(PointerEventData eventData) {
        AudioManager.Instance.SFXPlay(sfxType);
    }
}
