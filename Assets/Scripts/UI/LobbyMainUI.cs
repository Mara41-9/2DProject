using Unity.VisualScripting;
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
        SoundManager.Instance.PlaySFX("Sound/SFX_ButtonClick");
        UIManager.Instance.OpenPropilePopup();
        Debug.LogWarning("프로필 창이 열렸습니다.");
    }

    public void OnClick_OpenGameView()
    {
        var equippedWeapon = GameManager.Instance.GetEquippedWeapon();
        if(string.IsNullOrEmpty(equippedWeapon) == false)
        {
            UIManager.Instance.OpenLoadingUI();
            UIManager.Instance.CloseGameStartUI();
            UIManager.Instance.CloseLobbyMainUI();
            UIManager.Instance.OpenMainUI(UIType.HudUI);
            UIManager.Instance.OpenGameViewUI();
            SoundManager.Instance.PlayBGM("Sound/GameIn_BGM");
            
            Debug.LogWarning("게임 화면으로 이동합니다.");
        }
        else
        {
            UIManager.Instance.OpenCommonToastUI();
            var commonToastUI = UIManager.Instance.GetOpenedUI(UIRootType.ToastUI, UIType.CommonToastUI);
            if (commonToastUI == null) return;

            var component = commonToastUI.GetComponent<CommonToastUI>();
            if (component == null) return;

            component.SetPosition(new Vector2(0f, 349f));
            component.SetMessage("전투를 시작하려면 먼저 무기를 장착해주세요!");
        }
            
    }

    public void OnClick_OpenSKillPopup()
    {
        SoundManager.Instance.PlaySFX("Sound/SFX_ButtonClick");
        UIManager.Instance.OpenSkillPopup();
        Debug.LogWarning("스킬 창이 열렸습니다.");
    }

    public void OnClick_OpenInventory()
    {
        SoundManager.Instance.PlaySFX("Sound/SFX_ButtonClick");
        UIManager.Instance.OpenInventoryPopup();
        Debug.LogWarning("인벤토리 창이 열렸습니다.");
    }
   
}
