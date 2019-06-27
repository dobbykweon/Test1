using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardViewPager : MonoBehaviour {

    public RectTransform panelRectTransform;
    public GameObject panel;
    public GameObject[] packages;
    public RectTransform center;

    public float[] distance;
    public bool dragging = false;
    public int bttnDistance;
    public int minButtonNum;

    public float value;
    private TokenVo tokenVo;
    private ProfileVo currentProfileVo;

    int selectIdx = 0;

    uTools.TweenPosition tweenScroll;

    int maxPackCnt = 11;

    void Start()
    {
        //BMUtil.Instance.LoadTestStickData(SetData);
    }

    //void SetData(string result)
    //{
    //    List<RewardPackVo> rewardList = JsonHelper.getJsonArray<RewardPackVo>(result);

    //    if (rewardList.Count > 0)
    //    {
    //        for (int i = 0; i < rewardList.Count; i++)
    //        {
    //            GameObject package = Instantiate(Resources.Load("Prefabs/Reward/Package")) as GameObject;
    //            package.transform.SetParent(panel.transform, false);
    //            package.GetComponent<RewardPackController>().Init(rewardList[i], i, i == (rewardList.Count - 1));
    //            //selectIdx = i;
    //        }
    //    }
    //    else
    //    {
    //        // 획득한 스티커가 하나도 없을때 처리
    //        GameObject package = Instantiate(Resources.Load("Prefabs/Reward/Package")) as GameObject;
    //        package.transform.SetParent(panel.transform, false);
    //        package.GetComponent<RewardPackController>().Init(new RewardPackVo(), 0, true);
    //        //selectIdx = 0;
    //    }
    //    LogManager.Log("rewardList.Count  : " + rewardList.Count);

    //    if (rewardList.Count < maxPackCnt * 3)
    //    {
    //        GameObject locked = Instantiate(Resources.Load("Prefabs/Reward/Locked")) as GameObject;
    //        locked.transform.SetParent(panel.transform, false);
    //        panelRectTransform.sizeDelta = new Vector2(960 * panelRectTransform.childCount, panelRectTransform.sizeDelta.y);
    //        panelRectTransform.anchoredPosition = new Vector2((-960 * (panelRectTransform.childCount - 1) / 2) + 960, 0);
    //    }
    //    else
    //    {
    //        panelRectTransform.sizeDelta = new Vector2(960 * panelRectTransform.childCount, panelRectTransform.sizeDelta.y);
    //        panelRectTransform.anchoredPosition = new Vector2((-960 * (panelRectTransform.childCount - 1) / 2), 0);
    //    }

    //}

    internal void initailize () {
        tokenVo = ConfigurationData.Instance.GetValueFromJson<TokenVo> ("Token");
        currentProfileVo = ConfigurationData.Instance.GetValueFromJson<ProfileVo> ("CurrentProfile");

        tweenScroll = panel.GetComponent<uTools.TweenPosition>();

        RestService.Instance.GetRewardList(tokenVo, currentProfileVo.profile_idx,
            result =>
            {

                //List<RewardPackVo> rewardList = JsonHelper.getJsonArray<RewardPackVo> (result);

                List<RewardPackVo> rewardList = JsonHelper.getJsonArray<RewardPackVo>(result);

                /*
                int rewardListCount = rewardList.Count;
                int pageCount = (rewardListCount==0?1:rewardListCount) +1;

                packages = new GameObject[pageCount];

                LogManager.Log("packages : "+packages.Length);
                GameObject package;
                float width = 940f + 20f;
                if (rewardListCount == 0) {
                    package = Instantiate (Resources.Load ("Prefabs/Reward/Package")) as GameObject;
                    package.transform.SetParent (panel.transform, false);
                    package.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (width * 0f, 0f);
                    packages[0] = package;
                    
                } else {
                    for (int i = 0; i < rewardListCount; i++) {
                         
                        package = Instantiate (Resources.Load ("Prefabs/Reward/Package")) as GameObject;
                        package.transform.SetParent (panel.transform, false);
                        package.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (width * i , 0f);
                        package.GetComponentInChildren<Text>().text = rewardList[i].package_name.ToUpper();
                        string colorString = rewardList[i].package_color;
                        if(string.IsNullOrEmpty(colorString)){
                            colorString = "#F3B616FF";
                        }
                        Color titleColor = new Color();
                        ColorUtility.TryParseHtmlString(colorString, out titleColor);
                        package.GetComponentInChildren<Text>().color = titleColor;
                        

                        package.GetComponentInChildren<MonsterGroupScript>().SetMonsters(rewardList[i].package_name.Split(' ')[0], rewardList[i].reward_list);
                        packages[i] = package;

                    }
                }
                LogManager.Log("pageCount : "+pageCount);
                GameObject locked = Instantiate (Resources.Load ("Prefabs/Reward/Locked")) as GameObject;
                locked.transform.SetParent (panel.transform, false);
                locked.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (width * (pageCount-1), 0f);
                
                packages[pageCount-1] = locked;
                distance = new float[packages.Length];
                bttnDistance = (int)Mathf.Abs(packages[1].GetComponent<RectTransform>().anchoredPosition.x - packages[0].GetComponent<RectTransform>().anchoredPosition.x);

                panelRectTransform.sizeDelta = new Vector2(960 * panelRectTransform.childCount, panelRectTransform.sizeDelta.y);

                LogManager.Log("??? : " + (-960 * (panelRectTransform.childCount - 1) / 2) + 960);

                panelRectTransform.anchoredPosition = new Vector2((-960 * (panelRectTransform.childCount - 1) / 2) + 960, 0);

                */


                if (rewardList.Count > 0)
                {
                    for (int i = 0; i < rewardList.Count; i++)
                    {
                        GameObject package = Instantiate(Resources.Load("Prefabs/Reward/Package")) as GameObject;
                        package.transform.SetParent(panel.transform, false);
                        package.GetComponent<RewardPackController>().Init(rewardList[i], i, i == (rewardList.Count - 1));
                        selectIdx = i;
                    }
                }
                else
                {
                    // 획득한 스티커가 하나도 없을때 처리
                    GameObject package = Instantiate(Resources.Load("Prefabs/Reward/Package")) as GameObject;
                    package.transform.SetParent(panel.transform, false);
                    package.GetComponent<RewardPackController>().Init(new RewardPackVo(), 0, true);
                    selectIdx = 0;
                }

#if OFFLINE
                panelRectTransform.sizeDelta = new Vector2(960 * panelRectTransform.childCount, panelRectTransform.sizeDelta.y);
                panelRectTransform.anchoredPosition = new Vector2((-960 * (panelRectTransform.childCount - 1) / 2), 0);
#else

                if (rewardList.Count < maxPackCnt * 3)
                {
                    GameObject locked = Instantiate(Resources.Load("Prefabs/Reward/Locked")) as GameObject;
                    locked.transform.SetParent(panel.transform, false);
                    panelRectTransform.sizeDelta = new Vector2(960 * panelRectTransform.childCount, panelRectTransform.sizeDelta.y);
                    panelRectTransform.anchoredPosition = new Vector2((-960 * (panelRectTransform.childCount - 1) / 2) + 960, 0);
                }
                else
                {
                    panelRectTransform.sizeDelta = new Vector2(960 * panelRectTransform.childCount, panelRectTransform.sizeDelta.y);
                    panelRectTransform.anchoredPosition = new Vector2((-960 * (panelRectTransform.childCount - 1) / 2), 0);
                }
#endif

            }, exception =>
            {
                BrushMonSceneManager.Instance.LoadScene(SceneNames.Welcome.ToString());
            });

        AudioManager.Instance.playEffect ("reward_collection_pop");

        // RewardTemp Star = GameObject.Find ("/UI/RewardPanel").GetComponent<RewardTemp> ();
        // Star.temp ();

    }

    void Update () {
        if (packages.Length > 0) {
            for (int i = 0; i < packages.Length; i++) {

                distance[i] = Mathf.Abs (center.transform.position.x - packages[i].transform.position.x);

            }

            float minDistance = Mathf.Min (distance);

            for (int a = 0; a < packages.Length; a++) {

                if (minDistance == distance[a]) {

                    minButtonNum = a;

                }

            }

            //if (!dragging)
            //{
            //    LerpToBttn(minButtonNum * -bttnDistance);
            //}
        }
    }

    void setIndexPosition(int index)
    {
        distance[index] = Mathf.Abs (center.transform.position.x - packages[index].transform.position.x);
        minButtonNum = index;
        LerpToBttn(minButtonNum * -bttnDistance);
    }

    void LerpToBttn (int position) {
        float newX = Mathf.Lerp (panelRectTransform.anchoredPosition.x, position, Time.deltaTime * 20f);
        Vector2 newPosition = new Vector2 (newX, panelRectTransform.anchoredPosition.y);
        panelRectTransform.anchoredPosition = newPosition;

    }

    Vector3 startPos, endPos;

    public void StartDrag()
    {
        LogManager.Log("StartDrag : "+ Input.mousePosition);

        startPos = Input.mousePosition;

        dragging = true;
    }

    public void EndDrag()
    {
        LogManager.Log("EndDrag : " + Input.mousePosition);
        //dragging = false;

        endPos = Input.mousePosition;

        Check();

        //StartCoroutine(StopDragging());
    }

    void Check()
    {
        if (Vector2.Distance(startPos, endPos) < 50) return;

        float xPos = Mathf.Abs(startPos.x - endPos.x);
        float yPos = Mathf.Abs(startPos.y - endPos.y);

        if (xPos > yPos)
        {
            //좌우
            if (startPos.x > endPos.x)
            {
                MoveRight();
            }
            else if (startPos.x < endPos.x)
            {
                MoveLeft(); 
            }

        }
        else if (xPos < yPos)
        {
            //상하
            if (startPos.y > endPos.y)
            {
                LogManager.Log("위로이동");
            }
            else if (startPos.y < endPos.y)
            {
                LogManager.Log("아래로이동");
            }
        }
    }

    IEnumerator StopDragging()
    {
        yield return new WaitForSeconds(0.1f);
        dragging = false;
    }

    void MoveLeft()
    {
        selectIdx--;
        if (selectIdx < 0)
        {
            selectIdx = 0;
            return;
        }

        Move();
    }

    void MoveRight()
    {
        selectIdx++;
        if (selectIdx > panelRectTransform.childCount - 1)
        {
            selectIdx = panelRectTransform.childCount - 1;
            return;
        }

        Move();
    }

    private void Move()
    {
        if (panelRectTransform.childCount % 2 > 0)
        {
            tweenScroll.from = panelRectTransform.anchoredPosition;
            tweenScroll.to = new Vector2((selectIdx - panelRectTransform.childCount / 2) * -960, 0);
            tweenScroll.ResetToBeginning();
            tweenScroll.PlayForward();
        }
        else
        {
            tweenScroll.from = panelRectTransform.anchoredPosition;
            tweenScroll.to = new Vector2((selectIdx - panelRectTransform.childCount / 2) * -960 - 480, 0);
            tweenScroll.ResetToBeginning();
            tweenScroll.PlayForward();
        }
    }
}