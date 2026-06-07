using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryWeaponSlotUI : MonoBehaviour
{
    [Header("슬롯 기본 정보")]
    [SerializeField] private TMP_Text Text_Attack;
    [SerializeField] private Image Img_Icon;
    [SerializeField] private Image Img_Grade;
    [SerializeField] private Image Img_Frame;
    [SerializeField] private GameObject Gobj_Selected;   // 이미지가 아니라 게임오브젝트 -> 활성/비활성화 기능으로만 사용할거라서
    [SerializeField] private GameUIButton Btn_Slot;
    [SerializeField] private Image Img_Lock;

    private event Action<long> OnSelectEvent;

    public long SlotUniqueId { get; private set; }
    public string SlotDataId { get; private set; }

    private void OnEnable()
    {
        Btn_Slot.BindOnClickButtonEvent(OnClick_SelectSlot);
    }

    public string GetSlotDataId()
    {
        return SlotDataId;
    }

    // 아이템/무기 ID 받아서 해당 그 아이템/무기의 아이콘 스프라이트를 찾아 슬롯 이미지에 넣어주는 함수
    private void SetIcon(string DataId, int Attack)
    {
        var weaponData = GameDataManager.Instance.GetWeaponData(DataId);
        if (weaponData != null)
        {
            string weaponiconPath = weaponData.IconPath;
            if (weaponiconPath != null)
            {
                GameUtil.LoadAndSetSpriteImage(Img_Icon, weaponiconPath).Forget();
                Text_Attack.text = $"{Attack}";

                var weaponGradeDataId = weaponData.GradeDataId;
                if (string.IsNullOrEmpty(weaponGradeDataId)) return;

                var gradeData = GameDataManager.Instance.GetGradeData(weaponGradeDataId);
                if (gradeData == null) return;

                GameUtil.LoadAndSetSpriteImage(Img_Grade, gradeData.IconPath).Forget();

                return;
            }

            Debug.LogWarning($"Weapon 데이터에 아이콘 경로가 존재하지 않습니다.");
        }

        Debug.LogWarning($"Item/Weapon 데이터를 모두 찾을 수 없습니다! 경로:{DataId}");

    }

    // 이 오브젝트가 비활성될 때
    private void OnDisable()
    {
        // 등록돼있던 이벤트들 전부 제거
        OnSelectEvent = null;
    }


    // 슬롯이 생성된 후, 슬롯의 기본 정보(고유 번호)를 세팅하는 초기화 함수
    public void InitSlot(long slotUniqueId, string dataId, int Attack, EInventoryCategory curCategory)
    {
        SlotUniqueId = slotUniqueId;
        SlotDataId = dataId;
        SetIcon(dataId, Attack);
    }

    public void OnClick_SelectSlot()
    {
        SoundManager.Instance.PlaySFX("Sound/SFX_ButtonClick");

        OnSelectEvent?.Invoke(SlotUniqueId);
    }

    public void BindSlotSelectEvent(Action<long> onSelectEvent)
    {
        OnSelectEvent = onSelectEvent;   // 이벤트 등록
    }

    public void SetSelectedUI(bool isSelect)
    {
        Gobj_Selected.SetActive(isSelect);
    }

    public void SetLockUI(bool isLock)
    {
        Img_Lock.gameObject.SetActive(isLock);
    }
}
