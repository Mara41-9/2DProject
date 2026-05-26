using UnityEngine;

public class Monster2D : MonoBehaviour
{
    [SerializeField] private string _monsterName;

    public int _monsterInstanceId;
    private string _monsterDataId;

    [SerializeField] private SpriteRenderer SpriteRenderer_Monster;

    public Vector3 _moveDirection;   // 적이 이동할 방향 저장 변수

    private void Start()
    {
        RandomPickDirection();
    }

    private void Update()
    {
        SimpleEnemyMoveOnUpdate();
    }


    public void InitMonsterInfoOnCreated(int instanceId, string monsterDataId)
    {
        var monsterData = GameDataManager.Instance.GetMonsterData(monsterDataId);
        if(monsterData == null)
        {
            Debug.LogWarning($"유효하지 않은 몬스터 데이터 입니다! {monsterDataId}");
            return;
        }

        _monsterInstanceId = instanceId;
        _monsterDataId = monsterDataId;
    }

    public string GetFieldObjectDataId()
    {
        return _monsterDataId;
    }

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
        SpriteRenderer_Monster.flipX = (x < 0);
    }

    void SimpleEnemyMoveOnUpdate()
    {
        // 결정된 방향으로 매 프레임 이동
        transform.position += _moveDirection * 5.0f * Time.deltaTime;
    }

}
