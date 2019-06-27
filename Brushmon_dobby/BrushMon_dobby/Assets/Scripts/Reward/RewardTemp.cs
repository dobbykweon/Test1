using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using UnityEngine.UI;

public class RewardTemp : MonoBehaviour
{

    public SkeletonGraphic Star;
    public void temp()
    {
        Star = GameObject.Find("/UI/RewardPanel/ScrollPanel/Package(Clone)/Monster01/Star").GetComponent<SkeletonGraphic>();
        Button Character = GameObject.Find("/UI/RewardPanel/ScrollPanel/Package(Clone)/Monster01/Character").GetComponent<Button>();
        // Star = GameObject.Find("/UI/RewardPanel/ScrollPanel/Package/Monster01/Star").GetComponent<SkeletonGraphic>();
        // Button Character = GameObject.Find("/UI/RewardPanel/ScrollPanel/Package/Monster01/Character").GetComponent<Button>();
        Character.interactable = true;
        Character.onClick.AddListener(delegate
        {
            LogManager.Log("Selfy");
            BrushMonSceneManager.Instance.LoadScene("Selfy");
        });
        Star.AnimationState.SetAnimation(0, "Star_03_event", false);
    }

    void Selfy()
    {
        LogManager.Log("Selfy");
        BrushMonSceneManager.Instance.LoadScene("Selfy");
    }

}
