using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SuccessPopup : UIBase
{
    [Header("돌아가기 버튼")]
    [SerializeField] private GameUIButton Btn_GoMainUI;

    [Header("동적 생성할 슬롯")]
    [SerializeField] private GameObject Prefab_Slot;

    [Header("생성되는 위치")]
    [SerializeField] private Transform Transform_UISlotRoot;

    private Dictionary<long, SuccessPopupSlotUI> _slotList = new Dictionary<long, SuccessPopupSlotUI>();

    public void OnEnable()
    {
        Btn_GoMainUI.BindOnClickButtonEvent(OnClick_GoMainUIBtn);
        SetSuccessPopupSlotOnEnable();
    }

    public void OnDisable()
    {
        ClearSlotList();
    }

    private void OnClick_GoMainUIBtn()
    {
        GameManager.Instance.RefreshGame();
        Debug.LogWarning("로비메인UI로 이동합니다.");
        UIManager.Instance.OpenLoadingUI();
        UIManager.Instance.CloseSuccessPopup();
        UIManager.Instance.OpenLobbyUI();
    }

    private void SetSuccessPopupSlotOnEnable()
    {
        ClearSlotList();
        var obtainedItemList = GameManager.Instance.GetPlayerObtainedItemList();
        if (obtainedItemList == null) return;

        foreach(var obtainedItem in obtainedItemList)
        {
            CreateSlot(obtainedItem.ItemUniqueId, obtainedItem.ItemDataId, obtainedItem.ItemStackCount);
        }
    }

    private void CreateSlot(long uniqueId, string dataId, int stackCount)
    {
        var slot = Instantiate(Prefab_Slot, Transform_UISlotRoot);
        if (slot == null) return;

        var component = slot.GetComponent<SuccessPopupSlotUI>();
        if(component == null) return;

        component.InitSlot(dataId, stackCount);

        _slotList.Add(uniqueId, component);
    }

    private void ClearSlotList()
    {
        if(_slotList.Count > 0)
        {
            foreach(var slotKv in _slotList)
            {
                var slot = slotKv.Value;
                DestroyImmediate(slot.gameObject);
            }

            _slotList.Clear();
        }
    }

}
