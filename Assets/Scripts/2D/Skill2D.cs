using UnityEngine;

public class Skill2D : MonoBehaviour
{
    [Header("스킬 Id")]
    public int _skillInstanceId;
    public string _skillDataId;

    public void InitSkillInfoOnCreated(int instanceId, string skillDataId)
    {
        var skillData = GameDataManager.Instance.GetSkill(skillDataId);
        if (skillData == null) return;

        _skillInstanceId = instanceId;
        _skillDataId = skillDataId;
    }
}
