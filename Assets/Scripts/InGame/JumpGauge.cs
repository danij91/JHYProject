using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JumpGauge : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image img_gauge;

    private Coroutine coroutine = null;

    public void SetJumpGauge(float value)
    {
        if(value <= 0f)
        {
            if(coroutine == null)
            {
                coroutine = StartCoroutine(Co_GaugeMove());
            }
        }
        else
        {
            slider.value = value;
        }

        Color color;
        if(value >= 0.9f)
        {
            color = Color.red;
        }
        else
        {
            color = Color.cyan;
        }
        img_gauge.color = color;
    }

    private IEnumerator Co_GaugeMove()
    {
        while(slider.value > 0f)
        {
            slider.value -= Time.deltaTime;

            yield return null;
        }

        coroutine = null;
    }

    private void Update()
    {
        transform.LookAt(GameManager.Instance.GameCamera.transform);
    }

}
