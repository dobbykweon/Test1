using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetLayout : MonoBehaviour {

    [SerializeField] RectTransform target;

    [SerializeField] bool isWidth = false;
    [SerializeField] bool isHeight = false;

    [SerializeField] int left;
    [SerializeField] int right;
    [SerializeField] int top;
    [SerializeField] int bottom;

    LayoutElement layout;

    // Use this for initialization
    void Start()
    {
        layout = transform.GetComponent<LayoutElement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isWidth)
        {
            layout.preferredWidth = target.sizeDelta.x + left + right;
        }

        if (isHeight)
        {
            layout.preferredHeight = target.sizeDelta.y + top + bottom;
        }
    }
}
