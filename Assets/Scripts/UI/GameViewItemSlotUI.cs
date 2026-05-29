using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameViewItemSlotUI : MonoBehaviour
{
    [Header("슬롯 기본 정보")]
    [SerializeField] private Image Image_Icon;
    [SerializeField] private Image Image_Frame;
    [SerializeField] private Text Text_StackCount;
    [SerializeField] private GameUIButton Btn_Slot;
    [SerializeField] private GameObject Gobj_SelectedUI;
    [SerializeField] private GameUIButton Btn_UseItem;

    public long SlotItemUniqueId { get; private set; }
    public bool IsUseableItem { get; private set; }

    private void OnEnable()
    {
        ActiveUseSelectItemObject(false);
        Btn_Slot.BindOnClickButtonEvent(OnClick_SelectSlot);
        Btn_UseItem.BindOnClickButtonEvent(OnClick_UseItem, true);
    }

    private void SetIcon(string itemDataId, int stackCount)
    {
        var itemData = GameDataManager.Instance.GetItemData(itemDataId);
        if (itemData == null) return;

        var itemDataIconPath = itemData.IconPath;
        if(itemDataIconPath == null) return;

        GameUtil.LoadAndSetSpriteImage(Image_Icon, itemDataIconPath).Forget();
        Text_StackCount.text = $"{stackCount}";

        // 아이템의 UseItemType이 Null이 아니라면 사용 가능한 아이템으로 처리
        IsUseableItem = (string.IsNullOrEmpty(itemData.UseItemType) == false);
    }

    public void InitSlot(long slotuniqueId, string itemDataId, int stackCount)
    {
        SlotItemUniqueId = slotuniqueId;
        SetIcon(itemDataId, stackCount);
    }

    private void OnClick_SelectSlot()
    {
        ActiveUseSelectItemObject(true);
    }

    private void OnClick_UseItem()
    {
        RequestSelectUseItem();
    }

    // 아이템 사용 요청 + 전체 처리 흐름 담당
    private void RequestSelectUseItem()
    {
        // 실제 저장 데이터에서 아이템 사용했다면 true, 사용 못했다면 false 반환
        bool isItemUsed = GameManager.Instance.RequestUseItem(SlotItemUniqueId);

        bool isExist = false;

        // 만약 사용했다면
        if (isItemUsed == true)
        {
            var itemList = GameManager.Instance.GetPlayerItemList();
            foreach (var itemModel in itemList)
            {
                if (itemModel.ItemUniqueId == SlotItemUniqueId)
                {
                    isExist = true;
                    RefreshItemStackCount(itemModel.ItemStackCount);
                    ActiveUseSelectItemObject(false);

                    break;
                }
            }

            if (isExist == false)
            {
                var component = this.GetComponentInParent<GameViewUI>();

                // 아이템 슬롯 삭제 함수 요청
                component.RemoveItemSlot(SlotItemUniqueId);

                // 현재 선택된 아이템 UniqueId는 0으로 초기화
                SlotItemUniqueId = 0;
            }

        }

    }

    public void RefreshItemStackCount(int stackCount)
    {
        Text_StackCount.text = $"{stackCount}";
    }

    // 아이템 사용 오브젝트를 보이게 할지 숨길지 관리하는 함수
    private void ActiveUseSelectItemObject(bool isActive)
    {
        Gobj_SelectedUI.gameObject.SetActive(isActive);
    }
}
