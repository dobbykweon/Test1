using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPos : MonoBehaviour {

    public bool isPad = false;
    public bool isIPhoneX = false;

    public int posX;
    public int posY;



    // Use this for initialization
    void Start()
    {
#if UNITY_IOS
        //UnityEngine.UI.CanvasScaler canvasScaler = GameObject.Find("UI").GetComponent<UnityEngine.UI.CanvasScaler>();

        float screenBaseRatio = 1080f / 1920f;
        float screenRatio = (float)Screen.width / (float)Screen.height;

        Debug.Log(screenBaseRatio +" / "+ screenRatio);

        RectTransform rectTransform = transform.GetComponent<RectTransform>();

        if (Mathf.Abs(screenBaseRatio - screenRatio) > 0.01f)
        {
            if (screenBaseRatio < screenRatio)
            {
                if(isPad == true)
                    rectTransform.anchoredPosition += (Vector2.right * posX + Vector2.up * posY);
            }
            else
            {
                if(isIPhoneX == true)
                    rectTransform.anchoredPosition += (Vector2.right * posX + Vector2.up * posY);
            }
        }
#endif
    }
}
