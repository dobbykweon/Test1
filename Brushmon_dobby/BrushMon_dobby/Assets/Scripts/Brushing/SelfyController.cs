using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class SelfyController : MonoBehaviour {

    //[SerializeField] SelfyFaceAR selfyFaceAR;
    [SerializeField] AccessoryManager selfyFaceAR;

    public SkeletonGraphic CountDown;
    [SerializeField] GameObject Back;
    [SerializeField] GameObject objBtnHome;
    [SerializeField] GameObject Shot;
    [SerializeField] GameObject Close;
    // public GameObject Download;
    public GameObject Share;
    public GameObject Toast;
    private string accessory;
    public Image Monster;

    private string[] stickerData;
    private string destinationFile;

    public RawImage screenShot;

    [SerializeField] GameObject objSpaceman;

    private enum Pages { SELFY_BEFORE, SELFY_AFTER, SELFY_SHOOTING };
 private Pages currentPage = Pages.SELFY_BEFORE;

#if UNITY_IOS
 [DllImport ("__Internal")] public static extern void CaptureToCameraRoll (String fileName);
#endif

    string stickerName;

    
    //void Start()
    IEnumerator Start ()
    {
        stickerData = SceneDTO.Instance.GetValue("Sticker").Split(':');
        TrackingManager.Instance.Tracking("try_selfie", stickerData[0]);
        //Monster.sprite = Resources.Load<Sprite> ("Textures/" + stickerData[0]);
        Monster.enabled = false;

        LogManager.Log(stickerData[0] + "/" + stickerData[1] + "/" + stickerData[2]);

        stickerName = stickerData[0];

        ResourceLoadManager.Instance.CoInstantiateGameSpriteAsync(stickerData[0], sprite =>
        {
            Monster.sprite = sprite;
            Monster.enabled = true;
        });
        yield return new WaitForSeconds(1.5f);
        //selfyFaceAR.setCharacterData (stickerData[1], stickerData[2]);
        CountDown.AnimationState.Complete += onComplete;
    }

    public void onClickButton(string type)
    {
        switch (type)
        {
            case "Back":
                onBackPress();
                break;

            case "Shot":
                TrackingManager.Instance.Tracking("take_selfie", stickerData[0]);
                currentPage = Pages.SELFY_SHOOTING;
                CountDown.AnimationState.SetAnimation(0, "Action", false);
                Back.SetActive(false);
                Shot.SetActive(false);
                objBtnHome.SetActive(false);
                break;

            case "Close":
                currentPage = Pages.SELFY_BEFORE;
                Back.SetActive(true);
                Shot.SetActive(true);
                objBtnHome.SetActive(true);
                Close.SetActive(false);
                Share.SetActive(false);
                objSpaceman.SetActive(true);
                selfyFaceAR.SetPauseAndUserLocked(false);
                screenShot.gameObject.SetActive(false);
                break;

            case "Share":
                TrackingManager.Instance.Tracking("share_selfie", stickerData[0]);
                delayedShare();
                break;

        }
    }

    IEnumerator OpenShare()
    {
        yield return new WaitForSeconds(2f);

        TrackingManager.Instance.Tracking("share_selfie", stickerData[0]);
        delayedShare();
    }

    IEnumerator delayedDownload (string filePath) {
        while (!File.Exists (filePath)) {
            yield return new WaitForSeconds (0.5f);
        }
        yield return new WaitForSeconds (1f);

        WWW www;

        string temp_url = "file://"; /// www 를 쓰기 위해선 uri 경로 앞에 file:// 붙어야함

        LogManager.Log("filename : "+temp_url + filePath);

        www = new WWW(temp_url + filePath);

        screenShot.texture = www.texture;

        screenShot.gameObject.SetActive(true);

#if UNITY_IOS
        CaptureToCameraRoll (string.Format ("/{0}", ScreenshotName));
#elif UNITY_ANDROID
        fileMove ();
        // yield return new WaitForSeconds(1f);
        galleryRefresh ();
#endif
        StartCoroutine (DownloadCompleted ());
    }

    IEnumerator DownloadCompleted () {

        selfyFaceAR.SetPauseAndUserLocked(false);

        currentPage = Pages.SELFY_AFTER;
        Close.SetActive (true);
        Share.SetActive (true);
        objSpaceman.SetActive(false);
        objBtnHome.SetActive(true);
        Toast.SetActive (true);
        yield return new WaitForSeconds (1.0f);
        Toast.SetActive (false);
    }
    string ScreenshotName;
    private void onComplete (TrackEntry trackEntry) {
        selfyFaceAR.SetPauseAndUserLocked (true);
        AudioManager.Instance.playEffect ("camera_shutter");
        ScreenshotName = "Screenshot.png";

        screenShotPath = Application.persistentDataPath + "/" + ScreenshotName;
        if (File.Exists (screenShotPath)) File.Delete (screenShotPath);

        ScreenCapture.CaptureScreenshot (ScreenshotName);

        StartCoroutine (delayedDownload (screenShotPath));
    }

    public string screenShotPath;

    //     IEnumerator delayedShare (string text = "BrushMon") {
    //         string targetScreenShotPath = "";

    //         while (!File.Exists (targetScreenShotPath)) {
    //             yield return new WaitForSeconds (0.5f);
    //         }

    // #if UNITY_IOS 
    //         NativeShare.Share (text, screenShotPath, "", "", "image/png", true, "BrushMon");
    // #elif UNITY_ANDROID
    //         androidShare ();
    // #endif
    //     }

    void delayedShare (string text = "BrushMon") {
#if UNITY_IOS 
        NativeShare.Share (text, screenShotPath, "", "", "image/png", true, "BrushMon");
#elif UNITY_ANDROID
        androidShare ();
#endif
    }

    public void onBackPress () {
        selfyFaceAR.Terminate ();
        BrushMonSceneManager.Instance.LoadScene ("Reward");
    }

    void fileMove () {
        if (Application.platform == RuntimePlatform.Android) {
            try {
                string origin = System.IO.Path.Combine (Application.persistentDataPath, ScreenshotName);
                destinationFile = DateTime.Now.ToString ("yyyyMMddHHmmss") + "_" + ScreenshotName;

                using (AndroidJavaClass classPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer")) {
                    using (AndroidJavaObject jo = new AndroidJavaObject ("kr.lithos.applications.brushmon.utils.ShareUtils")) {
                        jo.CallStatic ("fileMove", new object[] { origin, destinationFile });
                    };
                };
            } catch (Exception e) {
                LogManager.Log ("fileMove Exception : " + e.Message);
            }
        }
    }

    public void galleryRefresh () {
        if (Application.platform == RuntimePlatform.Android) {
            try {
                using (AndroidJavaClass classPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer")) {
                    AndroidJavaObject objActivity = classPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
                    using (AndroidJavaObject jo = new AndroidJavaObject ("kr.lithos.applications.brushmon.utils.ShareUtils")) {
                        jo.CallStatic ("galleryRefresh", new object[] { objActivity, destinationFile });
                    };
                };
            } catch (Exception e) {
                LogManager.LogError ("galleryRefresh Exception : " + e.Message);
            }
        }
    }

    public void androidShare () {
        if (Application.platform == RuntimePlatform.Android) {
            try {
                using (AndroidJavaClass classPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer")) {
                    AndroidJavaObject objActivity = classPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
                    using (AndroidJavaObject jo = new AndroidJavaObject ("kr.lithos.applications.brushmon.utils.ShareUtils")) {
                        jo.CallStatic ("share", new object[] { objActivity, destinationFile });
                    };
                };
            } catch (Exception e) {
                LogManager.LogError("androidShare Exception : " + e.Message);
            }
        }
    }

    public void BtnHome()
    {
        selfyFaceAR.Terminate();
        BMUtil.Instance.OpenTitle();
    }

    void Update () {
        if (Input.GetKeyUp (KeyCode.Escape)) {
            switch (currentPage) {
                case Pages.SELFY_BEFORE:
                    onBackPress ();
                    return;
                case Pages.SELFY_SHOOTING:
                    return;
                case Pages.SELFY_AFTER:
                    onClickButton ("Close");
                    return;
            }
        }

        if(stickerName.Equals("sticker_" + BMUtil.Instance.SelectSelfyType.ToString() + "_" + BMUtil.Instance.SelectSelfyIdx.ToString("00")) == false)
        {
            stickerName = "sticker_" + BMUtil.Instance.SelectSelfyType.ToString() + "_" + BMUtil.Instance.SelectSelfyIdx.ToString("00");

            ResourceLoadManager.Instance.CoInstantiateGameSpriteAsync(stickerName, sprite =>
            {
                Monster.sprite = sprite;
                Monster.enabled = true;
            });
        }
    }
}