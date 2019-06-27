using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialItem : MonoBehaviour
{
    public void SetButton(RectTransform rect, Action action)
    {
        Button btn = transform.Find("Button").GetComponent<Button>();
        btn.onClick.AddListener(delegate { action(); });

        RectTransform btnRect = btn.GetComponent<RectTransform>();

        btnRect.anchorMax = rect.anchorMax;
        btnRect.anchorMin = rect.anchorMin;

        btnRect.anchoredPosition = rect.anchoredPosition;
        btnRect.sizeDelta = rect.sizeDelta;
    }
}
