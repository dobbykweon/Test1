using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MessageData
{
    public string key;
    public string value;
}

[Serializable]
public class MessageDataList
{
    public List<MessageData> data;
}

[Serializable]
public class Message_Data
{
    public bool isNew = true;
    public PushMessageType type;
    public string title;
    public string msg;
    public string link;
}

[Serializable]
public class Message_Data_List
{
    public List<Message_Data> datatList;
}

public class MessageManager : GlobalMonoSingleton<MessageManager>
{
    #region FMC 토큰 등록 관련
    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        LogManager.Log("Received Registration Token: " + token.Token);

        BMUtil.Instance.SetFcmToken(token.Token);

        Firebase.Messaging.FirebaseMessaging.Subscribe("/topics/All");
#if UNITY_IOS
		Firebase.Messaging.FirebaseMessaging.Subscribe ("/topics/IOS");
#elif UNITY_ANDROID
        Firebase.Messaging.FirebaseMessaging.Subscribe("/topics/Android");
#endif
        TrackingManager.Instance.Tracking("firebase_token", token.Token);

        SetScribeLanguage();
        SetFcmToken(token.Token);
    }

    public void SetScribeLanguage()
    {
        switch (Application.systemLanguage)
        {
            case SystemLanguage.Korean:
                Firebase.Messaging.FirebaseMessaging.Subscribe("/topics/KR");
                break;

            case SystemLanguage.English:
                Firebase.Messaging.FirebaseMessaging.Subscribe("/topics/EN");
                break;

            case SystemLanguage.ChineseTraditional:
                Firebase.Messaging.FirebaseMessaging.Subscribe("/topics/TW");
                break;

            case SystemLanguage.Chinese:
            case SystemLanguage.ChineseSimplified:
                Firebase.Messaging.FirebaseMessaging.Subscribe("/topics/CN");
                break;

            case SystemLanguage.Japanese:
                Firebase.Messaging.FirebaseMessaging.Subscribe("/topics/JP");
                break;

            default:
                Firebase.Messaging.FirebaseMessaging.Subscribe("/topics/EN");
                break;
        }
    }

    public void SetFcmToken(string fcm_token)
    {
#if !UNITY_EDITOR
#if !OFFLINE
        StartCoroutine(SendFcmToken(fcm_token));
#endif
#endif
    }

    IEnumerator SendFcmToken(string fcm_token)
    {
        TokenVo tokenVo = null;

        while (tokenVo == null)
        {
            tokenVo = ConfigurationData.Instance.GetValueFromJson<TokenVo>("Token");
            yield return null;
        }

        RestService.Instance.GetFcmTokenList(tokenVo,
            result =>
            {
                //BMUtil.Instance.OpenDialog("FcmToken", result);
                LogManager.Log("FcmToken : " + result);
#if !UNITY_EDITOR
                List<FcmTokenVo> fcmTokenList = JsonHelper.getJsonArray<FcmTokenVo>(result);

                bool isRegister = true;

                for (int i = 0; i < fcmTokenList.Count; i++)
                {
                    if (fcmTokenList[i].token.Equals(fcm_token))
                    {
                        isRegister = false;
                        break;
                    }
                }

                if (isRegister)
                {
                    FcmToken fcmToken = new FcmToken();
                    fcmToken.fcm_token = fcm_token;

                    RestService.Instance.RegisterFcmToken(tokenVo, fcmToken,
                        delegate
                        {
                            LogManager.Log("RegisterFcmToken Success");
                        },
                        exception =>
                        {
                            LogManager.Log("RegisterFcmToken Exception : " + exception.ToString());
                        }
                    );
                }
#endif
            },
            exception =>
            {
                LogManager.LogError("GetFcmTokenList exception : " + exception.ToString());
            }
        );
    }

#endregion

#region 메시지 수신 관련

    Message_Data_List data_list;
    public Message_Data_List Data_List
    {
        get
        {
            if (data_list == null) data_list = LoadData();
            return data_list;
        }
    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        //BMUtil.Instance.OpenDialog("Message", "DataCount : " + e.Message.Data.Count);

        if (e.Message.Notification != null)
        {
            LogManager.Log("=========== Received a FirebaseNotification ===========" + "\n" +
                "NotificationOpened : " + e.Message.NotificationOpened + "\n" +
                "Title : " + e.Message.Notification.Title + "\n" +
                "Body : " + e.Message.Notification.Body + "\n" +
                "Scene : " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
                );
            
            /* 실행중 메시지 수신 처리(안드로이드
            var noti = new Assets.SimpleAndroidNotifications.NotificationParams
            {
                Id = UnityEngine.Random.Range(0, int.MaxValue),
                Delay = TimeSpan.FromSeconds(0.1f),
                Title = e.Message.Notification.Title,
                Message = e.Message.Notification.Body,
                Ticker = "Ticker",
                Sound = true,
                Vibrate = true,
                Light = true,
                SmallIcon = "app_icon",
                //SmallIconColor = new Color(0, 0.5f, 0),
                LargeIcon = "app_icon"
            };

            Assets.SimpleAndroidNotifications.NotificationManager.SendCustom(noti);
            */
        }

        //LogManager.Log("===========Received a new message==========="
        //+ "\nError : " + e.Message.Error
        //+ "\nPriority : " + e.Message.Priority
        //+ "\nMessageType : " + e.Message.MessageType
        //+ "\nMessageId : " + e.Message.MessageId
        //+ "\nRawData : " + e.Message.RawData
        //+ "\nCollapseKey : " + e.Message.CollapseKey
        //+ "\nTo : " + e.Message.To
        //+ "\nFrom : " + e.Message.From
        //+ "\nCount : " + e.Message.Data.Count
        //);

        //foreach (KeyValuePair<string, string> data in e.Message.Data)
        //{
        //    LogManager.Log("key : " + data.Key + "\nvalue : " + data.Value);
        //}

        SetData(e.Message);


        //AddData(e.Message.Data);

        //if (e.Message.Notification != null)
        //{
        //    if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("Welcome"))
        //    {
        //        OpenLastMessage();
        //    }
        //}
    }

    Firebase.Messaging.FirebaseMessage message;

    void SetData(Firebase.Messaging.FirebaseMessage _message)
    {
        message = _message;
    }

    public Firebase.Messaging.FirebaseMessage GetData()
    {
        return message;
    }

    public void ResetData()
    {
        message = null;
    }



    
#region Add Test Data
#if UNITY_EDITOR
    public void TestSaveMessageData()
    {
        MessageDataList dataList = LoadAllMessageData();

        if (dataList == null)
        {
            dataList = new MessageDataList();
            dataList.data = new List<MessageData>();
        }

        for(int i = 0; i < 3; i++)
        {
            MessageData msgData = new MessageData();
            msgData.key = "Key" + i;
            msgData.value = "Value" + i;
            dataList.data.Add(msgData);
        }

        PlayerPrefs.SetString("PushMsg", JsonUtility.ToJson(dataList));
    }

    public void TestAddMsgData()
    {
        for (int i = 0; i < 10; i++)
        {
            Message_Data addData = new Message_Data();
            addData.title = "Title" + i;
            addData.msg = "Message" + i;

            LogManager.Log("addData : " + addData.title + "/" + addData.msg);

            Data_List.datatList.Add(addData);
        }

        SaveData(Data_List);
    }
#endif
#endregion

    void SaveMessageData(IDictionary<string, string> _data)
    {
        MessageDataList dataList = LoadAllMessageData();

        if (dataList == null)
        {
            dataList = new MessageDataList();
            dataList.data = new List<MessageData>();
        }

        foreach (KeyValuePair<string, string> data in _data)
        {
            MessageData msgData = new MessageData();
            msgData.key = data.Key;
            msgData.value = data.Value;
            dataList.data.Add(msgData);
        }

        PlayerPrefs.SetString("PushMsg", JsonUtility.ToJson(dataList));
    }

    Message_Data SetData(IDictionary<string, string> _data)
    {
        Message_Data data = new Message_Data();

        if (_data.ContainsKey("type") == true)
            data.type = (PushMessageType)Enum.Parse(typeof(PushMessageType), _data["type"]);

        if (_data.ContainsKey("title") == true)
            data.title = _data["title"];

        if (_data.ContainsKey("msg") == true)
            data.msg = _data["msg"];

        if (_data.ContainsKey("link") == true)
            data.link = _data["link"];

        return data;
    }

    void AddData(IDictionary<string, string> _data)
    {
        Data_List.datatList.Add(SetData(_data));
        SaveData(Data_List);
    }

    public void RemoveData(Message_Data _data)
    {
        if (Data_List.datatList.Contains(_data))
        {
            Data_List.datatList.Remove(_data);
        }

        SaveData(Data_List);
    }

    void SaveData(Message_Data_List dataList)
    {
        string jsonData = JsonUtility.ToJson(dataList);
        PlayerPrefs.SetString("PushMsg", jsonData);
    }

    public void SaveData()
    {
        SaveData(Data_List);
    }

    public Message_Data_List LoadData()
    {
        Message_Data_List data = null;

        if (PlayerPrefs.HasKey("PushMsg") == true)
        {
            string jData = PlayerPrefs.GetString("PushMsg");

            if (string.IsNullOrEmpty(jData) == true)
            {
                data = new Message_Data_List();
                data.datatList = new List<Message_Data>();
            }
            else
            {
                data = JsonUtility.FromJson<Message_Data_List>(jData);
            }
        }
        else
        {
            data = new Message_Data_List();
            data.datatList = new List<Message_Data>();
        }

        return data;
    }

    /// <summary>
    /// 최근에 들어온 메시지 보여주기
    /// </summary>
    public void OpenLastMessage()
    {
        List<Message_Data> dataList = Data_List.datatList;

        if (dataList != null && dataList.Count > 0)
        {
            BMUtil.Instance.OpenDialog(
                dataList[dataList.Count - 1].title,
                dataList[dataList.Count - 1].msg, 
                LocalizationManager.Instance.GetLocalizedValue("common_button_done"), "", "", 
                true, false, false, 
                delegate { RemoveMessage(); }, null, null);
        }
    }

    /// <summary>
    /// 가장 최근의 메시지 삭제
    /// </summary>
    void RemoveMessage()
    {
        LogManager.Log("RemoveMessage");
        List<Message_Data> dataList = Data_List.datatList;
        MessageManager.Instance.RemoveData(dataList[dataList.Count - 1]);
        BMUtil.Destroy(gameObject);
    }














    MessageDataList LoadAllMessageData()
    {
        string jData = PlayerPrefs.GetString("PushMsg");

        if (string.IsNullOrEmpty(jData) == true) return null;

        return JsonUtility.FromJson<MessageDataList>(jData);
    }

    MessageData LoadMessageData(string key)
    {
        string jData = PlayerPrefs.GetString("PushMsg", null);

        if (string.IsNullOrEmpty(jData) == true) return null;

        List<MessageData> data = JsonUtility.FromJson<MessageDataList>(jData).data;

        for(int i = 0; i < data.Count; i++)
        {
            if(data[i].key == key)
            {
                return data[i];
            }
        }

        return null;
    }

    void RemoveMessageData(string key)
    {
        string jData = PlayerPrefs.GetString("PushMsg", null);

        if (string.IsNullOrEmpty(jData) == true) return;

        MessageDataList dataList = JsonUtility.FromJson<MessageDataList>(jData);

        List<MessageData> data = dataList.data;

        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].key == key)
            {
                data.Remove(data[i]);
                break;
            }
        }

        if (dataList.data.Count > 0)
        {
            string jsonString = JsonUtility.ToJson(dataList);
            PlayerPrefs.SetString("PushMsg", jsonString);
        }
        else
        {
            PlayerPrefs.DeleteKey("PushMsg");
        }
    }

    int viewCnt = 0;

    public void OpenMessage()
    {
        MessageDataList dList = LoadAllMessageData();

        if (dList == null || dList.data == null || dList.data.Count == 0) return;

        if (dList.data.Count <= viewCnt) return;

        LogManager.Log("OpenMessage");

        string dataKey = dList.data[viewCnt].key;
        string dataValue = dList.data[viewCnt].value;

        Transform uiParnet = GameObject.Find("/UI").transform;

        if (uiParnet == null) return;

        //GameObject Dialog = Instantiate(Resources.Load("Prefabs/Panel-Dialog")) as GameObject;
        //Dialog.transform.SetParent(uiParnet, false);
        //DialogController controller = Dialog.GetComponent<DialogController>();
        //controller.Title.text = "";
        //controller.Body.text = dataValue;
        //controller.setActions(delegate { viewCnt++; OpenMessage(); }, delegate { RemoveMessageData(dList.data[viewCnt].key); OpenMessage(); });
        //controller.SetButtonName(LocalizationManager.Instance.GetLocalizedValue("common_button_done"), LocalizationManager.Instance.GetLocalizedValue("common_button_delete"));
        //controller.setButtonVisible(true, true);

        BMUtil.Instance.OpenDialog("", dataValue,
            LocalizationManager.Instance.GetLocalizedValue("common_button_done"), LocalizationManager.Instance.GetLocalizedValue("common_button_delete"), "",
            true, true, false,
            delegate { viewCnt++; OpenMessage(); }, delegate { RemoveMessageData(dList.data[viewCnt].key); OpenMessage(); }, null);

        //BMUtil.Instance.AddOpenDialogList(controller);
    }

    public void NextMessage()
    {
        LogManager.Log("NextMessage");

        MessageDataList dList = LoadAllMessageData();
        if (dList != null && dList.data.Count > 0) OpenMessage();
    }

#endregion

}
