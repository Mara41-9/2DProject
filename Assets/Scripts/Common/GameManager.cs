using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }

    // 현재 플레이 중인 데이터를 메모리에 들고 있는 변수
    private PlayerModel _playerModel = new PlayerModel();
    [SerializeField] private PlayerMovement _playerMovement;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        var WeaponData = GameDataManager.Instance.GetWeaponData("Weapon_Sword_1");
        if(WeaponData == null)
        {
            Debug.LogWarning("기본 무기 데이터를 찾을 수 없습니다.");
            return;
        }

        if(_playerModel.WeaponList.Count == 0)
        {
            AddWeapon(WeaponData.Id);
        }

        var ItemData = GameDataManager.Instance.GetItemData("Item_Potion_1");
        if(ItemData == null)
        {
            Debug.LogWarning("기본 아이템 데이터를 찾을 수 없습니다.");
            return;
        }

        if(_playerModel.ItemList.Count == 0)
        {
            AddItem(ItemData.Id, 1);
        }

        var SkillDataList = GameDataManager.Instance.SkillDataList;
        if(SkillDataList != null)
        {
            foreach(var skillData in SkillDataList)
            {
                AddSkill(skillData.Key);
            }
        }

        // 게임 시작하자마자 자동으로 세이브 데이터 불러옴
        LoadSaveData();

    }

    // 현재 플레이 데이터를 저장하는 함수
    public void SaveData()
    {
        // 현재 _playerViewModel 데이터를 넘겨서 JSON 저장 요청
        NetworkManager.Instance.RequestSaveData(_playerModel);
    }

    // 게임 저장 -> 종료
    public void SaveAndEndGame()
    {
        SaveData();
        // 유니티 게임 프로그램 종료 요청 함수 -> 현재 실행 중인 게임만 종료
        Application.Quit();
    }

    // 세이브 데이터 불러오기
    public void LoadSaveData()
    {
        // 저장 파일에서 읽어온 데이터를 현재 메모리 데이터로 넣기
        _playerModel = NetworkManager.Instance.RequestLoadSaveData();
    }

    public void RefreshGame()
    {
        // 플레이어 위치 초기화
        var playerPosition = _playerMovement.GetPlayerPosition();
        if (playerPosition == null) return;
        
        _playerMovement.transform.position = playerPosition;

        // 생성된 몬스터들 제거하기
        GameObjectManager.Instance.DestroyMonsterAll();

        // 플레이어 HP 초기화하기
        var player = GameDataManager.Instance.GetCharacterData("character_selly_01");
        if (player == null) return;

        _playerMovement.SetPlayerHp(player.Hp);
        
    }

    // 플레이어의 현재 총 Exp 가져오기
    public int GetPlayerExp()
    {
        return _playerModel.PlayerTotalExp;
    }

    // 플레이어 경험치 증가
    public void IncreasePlayerExp(int exp)
    {
        // 현재 경험치에 추가 경험치 더하기
        // 추후에 한곳에서 관리할 수 있게 익스텐션(확장메서드)으로 빼도 된다
        _playerModel.PlayerTotalExp += exp;
        Debug.LogWarning($"현재 누적 Exp: {_playerModel.PlayerTotalExp}");
    }

    public void IncreasePlayerLevel(int level)
    {
        _playerModel.PlayerLevel += level;
    }

    public int GetPlayerLevel()
    {
        return _playerModel.PlayerLevel;    
    }

    // 플레이어 인벤토리에 아이템 추가
    // -> 특정 아이템(itemDataId)을 개수(addItemCount)만큼 플레이어 인벤토리에 추가하자
    public void AddItem(string itemDataId, int addItemCount)
    {
        // 저장할 때 고유값 ID를 부여하기 위해 사용 (고유 번호 생성)
        long uniqueId = GameUtil.GenerateUniqueId();

        // TODO : 우선 쉽게 사용할 수 있도록 중복 처리는 빼두었다. 습득할때마다 아이템이 하나씩 추가되도록 해두고
        // 추후에 중복값은 StackCount가 다 찰때까지 누적해줄 수 있도록 로직을 추가하자
        var newItem = new ItemModel();
        newItem.ItemUniqueId = uniqueId;
        newItem.ItemDataId = itemDataId;
        newItem.ItemStackCount = addItemCount;

        _playerModel.ItemList.Add(newItem);
    }

    public void AddWeapon(string weaponDataId)
    {
        // 저장할 때 고유값 ID를 부여하기 위해 사용 (고유 번호 생성)
        long uniqueId = GameUtil.GenerateUniqueId();

        // TODO : 우선 쉽게 사용할 수 있도록 중복 처리는 빼두었다. 습득할때마다 아이템이 하나씩 추가되도록 해두고
        // 추후에 중복값은 StackCount가 다 찰때까지 누적해줄 수 있도록 로직을 추가하자
        var newItem = new WeaponModel();
        newItem.WeaponUniqueId = uniqueId;
        newItem.WeaponDataId = weaponDataId;
        newItem.WeaponStackCount = 1;

        _playerModel.WeaponList.Add(newItem);
    }

    public void AddSkill(string skillDataId)
    {
        // 저장할 때 고유값 ID를 부여하기 위해 사용 (고유 번호 생성)
        long uniqueId = GameUtil.GenerateUniqueId();

        // TODO : 우선 쉽게 사용할 수 있도록 중복 처리는 빼두었다. 습득할때마다 아이템이 하나씩 추가되도록 해두고
        // 추후에 중복값은 StackCount가 다 찰때까지 누적해줄 수 있도록 로직을 추가하자
        var newItem = new SkillModel();
        newItem.SkillUniqueId = uniqueId;
        newItem.SkillDataId = skillDataId;
        newItem.SkillStackCount = 1;

        _playerModel.SkillList.Add(newItem);
    }

    // 현재 장착 무기를 설정하자 -> 가져올땐 Get
    public void SetEquippedWeapon(string weaponDataId)
    {
        if(string.IsNullOrEmpty(weaponDataId))
        {
            Debug.LogWarning("장착할 무기 ID가 없습니다.");
            return;
        }

        var weaponData = GameDataManager.Instance.GetWeaponData(weaponDataId);
        if(weaponData == null)
        {
            Debug.LogWarning("무기 데이터를 찾을 수 없습니다.");
            return;
        }

        _playerModel.EquippedWeaponDataId = weaponDataId;
        Debug.LogWarning($"'{weaponData.Name}'무기가 장착되었습니다.");
    }

    public void SetEquippedSkill(string skillDataId)
    {
        if(string.IsNullOrEmpty(skillDataId))
        {
            Debug.LogWarning("장착할 스킬 ID가 없습니다.");
            return;
        }

        var skillData = GameDataManager.Instance.GetSkill(skillDataId);
        if(skillData == null)
        {
            Debug.LogWarning("스킬 데이터를 찾을 수 없습니다.");
            return;
        }

        _playerModel.EquippedSkillDataId = skillDataId;
        Debug.LogWarning($"'{skillData.Name}'스킬이 장착되었습니다.");
    }

    public string GetEquippedWeapon()
    {
        return _playerModel.EquippedWeaponDataId;
    }

    public string GetEquippedSkill()
    {
        return _playerModel.EquippedSkillDataId;
    }

    public List<ItemModel> GetPlayerItemList()
    {
        // _playerModel이 Private이므로 외부에서 ItemList를 받아올 수 있게 Get함수 사용
        return _playerModel.ItemList;
    }

    public List<WeaponModel> GetPlayerWeaponList()
    {
        // _playerModel이 Private이므로 외부에서 WeaponList를 받아올 수 있게 Get함수 사용
        return _playerModel.WeaponList;
    }

    public List<SkillModel> GetPlayerSkillList()
    {
        return _playerModel.SkillList;
    }
}
