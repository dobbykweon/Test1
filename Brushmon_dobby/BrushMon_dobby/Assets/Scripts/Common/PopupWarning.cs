using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupWarning : MonoBehaviour {

    public Toggle toggle;

    public void Close()
    {

        PlayerPrefs.SetInt("warning_check", (toggle.isOn) ? 1 : 0);
        BMUtil.Destroy(gameObject);
    }
}
