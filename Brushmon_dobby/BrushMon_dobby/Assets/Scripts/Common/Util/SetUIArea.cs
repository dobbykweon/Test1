using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUIArea : MonoBehaviour {

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
