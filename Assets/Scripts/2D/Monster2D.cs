using System;
using System.Collections;
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
    private Vector2 _monsterPosition;
    public int _currentHp;
    public int _maxHp;     // 최대 Hp
    public int _baseAtk;
    public bool _isAlive = true;

    [Header("공격 설정")]
    [SerializeField] private Transform _attackRange;
    [SerializeField] private float _attackRadius = 1.5f;
    [SerializeField] private LayerMask _PlayerLayer;

    [Header("애니메이터")]
    [SerializeField] private EntityAnimController AnimatorController_Entity;

    private MonsterData _monsterData;
    
    //private bool _lookRight = true;

    private Transform _leftPoint;
    private Transform _rightPoint;

    private Rigidbody2D _rigidbody;

    private bool _isAttack = false;

    private event Action<int, int> _onHpChanged;
    private event Action<int, int> _onMpChanged;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void OnDisable()
    {
        _isAlive = false;
        ResetBindStatChangedEvent();
    }

    private void Start()
    {
        RandomPickDirection();
        ChangeMonsterState(EntityAnimState.Walk);
        _monsterPosition = this.gameObject.transform.position;
    }

    private void Update()
    {
        SimpleEnemyMoveOnUpdate();

        if(_isAttack == false)
        {
            CheckInAttackRange();
        }
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
        _maxHp = _monsterData.BaseHp;
        _currentHp = _maxHp;
        _baseAtk = _monsterData.BaseAtk;
        _monsterInstanceId = instanceId;
        _monsterDataId = monsterDataId;

        UIManager.Instance.AddHudSlot(instanceId, this.gameObject.transform);

        //StartCoroutine(CheckAndUseSkill());
    }

    public void ResetMonster()
    {
        // 몬스터 Hp 초기화 시키기
        _currentHp = _maxHp;
        _isAlive = true;
        ChangeMonsterState(EntityAnimState.Walk);
        InvokeStatChangedEvent();

        // 몬스터 위치 초기화 시키기
        this.gameObject.transform.position = _monsterPosition;
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
        float randomX = UnityEngine.Random.Range(0, 2) == 0 ? -1f : 1f;
        // 왼쪽 또는 오른쪽 방향 벡터 생성
        _moveDirection = new Vector3(randomX, 0, 0);
        SetMeshDirectionByMoveDirection((int)_moveDirection.x);
    }

    void SetMeshDirectionByMoveDirection(int x)
    {
        // + 디테일을 살리기 위해 방향에 따라 캐릭터 리소스를 뒤집는다
        // 역시 중요한 로직은 아니다!
        // flipX = true -> 스프라이트 좌우반전
        SpriteRenderer_Monster.flipX = (x < 0);
    }

    void SimpleEnemyMoveOnUpdate()
    {
        if(_leftPoint == null || _rightPoint == null)
        {
            return;
        }

        if(_isAlive ==  false)
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

    private void CheckInAttackRange()
    {
        Collider2D hitPlayer = Physics2D.OverlapCircle(_attackRange.position, _attackRadius, _PlayerLayer);
        if (hitPlayer == null) { return; }

        if (hitPlayer.CompareTag("Player") == false) { return; }

        var player = GameObjectManager.Instance.GetLocalPlayer();
        if(player == null) { return; }

        float directionCheckValue = player.transform.localScale.x * this.transform.position.x;
        if(directionCheckValue < 0)
        {
            SetMeshDirectionByMoveDirection(-1);
        }

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        Vector3 originDirection = _moveDirection;
        _moveDirection = Vector3.zero;

        var player = GameObjectManager.Instance.GetLocalPlayer();
        if (player != null)
        {
            _isAttack = true;
            ChangeMonsterState(EntityAnimState.Atk);

            yield return new WaitForSeconds(0.6f);
            player.TakeDamage(_baseAtk);
            _isAttack = false;

            if(_isAttack == false)
            {
                _moveDirection = originDirection;
                // 공격 전 원래 바라보던 방향으로!
                SetMeshDirectionByMoveDirection((int)_moveDirection.x);
                ChangeMonsterState(EntityAnimState.Walk);
            }
        }
    }

    public void TakeDamage(int damage)
    {
         _currentHp -= damage;
        InvokeStatChangedEvent();

        if (_currentHp <= 0)
        {
            _currentHp = 0;
            
            MonsterDie();
            return;
        }

        StartCoroutine(DamageRoutine());

    }

    public void MonsterDie()
    {
        _isAlive = false;

        StartCoroutine(DieRoutine());
    }

    private void ChangeMonsterState(EntityAnimState newState)
    {
        AnimatorController_Entity.SetState(newState);
    }

    // 잠깐 피격 상태로 됐다가 다시 걷기 상태로 돌아가는 작업 (실행했다가 잠깐 멈췄다가 다시 이어서 실행 가능)
    // -> 피격 애니메이션을 잠깐만 보여주기 위해
    private IEnumerator DamageRoutine()
    {
        // 원래 방향대로 다시 이동시켜야 하므로 변수 선언
        Vector3 originDirection = _moveDirection;
        // 이동 멈추기
        _moveDirection = Vector3.zero;

        // 몬스터 애니메이션 상태 : Damaged
        ChangeMonsterState(EntityAnimState.Damaged);

        // 0.2초 기다려라
        yield return new WaitForSeconds(0.7f);

        if(_isAlive)
        {
            _moveDirection = originDirection;
            // 살아있으면 다시 Walk 상태로 변경
            ChangeMonsterState(EntityAnimState.Walk);
        }
    }

    private IEnumerator DieRoutine()
    {
        // 이동 멈추기
        _moveDirection = Vector3.zero;

        ChangeMonsterState(EntityAnimState.Die);

        yield return new WaitForSeconds(0.6f);

        GameObjectManager.Instance.DestroyMonster(_monsterInstanceId);
        UIManager.Instance.RemoveHudSlot(_monsterInstanceId);
    }

    public void BindOnStatChangedEvent(Action<int, int> hpChangeCallback, Action<int, int> mpChangeCallback)
    {
        _onHpChanged += hpChangeCallback;
        _onHpChanged += mpChangeCallback;
    }

    public void ResetBindStatChangedEvent()
    {
        _onHpChanged = null;
        _onMpChanged = null;
    }

    public void InvokeStatChangedEvent()
    {
        // 우선 HP든 MP든 하나라도 바뀌면 다 호출해준다
        _onHpChanged?.Invoke(_currentHp, _maxHp);
        // _onMpChanged?.Invoke(_currentMp);
    }

    private void OnDrawGizmos()
    {
        if(_attackRange != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_attackRange.position, _attackRadius);
        }
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
