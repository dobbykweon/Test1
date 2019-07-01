using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Dasony.Libs.Android;

public class WelcomeController : MonoBehaviour {
    //도연추가
    Permissions checker;

    [SerializeField] GameObject LockPanel;
    [SerializeField] Image ProfileImage;
    [SerializeField] Text ProfileName;
    [SerializeField] Text userID;
    [SerializeField] Text appVersion;
    [SerializeField] GameObject objBtnHelp;

    [SerializeField] Text txtBrushCnt;
    [SerializeField] GameObject objNoticBtn;


    private float LogoAnimationDelaySeconds = .8f;
    private Animator SidePanelAnimator;
    
    private bool isSkip;
    private GameObject UIRoot;
    
    private TokenVo tokenVo;
    private ProfileVo currentProfileVo;
    GameObject ReportDialog;
    GameObject objHelp = null;

    GameObject objNotice = null;
    GameObject objNoticePopup = null;

    private enum Pages {
        SIDE_MENU,
        WELCOME,
        REPORT_DIALOG
    }

    private Pages currentPage = Pages.WELCOME;

    Pages CurrentPages
    {
        get
        {
            currentPage = (Pages)Enum.Parse(typeof(Pages), ConfigurationData.Instance.GetValue<string>("WellcomePage", Pages.WELCOME.ToString()));
            return currentPage;
        }
        set
        {
            currentPage = value;
            ConfigurationData.Instance.SetValue<string>("WellcomePage", value.ToString());
        }
    }

    private void Awake()
    {
        LogManager.Log("=====WelcomeController.Awake=====");
        //테스트용 데이터 추가
        //MessageManager.Instance.TestAddMsgData();

        //임시로 마지막 데이터 보여주고 확인 누르면 메시지 삭제
        //MessageManager.Instance.OpenLastMessage();

        SidePanelAnimator = GameObject.Find("/UI/Parent2/Panel-SideMenu").GetComponent<Animator>();

        if (CurrentPages == Pages.SIDE_MENU)
            OnClick("Side-Open");
    }

    void Start ()
    {
        //도연추가 권한초기화
        checker= Permissions.Instance;
#if OFFLINE
        txtBrushCnt.text = BrushCountDate + "(" + BrushCount + "회)";
        objNoticBtn.SetActive(false);
#else
        txtBrushCnt.transform.parent.gameObject.SetActive(false);
#endif
        //try
        {
            isSkip = ConfigurationData.Instance.GetValue<bool> ("isSkip", false);
            UIRoot = GameObject.Find ("/UI/Parent");
            SkeletonGraphic monster = GameObject.Find ("/UI/Parent/Spine-Monster").GetComponent<SkeletonGraphic> ();
            SidePanelAnimator = GameObject.Find ("/UI/Parent2/Panel-SideMenu").GetComponent<Animator> ();
            //LockPanel.SetActive (false);

            Resolution resolution = Screen.currentResolution;
            float ratio = Screen.height / Screen.width;
            if (ratio >= 2) {
                monster.rectTransform.localScale = new Vector3 (0.85f, 0.85f, 0.85f);
            }

            tokenVo = ConfigurationData.Instance.GetValueFromJson<TokenVo> ("Token");
            currentProfileVo = ConfigurationData.Instance.GetValueFromJson<ProfileVo> ("CurrentProfile");

            UserVo userVo = ConfigurationData.Instance.GetValueFromJson<UserVo>("User");

            if (currentProfileVo != null) {
                ProfileImage.sprite = Resources.Load<Sprite> ("Textures/profile_" + currentProfileVo.character_name + "_active");
                ProfileName.text = currentProfileVo.name;
                userID.text = userVo.user_id;
            } else {
                ProfileImage.sprite = Resources.Load<Sprite> ("Textures/profile_cheese_active");
                // ProfileName.text = "로그인 해주세요.";
                ProfileName.text = LocalizationManager.Instance.GetLocalizedValue ("side_menu_text_field_description");
                userID.text = "";
            }
            AudioManager.Instance.playBGM ("opening");
            //StartCoroutine (StartLogoAnimation ());

            if(NativePlugin.Instance.GetNativeCountryCode() == "KR" && (userVo == null || userVo.app_language_text == "KR"))
            {
#if !OFFLINE
                objBtnHelp.SetActive(true);
                GetEventList();
#endif
            }

            appVersion.text = "Ver " + Application.version;

            userID.transform.parent.gameObject.SetActive(currentProfileVo != null);

            if (isSkip) return;

            GetPreSticker();

            CheckPushMessageData();
#if OFFLINE
#else
            if (NativePlugin.Instance.GetNativeCountryCode() == "KR" && (userVo == null || userVo.app_language_text == "KR"))
            {
                if (DateTime.Now < new DateTime(2019, 7, 7) && 
                    SceneDTO.Instance.GetValue("FromScene").Equals("IntroBranch"))
                {
                    OpenNoticePopup();
                }
            }
#endif       
            //BMUtil.Instance.CheckAppVersion(action =>
            //{
            //    if(action == false)
            //    {
            //        BMUtil.Instance.OpenDialog("New Version", "새로운 버전이 출시되었습니다.");
            //    }
            //});

        } 
        //catch (Exception e) {
        //    LogManager.LogError (e.Message);
        //}

    }

    void CheckPushMessageData()
    {
        Firebase.Messaging.FirebaseMessage message = MessageManager.Instance.GetData();

        if(message != null)
        {
            string title = "";
            string msg = "";

            if (message.Data.ContainsKey("app_title") == true)
                title = message.Data["app_title"];

            if (message.Data.ContainsKey("app_content") == true)
                msg = message.Data["app_content"];

            BMUtil.Instance.OpenDialog(title, msg);

            MessageManager.Instance.ResetData();
        }
    }

    void GetPreSticker()
    {
        if (currentProfileVo == null) return;

        LogManager.Log("profile_idx : " + currentProfileVo.profile_idx);

        RestService.Instance.GetPreSticker(tokenVo, currentProfileVo.profile_idx, result =>
        {
            LogManager.Log("GetPreSticker : " + result);
            ConfigurationData.Instance.SetJsonValue("PreSticker", result);
        }, exception =>
        {
            LogManager.LogWarning("exception: " + exception.Message);
        });
    }

    void GetEventList()
    {
        RestService.Instance.GetEventList(tokenVo, result =>
        {
            LogManager.Log("GetEventList : " + result);

            objHelp = Instantiate(Resources.Load("Prefabs/UIEventPopup")) as GameObject;
            objHelp.transform.SetParent(GameObject.Find("/UI/Parent2").transform, false);
            objHelp.GetComponent<UIEventPopup>().Init(result);
        },
        exception =>
        {
            LogManager.Log("GetEventList Error : " + exception.Message);
        });
    }

    
    IEnumerator RequestPermission () {
        
        yield return Application.RequestUserAuthorization (UserAuthorization.WebCam);


        //도연추가 안드로이드일경우 카메라권한체크
        Debug.Log("requestpermisson");

#if UNITY_ANDROID && !UNITY_EDITOR
        checker.CheckPermission(Permissions.CAMERA, (permission, granted) =>
        {
            if (!granted)
            {
                //BMUtil.Instance.OpenToast("권한없음");
                //BMUtil.Instance.OpenDialog (
                //     "카메라 권한 설정오류",
                //     "Camera 사용권한을 허락하셔야 사용가능한 기능입니다.\n아이폰 설정 > BrushMon > 카메라 기능을 허용해주세요.");

                LogManager.Log("UserAuthorization.WebCam: false");
        
                  Debug.Log("UserAuthorization.WebCam: false");
                if (!BMUtil.Instance.IsDialog)
                {
                    BMUtil.Instance.OpenDialog(
                    LocalizationManager.Instance.GetLocalizedValue("alert_dialog_permission_title_camera"),
                    LocalizationManager.Instance.GetLocalizedValue("alert_dialog_permission_body_camera")
                    );
                }
            }

            else
            {
                LogManager.Log("UserAuthorization.WebCam: true");
        
               Debug.Log("UserAuthorization.WebCam: true");
                //BMUtil.Instance.OpenToast("권한잇음");
                StartPlay();
            }
        });

#elif UNITY_IOS && !UNITY_EDITOR
        
        if (Application.HasUserAuthorization (UserAuthorization.WebCam)) {
            LogManager.Log ("UserAuthorization.WebCam: true");
        
               Debug.Log("UserAuthorization.WebCam: true");
            StartPlay ();
        } else {
            LogManager.Log ("UserAuthorization.WebCam: false");
        
               Debug.Log("UserAuthorization.WebCam: flase");
            // OpenDialog (
            //     "카메라 권한 설정오류",
            //     "Camera 사용권한을 허락하셔야 사용가능한 기능입니다.\n아이폰 설정 > BrushMon > 카메라 기능을 허용해주세요.");
            BMUtil.Instance.OpenDialog (
            LocalizationManager.Instance.GetLocalizedValue ("alert_dialog_permission_title_camera"),
            LocalizationManager.Instance.GetLocalizedValue ("alert_dialog_permission_body_camera")
            );            
        }
#endif

        yield break;
    }

    public void StartPlay () {
        if (isSkip) {
            SkipForBrushing ();
            return;

        }
        StartCoroutine (LoadScene ());
    }

    IEnumerator StartLogoAnimation () {
        yield return new WaitForSeconds (LogoAnimationDelaySeconds);
        //logo.AnimationState.SetAnimation (0, "Event", false);
        //logo.AnimationState.AddAnimation (0, "Idle", true, 0);
        yield return new WaitForSeconds(0.2f);
        //logoBG.enabled = true;
    }

    public void SkipForSignIn () {
        if (isSkip) {
            // OpenDialog (
            //     "회원전용 기능",
            //     "선택하신 기능은 로그인한 사용자만\n사용할 수 있는 기능입니다.\n회원가입 하시겠습니까?",
            //     "회원가입", "취소", true, true, delegate {
            //         BrushMonSceneManager.Instance.LoadScene (SceneNames.SignIn.ToString ());
            //     });

            
                BMUtil.Instance.OpenDialog(
                LocalizationManager.Instance.GetLocalizedValue("alert_dialog_common_title_members_only"),
                LocalizationManager.Instance.GetLocalizedValue("alert_dialog_common_body_members_only"),

                LocalizationManager.Instance.GetLocalizedValue("sign_button_sign_in"),
                LocalizationManager.Instance.GetLocalizedValue("common_button_cancel"),
                "",

                true, true, false,

                delegate
                {
                    BrushMonSceneManager.Instance.LoadScene(SceneNames.SignIn.ToString());
                }
            );
            

        }
    }

    public void SkipForBrushing () {

        if (isSkip) {
            // OpenDialog (
            //     "주의사항",
            //     "로그인 하지 않으시면 치아관리 기록과 스티커 획득이 되지 않아요.\n꾸준한 치아관리를 위해 회원가입을 진행해주세요.",
            //     "회원가입", "무시하기", true, true, delegate {
            //         BrushMonSceneManager.Instance.LoadScene (SceneNames.SignIn.ToString ());
            //     }, delegate {
            //         StartCoroutine (LoadScene ());
            //     }
            // );

            /*
            BMUtil.Instance.OpenDialog(
            LocalizationManager.Instance.GetLocalizedValue("alert_dialog_common_title_invitation_to_join"),
            LocalizationManager.Instance.GetLocalizedValue("alert_dialog_common_body_invitation_to_join"),
            LocalizationManager.Instance.GetLocalizedValue("sign_button_sign_in"),
            LocalizationManager.Instance.GetLocalizedValue("alert_dialog_common_button_ignore"),
            "",
            true, true, false, delegate {
                BrushMonSceneManager.Instance.LoadScene(SceneNames.SignIn.ToString());
            }, delegate {
                StartCoroutine(LoadScene());
            }
            );*/
            
            LogManager.Log("skipforbrushing isskip true");

            Debug.Log("skipforbrushing isskip  true");

            BMUtil.Instance.OpenDialog(
                LocalizationManager.Instance.GetLocalizedValue("alert_dialog_common_title_invitation_to_join"),
                LocalizationManager.Instance.GetLocalizedValue("alert_dialog_common_body_invitation_to_join"),

                LocalizationManager.Instance.GetLocalizedValue("sign_button_sign_in"),
                LocalizationManager.Instance.GetLocalizedValue("alert_dialog_common_button_ignore"),
                "",

                true, true, false,

                delegate
                {
                    BrushMonSceneManager.Instance.LoadScene(SceneNames.SignIn.ToString());
                },
                delegate
                {
                        StartCoroutine(LoadScene());
                }

                );

        }
    }

    public void OnClick (string target) {
        switch (target) {
            case "Start":

                LogManager.Log("start onclick");
                Debug.Log("start onclick");
                if (!GPSController.Instance.CheckGPS())
                {
                    // GPS 팝업 실행
                    GPSController.Instance.TurnOnGPS();
                }
                else
                {
                    FirebaseManager.Instance.LogEvent("br_main_brushing");
                    StartCoroutine(RequestPermission());
                }

                break;
            case "Profile":
                if (isSkip) {
                    SkipForSignIn ();
                    return;
                }
                ConfigurationData.Instance.SetValue<bool> ("FromWelcome", true);
                BrushMonSceneManager.Instance.LoadScene ("ProfileSelect");
                break;
            case "Setting":
                if (isSkip) {
                    SkipForSignIn ();
                    return;
                }
                ConfigurationData.Instance.SetValue<bool> ("FromWelcome", true);
                BrushMonSceneManager.Instance.LoadScene ("Setting");
                break;
            case "IntroMovie":
                ConfigurationData.Instance.SetValue<bool> ("FromWelcome", true);
                BrushMonSceneManager.Instance.LoadScene ("IntroMoviePlay");
                break;
            case "Side-Open":
                LogManager.Log("Side-Open");
                FirebaseManager.Instance.LogEvent("br_main_menu");
                CurrentPages = Pages.SIDE_MENU;
                SidePanelAnimator.SetTrigger ("Open");
                LockPanel.SetActive (true);
                break;
            case "Side-Close":
                LogManager.Log("Side-Close");
                CurrentPages = Pages.WELCOME;
                SidePanelAnimator.SetTrigger ("Close");
                LockPanel.SetActive (false);
                break;
            case "Reward":

                FirebaseManager.Instance.LogEvent("br_main_sticker");

                if (isSkip) {
                    SkipForSignIn ();
                    return;
                }
                ConfigurationData.Instance.SetValue<bool> ("FromWelcome", true);
                BrushMonSceneManager.Instance.LoadScene ("Reward");
                break;
            case "Report":

                FirebaseManager.Instance.LogEvent("br_main_report");

                if (isSkip) {
                    SkipForSignIn ();
                    return;
                }

                TrackingManager.Instance.Tracking ("welcome_report", "Open Report in Welcome");

                CurrentPages = Pages.REPORT_DIALOG;
                ReportDialog = Instantiate (Resources.Load ("Prefabs/Panel-ReportDialog")) as GameObject;
                ReportDialog.transform.SetParent (UIRoot.transform, false);
                ReportDialog.GetComponent<ReportDialog>().Init();

                break;

            case "Help":
                objHelp = Instantiate(Resources.Load("Prefabs/UIHelp")) as GameObject;
                objHelp.transform.SetParent(GameObject.Find("/UI/Parent2").transform, false);
                objHelp.GetComponent<UIHelp>().SetCloseCallback(HelpCloseCallback);
                OnClick("Side-Close");
                break;
        }
    }

    void HelpCloseCallback()
    {
        OnClick("Side-Open");
    }

    IEnumerator LoadScene () {

        if (isSkip == false)
        {
            bool isLoadPerSticker = false;

            RestService.Instance.GetPreSticker(tokenVo, currentProfileVo.profile_idx, result =>
            {
                LogManager.Log("GetPreSticker : " + result);
                ConfigurationData.Instance.SetJsonValue("PreSticker", result);

                isLoadPerSticker = true;

            }, exception =>
            {
                LogManager.LogError("exception: " + exception.Message);
            });

            while (isLoadPerSticker == false)
            {
                yield return null;
            }
        }

#if OFFLINE
        BrushCount = BrushCount + 1;
#endif

        AudioManager.Instance.playEffect ("connected");
        yield return new WaitForSeconds (0.8f);
        BrushMonSceneManager.Instance.LoadScene ("Brushing");
    }

    //public void OpenDialog (string title, string body, string doneName = "", string cancelName = "", bool isDone = true, bool isCancel = false, Action Done = null, Action Cancel = null) {
    //    GameObject Dialog = Instantiate (Resources.Load ("Prefabs/Panel-Dialog")) as GameObject;
    //    Dialog.transform.SetParent (UIRoot.transform, false);
    //    DialogController controller = Dialog.GetComponent<DialogController> ();
    //    controller.Title.text = title;
    //    controller.Body.text = body;
    //    controller.setActions (Done, Cancel);
    //    controller.SetButtonName (doneName, cancelName);
    //    controller.setButtonVisible (isDone, isCancel);
    //}

    uint exitCount = 0;
    void Update () {

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if(objHelp != null)
            {
                //OnClick("Side-Open");
                //Destroy(objHelp);
                return;
            }

            switch (CurrentPages)
            {
                case Pages.SIDE_MENU:
                    OnClick("Side-Close");
                    return;
                case Pages.REPORT_DIALOG:
                    CurrentPages = Pages.WELCOME;
                    ReportDialog.GetComponent<ReportDialog>().Close();
                    return;
            }

            exitCount++;
            if (!IsInvoking("disableDoubleClick"))
            {
                //StartCoroutine ("showToast");
                BMUtil.Instance.OpenToast(LocalizationManager.Instance.GetLocalizedValue("back_press_toast_destrory"));
                Invoke("disableDoubleClick", 1.5f);
            }
        }
        if (exitCount == 2) {
            CancelInvoke ("disableDoubleClick");
            StopAllCoroutines();
            Application.Quit ();
        }
    }

    void disableDoubleClick () {
        exitCount = 0;
    }

    IEnumerator showToast () {
        GameObject Toast = Instantiate (Resources.Load ("Prefabs/Toast")) as GameObject;
        Toast.transform.SetParent (UIRoot.transform, false);
        Toast.GetComponentInChildren<Text> ().text = LocalizationManager.Instance.GetLocalizedValue ("back_press_toast_destrory");
        Toast.SetActive (true);
        yield return new WaitForSeconds (1.5f);
        Destroy (Toast);
    }

    public void BtnLogout()
    {
        LogManager.Log("BtnLogout");

        BMUtil.Instance.OpenDialog(LocalizationManager.Instance.GetLocalizedValue("setting_button_logout"), LocalizationManager.Instance.GetLocalizedValue("logout_message"), "예", "아니오", true, true, delegate
        {
            ConfigurationData.Instance.CleanAll();
            LocalizationManager.Instance.SetLanguage(Application.systemLanguage);
            BrushMonSceneManager.Instance.LoadScene(SceneNames.SignIn.ToString());
        });

        //ConfigurationData.Instance.CleanAll();
        //LocalizationManager.Instance.SetLanguage(Application.systemLanguage);
        //BrushMonSceneManager.Instance.LoadScene(SceneNames.SignIn.ToString());
    }

    public void BtnEditProfile()
    {
        LogManager.Log("BtnEditProfile");

        if (currentProfileVo != null)
        {
            FirebaseManager.Instance.LogEvent("br_setting_profile_edit");
            ConfigurationData.Instance.SetJsonValue<ProfileVo>("SelectedProfile", currentProfileVo);
            BrushMonSceneManager.Instance.LoadScene(SceneNames.ProfileEdit.ToString());
        }
        else
        {
            BrushMonSceneManager.Instance.LoadScene(SceneNames.SignIn.ToString());
        }
    }

    public void BtnExit()
    {
        BMUtil.Instance.OpenDialog("종료", "브러쉬몬스터를 종료하시겠습니까?", "예", "아니오", true, true, delegate { Application.Quit(); });
    }

    public void OpenNotice()
    {
        objNotice = Instantiate(Resources.Load("Prefabs/UINotice")) as GameObject;
        objNotice.transform.SetParent(GameObject.Find("/UI/Parent2").transform, false);
    }

    void OpenNoticePopup()
    {
        if (PlayerPrefs.HasKey("notic_popup_show"))
        {
            if(PlayerPrefs.GetInt("notic_popup_show") < 1)
            {
                objNoticePopup = Instantiate(Resources.Load("Prefabs/UINoticePopup")) as GameObject;
                objNoticePopup.transform.SetParent(GameObject.Find("/UI/Parent2").transform, false);
                objNoticePopup.transform.Find("Img").GetComponent<Button>().onClick.AddListener(delegate { OpenNotice(); Destroy(objNoticePopup); });
            }
        }
        else
        {
            objNoticePopup = Instantiate(Resources.Load("Prefabs/UINoticePopup")) as GameObject;
            objNoticePopup.transform.SetParent(GameObject.Find("/UI/Parent2").transform, false);
            objNoticePopup.transform.Find("Img").GetComponent<Button>().onClick.AddListener(delegate { OpenNotice(); Destroy(objNoticePopup); });
        }
    }

    //public void SetTestTutorial()
    //{
    //    RectTransform rect = BMUtil.Instance.UIRoot.Find("Button-Start").GetComponent<RectTransform>();

    //    GameObject obj = BMUtil.Instance.LoadPrefab("TutorialItem");
    //    obj.GetComponent<TutorialItem>().SetButton(rect, delegate { TestTutorialAction(); });
    //}

    //void TestTutorialAction()
    //{
    //    LogManager.Log("TestTutorialAction");
    //}

#if OFFLINE
    int BrushCount
    {
        get
        {
            if (PlayerPrefs.HasKey("off_brush_count"))
            {
                return PlayerPrefs.GetInt("off_brush_count");
            }
            else
            {
                PlayerPrefs.SetInt("off_brush_count", 0);
                return 0;
            }
        }
        set
        {
            PlayerPrefs.SetInt("off_brush_count", value);
        }
    }

    string BrushCountDate
    {
        get
        {
            string now = DateTime.Now.ToString("MM.dd");

            if (PlayerPrefs.HasKey("off_brush_count_date"))
            {
                string date = PlayerPrefs.GetString("off_brush_count_date");

                if (now.Equals(date) == false)
                {
                    if(BrushCount <= 1)
                    {
                        PlayerPrefs.SetString("off_brush_count_date", now);
                        return now;
                    }
                    else
                    {
                        return date;
                    }
                }
                else
                {
                    return date;
                }
            }
            else
            {
                PlayerPrefs.SetString("off_brush_count_date", now);
                return now;
            }
        }
    }

    public void ResetBrushCount()
    {
        BMUtil.Instance.OpenDialog("양치 횟수 리셋", "저장된 양치 횟수를 리셋하시겠습니까?", "예", "아니오", true, true, delegate { BrushCount = 0; txtBrushCnt.text = BrushCountDate + "(" + BrushCount + "회)"; });
    }
#endif
}
