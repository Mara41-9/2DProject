using TMPro;
using UnityEngine;

public class MonsterInfoPopup : UIBase
{
    [SerializeField] private TMP_Text Text_MonsterName;
    [SerializeField] private TMP_Text Text_MonsterDesc;
    [SerializeField] private TMP_Text Text_MonsterAtk;

    private string _monsterDataId;

    private void OnEnable()
    {
        SetMonsterInfo(_monsterDataId);
    }

    public void SetMonsterInfo(string monsterDataId)
    {
        _monsterDataId = monsterDataId;

        var monsterData = GameDataManager.Instance.GetMonsterData(monsterDataId);
        if(monsterData != null)
        {
            Text_MonsterName.text = monsterData.Name;
            Text_MonsterDesc.text = monsterData.Description;
            Text_MonsterAtk.text = $"{monsterData.BaseAtk}";
        }
      
    }
}
