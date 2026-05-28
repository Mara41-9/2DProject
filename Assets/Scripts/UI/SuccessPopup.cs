using UnityEngine;
using UnityEngine.SceneManagement;

public class SuccessPopup : UIBase
{
    [SerializeField] private GameUIButton Btn_GoMainUI;

    public void OnEnable()
    {
        Btn_GoMainUI.BindOnClickButtonEvent(OnClick_GoMainUIBtn);
    }

    private void OnClick_GoMainUIBtn()
    {
        GameManager.Instance.RefreshGame();
        Debug.LogWarning("로비메인UI로 이동합니다.");
        UIManager.Instance.OpenLoadingUI();
        UIManager.Instance.CloseSuccessPopup();
        UIManager.Instance.OpenLobbyUI();
    }
}
