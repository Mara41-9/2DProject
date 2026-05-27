using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Monster2D : MonoBehaviour
{
    //[Header("몬스터 프리팹에서 미리 세팅할 데이터")]
    //public float SkillCoolTime;
    //public GameObject Prefab_MonsterSkillObject;

    [Header("몬스터 Id")]
    public int _monsterInstanceId;
    private string _monsterDataId;

    [Header("SpriteRenderer")]
    [SerializeField] private SpriteRenderer SpriteRenderer_Monster;

    [Header("몬스터 기본 정보")]
    public Vector3 _moveDirection;   // 적이 이동할 방향 저장 변수
    public int _baseHp;
    public int _baseAtk;
    public bool _isAlive = true;

    private MonsterData _monsterData;
    
    //private bool _lookRight = true;

    private Transform _leftPoint;
    private Transform _rightPoint;

    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void OnDisable()
    {
        _isAlive = false;
    }

    private void Start()
    {
        RandomPickDirection();
    }

    private void Update()
    {
        SimpleEnemyMoveOnUpdate();
    }

    public void SetMoveRange(Transform leftPoint, Transform rightPoint)
    {
        _leftPoint = leftPoint;
        _rightPoint = rightPoint;
    }


    public void InitMonsterInfoOnCreated(int instanceId, string monsterDataId)
    {
        var monsterData = GameDataManager.Instance.GetMonsterData(monsterDataId);
        if(monsterData == null)
        {
            Debug.LogWarning($"유효하지 않은 몬스터 데이터 입니다! {monsterDataId}");
            return;
        }

        _monsterData = monsterData;
        _baseHp = _monsterData.BaseHp;
        _baseAtk = _monsterData.BaseAtk;

        _monsterInstanceId = instanceId;
        _monsterDataId = monsterDataId;

        //StartCoroutine(CheckAndUseSkill());
    }

    public string GetMonsterDataId()
    {
        return _monsterDataId;
    }

    //private int GetFinalNormalAtkDamage(int baseAtk, float normalAtkMultiple)
    //{
    //    return GetFinalSkillDamage(baseAtk, normalAtkMultiple);
    //}

    //private int GetFinalSkillDamage(int baseAtk, float skillMultiple)
    //{
    //    return (int)(baseAtk * skillMultiple);
    //}

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
        if(_leftPoint == null || _rightPoint == null)
        {
            return;
        }

        // 결정된 방향으로 매 프레임 이동
        this.transform.position += _moveDirection * 2.0f * Time.deltaTime;

        // 몬스터가 왼쪽 경계보다 왼쪽까지 갔다면
        if(this.transform.position.x <= _leftPoint.position.x)
        {
            // 오른쪽으로 이동
            _moveDirection = Vector3.right;
            // 캐릭터 안 뒤집음 -> 오른쪽 보게 함
            SetMeshDirectionByMoveDirection(1);
        }
        // 몬스터가 오른쪽 경계보다 오른쪽까지 갔다면
        else if(this.transform.position.x >= _rightPoint.position.x)
        {
            // 왼쪽으로 이동
            _moveDirection = Vector3.left;
            // 캐릭터 뒤집음 -> 왼쪽 보게 함
            SetMeshDirectionByMoveDirection(-1);
        }
    }

    public void TakeDamage(int damage)
    {
        _baseHp -= damage;

        if (_baseHp <= 0)
        {
            _baseHp = 0;
            // 죽음 처리를 여기서 해두자
            MonsterDie();
        }
    }

    public void MonsterDie()
    {
        _isAlive = false;
        DelayDestroy(_monsterInstanceId);
    }

    // 몬스터 제거할 때 딜레이 걸 수 있도록
    // async -> 비동기 작업! (기다리는 작업)
    private async void DelayDestroy(int monsterId)
    {
        // 0.5초동안 기다려
        await System.Threading.Tasks.Task.Delay(500);

        GameObjectManager.Instance.DestroyMonster(monsterId);
    }


    // 코루틴이 등장한단는건 -> 유니태스크로 호환이 가능하다
    // 일정 시간마다 스킬을 사용할 예정
    // 스타트 코루틴은 이 몬스터가 생성된 시점에서 돌아도 됨!
    //IEnumerator CheckAndUseSkill()
    //{
    //    while(_isAlive)
    //    {
    //        yield return new WaitForSeconds(2.0f);

    //        if(_isAlive == false)
    //        {
    //            break;
    //        }

    //        UseSkill();
    //    }
    //}

    private void UseSkill()
    {
        //var gObj = Instantiate(Prefab_MonsterSkillObject, GameObjectManager.Instance.transform);
        //if (gObj == null) return;

        //var skillProjectileComponent = gObj.GetComponent<SkillProjectile>();
        //if(skillProjectileComponent == null) return;

        // // TODO : 추후 함수로 빠져야함
        //float skillMultiple = _monsterData.SkillAtkMultipleList.Count > 0 ? _monsterData.SkillAtkMultipleList[0] : 0;
        //int finalSkillDamage = GetFinalSkillDamage(_baseAtk, skillMultiple)
        //skillProjectileComponent.InitSkillObject(_lookRight, this.transform.position, finalSkillDamage);
    }
}
