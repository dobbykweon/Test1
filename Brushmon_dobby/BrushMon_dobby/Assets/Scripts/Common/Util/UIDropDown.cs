using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDropDown : Dropdown
{
    Transform tArrow;

    protected override void Start()
    {
        tArrow = transform.Find("Arrow");
        base.Start();
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        if (tArrow != null) tArrow.eulerAngles = Vector3.one;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        if (tArrow != null) tArrow.eulerAngles = Vector3.forward * 180;
    }

}
