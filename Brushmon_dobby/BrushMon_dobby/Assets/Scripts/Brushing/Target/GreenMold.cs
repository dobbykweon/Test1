using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class GreenMold : MonoBehaviour
{

    [SerializeField] private UIController GreenMoldUIC;
    [SerializeField] private SkeletonGraphic GreenMoldSpine;

    void Awake()
    {
  //      GreenMoldUIC = GetComponent<UIController>();
		//GreenMoldSpine = GetComponent<SkeletonGraphic>();
    }

    //private BrushStatus brushStatus;

    public void shwoGreenMold(BrushStatus brushStatus)
    {
		string step = brushStatus.ToString().Substring(brushStatus.ToString().LastIndexOf("_")+1);
        GreenMoldSpine.AnimationState.SetAnimation(0, String.Concat("Step_", step), true);
        AudioManager.Instance.playAudio("greenmold" + (step == "01" ? "_laugh" : (Int32.Parse(step) - 1).ToString()), delegate { GreenMoldUIC.HideAsync(); BrushFSM.Instance.NextAction(); }, 1f, 1f);
        GreenMoldUIC.Show();
    }

	public void InitAnimation(){
        GreenMoldSpine.AnimationState.SetEmptyAnimation(0, 0);
    }

}
