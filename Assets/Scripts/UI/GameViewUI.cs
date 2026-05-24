using TMPro;
using UnityEngine;

public class GameViewUI : MonoBehaviour
{
    [Header("플레이어 정보")]
    [SerializeField] private TMP_Text Text_PlayerName;
    [SerializeField] private TMP_Text Text_PlayerLevel;


    public void Start()
    {
        PlayerInfo();
    }

    private void PlayerInfo()
    {
        var player = GameDataManager.Instance.GetCharacterData("character_selly_01");
        if (player == null) return;

        Text_PlayerName.text = player.Name;

        var playerLevel = GameManager.Instance.GetPlayerLevel();
        Text_PlayerLevel.text = $"Lv.{playerLevel}";
    }
}
