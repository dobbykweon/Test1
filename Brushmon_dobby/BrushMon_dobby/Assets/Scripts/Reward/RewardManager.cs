using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class RewardManager : MonoSingleton<RewardManager> {
    private RewardGreenMold GreenMold;
    private SpaceShip SpaceShip;
    private RewardViewPager RewardViewPager;
    private GameObject UIRoot;
    private GameObject LockPanel;
    private RectTransform RewardPanel;
    private PreStickerVo preStickerVo;
    private string FromScene;
    private Image Monster;
    private GameObject ReportDialog;
    private enum Pages { REWARD, REPORT_DIALOG};
    private Pages currentPage = Pages.REWARD;

    bool isBrushEnd = false;
    public bool IsBrushEnd { get { return isBrushEnd; } }

    void Awake()
    {
        UIRoot = GameObject.Find("/UI");
        LockPanel = GameObject.Find("/UI/Panel-Lock");
        GreenMold = GameObject.Find("/UI/Parent/Spine-GreenMold").GetComponent<RewardGreenMold>();
        SpaceShip = GameObject.Find("/UI/Spine-SpaceShip").GetComponent<SpaceShip>();
        Monster = GameObject.Find("/UI/Image-Monster").GetComponent<Image>();
        RewardViewPager = GameObject.Find("/#Reword-VeiwPager").GetComponent<RewardViewPager>();
        RewardPanel = GameObject.Find("/UI/Parent/RewardPanel").GetComponent<RectTransform>();
    }

    void Start () {
        // yield return StartCoroutine(ResourceLoadManager.Instance.Initialize());
        preStickerVo = ConfigurationData.Instance.GetValueFromJson<PreStickerVo> ("PreSticker");
        TrackingManager.Instance.Tracking("reward", preStickerVo.name);
        // Monster.sprite = Resources.Load<Sprite> ("Textures/" + preStickerVo.name);
        Monster.enabled = false;
        ResourceLoadManager.Instance.CoInstantiateGameSpriteAsync (preStickerVo.name, sprite => {
            Monster.sprite = sprite;
            Monster.enabled = true;
        });
        FromScene = SceneDTO.Instance.GetValue ("FromScene");
        LockPanel.SetActive (false);

        AudioManager.Instance.playBGM ("reward_bgm", false);
        if (FromScene.Equals (SceneNames.Brushing.ToString ())) {
            isBrushEnd = true;
            GreenMold.shwoGreenMold ();
        } else {
            isBrushEnd = false;
            GameObject.Find ("/UI/Parent/Spine-GreenMold").SetActive (false);
            RewardViewPager.initailize ();
        }

        Resolution resolution = Screen.currentResolution;
        float ratio = Screen.height/Screen.width;
        if(ratio >= 2){
            RewardPanel.localScale = new Vector3(0.8f,0.8f,0.8f);
        }
    }

    public void openReport (bool isEnableMsg = false) {

        if (isEnableMsg == true)
            FirebaseManager.Instance.LogEvent("br_report_click");
        else
            FirebaseManager.Instance.LogEvent("br_report_open");

        TrackingManager.Instance.Tracking("reward", "Open Report in Reward");
        currentPage = Pages.REPORT_DIALOG;
        ReportDialog = Instantiate (Resources.Load ("Prefabs/Panel-ReportDialog")) as GameObject;
        ReportDialog.transform.SetParent (UIRoot.transform, false);
        ReportDialog.GetComponent<ReportDialog>().Init(isEnableMsg);
    }
    
    IEnumerator showAction (Action action, float delay = 0f) {
        yield return new WaitForSeconds (delay);
        action ();
    }

    internal void showSpaceShip () {
        SpaceShip.shwoSpaceShip ();
        StartCoroutine (
            showAction (delegate
            {
                RewardViewPager.initailize();
                StartCoroutine(showAction(delegate { openReport(); }));
            }, 4.833f)
        );
    }
    
    public void onBackPress () {
        BrushMonSceneManager.Instance.LoadScene (SceneNames.Welcome.ToString ());
    }

    void Update () {
        if (Input.GetKeyUp (KeyCode.Escape)) {
             switch (currentPage) {
                case Pages.REWARD:
                    onBackPress ();
                    return;
                case Pages.REPORT_DIALOG:
                    currentPage = Pages.REWARD;
                    ReportDialog.GetComponent<ReportDialog> ().Close ();
                    return;
            }
            
        }
    }
}