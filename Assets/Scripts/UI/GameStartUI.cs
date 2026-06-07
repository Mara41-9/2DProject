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
        SoundManager.Instance.PlaySFX("Sound/SFX_GameStart");
        UIManager.Instance.OpenLoadingUI();
        UIManager.Instance.OpenLobbyUI();
        Debug.LogWarning("로비 창이 열렸습니다.");
    }
}
