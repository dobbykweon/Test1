using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TweenInputField : InputField
{
    Vector3 firstPos;
    GameObject objTweenTarget;
    float uiScale;

    Action actionSelect = null;
    Action actionDeselete = null;

    protected override void Start()
    {
        firstPos = transform.parent.position;
        objTweenTarget = transform.parent.gameObject;
        uiScale = BMUtil.Instance.UIRoot.localScale.x;
    }

    public void SetSelectAction(Action select, Action deselect)
    {
        actionSelect = select;
        actionDeselete = deselect;
    }

    public override void OnSelect(BaseEventData eventData)
    {
        TouchScreenKeyboard.hideInput = true;

        base.OnSelect(eventData);
        PlayTween(true);

        if (actionSelect != null) actionSelect();
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        PlayTween(false);

        if (actionDeselete != null) actionDeselete();
    }

    void PlayTween(bool isFront)
    {
        if (isFront)
        {
            iTween.MoveTo(objTweenTarget, firstPos + Vector3.up * 400 * uiScale, 0.3f);
        }
        else
        {
            iTween.MoveTo(objTweenTarget, firstPos, 0.3f);
        }
    }

    public void EnablePassword(bool isView)
    {
        if (isView)
            contentType = ContentType.Alphanumeric;
        else
            contentType = ContentType.Password;

        UpdateLabel();
    }
}
