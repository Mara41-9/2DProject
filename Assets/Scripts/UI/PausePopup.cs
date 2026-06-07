using UnityEngine;

public class PausePopup : UIBase
{
    [SerializeField] private GameUIButton Btn_ContinueGame;
    [SerializeField] private GameUIButton Btn_GoToLobbyMain;

    private void OnEnable()
    {
        Btn_GoToLobbyMain.BindOnClickButtonEvent(OnClick_GoToLobbyMain);
        Btn_ContinueGame.BindOnClickButtonEvent(OnClick_ContinueGame);
    }

    private void OnClick_ContinueGame()
    {
        SoundManager.Instance.PlaySFX("Sound/SFX_ButtonClick");
        UIManager.Instance.ClosePausePopup();
    }

    private void OnClick_GoToLobbyMain()
    {
        SoundManager.Instance.PlaySFX("Sound/SFX_ButtonClick");
        GameManager.Instance.RefreshGame();
        UIManager.Instance.OpenLoadingUI();
        UIManager.Instance.CloseGameViewUI();
        UIManager.Instance.ClosePausePopup();
        UIManager.Instance.OpenLobbyUI();
    }
}
