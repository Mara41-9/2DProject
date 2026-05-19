using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Text Text_StackCount;
    [SerializeField] private GameUIButton Btn_Slot;
    [SerializeField] private Image Img_Icon;
    [SerializeField] private Image Img_Frame;

    // 슬롯 클릭 시 외부에 알려주는 이벤트 (int 값 전달)
    private event Action<int> OnSelectEvent;

    public int SlotInstanceId { get; private set; }
    public string SlotDataId { get; private set; }

    private void OnEnable()
    {
        Btn_Slot.BindOnClickButtonEvent(OnClick_SelectSlot);
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
        

        var weaponData = GameDataManager.Instance.GetWeaponData(DataId);
        if (weaponData != null)
        {
            string weaponiconPath = weaponData.IconPath;
            if (weaponiconPath != null)
            {
                GameUtil.LoadAndSetSpriteImage(Img_Icon, weaponiconPath).Forget();
                Text_StackCount.text = $"{Count}";

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
    public void InitSlot(int slotInstanceId, string dataId, int StackCount)
    {
        SlotInstanceId = slotInstanceId;
        SlotDataId = dataId;
        SetIcon(dataId, StackCount);
    }

    public void OnClick_SelectSlot()
    {
        OnSelectEvent?.Invoke(SlotInstanceId);
        Debug.Log($"{SlotInstanceId}눌러졌다");
    }

    public void BindSlotSelectEvent(Action<int> onSelectEvent)
    {
        OnSelectEvent = onSelectEvent;
    }

    
}
