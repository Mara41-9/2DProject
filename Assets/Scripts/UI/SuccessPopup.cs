using System.Collections.Generic;
using UnityEngine;

public class SuccessPopup : UIBase
{
    [Header("돌아가기 버튼")]
    [SerializeField] private GameUIButton Btn_GoMainUI;

    [Header("동적 생성할 슬롯")]
    [SerializeField] private GameObject Prefab_Slot;

    [Header("생성되는 아이템 슬롯 위치")]
    [SerializeField] private Transform Transform_ItemSlotRoot;

    [Header("생성되는 몬스터 슬롯 위치")]
    [SerializeField] private Transform Transform_MonsterSlotRoot;

    private Dictionary<long, SuccessPopupSlotUI> _itemSlotList = new Dictionary<long, SuccessPopupSlotUI>();
    private Dictionary<long, SuccessPopupSlotUI> _monsterSlotList = new Dictionary<long, SuccessPopupSlotUI>();

    public void OnEnable()
    {
        Btn_GoMainUI.BindOnClickButtonEvent(OnClick_GoMainUIBtn);
        SetSuccessPopupItemSlotOnEnable();
        SetSuccessPopupMonsterSlotOnEnable();
    }

    public void OnDisable()
    {
        ClearItemSlotList();
        ClearMonsterSlotList();
    }

    private void OnClick_GoMainUIBtn()
    {
        GameManager.Instance.RefreshGame();
        Debug.LogWarning("로비메인UI로 이동합니다.");
        UIManager.Instance.OpenLoadingUI();
        UIManager.Instance.CloseSuccessPopup();
        UIManager.Instance.OpenLobbyUI();
    }

    private void SetSuccessPopupItemSlotOnEnable()
    {
        var obtainedItemList = GameManager.Instance.GetPlayerObtainedItemList();
        if (obtainedItemList == null) return;

        foreach(var obtainedItem in obtainedItemList)
        {
            CreateItemSlot(obtainedItem.ItemUniqueId, obtainedItem.ItemDataId, obtainedItem.ItemStackCount);
        }
    }

    private void CreateItemSlot(long itemUniqueId, string itemDataId, int itemStackCount)
    {
        var slot = Instantiate(Prefab_Slot, Transform_ItemSlotRoot);
        if (slot == null) return;

        var component = slot.GetComponent<SuccessPopupSlotUI>();
        if(component == null) return;

        component.InitItemSlot(itemDataId, itemStackCount);

        _itemSlotList.Add(itemUniqueId, component);
    }

    private void SetSuccessPopupMonsterSlotOnEnable()
    {
        var defeatedMonsterList = GameManager.Instance.GetDefeatedMonsterList();
        if(defeatedMonsterList == null) return;

        foreach(var defeatedMonster in defeatedMonsterList)
        {
            CreateMonsterSlot(defeatedMonster.MonsterUniqueId, defeatedMonster.MonsterDataId, defeatedMonster.MonsterStackCount);
        }
    }

    private void CreateMonsterSlot(long monsterUniqueId, string monsterDataId, int monsterStackCount)
    {
        var slot = Instantiate(Prefab_Slot, Transform_MonsterSlotRoot);
        if(slot == null) return;

        var component = slot.GetComponent<SuccessPopupSlotUI>();
        if(component == null) return;

        component.InitMonsterSlot(monsterDataId, monsterStackCount);

        _monsterSlotList.Add(monsterUniqueId, component);
    }

    private void ClearItemSlotList()
    {
        if(_itemSlotList.Count > 0)
        {
            foreach(var slotKv in _itemSlotList)
            {
                var slot = slotKv.Value;
                DestroyImmediate(slot.gameObject);
            }

            _itemSlotList.Clear();
        }
    }

    private void ClearMonsterSlotList()
    {
        if (_monsterSlotList.Count > 0)
        {
            foreach (var slotKv in _monsterSlotList)
            {
                var slot = slotKv.Value;
                DestroyImmediate(slot.gameObject);
            }

            _monsterSlotList.Clear();
        }
    }

}
