using System.Collections.Generic;
using UnityEngine;

public class SkillPopup : UIBase
{
    // 생성할 슬롯 오브젝트
    [SerializeField] private GameObject Prefab_Slot;

    // 슬롯을 어디에 생성할 것인가 - 위치
    [SerializeField] private Transform Transform_UISlotRoot;

    // GameUIButton 타입의 Button_CreateSlot 변수를 인스펙터에서 직접 연결할 수 있도록
    [SerializeField] private GameUIButton Button_CreateSlot;

    [SerializeField] private GameUIButton Btn_ClosePopup;

    // Key: int , Value: SlotSkillUI 컴포넌트 인 딕셔너리 선언
    private Dictionary<int, SlotUI> _itemSlotList = new Dictionary<int, SlotUI>();

    private int _generatedKey = 0;


    private void OnEnable()
    {
        Button_CreateSlot.BindOnClickButtonEvent(OnClick_CreateSlot);
        Btn_ClosePopup.BindOnClickButtonEvent(OnClick_CloseSkillPopup);
    }

    public void OnClick_CloseSkillPopup()
    {
        UIManager.Instance.ClosePopupUI(UIType.SkillPopup);
        Debug.LogWarning("스킬 창이 닫혔습니다.");
    }

    private void OnClick_CreateSlot()
    {
        CreateSlot();
    }

    private void CreateSlot()
    {
        // Prefab_Slot을 Transform_UISlotRoot에 실체화 - 동적생성
        var gObj = Instantiate(Prefab_Slot, Transform_UISlotRoot);
        if(gObj == null) return;

        // 자식 슬롯의 컴포넌트 가져오기 -> 위에 게임 오브젝트는 스크립트가 아직 아니기 때문에
        var slotComponent = gObj.GetComponent<SlotUI>();
        if (slotComponent == null) return;

        _generatedKey++;

        slotComponent.SlotInstanceId = _generatedKey;
        slotComponent.gameObject.name = $"SkillSlot : {slotComponent.SlotInstanceId}";

        // 생성된 슬롯의 고유 번호(SlotInstanceId)를 Key로, 그 슬롯의 SlotSkillUI 컴포넌트를 Value로 해서 딕셔너리에 저장
        _itemSlotList.Add(slotComponent.SlotInstanceId, slotComponent);

        slotComponent.BindSlotSelectEvent(OnChildSlotSelected);
    }

    private void OnChildSlotSelected(int selectedSlotInstanceId)
    {
        Debug.LogWarning($"자식 슬롯 {selectedSlotInstanceId} 선택됨!");
    }
}
