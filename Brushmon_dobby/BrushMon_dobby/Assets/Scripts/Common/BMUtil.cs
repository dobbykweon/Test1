using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AppLanguage
{
    KR = 0,
    EN = 1,
    TW = 2,
    JP = 3,
    CN = 4,
    None = 999,
}

public enum AppVoice
{
    KR = 0,
    EN = 1,
    TW = 2,
    JP = 3,
    CN = 4,
    None = 999,
}

public class BMUtil : GlobalMonoSingleton<BMUtil>
{
    public bool isNetDisconnect = false;

    #region 언어 관련
    public AppLanguage GetAppLanguage(SystemLanguage language)
    {
        switch (language)
        {
            case SystemLanguage.Korean:
                return AppLanguage.KR;

            case SystemLanguage.English:
                return AppLanguage.EN;

            case SystemLanguage.ChineseTraditional:
                return AppLanguage.TW;

            case SystemLanguage.Japanese:
                return AppLanguage.JP;

            //case SystemLanguage.ChineseSimplified:
            //    return AppLanguage.CN;

            default:
                return AppLanguage.EN;
        }
    }

    public SystemLanguage GetAppLanguage(AppLanguage language)
    {
        switch (language)
        {
            case AppLanguage.KR:
                return SystemLanguage.Korean;

            case AppLanguage.EN:
                return SystemLanguage.English;

            case AppLanguage.TW:
                return SystemLanguage.ChineseTraditional;

            case AppLanguage.JP:
                return SystemLanguage.Japanese;

            case AppLanguage.CN:
                return SystemLanguage.ChineseSimplified;

            default:
                return SystemLanguage.English;
        }
    }


    public AppVoice GetAppVoice(SystemLanguage language)
    {
        switch (language)
        {
            case SystemLanguage.Korean:
                return AppVoice.KR;

            case SystemLanguage.English:
                return AppVoice.EN;

            case SystemLanguage.ChineseTraditional:
                return AppVoice.TW;

            case SystemLanguage.Japanese:
                return AppVoice.JP;

            case SystemLanguage.ChineseSimplified:
                return AppVoice.CN;

            default:
                return AppVoice.EN;
        }
    }

    public SystemLanguage GetAppVoice(AppVoice language)
    {
        switch (language)
        {
            case AppVoice.KR:
                return SystemLanguage.Korean;

            case AppVoice.EN:
                return SystemLanguage.English;

            case AppVoice.TW:
                return SystemLanguage.ChineseTraditional;

            case AppVoice.JP:
                return SystemLanguage.Japanese;

            case AppVoice.CN:
                return SystemLanguage.ChineseSimplified;

            default:
                return SystemLanguage.English;
        }
    }
    #endregion

    #region 네트워크 관련

    public void StartNetCoroutine(IEnumerator coroutine)
    {
        OpenLoading();
        StartCoroutine(coroutine);
    }

    public void ReTryNetConnect(IEnumerator coroutine, int retryCount, Action actionDone = null, System.Net.HttpStatusCode statusCode = System.Net.HttpStatusCode.OK, string exceptionMsg = null)
    {
        StopCoroutine(coroutine);

        if (retryCount > 3)
        {
//            GameObject Dialog = Instantiate(Resources.Load("Prefabs/Panel-Dialog")) as GameObject;
//            Dialog.transform.SetParent(UIRoot, false);
//            DialogController controller = Dialog.GetComponent<DialogController>();
//#if !UNITY_EDITOR
//            controller.Title.text = LocalizationManager.Instance.GetLocalizedValue("net_connect_error_title");
//            controller.Body.text = LocalizationManager.Instance.GetLocalizedValue("net_connect_error_message");
//            controller.SetButtonName(LocalizationManager.Instance.GetLocalizedValue("common_button_done"), LocalizationManager.Instance.GetLocalizedValue("net_connect_retry"));
//#else
//            controller.Title.text = "네트워크 오류" + ((statusCode == System.Net.HttpStatusCode.OK) ? "" : "(" + ((int)statusCode) + ")");
//            controller.Body.text = "다시시도 하시겠습니까?" + ((string.IsNullOrEmpty(exceptionMsg)) ? "" : ("\n" + exceptionMsg));
//            controller.SetButtonName(LocalizationManager.Instance.GetLocalizedValue("common_button_done"), LocalizationManager.Instance.GetLocalizedValue("net_connect_retry"));
//#endif
//            controller.setActions(null, delegate { StartCoroutine(coroutine); });
//            controller.setButtonVisible(true, true);

            OpenDialog(LocalizationManager.Instance.GetLocalizedValue("net_connect_error_title"),
                LocalizationManager.Instance.GetLocalizedValue("net_connect_error_message"),
                LocalizationManager.Instance.GetLocalizedValue("common_button_done"), LocalizationManager.Instance.GetLocalizedValue("net_connect_retry"),
                true, true,
                actionDone, delegate { StartCoroutine(coroutine); }
                );
        }
        else
        {
            StartCoroutine(coroutine);
        }
    }

    Transform uiRoot = null;
    public Transform UIRoot
    {
        get
        {
            if (uiRoot == null)
                uiRoot = GameObject.Find("/UI").transform;
            return uiRoot;
        }
    }

    GameObject uiLoading = null;
    GameObject UILoading
    {
        get
        {
            if (uiLoading == null)
            {
                uiLoading = Instantiate(Resources.Load("Prefabs/UILoading")) as GameObject;
                uiLoading.transform.SetParent(UIRoot, false);
            }
            return uiLoading;
        }
    }

    int loadingCnt = 0;

    public void OpenLoading()
    {
        UILoading.SetActive(true);

        loadingCnt++;
    }

    public void CloseLoading()
    {
        if (loadingCnt > 0) loadingCnt--;

        if (loadingCnt == 0)
        {
            UILoading.SetActive(false);
        }
    }

    public bool IsLoading()
    {
        return uiLoading != null && uiLoading.activeInHierarchy == true;
    }



    #endregion

    public void CheckAppVersion(Action<bool> action)
    {
        //IOS 앱 버전을 가져와서 체크하여 버전이 틀리면 return false;

        HttpClient client = new HttpClient();
        Uri uri = new Uri("http://itunes.apple.com/lookup?bundleId=com.kittenplanet.brushmon");

        client.GetString(uri, callback =>
        {
            Dictionary<string, object> dData = MiniJson.Deserialize(callback.Data) as Dictionary<string, object>;
            string version = ((Dictionary<string, object>)(((List<object>)dData["results"])[0]))["version"].ToString();
            action(version.Equals(Application.version));
        });
    }

    Stack<UIDialog> openDialog = new Stack<UIDialog>();

    public void OpenDialog(string title, string body, string doneName, string cancelName = "", bool isDone = true, bool isCancel = false, Action Done = null, Action Cancel = null)
    {
        OpenDialog(title, body, doneName, cancelName, "", isDone, isCancel, false, Done, Cancel);
    }

    public void OpenDialog(string title, string body, string doneName = "", string cancelName = "", string detailName = "", bool isDone = true, bool isCancel = false, bool isDetail = false, Action Done = null, Action Cancel = null, Action Detail = null)
    {
        //GameObject Dialog = Instantiate(Resources.Load("Prefabs/Panel-Dialog")) as GameObject;
        //Dialog.transform.SetParent(UIRoot.transform, false);
        //DialogController controller = Dialog.GetComponent<DialogController>();

        GameObject Dialog = Instantiate(Resources.Load("Prefabs/UIDialog")) as GameObject;
        Dialog.transform.SetParent(UIRoot.transform, false);
        UIDialog controller = Dialog.GetComponent<UIDialog>();

        controller.Title.text = title;
        controller.Body.text = body;
        controller.setActions(Done, Cancel, Detail);
        controller.SetButtonName(doneName, cancelName, detailName);
        controller.setButtonVisible(isDone, isCancel, isDetail);
        
        AddOpenDialogList(controller);
    }

    public void AddOpenDialogList(UIDialog controller)
    {
        if (openDialog.Count > 0)
            openDialog.Peek().gameObject.SetActive(false);

        openDialog.Push(controller);
    }

    public void CloseDialog()
    {
        LogManager.Log("openDialog.Count : " + openDialog.Count);

        openDialog.Pop();
        if (openDialog.Count > 0) openDialog.Peek().gameObject.SetActive(true);
    }

    public void ClearDialog()
    {
        openDialog.Clear();
    }

    public bool IsDialog
    {
        get
        {
            return openDialog.Count > 0;
        }
    }


    public bool IsiPhoneX()
    {
#if UNITY_IOS
        float screenBaseRatio = 1080f / 1920f;
        float screenRatio = (float)Screen.width / (float)Screen.height;

        if (Mathf.Abs(screenBaseRatio - screenRatio) > 0.01f)
        {
            if (screenBaseRatio > screenRatio)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
#else
        return false;
#endif
    }

    public bool IsiPad()
    {
#if UNITY_IOS
        float screenBaseRatio = 1080f / 1920f;
        float screenRatio = (float)Screen.width / (float)Screen.height;

        if (Mathf.Abs(screenBaseRatio - screenRatio) > 0.01f)
        {
            if (screenBaseRatio < screenRatio)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
#else
        return false;
#endif
    }


    public void SaveUserAccount(SignVo signVo)
    {
        UserAccountVo userAccountVo;

        LogManager.Log("SaveUserAccount : " + signVo.email + "/" + signVo.password);

        if (ConfigurationData.Instance.HasKey("AccountList") == true)
            userAccountVo = ConfigurationData.Instance.GetValueFromJson<UserAccountVo>("AccountList");
        else
            userAccountVo = new UserAccountVo();

        bool isAdd = true;

        for (int i = 0; i < userAccountVo.list.Count; i++)
        {
            if (userAccountVo.list[i].email.Equals(signVo.email) == true)
                isAdd = false;
        }

        if(isAdd == true)
            userAccountVo.list.Add(signVo);

        ConfigurationData.Instance.SetJsonValue("AccountList", JsonUtility.ToJson(userAccountVo));
    }

    public List<SignVo> LoadUserAccount()
    {
        if (ConfigurationData.Instance.HasKey("AccountList") == true)
        {
            return ConfigurationData.Instance.GetValueFromJson<UserAccountVo>("AccountList").list;
        }
        else
        {
            return null;
        }
    }

    public void RemoveUserAccount(string email)
    {
        UserAccountVo userAccountVo = ConfigurationData.Instance.GetValueFromJson<UserAccountVo>("AccountList");

        for (int i = 0; i < userAccountVo.list.Count; i++)
        {
            LogManager.Log(userAccountVo.list[i].email + "//" + email);

            if (userAccountVo.list[i].email.Equals(email) == true)
            {
                userAccountVo.list.RemoveAt(i);
                break;
            }
        }

        ConfigurationData.Instance.SetJsonValue("AccountList", JsonUtility.ToJson(userAccountVo));
    }

    /// <summary>
    /// 오브젝트 즉시 삭제
    /// </summary>
    /// <param name="obj"></param>
    public static void Destroy(UnityEngine.Object obj)
    {
        if (obj)
        {
            if (obj is Transform)
            {
                Transform t = (obj as Transform);
                GameObject go = t.gameObject;

                if (Application.isPlaying)
                {
                    t.parent = null;
                    UnityEngine.Object.Destroy(go);
                }
                else UnityEngine.Object.DestroyImmediate(go);
            }
            else if (obj is GameObject)
            {
                GameObject go = obj as GameObject;
                Transform t = go.transform;

                if (Application.isPlaying)
                {
                    t.parent = null;
                    UnityEngine.Object.Destroy(go);
                }
                else UnityEngine.Object.DestroyImmediate(go);
            }
            else if (Application.isPlaying) UnityEngine.Object.Destroy(obj);
            else UnityEngine.Object.DestroyImmediate(obj);
        }
    }

    #region Toast 메시지

    public void OpenNextSceneToast(string msg)
    {
        StartCoroutine(NextSceneToast(msg));
    }

    IEnumerator NextSceneToast(string msg)
    {
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(showToast(msg));
    }

    public void OpenToast(string msg)
    {
        StartCoroutine(showToast(msg));
    }

    IEnumerator showToast(string msg)
    {
        //LogManager.Log("showToast : " + msg);

        //GameObject Toast = Instantiate(Resources.Load("Prefabs/Toast")) as GameObject;
        //Toast.transform.SetParent(UIRoot.transform, false);
        //Toast.GetComponentInChildren<UnityEngine.UI.Text>().text = msg;
        //Toast.SetActive(true);
        //yield return new WaitForSeconds(1.5f);
        //Destroy(Toast);

        GameObject toast = Instantiate(Resources.Load("Prefabs/Toast")) as GameObject;

        if (toast != null)
        {
            toast.transform.SetParent(UIRoot, false);
            toast.SetActive(true);
        }

        UnityEngine.UI.Text txtMsg = toast.GetComponentInChildren<UnityEngine.UI.Text>();
        txtMsg.text = msg;

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        float sizeX = txtMsg.rectTransform.sizeDelta.x + 72 + 72;

        if (toast != null)
        {
            toast.GetComponent<RectTransform>().sizeDelta = new Vector2((sizeX > 500) ? sizeX : 500f, 131f);
            toast.GetComponent<UnityEngine.UI.Image>().enabled = true;
            txtMsg.color = Color.white;
        }

        yield return new WaitForSeconds(2f);

        if (toast != null) toast.GetComponent<uTools.TweenAlpha>().PlayForward();

        yield return new WaitForSeconds(0.3f);

        if (toast != null) GameObject.Destroy(toast);
    }
    #endregion

    public GameObject LoadPrefab(string path, Transform parent = null)
    {
        GameObject obj = Instantiate(Resources.Load("Prefabs/"+path)) as GameObject;

        if (obj == null) return null;

        if(parent != null)
            obj.transform.SetParent(parent, false);
        else
            obj.transform.SetParent(UIRoot, false);

        return obj;
    }

    /// <summary>
    /// 암호화 디코딩
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public string Decode(string value)
    {
#if HANWHA
        value = Base64.base64Decode(Utility.MD5Manager.DESDecrypt(value));
        LogManager.Log("Decode : " + value);
        return value;
#else
        return value;
#endif
    }

    /// <summary>
    /// 암호화 인코딩
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public string Encode(string value)
    {
#if HANWHA
        value = Utility.MD5Manager.DESEncrypt(Base64.base64Encode(value));
        LogManager.Log("Encode : " + value);
        return value;
#else
        return value;
#endif
    }

    //일반문자와 특수문자외의 입력이 들어오면 강제로 삭제(이모티콘 강제 삭제)
    public string CheckText(string text)
    {
        return System.Text.RegularExpressions.Regex.Replace(text, @"\uD83D[\uDC00-\uDFFF]|\uD83C[\uDC00-\uDFFF]|\uFFFD", "");
    }

    string fcmToken;

    public void SetFcmToken(string token)
    {
        fcmToken = token;
    }

    public string GetFCMToken()
    {
        return fcmToken;
    }

    public void FcmTokenList()
    {
        TokenVo tokenVo = null;

        while (tokenVo == null)
        {
            tokenVo = ConfigurationData.Instance.GetValueFromJson<TokenVo>("Token");
        }

        RestService.Instance.GetFcmTokenList(tokenVo,
            result =>
            {
                LogManager.Log("FcmToken : " + result);

                List<FcmTokenVo> fcmTokenList = JsonHelper.getJsonArray<FcmTokenVo>(result);

                bool isRegister = true;

                for (int i = 0; i < fcmTokenList.Count; i++)
                {
                    LogManager.Log("token" + i + " : " + fcmTokenList[i].token);

                    //if (fcmTokenList[i].token.Equals(GetFCMToken()))
                    //{
                    //    isRegister = false;
                    //    break;
                    //}
                }

                //if (isRegister)
                //{
                //    FcmToken fcmToken = new FcmToken();
                //    fcmToken.fcm_token = GetFCMToken();

                //    RestService.Instance.RegisterFcmToken(tokenVo, fcmToken,
                //        delegate
                //        {
                //            LogManager.Log("RegisterFcmToken Success");
                //        },
                //        exception =>
                //        {
                //            LogManager.Log("RegisterFcmToken Exception : " + exception.ToString());
                //        }
                //    );
                //}
            },
            exception =>
            {
                LogManager.LogError("GetFcmTokenList exception : " + exception.ToString());
            }
        );
    }

    long selfyIdx = 1;
    CharacterType selfyType = CharacterType.cheese;
    public long SelectSelfyIdx
    {
        get { return selfyIdx; }
        set { selfyIdx = value; }
    }

    public CharacterType SelectSelfyType
    {
        get { return selfyType; }
        set { selfyType = value; }
    }

    long selectStickerIndex = 1;
    public long SelectStickerIndex
    {
        get { return selectStickerIndex; }
        set { selectStickerIndex = value; }
    }

    public void LoadTestStickData(Action<string> action, string fileName = "RewardListAll.json")
    {
        StartCoroutine(LoadTestData(fileName, action));
    }

    IEnumerator LoadTestData(string fileName, Action<string> action)
    {
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
        string dataAsJson;

        if (filePath.Contains("://"))
        {
            UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(filePath);
            yield return www.SendWebRequest();
            dataAsJson = www.downloadHandler.text;
        }
        else
            dataAsJson = System.IO.File.ReadAllText(filePath);

        action(dataAsJson);
    }

    public DateTime local_create_date(long create_date)
    {
        var posixTime = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);
        var time = posixTime.AddMilliseconds(create_date);
        return time.ToLocalTime();
    }

    public long GetGMTinMs(DateTime dateTime)
    {
        var unixTime = dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        //LogManager.Log("dateTime : " + dateTime.ToUniversalTime().ToLongDateString());
        //LogManager.Log("UTC : " + (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).ToLongDateString());

        return (long)unixTime.TotalMilliseconds;
    }

    public void OpenTitle()
    {
        PlayerPrefs.DeleteKey("WellcomePage");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Welcome");
    }

    public bool IsStore()
    {
        switch (NativePlugin.Instance.GetNativeCountryCode())
        {
            case "KR":
                return true;

            default:
                return false;
        }
    }

    public void OpenStore()
    {
        switch (Application.systemLanguage)
        {
            case SystemLanguage.Korean:
                Application.OpenURL("");
                break;

            case SystemLanguage.Japanese:
                Application.OpenURL("");
                break;
        }
    }

}
