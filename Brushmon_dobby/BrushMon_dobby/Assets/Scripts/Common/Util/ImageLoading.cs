using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageLoading : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "IntroBranch");
    }
}
