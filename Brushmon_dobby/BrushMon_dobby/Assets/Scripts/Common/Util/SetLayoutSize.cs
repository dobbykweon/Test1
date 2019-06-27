using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetLayoutSize : MonoBehaviour {

    [SerializeField] List<RectTransform> rects;

    [SerializeField] bool isWidth = false;
    [SerializeField] bool isHeight = false;

    LayoutElement layoutElement;

    Vector2 size;

    private void Start()
    {
        layoutElement = GetComponent<LayoutElement>();

        size = Vector2.zero;


    }

    private void Update()
    {
        for (int i = 0; i < rects.Count; i++)
        {
            LogManager.Log(rects[i].name + " : " + rects[i].sizeDelta);

            if (size.x < rects[i].sizeDelta.x)
                size.x = rects[i].sizeDelta.x;

            if (size.y < rects[i].sizeDelta.y)
                size.y = rects[i].sizeDelta.y;
        }

        LogManager.Log("size : " + size);

        if (isWidth)
        {
            layoutElement.preferredWidth = size.x + 20;
        }

        if (isHeight)
        {
            layoutElement.preferredHeight = size.y + 20;
        }
    }
}
