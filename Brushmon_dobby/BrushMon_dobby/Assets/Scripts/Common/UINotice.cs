using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINotice : MonoBehaviour
{
    [SerializeField] GameObject item;

    [SerializeField] GameObject objList;
    [SerializeField] GameObject objMsg;

    private void Start()
    {
        item.SetActive(DateTime.Now < new DateTime(2019, 7, 7));
    }

    public void BtnBack()
    {
        if(objMsg.activeInHierarchy == true)
        {
            objMsg.SetActive(false);
            objList.SetActive(true);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SelectItem()
    {
        objMsg.SetActive(true);
        objList.SetActive(false);
    }
}
