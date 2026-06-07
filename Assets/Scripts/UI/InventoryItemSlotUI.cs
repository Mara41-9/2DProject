using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemSlotUI : MonoBehaviour
{
    [Header("슬롯 기본 정보")]
    [SerializeField] private TMP_Text Text_StackCount;
    [SerializeField] private Image Img_Icon;
    [SerializeField] private Image Img_Grade;
    [SerializeField] private Image Img_Frame;
    [SerializeField] private GameObject Gobj_Selected;   // 이미지가 아니라 게임오브젝트 -> 활성/비활성화 기능으로만 사용할거라서
    [SerializeField] private GameUIButton Btn_Slot;
    [SerializeField] private Image Img_Lock;

    // 슬롯 클릭 시 외부에 알려주는 이벤트 (int 값 전달)
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
    private void SetIcon(string DataId, int Count)
    {
        // GameDataManager에서 해당 아이템 정보 받아옴
        var itemData = GameDataManager.Instance.GetItemData(DataId);
        if (itemData != null)
        {
            // 아이템의 아이콘 경로 받아옴
            string itemIconPath = itemData.IconPath;
            if (itemIconPath != null)
            {
                // iconPath에 있는 Sprite 로드하고 로드 완료되면 Img_Icon 이미지에 넣자
                //ResourceManager.Instance.LoadSprite(iconPath, (sprite) =>
                //{
                //    Img_Icon.sprite = sprite;
                //});


                // 어드레서블 적용 -> 비동기로 바뀜
                GameUtil.LoadAndSetSpriteImage(Img_Icon, itemIconPath).Forget();
                Text_StackCount.text = $"{Count}";

                var itemGradeDataId = itemData.GradeDataId;
                if (string.IsNullOrEmpty(itemGradeDataId)) return;

                var GradeData = GameDataManager.Instance.GetGradeData(itemGradeDataId);
                if(GradeData != null)
                {
                    GameUtil.LoadAndSetSpriteImage(Img_Grade, GradeData.IconPath).Forget();
                }


                //// 실제 이미지 파일 불러옴
                //var sprite = GameUtil.LoadSpriteCanBeNull(iconPath);
                //if (sprite == null)
                //{
                //    Debug.LogWarning($"Sprite를 불러올 수 없습니다! 경로:{iconPath}");
                //    return;
                //}

                //Img_Icon.sprite = sprite;

                return;

            }

            Debug.LogWarning($"Item 데이터에 아이콘 경로가 존재하지 않습니다.");

        }
    }

    // 이 오브젝트가 비활성될 때
    private void OnDisable()
    {
        // 등록돼있던 이벤트들 전부 제거
        OnSelectEvent = null;
    }


    // 슬롯이 생성된 후, 슬롯의 기본 정보(고유 번호)를 세팅하는 초기화 함수
    public void InitSlot(long slotUniqueId, string dataId, int StackCount, EInventoryCategory curCategory)
    {
        SlotUniqueId = slotUniqueId;
        SlotDataId = dataId;
        SetIcon(dataId, StackCount);
    }

    public void OnClick_SelectSlot()
    {
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
