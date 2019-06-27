using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMainLogo : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        if (Screen.safeArea.width != Screen.width || Screen.safeArea.height != Screen.height)
        {
            RectTransform rect = gameObject.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y - 40);
        }
    }
}
