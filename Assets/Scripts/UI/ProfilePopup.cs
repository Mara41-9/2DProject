using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ProfilePopup : UIBase
{
    [Header("Text")]
    [SerializeField] private Text Text_CharacterName;
    [SerializeField] private Text Text_CharacterDesc;
    [SerializeField] private Text Text_Level;
    [SerializeField] private Text Text_SkillName;
    [SerializeField] private Text Text_SkillDesc;
    [SerializeField] private Text Text_WeaponName;
    [SerializeField] private Text Text_WeaponDesc;

    [Header("Button")]
    [SerializeField] private GameUIButton Btn_ClosePopup;
    [SerializeField] private GameUIButton Btn_BackClose;

    private int _level = 0;

    private void OnEnable()
    {
        Btn_ClosePopup.BindOnClickButtonEvent(OnClick_CloseProfilePopup);
        Btn_BackClose.BindOnClickButtonEvent(OnClick_CloseProfilePopup);

        _level++;
        Text_Level.text = $"{_level}";

        // IEnumerator를 매 프레임 체크해줘!
        // StartCoroutine(CoCloseSelf());
    }

    public void OnClick_CloseProfilePopup()
    {
        UIManager.Instance.ClosePopupUI(UIType.ProfilePopup);
        UIManager.Instance.ClosePopupUI(UIType.ProfilePopup);
        Debug.LogWarning("프로필 창이 닫혔습니다.");
    }


    public void Start()
    {
        GameUtil.LoadFullData();

        var myHero = GameDataManager.Instance.GetCharacterData("character_selly_01");

        // 캐릭터 이름 출력하기
        if (myHero != null)
        {
            Debug.Log($"로드된 캐릭터 이름: {myHero.Name}");

            // 팝업창에 캐릭터 이름 가져오기
            Text_CharacterName.text = myHero.Name;
        }


        // 문자열을 담아둘 임시 변수
        string dummyCharacterDesc = string.Empty;
        string dummySkillName = string.Empty;
        string dummySkillDesc = string.Empty;
        string dummyWeaponName = string.Empty;
        string dummyWeaponDesc = string.Empty;

        if(myHero.Description.Contains("<nl>"))
        {
            // myHero.Description 안에 있는 <nl>라는 문자열을 전부 찾아서, 줄바꿈(\n)으로 바꾼다
            dummyCharacterDesc = myHero.Description.Replace("<nl>", "\n");
            Debug.Log($"로드된 캐릭터 설명: {dummyCharacterDesc}");
            Text_CharacterDesc.text = dummyCharacterDesc;
        }
        else
        {
            return;
        }


        // 가지고 있는 기술(스킬) 데이터 가져오기
        if (myHero.SkillList != string.Empty)
        {
            string[] skillNameList = myHero.SkillList.Split(',');
            foreach (string skillName in skillNameList)
            {
                var skillData = GameDataManager.Instance.GetSkill(skillName);
                if (skillData != null)
                {
                    if (dummySkillName != string.Empty)
                    {
                        dummySkillName += ", ";
                    }

                    dummySkillName += skillData.Name;
                    dummySkillDesc = skillData.Description;


                }

            }

            Debug.Log($"로드된 캐릭터 스킬: {dummySkillName} {dummySkillDesc}");
            Text_SkillName.text = dummySkillName;
            Text_SkillDesc.text = dummySkillDesc;
        }


        // 가지고 있는 무기 데이터 가져오기
        if(myHero.UseWeaponId != string.Empty)
        {
            var weaponData = GameDataManager.Instance.GetWeaponData(myHero.UseWeaponId);

            if(weaponData != null)
            {
                dummyWeaponName = weaponData.Name;
                dummyWeaponDesc = weaponData.Description;
                Debug.Log($"로드된 캐릭터 무기: {dummyWeaponName} {dummyWeaponDesc}");
                Text_WeaponName.text = dummyWeaponName;
                Text_WeaponDesc.text = dummyWeaponDesc;
            }
            
        }
        

        //if (string.IsNullOrEmpty(myHero.UseWeaponId) == false)
        //{
        //    var weaponData = GameDataManager.Instance.GetWeaponData(myHero.UseWeaponId);
        //    if (weaponData != null)
        //    {
        //        Debug.Log($"로드된 캐릭터: {myHero.Name}는 사용무기로 {weaponData.Name}을 갖고 있다!");
        //    }
        //}
    }


    // 1.5초 기다렸다가 프로필 팝업을 닫는 코루틴 함수
    // IEnumerator : 코드의 실행 중간 지점을 기억하고, 잠시 멈췄다 다시 시작할 수 있는 기능
    //IEnumerator CoCloseSelf()
    //{
    //    Debug.Log("코루틴 처음 불려짐");

    //    // yield return : 이 지점부터 다른 작업 할 수 있게 양보, 정해진 조건이 되면 여기서부터 다시 실행
    //    yield return new WaitForSeconds(1.5f);
    //    UIManager.Instance.CloseSpecificUI(UIType.ProfilePopup);
    //    Debug.Log("프로필 창이 1.5초 뒤에 닫힘");

    //}
}
