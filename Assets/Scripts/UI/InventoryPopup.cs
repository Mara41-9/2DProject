using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum EInventoryCategory
{
    None = 0,
    WeaponCategory,
    ItemCategory
}

public class InventoryPopup : UIBase
{
    [Header("동적 생성할 프리팹")]
    [SerializeField] private GameObject Prefab_ItemSlot;    // 생성할 아이템 슬롯 오브젝트
    [SerializeField] private GameObject Prefab_WeaponSlot;    // 생성할 무기 슬롯 오브젝트

    [Header("슬롯 리스트 영역")]
    [SerializeField] private Transform Transform_UISlotRoot;   // 슬롯이 생성되는 곳

    [Header("팝업창 닫기 버튼")]
    [SerializeField] private GameUIButton Btn_ClosePopup;
    [SerializeField] private GameUIButton Btn_BackClose;

    [Header("선택된 슬롯 정보")]
    [SerializeField] private Image Img_SelectedSlot;
    [SerializeField] private TMP_Text Text_Name;
    [SerializeField] private TMP_Text Text_Description;

    [Header("상단 카테고리")]
    [SerializeField] private GameUIButton Btn_WeaponCategory;
    [SerializeField] private GameUIButton Btn_ItemCategory;

    [Header("사용/장착 버튼")]
    [SerializeField] private TMP_Text Text_UseButton;
    [SerializeField] private GameUIButton Btn_UseButton;

    // 그 안에 있는 UI요소를 직접 하나하나 껐다 켰다 하는게 아니라, 그 레이아웃의 대표 오브젝트만 껐다 켰다 하는게 압도적으로 편함
    //[Header("부가 정보")]
    //[SerializeField] private GameObject Layout_SubInfoWeapon;


    // 딕셔너리 - 생성된 슬롯들을 ID 번호와 SlotUI 컴포넌트로 저장
    private Dictionary<long, InventoryItemSlotUI> _slotItemList = new Dictionary<long, InventoryItemSlotUI>();
    private Dictionary<long, InventoryWeaponSlotUI> _slotWeaponList = new Dictionary<long, InventoryWeaponSlotUI>();

    private InventoryItemSlotUI _selectedItemSlot;
    private InventoryWeaponSlotUI _selectedWeaponSlot;

    private EInventoryCategory _curCategory = EInventoryCategory.None;

    private void OnEnable()
    {
        Btn_ClosePopup.BindOnClickButtonEvent(OnClick_CloseUI);
        Btn_BackClose.BindOnClickButtonEvent(OnClick_CloseUI);
        Btn_WeaponCategory.BindOnClickButtonEvent(OnClick_WeaponCategory);
        Btn_ItemCategory.BindOnClickButtonEvent(OnClick_ItemCategory);
        Btn_UseButton.BindOnClickButtonEvent(OnClick_UseButton);

        // 인벤토리 창이 "열릴 때마다" 초기 상태를 다시 세팅
        _curCategory = EInventoryCategory.WeaponCategory;
        SetInventorySlotOnEnable(_curCategory);
        Text_UseButton.text = "장착";
    }

    // 인벤토리창을 닫을 때, 생성됐던 슬롯들 제거하고 딕셔너리 초기화
    private void OnDisable()
    {
        ClearSlotList();
    }

    private void OnClick_CloseUI()
    {
        UIManager.Instance.ClosePopupUI(UIType.InventoryPopup);
        Debug.LogWarning("인벤토리 창이 닫혔습니다.");
    }

    public void OnClick_WeaponCategory()
    {
        ClearSelectedInfo();
        _curCategory = EInventoryCategory.WeaponCategory;
        SetInventoryLayoutByCategory(_curCategory);
        Text_UseButton.text = "장착";
    }

    public void OnClick_ItemCategory()
    {
        ClearSelectedInfo();
        _curCategory = EInventoryCategory.ItemCategory;
        SetInventoryLayoutByCategory(_curCategory);
        Text_UseButton.text = "사용";
    }

    public void OnClick_UseButton()
    {
        

        if(_curCategory == EInventoryCategory.WeaponCategory)
        {
            if (_selectedWeaponSlot == null)
            {
                Debug.LogWarning("선택된 무기 슬롯이 존재하지 않습니다.");
                return;
            }

            var slotDataId = _selectedWeaponSlot.GetSlotDataId();
            if (string.IsNullOrEmpty(slotDataId) == true) return;

            var slotData = GameDataManager.Instance.GetWeaponData(slotDataId);
            if (slotData == null) return;

            var playerModel = GameManager.Instance.GetPlayerModel();
            if(playerModel == null) return;

            if (playerModel.PlayerLevel >= slotData.RequiredLevel)
            {
                GameManager.Instance.SetEquippedWeapon(_selectedWeaponSlot.SlotDataId);

                // 플레이어가 존재하면 공격력 갱신 함수 호출
                var player = GameObjectManager.Instance.GetLocalPlayer();
                if (player != null)
                {
                    player.UpdateBaseAtk();
                }
            }
            else
            {
                UIManager.Instance.OpenCommonToastUI();

                var commonToastUI = UIManager.Instance.GetOpenedUI(UIRootType.ToastUI, UIType.CommonToastUI);
                if (commonToastUI == null) return;

                var component = commonToastUI.GetComponent<CommonToastUI>();
                if(component == null) return;

                component.SetMessage("현재 레벨에선 이 무기를 장착할 수 없습니다.");
            }
            
        }
        else if(_curCategory == EInventoryCategory.ItemCategory)
        {
            if (_selectedItemSlot == null)
            {
                Debug.LogWarning("선택된 아이템 슬롯이 존재하지 않습니다.");
                return;
            }

            var itemData = GameDataManager.Instance.GetItemData(_selectedItemSlot.SlotDataId);
            if (itemData == null) return;

            if(itemData.UseItemType == "StatChangeHp")
            {
                UIManager.Instance.OpenCommonToastUI();
                var commonToastUI = UIManager.Instance.GetOpenedUI(UIRootType.ToastUI, UIType.CommonToastUI);
                if (commonToastUI == null) { return; }

                var component = commonToastUI.GetComponent<CommonToastUI>();
                if(component == null) { return; }

                component.SetMessage("회복 아이템은 전투 중에만 사용할 수 있습니다.");
            }
        }
    }

    private void SetInventoryLayoutByCategory(EInventoryCategory category)
    {
        switch(category)
        {
            case EInventoryCategory.WeaponCategory:
                SetInventorySlotOnEnable(category);
                break;
            case EInventoryCategory.ItemCategory:
                SetInventorySlotOnEnable(category);
                break;
            default:
                break;
        }
    }

    // 인벤토리 UI가 열릴 때 현재 플레이어 무기/아이템을 기준으로 슬롯 생성
    private void SetInventorySlotOnEnable(EInventoryCategory curCategory)
    {
        // 기존 슬롯 초기화 -> 슬롯 중복 생성, UI 꼬임, 데이터 꼬임 방지
        ClearSlotList();

        if (curCategory == EInventoryCategory.ItemCategory)
        {
            // 현재 플레이어가 가진 아이템 전체 가져오기
            var itemList = GameManager.Instance.GetPlayerItemList();
            if (itemList != null)
            {
                // 아이템 하나당 슬롯 하나 생성
                foreach (var itemModel in itemList)
                {
                    CreateItemSlot(itemModel.ItemUniqueId, itemModel.ItemDataId, itemModel.ItemStackCount, EInventoryCategory.ItemCategory);
                }
            }
            else
            {
                Debug.LogWarning("보유한 아이템이 없습니다!");
            }
        }
        else if(curCategory == EInventoryCategory.WeaponCategory)
        {
            var weaponList = GameManager.Instance.GetPlayerWeaponList();
            if (weaponList != null)
            {
                // 무기 하나당 슬롯 하나 생성
                foreach (var weaponModel in weaponList)
                {
                    var slot = CreateWeaponSlot(weaponModel.WeaponUniqueId, weaponModel.WeaponDataId, weaponModel.WeaponAttack, EInventoryCategory.WeaponCategory);

                    var playerModel = GameManager.Instance.GetPlayerModel();
                    if (playerModel == null) return;

                    var weponData = GameDataManager.Instance.GetWeaponData(weaponModel.WeaponDataId);
                    if (weponData == null) return;

                    slot.SetLockUI(playerModel.PlayerLevel < weponData.RequiredLevel);
                }

            }
            else
            {
                Debug.LogWarning("보유한 무기가 없습니다!");
            }
        }
            
    }

    private InventoryItemSlotUI CreateItemSlot(long UniqueId, string DataId, int StackCount, EInventoryCategory curCategory)
    {
        // Prefab_Slot을 Transform_UISlotRoot 자식으로 생성
        var gObj = Instantiate(Prefab_ItemSlot, Transform_UISlotRoot);
        if (gObj == null) return null;

        // 생성된 슬롯 오브젝트에서 SlotUI 컴포넌트 가져옴
        var itemSlotComponent = gObj.GetComponent<InventoryItemSlotUI>();
        if (itemSlotComponent == null) return null;

        // 생성된 슬롯에 고유번호 넣어줌
        itemSlotComponent.InitSlot(UniqueId, DataId, StackCount, curCategory);
        // 슬롯 오브젝트 이름 바꿈 -> 하이어라키에서 보기 쉽게
        itemSlotComponent.gameObject.name = $"InventorySlot : {itemSlotComponent.SlotUniqueId}";

        _slotItemList.Add(itemSlotComponent.SlotUniqueId, itemSlotComponent);

        // 슬롯이 클릭됐을 때, OnChildSlotSelected 함수가 실행되도록
        itemSlotComponent.BindSlotSelectEvent(OnChildItemSlotSelected);

        return itemSlotComponent;

    }

    private InventoryWeaponSlotUI CreateWeaponSlot(long UniqueId, string DataId, int Attack, EInventoryCategory curCategory)
    {
        // Prefab_Slot을 Transform_UISlotRoot 자식으로 생성
        var gObj = Instantiate(Prefab_WeaponSlot, Transform_UISlotRoot);
        if (gObj == null) return null;

        // 생성된 슬롯 오브젝트에서 SlotUI 컴포넌트 가져옴
        var weaponSlotComponent = gObj.GetComponent<InventoryWeaponSlotUI>();
        if (weaponSlotComponent == null) return null;

        // 생성된 슬롯에 고유번호 넣어줌
        weaponSlotComponent.InitSlot(UniqueId, DataId, Attack, curCategory);
        // 슬롯 오브젝트 이름 바꿈 -> 하이어라키에서 보기 쉽게
        weaponSlotComponent.gameObject.name = $"InventorySlot : {weaponSlotComponent.SlotUniqueId}";

        _slotWeaponList.Add(weaponSlotComponent.SlotUniqueId, weaponSlotComponent);

        // 슬롯이 클릭됐을 때, OnChildSlotSelected 함수가 실행되도록
        weaponSlotComponent.BindSlotSelectEvent(OnChildWeaponSlotSelected);

        return weaponSlotComponent;

    }

    private void ClearSelectedInfo()
    {
        Img_SelectedSlot.gameObject.SetActive(false);
        Text_Name.text = "";
        Text_Description.text = "";
    }

    private void ClearSlotList()
    {
        if (_slotItemList.Count > 0)
        {
            foreach (var slotKv in _slotItemList)
            {
                var slot = slotKv.Value;
                DestroyImmediate(slot.gameObject);
            }

            _slotItemList.Clear();
        }

        if( _slotWeaponList.Count > 0)
        {
            foreach (var slotKv in _slotWeaponList)
            {
                var slot = slotKv.Value;
                DestroyImmediate(slot.gameObject);
            }

            _slotWeaponList.Clear();
        }
    }

    // 자식 슬롯이 클릭됐을 때 실행되는 함수
    private void OnChildItemSlotSelected(long selectedUniqueId)
    {
        Img_SelectedSlot.gameObject.SetActive(true);

        var itemSlot = _slotItemList[selectedUniqueId];
        _selectedItemSlot = itemSlot;

        var itemData = GameDataManager.Instance.GetItemData(_selectedItemSlot.SlotDataId);
        if (itemData != null)
        {
            Debug.LogWarning($"'{itemData.Name}'이(가) 선택됐다. 슬롯 고유 번호 : {selectedUniqueId}");

            GameUtil.LoadAndSetSpriteImage(Img_SelectedSlot, itemData.IconPath).Forget();

            Text_Name.text = itemData.Name;
            Text_Description.text = itemData.Description;

            // 현재 클릭된 슬롯만 선택 상태로 만들고, 나머지 슬롯은 선택 해제
            foreach (var selectedSlotKv in _slotItemList)
            {
                var selectedSlot = selectedSlotKv.Value;
                var dataId = selectedSlot.GetSlotDataId();
                selectedSlot.SetSelectedUI(itemSlot.SlotDataId == dataId);
            }
        }

    }

    private void OnChildWeaponSlotSelected(long selectedUniqueId)
    {
        Img_SelectedSlot.gameObject.SetActive(true);

        var weponSlot = _slotWeaponList[selectedUniqueId];
        _selectedWeaponSlot = weponSlot;

        var weaponData = GameDataManager.Instance.GetWeaponData(_selectedWeaponSlot.SlotDataId);
        if (weaponData != null)
        {
            Debug.LogWarning($"'{weaponData.Name}'이(가) 선택됐다. 슬롯 고유 번호 : {selectedUniqueId}");

            GameUtil.LoadAndSetSpriteImage(Img_SelectedSlot, weaponData.IconPath).Forget();

            Text_Name.text = weaponData.Name;
            Text_Description.text = weaponData.Description;

            foreach (var selectedSlotKv in _slotWeaponList)
            {
                var selectedSlot = selectedSlotKv.Value;
                var dataId = selectedSlot.GetSlotDataId();
                selectedSlot.SetSelectedUI(weponSlot.SlotDataId == dataId);
            }
        }

    }
}
