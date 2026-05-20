using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlotUI : MonoBehaviour
{
    [Header("슬롯 기본 정보")]
    [SerializeField] private Text Text_StackCount;
    [SerializeField] private Image Img_Icon;
    [SerializeField] private Image Img_Frame;
    [SerializeField] private GameUIButton Btn_Slot;
    [SerializeField] private GameObject Gobj_Selected;

    private event Action<int> OnSelectEvent;

    // 각 슬롯의 고유 번호(ID)
    public int SlotInstanceId { get; private set; }
    public string SlotDataId { get; private set; }

    private void OnEnable()
    {
        // 등록된 이벤트 함수가 있다면, 현재 슬롯의 번호(SlotInstanceId)를 전달하면서 호출
        Btn_Slot.BindOnClickButtonEvent(OnClick_SelectItem);
    }

    // 스킬 ID 받아서 해당 그 스킬의 아이콘 스프라이트를 찾아 슬롯 이미지에 넣어주는 함수
    private void SetIcon(string DataId, int Count)
    {
        var skillData = GameDataManager.Instance.GetSkill(DataId);
        if(skillData != null)
        {
            string skillIconPath = skillData.IconPath;
            if(skillIconPath != null)
            {
                GameUtil.LoadAndSetSpriteImage(Img_Icon, skillIconPath).Forget();
                Text_StackCount.text = $"{Count}";

                return;
            }
            else
            {
                Debug.LogWarning($"스킬 데이터의 IconPath가 존재하지 않습니다.");
            }
        }
        else
        {
            Debug.LogWarning($"스킬 데이터가 존재하지 않습니다.");
        }

    }

    public void InitSlot(int slotInstanceId, string dataId, int StackCount)
    {
        SlotInstanceId = slotInstanceId;
        SlotDataId = dataId;
        SetIcon(dataId, StackCount);
    }


    private void OnClick_SelectItem()
    {
        // 부모한테 알려주자
        // OnSelectEvent에 연결된 함수가 null이 아니면, SlotInstanceId 값을 넘겨서 실행
        OnSelectEvent?.Invoke(SlotInstanceId);

        Debug.Log($"{SlotInstanceId}이 눌러졌다");
    }


    // 외부에서 호출 가능한 함수
    // -> Action<int> 타입의 함수를 매개변수로 받음
    // 즉, int 하나를 받아서 처리하는 함수를 넘겨받겠다는 뜻!
    public void BindSlotSelectEvent(Action<int> onSelectEvent)
    {
        // 외부(부모 객체)에서 전달받은 함수를 OnSelectEvent에 등록
        // 슬롯이 클릭될 때 그 함수가 실행
        OnSelectEvent = onSelectEvent;
    }
}
