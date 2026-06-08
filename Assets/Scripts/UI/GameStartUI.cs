using UnityEngine;

public class GameStartUI : UIBase
{
    [SerializeField] private GameUIButton Btn_GameStart;
    [SerializeField] private GameUIButton Btn_ExitGame;

    private void OnEnable()
    {
        Btn_GameStart.BindOnClickButtonEvent(OnClick_GameStartButton);
        Btn_ExitGame.BindOnClickButtonEvent(OnClick_ExitGameButton);
    }

    private void OnClick_GameStartButton()
    {
        SoundManager.Instance.PlaySFX("Sound/SFX_GameStart");
        UIManager.Instance.OpenLoadingUI();
        UIManager.Instance.OpenLobbyUI();
        Debug.LogWarning("로비 창이 열렸습니다.");
    }

    private void OnClick_ExitGameButton()
    {
        SoundManager.Instance.PlaySFX("Sound/SFX_GameStart");
        UIManager.Instance.OpenGameExitPopup();
    }
}
