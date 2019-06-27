using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetScreenSize : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        Debug.Log("SetScreenSize");
        UnityEngine.UI.CanvasScaler canvasScaler = GameObject.Find("UI").GetComponent<UnityEngine.UI.CanvasScaler>();
        float screenBaseRatio = 1200f/1920f;
        float screenRatio = (float)Screen.width / (float)Screen.height;

        canvasScaler.matchWidthOrHeight = (screenBaseRatio >= screenRatio) ? 0 : 1;
    }
}
