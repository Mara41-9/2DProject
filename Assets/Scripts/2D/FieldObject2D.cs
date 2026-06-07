using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class FieldObject2D : MonoBehaviour
{
    [SerializeField] private int _fieldObjectInstanceId;     // 필드 오브젝트 개별 인스턴스 ID
    [SerializeField] private string _fieldObjectDataId;      // 어떤 종류의 오브젝트인지 ID
    [SerializeField] private string _fieldObjectName;        // 오브젝트 이름

    // 필드 오브젝트 생성 직후 초기화 함수
    public void InitFieldObjectInfoOnCreated(int instanceId, string fieldObjectDataId)
    {
        // GameDataManager에서 해당 ID의 데이터 가져오기
        var fieldObjectData = GameDataManager.Instance.GetFieldObjectData(fieldObjectDataId);
        if(fieldObjectData == null)
        {
            Debug.LogWarning($"유효하지 않은 필드 오브젝트 데이터 입니다! {fieldObjectDataId}");
            return;
        }

        // 현재 오브젝트 데이터 ID 저장
        _fieldObjectInstanceId = instanceId;
        _fieldObjectDataId = fieldObjectDataId;
    }

    // 현재 오브젝트 데이터 ID 반환 함수
    public string GetFieldObjectDataId()
    {
        return _fieldObjectDataId;
    }

    // 충돌 -> 해당 오브젝트 타입 확인 -> 아이템 드랍 계산 -> 획득 아이템 리스트에 추가 -> 오브젝트 삭제
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌한게 플레이어라면
        if (collision.CompareTag("Player") == true)
        {
            // 현재 상호작용한 이 오브젝트가 어떤 종류인지 데이터에서 조회
            var fieldObjectData = GameDataManager.Instance.GetFieldObjectData(_fieldObjectDataId);
            if (fieldObjectData == null)
            {
                Debug.LogWarning($"유효하지 않은 필드 오브젝트 데이터 입니다! {_fieldObjectDataId}");
                return;
            }

            // 상호작용한 필드 오브젝트가 채집물 or 드랍아이템 타입이라면
            if (fieldObjectData.FieldObjectType == "Harvest" || fieldObjectData.FieldObjectType == "DropItem")
            {
                // 필드 오브젝트의 DropItemDataId가 null이라면
                if (string.IsNullOrEmpty(fieldObjectData.DropItemDataId))
                {
                    return;
                }

                // 현재 상호작용한 필드 아이템의 DropItemDataId로 Item 데이터 조회 
                var itemData = GameDataManager.Instance.GetItemData(fieldObjectData.DropItemDataId);
                if(itemData == null)
                {
                    return;
                }

                if(itemData.UseItemType == "Currency")
                {
                    SoundManager.Instance.PlaySFX("Sound/SFX_PickupCoin");
                }

                if(itemData.UseItemType == "StatChangeHp")
                {
                    SoundManager.Instance.PlaySFX("Sound/SFX_PickupItem");
                }

                int itemCount = 1;
                
                // GameManager에게 획득 아이템 추가 요청
                GameManager.Instance.AddObtainedItem(itemData.Id, itemCount);

                // GameObjectManager에게 이 오브젝트 비활성화/제거 요청
                GameObjectManager.Instance.RequestDestroyFieldObject(_fieldObjectInstanceId);
            }

        }
    }
}
