using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ToggleSwitchButton : MonoBehaviour
{
    [SerializeField] private Button btn;
    [SerializeField] private Image img_on;

    private Func<bool> getter;
    private Action<bool> setter;


    public void Bind(Func<bool> getter, Action<bool> setter, UnityAction onClick = null)
    {
        this.getter = getter;
        this.setter = setter;

        UpdateToggleImage(getter());

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnToggleClicked);

        if (onClick != null)
        {
            btn.onClick.AddListener(onClick);
        }
    }


    private void UpdateToggleImage(bool isOn)
    {
        img_on.gameObject.SetActive(isOn);
    }

    private void OnToggleClicked()
    {
        if (getter != null && setter != null)
        {
            setter(!getter());
            UpdateToggleImage(getter());
        }
    }
}