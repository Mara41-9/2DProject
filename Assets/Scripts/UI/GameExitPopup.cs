using UnityEngine;

public class GameExitPopup : UIBase
{
    [SerializeField] private GameUIButton Btn_Cancel;
    [SerializeField] private GameUIButton Btn_Exit;

    private void OnEnable()
    {
        Btn_Cancel.BindOnClickButtonEvent(OnClick_Cancel);
        Btn_Exit.BindOnClickButtonEvent(OnClick_Exit);
    }

    private void OnClick_Cancel()
    {
        SoundManager.Instance.PlaySFX("Sound/SFX_GameStart");
        UIManager.Instance.CloseGameExitPopup();
    }

    private void OnClick_Exit()
    {
        SoundManager.Instance.PlaySFX("Sound/SFX_GameStart");
        Application.Quit();
    }
}
