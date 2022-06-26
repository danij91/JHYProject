using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageItem : MonoBehaviour {
    [SerializeField] private GameObject GameObj_selected;
    [SerializeField] private TMP_Text txt_language;
    [SerializeField] private Button btn_language;

    public void SetSelected() {
        GameObj_selected.gameObject.SetActive(true);
    }

    public void SetDeselected() {
        GameObj_selected.gameObject.SetActive(false);
    }

    public void SetOnClicked(Action onClicked = null) {
        btn_language.onClick.AddListener(() => onClicked?.Invoke());
    }

    public void SetLanguageText(string language) {
        txt_language.text = language;
    }
}
