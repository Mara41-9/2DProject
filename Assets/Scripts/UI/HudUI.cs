using System.Collections.Generic;
using UnityEngine;

public class HudUI : UIBase
{
    [SerializeField] private GameObject Prefab_HudSlot;
    [SerializeField] private Transform Transform_SlotRoot;

    private Dictionary<int, HudSlotUI> _hudSlotList = new Dictionary<int, HudSlotUI>();

    public void AddHudSlot(int instanceId)
    {
        CreateHudSlot(instanceId);
    }

    public void RemoveHudSlot()
    {

    }

    private void CreateHudSlot(int instanceId)
    {
        var gObj = Instantiate(Prefab_HudSlot, Transform_SlotRoot);
        if (gObj == null) return;

        var slotComponent = gObj.GetComponent<HudSlotUI>();
        if(slotComponent == null) return;

        _hudSlotList.Add(instanceId, slotComponent);

    }
}

