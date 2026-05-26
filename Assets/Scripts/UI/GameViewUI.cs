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

    [Header("스킬")]
    [SerializeField] private Image Image_UseSkill;

    [Header("일시정지")]
    [SerializeField] private GameUIButton Btn_Pause;

    [Header("아이템")]
    [SerializeField] private GameObject Prefab_ItemSlot;
    [SerializeField] private Transform Transform_UISlotRoot;

    private Dictionary<int, GameViewItemSlotUI> _itemSlotList = new Dictionary<int, GameViewItemSlotUI>();

    private int _generatedKey;

    private void OnEnable()
    {
        RefreshEquippedSkill();
        RefreshEquippedWeapon();

        SetItemSlotOnEnable();

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

    private void SetItemSlotOnEnable()
    {
        var ItemList = GameManager.Instance.GetPlayerItemList();
        if( ItemList == null ) return;

        foreach(var itemModel in ItemList)
        {
            CreateSlot(itemModel.ItemDataId, itemModel.ItemStackCount);
        }
    }

    private void CreateSlot(string itemDataId, int stackCount)
    {
        var gObj = Instantiate(Prefab_ItemSlot, Transform_UISlotRoot);
        if( gObj == null ) return;

        var slotComponent = gObj.GetComponent<GameViewItemSlotUI>();
        if( slotComponent == null ) return;

        _generatedKey++;

        slotComponent.InitSlot(_generatedKey, itemDataId, stackCount);
        _itemSlotList.Add(slotComponent.SlotInstanceId, slotComponent);
    }
}
