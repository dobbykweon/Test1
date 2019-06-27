using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class RewardGreenMold : MonoBehaviour {

	// Use this for initialization
	private UIController GreenMoldUIC;
    private SkeletonGraphic GreenMoldSpine;

    void Awake()
    {
        GreenMoldUIC = GetComponent<UIController>();
		GreenMoldSpine = GetComponent<SkeletonGraphic>();
    }

    public void shwoGreenMold()
    {
		GreenMoldSpine.AnimationState.SetAnimation(0, "Step_04", true);
        AudioManager.Instance.playAudio("reward_greenmold",null);
        StartCoroutine(showSpaceShip());
        GreenMoldUIC.Show();
    }

    IEnumerator showSpaceShip(){
        yield return new WaitForSeconds(3.395f);
        GreenMoldUIC.HideAsync(); 
        RewardManager.Instance.showSpaceShip();
    }

	public void InitAnimation(){
        GreenMoldSpine.AnimationState.SetEmptyAnimation(0, 0);
    }
}
