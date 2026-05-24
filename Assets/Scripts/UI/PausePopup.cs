using UnityEngine;

public class PausePopup : UIBase
{
    [SerializeField] private GameUIButton Btn_ContinueGame;
    [SerializeField] private GameUIButton Btn_GoToLobbyMain;

    private void OnEnable()
    {
        Btn_GoToLobbyMain.BindOnClickButtonEvent(OnClick_GoToLobbyMain);
    }

    private void OnClick_ContinueGame()
    {
        
    }

    private void OnClick_GoToLobbyMain()
    {
        UIManager.Instance.OpenLoadingUI();
        UIManager.Instance.CloseGameViewUI();
        UIManager.Instance.ClosePausePopup();
        UIManager.Instance.OpenLobbyMainUI();
    }
}
