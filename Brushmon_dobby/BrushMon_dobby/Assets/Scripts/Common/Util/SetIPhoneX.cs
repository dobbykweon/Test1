using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetIPhoneX : MonoBehaviour {

    public RectTransform rectTransform;

    public bool isPad = false;
    public bool isIPhoneX = false;

    // Use this for initialization
    void Start()
    {
        if(BMUtil.Instance.IsiPhoneX() == true && isIPhoneX == true)
            rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, rectTransform.offsetMax.y - 100);
    }
	
}
