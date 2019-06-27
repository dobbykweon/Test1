using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardStarController : MonoBehaviour
{
    public List<Animation> animations;

    private void Awake()
    {

    }

    public void StartRewardStar(int cnt)
    {
        StartCoroutine(StartRewardStarAni(cnt));
    }

    IEnumerator StartRewardStarAni(int cnt)
    {
        yield return new WaitForSeconds(0.5f);

        switch (cnt)
        {
            case 0:
                break;
            case 1:
                animations[0].transform.localPosition = new Vector3(0, 0, 0);
                break;
            case 2:
                animations[0].transform.localPosition = new Vector3(-25, 0, 0);
                animations[1].transform.localPosition = new Vector3(25, 0, 0);
                break;
            case 3:
                animations[0].transform.localPosition = new Vector3(-50, 0, 0);
                animations[1].transform.localPosition = new Vector3(0, 0, 0);
                animations[2].transform.localPosition = new Vector3(50, 0, 0);
                break;
        }

        for (int i = 0; i < animations.Count; i++)
        {
            if (cnt > i)
            {
                animations[i].gameObject.SetActive(true);
                animations[i].Play("Reward-Star");
            }
            else
            {
                animations[i].gameObject.SetActive(false);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void StartRewardIdle(int cnt)
    {
        switch (cnt)
        {
            case 0:
                break;
            case 1:
                animations[0].transform.localPosition = new Vector3(0, 0, 0);
                break;
            case 2:
                animations[0].transform.localPosition = new Vector3(-25, 0, 0);
                animations[1].transform.localPosition = new Vector3(25, 0, 0);
                break;
            case 3:
                animations[0].transform.localPosition = new Vector3(-50, 0, 0);
                animations[1].transform.localPosition = new Vector3(0, 0, 0);
                animations[2].transform.localPosition = new Vector3(50, 0, 0);
                break;
        }

        for (int i = 0; i < animations.Count; i++)
        {
            if (cnt > i)
            {
                animations[i].gameObject.SetActive(true);
                animations[i].Play("Reward-Star-Idle");
            }
            else
            {
                animations[i].gameObject.SetActive(false);
            }
        }
    }
}
