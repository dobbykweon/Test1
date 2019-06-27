using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class MonsterGroupScript : MonoBehaviour {

	public GameObject[] Monsters;
	public Text Title;
	// List<RewardStickerVo> list;

	IEnumerator Start(){
		yield return ResourceLoadManager.Instance.Initialize();
	}

	public void SetMonsters (string monsterName, List<RewardStickerVo> list) {

        #region 스티커 오류로 인한 임시 리스트 정리 소스
        Dictionary<long, RewardStickerVo> dList = new Dictionary<long, RewardStickerVo>();

        PreStickerVo preStickerVo = ConfigurationData.Instance.GetValueFromJson<PreStickerVo>("PreSticker");

        for (int i = 0; i < list.Count; i++)
        {
            if (dList.ContainsKey(list[i].sticker_idx) == true)
            {
                dList[list[i].sticker_idx].brushing_count += list[i].brushing_count;
            }
            else
            {
                dList.Add(list[i].sticker_idx, list[i]);
            }
        }

        list = null;
        list = new List<RewardStickerVo>();

        foreach (KeyValuePair<long, RewardStickerVo> item in dList)
        {
            list.Add(item.Value);
        }
        #endregion


        for (int i = 0; i < 7; i++)
        {
            RewardStarController starController = Monsters[i].GetComponentInChildren<RewardStarController>();
            Image monster = Monsters[i].GetComponentInChildren<Image>();
            Button button = Monsters[i].GetComponentInChildren<Button>();
            Monsters[i].AddComponent<TargetObjectVo>();
            TargetObjectVo targetObject = Monsters[i].GetComponent<TargetObjectVo>();

            monster.enabled = false;
            ResourceLoadManager.Instance.CoInstantiateGameSpriteAsync("sticker_" + monsterName + "_gray"
            , sprite =>
            {
                monster.sprite = sprite;
                monster.enabled = true;
            });
        }

        long lastIdx = 0;

        foreach (KeyValuePair<long, RewardStickerVo> item in dList)
        {
            SkeletonGraphic star = Monsters[(item.Key - 1) % 7].GetComponentInChildren<SkeletonGraphic>();
            RewardStarController starController = Monsters[(item.Key - 1) % 7].GetComponentInChildren<RewardStarController>();
            Image monster = Monsters[(item.Key - 1) % 7].GetComponentInChildren<Image>();
            Button button = Monsters[(item.Key - 1) % 7].GetComponentInChildren<Button>();
            TargetObjectVo targetObject = Monsters[(item.Key - 1) % 7].GetComponent<TargetObjectVo>();

            LogManager.Log("idx??? : " + preStickerVo.idx + "/" + item.Key);

            monster.enabled = false;

            //ResourceLoadManager.Instance.CoInstantiateGameSpriteAsync(item.Value.sticker_name
            //, sprite =>
            //{
            //    monster.sprite = sprite;
            //    monster.enabled = true;
            //});

            if (lastIdx < item.Key)
            {
                lastIdx = item.Key;
            }

            //if (dList.Count == item.Key % 7)
            //{
            //    starController.StartRewardStar(item.Value.brushing_count);
            //}
            //else
            //{
            starController.StartRewardIdle(item.Value.brushing_count);
            //}

            button.interactable = true;
            targetObject.setObject(item.Value);
            button.onClick.AddListener(() =>
            {

                if (RewardManager.Instance.IsBrushEnd)
                    FirebaseManager.Instance.LogEvent("br_brushing_selfie");
                else
                    FirebaseManager.Instance.LogEvent("br_sticker_selfie");

                long idx = (item.Key - 1) / 7 / 3 * 7 + (item.Key - 1) % 7 + 1;


                BMUtil.Instance.SelectSelfyIdx = idx;
                BMUtil.Instance.SelectSelfyType = (CharacterType)(((item.Key - 1) / 7) % 3);

                TargetObjectVo target = button.GetComponentInParent<TargetObjectVo>();
                //LogManager.Log(target.getObject<RewardStickerVo>().accessory_name);
                SceneDTO.Instance.SetValue("Sticker", target.getObject<RewardStickerVo>().sticker_name + ":" + target.getObject<RewardStickerVo>().accessory_name + ":" + target.getObject<RewardStickerVo>().accessory_type);
                BrushMonSceneManager.Instance.LoadScene(SceneNames.Selfy.ToString());
            });
        }

        if (lastIdx > 0)
        {
            RewardStarController sController = Monsters[(lastIdx - 1) % 7].GetComponentInChildren<RewardStarController>();
            sController.StartRewardStar(dList[lastIdx].brushing_count);
        }

        //for (int i = 0; i < 7; i++)
        //{

        //    //SkeletonGraphic star = Monsters[i].GetComponentInChildren<SkeletonGraphic> ();
        //    RewardStarController starController = Monsters[i].GetComponentInChildren<RewardStarController>();
        //    Image monster = Monsters[i].GetComponentInChildren<Image>();
        //    Button button = Monsters[i].GetComponentInChildren<Button>();
        //    Monsters[i].AddComponent<TargetObjectVo>();
        //    TargetObjectVo targetObject = Monsters[i].GetComponent<TargetObjectVo>();

        //    //if (list.Count > 0 && i < list.Count)
        //    if(dList.ContainsKey(i+1) == true)
        //    {

        //        if (i == (list.Count - 1))
        //        {
        //            starController.StartRewardStar(dList[i+1].brushing_count);
        //        }
        //        else
        //        {
        //            starController.StartRewardIdle(dList[i+1].brushing_count);
        //        }

        //        monster.enabled = false;
        //        ResourceLoadManager.Instance.CoInstantiateGameSpriteAsync(dList[i+1].sticker_name
        //        , sprite => {
        //            monster.sprite = sprite;
        //            monster.enabled = true;
        //        });
        //        button.interactable = true;
        //        targetObject.setObject(dList[i+1]);
        //        button.onClick.AddListener(() => {

        //            if (RewardManager.Instance.IsBrushEnd)
        //                FirebaseManager.Instance.LogEvent("br_brushing_selfie");
        //            else
        //                FirebaseManager.Instance.LogEvent("br_sticker_selfie");

        //            TargetObjectVo target = button.GetComponentInParent<TargetObjectVo>();
        //            LogManager.Log(target.getObject<RewardStickerVo>().accessory_name);
        //            SceneDTO.Instance.SetValue("Sticker", target.getObject<RewardStickerVo>().sticker_name + ":" + target.getObject<RewardStickerVo>().accessory_name + ":" + target.getObject<RewardStickerVo>().accessory_type);
        //            BrushMonSceneManager.Instance.LoadScene(SceneNames.Selfy.ToString());
        //        });

        //    }
        //    else
        //    {
        //        // monster.sprite = Resources.Load<Sprite> ("Textures/sticker_" + monsterName + "_gray");
        //        monster.enabled = false;
        //        ResourceLoadManager.Instance.CoInstantiateGameSpriteAsync("sticker_" + monsterName + "_gray"
        //        , sprite => {
        //            monster.sprite = sprite;
        //            monster.enabled = true;
        //        });
        //    }
        //}
    }

	IEnumerator startAnimation(SkeletonGraphic star, int count ){
		yield return new WaitForSeconds(0.5f);
		star.AnimationState.SetAnimation (0, "Star_" + getValidtionData (count) + "_event", false);
	}

	string getValidtionData (int count) {
		if (count <= 3)
			return count.ToString ("D2");
		return "03";
	}

}