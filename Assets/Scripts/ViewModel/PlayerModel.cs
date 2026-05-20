using System;
using System.Collections.Generic;
using UnityEngine;


// 플레이어 전체 저장 데이터를 담는 클래스
// JsonUtility로 직렬화하려면, Mono를 상속받지 않도록 주의하자!
// [Serializable] : 이 클래스를 JSON으로 저장하거나 불러올 수 있게 해주는 표시
[Serializable]
public class PlayerModel
{
    public string PlayerName;             // 플레이어 이름
    public int PlayerTotalExp;            // 플레이어 총 경험치 -> 레벨 계산에 사용
    public string LastMapDataId;          // 마지막으로 있었던 맵의 ID
    public Vector3 LastMapPosition;       // 마지막으로 있던 위치
    public string EquippedWeaponDataId;   // 장착된 무기 데이터 ID

    public List<ItemModel> ItemList = new List<ItemModel>();  // 플레이어가 가지고 있는 아이템 목록
    public List<WeaponModel> WeaponList = new List<WeaponModel>();   // 플레이어가 가지고 있는 무기 목록

}

// 습득 아이템의 저장 데이터를 담는 클래스
[Serializable]
public class ItemModel
{
    public long ItemUniqueId;    // 아이템 고유 ID
    public string ItemDataId;   // 어떤 종류의 아이템인지 알려주는 ID
    public int ItemStackCount;  // 아이템 개수
}

[Serializable]
public class WeaponModel
{
    public long WeaponUniqueId;
    public string WeaponDataId;
    public int WeaponStackCount;
}

