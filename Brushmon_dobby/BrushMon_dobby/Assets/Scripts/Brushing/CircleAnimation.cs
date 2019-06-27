using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleAnimation : MonoBehaviour
{
    public Color color;
    public float size = 100f;
    public float Radius = 30f;
    public float time = 3f;
    public bool isReverse = false;

    // Use this for initialization
    void Start()
    {
        RectTransform target = transform.GetChild(0).GetComponent<RectTransform>();
        target.anchoredPosition = Vector2.right * Radius;
        target.GetComponent<Image>().color = color;
        target.sizeDelta = Vector2.one * size;

        if (isReverse == true)
        {
            iTween.RotateBy(gameObject, iTween.Hash("z", -1, "time", time, "easeType", "linear", "loopType", "loop"));
            iTween.RotateBy(target.gameObject, iTween.Hash("z", 1, "time", time, "easeType", "linear", "loopType", "loop"));
        }
        else
        {
            iTween.RotateBy(gameObject, iTween.Hash("z", 1, "time", time, "easeType", "linear", "loopType", "loop"));
            iTween.RotateBy(target.gameObject, iTween.Hash("z", -1, "time", time, "easeType", "linear", "loopType", "loop"));
        }
    }
}