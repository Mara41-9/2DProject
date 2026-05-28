using System;
using UnityEngine;
using UnityEngine.UI;

public class GameUIButton : MonoBehaviour
{
    [SerializeField] private Button Button_Base;
    [SerializeField] private Image Image_Base;
    [SerializeField] private Image Image_Text;

    // 자동으로 이벤트를 제거할지 말지 구분하는 변수
    // false -> 자동으로 이벤트 제거
    // true -> 직접 이벤트 제거
    private bool _isSlotMenualUnbindEvent;

    private void Awake()
    {
        InitUIButton();
    }

    private void OnEnable()
    {
    }

    // UI가 꺼질 때 버튼에 등록된 클릭 이벤트 전부 제거
    private void OnDisable()
    {
        if(_isSlotMenualUnbindEvent == false)
        {
            Button_Base.onClick.RemoveAllListeners();
        }
    }

    private void InitUIButton()
    {
        if (Button_Base != null)
        {
            return;
        }

        var button = this.gameObject.GetComponentInChildren<Button>();
        if (button != null)
        {
            this.Button_Base = button;
        }
    }

    public void BindOnClickButtonEvent(Action onClickCallback, bool isMenualUnbindEvent = false)
    {
        if (Button_Base == null) return;

        Button_Base.onClick.AddListener(new UnityEngine.Events.UnityAction(onClickCallback));
        _isSlotMenualUnbindEvent = isMenualUnbindEvent;
    }

    public void UnBindOnClickButtonEvent(Action onClickCallback)
    {
        if (Button_Base == null) return;

        Button_Base.onClick.RemoveListener(new UnityEngine.Events.UnityAction(onClickCallback));
    }

}
