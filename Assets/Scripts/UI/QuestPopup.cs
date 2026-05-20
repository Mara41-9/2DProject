//using System.Collections.Generic;
//using UnityEngine;

//public class QuestPopup : UIBase
//{
//    // 생성할 슬롯 오브젝트
//    [SerializeField] private GameObject Prefab_Slot;

//    // 슬롯을 어디에 생성할 것인가
//    [SerializeField] private Transform Transform_UISlotRoot;

//    // 슬롯을 생성할 버튼 오브젝트
//    [SerializeField] private GameUIButton Btn_CreateSlot;

//    // 팝업창을 닫을 버튼 오브젝트
//    [SerializeField] private GameUIButton Btn_ClosePopup;

//    // 딕셔너리 - 생성된 슬롯들을 ID 번호와 SlotUI 컴포넌트로 저장
//    private Dictionary<int, SkillSlotUI> _itemSlotList = new Dictionary<int, SkillSlotUI>();

//    // 슬롯마다 고유 번호를 붙이기 위한 변수
//    private int _generatedKey = 0;

//    private void OnEnable()
//    {
//        Btn_CreateSlot.BindOnClickButtonEvent(OnClick_CreateSlot);
//        Btn_ClosePopup.BindOnClickButtonEvent(OnClick_ClosePopup);
//    }

//    private void OnClick_CreateSlot()
//    {
//        CreateSlot();
//    }

//    private void OnClick_ClosePopup()
//    {
//        UIManager.Instance.ClosePopupUI(UIType.QuestPopup);
//        Debug.LogWarning("퀘스트 창이 닫혔습니다.");
//    }


//    private void CreateSlot()
//    {
//        // Prefab_Slot을 Transform_UISlotRoot 자식으로 생성
//        var gObj = Instantiate(Prefab_Slot, Transform_UISlotRoot);
//        if (gObj == null) return;

//        // 생성된 슬롯 오브젝트에서 SlotUI 컴포넌트 가져옴
//        var slotComponent = gObj.GetComponent<SlotUI>();
//        if(slotComponent == null) return;

//        // 슬롯 번호 1 증가
//        _generatedKey++;

//        // 생성된 슬롯에 고유번호 넣어줌
//        slotComponent.SlotInstanceId = _generatedKey;
//        // 슬롯 오브젝트 이름 바꿈 -> 하이어라키에서 보기 쉽게
//        slotComponent.gameObject.name = $"QuestSlot : {slotComponent.SlotInstanceId}";

//        _itemSlotList.Add(slotComponent.SlotInstanceId, slotComponent);

//        // 슬롯이 클릭됐을 때, OnChildSlotSelected 함수가 실행되도록
//        slotComponent.BindSlotSelectEvent(OnChildSlotSelected);

//    }

//    // 자식 슬롯이 클릭됐을 때 실행되는 함수
//    private void OnChildSlotSelected(int selectedSlotInstanceId)
//    {
//        Debug.LogWarning($"자식 슬롯 {selectedSlotInstanceId} 선택됨!");
//    }
//}
