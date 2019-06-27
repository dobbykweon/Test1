using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetTxt : MonoBehaviour
{
    Text text;

    // Use this for initialization
    void Start()
    {
        text = gameObject.GetComponent<Text>();
    }

    public void Set(float value)
    {
        text.text = value.ToString();
    }
}
