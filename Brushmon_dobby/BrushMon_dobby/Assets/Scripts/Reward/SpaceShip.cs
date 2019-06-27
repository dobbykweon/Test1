using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class SpaceShip : MonoBehaviour {

	private UIController SpaceShipUIC;
    private SkeletonGraphic SpaceShipSpine;
	private GameObject MonsterObject;
	// private Image MonsterImage;
    private Animator MonsterAnimator;
    void Awake()
    {
        SpaceShipUIC = GetComponent<UIController>();
		SpaceShipSpine = GetComponent<SkeletonGraphic>();

		MonsterObject = GameObject.Find("/UI/Image-Monster");
        // MonsterImage = MonsterObject.GetComponent<Image>();
        MonsterAnimator = MonsterObject.GetComponent<Animator>();
    }

    public void shwoSpaceShip()
    {
		SpaceShipSpine.enabled = true;
		SpaceShipSpine.AnimationState.SetAnimation(0, "Idle", false);
        AudioManager.Instance.playAudio("reward", null);
        SpaceShipUIC.Show();
        MonsterAnimator.SetTrigger("Reward-Monster-Up");
        //StartCoroutine(animateMonsterUp());
    }

    internal float getCurrentAnimationDuration(){
        float time = 0;
        if(SpaceShipSpine.AnimationState.GetCurrent(0) != null)
            time = SpaceShipSpine.AnimationState.GetCurrent(0).AnimationTime;
        return time;
    }

	IEnumerator animateMonsterUp(){
		yield return new WaitForSeconds(1f);
		MonsterAnimator.SetTrigger("Reward-Monster-Up");
	}

	public void InitAnimation(){
        SpaceShipSpine.AnimationState.SetEmptyAnimation(0, 0);
    }
}
