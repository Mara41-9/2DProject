using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterInfoPopup : UIBase
{
    [SerializeField] private Image Image_Monster;
    [SerializeField] private TMP_Text Text_MonsterName;
    [SerializeField] private TMP_Text Text_MonsterDesc;
    [SerializeField] private TMP_Text Text_MonsterAtk;
    [SerializeField] private TMP_Text Text_MonsterHp;

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
            GameUtil.LoadAndSetSpriteImage(Image_Monster, monsterData.IconPath).Forget();
            Text_MonsterName.text = monsterData.Name;
            Text_MonsterDesc.text = monsterData.Description;
            Text_MonsterAtk.text = $"{monsterData.BaseAtk}";
            Text_MonsterHp.text = $"{monsterData.BaseHp}";
        }
    }

    public void SetPosition(Vector2 slotPosition)
    {
        this.transform.position = slotPosition + new Vector2(250f, -100f);
    }
}
