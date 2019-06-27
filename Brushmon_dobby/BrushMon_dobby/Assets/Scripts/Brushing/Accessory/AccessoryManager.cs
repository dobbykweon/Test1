using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ULSTrackerForUnity;
using Spine.Unity;
using UnityEngine.SceneManagement;

public enum AccessoryIdx
{
    mask = 0,
    head = 1,
    glasses = 2,
    nose = 3,
    mustache = 4,
    earL = 5,
    earR = 6,
    jaw = 7,
    front_01 = 8,
    front_02 = 9,
    End = 10,
}

public class AccessoryManager : MonoBehaviour
{
    [SerializeField] Renderer ren;

    [SerializeField] List<SpriteRenderer> listAccessoryRen;

    [SerializeField] GameObject brush;
    [SerializeField] SkeletonAnimation brushSpine;
    [SerializeField] SkeletonAnimation brushEffectSpine;
    [SerializeField] List<SkeletonAnimation> greenMold;

    float[] trackPoints = null;

    Transform tLEye;
    Transform tREye;

    bool initDone = false;

    float maxCamSize;       //카메라 최대 사이즈
    float minCamSize;       //카메라 최소 사이즈
    float beforeDis = 0;    //이전 사이즈 조정값

    private void Start()
    {
        Init();

        tLEye = new GameObject().transform;
        tREye = new GameObject().transform;
        tREye.parent  = tLEye.parent = transform.parent;

        GetResource();

        if (SceneManager.GetActiveScene().name.Equals("Selfy"))
        {
            isTracking = true;
        }

        if (brushEffectSpine != null)
        {
            brushEffectSpine.AnimationState.Start += delegate
            {
                SetEffectRunning(true);
            };
            brushEffectSpine.AnimationState.Complete += delegate
            {
                SetEffectRunning(false);
            };

            brushEffectSpine.AnimationState.SetAnimation(0, "Idle", false);
        }

        if (PlayerPrefs.HasKey("is_reverse"))
        {
            if (PlayerPrefs.GetInt("is_reverse") > 0)
                Camera.main.transform.localEulerAngles = new Vector3(Camera.main.transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, 180);
        }
    }

    public void Init()
    {
        Plugins.OnPreviewStart = initCameraTexture;

        int initTracker = Plugins.ULS_UnityTrackerInit();

        if (initTracker < 0)
        {
            LogManager.Log("Failed to initialize tracker.");
        }
        else
        {
            LogManager.Log("Tracker initialization succeeded");
        }

        Plugins.ULS_UnityTrackerEnable(true);

        trackPoints = new float[Plugins.MAX_TRACKER_POINTS * 2];
    }

    private bool isTerminate = false;
    public void Terminate()
    {
        if (!isTerminate)
        {
            isTerminate = true;
            Plugins.ULS_UnityTrackerEnable(false);
            Plugins.ULS_UnityTrackerTerminate();
            //Destroy(gameObject);
        }
    }

    // 리소스 읽기
    void GetResource()
    {
        CharacterType charType = CharacterType.cheese;
        long charidx = 1;

        string resName;

        if (SceneManager.GetActiveScene().name.Equals("Selfy"))
        {
            resName = "_" + BMUtil.Instance.SelectSelfyType.ToString() + "_" + BMUtil.Instance.SelectSelfyIdx.ToString("00");
        }
        else
        {
            PreStickerVo preStickerVo = ConfigurationData.Instance.GetValueFromJson<PreStickerVo>("PreSticker");

            if (preStickerVo != null)
            {
                charidx = (preStickerVo.idx - 1) / 7 / 3 * 7 + (preStickerVo.idx - 1) % 7 + 1;
                charType = (CharacterType)((preStickerVo.idx - 1) / 7 % 3);
            }

            resName = "_" + charType.ToString() + "_" + charidx.ToString("00");
        }
        

        ResourceLoadManager.Instance.CoInstantiateGameSpriteAsync(AccessoryIdx.mask.ToString().ToLower() + resName, sprite => { listAccessoryRen[(int)AccessoryIdx.mask].sprite = sprite; }, ResourceType.accessory);
        ResourceLoadManager.Instance.CoInstantiateGameSpriteAsync(AccessoryIdx.head.ToString().ToLower() + resName, sprite => { listAccessoryRen[(int)AccessoryIdx.head].sprite = sprite; }, ResourceType.accessory);
        ResourceLoadManager.Instance.CoInstantiateGameSpriteAsync(AccessoryIdx.glasses.ToString().ToLower() + resName, sprite => { listAccessoryRen[(int)AccessoryIdx.glasses].sprite = sprite; }, ResourceType.accessory);
        ResourceLoadManager.Instance.CoInstantiateGameSpriteAsync(AccessoryIdx.nose.ToString().ToLower() + resName, sprite => { listAccessoryRen[(int)AccessoryIdx.nose].sprite = sprite; }, ResourceType.accessory);
        ResourceLoadManager.Instance.CoInstantiateGameSpriteAsync(AccessoryIdx.mustache.ToString().ToLower() + resName, sprite => { listAccessoryRen[(int)AccessoryIdx.mustache].sprite = sprite; }, ResourceType.accessory);
        ResourceLoadManager.Instance.CoInstantiateGameSpriteAsync(AccessoryIdx.earL.ToString().ToLower() + resName, sprite => { listAccessoryRen[(int)AccessoryIdx.earL].sprite = sprite; }, ResourceType.accessory);
        ResourceLoadManager.Instance.CoInstantiateGameSpriteAsync(AccessoryIdx.earR.ToString().ToLower() + resName, sprite => { listAccessoryRen[(int)AccessoryIdx.earR].sprite = sprite; }, ResourceType.accessory);
        ResourceLoadManager.Instance.CoInstantiateGameSpriteAsync(AccessoryIdx.jaw.ToString().ToLower() + resName, sprite => { listAccessoryRen[(int)AccessoryIdx.jaw].sprite = sprite; }, ResourceType.accessory);
        ResourceLoadManager.Instance.CoInstantiateGameSpriteAsync(AccessoryIdx.front_01.ToString().ToLower() + resName, sprite => { listAccessoryRen[(int)AccessoryIdx.front_01].sprite = sprite; }, ResourceType.accessory);
        ResourceLoadManager.Instance.CoInstantiateGameSpriteAsync(AccessoryIdx.front_02.ToString().ToLower() + resName, sprite => { listAccessoryRen[(int)AccessoryIdx.front_02].sprite = sprite; }, ResourceType.accessory);
    }

    // 악세사리 이미지의 스케일 계산
    float GetScale()
    {
        // 20f 와 2.3f는 어떻게 나온 숫자인가???
        float scale = (20f * Plugins.ULS_UnityGetScaleInImage()) / 2.3f;
        return scale;
    }

    void initCameraTexture(Texture preview, int rotate)
    {
#if (UNITY_IPHONE || UNITY_TVOS) && !UNITY_EDITOR
        if (rotate == 0)
            Camera.main.transform.localEulerAngles = new Vector3(Camera.main.transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, 90);
#endif

        int w = preview.width;
        int h = preview.height;
        ren.material.mainTexture = preview;


#if UNITY_STANDALONE || UNITY_EDITOR
        ren.transform.localScale = new Vector3(w, h, 1);
        ren.transform.localPosition = new Vector3(w / 2, h / 2, 1);
        ren.transform.parent.localScale = new Vector3(-1, -1, 1);
        ren.transform.parent.localPosition = new Vector3(w / 2, h / 2, 0);
        ren.transform.parent.transform.eulerAngles = new Vector3(0, 0, rotate);

        maxCamSize = h / 2f;

#elif UNITY_IOS || UNITY_ANDROID
        int orthographicSize = w / 2;
        if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight) {
            orthographicSize = h / 2;
        }
        ren.transform.localScale = new Vector3 (w, h, 1);
        ren.transform.localPosition = new Vector3 (w / 2, h / 2, 1); //anchor: left-bottom
        ren.transform.parent.localPosition = Vector3.zero; //reset position for rotation
        ren.transform.parent.transform.eulerAngles = new Vector3 (0, 0, rotate); //orientation
        ren.transform.parent.localPosition = ren.transform.parent.transform.TransformPoint (-ren.transform.localPosition); //move to center
        
        maxCamSize = orthographicSize;
#endif

        minCamSize = maxCamSize / 2f;

        Camera.main.orthographicSize = ConfigurationData.Instance.GetValue<float>("CameraZoom", maxCamSize);

        ren.enabled = true;
        initDone = true;
    }

    private void Update()
    {
        if (initDone == false) return;

        if (0 < Plugins.ULS_UnityGetPoints(trackPoints) && isTracking == true)
        {
            tLEye.localPosition = new Vector2(trackPoints[14 * 2], trackPoints[14 * 2 + 1]);
            tREye.localPosition = new Vector2(trackPoints[20 * 2], trackPoints[20 * 2 + 1]);

            Vector2 relative = tLEye.InverseTransformPoint(tREye.position);
            float angle = Mathf.Atan2(relative.y, relative.x) * Mathf.Rad2Deg;

            for (int i = 0; i < listAccessoryRen.Count; i++)
            {
                listAccessoryRen[i].transform.parent.localPosition = GetTargetPos((AccessoryIdx)i) + Vector3.back * i;
                listAccessoryRen[i].transform.parent.localScale = (Vector3)(Vector2.one * GetScale()) + Vector3.forward;
#if UNITY_STANDALONE || UNITY_EDITOR
                if (i != (int)AccessoryIdx.front_02 && i != (int)AccessoryIdx.front_01)
                    listAccessoryRen[i].transform.parent.eulerAngles = new Vector3(0, 0, angle);
#elif UNITY_ANDROID
                if (i != (int)AccessoryIdx.front_02 && i != (int)AccessoryIdx.front_01)
                    listAccessoryRen[i].transform.parent.eulerAngles = new Vector3(0, 0, angle + 90);
                else
                    listAccessoryRen[i].transform.parent.eulerAngles = new Vector3(0, 0, 180);
#elif UNITY_IOS
                if (i != (int)AccessoryIdx.front_02 && i != (int)AccessoryIdx.front_01)
                    listAccessoryRen[i].transform.parent.eulerAngles = new Vector3(180, 180, angle + 90);
                else
                    listAccessoryRen[i].transform.parent.eulerAngles = new Vector3(180, 180, 0);
#endif
                listAccessoryRen[i].enabled = true;
            }
        }
        else
        {
            for (int i = 0; i < listAccessoryRen.Count; i++)
                listAccessoryRen[i].enabled = false;
        }

        Tracking();


        if (BMUtil.Instance.IsDialog == false)
        {
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
    }

    Vector3 GetTargetPos(AccessoryIdx idx)
    {
        Vector2 pos = new Vector2(trackPoints[GetTargetIdx(idx) * 2], trackPoints[GetTargetIdx(idx) * 2 + 1]);
        return pos;
    }

    int GetTargetIdx(AccessoryIdx idx)
    {
        switch (idx)
        {
            case AccessoryIdx.mask: return 9;
            case AccessoryIdx.head: return 9;
            case AccessoryIdx.glasses: return 9;
            case AccessoryIdx.nose: return 10;
            case AccessoryIdx.mustache: return 12;
            case AccessoryIdx.earL: return 4;
            case AccessoryIdx.earR: return 0;
            case AccessoryIdx.jaw: return 2;
            case AccessoryIdx.front_01: return 2;
            case AccessoryIdx.front_02: return 12;
        }

        return 0;
    }

    bool isTracking = false;
    private void setTrackingTooth(bool isTracking)
    {
        this.isTracking = isTracking;
    }

    bool isEffectRunning = false;
    void SetEffectRunning(bool isRunning)
    {
        isEffectRunning = isRunning;
    }

    int TargetRegion = 1;
    private void setToothMatchRegion(int target)
    {
        //LogManager.Log("AR setToothMatchRegion: " + target);
        TargetRegion = target;
    }

    public void shwoBrushAR(BrushStatus brushStatus, int stepInt, bool isLeft = false)
    {
        brushSpine.AnimationState.SetEmptyAnimation(0, 0);
#if !OFFLINE
        for (int i = 0; i < greenMold.Count; i++)
            greenMold[i].AnimationState.SetEmptyAnimation(0, 0);
#else
        for (int i = 0; i < greenMold.Count; i++)
            greenMold[i].gameObject.SetActive(false);
#endif
            string step = stepInt.ToString("D2");
        string animationName = String.Concat("brush_", step);
        //float audioDuration = 0;
        if (stepInt % 2 == 0)
        {
            string audioName = String.Concat("cheerup", new System.Random().Next(1, 10).ToString("D2"));
            //audioDuration = AudioManager.Instance.getDuration (audioName);
            AudioManager.Instance.playAudio(audioName, null, 3f, 0);
        }
        if (isLeft && ("06".Equals(step) || "09".Equals(step) || "12".Equals(step) || "15".Equals(step)))
        {
            animationName = animationName + "_left";
        }
        // float delay = audioDuration == 0 ? 5f : audioDuration + 3f;
        float delay = 6f;

        delay = BrushFSM.Instance.TimeAR;

        StartCoroutine(PlayAnimation(animationName, stepInt, delay));
    }

    public void showBrushEffect()
    {
        if (!isEffectRunning)
        {
            brushEffectSpine.AnimationState.SetAnimation(0, "Event", false);
            AudioManager.Instance.playEffectNotStop("good_brushing", 0);
        }
    }

    private string currentAnimationName;
    IEnumerator PlayAnimation(string animationName, int matchRegion, float delay)
    {
        currentAnimationName = animationName;
        setToothMatchRegion(matchRegion);
        setTrackingTooth(true);
        brushSpine.AnimationState.SetAnimation(0, animationName, true);
#if !OFFLINE
        for (int i = 0; i < greenMold.Count; i++)
        {
            greenMold[i].gameObject.SetActive(true);
            greenMold[i].AnimationState.SetAnimation(0, "Step_" + (i + 1).ToString("00"), true);
        }

        StartCoroutine(GreenMold());
#endif
        yield return new WaitForSeconds(delay);
        setTrackingTooth(false);
        BrushFSM.Instance.NextAction();
    }

    IEnumerator GreenMold()
    {
        yield return new WaitForSeconds(1.5f);
        greenMold[0].gameObject.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        greenMold[1].gameObject.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        greenMold[2].gameObject.SetActive(false);
    }


    Vector2 trackingTarget;
    private void Tracking()
    {
        if (brush == null) return;

        if (isTracking == false)
        {
            brush.SetActive(false);
            return;
        }

        if(trackingTarget == null) trackingTarget = new Vector2();

        switch (TargetRegion)
        {
            case 1:
            case 5:
            case 11:
                //trackingTarget = Vector2.Lerp(_marks[29].transform.localPosition, _marks[25].transform.localPosition, 0.7f);
                trackingTarget = Vector2.Lerp(new Vector2(trackPoints[29 * 2], trackPoints[29 * 2 + 1]), new Vector2(trackPoints[25 * 2], trackPoints[25 * 2 + 1]), 0.7f);

                break;
            case 6:
            case 12:
                //trackingTarget = _marks[29].transform.localPosition;
                trackingTarget = new Vector2(trackPoints[29 * 2], trackPoints[29 * 2 + 1]);
                break;
            case 13:
            case 2:
            case 7:
                //trackingTarget = Vector2.Lerp(_marks[29].transform.localPosition, _marks[22].transform.localPosition, 0.7f);
                trackingTarget = Vector2.Lerp(new Vector2(trackPoints[29 * 2], trackPoints[29 * 2 + 1]), new Vector2(trackPoints[22 * 2], trackPoints[22 * 2 + 1]), 0.7f);
                break;
            case 14:
            case 3:
            case 8:
                //trackingTarget = Vector2.Lerp(_marks[28].transform.localPosition, _marks[22].transform.localPosition, 0.7f);
                trackingTarget = Vector2.Lerp(new Vector2(trackPoints[28 * 2], trackPoints[28 * 2 + 1]), new Vector2(trackPoints[22 * 2], trackPoints[22 * 2 + 1]), 0.7f);
                break;
            case 9:
            case 15:
                //trackingTarget = _marks[28].transform.localPosition;
                trackingTarget = new Vector2(trackPoints[28 * 2], trackPoints[28 * 2 + 1]);
                break;
            case 10:
            case 4:
            case 16:
                //trackingTarget = Vector2.Lerp(_marks[28].transform.localPosition, _marks[25].transform.localPosition, 0.7f);
                trackingTarget = Vector2.Lerp(new Vector2(trackPoints[28 * 2], trackPoints[28 * 2 + 1]), new Vector2(trackPoints[25 * 2], trackPoints[25 * 2 + 1]), 0.7f);
                break;
        }

        Vector2 relative = tLEye.InverseTransformPoint(tREye.position);
        float angle = Mathf.Atan2(relative.y, relative.x) * Mathf.Rad2Deg;

#if UNITY_STANDALONE || UNITY_EDITOR
        brush.transform.eulerAngles = new Vector3(0, 0, angle) + Vector3.forward * 180f;
#elif UNITY_ANDROID
        brush.transform.eulerAngles = new Vector3(0, 0, angle + 90) + Vector3.forward * 180f;
#elif UNITY_IOS
        brush.transform.eulerAngles = new Vector3(180, 180, angle + 90) + Vector3.forward * 180f;
#endif
        brush.transform.localPosition = (Vector3)trackingTarget + Vector3.back * 10f;
        float scale = (20f * Plugins.ULS_UnityGetScaleInImage()) / 4f;

        brush.transform.localScale = new Vector2(scale, scale);
        brush.SetActive(true);
    }

    public void SetPauseAndUserLocked(bool paused)
    {
        Plugins.ULS_UnityPauseCamera(paused);
    }

    public void BtnLeft()
    {
        if (BMUtil.Instance.SelectStickerIndex > 0) BMUtil.Instance.SelectStickerIndex--;

        BMUtil.Instance.SelectSelfyIdx = (BMUtil.Instance.SelectStickerIndex - 1) / 7 / 3 * 7 + (BMUtil.Instance.SelectStickerIndex - 1) % 7 + 1;
        BMUtil.Instance.SelectSelfyType = (CharacterType)((BMUtil.Instance.SelectStickerIndex - 1) / 7 % 3);

        GetResource();
    }

    public void BtnRight()
    {
        BMUtil.Instance.SelectStickerIndex++;

        BMUtil.Instance.SelectSelfyIdx = (BMUtil.Instance.SelectStickerIndex - 1) / 7 / 3 * 7 + (BMUtil.Instance.SelectStickerIndex - 1) % 7 + 1;
        BMUtil.Instance.SelectSelfyType = (CharacterType)((BMUtil.Instance.SelectStickerIndex - 1) / 7 % 3);

        string resName = "sticker_" + BMUtil.Instance.SelectSelfyType.ToString() + "_" + BMUtil.Instance.SelectSelfyIdx.ToString("00");



        GetResource();
    }
}
