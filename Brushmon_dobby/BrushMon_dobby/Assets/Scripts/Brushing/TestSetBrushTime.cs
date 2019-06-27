using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSetBrushTime : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private float timeGuide = 5f;
    [SerializeField] private float timeAR = 6f;
#endif

    private void Start()
    {
#if UNITY_EDITOR
        PlayerPrefs.SetFloat("timeGuide", timeGuide);
        PlayerPrefs.SetFloat("timeAR", timeAR);
#endif
    }

}
