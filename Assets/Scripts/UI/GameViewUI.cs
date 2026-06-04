using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameViewUI : UIBase
{
    [Header("플레이어 정보")]
    [SerializeField] private TMP_Text Text_PlayerName;
    [SerializeField] private TMP_Text Text_PlayerLevel;
    [SerializeField] private Slider Slider_Hp;
    [SerializeField] private TMP_Text Text_Hp;

    [Header("기본 공격")]
    [SerializeField] private GameUIButton Btn_BasicAttack;
    [SerializeField] private Image Image_Weapon;

    [Header("스킬")]
    [SerializeField] private GameUIButton Btn_UseSkill;
    [SerializeField] private Image Image_Skill;

    [Header("일시정지")]
    [SerializeField] private GameUIButton Btn_Pause;

    [Header("아이템")]
    [SerializeField] private GameObject Prefab_ItemSlot;
    [SerializeField] private Transform Transform_UIItemSlotRoot;

    [Header("몬스터")]
    [SerializeField] private GameObject Prefab_MonsterSlot;
    [SerializeField] private Transform Transform_UIMonsterSlotRoot;

    private Dictionary<long, GameViewItemSlotUI> _itemSlotList = new Dictionary<long, GameViewItemSlotUI>();
    private Dictionary<int, GameViewMonsterSlotUI> _monsterSlotList = new Dictionary<int, GameViewMonsterSlotUI>();

    private int _generatedMonsterKey;

    private void OnEnable()
    {
        RefreshEquippedSkill();
        RefreshEquippedWeapon();

        SetItemSlotOnEnable();
        SetMonsterSlotOnEnable();

        var player = GameObjectManager.Instance.GetLocalPlayer();
        if (player == null) return;

        TryBindSetChangedEvent(player.gameObject);

        Btn_Pause.BindOnClickButtonEvent(OnClick_PauseButton);
        Btn_BasicAttack.BindOnClickButtonEvent(OnClick_BasicAttackButton);
        Btn_UseSkill.BindOnClickButtonEvent(OnClick_UseSkill);
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

    private void OnClick_UseSkill()
    {

    }

    // 아이템 슬롯 제거 함수
    public void RemoveItemSlot(long removedItemUniqueId)
    {
        // 저장 정보에서 먼저! 아이템이 제거된 후에!!!
        // 그 다음에 슬롯을 제거해야 한다
        if(_itemSlotList.ContainsKey(removedItemUniqueId) == false)
        {
            Debug.LogError("이상합니다! 제거가 된 아이템의 슬롯을 찾을 수가 없네요!");
            return;
        }

        var slotComponent = _itemSlotList[removedItemUniqueId];
        _itemSlotList.Remove(removedItemUniqueId);
        Destroy(slotComponent.gameObject);
    }

    private void PlayerInfo()
    {
        // 캐릭터 원본 데이터 가져오기
        var playerData = GameDataManager.Instance.GetCharacterData("Character_Toto_01");
        if (playerData == null) return;

        Text_PlayerName.text = playerData.Name;

        var playerLevel = GameManager.Instance.GetPlayerLevel();
        Text_PlayerLevel.text = $"Lv.{playerLevel}";

        // 실제 플레이중인 캐릭터 객체 가져오기
        var player = GameObjectManager.Instance.GetLocalPlayer();
        if (player == null) return;

        Text_Hp.text = $"{player._currentHp} / {player._maxHp}";

    }

    public void RefreshEquippedSkill()
    {
        var equippedSkillId = GameManager.Instance.GetEquippedSkill();
        if( equippedSkillId == null ) return;

        var equippedSkill = GameDataManager.Instance.GetSkill(equippedSkillId);
        if( equippedSkill == null ) return;

        GameUtil.LoadAndSetSpriteImage(Image_Skill, equippedSkill.IconPath).Forget();

    }

    public void RefreshEquippedWeapon()
    {
        var equippedWeaponId = GameManager.Instance.GetEquippedWeapon();
        if(equippedWeaponId == null ) return;

        var equippedWeapon = GameDataManager.Instance.GetWeaponData(equippedWeaponId);
        if(equippedWeapon == null ) return;

        GameUtil.LoadAndSetSpriteImage(Image_Weapon, equippedWeapon.IconPath).Forget();
    }

    // 플레이어가 가지고 있는 아이템 불러와서 CreateItemSlot 함수 호출 
    private void SetItemSlotOnEnable()
    {
        ClearItemList();

        var ItemList = GameManager.Instance.GetPlayerItemList();
        if( ItemList == null ) return;

        foreach(var itemModel in ItemList)
        {
            var itemData = GameDataManager.Instance.GetItemData(itemModel.ItemDataId);
            if (itemData == null) return;

            if(itemData.UseItemType == "StatChangeHp")
            {
                CreateItemSlot(itemModel.ItemUniqueId, itemModel.ItemDataId, itemModel.ItemStackCount);
            }
                
        }
    }

    // 아이템 슬롯 생성
    private void CreateItemSlot(long UniqueId, string itemDataId, int stackCount)
    {
        var gObj = Instantiate(Prefab_ItemSlot, Transform_UIItemSlotRoot);
        if( gObj == null ) return;

        var slotComponent = gObj.GetComponent<GameViewItemSlotUI>();
        if( slotComponent == null ) return;

        slotComponent.InitSlot(UniqueId, itemDataId, stackCount);
        _itemSlotList.Add(slotComponent.SlotItemUniqueId, slotComponent);

        
    }

    private void SetMonsterSlotOnEnable()
    {
        ClearMonsterList();

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

        _generatedMonsterKey++;

        slotComponent.InitSlot(_generatedMonsterKey, monsterDataId);
        _monsterSlotList.Add(slotComponent.SlotInstanceId, slotComponent);
    }

    private void ClearMonsterList()
    {
        if(_monsterSlotList.Count > 0)
        {
            foreach(var monsterKv in _monsterSlotList)
            {
                var monster = monsterKv.Value;
                DestroyImmediate(monster.gameObject);
            }

            _monsterSlotList.Clear();
        }

        _generatedMonsterKey = 0;
    }

    private void ClearItemList()
    {
        if(_itemSlotList.Count > 0)
        {
            foreach(var itemKv in _itemSlotList)
            {
                var item = itemKv.Value;
                DestroyImmediate(item.gameObject);
            }

            _itemSlotList.Clear();
        }
    }

    // 이 오브젝트가 플레이어라면 이벤트를 구독하자
    private void TryBindSetChangedEvent(GameObject gObj)
    {
        var player = gObj.GetComponent<PlayerMovement>();
        if(player != null)
        {
            player.BindOnStatChangedEvent(OnTargetEntityHpChanged, OnTargetEntityMpChanged);
        }
    }

    private void OnTargetEntityHpChanged(int curHp, int maxHp)
    {
        Slider_Hp.value = (curHp / (float)maxHp);

        var player = GameObjectManager.Instance.GetLocalPlayer();
        if(player != null)
        {
            Text_Hp.text = $"{curHp} / {maxHp}";
        }

    }

    private void OnTargetEntityMpChanged(int curMp, int maxMp)
    {
        Slider_Hp.value = (curMp / (float)maxMp);
    }

}
