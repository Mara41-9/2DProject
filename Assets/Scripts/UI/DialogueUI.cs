using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : UIBase
{
    [SerializeField] private Text Text_Description;    // 대사 출력용 Text UI
    [SerializeField] private GameUIButton Btn_Next;    // 다음 버튼

    private string _currentDialogueId;    // 현재 진행중인 다이얼로그 ID
    private Queue<string> _descriptionQueue = new Queue<string>();    // 대사 보관 자료구조 큐

    public void OnEnable()
    {
        Btn_Next.BindOnClickButtonEvent(OnClick_Next);
        
    }

    public void OnClick_Next()
    {
        // 다음 대사가 있나
        bool isNextDescriptionExist = CheckAndSetDescription();
        // 아직 출력할 대사가 남아있다면
        if(isNextDescriptionExist)
        {
            // 현재 다이얼로그 안에서 계속 진행 -> 함수 종료
            return;
        }

        // 다음 다이얼로그가 있나
        bool isNextDialogueExist = CheckAndStartNextDialogue();
        // 다음 다이얼로그가 없다면
        if (isNextDialogueExist == false)
        {
            // 창 닫기
            UIManager.Instance.CloseContentUI(UIType.DialogueUI);
        }

    }

    // 다음 다이얼로그가 있는지 확인
    private bool CheckAndStartNextDialogue()
    {
        // 현재 다이얼로그 데이터 가져오기
        var dialogueData = GameDataManager.Instance.GetDialogueData(_currentDialogueId);
        if (dialogueData == null)
        {
            Debug.LogWarning($"다이얼로그 데이터가 존재하지 않습니다 {dialogueData}");
            return false;
        }

        // 현재 데이터를 기준으로 다음 다이얼로그가 있는지 체크해보고, 있다면 다음 다이얼로그를 시작
        string nextDialogueId = dialogueData.NextDialogueId;
        // 다음 다이얼로그 ID 존재하면
        if (string.IsNullOrEmpty(nextDialogueId) == false)
        {
            // 다음 다이얼로그 시작
            StartDialogue(nextDialogueId);
            return true;
        }

        return false;
    }

    // 다이얼로그 시작 함수
    public void StartDialogue(string dialogueId)
    {
        // ID로 데이터 가져오기
        var dialogueData = GameDataManager.Instance.GetDialogueData(dialogueId);
        if(dialogueData == null)
        {
            Debug.LogWarning($"다이얼로그 데이터가 존재하지 않습니다 {dialogueData}");
            return;
        }

        // 현재 진행중인 다이얼로그 Id는 다음 다이얼로그가 있는지 체크할 때 쓸 수 있도록 보관
        _currentDialogueId = dialogueId;

        // 만약 대사에 <np> 키워드가 있다면
        if (dialogueData.Description.Contains("<np>"))
        {
            // <np> 기준으로 문자열 분리
            string[] dialogueDescriptionList = dialogueData.Description.Split("<np>");
            // 분리된 문자열 하나씩 반복
            foreach(string desc in dialogueDescriptionList)
            {
                // 데이터를 큐에 순서대로 저장
                _descriptionQueue.Enqueue(desc);
            }

            // 다음 대사 출력
            CheckAndSetDescription();
        }
        else
        {
            // Np 태그가 없다면 바로 다이얼로그 UI에 출력
            SetCurrentDialogueDescription(dialogueData.Description);
        }

       
    }

    // 다음 대사 출력
    private bool CheckAndSetDescription()
    {
        // 큐에 남은 대사가 있나
        bool isNextDescriptionExsist = (_descriptionQueue.Count > 0);
        // 있다면
        if(isNextDescriptionExsist)
        {
            // 큐의 맨 앞 대사 꺼내기
            string desc = _descriptionQueue.Dequeue();
            // UI에 출력
            SetCurrentDialogueDescription(desc);
        }

        return isNextDescriptionExsist;
    }

    // 실제 UI Text 변경 함수
    private void SetCurrentDialogueDescription(string description)
    {
        // 화면에 대사 출력
        Text_Description.text = description;
    }
}
