using System.Collections.Generic;
using System.Threading;
using UnityEngine;


// 어떤 컴포넌트가 필수로 필요하다는 것을 강제
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("플레이어의 위치")]
    [SerializeField] private Vector2 _playerPosition = new Vector2(-17f, -1f);

    [Header("이동 설정")]
    [SerializeField] private float _moveSpeed = 8f;   // 움직임 속도
    [SerializeField] private float _jumpForce = 7f;  // 점프 힘

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

    // 우선 직접 들고 있다가 추후에 UI매니저한테 요청하도록 개선해볼 것
    [SerializeField] private ScoreUI _scoreUI;
    [SerializeField] private GameTestUI _gameTestUI;

    private Rigidbody2D _rigidbody;
    private bool _isGrounded;
    private float _horizontalInput;  // 플레이어의 좌우 입력값을 저장하는 변수
    private bool _lookRight = true;

    private int _currentScore;
    public int _currentHp; //{ get; private set; }

    public Vector2 PlayerPosition { get; private set; }

    private HashSet<int> _hitMonsters = new HashSet<int>();   // 중복을 허용하지 않기 위해 HashSet 사용

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        // 2D 캐릭터가 물리 충돌 시, 회전해서 넘어지는 것 방지
        // constraints : 움직임 제한 설정
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Start()
    {
        // 나 스스로를 등록한다. -> 씬에 있는 그 2D 플레이어가 등록됨
        GameObjectManager.Instance.RegisterLocalPlayer(this);

        this.transform.position = _playerPosition;

        var player = GameDataManager.Instance.GetCharacterData("Character_Toto_01");
        if (player == null)
        {
            return;
        }

        _currentHp = player.Hp;
    }

    private void Update()
    {
        // 좌우 입력값 받아서 _horizontalInput 변수에 저장
        // A키 / <- 방향키 = -1 , 입력 없음 = 0 , D 키 / → 방향키 = 1
        _horizontalInput = Input.GetAxisRaw("Horizontal");

        // 점프 입력
        if(Input.GetButtonDown("Jump") && _isGrounded)
        {
            Jump();
        }

        // _horizontalInput가 0이 아니면 움직이는 중!
        bool isMoving = (_horizontalInput != 0);

        // 컨트롤 키를 누르면
        if (Input.GetKey(KeyCode.LeftControl))
        {
            // 공격해라
            ChangePlayerState(EntityAnimState.Atk);
            Attack();
        }
        else  // 그게 아니라면
        {
            // 걷거나 가만히 있어라
            ChangePlayerState(isMoving ? EntityAnimState.Walk : EntityAnimState.Idle);
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

    private void FixedUpdate()
    {
        // Physics2D.OverlapCircle : 원 모양 범위 안에 특정 오브젝트가 있는지 검사
        //                            -> 반환값 : bool
        // (원의 중심 위치, 검사 범위 크기, 어떤 레이어)
        _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _checkRadius, _groundLayer);

        // Rigidbody 이동은 FixedUpdate 함수에서!
        Move();
        
    }

    public Vector2 GetPlayerPosition()
    {
        PlayerPosition = _playerPosition;
        return PlayerPosition;
    }

    public void SetPlayerHp(int playerHp)
    {
        _currentHp = playerHp;

        if(_gameTestUI != null)
        {
            _gameTestUI.PlayerHp(_currentHp);
        }
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
    private void Attack()
    {
        // 원 범위 안에 들어온 Collider들을 전부 찾아라
        Collider2D[] hitMonsters = Physics2D.OverlapCircleAll(_attackPoint.position, _attackRadius, _monsterLayer);

        foreach(Collider2D enemy in hitMonsters)
        {
            // Collider가 붙어있는 오브젝트에서 Monster2D 스크립트 가져오자
            Monster2D monster = enemy.GetComponent<Monster2D>();

            // 각 몬스터의 고유 ID 저장
            int id = monster._monsterInstanceId;

            // 이미 맞은 몬스터면 스킵 (반복문 나가라)
            if(_hitMonsters.Contains(id))
            {
                continue;
            }

            // 맞은 몬스터는 리스트에 등록
            _hitMonsters.Add(id);

            if (monster != null)
            {
                DelayDestroy(id);
            }
        }
    }

    // 몬스터 제거할 때 딜레이 걸 수 있도록
    // async -> 비동기 작업! (기다리는 작업)
    private async void DelayDestroy(int monsterId)
    {
        // 0.5초동안 기다려
        await System.Threading.Tasks.Task.Delay(500);

        GameObjectManager.Instance.DestroyMonster(monsterId);
        AddGameScore();

        _hitMonsters.Remove(monsterId);
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

    // 적 충돌 시 처리하는 함수
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") == false)
        {
            return;
        }

        var monsterComponent = collision.gameObject.GetComponent<Monster2D>();

        if(monsterComponent != null)
        {
            if((monsterComponent._moveDirection.x * this.transform.localScale.x) < 0)
            {
                _currentHp -= 5;

                Debug.LogWarning($"셀리 공주의 남은 Hp: {_currentHp}");
                if(_gameTestUI != null)
                {
                    _gameTestUI.PlayerHp(_currentHp);
                }

                if(_currentHp <= 0)
                {
                    UIManager.Instance.OpenGameOverPopup();
                }
            }

        }
        else
        {
            Debug.Log($"충돌한 적 객체에서 컴포넌트를 찾을 수 없습니다 : {gameObject.name}");
            return;
        }


        //// 플레이어의 콜리전에 충돌한 객체가 Enemy 태그가 아니라면
        //if(collision.gameObject.CompareTag("Enemy") == false)
        //{
        //    return;
        //}

        //// 충돌한 몬스터의 정보를 받아오자
        //var enemyComponent = collision.gameObject.GetComponent<Monster2D>();

        //if(enemyComponent == null)
        //{
        //    Debug.Log($"충돌한 적 객체에서 컴포넌트를 찾을 수 없습니다 : {gameObject.name}");
        //    return;
        //}

        //if(_isAttacking)
        //{
        //    // 충돌된 오브젝트를 플레이어가 직접 제거하는게 아니라, Id로 게임오브젝트 매니저한테 삭제 요청
        //    GameObjectManager.Instance.DestroyMonster(enemyComponent._monsterInstanceId);
        //    // 피그마를 잡으면 스코어를 올려주자!
        //    AddGameScore();
        //}

    }

    private void AddGameScore()
    {
        _currentScore++;
        _scoreUI.AddGameScore(_currentScore);
        
        
    }
}
