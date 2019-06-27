using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
public class WelcomManager : MonoBehaviour
{

    public float LogoAnimationDelaySeconds = .8f;
    private SkeletonGraphic logo;
    
    void Awake()
    {
        
    }
    void Start()
    {
        logo = GameObject.Find("/UI/Spine-Logo").GetComponent<SkeletonGraphic>();
        StartCoroutine(StartLogoAnimation());
    }

    IEnumerator StartLogoAnimation(){
        yield return new WaitForSeconds(LogoAnimationDelaySeconds);
        logo.AnimationState.SetAnimation(0, "Event", false);
        logo.AnimationState.AddAnimation(0, "Idle", true, 0);
    }
}
