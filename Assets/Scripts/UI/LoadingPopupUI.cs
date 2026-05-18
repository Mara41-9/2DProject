using UnityEngine;
using UnityEngine.UI;

public class LoadingPopupUI : MonoBehaviour
{
    [SerializeField] private Image Img_Loading;

    private void Start()
    {
        Sprite loadedSprite = Resources.Load<Sprite>("0_Nara_Resource/Image/");
    }
}
