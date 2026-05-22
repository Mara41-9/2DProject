using UnityEngine;

public class GameStartUI : UIBase
{
    [SerializeField] private GameUIButton Btn_GameStart;
    [SerializeField] private GameUIButton Btn_Tutorial;

    private void OnEnable()
    {
        Btn_GameStart.BindOnClickButtonEvent(OnClick_GameStartButton);
    }

    private void OnClick_GameStartButton()
    {
        UIManager.Instance.OpenLoadingUI();
        UIManager.Instance.OpenLobbyMainUI();
        Debug.LogWarning("로비 창이 열렸습니다.");
    }
}
