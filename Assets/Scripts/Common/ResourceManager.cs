using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    // 딕셔너리 - 로드한 에셋들의 정보를 저장해둠
    // AsyncOperationHandle : 어드레서블이 에셋 로드하면 반환하는 객체
    // AsyncOperation (비동기) : 게임은 계속 돌아가, 에셋들은 백그라운드에서 로드!
    private Dictionary<string, AsyncOperationHandle> _handles = new Dictionary<string, AsyncOperationHandle>();


    // 원하는 타입의 에셋을 비동기로 로드 -> 로드 완료되면 callback으로 넘겨주는 함수
    // <T> / 제네릭 T : 아직 타입이 정해지지 않은 임시 타입
    // string address : 어드레서블 주소
    // System.Action<T> callback : T 타입 값을 전달받는 함수
    // where T : UnityEngine.Object : T는 UnityEngine.Object를 상속한 타입만 가능 -> Sprite, GameObject, AudioClip...
    // 제한하는 이유 : 어드레서블은 유니티 에셋만 로드하기 때문!
    public void LoadAsset<T>(string address, System.Action<T> callback) where T : UnityEngine.Object
    {
        // _handles 딕셔너리에 address 키가 존재하는지 확인
        // 있다 -> true 반환 -> handle 변수 안에 값 넣어줌
        // 없다 -> false 반환
        if (_handles.TryGetValue(address, out AsyncOperationHandle handle))
        {
            // callback이 null이 아니면 callback 함수 실행하면서 handle.Result(로드된 실제 에셋 결과)를 T(타입)으로 변환
            callback?.Invoke(handle.Result as T);
            return;
        }

        // 어드레서블 시스템으로 에셋을 비동기 로드하고, 그 로드 작업 정보를 loadHandle에 저장
        AsyncOperationHandle<T> loadHandle = Addressables.LoadAssetAsync<T>(address);

        // 에셋 로드가 끝날 때(Completed) 실행할 처리 (로드 끝났을 때 이 코드 실행해줘!)
        // Completed 이벤트에 함수 추가
        // op : 로드 완료된 operation 정보 -> 상태(Status), 결과(Result) 등이 들어있음
        loadHandle.Completed += (op) =>
        {
            // 만약 로드 성공했다면
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                // 로드 성공한 에셋 정보를 address 이름으로 _handles Dictionary에 저장
                // _handles.Add(address, op) 와 같음
                _handles[address] = op;
                // callback이 null 아니면 callback 함수 실행하면서 op.Result(로드 완료된 에셋 결과)를 전달
                callback?.Invoke(op.Result);
            }
            else
            {
                Debug.LogError($"에셋 로드 실패: {address}");
            }
        };
    }


    public async UniTask<T> LoadAsset<T>(string address) where T : UnityEngine.Object
    {
        // 1. 이미 로드된 에셋인지 확인 (캐싱 확인)
        if (_handles.TryGetValue(address, out AsyncOperationHandle handle))
        {
            // 이미 완료된 핸들이라면 즉시 결과 반환
            return handle.Result as T;
        }

        // 2. 어드레서블 로드 실행 (UniTask로 변환)
        // ToUniTask()를 사용하여 await 가능하게 만듭니다.
        AsyncOperationHandle<T> loadHandle = Addressables.LoadAssetAsync<T>(address);

        try
        {
            T result = await loadHandle.ToUniTask();

            // 3. 핸들 저장 (성공 시)
            _handles[address] = loadHandle;
            return result;
        }
        catch (System.Exception e)
        {
            // 4. 실패 시 예외 처리
            Debug.LogError($"에셋 로드 실패: {address} / Error: {e.Message}");

            // 실패한 핸들도 메모리 해제가 필요할 수 있으므로 상황에 따라 Release 처리
            if (loadHandle.IsValid())
                Addressables.Release(loadHandle);

            return null;
        }
    }


    // 어드레서블 프리팹을 비동기로 생성하는 함수
    // Transform parent = null : 부모를 안 넣어도 된다
    public async UniTask<GameObject> InstantiateAsync(string address, Transform parent = null, bool instantiateInWorldSpace = false)
    {
        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(address, parent, instantiateInWorldSpace);

        try
        {
            GameObject instance = await handle.ToUniTask();
            return instance;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"프리팹 생성 실패: {address} / Error: {e.Message}");

            if(handle.IsValid())
            {
                Addressables.Release(handle);
            }

            return null;
        }
    }


    // 스프라이트를 로드하는 함수
    // System.Action<Sprite> callback : 로드 끝나면 Sprite를 넘겨주면서 실행할 함수
    public void LoadSprite(string address, System.Action<Sprite> callback)
    {
        // 이미 로드한 스프라이트인가?
        if(_handles.TryGetValue(address, out AsyncOperationHandle handle))
        {
            // 이미 있다면 -> 저장된 Sprite를 callback으로 바로 넘겨줌
            callback?.Invoke(handle.Result as Sprite);
            return;
        }

        // address에 있는 Sprite를 비동기로 로드 시작 -> 그 정보를 handleOrigin에 저장
        AsyncOperationHandle<Sprite> handleOrigin = Addressables.LoadAssetAsync<Sprite>(address);

        handleOrigin.Completed += (op) =>
        {
            if(op.Status == AsyncOperationStatus.Succeeded)
            {
                _handles[address] = op;
                callback?.Invoke(op.Result);
            }
            else
            {
                Debug.LogError($"스프라이트 로드 실패: {address}");
            }
        };
    }


    public async UniTask<Sprite> LoadSprite(string address)
    {
        // 1. 이미 로드된 스프라이트인지 확인 (캐시 활용)
        if (_handles.TryGetValue(address, out AsyncOperationHandle handle))
        {
            // 결과가 Sprite인지 확인 후 반환
            return handle.Result as Sprite;
        }

        // 2. 스프라이트 형식으로 로드 실행
        AsyncOperationHandle<Sprite> handleOrigin = Addressables.LoadAssetAsync<Sprite>(address);

        try
        {
            // ToUniTask()를 통해 비동기 대기
            Sprite result = await handleOrigin.ToUniTask();

            // 3. 핸들 저장 (나중에 Release하기 위함)
            _handles[address] = handleOrigin;

            return result;
        }
        catch (System.Exception)
        {
            // 4. 로드 실패 시 처리
            Debug.LogError($"스프라이트 로드 실패: {address}");

            if (handleOrigin.IsValid())
                Addressables.Release(handleOrigin);

            return null;
        }
    }


    // 로드했던 어드레서블 에셋을 메모리에서 해제하는 함수
    public void Release(string address)
    {
        // _handles 딕셔너리에 address 키가 존재? -> 있으면 handle 변수 안에 값 넣어
        if (_handles.TryGetValue(address, out AsyncOperationHandle handle))
        {
            // 어드레서블 메모리 해제
            Addressables.Release(handle);
            // 딕셔너리에서도 제거
            _handles.Remove(address);
            Debug.Log($"에셋 메모리 해제 완료 : {address}");
        }
    }
}
