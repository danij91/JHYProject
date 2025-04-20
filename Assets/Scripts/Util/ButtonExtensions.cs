using UnityEngine.UI;
using UnityEngine.Events;

public static class ButtonExtensions
{
    /// <summary>
    /// 기존 리스너를 모두 제거한 뒤 새 리스너 하나만 등록합니다.
    /// 사용법: button.AddListener(() => { ... });
    /// </summary>
    public static void AddOnClickListener(this Button button, UnityAction action, bool reset = true)
    {
        if (reset)
        {
            button.onClick.RemoveAllListeners();
        }
        button.onClick.AddListener(action);
    }

    /// <summary>
    /// 버튼 자신을 인자로 받는 리스너도 지원합니다.
    /// 사용법: button.AddListener(self => { Debug.Log(self.name); });
    /// </summary>
    public static void AddOnClickListener(this Button button, UnityAction<Button> actionWithSelf, bool reset = true)
    {
        if (reset)
        {
            button.onClick.RemoveAllListeners();
        }
        button.onClick.AddListener(() => actionWithSelf(button));
    }
}