using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectManager : MonoBehaviour
{
    [SerializeField] private Transform Root_Enemy;

    public static GameObjectManager Instance { get; set; }

    // 생성된 오브젝트의 키
    private int _objectInstanceKeyGenerator;

    // 생성된 오브젝트의 생명을 보관
    private Dictionary<int, FieldObject2D> _fieldObjectContainer = new Dictionary<int, FieldObject2D>();
    private Dictionary<int, Monster2D> _monsterContainer = new Dictionary<int, Monster2D>();

    private PlayerMovement _localPlayer;

    private void Awake()
    {
        Instance = this;
    }

    // 등록
    public void RegisterLocalPlayer(PlayerMovement localplayer)
    {
        _localPlayer = localplayer;
    }

    // 가져오기
    // 프로퍼티 기능이 있긴 하지만, 그래도 그 프로퍼티를 직접 참조하는 것보다는 Get 함수를 한정적으로 사용하는 것이 좋음
    public PlayerMovement GetLocalPlayer()
    {
        if(_localPlayer == null)
        {
            Debug.LogError("등록된 플레이어가 없는데, 참조하려고 시도하고 있습니다!!");
            return null;
        }

        return _localPlayer;
    }


    //[필드 오브젝트] ====================================================================================================

    // 필드 오브젝트를 생성하는 비동기 함수
    public async UniTaskVoid CreateFieldObject(string fieldObjectDataId, Transform spawnSpot)
    {
        // GameDataManager에서 FieldObject 데이터 가져오기
        var fieldObject = GameDataManager.Instance.GetFieldObjectData(fieldObjectDataId);
        if (fieldObject != null)
        {
            // 어드레서블 기반 비동기 생성
            // fieldObject.PrefabPath: 생성할 프리팹 주소, Root_Enemy: 생성된 오브젝트 부모, true: 월드 좌표 유지
            var createdObj = await ResourceManager.Instance.InstantiateAsync(fieldObject.PrefabPath, Root_Enemy, true);
            // 생성된 오브젝트의 위치 설정
            createdObj.transform.position = spawnSpot.position;
            // 생성된 오브젝트를 관리 시스템에 등록
            AddFieldObjectOnCreate(createdObj, fieldObjectDataId);
        }
    }

    // 생성 완료된 필드 오브젝트를 관리 컨테이너에 등록
    private void AddFieldObjectOnCreate(GameObject createdObject, string fieldObjectDataId)
    {
        // 생성된 오브젝트 키 증가
        _objectInstanceKeyGenerator++;
        // 현재 생성된 고유 ID 저장
        var generatedInstanceId = _objectInstanceKeyGenerator;
        // 생성된 오브젝트에서 FieldObject2D 컴포넌트 가져오기
        var fieldObject = createdObject.GetComponent<FieldObject2D>();

        if (fieldObject != null)
        {
            _fieldObjectContainer.Add(generatedInstanceId, fieldObject);
            // 생성된 오브젝트 내부 데이터 초기화
            fieldObject.InitFieldObjectInfoOnCreated(generatedInstanceId, fieldObjectDataId);
        }
    }

    // 특정 instanceId를 가진 필드 오브젝트 제거
    public void RequestDestroyFieldObject(int instanceId)
    {
        // instanceId로 실제 오브젝트 찾기
        var fieldObjectComponent = GetFieldObjectByInstanceId(instanceId);
        if (fieldObjectComponent == null)
        {
            return;
        }

        // 딕셔너리에서 해당 오브젝트 제거
        _fieldObjectContainer.Remove(instanceId);
        // 씬에 있는 실제 오브젝트도 삭제
        Destroy(fieldObjectComponent.gameObject);
    }

    // 인스턴스 ID로 오브젝트 찾기
    public FieldObject2D GetFieldObjectByInstanceId(int fieldObjectInstanceId)
    {
        // _fieldObjectContainer 안에 해당 키가 없으면
        if (_fieldObjectContainer.ContainsKey(fieldObjectInstanceId) == false)
        {
            Debug.LogError($"{fieldObjectInstanceId} 찾으려는 필드 오브젝트가 유효하지 않습니다");
            return null;
        }

        return _fieldObjectContainer[fieldObjectInstanceId];
    }



    //[몬스터] ====================================================================================================

    // 몬스터를 생성하는 비동기 함수
    public async UniTaskVoid CreateMonster(string monsterDataId, Transform spawnSpot, Transform leftPoint, Transform rightPoint, SpawnSpot dropItemSpawnSpot)
    {
        // GameDataManager에서 Monster 데이터 가져오기
        var monster = GameDataManager.Instance.GetMonsterData(monsterDataId);
        if (monster == null)
        {
            Debug.LogError($"몬스터 데이터 없음 : {monsterDataId}");
            return;
        }

        Debug.Log($"몬스터 데이터를 가져왔다! : {monsterDataId}");

        // 어드레서블 기반 비동기 생성
        // fieldObject.PrefabPath: 생성할 프리팹 주소, Root_Enemy: 생성된 오브젝트 부모, true: 월드 좌표 유지
        var createdObj = await ResourceManager.Instance.InstantiateAsync(monster.PrefabPath, Root_Enemy, true);
        if (createdObj == null)
        {
            Debug.LogError("몬스터 생성 실패");
            return;
        }

        Debug.Log($"몬스터가 생성됐다! : {createdObj.name}");

        // 생성된 오브젝트의 위치 설정
        createdObj.transform.position = spawnSpot.position;
        // 생성된 오브젝트를 관리 시스템에 등록
        AddMonsterOnCreate(createdObj, monsterDataId);

        var monsterComponent = createdObj.GetComponent<Monster2D>();
        if (monsterComponent == null)
        {
            return;
        }

        monsterComponent.SetMoveRange(leftPoint, rightPoint);
        monsterComponent.SetDropItemSpawnSpot(dropItemSpawnSpot);

    }


    // 생성 완료된 필드 오브젝트를 관리 컨테이너에 등록
    private void AddMonsterOnCreate(GameObject createdMonster, string monsterDataId)
    {
        // 생성된 오브젝트 키 증가
        _objectInstanceKeyGenerator++;
        // 현재 생성된 고유 ID 저장
        var generatedInstanceId = _objectInstanceKeyGenerator;
        // 생성된 오브젝트에서 FieldObject2D 컴포넌트 가져오기
        var monster = createdMonster.GetComponent<Monster2D>();

        if (monster != null)
        {
            _monsterContainer.Add(generatedInstanceId, monster);
            // 생성된 오브젝트 내부 데이터 초기화
            monster.InitMonsterInfoOnCreated(generatedInstanceId, monsterDataId);
        }
    }

    // 인스턴스 ID로 오브젝트 찾기
    public Monster2D GetMonsterByInstanceId(int monsterInstanceId)
    {
        // _fieldObjectContainer 안에 해당 키가 없으면
        if (_monsterContainer.ContainsKey(monsterInstanceId) == false)
        {
            Debug.LogError($"{monsterInstanceId} 찾으려는 필드 오브젝트가 유효하지 않습니다");
            return null;
        }

        return _monsterContainer[monsterInstanceId];
    }

    public Dictionary<int, Monster2D> GetMonsterByList()
    {
        return _monsterContainer;
    }

    public void DestroyMonster(int monsterInstanceId)
    {
        if(_monsterContainer.TryGetValue(monsterInstanceId, out Monster2D monster))
        {
            _monsterContainer.Remove(monsterInstanceId);

            Destroy(monster.gameObject);

            Debug.Log($"몬스터 제거 완료 : {monsterInstanceId}");
        }
        else
        {
            Debug.LogError($"몬스터를 찾을 수 없음 : {monsterInstanceId}");
        }

    }

    // 생성된 모든 몬스터를 제거하는 함수
    public void DestroyMonsterAll()
    {
        foreach(var monsterKv in _monsterContainer)
        {
            var monster = monsterKv.Value;
            if (monster == null) return;

            Destroy(monster.gameObject);
        }

        _monsterContainer.Clear();
    }

}
