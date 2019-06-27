﻿#define DRAW_MARKERS
using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using ULSTrackerForUnity;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BrushFaceAR : MonoBehaviour {

#if DRAW_MARKERS
    public TextMesh marker = null;
    List<TextMesh> _marks = new List<TextMesh> ();
#endif

    [SerializeField] private AccessoryController accessoryController;

    public int TargetRegion = 1;
    private float orthographicSizeCustom;
    private float orthographicSizeOri;
    float[] _trackPoints = null;
    bool initDone = false;

    private GameObject Brush;
    private SkeletonAnimation BrushSpine;
    private SkeletonAnimation BrushEffectSpine;

    private SpriteRenderer Hat;
    private SpriteRenderer Glasses;

    private SpriteRenderer activeAccessory;

    private float maxCamSize;       //카메라 최대 사이즈
    private float minCamSize;       //카메라 최소 사이즈
    private float beforeDis = 0;    //이전 사이즈 조정값

    void Start () {
        initailize ();
    }

    private bool isTerminate = false;
    public void Terminate () {
        if (!isTerminate) {
            isTerminate = true;
            Plugins.ULS_UnityTrackerEnable (false);
            Plugins.ULS_UnityTrackerTerminate ();
            Destroy (gameObject);
        }
    }

    bool isEffectRunning = false;
    internal void initailize () {
        Brush = GameObject.Find ("/ARCamera/BrushRoot");
        BrushSpine = GameObject.Find ("/ARCamera/BrushRoot/Spine-Brush").GetComponent<SkeletonAnimation> ();
        BrushEffectSpine = GameObject.Find ("/ARCamera/BrushRoot/Spine-Brush-Effect").GetComponent<SkeletonAnimation> ();
        Hat = GameObject.Find ("/ARCamera/Accessory/Hat").GetComponent<SpriteRenderer> ();
        Glasses = GameObject.Find ("/ARCamera/Accessory/Glasses").GetComponent<SpriteRenderer> ();
        BrushEffectSpine.AnimationState.Start += delegate {
            isEffectRunning = true;
        };
        BrushEffectSpine.AnimationState.Complete += delegate {
            isEffectRunning = false;
        };
        BrushEffectSpine.AnimationState.SetAnimation (0, "Idle", false);
        InitializeTrackerAndCheckKey ();
        _trackPoints = new float[Plugins.MAX_TRACKER_POINTS * 2];
        Application.targetFrameRate = 60;

#if DRAW_MARKERS
        for (int i = 0; i < Plugins.MAX_TRACKER_POINTS; ++i) {
            var g = Instantiate (marker);
            g.transform.parent = transform.parent;
            g.gameObject.SetActive (false);
            _marks.Add (g);
        }
#endif
        accessoryController.Init(_marks);

        if (PlayerPrefs.HasKey("is_reverse"))
        {
            if (PlayerPrefs.GetInt("is_reverse") > 0)
                Camera.main.transform.localEulerAngles = new Vector3(Camera.main.transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, 180);
        }
    }

    void InitializeTrackerAndCheckKey () {
        Plugins.OnPreviewStart = initCameraTexture;

        int initTracker = Plugins.ULS_UnityTrackerInit ();
        if (initTracker < 0) {
            LogManager.Log ("Failed to initialize tracker.");
        } else {
            LogManager.Log ("Tracker initialization succeeded");
        }
        Plugins.ULS_UnityTrackerEnable (true);
    }

    void initCameraTexture(Texture preview, int rotate)
    {
#if (UNITY_IPHONE || UNITY_TVOS) && !UNITY_EDITOR
      if (rotate != -90)
          Camera.main.transform.localEulerAngles = new Vector3(Camera.main.transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, rotate + 90);
#endif

        int w = preview.width;
        int h = preview.height;
        GetComponent<Renderer>().material.mainTexture = preview;

#if UNITY_STANDALONE || UNITY_EDITOR
        transform.localScale = new Vector3(w, h, 1);
        transform.localPosition = new Vector3(w / 2, h / 2, 1);
        transform.parent.localScale = new Vector3(-1, -1, 1);
        transform.parent.localPosition = new Vector3(w / 2, h / 2, 0);
        transform.parent.transform.eulerAngles = new Vector3(0, 0, rotate);
        
        maxCamSize = h / 2f;

#elif UNITY_IOS || UNITY_ANDROID
        int orthographicSize = w / 2;
        if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight) {
            orthographicSize = h / 2;
        }
        transform.localScale = new Vector3 (w, h, 1);
        transform.localPosition = new Vector3 (w / 2, h / 2, 1); //anchor: left-bottom
        transform.parent.localPosition = Vector3.zero; //reset position for rotation
        transform.parent.transform.eulerAngles = new Vector3 (0, 0, rotate); //orientation
        transform.parent.localPosition = transform.parent.transform.TransformPoint (-transform.localPosition); //move to center
        
        maxCamSize = orthographicSize;

#endif

        minCamSize = maxCamSize / 2f;

        Camera.main.orthographicSize = ConfigurationData.Instance.GetValue<float>("CameraZoom", maxCamSize);

        orthographicSizeOri = Camera.main.orthographicSize;
        orthographicSizeCustom = orthographicSizeOri;
        // Camera.main.orthographicSize = ConfigurationData.Instance.GetValue<float>("CameraZoom", orthographicSizeCustom);
        initDone = true;
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = true;
    }

    void Update () {
        if (!initDone)
            return;
        if (0 < Plugins.ULS_UnityGetPoints(_trackPoints))
        {
#if DRAW_MARKERS
            for (int j = 0; j < Plugins.MAX_TRACKER_POINTS; ++j)
            {
                int v = j * 2;
                Vector2 pt = new Vector2(_trackPoints[v], _trackPoints[v + 1]);
                _marks[j].transform.localPosition = (pt);
                _marks[j].transform.eulerAngles = new Vector3(0, 0, 0);
                //_marks[j].gameObject.SetActive(false);
                _marks[j].text = j.ToString();
            }

            if (isTracking)
            {
                Vector2 target = new Vector2();
                switch (TargetRegion)
                {
                    case 1:
                    case 5:
                    case 11:
                        target = Vector2.Lerp(_marks[29].transform.localPosition, _marks[25].transform.localPosition, 0.7f);
                        break;
                    case 6:
                    case 12:
                        target = _marks[29].transform.localPosition;
                        break;
                    case 13:
                    case 2:
                    case 7:
                        target = Vector2.Lerp(_marks[29].transform.localPosition, _marks[22].transform.localPosition, 0.7f);
                        break;
                    case 14:
                    case 3:
                    case 8:
                        target = Vector2.Lerp(_marks[28].transform.localPosition, _marks[22].transform.localPosition, 0.7f);
                        break;
                    case 9:
                    case 15:
                        target = _marks[28].transform.localPosition;
                        break;
                    case 10:
                    case 4:
                    case 16:
                        target = Vector2.Lerp(_marks[28].transform.localPosition, _marks[25].transform.localPosition, 0.7f);
                        break;
                }

                Brush.SetActive(true);

                if (BrushSpine.AnimationState.GetCurrent(0) == null)
                    BrushSpine.AnimationState.SetAnimation(0, currentAnimationName, true);

                if ("hat".Equals(accessoryType))
                {
                    Hat.enabled = true;
                    Glasses.enabled = false;
                }
                else
                {
                    Hat.enabled = false;
                    Glasses.enabled = true;
                }

                Brush.transform.eulerAngles = accessoryController.transform.eulerAngles + Vector3.forward * 180f;
                Brush.transform.localPosition = target;

                float scale = (20f * Plugins.ULS_UnityGetScaleInImage()) / 4f;
                Brush.transform.localScale = new Vector2(scale, scale);

//                float accessoryScale = (20f * Plugins.ULS_UnityGetScaleInImage()) / 2.3f;
//                float x = Vector2.Distance(_marks[9].transform.localPosition, _marks[10].transform.localPosition);
//                Vector2 cap = Vector2.Lerp(_marks[5].transform.localPosition, _marks[8].transform.localPosition, 0.5f);
//#if UNITY_IOS
//                cap.x = cap.x - (x * 2.3f);
//#elif UNITY_ANDROID
//                cap.x = cap.x + (x * 2.3f);
//#endif
                //Hat.transform.localPosition = cap;
                //Hat.transform.localEulerAngles = Vector3.zero;
                //Hat.transform.localScale = new Vector2(accessoryScale * 1f, accessoryScale * 1f);

                //Glasses.transform.localPosition = _marks[9].transform.localPosition;
                //Glasses.transform.localEulerAngles = Vector3.zero;
                //Glasses.transform.localScale = new Vector2(accessoryScale, accessoryScale);

            }
            else
            {
                Brush.SetActive(false);

                if ("hat".Equals(accessoryType))
                {
                    Hat.enabled = false;
                    Glasses.enabled = false;
                }
                else
                {
                    Hat.enabled = false;
                    Glasses.enabled = false;
                }
            }
#endif
        }
        else
        {
            for (int j = 0; j < Plugins.MAX_TRACKER_POINTS; ++j)
            {
                _marks[j].gameObject.SetActive(false);
            }

            Brush.SetActive(false);
            if ("hat".Equals(accessoryType))
            {
                Hat.enabled = false;
                Glasses.enabled = false;
            }
            else
            {
                Hat.enabled = false;
                Glasses.enabled = false;
            }
        }

#if (UNITY_EDITOR || UNITY_STANDALONE_WIN)

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            float size = Camera.main.orthographicSize - (Input.GetAxis("Mouse ScrollWheel") * 100);

            if (size > maxCamSize) size = maxCamSize;
            if (size < minCamSize) size = minCamSize;

            Camera.main.orthographicSize = size;
            ConfigurationData.Instance.SetValue<float>("CameraZoom", size);
        }

#else
        if (Input.touchCount >= 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            float afterDis = Vector2.Distance(touchZero.position, touchOne.position);

            if (beforeDis > 0)
            {
                float ratio = (1f - (beforeDis / afterDis)) / 2f;
                float size = Camera.main.orthographicSize - Camera.main.orthographicSize * ratio;

                if (size > maxCamSize) size = maxCamSize;
                if (size < minCamSize) size = minCamSize;

                Camera.main.orthographicSize = size;

                ConfigurationData.Instance.SetValue<float>("CameraZoom", size);
            }

            beforeDis = afterDis;
        }
        else
        {
            beforeDis = 0;
        }
#endif

    }

    // private string accessoryName;
    private string accessoryType;
    internal void setCharacterData (string accessoryName, string accessoryType) {

        LogManager.Log ("accessoryType: " + accessoryType);
        // this.accessoryName = accessoryName;
        this.accessoryType = accessoryType;
        if ("hat".Equals(accessoryType))
        {
            // Hat.sprite = Resources.Load<Sprite> ("Textures/" + accessoryName);
            Hat.enabled = false;
            ResourceLoadManager.Instance.CoInstantiateGameSpriteAsync(accessoryName, sprite =>
            {
                Hat.sprite = sprite;
                Hat.enabled = true;
            });

        }
        else if ("glasses".Equals(accessoryType))
        {
            // Glasses.sprite = Resources.Load<Sprite> ("Textures/" + accessoryName);
            Glasses.enabled = false;
            ResourceLoadManager.Instance.CoInstantiateGameSpriteAsync(accessoryName, sprite =>
            {
                Glasses.sprite = sprite;
                Glasses.enabled = true;
            });
        }
        else
        {
            // Glasses.sprite = Resources.Load<Sprite> ("Textures/" + accessoryName);
            Hat.enabled = false;
            ResourceLoadManager.Instance.CoInstantiateGameSpriteAsync(accessoryName, sprite =>
            {
                Glasses.sprite = sprite;
                Glasses.enabled = true;
            });
        }
    }

    bool isTracking = false;
    private void setTrackingTooth (bool isTracking) {
        this.isTracking = isTracking;
    }

    private void setToothMatchRegion (int target) {
        LogManager.Log ("AR setToothMatchRegion: " + target);
        TargetRegion = target;
    }

    public void shwoBrushAR (BrushStatus brushStatus, int stepInt, bool isLeft = false) {
        BrushSpine.AnimationState.SetEmptyAnimation (0, 0);
        string step = stepInt.ToString ("D2");
        string animationName = String.Concat ("brush_", step);
        //float audioDuration = 0;
        if (stepInt % 2 == 0) {
            string audioName = String.Concat ("cheerup", new System.Random ().Next (1, 10).ToString ("D2"));
            //audioDuration = AudioManager.Instance.getDuration (audioName);
            AudioManager.Instance.playAudio (audioName, null, 3f, 0);
        }
        if (isLeft && ("06".Equals (step) ||"09".Equals (step) ||"12".Equals (step) ||"15".Equals (step))) {
            animationName = animationName + "_left";
        }
        // float delay = audioDuration == 0 ? 5f : audioDuration + 3f;
        float delay = 6f;

        delay = BrushFSM.Instance.TimeAR;

        StartCoroutine (PlayAnimation (animationName, stepInt, delay));
    }

    public void showBrushEffect () {
        if (!isEffectRunning) {
            BrushEffectSpine.AnimationState.SetAnimation (0, "Event", false);
        }
    }
    private string currentAnimationName;
    IEnumerator PlayAnimation (string animationName, int matchRegion, float delay) {
        currentAnimationName = animationName;
        setToothMatchRegion (matchRegion);
        setTrackingTooth (true);
        BrushSpine.AnimationState.SetAnimation (0, animationName, true);
        yield return new WaitForSeconds (delay);
        setTrackingTooth (false);
        BrushFSM.Instance.NextAction ();

    }
}