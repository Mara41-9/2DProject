using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameViewUI : UIBase
{
    [Header("플레이어 정보")]
    [SerializeField] private TMP_Text Text_PlayerName;
    [SerializeField] private TMP_Text Text_PlayerLevel;

    [Header("스킬")]
    [SerializeField] private Image Img_Skill;

    private void OnEnable()
    {
        RefreshEquippedSkill();
    }

    private void Start()
    {
        PlayerInfo();
    }

    private void PlayerInfo()
    {
        var player = GameDataManager.Instance.GetCharacterData("character_selly_01");
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

        GameUtil.LoadAndSetSpriteImage(Img_Skill, equippedSkill.IconPath).Forget();
    }
}
