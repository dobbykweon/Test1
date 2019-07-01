using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SetUIArea : MonoBehaviour {


    // 도비추가
    private const float inchToCm = 2.54f;

    [SerializeField] private EventSystem eventSystem = null;

    [SerializeField] private float dragThresholdCM = 0.5f;

    private void SetDragThreshold()
    {
        if (eventSystem != null)
        {
            eventSystem.pixelDragThreshold = (int)(dragThresholdCM * Screen.dpi / inchToCm);
        }
    }

    private void Awake()
    {
        SetDragThreshold();
    }

    // Use this for initialization
    void Start()
    {

        RectTransform tUIParent = transform.GetComponent<RectTransform>();

#if UNITY_EDITOR
        /*
        Rect safeArea = new Rect(0, 102, 1125, 2202);
        float width = 1125;
        float height = 2436;

        tUIParent.anchorMin = new Vector2(safeArea.x / width, safeArea.y / height);
        tUIParent.anchorMax = new Vector2((safeArea.x + safeArea.width) / width, (safeArea.y + safeArea.height) / height);
        */
#else
        if (Screen.safeArea.width != Screen.width || Screen.safeArea.height != Screen.height)
        {
            tUIParent.anchorMin = new Vector2(Screen.safeArea.x / Screen.width, Screen.safeArea.y / Screen.height);
            tUIParent.anchorMax = new Vector2((Screen.safeArea.x + Screen.safeArea.width) / Screen.width, (Screen.safeArea.y + Screen.safeArea.height) / Screen.height);
        }
#endif
    }

}
