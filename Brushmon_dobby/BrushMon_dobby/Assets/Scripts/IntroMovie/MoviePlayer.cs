using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Video;
using System;
using System.Collections.Generic;
using Spine.Unity;

public class MoviePlayer : MonoBehaviour
{
    private SkeletonGraphic movie;


    void Start()
    {
        Application.runInBackground = true;
        SkeletonGraphic movie = GetComponent<SkeletonGraphic>();
        AudioManager.Instance.playAudio("story_movie_audio", null);
        movie.AnimationState.Complete += delegate {
            TrackingManager.Instance.Tracking("video_complete", "Intro Movie Complete");
            NextScene();
        };
        TrackingManager.Instance.Tracking("play_video", "Intro Movie Play");
        FirebaseManager.Instance.LogEvent("br_screen_start");
    }

    public void Skip()
    {
        TrackingManager.Instance.Tracking("video_skip", "Intro Movie Skip");
        FirebaseManager.Instance.LogEvent("br_intro_skip");
        NextScene();
    }

    public void NextScene()
    {
        if (SceneDTO.Instance.GetValue("FromScene").Equals(SceneNames.Welcome.ToString()))
            BrushMonSceneManager.Instance.LoadScene("Welcome");
        else
            BrushMonSceneManager.Instance.LoadScene("Tutorial");
    }

    public void Update()
    {
        //if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Skip();
                return;
            }
        }
    }
}
