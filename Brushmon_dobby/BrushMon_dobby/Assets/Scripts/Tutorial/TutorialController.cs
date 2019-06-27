using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Events;

public class TutorialController : MonoBehaviour
{
    public ScrollSnap_ snap;
    public GameObject startButton;


    void Start()
    {
        snap.onLerpCompleteCustom = onChanged;
        snap.OnDraging = OnDraging;

        FirebaseManager.Instance.LogEvent("br_screen_tutorial01");
    }

    public void onStartClick()
    {
        ConfigurationData.Instance.SetValue<bool>("isFirstEnter", false);

#if OFFLINE
        ConfigurationData.Instance.SetValue<bool>("isSkip", true);
        BrushMonSceneManager.Instance.LoadScene(SceneNames.Welcome.ToString());
#else
        BrushMonSceneManager.Instance.LoadScene(SceneNames.SignIn.ToString());
#endif
        FirebaseManager.Instance.LogEvent("br_tutorial_end");
    }

    void onChanged(int index){

        FirebaseManager.Instance.LogEvent("br_screen_tutorial0" + (index + 1));
        startButton.SetActive(index == 4);
    }

    void OnDraging(){
        startButton.SetActive(false);
    }

    
}