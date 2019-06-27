using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetTextColor : MonoBehaviour
{
    [SerializeField] Color back;
    [SerializeField] Color select;

    Text text;

    public void Set(bool isOn)
    {
        if (text == null) text = gameObject.GetComponent<Text>();

        text.color = isOn ? select : back;

        //text.color = new Color(text.color.r, text.color.g, text.color.b, (isOn) ? 1f : 0.4f);
    }
}
