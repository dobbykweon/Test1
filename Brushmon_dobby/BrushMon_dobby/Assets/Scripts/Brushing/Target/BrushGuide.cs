using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class BrushGuide : MonoBehaviour
{

    private UIController BrushGuideUIC;
    private SkeletonGraphic BrushGuideSpine;

    void Awake()
    {
        BrushGuideUIC = GetComponent<UIController>();
        BrushGuideSpine = GetComponent<SkeletonGraphic>();
    }

    //private BrushStatus brushStatus;
    public void shwoBrushGuide(BrushStatus brushStatus, bool isLeft = false)
    {
        string animationName = brushStatus.ToString().ToLower();
        string step = brushStatus.ToString().Substring(brushStatus.ToString().LastIndexOf("_") + 1);

        AudioManager.Instance.playEffect("phase_change");
        AudioManager.Instance.playAudio(String.Concat("guide", step), 
            delegate
            {
                //LogManager.LogError("????????????????????");
                //AudioManager.Instance.playEffect("phase_change");
                //BrushFSM.Instance.NextAction(0.5f);
                //BrushGuideUIC.Hide();
                //BrushFSM.Instance.hideGradation();
            }, 0.5f, 0.5f);

        EnableBrushGuideUI(true);

        if (isLeft && ("06".Equals(step) || "09".Equals(step) || "12".Equals(step) || "15".Equals(step))){
            animationName = animationName+"_left";
        }

        StartCoroutine(PlayAnimation(animationName, 0));
    }

    public void EnableBrushGuideUI(bool isEnable)
    {
        if (isEnable)
        {
            BrushGuideUIC.Show();
        }
        else
        {
            BrushGuideUIC.Hide();
        }
    }

    IEnumerator PlayAnimation(string animationName, float delay, bool isLeft = false)
    {
        yield return new WaitForSeconds(delay);
        BrushGuideSpine.AnimationState.SetAnimation(0, animationName, true);
    }

    public void InitAnimation()
    {
        BrushGuideSpine.AnimationState.SetEmptyAnimation(0, 0);
    }

}
