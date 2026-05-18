using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private Text Text_CurrentScore;

    public void AddGameScore(int currentScore)
    {
        Text_CurrentScore.text = $"잡은 돼지 수 : {currentScore}";

        GameManager.Instance.IncreasePlayerExp(10);
        

        if(currentScore == 20)
        {
            Debug.LogWarning("저장 시도!");
            GameManager.Instance.SaveData();
            UIManager.Instance.OpenSuccessPopup();
        }

    }

    
}
