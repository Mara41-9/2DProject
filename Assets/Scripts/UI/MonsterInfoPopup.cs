using TMPro;
using UnityEngine;

public class MonsterInfoPopup : UIBase
{
    [SerializeField] private TMP_Text Text_MonsterName;
    [SerializeField] private TMP_Text Text_MonsterDesc;
    [SerializeField] private TMP_Text Text_MonsterAtk;

    private void OnEnable()
    {
        SetPopupUI();
    }

    private void SetPopupUI()
    {
        
    }
}
