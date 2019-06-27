using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGColor : MonoBehaviour {

    // Use this for initialization
    private void Start()
    {
        Image gradationImage = GetComponent<Image>();

        gradationImage.material.SetColor("_StartColor", GetColor(0x17, 0x7d, 0x8c, 0xff));
        gradationImage.material.SetColor("_EndColor", GetColor(0x15, 0x31, 0x62, 0xff));
    }

    Color GetColor(float r, float g, float b, float a)
    {
        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }
}
