using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINoticePopup : MonoBehaviour
{
    [SerializeField] Toggle toggle;

    public void Close()
    {
        if(toggle.isOn == true)
        {
            PlayerPrefs.SetInt("notic_popup_show", 1);
        }

        Destroy(gameObject);
    }

}
