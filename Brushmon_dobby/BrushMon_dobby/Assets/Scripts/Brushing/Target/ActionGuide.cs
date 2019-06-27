using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class ActionGuide : MonoBehaviour {

	private UIController ActionGuideUIC;
	private SkeletonGraphic ActionGuideSpine;
	
	void Awake () {
		ActionGuideUIC = GetComponent<UIController>();
        ActionGuideSpine = GetComponent<SkeletonGraphic>();
	}

    //private BrushStatus brushStatus;

    public void shwoActionGuide(BrushStatus brushStatus, bool isHandLeft = false)
    {
        LogManager.Log("isHandLeft(" + brushStatus.ToString() + ") : " + isHandLeft);

        if (brushStatus.ToString().StartsWith("FINISH") && isHandLeft == false)
        {
            transform.localEulerAngles = new Vector3(0, 180f, 0);
        }
        else
        {
            transform.localEulerAngles = Vector3.zero;
        }

        string statusName = brushStatus.ToString().ToLower();
        PlayAnimation(statusName);
        AudioManager.Instance.playAudio(
            statusName,
            delegate
            {
                ActionGuideUIC.Hide();
                BrushFSM.Instance.NextAction();
            }, 0f, 1f
        );
        ActionGuideUIC.Show();
    }

	void PlayAnimation(string statusName, bool isHandLeft=false)
    {
        string animationName = statusName;
        if (animationName.IndexOf("prepare_paste") > -1){
            if(isHandLeft)
                animationName = animationName+"_left";

            StartCoroutine(PlayAnimation(animationName, 3f, false, 0.5f));
        }else{
            ActionGuideSpine.AnimationState.SetAnimation(0, String.Concat(animationName, "_idle"), false);
            ActionGuideSpine.AnimationState.SetAnimation(0, animationName, true);
        }
    }

    IEnumerator PlayAnimation(string animationName, float delay, bool isLoop = true, float speed = 1)
    {
        ActionGuideSpine.AnimationState.TimeScale = speed;
        ActionGuideSpine.AnimationState.SetAnimation(0, String.Concat(animationName, "_idle"), false);
        yield return new WaitForSeconds(delay);
        ActionGuideSpine.AnimationState.SetAnimation(0, animationName, isLoop);
    }

    public void InitAnimation(){
        ActionGuideSpine.AnimationState.SetEmptyAnimation(0, 0);
    }
    
    
}
