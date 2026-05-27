using Cysharp.Threading.Tasks;
using System.Collections.Generic;
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

    [Header("기본 공격")]
    [SerializeField] private GameUIButton Btn_BasicAttack;

    [Header("일시정지")]
    [SerializeField] private GameUIButton Btn_Pause;

    [Header("아이템")]
    [SerializeField] private GameObject Prefab_ItemSlot;
    [SerializeField] private Transform Transform_UIItemSlotRoot;

    [Header("몬스터")]
    [SerializeField] private GameObject Prefab_MonsterSlot;
    [SerializeField] private Transform Transform_UIMonsterSlotRoot;

    private Dictionary<int, GameViewItemSlotUI> _itemSlotList = new Dictionary<int, GameViewItemSlotUI>();
    private Dictionary<int, GameViewMonsterSlotUI> _monsterSlotList = new Dictionary<int, GameViewMonsterSlotUI>();

    private int _generatedKey;

    private void OnEnable()
    {
        RefreshEquippedSkill();
        RefreshEquippedWeapon();

        SetItemSlotOnEnable();
        SetMonsterSlotOnEnable();

        Btn_Pause.BindOnClickButtonEvent(OnClick_PauseButton);
        Btn_BasicAttack.BindOnClickButtonEvent(OnClick_BasicAttackButton);
    }

    private void Start()
    {
        PlayerInfo();
    }

    private void OnClick_PauseButton()
    {
        UIManager.Instance.OpenPausePopup();
    }

    private void OnClick_BasicAttackButton()
    {
        var player = GameObjectManager.Instance.GetLocalPlayer();
        if (player == null) return;

        player.Attack();
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

    private void SetItemSlotOnEnable()
    {
        var ItemList = GameManager.Instance.GetPlayerItemList();
        if( ItemList == null ) return;

        foreach(var itemModel in ItemList)
        {
            CreateItemSlot(itemModel.ItemDataId, itemModel.ItemStackCount);
        }
    }

    private void CreateItemSlot(string itemDataId, int stackCount)
    {
        var gObj = Instantiate(Prefab_ItemSlot, Transform_UIItemSlotRoot);
        if( gObj == null ) return;

        var slotComponent = gObj.GetComponent<GameViewItemSlotUI>();
        if( slotComponent == null ) return;

        _generatedKey++;

        slotComponent.InitSlot(_generatedKey, itemDataId, stackCount);
        _itemSlotList.Add(slotComponent.SlotInstanceId, slotComponent);
    }

    private void SetMonsterSlotOnEnable()
    {
        var monsterList = GameDataManager.Instance.MonsterDataList;
        if( monsterList == null ) return;

        foreach(var monster in monsterList)
        {
            CreateMonsterSlot(monster.Key);
        }
    }

    private void CreateMonsterSlot(string monsterDataId)
    {
        var gObj = Instantiate(Prefab_MonsterSlot, Transform_UIMonsterSlotRoot);
        if( gObj == null ) return;

        var slotComponent = gObj.GetComponent<GameViewMonsterSlotUI>();
        if( slotComponent == null ) return;

        _generatedKey++;

        slotComponent.InitSlot(_generatedKey, monsterDataId);
        _monsterSlotList.Add(slotComponent.SlotInstanceId, slotComponent);
    }

}
