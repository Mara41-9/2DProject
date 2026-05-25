using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameViewUI : UIBase
{
    [Header("플레이어 정보")]
    [SerializeField] private TMP_Text Text_PlayerName;
    [SerializeField] private TMP_Text Text_PlayerLevel;
    [SerializeField] private Image Image_SelectedWeapon;
    [SerializeField] private Image Image_SelectedSkill;

    [Header("스킬")]
    [SerializeField] private Image Image_UseSkill;

    [Header("일시정지")]
    [SerializeField] private GameUIButton Btn_Pause;

    private void OnEnable()
    {
        RefreshEquippedSkill();
        RefreshEquippedWeapon();

        Btn_Pause.BindOnClickButtonEvent(OnClick_PauseButton);
    }

    private void Start()
    {
        PlayerInfo();
    }

    private void OnClick_PauseButton()
    {
        UIManager.Instance.OpenPausePopup();
    }

    private void PlayerInfo()
    {
        var player = GameDataManager.Instance.GetCharacterData("Character_Toto_01");
        if (player == null) return;

        Text_PlayerName.text = player.Name;

        var playerLevel = GameManager.Instance.GetPlayerLevel();
        Text_PlayerLevel.text = $"Lv.{playerLevel}";

    }

    public void RefreshEquippedSkill()
    {
        var equippedSkillId = GameManager.Instance.GetEquippedSkill();
        if( equippedSkillId == null ) return;

        var equippedSkill = GameDataManager.Instance.GetSkill(equippedSkillId);
        if( equippedSkill == null ) return;

        GameUtil.LoadAndSetSpriteImage(Image_UseSkill, equippedSkill.IconPath).Forget();
        GameUtil.LoadAndSetSpriteImage(Image_SelectedSkill, equippedSkill.IconPath).Forget();
    }

    public void RefreshEquippedWeapon()
    {
        var equippedWeaponId = GameManager.Instance.GetEquippedWeapon();
        if(equippedWeaponId == null ) return;

        var equippedWeapon = GameDataManager.Instance.GetWeaponData(equippedWeaponId);
        if(equippedWeapon == null ) return;

        GameUtil.LoadAndSetSpriteImage(Image_SelectedWeapon, equippedWeapon.IconPath).Forget();
    }
}
