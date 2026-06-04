using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SuccessPopupSlotUI : MonoBehaviour
{
    [Header("슬롯 정보")]
    [SerializeField] private Image Image_Bg;
    [SerializeField] private Image Image_ObtainedContents;
    [SerializeField] private Text Text_StackCount;

    private void SetIcon(string dataId, int stackCount)
    {
        var itemData = GameDataManager.Instance.GetItemData(dataId);
        if (itemData == null) return;

        var iconPath = itemData.IconPath;
        if (iconPath == null) return;

        GameUtil.LoadAndSetSpriteImage(Image_ObtainedContents, iconPath).Forget();
        Text_StackCount.text = $"{stackCount}";
    }

    public void InitSlot(string dataId, int stackCount)
    {
        SetIcon(dataId, stackCount);
    }
}
