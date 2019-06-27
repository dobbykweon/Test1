using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CharacterType
{
    cheese,
    cherry,
    soda,
    none,
}

public class RewardPackController : MonoBehaviour
{
    [SerializeField] Text txtPackageName;

    [SerializeField] List<Image> charImg;

    CharacterType type = CharacterType.none;

    public void Init(RewardPackVo rewardPack, int count, bool isLast = false)
    {
        gameObject.name = count.ToString();

        type = (CharacterType)(count % 3);
        txtPackageName.text = "<color=#" + GetPackageNameColor() + ">" + (type.ToString() + " FAMILY").ToUpper() + "</color>";

        if (rewardPack == null) LogManager.Log("rewardPack == null");
        if (rewardPack.reward_list == null) LogManager.Log("rewardPack.reward_list");

        if (rewardPack.reward_list != null)
        {
            for (int i = 0; i < rewardPack.reward_list.Count; i++)
            {
                if (i < 7)
                    ResourceLoad(i, rewardPack.reward_list[i], i == rewardPack.reward_list.Count - 1 && isLast);
            }

            if (rewardPack.reward_list.Count < 7)
            {
                for (int i = rewardPack.reward_list.Count; i < 7; i++)
                {
                    EmptyResourceLoad(i, i == 6);
                }
            }
        }
        else
        {
            for (int i = 0; i < 7; i++)
            {
                EmptyResourceLoad(i, i == 6);
            }
        }
    }

    void ResourceLoad(int imgIdx, RewardStickerVo stickerVo, bool isLast)
    {
        long idx = (stickerVo.sticker_idx - 1) / 7 / 3 * 7 + (stickerVo.sticker_idx - 1) % 7 + 1;

        LogManager.Log("Res : " + ("sticker_" + type.ToString() + "_" + idx.ToString("00")));

        ResourceLoadManager.Instance.CoInstantiateGameSpriteAsync("sticker_" + type.ToString() + "_" + idx.ToString("00"),
            sprite =>
            {
                charImg[imgIdx].sprite = sprite;
                charImg[imgIdx].transform.GetComponent<Button>().interactable = true;

                if (isLast)
                    charImg[imgIdx].transform.parent.GetComponentInChildren<RewardStarController>().StartRewardStar(stickerVo.brushing_count > 3 ? 3 : stickerVo.brushing_count);
                else
                    charImg[imgIdx].transform.parent.GetComponentInChildren<RewardStarController>().StartRewardIdle(stickerVo.brushing_count > 3 ? 3 : stickerVo.brushing_count);

                charImg[imgIdx].transform.GetComponent<Button>().onClick.AddListener(() =>
                {
                    if (RewardManager.Instance.IsBrushEnd)
                        FirebaseManager.Instance.LogEvent("br_brushing_selfie");
                    else
                        FirebaseManager.Instance.LogEvent("br_sticker_selfie");

                    BMUtil.Instance.SelectSelfyIdx = idx;
                    BMUtil.Instance.SelectSelfyType = type;
                    BMUtil.Instance.SelectStickerIndex = stickerVo.sticker_idx;

                    SceneDTO.Instance.SetValue("Sticker", stickerVo.sticker_name + ":" + stickerVo.accessory_name + ":" + stickerVo.accessory_type);

                    BrushMonSceneManager.Instance.LoadScene(SceneNames.Selfy.ToString());
                });

                if (isLast == true && imgIdx == 6) BMUtil.Instance.CloseLoading();
            });
    }

    void EmptyResourceLoad(int imgIdx, bool isLast)
    {
        ResourceLoadManager.Instance.CoInstantiateGameSpriteAsync("sticker_" + type.ToString() + "_gray",
            sprite =>
            {
                charImg[imgIdx].sprite = sprite;
                if (isLast) BMUtil.Instance.CloseLoading();
            });
    }

    string GetPackageNameColor()
    {
        switch (type)
        {
            case CharacterType.cheese: return "F3B616";
            case CharacterType.cherry: return "E61F74";
            case CharacterType.soda: return "48C0EA";
            default: return "F3B616";
        }
    }

}
