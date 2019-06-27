using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetDropdownItemLabel : MonoBehaviour {

    [SerializeField] Color defaultColor;
    [SerializeField] Color selectColor;

    Text txtLabel;
    Toggle toggle;

	// Use this for initialization
	void Start () {

        Set();
        SetSelect(toggle.isOn);
    }
	
    void Set()
    {
        if (txtLabel == null) txtLabel = transform.GetComponent<Text>();
        if (toggle == null) toggle = transform.parent.GetComponent<Toggle>();
    }

    void SetSelect(bool isSelect)
    {
        if (isSelect)
        {
            txtLabel.color = selectColor;
        }
        else
        {
            txtLabel.color = defaultColor;
        }
    }

}
