using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMainUI : UIBase
{
    // Layout_Top - 맨 윗 부분
    [Header("Layout_Top")]
    [SerializeField] private GameUIButton Btn_Profile;

    // Layout_Top - 왼쪽 부분
    [Header("Layout_Top")]
    [SerializeField] private GameUIButton Btn_Start;

    // Layout_Right - 오른쪽 부분
    [Header("Layout_Right")]
    [SerializeField] private GameUIButton Btn_Skill;
    [SerializeField] private GameUIButton Btn_Inventory;

    private void OnEnable()
    {
        Btn_Profile.BindOnClickButtonEvent(OnClick_OpenProfile);
        Btn_Start.BindOnClickButtonEvent(OnClick_OpenGameView);
        Btn_Skill.BindOnClickButtonEvent(OnClick_OpenSKillPopup);
        Btn_Inventory.BindOnClickButtonEvent(OnClick_OpenInventory);
    }

    public void OnClick_OpenProfile()
    {
        UIManager.Instance.OpenPropilePopup();
        Debug.LogWarning("프로필 창이 열렸습니다.");
    }

    public void OnClick_OpenGameView()
    {
        UIManager.Instance.CloseGameStartUI();
        UIManager.Instance.CloseLobbyMainUI();
        UIManager.Instance.OpenLoadingUI();
        Debug.LogWarning("게임 화면으로 이동합니다.");
    }

    public void OnClick_OpenSKillPopup()
    {
        UIManager.Instance.OpenSkillPopup();
        Debug.LogWarning("스킬 창이 열렸습니다.");
    }

    public void OnClick_OpenInventory()
    {
        UIManager.Instance.OpenInventoryPopup();
        Debug.LogWarning("인벤토리 창이 열렸습니다.");
    }
   
}
