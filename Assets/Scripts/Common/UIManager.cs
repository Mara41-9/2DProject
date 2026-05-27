using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class UIManager : UIBase
{
    // UI를 어느 레이어(층)에 띄울지 관리하기 위한 Canvas
    [SerializeField] Canvas Canvas_BgRoot;         // 배경 전용 - 맨 뒤에 깔리는 UI
    [SerializeField] Canvas Canvas_MainRoot;       // 메인 UI 전용 - 핵심 UI
    [SerializeField] Canvas Canvas_ContentRoot;    // 콘텐츠 전용 - 메인 UI 위에 크게 열리는 창들
    [SerializeField] Canvas Canvas_PopupRoot;      // 팝업 전용 - 일시적으로 뜨는 작은 창
    [SerializeField] Canvas Canvas_ToastRoot;      // 메시지 띄울 UI
    [SerializeField] Canvas Canvas_VeryFrontRoot;  // 가장 앞에 떠야 하는 UI - 로딩 화면

    public static UIManager Instance { get; set; }

    // 생성된 UI를 저장해두고 다시 열 때 재사용하기 위한 UI 캐시(Dictionary)
    private Dictionary<UIType, UIBase> _createdUIDic = new Dictionary<UIType, UIBase>();

    // 활성, 비활성에 관한 자료구조
    // HashSet -> 중복 방지 : 같은 UI를 두 번 열려고 해도 하나만 유지됨!
    private HashSet<UIType> _opendUIDic = new HashSet<UIType>();

    private void Awake()
    {
        // 현재 씬에 있는 UIManager를 전역 접근 가능하게 등록!
        // Awake -> 가장 먼저 실행되는 초기화 함수
        Instance = this;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "UI_Basic")
        {
            // 매니저들이 탄생한 후에 UI매니저가 처음으로 게임이 실행될 때 필요한 UI들을 오픈해준다!
            this.ShowStartupUIOnGameStart();
        }
        else
        {
            return;
        }

    }

    // UI를 열기 위한 함수
    public UIBase OpenUI(UIRootType uiRootType, UIType uiType, bool isInitialHide = false)
    {
        // 이미 생성된 UI 가져오기
        var openedUI = GetCreatedUI(uiRootType, uiType);

        // 열었을 때 기본적으로 숨겨서 열 것인지 체크
        bool isSetActiveOnOpen = (isInitialHide == false);

        // 이미 열려있는 UI인지 검사
        if(_opendUIDic.Contains(uiType) == false)
        {
            // 실제 UI를 켜거나 끔
            openedUI.gameObject.SetActive(isSetActiveOnOpen);
            _opendUIDic.Add(uiType);
        }

        // 열린 UI 반환
        return openedUI;

    }

    // UI를 닫기 위한 함수
    public void CloseUI(UIRootType uiRootType, UIType uiType)
    {
        // 열려있는 UI만 닫겠다
        if(_opendUIDic.Contains(uiType))
        {
            // 이미 만들어져 있는 UI 오브젝트를 가져온다
            var openedUI = _createdUIDic[uiType];
            // UI를 꺼라
            openedUI.gameObject.SetActive(false);

            // 열린 UI 목록에서 제거
            _opendUIDic.Remove(uiType);
        }
    }

    // UI가 생성될 부모(Canvas)를 찾아주는 함수
    private Transform GetRootTransform(UIRootType uiRootType)
    {
        // 반환할 부모 Transform 변수 생성 - 초기값: null
        Transform root = null;

        switch(uiRootType)
        {
            case UIRootType.BackgroundUI:
                root = Canvas_BgRoot.transform;
                break;
            case UIRootType.MainUI:
                root = Canvas_MainRoot.transform;
                break;
            case UIRootType.ContentUI:
                root = Canvas_ContentRoot.transform;
                break;
            case UIRootType.PopupUI:
                root = Canvas_PopupRoot.transform;
                break;
            case UIRootType.VeryFrontUI:
                root = Canvas_VeryFrontRoot.transform;
                break;
            case UIRootType.ToastUI:
                root = Canvas_ToastRoot.transform;
                break;
        }

        return root;

    }

    // UI를 생성하기 위한 함수
    private void CreateUI(UIRootType uiRootType, UIType uiType)
    {
        // 이 UI가 생성된 적 없다면...
        if (_createdUIDic.ContainsKey(uiType) == false)
        {
            // UI 프리팹 경로 가져오기
            string path = this.GetUIPath(uiRootType, uiType);

            // Resources 폴더에서 프리팹 불러오기
            // Resources.Load() -> 경로 기반으로 파일 로드, 반환 타입은 Object라서 캐스팅 필요
            GameObject loadedObj = (GameObject)Resources.Load(path);

            // UI를 어디 Canvas 밑에 생성할지 찾기
            Transform root = GetRootTransform(uiRootType);

            // root 밑에 불러온 프리팹을 실제 게임에 생성
            GameObject gObj = Instantiate(loadedObj, root);

            if (gObj != null)
            {
                // UIBase 컴포넌트 가져오기
                var uiBase = gObj.GetComponent<UIBase>();
                // 생성된 UI 저장
                _createdUIDic.Add(uiType, uiBase);
            }
        }
    }

    // 열려있는 애를 밖에서 사용하기 위해서 함수 구현 -> public
    public UIBase GetOpenedUI(UIRootType uiRootType, UIType uiType)
    {
        return GetCreatedUI(uiRootType, uiType);
    }

    // 생성된 UI를 가져오는 함수 - 이 함수 안에서만 사용하기 위해 private
    private UIBase GetCreatedUI(UIRootType uiRootType, UIType uiType)
    {
        // 이 UI가 아직 만들어져 있지 않다면
        if( _createdUIDic.ContainsKey(uiType) == false)
        {
            // 만들어라
            CreateUI(uiRootType, uiType);
        }

        // 딕셔너리에서 해당 UI 꺼내서 반환
        return _createdUIDic[uiType];
    }

    // MainUI 전용 Open 함수
    public UIBase OpenMainUI(UIType uiType)
    {
        return OpenUI(UIRootType.MainUI, uiType);
    }

    public void CloseMainUI(UIType uiType)
    {
        CloseUI(UIRootType.MainUI, uiType);
    }

    // ContentUI 전용 Open 함수
    public UIBase OpenContentUI(UIType uiType)
    {
        return OpenUI(UIRootType.ContentUI, uiType);
    }

    // PopupUI 전용 Open 함수
    public UIBase OpenPopupUI(UIType uiType)
    {
        return OpenUI(UIRootType.PopupUI, uiType);
    }

    // ContentUI 전용 Close 함수
    public void CloseContentUI(UIType uiType)
    {
        CloseUI(UIRootType.ContentUI, uiType);
    }

    // PopupUI 전용 Close 함수
    public void ClosePopupUI(UIType uiType)
    {
        CloseUI(UIRootType.PopupUI, uiType);
    }
}
