using UnityEngine;
using UnityEngine.UI;

public class HudSlotUI : MonoBehaviour
{
    [SerializeField] private int SlotOffestY;     // HUD 슬롯을 대상보다 얼마나 위에 띄울지
    [SerializeField] private Slider Slider_Hp;
    [SerializeField] private Slider Slider_Mp;

    private int _instanceId;

    // 참조형을 기록(캐싱)
    private Transform _targetTransform;

    public void InitSlot(int instanceId, Transform targetTransform)
    {
        _instanceId = instanceId;
        _targetTransform = targetTransform;
        SlotOffestY = 50;

        TryBindSetChangedEvent(targetTransform.gameObject);
    }

    // gObj가 몬스터거나, 플레이어라면 GetComponent를 시도해보고 잘 되면 그곳에 있는 이벤트를 구독하자!
    private void TryBindSetChangedEvent(GameObject gObj)
    {
        var player = gObj.GetComponent<PlayerMovement>();
        if(player != null)
        {
            player.BindOnStatChangedEvent(OnTargetEntityHpChanged, OnTargetEntityMpChanged);
            return;
        }

        var monster = gObj.GetComponent<Monster2D>();
        if(monster != null)
        {
            monster.BindOnStatChangedEvent(OnTargetEntityHpChanged, OnTargetEntityMpChanged);
            return;
        }
    }

    private void OnTargetEntityHpChanged(int curHp, int maxHp)
    {
        Slider_Hp.value = (curHp / (float)maxHp);
    }

    private void OnTargetEntityMpChanged(int curMp, int maxMp)
    {
        Slider_Hp.value = (curMp / (float)maxMp);
    }

    private void Update()
    {
        // 참조형을 캐싱할때는 꼭! 널체크를 사용부에서 신경써주자
        if(_targetTransform != null)
        {
            //this.gameObject.transform.position = _targetTransform.position;

            // world 좌표 -> Screen 좌표 변환
            Vector2 screenPos = Camera.main.WorldToScreenPoint(_targetTransform.position);

            // UGUI에서 사용하려고
            var rectTransform = this.GetComponent<RectTransform>();
            if(rectTransform != null)
            {
                Vector2 finalScreenPos = new Vector2(screenPos.x, screenPos.y - SlotOffestY);
                rectTransform.anchoredPosition = finalScreenPos;
            }
        }
    }
}
