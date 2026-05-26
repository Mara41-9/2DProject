using UnityEngine;

// 스폰 스팟이 어떤 역할을 하는지
public enum SpawnSpotType
{
    None = 0,
    Harvest,    // 수확물 생성
    DropItem,   // 아이템 생상
    Dialogue,   // 다이얼로그 발생 
    Monster     // 몬스터 생성
}

// 언제 실행할지
public enum StartSpawnType
{
    None = 0,
    OnAwake,    // 씬 로드되자마자 실행
    OnEnable,   // 활성화 되면 실행
    OnRange,    // 플레이어가 범위 안에 들어왔을 때 실행
}

public class SpawnSpot : MonoBehaviour
{
    [SerializeField] private SpawnSpotType _spawnSpotType;
    [SerializeField] private StartSpawnType _startSpawnType;

    [SerializeField] private string _spawnObjectDataId;          // 생성할 데이터 ID
    [SerializeField] private Collider2D Collider_OnSpawnStart;   // 플레이어 감지용 트리거 콜라이더 - OnRange일때만!

    [SerializeField] private Transform _leftPoint;
    [SerializeField] private Transform _rightPoint;

    private void Awake()
    {
        // OnAwake 타입이면 게임 시작하자마자 스폰
        if (_startSpawnType == StartSpawnType.OnAwake)
        {
            StartSpawn();
        }
    }

    private void Start()
    {
        // OnEnable 타입이면 스폰
        if (_startSpawnType == StartSpawnType.OnEnable)
        {
            StartSpawn();
        }

        // 콜라이더가 실제로 존재하면
        if(Collider_OnSpawnStart != null)
        {
            // enabled : 콜라이더 활성화? 비활성화?
            Collider_OnSpawnStart.enabled = (_startSpawnType == StartSpawnType.OnRange);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") == true)
        {
            StartSpawn();
        }
    }


    public void StartSpawn()
    {
        switch (_spawnSpotType)
        {
            // 비동기로 필드 오브젝트 생성
            case SpawnSpotType.Harvest:
            case SpawnSpotType.DropItem:
                GameObjectManager.Instance.CreateFieldObject(_spawnObjectDataId, this.transform).Forget();
                this.gameObject.SetActive(false);   // 생성 후 비활성화 -> 중복 생성 방지 
                break;
            case SpawnSpotType.Monster:
                GameObjectManager.Instance.CreateMonster(_spawnObjectDataId, this.transform, _leftPoint, _rightPoint).Forget();
                this.gameObject.SetActive(false);
                break;
            case SpawnSpotType.Dialogue:
                UIManager.Instance.OpenDialogueUI(_spawnObjectDataId);
                this.gameObject.SetActive(false);
                break;
        }
    }
}
