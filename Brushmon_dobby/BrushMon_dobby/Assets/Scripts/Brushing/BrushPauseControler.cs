using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrushPauseControler : MonoBehaviour
{

    public Button PlayButton;
    public Text PlayButtonText;

    public bool isGameOutPause = false;

    public void setPlayButton(bool isEnable, string text)
    {
        PlayButton.interactable = isEnable;
        PlayButtonText.text = text;
    }
    public void Stop()
    {
        // if (!isGameOutPause)
        // {
        //     Destroy(gameObject);
        //     BrushFSM.Instance.onBackPress();
        // }
        // else
        // {
        //     BLEManager.Instance.sendData("q");
        // 	BLEManager.Instance.disconnect();
        // 	SceneDTO.Instance.Clear();
        // 	BrushMonSceneManager.Instance.LoadScene("Welcome");
        // }

        //Destroy(gameObject);
        BrushFSM.Instance.onBackPress();
    }

    public void Play()
    {
        // if (!isGameOutPause)
        // {
        //     Destroy(gameObject);
        // 	BrushFSM.Instance.GamePlay();
        // }
        // else
        // {
        //     BrushMonSceneManager.Instance.LoadScene("Brushing");
        // }
        BrushFSM.Instance.GamePlay();
        Destroy(gameObject);
    }
}
