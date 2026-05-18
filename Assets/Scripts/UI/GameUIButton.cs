using System;
using UnityEngine;
using UnityEngine.UI;

public class GameUIButton : MonoBehaviour
{
    [SerializeField] private Button Button_Base;
    [SerializeField] private Image Image_Base;
    [SerializeField] private Image Image_Text;

    private void Awake()
    {
        InitUIButton();
    }

    private void OnEnable()
    {
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

    public void BindOnClickButtonEvent(Action onClickCallback)
    {
        if (Button_Base == null) return;

        Button_Base.onClick.AddListener(new UnityEngine.Events.UnityAction(onClickCallback));

    }

    public void UnBindOnClickButtonEvent(Action onClickCallback)
    {
        if (Button_Base == null) return;

        Button_Base.onClick.RemoveListener(new UnityEngine.Events.UnityAction(onClickCallback));
    }

}
