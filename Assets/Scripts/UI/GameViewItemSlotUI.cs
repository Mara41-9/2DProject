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
    [SerializeField] private GameObject Gobj_Selected;
    [SerializeField] private GameUIButton Btn_Slot;

    public int SlotInstanceId { get; private set; }
    public bool IsUseableItem { get; private set; }

    private event Action<int> _onSlotSelected;

    private void OnEnable()
    {
        Btn_Slot.BindOnClickButtonEvent(InvokeOnClickSelectSlot);
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

    public void InitSlot(int slotInstanceId, string itemDataId, int stackCount)
    {
        SlotInstanceId = slotInstanceId;
        SetIcon(itemDataId, stackCount);
    }

    // 등록된 이벤트 함수들 실행
    public void InvokeOnClickSelectSlot()
    {
        _onSlotSelected?.Invoke(SlotInstanceId);
    }

    // 이벤트 등록
    public void BindSlotSelectEvent(Action<int> onSelectEvent)
    {
        _onSlotSelected += onSelectEvent;
    }

    // 선택 표시 구현하는 함수
    public void SetSelectedUI(bool isSelect)
    {
        Gobj_Selected.SetActive(isSelect);
    }
}
