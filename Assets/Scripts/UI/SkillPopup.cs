using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillPopup : UIBase
{
    [Header("동적 생성할 프리팹")]
    [SerializeField] private GameObject Prefab_Slot;

    [Header("슬롯 리스트 영역")]
    [SerializeField] private Transform Transform_UISlotRoot;

    [Header("팝업창 닫기 버튼")]
    [SerializeField] private GameUIButton Btn_ClosePopup;
    [SerializeField] private GameUIButton Btn_BackClosePopup;

    [Header("선택된 슬롯 정보")]
    [SerializeField] private Image Img_SelectedSlot;
    [SerializeField] private TMP_Text Text_SelectedSlotName;
    [SerializeField] private TMP_Text Text_SelectedSlotDesc;

    // Key: int , Value: SlotSkillUI 컴포넌트 인 딕셔너리 선언
    private Dictionary<int, SkillSlotUI> _skillSlotList = new Dictionary<int, SkillSlotUI>();

    private int _generatedKey = 0;


    private void OnEnable()
    {
        Btn_ClosePopup.BindOnClickButtonEvent(OnClick_CloseSkillPopup);
        Btn_BackClosePopup.BindOnClickButtonEvent(OnClick_CloseSkillPopup);

        SetSkillSlotOnEnable();
    }

    public void OnClick_CloseSkillPopup()
    {
        UIManager.Instance.ClosePopupUI(UIType.SkillPopup);
        Debug.LogWarning("스킬 창이 닫혔습니다.");
    }

    // 스킬팝업이 열릴 때 현재 플레이어가 가지고있는 스킬을 기준으로 슬롯 생성
    private void SetSkillSlotOnEnable()
    {
        var skillList = GameManager.Instance.GetPlayerSkillList();
        if(skillList != null)
        {
            foreach(var skillModel in skillList)
            {
                CreateSlot(skillModel.SkillDataId, skillModel.SkillStackCount);
            }
        }
        else
        {
            Debug.LogWarning("보유한 스킬이 없습니다!");
        }
    }

    private void CreateSlot(string DataId, int StackCount)
    {
        // Prefab_Slot을 Transform_UISlotRoot에 실체화 - 동적생성
        var gObj = Instantiate(Prefab_Slot, Transform_UISlotRoot);
        if(gObj == null) return;

        // 자식 슬롯의 컴포넌트 가져오기 -> 위에 게임 오브젝트는 스크립트가 아직 아니기 때문에
        var slotComponent = gObj.GetComponent<SkillSlotUI>();
        if (slotComponent == null) return;

        _generatedKey++;

        slotComponent.InitSlot(_generatedKey, DataId, StackCount);
        slotComponent.gameObject.name = $"SkillSlot : {slotComponent.SlotInstanceId}";

        // 생성된 슬롯의 고유 번호(SlotInstanceId)를 Key로, 그 슬롯의 SlotSkillUI 컴포넌트를 Value로 해서 딕셔너리에 저장
        _skillSlotList.Add(slotComponent.SlotInstanceId, slotComponent);

        slotComponent.BindSlotSelectEvent(OnChildSlotSelected);
    }

    private void OnChildSlotSelected(int selectedSlotInstanceId)
    {
        Debug.LogWarning($"자식 슬롯 {selectedSlotInstanceId} 선택됨!");

        var slot = _skillSlotList[selectedSlotInstanceId];
        if (slot == null) return;

        var skillData = GameDataManager.Instance.GetSkill(slot.SlotDataId);
        if (skillData != null)
        {
            string skillDataIconPath = skillData.IconPath;
            if(skillDataIconPath != null)
            {
                GameUtil.LoadAndSetSpriteImage(Img_SelectedSlot, skillDataIconPath).Forget();

                Text_SelectedSlotName.text = skillData.Name;
                Text_SelectedSlotDesc.text = skillData.Description;
            }
            
        }
    }
}
