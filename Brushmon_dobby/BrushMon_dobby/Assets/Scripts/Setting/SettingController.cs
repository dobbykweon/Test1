using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingController : MonoBehaviour {
    UserVo userVo;
    TokenVo tokenVo;
    public Slider Vibrator;

    public GameObject BackButton;

    public Toggle togglePaste;  //치약짜기 체크
    public Toggle toggleClean;  //양치후 정리 체크

    public Toggle toggleMarketing;  //마케팅 정보 수신 동의
    public Toggle toggleMessage;    //메시지 수신 동의

    public Toggle toggleReverse;    //양치화면 반전
    public Toggle toggleIntroMovie; //인트로 영상 보기


    public Dropdown language;   //언어선택 추가
    public Dropdown voice;      //음성선택 추가

    public GameObject Hidden;
    public InputField BleDeviceName;

    private bool isPaste = true;
    private bool isClean = true;

    GameObject objConnectingBrush = null;

    public void onToggleValueChangePaste (bool value) {
        isPaste = value;
    }
    public void onToggleValueChangeClean (bool value) {
        isClean = value;
    }

    public void OnToggleValueChangeReverse(bool value)
    {

    }

    void Start () {
        TrackingManager.Instance.Tracking ("setting", "Setting");
        tokenVo = ConfigurationData.Instance.GetValueFromJson<TokenVo> ("Token");

        RestService.Instance.GetUser (tokenVo, user => {
            userVo = JsonUtility.FromJson<UserVo> (user);
            togglePaste.isOn = userVo.paste_exist;
            toggleClean.isOn = userVo.clean_up_exist;
            toggleMarketing.isOn = userVo.agree_marketing;
            toggleMessage.isOn = userVo.agree_message; ;

            if( string.IsNullOrEmpty(userVo.app_language_text) == true)
                userVo.app_language_text = BMUtil.Instance.GetAppLanguage(Application.systemLanguage).ToString();

            if (string.IsNullOrEmpty(userVo.app_language_voice) == true)
                userVo.app_language_voice = BMUtil.Instance.GetAppVoice(Application.systemLanguage).ToString();

            language.value = (int)System.Enum.Parse(typeof(AppLanguage), userVo.app_language_text);
            voice.value = (int)System.Enum.Parse(typeof(AppVoice), userVo.app_language_voice);

            SelectLanguage(language.value);
            SelectVoice(voice.value);

            Vibrator.value = userVo.vibration;

            toggleReverse.isOn = ConfigurationData.Instance.GetValue<int>("is_reverse", 0) > 0 ? true : false;
            toggleIntroMovie.isOn = ConfigurationData.Instance.GetValue<int>("is_intro_movie", 0) > 0 ? true : false;


#if OFFLINE
            toggleIntroMovie.transform.parent.gameObject.SetActive(false);
#endif

#if UNITY_IOS
            toggleReverse.transform.parent.gameObject.SetActive(false);
#else
            toggleReverse.transform.parent.gameObject.SetActive(true);
#endif

        }, exception => {
            LogManager.Log ("exception : " + exception.Message);
            BrushMonSceneManager.Instance.LoadScene(SceneNames.Welcome.ToString());
        });
    }

    public void onClick (string type) {
        switch (type) {
            case "ProfileEdit":
                SaveSetting(delegate
                {
                    FirebaseManager.Instance.LogEvent("br_setting_profile_edit");
                    BMUtil.Instance.OpenNextSceneToast(LocalizationManager.Instance.GetLocalizedValue("msg_setting_save"));
                    BrushMonSceneManager.Instance.LoadScene(SceneNames.ProfileEditSelect.ToString());
                }, exception =>
                {
                    LogManager.Log("ProfileEdit Error : " + exception.Message);
                });
                break;
            case "Logout":
                SaveSetting(delegate
                {
                    FirebaseManager.Instance.LogEvent("br_setting_logout");
                    TrackingManager.Instance.Tracking ("logout", "Logout");
                
                    ConfigurationData.Instance.CleanAll ();
                    BMUtil.Instance.OpenNextSceneToast(LocalizationManager.Instance.GetLocalizedValue("msg_setting_save"));
                    LocalizationManager.Instance.SetLanguage(Application.systemLanguage);
                    BrushMonSceneManager.Instance.LoadScene (SceneNames.SignIn.ToString ());
                }, exception =>
                {
                    LogManager.Log("Logout Error : " + exception.Message);
                });
                break;
        }
    }

    public void onBackPress () {

        //ConfigurationData.Instance.SetValue<int>("is_reverse", toggleReverse.isOn ? 1 : 0);

        ////양치후 정리
        //if (userVo.clean_up_exist != toggleClean.isOn) FirebaseManager.Instance.LogEvent("br_setting_wrapup");
        //userVo.clean_up_exist = toggleClean.isOn;

        ////치약짜기
        //if(userVo.paste_exist != togglePaste.isOn) FirebaseManager.Instance.LogEvent("br_setting_squeezing");
        //userVo.paste_exist = togglePaste.isOn;

        ////진동세기
        //if (userVo.vibration != (int)Vibrator.value) FirebaseManager.Instance.LogEvent("br_setting_vibration", "power", Vibrator.value);
        //userVo.vibration = (int) Vibrator.value;

        ////언어변경
        //if (userVo.app_language_text.Equals(((AppLanguage)language.value).ToString()) == false) FirebaseManager.Instance.LogEvent("br_setting_language");
        //userVo.app_language_text = ((AppLanguage)language.value).ToString();

        ////보이스변경
        //if (userVo.app_language_voice.Equals(((AppVoice)voice.value).ToString()) == false) FirebaseManager.Instance.LogEvent("br_setting_voice");
        //userVo.app_language_voice = ((AppVoice)voice.value).ToString();


        //userVo.agree_marketing = toggleMarketing.isOn;
        //userVo.agree_message = toggleMessage.isOn;

        //userVo.device_country = NativePlugin.Instance.GetNativeCountryCode();
        //userVo.device_language = NativePlugin.Instance.GetNativeLanguageCode();
        //userVo.app_version = Application.version;

        //ConfigurationData.Instance.SetJsonValue ("User", userVo);
        //RestService.Instance.UpdateUserSetting (tokenVo, userVo, delegate {
        //    BrushMonSceneManager.Instance.LoadScene (SceneNames.Welcome.ToString ());
        //}, exception => {
        //    LogManager.Log (exception.Message);
        //    BrushMonSceneManager.Instance.LoadScene (SceneNames.Welcome.ToString ());
        //});

        SaveSetting(
            delegate {
                BMUtil.Instance.OpenNextSceneToast(LocalizationManager.Instance.GetLocalizedValue("msg_setting_save"));
                BrushMonSceneManager.Instance.LoadScene(SceneNames.Welcome.ToString());
            }, exception => {
                LogManager.Log(exception.Message);
                BrushMonSceneManager.Instance.LoadScene(SceneNames.Welcome.ToString());
            });

        AudioManager.Instance.LoadAudio();
    }

    void SaveSetting(System.Action action, System.Action<System.Exception> exception)
    {
        ConfigurationData.Instance.SetValue<int>("is_reverse", toggleReverse.isOn ? 1 : 0);
        ConfigurationData.Instance.SetValue<int>("is_intro_movie", toggleIntroMovie.isOn ? 1 : 0);

        //양치후 정리
        if (userVo.clean_up_exist != toggleClean.isOn) FirebaseManager.Instance.LogEvent("br_setting_wrapup");
        userVo.clean_up_exist = toggleClean.isOn;

        //치약짜기
        if (userVo.paste_exist != togglePaste.isOn) FirebaseManager.Instance.LogEvent("br_setting_squeezing");
        userVo.paste_exist = togglePaste.isOn;

        //진동세기
        if (userVo.vibration != (int)Vibrator.value) FirebaseManager.Instance.LogEvent("br_setting_vibration", "power", Vibrator.value);
        userVo.vibration = (int)Vibrator.value;

        //언어변경
        if (userVo.app_language_text.Equals(((AppLanguage)language.value).ToString()) == false) FirebaseManager.Instance.LogEvent("br_setting_language");
        userVo.app_language_text = ((AppLanguage)language.value).ToString();

        //보이스변경
        if (userVo.app_language_voice.Equals(((AppVoice)voice.value).ToString()) == false) FirebaseManager.Instance.LogEvent("br_setting_voice");
        userVo.app_language_voice = ((AppVoice)voice.value).ToString();


        userVo.agree_marketing = toggleMarketing.isOn;
        userVo.agree_message = toggleMessage.isOn;

        userVo.device_country = NativePlugin.Instance.GetNativeCountryCode();
        userVo.device_language = NativePlugin.Instance.GetNativeLanguageCode();
        userVo.app_version = Application.version;

        ConfigurationData.Instance.SetJsonValue("User", userVo);
        RestService.Instance.UpdateUserSetting(tokenVo, userVo, action, exception);
    }

    void FailSave()
    {
        LogManager.Log("셋팅 저장 실패");
    }

    public void Update () {
        //if (Application.platform == RuntimePlatform.Android)
        if(objConnectingBrush == null)
        {
            if (Input.GetKeyUp (KeyCode.Escape)) {
                onBackPress ();
                return;
            }
        }
    }

    private int BleScanDeviceNameCount = 0;
    public void showHidden () {
        //if (userVo != null && userVo.user_id.IndexOf ("temp@kittenpla.net") > -1)
        {
            if (BleScanDeviceNameCount == 0) {
                StartCoroutine (coSkip ());
            }
            BleScanDeviceNameCount++;
            if (BleScanDeviceNameCount > 3) {
                Hidden.SetActive (true);
                BleDeviceName.text = ConfigurationData.Instance.GetValue<string> ("BleDeviceName", "BMT100");
            }
        }
    }

    IEnumerator coSkip () {
        yield return new WaitForSecondsRealtime (1f);
        BleScanDeviceNameCount = 0;
    }

    public void SaveBleScanDeviceName () {
        if (!string.IsNullOrEmpty (BleDeviceName.text)) {
            ConfigurationData.Instance.SetValue<string> ("BleDeviceName", BleDeviceName.text);
        }
        Hidden.SetActive (false);
    }

    int GetSelectLanguageDropDownIndex()
    {
        SystemLanguage selectLanguage = LocalizationManager.Instance.LoadLanguage();

        switch(selectLanguage)
        {
            case SystemLanguage.Korean:             return 0;
            case SystemLanguage.English:            return 1;
            case SystemLanguage.ChineseTraditional: return 2;
            case SystemLanguage.Japanese:           return 3;
            case SystemLanguage.ChineseSimplified:  return 4;
            default:                                return 1;
        }
    }

    int GetSelectVoiceDropDownIndex()
    {
        SystemLanguage selectLanguage = LocalizationManager.Instance.LoadVoice();

        switch (selectLanguage)
        {
            case SystemLanguage.Korean:             return 0;
            case SystemLanguage.English:            return 1;
            case SystemLanguage.ChineseTraditional: return 2;
            case SystemLanguage.Japanese:           return 3;
            case SystemLanguage.ChineseSimplified:  return 4;
            default:                                return 1;
        }
    }

    public void SelectLanguage(int selectValue)
    {
        LogManager.Log("SelectLanguage : " + selectValue);

        SystemLanguage selectLanguage;

        switch(selectValue)
        {
            case 0:
                selectLanguage = SystemLanguage.Korean;
                break;

            case 1:
                selectLanguage = SystemLanguage.English;
                break;

            case 2:
                selectLanguage = SystemLanguage.ChineseTraditional;
                break;

            case 3:
                selectLanguage = SystemLanguage.Japanese;
                break;

            case 4:
                selectLanguage = SystemLanguage.ChineseSimplified;
                break;

            default : 
                selectLanguage = SystemLanguage.English;
                break;
        }

        LocalizationManager.Instance.SetLanguage(selectLanguage);
        
        LocalizedText[] txts = BMUtil.Instance.UIRoot.GetComponentsInChildren<LocalizedText>();
        for(int i = 0 ; i < txts.Length ; i++)
        {
            txts[i].ReLoadText();
        }
    }

    public void SelectVoice(int selectValue)
    {
        SystemLanguage selectLanguage;

        switch (selectValue)
        {
            case 0:
                selectLanguage = SystemLanguage.Korean;
                break;

            case 1:
                selectLanguage = SystemLanguage.English;
                break;

            case 2:
                selectLanguage = SystemLanguage.ChineseTraditional;
                break;

            case 3:
                selectLanguage = SystemLanguage.Japanese;
                break;

            case 4:
                selectLanguage = SystemLanguage.ChineseSimplified;
                break;

            default:
                selectLanguage = SystemLanguage.English;
                break;
        }

        LocalizationManager.Instance.SaveVoice(selectLanguage);
    }

    //public void SendEmail()
    //{
    //    string mailto = "chlee@kittenpla.net";
    //    string subject = EscapeURL("이메일 테스트");
    //    string body = EscapeURL
    //        (
    //         "내용\n내용\n내용\n내용\n내용\n내용\n내용\n내용\n내용\n내용\n내용\n" +
    //         "________" +
    //         "Device Model : " + SystemInfo.deviceModel + "\n\n" +
    //         "Device OS : " + SystemInfo.operatingSystem + "\n\n" +
    //         "________"
    //        );

    //    Application.OpenURL("mailto:" + mailto + "?subject=" + subject + "&body=" + body);
    //}

    //private string EscapeURL(string url)
    //{
    //    return WWW.EscapeURL(url).Replace("+", "%20");
    //}

    public void SendEmail()
    {
        //UserVo userVo = ConfigurationData.Instance.GetValueFromJson<UserVo>("User");
        //ProfileVo currentProfileVo = ConfigurationData.Instance.GetValueFromJson<ProfileVo>("CurrentProfile");

        //string userInfo = "\n\n" +
        //    "ID : " + userVo.user_id + "\n" +
        //    "Profile Name : " + currentProfileVo.name + "\n" +
        //    "Device Model : " + SystemInfo.deviceModel + "\n" +
        //    "Device OS : " + SystemInfo.operatingSystem + "\n";

        //MailMessage mail = new MailMessage();

        //mail.From = new MailAddress("leech0051@gmail.com");
        //mail.To.Add("leech0051@gmail.com");
        //mail.Subject = "고객문의";
        //mail.Body = "이메일 전송 테스트 입니다." + userInfo;


        //SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
        //smtpServer.Port = 587;
        //smtpServer.Credentials = new System.Net.NetworkCredential("abydos3755@gmail.com", "1qaz2wsx~") as ICredentialsByHost;
        //smtpServer.EnableSsl = true;
        //ServicePointManager.ServerCertificateValidationCallback =
        //delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        //{ return true; };
        //smtpServer.Send(mail);
        //Debug.Log("success");
    }

    public void BtnSetVibrator()
    {
#if UNITY_ANDROID
        // GPS 꺼져있으면
        if (!GPSController.Instance.CheckGPS())
        {
            // GPS 팝업 실행
            GPSController.Instance.TurnOnGPS();
        }
        else
        {
            objConnectingBrush = BMUtil.Instance.LoadPrefab("UIConnectingBrush");
            objConnectingBrush.GetComponent<UIConnectingBrush>().InitSetVibration(SetVibrator, userVo.vibration);
        }
#else
        objConnectingBrush = BMUtil.Instance.LoadPrefab("UIConnectingBrush");
        objConnectingBrush.GetComponent<UIConnectingBrush>().InitSetVibration(SetVibrator, userVo.vibration);
#endif
    }

    void SetVibrator(int _vibration)
    {
        userVo.vibration = _vibration;
        Vibrator.value = userVo.vibration;
        
        //currentProfileVo.vibration = _vibration;
        //Vibrator.value = _vibration;
    }

    public void BtnSave()
    {
        SaveSetting(
            delegate
            {
                BMUtil.Instance.OpenToast(LocalizationManager.Instance.GetLocalizedValue("msg_setting_save"));
            }, exception =>
            {
                LogManager.Log(exception.Message);
                BrushMonSceneManager.Instance.LoadScene(SceneNames.Welcome.ToString());
            });
    }

}