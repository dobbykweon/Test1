using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BrushFSM : MonoSingleton<BrushFSM> {

    [SerializeField] private float timeGuide = 3f;
    [SerializeField] private float timeAR = 6.75f;

    public float TimeGuide { get { return timeGuide; } }
    public float TimeAR { get { return timeAR; } }

    public BrushStatus currentBrushStatus = BrushStatus.PREPARE_PASTE;

#if OFFLINE
    BrushStatus[] offlineStatus = {
        BrushStatus.PREPARE_PASTE, BrushStatus.GREEN_MOLD_01, BrushStatus.BRUSH_STANDBY, BrushStatus.BRUSH_BUBBLE_EXPLOSION,
        BrushStatus.BRUSH_GUIDE_01, BrushStatus.BRUSH_AR_GUIDE_01,
        BrushStatus.BRUSH_GUIDE_02, BrushStatus.BRUSH_AR_GUIDE_02,
        BrushStatus.BRUSH_GUIDE_04, BrushStatus.BRUSH_AR_GUIDE_04,
        BrushStatus.BRUSH_GUIDE_03, BrushStatus.BRUSH_AR_GUIDE_03,
        BrushStatus.BRUSH_GUIDE_06, BrushStatus.BRUSH_AR_GUIDE_06,
        BrushStatus.BRUSH_GUIDE_11, BrushStatus.BRUSH_AR_GUIDE_11,
        BrushStatus.BRUSH_GUIDE_13, BrushStatus.BRUSH_AR_GUIDE_13,
        BrushStatus.FINISH_TONGUE, BrushStatus.FINISH_BRUSH, BrushStatus.FINISH_WATER, BrushStatus.FINISH_GARGLE, BrushStatus.FINISH_CUP};

    int offlineCnt = 0;
#endif

    public enum ActivityStatus {
        ACTIVE,
        // PAUSE, 
        // RESUME, 
        PAUSED,
        OUT_PAUSE
    }

    [SerializeField] private ActionGuide ActionGuide;
    [SerializeField] private BrushGuide BrushGuide;
    [SerializeField] private GreenMold GreenMold;
    [SerializeField] private Gradation Gradation;
    [SerializeField] private BubbleMonster BubbleMonster;
    //private BrushFaceAR BrushFaceAR;
    [SerializeField] private AccessoryManager BrushFaceAR;

    private GameObject UIRoot;

    public ActivityStatus currentActivityStatus = ActivityStatus.ACTIVE;
    [SerializeField] private SkeletonGraphic BluetoothStatus;
    [SerializeField] private GameObject BrushConnectText;
    private bool isSkip;
    private TokenVo tokenVo;
    private PreStickerVo preStickerVo;
    private ProfileVo currentProfileVo;
    private UserVo userVo;

    private bool hasBrush = false;

    // string currentState = null;
    bool isCommandable = false;

    float pauseTime = 0;
    float startPauseTime = 0;

    [SerializeField] private SkeletonGraphic introMovie;
    [SerializeField] private GameObject objUIBrush;

    [SerializeField] uTools.TweenAlpha tweenPade;

    private void OnBleStatus (string state) {
        LogManager.Log ("=====onBleStatus: " + state);

        // currentState = state;
        switch (state) {
            // 스캔시작
            case "Scanning":
                BluetoothStatus.AnimationState.SetAnimation (0, "Scan", true);
                break;
            
            // 스캔종료
            case "OnScanned":
                if (currentBrushStatus.Equals(BrushStatus.BRUSH_STANDBY))
                {
                    BubbleMonster.BuubleClick();
                }

                StopCoroutine("CheckConnect");
                StartCoroutine("CheckConnect");
                break;

            // 스캔중지
            case "OnScanStoped":
                break;

            // 연결후 연동준비 완료
            case "OnCommandable":

                //if (currentBrushStatus.Equals (BrushStatus.BRUSH_STANDBY)) {
                //    BubbleMonster.BuubleClick ();
                //}
                //BLEManager.Instance.sendDataDelay ("s", 0.4f);
                //BLEManager.Instance.setToothBrushing (true);

                StopCoroutine("CheckConnect");

                StartCoroutine(StartBrushing());
                
                BluetoothStatus.AnimationState.SetAnimation(0, "Connect", true);

                hasBrush = true;
                isCommandable = true;
                break;

            // 연결성공
            case "OnConnected":
                hasBrush = true;
                BluetoothStatus.AnimationState.SetAnimation (0, "Connect", true);
                break;

            case "OnDisconnected":
                BLEManager.Instance.setToothBrushing (false);
                BluetoothStatus.AnimationState.SetAnimation (0, "Disconnect", true);
                break;
        }
        setDeviceVibration (state);
        BluetoothStatus.gameObject.SetActive(hasBrush);
    }

    void setDeviceVibration (string data) {
        if (data.IndexOf ("DeviceVibration") > -1) {
            string vibration = data.Split (':') [1];
            if (userVo != null && userVo.vibration.ToString () != vibration) {
                BLEManager.Instance.sendDataDelay (userVo.vibration.ToString (), 0.2f);
            }
        }
    }

    internal void OnValidateToothBehavior (int result) {
        if (result == 1 && currentBrushStatus.ToString ().StartsWith ("BRUSH_AR_GUIDE")) {
            BrushFaceAR.showBrushEffect ();
            //AudioManager.Instance.playEffectNotStop ("good_brushing", 0);
        }
    }

    internal void OnKeyStateBehavior (int key) {

        switch (currentActivityStatus) {
            case ActivityStatus.PAUSED:
                GamePlay ();
                break;
            case ActivityStatus.ACTIVE:
                GamePause (false);
                break;
        }
    }

    void OnApplicationPause (bool pause) {
        if (currentBrushStatus >= BrushStatus.BRUSH_STANDBY) {
            if (pause) {
                GamePause (false);
            } else {
                pauseTime = Time.realtimeSinceStartup - startPauseTime;
                GamePlay ();
            }
        }
    }

    void Start()
    {
        isFinished = false;
        getPreSticker();
        if (preStickerVo != null)
            TrackingManager.Instance.Tracking("brushing_start", preStickerVo.name);
        UIRoot = GameObject.Find("/UI");
        //ActionGuide = GameObject.Find("/UI/Parent/Spine-ActionGuide").GetComponent<ActionGuide>();
        //GreenMold = GameObject.Find("/UI/Parent/Spine-GreenMold").GetComponent<GreenMold>();
        //Gradation = GameObject.Find("/UI/Fillter-Gradation").GetComponent<Gradation>();
        //BubbleMonster = GameObject.Find("/UI/Spine-Bubble").GetComponent<BubbleMonster>();
        //BrushGuide = GameObject.Find("/UI/Parent/Spine-BrushGuide").GetComponent<BrushGuide>();
        
        //BrushFaceAR = GameObject.Find("/ARCamera/Accessory").GetComponent<AccessoryManager>();
        //BluetoothStatus = GameObject.Find("/UI/Spine-BluetoothStatus").GetComponent<SkeletonGraphic>();
        //BrushConnectText = GameObject.Find("/UI/BrushConnectText");
        BrushConnectText.SetActive(false);
        isSkip = ConfigurationData.Instance.GetValue<bool>("isSkip", false);

        tokenVo = ConfigurationData.Instance.GetValueFromJson<TokenVo>("Token");
        userVo = ConfigurationData.Instance.GetValueFromJson<UserVo>("User");
        currentProfileVo = ConfigurationData.Instance.GetValueFromJson<ProfileVo>("CurrentProfile");

        string beforeBrushStatus = SceneDTO.Instance.GetValue("currentBrushStatus");
        if (!string.IsNullOrEmpty(beforeBrushStatus))
        {
            currentBrushStatus = (BrushStatus)Enum.Parse(typeof(BrushStatus), beforeBrushStatus);
        }
        BLEManager.Instance.setOnBleStatus(OnBleStatus);
        BLEManager.Instance.setOnKeyStateBehavior(OnKeyStateBehavior);
        BLEManager.Instance.setOnValidateToothBehavior(OnValidateToothBehavior);
        BluetoothStatus.AnimationState.SetEmptyAnimation(0, 0);
        //AudioManager.Instance.playBGM("bgm1");
        Gradation.SetGradation(currentBrushStatus);

        objUIBrush.SetActive(false);

#if UNITY_EDITOR
        //테스트용 가이드/AR 시간 셋팅
        timeGuide = PlayerPrefs.GetFloat("timeGuide");
        timeAR = PlayerPrefs.GetFloat("timeAR");
#endif

        PlayAction(currentBrushStatus);

    }

    void getPreSticker () {
        preStickerVo = ConfigurationData.Instance.GetValueFromJson<PreStickerVo> ("PreSticker");

        if (preStickerVo == null)
        {
            preStickerVo = new PreStickerVo();
            preStickerVo.accessory_name = "accessory_cheese_01";
            preStickerVo.accessory_type = "hat";
            preStickerVo.name = "sticker_cheese_01";
        }
    }

    bool isFinished = false;
    internal void PlayAction(BrushStatus brushStatus)
    {
        if (!isFinished)
        {
            ActionGuide.InitAnimation();
            GreenMold.InitAnimation();
            BrushGuide.InitAnimation();

            currentBrushStatus = brushStatus;
            Gradation.SetGradation(currentBrushStatus);
            PlayBGM();
            AudioManager.Instance.StopEffect("good_brushing");

            if (currentBrushStatus.Equals(BrushStatus.INTRO_MOVIE))
            {
                if (PlayerPrefs.HasKey("is_intro_movie"))
                {
                    if (PlayerPrefs.GetInt("is_intro_movie") > 0)
                    {
                        introMovie.transform.parent.gameObject.SetActive(true);

                        AudioManager.Instance.playAudio("story_movie_audio", null);
                        introMovie.AnimationState.Complete += delegate
                        {
                            //introMovie.transform.parent.gameObject.SetActive(false);
                            //PlayAction(BrushStatus.PREPARE_PASTE);

                            StartCoroutine(FadeIntroMovie());
                        };

                        introMovie.AnimationState.SetAnimation(0, "Scene_all", false);
                    }
                    else
                    {
                        introMovie.transform.parent.gameObject.SetActive(false);
                        PlayAction(BrushStatus.PREPARE_PASTE);
                    }
                }
                else
                {
                    introMovie.transform.parent.gameObject.SetActive(false);
                    PlayAction(BrushStatus.PREPARE_PASTE);
                }
            }
            else if (currentBrushStatus.Equals(BrushStatus.PREPARE_PASTE))
            {
                objUIBrush.SetActive(true);
                AudioManager.Instance.playBGM("bgm1");

                if (isSkip)
                {
                    ActionGuide.shwoActionGuide(currentBrushStatus, false);
                    return;
                }

                if (userVo.paste_exist)
                    ActionGuide.shwoActionGuide(currentBrushStatus, currentProfileVo.hand_type == "left");
                else
                    NextAction();
            }
            else if (currentBrushStatus.ToString().StartsWith("FINISH"))
            {
                BluetoothStatus.enabled = false;
                BLEManager.Instance.disconnect();

                if (isSkip)
                {
                    ActionGuide.shwoActionGuide(currentBrushStatus, false);
                    return;
                }
                if (userVo.clean_up_exist)
                    ActionGuide.shwoActionGuide(currentBrushStatus, currentProfileVo.hand_type == "left");
                else
                    FinishBrusing();
            }
            else if (currentBrushStatus.ToString().StartsWith("GREEN_MOLD"))
            {
                GreenMold.shwoGreenMold(currentBrushStatus);
                if (currentBrushStatus.Equals(BrushStatus.GREEN_MOLD_01))
                {
                    if (!BLEManager.Instance.isConnected())
                        BLEManager.Instance.startScan();
                }
            }
            else if (currentBrushStatus.Equals(BrushStatus.BRUSH_STANDBY))
            {
                BrushConnectText.SetActive(true);
                //BrushFaceAR.setCharacterData(preStickerVo.accessory_name, preStickerVo.accessory_type);
                BubbleMonster.shwoStandBy(currentBrushStatus, preStickerVo.name);

                //if (BLEManager.Instance.isConnected() && BubbleMonster.isBubblExplosion())
                //{
                //    BubbleMonster.BuubleClick();

                //    return;
                //}
            }
            else if (currentBrushStatus.Equals(BrushStatus.BRUSH_BUBBLE_EXPLOSION))
            {
                BrushConnectText.SetActive(false);
                BubbleMonster.BubblExplosion();
            }
            else if (currentBrushStatus.ToString().StartsWith("BRUSH_GUIDE"))
            {
                LogManager.Log("currentBrushStatus : " + currentBrushStatus);
                BubbleMonster.EarthBubbleMonster();

                if (isSkip)
                {
                    BrushGuide.shwoBrushGuide(currentBrushStatus, false);

                    if (currentBrushStatus.Equals(BrushStatus.BRUSH_GUIDE_01))
                    {
                        StartCoroutine(EndBrushGuide(TimeGuide + 3));
                    }
                    else
                    {
                        StartCoroutine(EndBrushGuide(TimeGuide));
                    }
                    return;
                }

                BrushGuide.shwoBrushGuide(currentBrushStatus, currentProfileVo.hand_type == "left");

                if (currentBrushStatus.Equals(BrushStatus.BRUSH_GUIDE_01))
                {
                    StartCoroutine(EndBrushGuide(TimeGuide + 3));
                }
                else
                {
                    StartCoroutine(EndBrushGuide(TimeGuide));
                }
            }
            else if (currentBrushStatus.ToString().StartsWith("BRUSH_AR_GUIDE"))
            {
                string step = currentBrushStatus.ToString().Substring(currentBrushStatus.ToString().LastIndexOf("_") + 1);
                int stepInt = Int32.Parse(step);
                BLEManager.Instance.setToothMatchRegion(stepInt);
                if (isSkip)
                {
                    BrushFaceAR.shwoBrushAR(currentBrushStatus, stepInt, false);
                    return;
                }
                BrushFaceAR.shwoBrushAR(currentBrushStatus, stepInt, currentProfileVo.hand_type == "left");
            }
            //LogManager.Log("============================================");
            //LogManager.Log("currentBrushStatus: " + currentBrushStatus + " / " + brushStatus);
            //LogManager.Log("============================================");
        }
    }

    IEnumerator EndBrushGuide(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        AudioManager.Instance.playEffect("phase_change");
        NextAction();
        BrushGuide.EnableBrushGuideUI(false);
        BrushFSM.Instance.hideGradation();
    }

    internal void hideGradation () {
        Gradation.hideGradation ();
    }

    IEnumerator PlayAction(BrushStatus target, float beforeDelay = 0f)
    {
        //LogManager.Log("PlayAction : " + target.ToString() + "(" + beforeDelay + ")");

        yield return new WaitForSeconds(beforeDelay);
        PlayAction(target);
    }

    private IEnumerator playCoroutine;
    internal void NextAction (float beforeDelay = 0f)
    {
        LogManager.Log("NextAction : " + beforeDelay);

        if (!isFinished) {
            try {
                BrushStatus status = GetNextBrushStatus ();
                StartCoroutine (playCoroutine = PlayAction (status, beforeDelay));

            } catch (IndexOutOfRangeException e) {
                LogManager.Log (e.Message);
                FinishBrusing ();
            }
        }
    }

    public void FinishBrusing () {
        BrushFaceAR.Terminate ();
        if (isSkip) {
            BrushMonSceneManager.Instance.LoadScene ("Welcome");
            return;
        }
        int[] brushingResult;
        if (hasBrush)
        {
            brushingResult = BLEManager.Instance.getBrushingResult();
        }
        else
        {
            int[] partScore = { -1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            brushingResult = partScore;
        }


#if OFFLINE
        brushingResult[5] = 1;
        brushingResult[7] = 1;
        brushingResult[8] = 1;
        brushingResult[9] = 1;
        brushingResult[10] = 1;
        brushingResult[12] = 1;
        brushingResult[14] = 1;
        brushingResult[15] = 1;
        brushingResult[16] = 1;
#endif

        BrushingResultDTO brushingResultDTO = new BrushingResultDTO ();
        brushingResultDTO.profile_idx = currentProfileVo.profile_idx;
        brushingResultDTO.sticker_name = preStickerVo.name;
        List<string> resultList = new List<string> ();

        for (int i = 1; i < brushingResult.Length; i++)
        {
#if OFFLINE
            resultList.Add (brushingResult[i] == 0 ? "BAD" : "GOOD");
#else
            resultList.Add(brushingResult[i] == 0 ? "bad" : "good");
#endif
        }
        brushingResultDTO.result_list = resultList;
        brushingResultDTO.create_date = CommonUtil.GetGMTInMS();
        brushingResultDTO.use_brush = hasBrush;
#if UNITY_IOS
        brushingResultDTO.os = "IOS";
#elif UNITY_ANDROID
        brushingResultDTO.os = "Android";
#endif
        FirebaseManager.Instance.LogEvent("br_brushing_finish", "use_brush", hasBrush ? "true" : "false");
            
        TrackingManager.Instance.Tracking("brushing_complete", string.Join(",", resultList.ToArray()));

#if OFFLINE
        PlayerPrefs.SetInt("sticker_idx", (PlayerPrefs.GetInt("sticker_idx", 0) + 1) % 21);
#endif

        //RestService.Instance.RegisterBrushingData (tokenVo, brushingResultDTO, delegate {
        //    ConfigurationData.Instance.SetValue<bool> ("FromWelcome", false);
        //    BrushMonSceneManager.Instance.LoadScene ("Reward");
        //}, exception => { });

        SendBrushData(brushingResultDTO);
#if UNITY_EDITOR
        isEnd = true;
#endif
    }

    void SendBrushData(BrushingResultDTO brushingResultDTO)
    {
        LogManager.Log("SendBrushData : " + JsonUtility.ToJson(brushingResultDTO));

        RestService.Instance.CheckConnectServer(
            result => {

                FirebaseManager.Instance.LogEvent("br_brushing_finish", "use_brush", hasBrush ? "true" : "false");

                if (result == true)
                {
                    TrackingManager.Instance.Tracking("brushing_complete", string.Join(",", brushingResultDTO.result_list.ToArray()));

                    RestService.Instance.RegisterBrushingData(tokenVo, brushingResultDTO, 
                        delegate
                        {
                            ConfigurationData.Instance.SetValue<bool>("FromWelcome", false);
                            BrushMonSceneManager.Instance.LoadScene("Reward");
                        }, 
                        exception =>
                        {
                            BMUtil.Instance.OpenDialog(LocalizationManager.Instance.GetLocalizedValue("net_connect_error_title"),
                                LocalizationManager.Instance.GetLocalizedValue("net_connect_error_message"),
                                LocalizationManager.Instance.GetLocalizedValue("common_button_done"), LocalizationManager.Instance.GetLocalizedValue("net_connect_retry"),
                                true, true,
                                delegate { BrushMonSceneManager.Instance.LoadScene("Welcome"); }, delegate { SendBrushData(brushingResultDTO); }
                                );
                        });
                }
                else
                {

                    BMUtil.Instance.OpenDialog(LocalizationManager.Instance.GetLocalizedValue("net_connect_error_title"),
                        LocalizationManager.Instance.GetLocalizedValue("net_connect_error_message"),
                        LocalizationManager.Instance.GetLocalizedValue("common_button_done"), LocalizationManager.Instance.GetLocalizedValue("net_connect_retry"),
                        true, true,
                        delegate { BrushMonSceneManager.Instance.LoadScene("Welcome"); }, delegate { SendBrushData(brushingResultDTO); }
                        );
                }
            });
    }


    private BrushStatus GetNextBrushStatus()
    {
#if OFFLINE

        List<ProfileVo> listProfile = JsonHelper.getJsonArray<ProfileVo>(PlayerPrefs.GetString("OFF_ProfileList"));

        if (currentProfileVo.profile_idx == listProfile[0].profile_idx)
        {
            offlineCnt++;
            BrushStatus status = offlineStatus[offlineCnt];
            return status;
        }
        else
        {
            return (BrushStatus)GetBrushStatusArray().GetValue((int)currentBrushStatus + 1);
        }
#else
        return (BrushStatus)GetBrushStatusArray().GetValue((int)currentBrushStatus + 1);
#endif
    }

    private Array GetBrushStatusArray () {
        return Enum.GetValues (typeof (BrushStatus));
    }

    private void PlayBGM () {
        if (currentBrushStatus <= BrushStatus.BRUSH_AR_GUIDE_06) {
            PlayBGM ("bgm1");
        } else if (currentBrushStatus <= BrushStatus.BRUSH_AR_GUIDE_10) {
            PlayBGM ("bgm2");
        } else if (currentBrushStatus <= BrushStatus.BRUSH_AR_GUIDE_16) {
            PlayBGM ("bgm3");
        } else if (currentBrushStatus <= BrushStatus.FINISH_WATER) {
            PlayBGM ("bgm4");
        }
    }

    internal void PlayBGM (string name) {
        if (!AudioManager.Instance.isPlayingBGM (name)) {
            AudioManager.Instance.playBGM (name);
        }
    }

    float keyTime = 0;

    void Update () {
#if !UNITY_EDITOR
        if (Application.platform == RuntimePlatform.Android)
#endif
        {
            if (Time.realtimeSinceStartup - keyTime > 0.2f)
            {
                if (Input.GetKey(KeyCode.Escape))
                {
                    keyTime = Time.realtimeSinceStartup;

                    if (PausePanel == null)
                    {
                        GamePause(false);
                    }
                    else
                    {
                        PausePanel.GetComponent<BrushPauseControler>().Play();
                    }

                    return;
                }
            }
        }

        if (hasBrush == true &&                                        // 칫솔을 사용하고
            currentActivityStatus == ActivityStatus.ACTIVE &&          // 양치중이고
            currentBrushStatus > BrushStatus.BRUSH_BUBBLE_EXPLOSION && // 물방울 터트리고 나서부터
            currentBrushStatus < BrushStatus.FINISH_TONGUE &&          // 양치후 정리 직전까지
            BLEManager.instance.IsConnectPeripheral == false)
        {
            GamePause(false);
            // 일시정지 후 팝업 보여주기
            BMUtil.Instance.OpenDialog(
                LocalizationManager.Instance.GetLocalizedValue("title_bluetooth_connect_error"),    //"블루투스 연결끊김"
                LocalizationManager.Instance.GetLocalizedValue("msg_bluetooth_connect_error"),    //"칫솔과 앱의 연결이 끊어졌습니다.\n스마트폰의 블루투스 상태를 확인해 주세요.\n메인화면으로 돌아갑니다.", 
                LocalizationManager.Instance.GetLocalizedValue("common_button_done"), "", "", 
                true, false, false, onBackPress);
        }
    }

    public void onBackPress () {
        LogManager.Log ("onBackPress()");
        FirebaseManager.Instance.LogEvent("br_brushing_cancel", "BrushStatus", currentBrushStatus.ToString());
        TrackingManager.Instance.Tracking("brushing_stop", currentBrushStatus.ToString());
        isFinished = true;
        Time.timeScale = 1f;
        if (playCoroutine != null) {
            StopCoroutine (playCoroutine);
        }

        StartCoroutine(Disconnect());
    }

    IEnumerator Disconnect()
    {
        BLEManager.Instance.disconnect();

        while (BLEManager.instance.IsEndDisconnect == false)
        {
            yield return null;
        }

        BrushFaceAR.Terminate();
        BrushMonSceneManager.Instance.LoadScene("Welcome");
    }

    public void GamePlay()
    {
        if (pauseTime > 180f)
        {
            BMUtil.Instance.OpenDialog(
                LocalizationManager.Instance.GetLocalizedValue("title_brush_stop"),    //"양치중단", 
                LocalizationManager.Instance.GetLocalizedValue("msg_brush_stop"),    //"일시정지시간이 3분이 초과되어 양치가 종료되었습니다.\n양치를 다시 시작해주세요.\n메인화면으로 돌아갑니다.", 
                LocalizationManager.Instance.GetLocalizedValue("common_button_done"), "", "", 
                true, false, false, 
                onBackPress);
            return;
        }

        if (PausePanel != null) Destroy(PausePanel);
        currentActivityStatus = ActivityStatus.ACTIVE;
        Time.timeScale = 1f;
        AudioManager.Instance.Pause(false);
        BLEManager.Instance.sendData("s");
#if UNITY_EDITOR
        isPause = false;
#endif
    }

    GameObject PausePanel;
    public void GamePause () {
        currentActivityStatus = ActivityStatus.PAUSED;
        Time.timeScale = 0f;
        BLEManager.Instance.sendData ("p");
        AudioManager.Instance.Pause (true);
        PausePanel = Instantiate (Resources.Load ("Prefabs/Panel-Pause")) as GameObject;
        PausePanel.SetActive (true);
        PausePanel.transform.SetParent (UIRoot.transform, false);
    }

    public void GamePause(bool isEnablePlayBtn)
    {
        if (PausePanel != null) return;

        string btnText = LocalizationManager.Instance.GetLocalizedValue("app_button_resume");
        GamePause();
        PausePanel.GetComponent<BrushPauseControler>().setPlayButton(true, btnText);

        StartCoroutine("StartPauseTimeCheck");

#if UNITY_EDITOR
        isPause = true;
#endif
    }

    void OnApplicationQuit () {
        LogManager.Log ("OnApplicationQuit");
        BLEManager.Instance.disconnect ();
        BrushFaceAR.Terminate ();
    }

    IEnumerator StartBrushing()
    {
        while (currentBrushStatus < BrushStatus.BRUSH_STANDBY)
        {
            yield return null;
        }

        BubbleMonster.BuubleClick();

        BLEManager.Instance.sendData("s");
        BLEManager.Instance.setToothBrushing(true);
    }

    IEnumerator StartPauseTimeCheck()
    {
        LogManager.Log("Bluetooth Start Pause");

        pauseTime = 0;
        startPauseTime = Time.realtimeSinceStartup;

        while (currentActivityStatus == ActivityStatus.PAUSED)
        {
            yield return null;
            pauseTime = Time.realtimeSinceStartup - startPauseTime;
        }

        if (currentActivityStatus == ActivityStatus.ACTIVE && pauseTime > 0)
        {
            LogManager.Log("Bluetooth 정지시간 : " + pauseTime + "초");
        }
    }

    IEnumerator CheckConnect()
    {
        while (BLEManager.instance.IsConnectPeripheral == true)
        {
            yield return null;
        }

        // 스켄 재시작
        BLEManager.Instance.startScan();
    }

    IEnumerator FadeIntroMovie()
    {
        tweenPade.gameObject.SetActive(true);
        tweenPade.PlayForward();

        yield return new WaitForSeconds(1.5f);

        introMovie.transform.parent.gameObject.SetActive(false);
        objUIBrush.SetActive(true);
        tweenPade.PlayReverse();

        yield return new WaitForSeconds(1.5f);

        tweenPade.gameObject.SetActive(false);
        PlayAction(BrushStatus.PREPARE_PASTE);
    }

#if UNITY_EDITOR
    bool isEnd = false;
    bool isPause = false;
    public IEnumerator StartBrushTimeCheck()
    {
        float time = 0;
        isEnd = false;

        while (isEnd == false)
        {
            if(isPause == false)
                time += Time.deltaTime;

            yield return null;
        }

        LogManager.Log("End Time : " + time);
    }
#endif

}