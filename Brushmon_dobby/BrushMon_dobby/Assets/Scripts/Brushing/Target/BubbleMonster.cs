using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class BubbleMonster : MonoBehaviour {

    [SerializeField] private GameObject objPauseButton;

    [SerializeField] private UIController BubbleMonsterUIC;
    [SerializeField] private SkeletonGraphic BubbleSpine;
    [SerializeField] private GameObject BubbleMonsterObject;
    [SerializeField] private Image BubbleMonsterImage;
    [SerializeField] private Animator BubbleMonsterAnimator;
    void Awake () {
        //BubbleMonsterUIC = GetComponent<UIController> ();
        //BubbleSpine = GetComponent<SkeletonGraphic> ();
        //BubbleMonsterObject = GameObject.Find ("/UI/BubbleMonsterCenter/Image-BubbleMonster");
        //BubbleMonsterImage = BubbleMonsterObject.GetComponent<Image> ();
        //BubbleMonsterAnimator = BubbleMonsterObject.GetComponent<Animator> ();
    }

    //private BrushStatus brushStatus;

    public void shwoStandBy (BrushStatus brushStatus, string BubbleMonsterName = "sticker_cheese_01") {
        AudioManager.Instance.playAudio ("standby", null, 0f, 0f);
        // BubbleMonsterImage.sprite = (string.Concat("Textures/", BubbleMonsterName));

        BubbleMonsterImage.enabled = false;

        ResourceLoadManager.Instance.CoInstantiateGameSpriteAsync(BubbleMonsterName, sprite =>
        {
            BubbleMonsterImage.sprite = sprite;
            BubbleMonsterImage.enabled = true;
        });

        BubbleMonsterImage.SetNativeSize ();
        BubbleSpine.AnimationState.SetAnimation (0, "Idle", true);
        BubbleMonsterAnimator.SetTrigger ("Idle");
        BubbleMonsterUIC.Show ();
    }

    public void BuubleClick () {
        if (BrushFSM.Instance.currentBrushStatus.Equals (BrushStatus.BRUSH_STANDBY))
        {
            BLEManager.Instance.StopScan();
#if UNITY_EDITOR
            StartCoroutine(BrushFSM.Instance.StartBrushTimeCheck());
#endif
            BrushFSM.Instance.NextAction ();
        }
    }

    public void EarthBubbleMonster () {
        BubbleMonsterImage.SetNativeSize ();
        BubbleMonsterAnimator.SetTrigger ("Init-Earth-Idle");

    }

    public void BubblExplosion () {
        if (!BubbleMonsterUIC.isShow) {
            BrushFSM.Instance.PlayAction (BrushStatus.BRUSH_STANDBY);
        } else {
            showBubbleExplosion ();
        }
    }

    public bool isBubblExplosion () {
        return BubbleMonsterUIC.isShow;
    }

    private void showBubbleExplosion () {
        AudioManager.Instance.playEffect ("connected");
        BubbleSpine.AnimationState.SetAnimation (0, "Event_01", false);
        BubbleMonsterAnimator.SetTrigger ("Down");
        BubbleMonsterAnimator.SetTrigger ("Earth-Idle");
        BrushFSM.Instance.NextAction (2f);
    }

    public void InitAnimation () {

    }
}