using UnityEngine;

public class GoalPoint : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("GoalPoint Start");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") == true)
        {
            UIManager.Instance.OpenSuccessPopup();
        }
    }
   
}
