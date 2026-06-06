using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameViewMonsterSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image Image_Frame;
    [SerializeField] private Image Image_Monster;
    [SerializeField] private Image Image_Grade;

    public int SlotInstanceId { get; private set; }
    public string SlotDataId { get; private set; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.Instance.OpenMonsterInfoPopup();

        var openedPopupUI = UIManager.Instance.GetOpenedUI(UIRootType.PopupUI, UIType.MonsterInfoPopup);
        if (openedPopupUI == null) return;

        var component = openedPopupUI.GetComponent<MonsterInfoPopup>();
        if (component == null) return;

        component.SetMonsterInfo(SlotDataId);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.CloseMonsterInfoPopup();
    }

    private void SetIcon(string monsterDataId)
    {
        var MonsterData = GameDataManager.Instance.GetMonsterData(monsterDataId);
        if (MonsterData == null) return;

        var MonsterIconPath = MonsterData.IconPath;
        if (MonsterIconPath == null) return;

        GameUtil.LoadAndSetSpriteImage(Image_Monster, MonsterIconPath).Forget();
    }

    public void InitSlot(int slotInstanceId, string monsterDataId)
    {
        SlotInstanceId = slotInstanceId;
        SlotDataId = monsterDataId;
        SetIcon(SlotDataId);
    }
}
