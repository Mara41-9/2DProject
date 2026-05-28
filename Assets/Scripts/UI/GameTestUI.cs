using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameTestUI : MonoBehaviour
{
    // [SerializeField] private DaniTechUIButton Button_BBB;

    [SerializeField] private SpawnSpot SpawnSpot_Monster;

    [SerializeField] private GameUIButton Btn_Exit;
    [SerializeField] private GameUIButton Btn_Inventory;

    [SerializeField] private Text Text_CurrentHp;


    public void OnEnable()
    {
        Btn_Exit.BindOnClickButtonEvent(OnClick_ExitButton);
        Btn_Inventory.BindOnClickButtonEvent(OnClick_InventoryBtn);
        
    }

    public void OnClick_SelectTestBtn()
    {
        SpawnSpot_Monster.StartSpawn();
    }

    public void OnClick_InventoryBtn()
    {
        UIManager.Instance.OpenPopupUI(UIType.InventoryPopup);
    }

    private void OnClick_ExitButton()
    {
        GameManager.Instance.RefreshGame();
        Debug.LogWarning("로비메인UI로 이동합니다.");
        UIManager.Instance.OpenLoadingUI();
        UIManager.Instance.OpenLobbyUI();
    }

    public void PlayerHp(int currentHp)
    {
        Text_CurrentHp.text = $"플레이어 HP : {currentHp}";
    }
}
