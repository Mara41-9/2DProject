using UnityEngine;

public class GoalPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") == true)
        {
            UIManager.Instance.OpenSuccessPopup();
        }
    }
   
}
