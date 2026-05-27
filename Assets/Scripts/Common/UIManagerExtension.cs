using UnityEngine;

// "UI를 어떤 Canvas Root에 생성할 것인가"를 구분!
public enum UIRootType
{
    None = 0,
    BackgroundUI,
    MainUI,
    ContentUI,
    PopupUI,
    ToastUI,
    VeryFrontUI,
}

public enum UIType
{
    GameStartUI,
    LobbyMainUI,
    ProfilePopup,
    SkillPopup,
    QuestPopup,
    InventoryPopup,
    CommonToastUI,
    GameViewUI,
    PausePopup,
    LoadingUI,
    DialogueUI,
    SuccessPopup,
    GameOverPopup,
}

public static partial class UIManagerExtension
{
    // this UIManager uiManager -> 이 함수는 UIManager 전용 확장 함수다!
    // UIType → Resources 경로(string)로 변환하는 함수
    public static string GetUIPath(this UIManager uiManager, UIRootType uiRootType, UIType uiType)
    {
        string path = string.Empty; // "" == string.Empty

        // Resources.Load를 할 경로를 직접 명시
        path = $"Prefabs/UI/{uiRootType}/{uiType}";
        return path;
    }

    public static void ShowStartupUIOnGameStart(this UIManager uiManager)
    {
        uiManager.OpenLoadingUI();
        uiManager.OpenGameStartUI();
    }

    public static void OpenGameStartUI(this UIManager uiManager)
    {
        var uiBase = uiManager.OpenMainUI(UIType.GameStartUI);
        if(uiBase == null)
        {
            Debug.LogWarning($"UI가 생성되지 않았습니다");
            return;
        }
    }
    
    public static void CloseGameStartUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRootType.MainUI, UIType.GameStartUI);
    }

    public static void OpenLobbyMainUI(this UIManager uiManager)
    {
        var uiBase = uiManager.OpenMainUI(UIType.LobbyMainUI);
        if(uiBase == null)
        {
            Debug.LogWarning($"UI가 생성되지 않았습니다");
            return;
        }
    }

    public static void CloseLobbyMainUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRootType.MainUI, UIType.LobbyMainUI);
    }

    public static void OpenCommonToastUI(this UIManager uiManager)
    {
        var uiBase = uiManager.OpenUI(UIRootType.ToastUI, UIType.CommonToastUI);
        if( uiBase == null)
        {
            Debug.LogWarning($"UI가 생성되지 않았습니다");
            return;
        }
    }

    public static void CloseCommonToastUI( this UIManager uiManager)
    {
        uiManager.CloseUI(UIRootType.ToastUI, UIType.CommonToastUI);
    }

    public static void OpenPropilePopup(this UIManager uiManager)
    {
        // 팝업 UI 가져오기 (없으면 생성까지 자동)
        var uiBase = uiManager.OpenPopupUI(UIType.ProfilePopup);

        if (uiBase == null)
        {
            Debug.LogWarning($"UI가 생성되지 않았습니다");
            return;
        }
    }

    public static void OpenSkillPopup(this UIManager uiManager)
    {
        var uiBase = uiManager.OpenPopupUI(UIType.SkillPopup);

        if (uiBase == null)
        {
            Debug.LogWarning($"UI가 생성되지 않았습니다");
            return;
        }
    }

    public static void OpenQuestPopup(this UIManager uiManager)
    {
        var uiBase = uiManager.OpenPopupUI(UIType.QuestPopup);

        if (uiBase == null)
        {
            Debug.LogWarning($"UI가 생성되지 않았습니다");
            return;
        }
    }

    public static void OpenInventoryPopup(this UIManager uiManager)
    {
        var uiBase = uiManager.OpenPopupUI(UIType.InventoryPopup);

        if (uiBase == null)
        {
            Debug.LogWarning($"UI가 생성되지 않았습니다");
            return;
        }
    }


    public static void OpenLoadingUI(this UIManager uiManager)
    {
        var uiBase = uiManager.OpenUI(UIRootType.VeryFrontUI, UIType.LoadingUI);
        if(uiBase == null)
        {
            Debug.LogWarning($"UI가 생성되지 않았습니다");
            return;
        }
    }

    public static void CloseLoadingUI(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRootType.VeryFrontUI, UIType.LoadingUI);
    }

    public static void OpenGameViewUI(this UIManager uiManager)
    {
        var uiBase = uiManager.OpenContentUI(UIType.GameViewUI);
        if( uiBase == null)
        {
            Debug.LogWarning($"UI가 생성되지 않았습니다");
            return;
        }
    }

    public static void CloseGameViewUI(this UIManager uiManager)
    {
        uiManager.CloseContentUI(UIType.GameViewUI);
    }

    public static void OpenPausePopup(this UIManager uiManager)
    {
        var uiBase = uiManager.OpenPopupUI(UIType.PausePopup);
        if(uiBase == null)
        {
            Debug.LogWarning($"UI가 생성되지 않았습니다");
            return;
        }
    }

    public static void ClosePausePopup(this UIManager uiManager)
    {
        uiManager.ClosePopupUI(UIType.PausePopup);
    }

    public static void OpenDialogueUI(this UIManager uiManager, string startDialogueId)
    {
        var uiBase = uiManager.OpenContentUI(UIType.DialogueUI);
        if( uiBase == null)
        {
            Debug.LogWarning($"UI가 생성되지 않았습니다");
            return;
        }

        if(uiBase is DialogueUI dialogueUI)
        {
            dialogueUI.StartDialogue(startDialogueId);
        }
    }

    public static void OpenSuccessPopup(this UIManager uiManager)
    {
        var uiBase = uiManager.OpenPopupUI(UIType.SuccessPopup);
        if(uiBase == null)
        {
            Debug.LogWarning($"UI가 생성되지 않았습니다");
            return;
        }
    }

    public static void CloseSuccessPopup(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRootType.PopupUI, UIType.SuccessPopup);
    }

    public static void OpenGameOverPopup(this UIManager uiManager)
    {
        var uiBase = uiManager.OpenPopupUI(UIType.GameOverPopup);
        if(uiBase == null)
        {
            Debug.LogWarning($"UI가 생성되지 않았습니다");
            return;
        }
    }

    public static void CloseGameOverPopup(this UIManager uiManager)
    {
        uiManager.CloseUI(UIRootType.PopupUI, UIType.GameOverPopup);
    }
}
