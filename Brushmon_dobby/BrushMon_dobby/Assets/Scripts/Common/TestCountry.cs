using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class TestCountry : MonoBehaviour
{
    public Text text;

    private void Start()
    {
        text.text = NativePlugin.Instance.GetNativeCountryCode() + "/" + NativePlugin.Instance.GetNativeLanguageCode();
    }
}
