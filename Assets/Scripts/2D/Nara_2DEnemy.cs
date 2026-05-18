using UnityEngine;

public class Nara_2DEnemy : MonoBehaviour
{
    [SerializeField] private SpriteRenderer SpriteRenderer_Enemy;

    public int EntityInstanceId { get; private set; }  // 적 객체의 고유 ID

    private Vector3 _moveDirection;   // 적이 이동할 방향 저장 변수

    // 게임 시작 시 1번 실행
    private void Start()
    {
        RandomPickDirection();
    }

    // 매 프레임 실행
    private void Update()
    {
        SimpleEnemyMoveOnUpdate();
    }

    // 적 초기화 함수
    public void InitEnemyInfo(int instanceId)
    {
        EntityInstanceId = instanceId;
    }

    // 랜덤 방향 결정 함수
    void RandomPickDirection()
    {
        // 랜덤값이 0이면 -1, 0이 아니면 1
        float randomX = Random.Range(0, 2) == 0 ? -1f : 1f;
        // 왼쪽 또는 오른쪽 방향 벡터 생성
        _moveDirection = new Vector3(randomX, 0, 0);
        SetMeshDirectionByMoveDirection((int)_moveDirection.x);
    }

    void SetMeshDirectionByMoveDirection(int x)
    {
        // + 디테일을 살리기 위해 방향에 따라 캐릭터 리소스를 뒤집는다
        // 역시 중요한 로직은 아니다!
        SpriteRenderer_Enemy.flipX = (x < 0);
    }

    // 실제 이동 처리 함수
    void SimpleEnemyMoveOnUpdate()
    {
        // 결정된 방향으로 매 프레임 이동
        transform.position += _moveDirection * 5.0f * Time.deltaTime;
    }
}
