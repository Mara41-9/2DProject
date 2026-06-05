using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommonToastUI : UIBase
{
    [SerializeField] private Image Image_Bg;
    [SerializeField] private Image Image_Frame;
    [SerializeField] private TMP_Text Text_Message;
    [SerializeField] private RectTransform ToastUI_Position;

    public void SetPosition(Vector2 pos)
    {
        // anchoredPosition : 앵커(Anchor)를 기준으로 한 위치
        ToastUI_Position.anchoredPosition = pos;
    }

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
