using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


// 어떤 컴포넌트가 필수로 필요하다는 것을 강제
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("플레이어의 초기 위치")]
    [SerializeField] private Vector2 _playerPosition;

    [Header("이동 설정")]
    [SerializeField] private float _moveSpeed = 8f;   // 움직임 속도
    [SerializeField] private float _jumpForce = 15f;  // 점프 힘

    // 지면 체크를 안 하면 적, 아이템 등도 바닥으로 인식할 수 있음!
    [Header("지면 체크 설정")]
    [SerializeField] private Transform _groundCheck;     // 발 밑에 배치할 빈 오브젝트
    [SerializeField] private float _checkRadius = 0.5f;  // 체크 범위
    [SerializeField] private LayerMask _groundLayer;     // 지면으로 인식할 레이어 - 어떤 오브젝트를 바닥으로?

    [Header("애니메이터")]
    [SerializeField] private EntityAnimController AnimatorController_Entity;

    [Header("공격 설정")]
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private float _attackRadius = 1f;
    [SerializeField] private LayerMask _monsterLayer;

    [Header("스킬")]
    [SerializeField] public GameObject _skill;

    private Rigidbody2D _rigidbody;
    private bool _isGrounded;
    private float _horizontalInput;  // 플레이어의 좌우 입력값을 저장하는 변수
    private bool _lookRight = true;

    private int _currentScore;

    [Header("HP")]
    public int _maxHp;     // 최대 Hp
    public int _currentHp;  // 현재 Hp 

    [Header("공격력")]
    public int _baseAtk;

    private bool _isAttack;

    // 이벤트 선언 - HP/MP 변경 알림 함수를 저장해둘 공간
    private event Action<int, int> _onHpChanged;
    private event Action<int, int> _onMpChanged;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        // 2D 캐릭터가 물리 충돌 시, 회전해서 넘어지는 것 방지
        // constraints : 움직임 제한 설정
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Start()
    {
        var player = GameDataManager.Instance.GetCharacterData("Character_Toto_01");
        if (player == null)
        {
            return;
        }

        _maxHp = player.Hp;
        _currentHp = _maxHp;

        // 나 스스로를 등록한다. -> 씬에 있는 그 2D 플레이어가 등록됨
        GameObjectManager.Instance.RegisterLocalPlayer(this);

        // 플레이어가 생성되면 AddHudSlot 함수 호출 - 일단 쉽게 0번 등록
        UIManager.Instance.AddHudSlot(0, this.gameObject.transform);

        // 플레이어의 시작 위치를 초기 위치로 저장
        _playerPosition = this.gameObject.transform.position;

        _skill.gameObject.SetActive(false);

        
    }

    private void Update()
    {
        // 좌우 입력값 받아서 _horizontalInput 변수에 저장
        // A키 / <- 방향키 = -1 , 입력 없음 = 0 , D 키 / → 방향키 = 1
        _horizontalInput = Input.GetAxisRaw("Horizontal");

        // _horizontalInput가 0이 아니면 움직이는 중!
        bool isMoving = (_horizontalInput != 0);

        // 점프 입력
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            Jump();
        }
        
        if(_isAttack == false)
        {
            if (_isGrounded == false)
            {
                ChangePlayerState(EntityAnimState.Jump);
            }
            else if(isMoving)
            {
                ChangePlayerState(EntityAnimState.Walk);
            }
            else
            {
                ChangePlayerState(EntityAnimState.Idle);
            }
        }

        // 캐릭터 방향 전환 
        if (_horizontalInput > 0 && !_lookRight)
        {
            Flip();
        }
        else if (_horizontalInput < 0 && _lookRight)
        {
            Flip();
        }
    }

    // 물리 연산 전용 함수
    private void FixedUpdate()
    {
        // Physics2D.OverlapCircle : 원 모양 범위 안에 특정 오브젝트가 있는지 검사
        //                            -> 반환값 : bool
        // (원의 중심 위치, 검사 범위 크기, 어떤 레이어)
        _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _checkRadius, _groundLayer);

        // Rigidbody 이동은 FixedUpdate 함수에서!
        Move();
        
    }

    // 현재 장착한 무기의 공격력을 플레이어 기본 공격력에 적용하는 함수 
    public void UpdateBaseAtk()
    {
        var equippedWeaponDataId = GameManager.Instance.GetEquippedWeapon();
        if (string.IsNullOrEmpty(equippedWeaponDataId)) return;

        var equippedWeaponData = GameDataManager.Instance.GetWeaponData(equippedWeaponDataId);
        if (equippedWeaponData == null) return;

        _baseAtk = equippedWeaponData.BaseAtk;
    }

    // 플레이어 위치 초기 위치 반환
    public Vector2 GetPlayerPosition()
    {
        return _playerPosition;
    }

    private void ChangePlayerState(EntityAnimState newState)
    {
        AnimatorController_Entity.SetState(newState);
    }

    private void Move()
    {
        // Y축 속도는 유지, X축 속도만 변경 -> 좌우 이동!
        // _rigidbody.linearVelocity : Rigidbody2D의 현재 속도
        _rigidbody.linearVelocity = new Vector2(_horizontalInput * _moveSpeed, _rigidbody.linearVelocity.y);
    }

    private void Jump()
    {
        // X축 속도 유지, 위쪽 속도를 점프 힘으로 변경
        _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, _jumpForce);

    }


    // 캐릭터 방향 반대로 뒤집는 함수
    private void Flip()
    {
        _lookRight = !_lookRight;                // true -> false , false -> true
        Vector3 scaler = transform.localScale;   // 현재 오브젝트의 크기 정보 가져오기 (Scale)
        scaler.x *= -1;                          // Unity에서 Scale X가 음수가 되면 스프라이트가 좌우 반전
        transform.localScale = scaler;           // 마지막으로 바뀐 값을 실제 오브젝트에 적용
    }


    // 공격 범위 안에 있는 몬스터들을 찾아서 제거하는 함수
    public void Attack()
    {
        // 원 범위 안에 들어온 Collider들을 전부 찾아라
        Collider2D[] hitMonsters = Physics2D.OverlapCircleAll(_attackPoint.position, _attackRadius, _monsterLayer);
        StartCoroutine(AttackRoutine());

        foreach(Collider2D enemy in hitMonsters)
        {
            // Collider가 붙어있는 오브젝트에서 Monster2D 스크립트 가져오자
            Monster2D monster = enemy.GetComponent<Monster2D>();
            if (monster == null) return;

            // 각 몬스터의 고유 ID 저장
            int id = monster._monsterInstanceId;

            var monsterData = GameDataManager.Instance.GetMonsterData(monster.GetMonsterDataId());
            if(monsterData == null) return;

            monster.TakeDamage(_baseAtk);
            Debug.LogWarning($"토토가 {monsterData.Name}에게 {_baseAtk}만큼 데미지를 입혔다!    {monsterData.Name}의 Hp : {monster._currentHp}");

        }
    }

    public void TakeDamage(int damage)
    {
        _currentHp -= damage;
        InvokeStatChangedEvent();

        if(_currentHp <= 0)
        {
            _currentHp = 0;
            UIManager.Instance.OpenGameOverPopup();
            return;
        }
    }

    private IEnumerator AttackRoutine()
    {
        _isAttack = true;

        ChangePlayerState(EntityAnimState.Atk);

        yield return new WaitForSeconds(1.1f);

        _isAttack = false;

        if (_isAttack == false)
        {
            ChangePlayerState(EntityAnimState.Idle);
        }
    }

    private void OnDrawGizmos()
    {
        if(_groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_groundCheck.position, _checkRadius);
        }

        if(_attackPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_attackPoint.position, _attackRadius);
        }
    }

    // 스탯이 바꼈을 때 실행할 함수를 등록하는 함수 - 이벤트 등록
    public void BindOnStatChangedEvent(Action<int, int> hpChangeCallback, Action<int, int> mpChangeCallback)
    {
        // _onHpChanged 이벤트에 함수 추가
        _onHpChanged += hpChangeCallback;
        // _onMpChanged 이벤트에 함수 추가
        _onMpChanged += mpChangeCallback;
    }

    public void ResetBindStatChangedEvent()
    {
        _onHpChanged = null;
        _onMpChanged = null;
    }

    // 등록된 이벤트 함수들을 실행시키는 함수
    public void InvokeStatChangedEvent()
    {
        // 우선 HP든 MP든 하나라도 바뀌면 다 호출해준다
        _onHpChanged?.Invoke(_currentHp, _maxHp);
       // _onMpChanged?.Invoke(_currentMp);
    }

    public void AddHp(int hp)
    {
        if((_maxHp - _currentHp) < hp)
        {
            _currentHp = _maxHp;
            InvokeStatChangedEvent();
            return;
        }

        _currentHp += hp;
        // 기존의 스탯 변경이 됐으므로 함수 호출해주자
        InvokeStatChangedEvent();
    }

}
