using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineBrain CinemachineBrain_Main;

    public static CameraManager Instance { get; private set; }

    private Camera _mainCamera;

    private void Awake()
    {
        Instance = this;

        // 하이어라키에 있는 기본 Main Camera 받아와짐
        _mainCamera = Camera.main;
    }

    public Camera GetMainCamera()
    {
        return _mainCamera;
    }

    public CinemachineBrain GetMainCinemachineBrain()
    {
        if(CinemachineBrain_Main == null)
        {
            var gObj = GameObject.Find("MainCamera");
            if(gObj != null)
            {
                var brain = gObj.GetComponent<CinemachineBrain>();
                if(brain != null)
                {
                    CinemachineBrain_Main = brain; 
                }
            }
        }

        return CinemachineBrain_Main;
    }
}
