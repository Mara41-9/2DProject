using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GameViewMonsterSlotUI : MonoBehaviour
{
    [SerializeField] private Image Image_Frame;
    [SerializeField] private Image Image_Monster;
    [SerializeField] private Image Image_Grade;

    public int SlotInstanceId { get; private set; }

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
        SetIcon(monsterDataId);
    }
}
