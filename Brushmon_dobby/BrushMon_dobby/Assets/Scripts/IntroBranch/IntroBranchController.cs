using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroBranchController : MonoBehaviour {

    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;

    bool isConnectServer = false;
    bool isCheckEnd = false;

    public GameObject objWarning;

    IEnumerator Start () 
    {
        PlayerPrefs.DeleteKey("WellcomePage");

        LogManager.Log("START!!!!!!");

        while (Caching.ready == false)
        {
            yield return null;
        }

        LocalizationManager.Instance.SetLanguage(Application.systemLanguage);

        //LocalizationManager.Instance.SetLanguage(SystemLanguage.Japanese);
        //LocalizationManager.Instance.SetLanguage(SystemLanguage.English);
        //LocalizationManager.Instance.SetLanguage(SystemLanguage.ChineseTraditional);

        while (LocalizationManager.Instance.IsReady == false) yield return null;

        
        if (!GPSController.Instance.CheckGPS())
        {
            // GPS 팝업 실행
            GPSController.Instance.TurnOnGPS();
        }

        if (objWarning != null)
        {
            if (PlayerPrefs.HasKey("warning_check") == false || PlayerPrefs.GetInt("warning_check") == 0)
            {
                objWarning.SetActive(true);
                while (objWarning != null && objWarning.activeInHierarchy == true) yield return null;
            }
        }

#if UNITY_IOS
        if (PlayerPrefs.HasKey("is_reverse"))
        {
            PlayerPrefs.DeleteKey("is_reverse");
        }
#endif
        {

            yield return StartCoroutine(CheckConnectServer());

            if (isConnectServer == false)
            {
                LocalizationManager.Instance.SetLanguage(Application.systemLanguage);
                LocalizationManager.Instance.SaveVoice(Application.systemLanguage);

                while (LocalizationManager.Instance.IsReady == false) yield return null;

                /* 오프라인 모드????
                //네트워크 오류
                //연결된 네트워크가 없습니다.\n오프라인모드로 사용하시겠습니까?\n오프라인모드는 양치하기 기능만 사용 가능합니다.\n오프라인모드에서는 양치결과와 스티커 획득이 불가능 합니다.
                GameObject Dialog = Instantiate(Resources.Load("Prefabs/Panel-Dialog")) as GameObject;
                Dialog.transform.SetParent(GameObject.Find("/UI").transform, false);
                DialogController controller = Dialog.GetComponent<DialogController>();
                controller.Title.text = LocalizationManager.Instance.GetLocalizedValue("net_connect_error_title");
                controller.Body.text = "연결된 네트워크가 없습니다.\n오프라인모드로 사용하시겠습니까?\n오프라인모드는 양치하기 기능만 사용 가능합니다.\n오프라인모드에서는 양치결과와 스티커 획득이 불가능 합니다.";
                controller.setActions(delegate { BMUtil.Instance.SetOfflineMode(true); StartCoroutine(Start()); }, delegate { StartCoroutine(Start()); });
                controller.SetButtonName("오프라인모드", LocalizationManager.Instance.GetLocalizedValue("net_connect_retry"));
                controller.setButtonVisible(true, true);
                */

                //GameObject Dialog = Instantiate(Resources.Load("Prefabs/Panel-Dialog")) as GameObject;
                //Dialog.transform.SetParent(GameObject.Find("/UI").transform, false);
                //DialogController controller = Dialog.GetComponent<DialogController>();
                //controller.Title.text = LocalizationManager.Instance.GetLocalizedValue("net_connect_error_title");
                //controller.Body.text = LocalizationManager.Instance.GetLocalizedValue("net_connect_error_message");
                //controller.setActions(delegate { Application.Quit(); }, delegate { StartCoroutine(Start()); });
                //controller.SetButtonName(LocalizationManager.Instance.GetLocalizedValue("app_quit"), LocalizationManager.Instance.GetLocalizedValue("net_connect_retry"));
                //controller.setButtonVisible(true, true);

                BMUtil.Instance.OpenDialog(
                    LocalizationManager.Instance.GetLocalizedValue("net_connect_error_title"), LocalizationManager.Instance.GetLocalizedValue("net_connect_error_message"),
                    LocalizationManager.Instance.GetLocalizedValue("app_quit"), LocalizationManager.Instance.GetLocalizedValue("net_connect_retry"), "",
                    true, true, false,
                    delegate { Application.Quit(); }, delegate { StartCoroutine(Start()); }
                    );

                //BMUtil.Instance.AddOpenDialogList(controller);

                yield return null;
            }
            else
            {
                setFirebase();

                yield return StartCoroutine(ResourceLoadManager.Instance.Initialize());

#if OFFLINE
                if (ConfigurationData.Instance.HasKey("Token") == false)
                {
                    RestService.Instance.SignIn(new SignVo("", ""),
                        token =>
                        {
                            ConfigurationData.Instance.SetJsonValue("Token", token);
                        },
                        exception =>
                        {
                        });
                }
#endif

                // Firebase 유저 속성 설정
                FirebaseManager.Instance.SetUserInfo();
#if !OFFLINE
                bool isFirstEnter = ConfigurationData.Instance.GetValue<bool>("isFirstEnter", true);

                if (isFirstEnter)
                {
                    BrushMonSceneManager.Instance.LoadScene(SceneNames.IntroMoviePlay.ToString());
                }
                else
#endif
                {
                    TokenVo tokenVo = ConfigurationData.Instance.GetValueFromJson<TokenVo>("Token");

                    if (tokenVo != null && tokenVo.access_token != null)
                    {
                       RestService.Instance.GetUser(tokenVo, 
                       user =>
                       {
                           signAfter(user);
                       }, 
                       exception =>
                       {
                           LogManager.Log("GetUser Error : " + exception.Message);

                           if ("expires_in".Equals(exception.Message))
                           {
                               RestService.Instance.RefreshToken(tokenVo, user =>
                               {
                                   signAfter(user);
                               }, reexception =>
                               {
                                   LogManager.LogWarning("RefreshToken Error(Token 인증만료) :" + reexception.Message);
                                   BrushMonSceneManager.Instance.LoadScene(SceneNames.SignIn.ToString());
                               });
                           }
                           else
                           {
                               LocalizationManager.Instance.SetLanguage(Application.systemLanguage);
                               LocalizationManager.Instance.SaveVoice(Application.systemLanguage);

                               // GetUser 에러 났을때 처리가 없음 어떻게 해야 하나????
                               BrushMonSceneManager.Instance.LoadScene(SceneNames.SignIn.ToString());
                           }
                       });
                    }
                    else
                    {
                        LocalizationManager.Instance.SaveVoice(Application.systemLanguage);
                        LocalizationManager.Instance.SetLanguage(Application.systemLanguage);

                        //LocalizationManager.Instance.SetLanguage(SystemLanguage.Japanese);
                        //LocalizationManager.Instance.SetLanguage(SystemLanguage.English);
                        //LocalizationManager.Instance.SetLanguage(SystemLanguage.ChineseTraditional);

                        while (LocalizationManager.Instance.IsReady == false) yield return null;
                        BrushMonSceneManager.Instance.LoadScene(SceneNames.SignIn.ToString());
                    }
                }
            }
        }

        
    }

    /// <summary>
    /// 서버 연결 체크
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckConnectServer()
    {
        isCheckEnd = false;
        isConnectServer = false;

        RestService.Instance.CheckConnectServer(
            isConnect =>
            {
                isConnectServer = isConnect;
                isCheckEnd = true;
            }
        );

        while (isCheckEnd == false)
        {
            yield return null;
        }
    }

    void InitializeFirebase()
    {
        LogManager.Log("InitializeFirebase!!!");
        FirebaseManager.Instance.FirebaseInit();
    }

    void signAfter(string user){
        //FirebaseManager.Instance.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventLogin);

        ConfigurationData.Instance.SetJsonValue ("User", user);
        UserVo userVo = ConfigurationData.Instance.GetValueFromJson<UserVo> ("User");
        ProfileVo profileVo = ConfigurationData.Instance.GetValueFromJson<ProfileVo> ("CurrentProfile");

        LocalizationManager.Instance.SetLanguage(LocalizationManager.Instance.LoadLanguage());
        LocalizationManager.Instance.SaveVoice(LocalizationManager.Instance.LoadVoice());

        //userVo 체크
        userVo.app_version = Application.version;
        userVo.device_country = NativePlugin.Instance.GetNativeCountryCode();
        userVo.device_language = NativePlugin.Instance.GetNativeLanguageCode();
        userVo.app_language_text = BMUtil.Instance.GetAppLanguage(LocalizationManager.Instance.LoadLanguage()).ToString();
        userVo.app_language_voice = BMUtil.Instance.GetAppVoice(LocalizationManager.Instance.LoadVoice()).ToString();

        TokenVo tokenVo = ConfigurationData.Instance.GetValueFromJson<TokenVo>("Token");
        ConfigurationData.Instance.SetJsonValue("User", userVo);
        RestService.Instance.UpdateUserSetting(tokenVo, userVo, 
            delegate 
            {
                LogManager.Log("UpdateUserSetting End");
            }, 
            exception =>
            {
                LogManager.Log("UpdateUserSetting Error");
                LogManager.LogError(exception.Message);
            }
        );
        

        if (profileVo != null && profileVo.character_name != null)
        {
#if UNITY_IOS
            TrackingManager.Instance.Tracking ("launch", "IOS");
#elif UNITY_ANDROID
            TrackingManager.Instance.Tracking("launch", "Android");
#endif
            TrackingManager.Instance.Tracking("language", Application.systemLanguage.ToString());


            if (userVo != null)
                TrackingManager.Instance.Tracking("signIn", userVo.user_id);
            BrushMonSceneManager.Instance.LoadScene(SceneNames.Welcome.ToString());
        }
        else
        {
            if (userVo != null)
            {
                if (userVo.profile_count > 0)
                {
                    BrushMonSceneManager.Instance.LoadScene(SceneNames.ProfileSelect.ToString());
                }
                else
                {
                    BrushMonSceneManager.Instance.LoadScene(SceneNames.ProfileCreate.ToString());
                }
            }
            else
            {
                LogManager.LogError("userVO == null");
            }
        }
    }

    void setFirebase()
    {
        //Firebase.FirebaseApp.CheckAndFixDependenciesAsync ().ContinueWith (task => {
        //    dependencyStatus = task.Result;
        //    if (dependencyStatus == Firebase.DependencyStatus.Available)
        //    {
        //        InitializeFirebase();
        //    }
        //    else
        //    {
        //        LogManager.LogError(
        //            "Could not resolve all Firebase dependencies: " + dependencyStatus);
        //    }
        //});

        InitializeFirebase();
    }

//    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
//    {
//        LogManager.Log("Received Registration Token: " + token.Token);

//        Firebase.Messaging.FirebaseMessaging.Subscribe("/topics/All");
//#if UNITY_IOS
//		Firebase.Messaging.FirebaseMessaging.Subscribe ("/topics/IOS");
//#elif UNITY_ANDROID
//        Firebase.Messaging.FirebaseMessaging.Subscribe("/topics/Android");
//#endif

//        SetScribeLanguage();

//        BMUtil.Instance.SetFcmToken(token.Token);
//        TrackingManager.Instance.Tracking("firebase_token", token.Token);
//    }

//    public void SetScribeLanguage()
//    {
//        switch (Application.systemLanguage)
//        {
//            case SystemLanguage.Korean:
//                Firebase.Messaging.FirebaseMessaging.Subscribe("/topics/KR");
//                break;

//            case SystemLanguage.English:
//                Firebase.Messaging.FirebaseMessaging.Subscribe("/topics/EN");
//                break;

//            case SystemLanguage.ChineseTraditional:
//                Firebase.Messaging.FirebaseMessaging.Subscribe("/topics/TW");
//                break;

//            case SystemLanguage.Chinese:
//            case SystemLanguage.ChineseSimplified:
//                Firebase.Messaging.FirebaseMessaging.Subscribe("/topics/CN");
//                break;

//            case SystemLanguage.Japanese:
//                Firebase.Messaging.FirebaseMessaging.Subscribe("/topics/JP");
//                break;

//            default:
//                Firebase.Messaging.FirebaseMessaging.Subscribe("/topics/EN");
//                break;
//        }
//    }

}