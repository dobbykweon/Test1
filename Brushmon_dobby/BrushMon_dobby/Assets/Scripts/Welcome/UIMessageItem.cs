using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PushMessageType
{
    None = 0,
    Normal = 1,
    Link = 2,
}

public class UIMessageItem : MonoBehaviour {

    private Message_Data data;

    public Text txtTitle;
    public Text txtMessage;

    Action resetPosition;

    public void SetMsg(Message_Data _data, Action _resetPosition)
    {
        data = _data;
        resetPosition = _resetPosition;

        txtTitle.text = data.title;
        txtMessage.text = data.msg;
    }

    public void OnClickMessage()
    {
        LogManager.Log("OnClickMessage(" + data.type + ") : " + data.title + "/" + data.msg);

        if (data.isNew == true)
        {
            data.isNew = false;
            MessageManager.Instance.SaveData();
        }
        
        switch (data.type)
        {
            case PushMessageType.None:
            case PushMessageType.Normal:
                BMUtil.Instance.OpenDialog(data.title, data.msg, "닫기", "삭제", "", true, true, false, null, delegate { RemoveMessage(); }, null);
                break;
            case PushMessageType.Link:
                BMUtil.Instance.OpenDialog(data.title, data.msg, "닫기", "삭제", "자세히", true, true, true, null, delegate { RemoveMessage(); }, delegate { if (!string.IsNullOrEmpty(data.link)) Application.OpenURL("http://www.naver.com"/*data.link*/); });
                break;
        }
    }

    void RemoveMessage()
    {
        MessageManager.Instance.RemoveData(data);
        BMUtil.Destroy(gameObject);
        resetPosition();
    }
}
