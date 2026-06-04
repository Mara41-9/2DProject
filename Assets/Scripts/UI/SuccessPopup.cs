using UnityEngine;
using UnityEngine.SceneManagement;

public class SuccessPopup : UIBase
{
    [Header("돌아가기 버튼")]
    [SerializeField] private GameUIButton Btn_GoMainUI;

    [Header("동적 생성할 슬롯")]
    [SerializeField] private GameObject Prefab_Slot;

    [Header("생성되는 위치")]
    [SerializeField] private Transform Transform_UISlotRoot;

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

    private void SetIcon()
    {

    }
}
