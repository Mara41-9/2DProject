using UnityEngine;

public class HudSlotUI : MonoBehaviour
{
    [SerializeField] private int SlotOffestY;

    private int _instanceId;

    // 참조형을 기록(캐싱)
    private Transform _targetTransform;

    public void InitSlot(int instanceId, Transform targetTransform)
    {
        _instanceId = instanceId;
        _targetTransform = targetTransform;
        SlotOffestY = 120;
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
