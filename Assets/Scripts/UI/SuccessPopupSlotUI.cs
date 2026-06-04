using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SuccessPopupSlotUI : MonoBehaviour
{
    [Header("슬롯 정보")]
    [SerializeField] private Image Image_Bg;
    [SerializeField] private Image Image_ObtainedContents;
    [SerializeField] private TMP_Text Text_StackCount;

    private void SetItemIcon(string itemDataId, int ItemStackCount)
    {
        var itemData = GameDataManager.Instance.GetItemData(itemDataId);
        if (itemData == null) return;

        var iconPath = itemData.IconPath;
        if (iconPath == null) return;

        GameUtil.LoadAndSetSpriteImage(Image_ObtainedContents, iconPath).Forget();
        Text_StackCount.text = $"{ItemStackCount}";
    }

    public void InitItemSlot(string itemDataId, int ItemStackCount)
    {
        SetItemIcon(itemDataId, ItemStackCount);
    }

    private void SetMonsterIcon(string monsterDataId, int monsterStackCount)
    {
        var monsterData = GameDataManager.Instance.GetMonsterData(monsterDataId);
        if(monsterData == null) return;

        var iconPath = monsterData.IconPath;
        if(iconPath == null) return;

        GameUtil.LoadAndSetSpriteImage(Image_ObtainedContents, iconPath).Forget();
        Text_StackCount.text = $"{monsterStackCount}";
    }

    public void InitMonsterSlot(string monsterDataId, int monsterStackCount)
    {
        SetMonsterIcon(monsterDataId, monsterStackCount);
    }
}
