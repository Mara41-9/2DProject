using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }

    // 현재 플레이 중인 데이터를 메모리에 들고 있는 변수
    private PlayerModel _playerModel = new PlayerModel();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // 게임 시작하자마자 자동으로 세이브 데이터 불러옴
        LoadSaveData();

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
}
