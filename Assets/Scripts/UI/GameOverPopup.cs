using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverPopup : UIBase
{
    [SerializeField] private GameUIButton Btn_GoMainUI;

    public void OnEnable()
    {
        Btn_GoMainUI.BindOnClickButtonEvent(OnClick_GoMainUIBtn);
    }

    private void OnClick_GoMainUIBtn()
    {
        Debug.LogWarning("로비메인UI로 이동합니다.");
        UIManager.Instance.OpenLoadingUI();
        UIManager.Instance.CloseGameOverPopup();
        UIManager.Instance.OpenLobbyMainUI();
    }
}
