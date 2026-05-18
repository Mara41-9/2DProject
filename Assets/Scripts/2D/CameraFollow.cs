using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("따라갈 플레이어")]
    [SerializeField] private Transform Prefab_Player;

    [Header("카메라 이동 속도")]
    [SerializeField] private float _moveSpeed = 5f;

    [Header("카메라 거리값")]
    [SerializeField] private Vector3 _offset = new Vector3(8f, 3.5f, -10f);


    private void LateUpdate()
    {
        StartMove();
        
    }

    private void StartMove()
    {

        Vector3 targetPos = _offset + Prefab_Player.position;

        this.transform.position = Vector3.Lerp(transform.position, targetPos, _moveSpeed * Time.deltaTime);

    }
}
