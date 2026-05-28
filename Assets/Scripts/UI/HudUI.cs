using System.Collections.Generic;
using UnityEngine;

public class HudUI : UIBase
{
    [SerializeField] private GameObject Prefab_HudSlot;
    [SerializeField] private Transform Transform_SlotRoot;

    private Dictionary<int, HudSlotUI> _hudSlotList = new Dictionary<int, HudSlotUI>();

    public void AddHudSlot(int instanceId, Transform targetTransform)
    {
        CreateHudSlot(instanceId, targetTransform);
    }

    public void RemoveHudSlot(int instanceId)
    {
        // 생성이 된게 맞다면
        if(_hudSlotList.ContainsKey(instanceId) == true)
        {
            var slot = _hudSlotList[instanceId];

            // Destroy는 컴포넌트인 slot이 아니라 slot.gameObject
            Destroy(slot.gameObject);

            _hudSlotList.Remove(instanceId);
        }
    }

    // HudSlot 생성
    private void CreateHudSlot(int instanceId, Transform targetTransform)
    {
        var gObj = Instantiate(Prefab_HudSlot, Transform_SlotRoot);
        if (gObj == null) return;

        var slotComponent = gObj.GetComponent<HudSlotUI>();
        if(slotComponent == null) return;

        // 동적 생성된 자식 슬롯(게임오브젝트) 안에 있는 컴포넌트도 잘 가져왔다
        slotComponent.InitSlot(instanceId, targetTransform);

        _hudSlotList.Add(instanceId, slotComponent);

    }
}

