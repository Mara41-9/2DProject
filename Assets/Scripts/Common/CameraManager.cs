using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineBrain CinemachineBrain_Main;
    [SerializeField] private CinemachineBrainEvents CinemachineBrainEvenets_Main;

    public static CameraManager Instance { get; private set; }

    private Camera _mainCamera;

    private void Awake()
    {
        Instance = this;

        // 하이어라키에 있는 기본 Main Camera 받아와짐
        _mainCamera = Camera.main;

        if (CinemachineBrain_Main == null)
        {
            var gObj = GameObject.Find("Main Camera");
            if (gObj != null)
            {
                var brain = gObj.GetComponent<CinemachineBrain>();
                if (brain != null)
                {
                    CinemachineBrain_Main = brain;
                }

                var brainEvents = gObj.GetComponent<CinemachineBrainEvents>();
                if (brainEvents != null)
                {
                    CinemachineBrainEvenets_Main = brainEvents;
                }
            }
        }
    }

    public Camera GetMainCamera()
    {
        return _mainCamera;
    }

    public CinemachineBrain GetMainCinemachineBrain()
    {
        return CinemachineBrain_Main;
    }

    public void BindMainCameraUpdatedEvent(Action<CinemachineBrain> callbackEvent)
    {
        CinemachineBrainEvenets_Main.BrainUpdatedEvent.AddListener(callbackEvent.Invoke);
    }

    public void UnBindMainCameraUpdatedEvent(Action<CinemachineBrain> callbackEvent)
    {
        CinemachineBrainEvenets_Main.BrainUpdatedEvent.RemoveListener(callbackEvent.Invoke);
    }
}
