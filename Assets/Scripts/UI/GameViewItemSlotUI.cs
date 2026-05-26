using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameViewItemSlotUI : MonoBehaviour
{
    [Header("슬롯 기본 정보")]
    [SerializeField] private Image Image_Icon;
    [SerializeField] private Image Image_Frame;
    [SerializeField] private Text Text_StackCount;

    public int SlotInstanceId { get; private set; }

    private void SetIcon(string itemDataId, int stackCount)
    {
        var itemData = GameDataManager.Instance.GetItemData(itemDataId);
        if (itemData == null) return;

        var itemDataIconPath = itemData.IconPath;
        if(itemDataIconPath == null) return;

        GameUtil.LoadAndSetSpriteImage(Image_Icon, itemDataIconPath).Forget();
        Text_StackCount.text = $"{stackCount}";
    }

    public void InitSlot(int slotInstanceId, string itemDataId, int stackCount)
    {
        SlotInstanceId = slotInstanceId;
        SetIcon(itemDataId, stackCount);
    }
}
