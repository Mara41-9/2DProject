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
        Debug.LogWarning("UI씬 메인UI로 이동합니다.");
        SceneManager.LoadScene("UI_Basic");
    }
}
