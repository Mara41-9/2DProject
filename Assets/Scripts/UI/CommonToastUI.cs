using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommonToastUI : UIBase
{
    [SerializeField] private Image Image_Bg;
    [SerializeField] private Image Image_Frame;
    [SerializeField] private TMP_Text Text_Message;

    public void SetMessage(string message)
    {
        Text_Message.text = message;
        StartCoroutine(CloseAfterDelay());
    }

    private IEnumerator CloseAfterDelay()
    {
        yield return new WaitForSeconds(2.0f);
        UIManager.Instance.CloseCommonToastUI();
    }
}
