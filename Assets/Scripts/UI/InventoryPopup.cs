using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPopup : UIBase
{
    [Header("동적 생성할 프리팹")]
    [SerializeField] private GameObject Prefab_Slot;    // 생성할 슬롯 오브젝트

    [Header("슬롯 리스트 영역")]
    [SerializeField] private Transform Transform_UISlotRoot;   // 슬롯이 생성되는 곳

    [Header("팝업창 닫기 버튼")]
    [SerializeField] private GameUIButton Btn_ClosePopup;
    [SerializeField] private GameUIButton Btn_BackClose;

    [Header("선택된 슬롯 정보")]
    [SerializeField] private Image Img_SelectedSlot;
    [SerializeField] private TMP_Text Text_Name;
    [SerializeField] private TMP_Text Text_Description;

    // 그 안에 있는 UI요소를 직접 하나하나 껐다 켰다 하는게 아니라, 그 레이아웃의 대표 오브젝트만 껐다 켰다 하는게 압도적으로 편함
    //[Header("부가 정보")]
    //[SerializeField] private GameObject Layout_SubInfoWeapon;
    

    // 딕셔너리 - 생성된 슬롯들을 ID 번호와 SlotUI 컴포넌트로 저장
    private Dictionary<int, InventorySlotUI> _slotList = new Dictionary<int, InventorySlotUI>();

    // 슬롯마다 고유 번호를 붙이기 위한 변수
    private int _generatedKey = 0;

    private void OnEnable()
    {
        Btn_ClosePopup.BindOnClickButtonEvent(OnClick_CloseUI);
        Btn_BackClose.BindOnClickButtonEvent(OnClick_CloseUI);

        SetInventorySlotOnEnable();

    }

    // 인벤토리 UI가 열릴 때 현재 플레이어 아이템을 기준으로 슬롯 생성
    private void SetInventorySlotOnEnable()
    {
        // 기존 슬롯 초기화 -> 슬롯 중복 생성, UI 꼬임, 데이터 꼬임 방지
        if(_slotList.Count > 0)
        {
            foreach(var slot in _slotList)
            {
                // DestroyImmediate: 즉시 삭제
                DestroyImmediate(slot.Value.gameObject);
            }

            _slotList.Clear();
        }

        // 현재 플레이어가 가진 아이템 전체 가져오기
        var itemList = GameManager.Instance.GetPlayerItemList();
        if (itemList != null)
        {
            // 아이템 하나당 슬롯 하나 생성
            foreach (var itemModel in itemList)
            {
                CreateSlot(itemModel.ItemDataId, itemModel.ItemStackCount);
            }
        }
        else
        {
            Debug.LogWarning("보유한 아이템이 없습니다!");
        }

        var weaponList = GameManager.Instance.GetPlayerWeaponList();
        if(weaponList != null)
        {
            // 무기 하나당 슬롯 하나 생성
            foreach (var weaponModel in weaponList)
            {
                CreateSlot(weaponModel.WeaponDataId, weaponModel.WeaponStackCount);
            }

        }
        else
        {
            Debug.LogWarning("보유한 무기가 없습니다!");
        }
            
    }

    private void OnClick_CloseUI()
    {
        UIManager.Instance.ClosePopupUI(UIType.InventoryPopup);
        Debug.LogWarning("인벤토리 창이 닫혔습니다.");
    }


    private void CreateSlot(string DataId, int StackCount)
    {
        // Prefab_Slot을 Transform_UISlotRoot 자식으로 생성
        var gObj = Instantiate(Prefab_Slot, Transform_UISlotRoot);
        if (gObj == null) return;

        // 생성된 슬롯 오브젝트에서 SlotUI 컴포넌트 가져옴
        var slotComponent = gObj.GetComponent<InventorySlotUI>();
        if (slotComponent == null) return;

        // 슬롯 번호 1 증가
        _generatedKey++;

        // 생성된 슬롯에 고유번호 넣어줌
        slotComponent.InitSlot(_generatedKey, DataId, StackCount);
        // 슬롯 오브젝트 이름 바꿈 -> 하이어라키에서 보기 쉽게
        slotComponent.gameObject.name = $"InventorySlot : {slotComponent.SlotInstanceId}";

        _slotList.Add(slotComponent.SlotInstanceId, slotComponent);

        // 슬롯이 클릭됐을 때, OnChildSlotSelected 함수가 실행되도록
        slotComponent.BindSlotSelectEvent(OnChildSlotSelected);

    }

    // 자식 슬롯이 클릭됐을 때 실행되는 함수
    private void OnChildSlotSelected(int selectedSlotInstanceId)
    {
        Debug.LogWarning($"자식 슬롯 {selectedSlotInstanceId} 선택됨!");

        var slot = _slotList[selectedSlotInstanceId];

        var itemData = GameDataManager.Instance.GetItemData(slot.SlotDataId);
        if (itemData != null)
        {
            Text_Name.text = itemData.Name;
        }

        var weaponData = GameDataManager.Instance.GetWeaponData(slot.SlotDataId);
        if(weaponData != null)
        {
            GameUtil.LoadAndSetSpriteImage(Img_SelectedSlot, weaponData.IconPath).Forget();

            Text_Name.text = weaponData.Name;
            Text_Description.text = weaponData.Description;
        }
        
    }
}
