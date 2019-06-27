using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class ReportDialog : MonoBehaviour {

    public GameObject objDialogRoot;

	public Text Date;
	public Text Time;

	public Text ResultText;

	public SkeletonGraphic bagde;

	private Action target;
	private TokenVo tokenVo;
	private ProfileVo currentProfileVo;

	public GameObject ToothRoot;

	public GameObject Empty;

    public Image backImg;

    public GameObject objArrow;

    List<LatestRewardResultVo> listReportData = new List<LatestRewardResultVo>();

    int selectIdx = 0;
    int offset = 0;
    bool isMaxReport = false;

    Dictionary<int, GameObject> ojbListToothBad;

    void Awake ()
    {
        backImg = gameObject.GetComponent<Image>();
        backImg.color = new Color(0, 0, 0, 0);

        tokenVo = ConfigurationData.Instance.GetValueFromJson<TokenVo> ("Token");
		currentProfileVo = ConfigurationData.Instance.GetValueFromJson<ProfileVo> ("CurrentProfile");

        if (ojbListToothBad == null)
        {
            ojbListToothBad = new Dictionary<int, GameObject>();

            for (int i = 0; i < 16; i++)
            {
                GameObject ToothBad = Instantiate(Resources.Load("Prefabs/ToothBad")) as GameObject;
                ToothBad.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/tooth_" + (i + 1).ToString("D2"));
                ToothBad.transform.SetParent(ToothRoot.transform, false);
                ToothBad.SetActive(false);
                ojbListToothBad.Add(i, ToothBad);
            }
        }
    }

    public void Init(bool isNotUseMsgView = true)
    {
        if (isNotUseMsgView == false)
        {
            objArrow.SetActive(false);
            LatestBrushResult(isNotUseMsgView);
        }
        else
        {
            isMaxReport = false;
            objArrow.SetActive(true);
            AllBrushResult(0);
        }
    }

    void AllBrushResult(int offset)
    {
        Debug.Log("offset : " + offset);

        //RestService.Instance.GetBrushResult(tokenVo, currentProfileVo.profile_idx,
        RestService.Instance.GetBrushResult(tokenVo, currentProfileVo.profile_idx, 1, offset, true,
            result =>
            {
                LogManager.Log("result : "+ result);
                List<LatestRewardResultVo> _listReportData = JsonHelper.getJsonArray<LatestRewardResultVo>(result);

                if (_listReportData.Count == 0) isMaxReport = true;

#if OFFLINE
                //listReportData = _listReportData;
                listReportData = new List<LatestRewardResultVo>();

                for (int i = 0; i < _listReportData.Count; i++)
                    listReportData.Add(_listReportData[_listReportData.Count - 1 - i]);

                isMaxReport = true;
#else
                int cnt = 0;
                while (_listReportData.Count > cnt)
                {
                    listReportData.Add(_listReportData[cnt]);
                    cnt++;
                }
#endif

                if (listReportData.Count > 0 && _listReportData.Count > 0)
                {
                    selectIdx = offset;
                    backImg.color = new Color(0, 0, 0, 0.7f);
                    objDialogRoot.SetActive(true);
                    ViewResultData(listReportData[selectIdx]);
                }
                else if(listReportData.Count == 0 && _listReportData.Count == 0)
                {
                    LatestBrushResult(true);
                    //SetNotUsedMessage();
                    //Close();
                }
                else
                {
                    selectIdx--;
                }

                LogManager.Log("AllBrushResult Success!!!");

            }, exception =>
            {
                LogManager.Log("AllBrushResult : " + exception.GetBaseException());
                Close();
            }
        );
    }

    void LatestBrushResult(bool isNotUseMsgView)
    {
        RestService.Instance.GetLatestBrushResult(tokenVo, currentProfileVo.profile_idx,
            result => {
                LogManager.Log("LatestBrushResult : "+ result);
                LatestRewardResultVo latest = JsonUtility.FromJson<LatestRewardResultVo>(result);

                if (latest.use_brush == true)
                {
                    backImg.color = new Color(0, 0, 0, 0.7f);
                    objDialogRoot.SetActive(true);
                    ViewResultData(latest);
                }
                else
                {
                    if (isNotUseMsgView == true) SetNotUsedMessage();
                    Close();
                }

            }, exception => {
                LogManager.Log("LatestBrushResult : " + exception.GetBaseException());
            }
        );
    }

    void ViewResultData(LatestRewardResultVo latest)
    {
        if (latest.result_list.Count > 0)
        {
            Empty.SetActive(false);



            //if (NativePlugin.Instance.GetNativeCountryCode() == "US")
            //{
            //    Date.text = latest.local_create_date.ToString("MMMM dd");
            //    Time.text = latest.local_create_date.ToString("ddd H:mm tt");
            //}
            //else
            {
                Date.text = latest.local_create_date.ToString("yyyy-MM-dd");
                Time.text = latest.local_create_date.ToString("ddd H:mm");
            }

            int count = 0;

            //ToothRoot.transform.DetachChildren();

            for (int i = 0; i < latest.result_list.Count; i++)
            {
                string item = latest.result_list[i];
                if ("BAD".Equals(item))
                {
                    ojbListToothBad[i].SetActive(true);
                    count++;
                }
                else
                {
                    ojbListToothBad[i].SetActive(false);
                }
            }
            string resultText = "";
            if (count <= 4)
            {
                // resultText = "정말 잘했어요.";
                resultText = LocalizationManager.Instance.GetLocalizedValue("report_level_01");
                bagde.AnimationState.SetAnimation(0, "Event_1", false);
            }
            else if (count <= 8)
            {
                // resultText = "다음번엔 더 꼼꼼하게 양치해봐요.";
                resultText = LocalizationManager.Instance.GetLocalizedValue("report_level_02");
                bagde.AnimationState.SetAnimation(0, "Event_2", false);
            }
            else if (count <= 12)
            {
                // resultText = "입안이 충분히 깨끗해지지 않았어요.\n다음번엔 좀 더 집중해서 따라해봐요!";
                resultText = LocalizationManager.Instance.GetLocalizedValue("report_level_03");
                bagde.AnimationState.SetAnimation(0, "Event_3", false);
            }
            else if (count <= 16)
            {
                // resultText = "전혀 집중하지 않았군요!";
                resultText = LocalizationManager.Instance.GetLocalizedValue("report_level_04");
                bagde.AnimationState.SetAnimation(0, "Event_4", false);
            }
            ResultText.text = resultText;
        }
        else
        {
            Empty.SetActive(true);
        }
    }

	public void Close () {
		if(target !=null)
			target ();
		Destroy (gameObject);
	}

	public void targetClose (Action action) {
		target = action;
	}

    public void BtnNext()
    {
        if (selectIdx == 0) return;
        selectIdx--; 
        ViewResultData(listReportData[selectIdx]);

        FirebaseManager.Instance.LogEvent("br_report_next");
    }

    public void BtnPre()
    {
        selectIdx++;

        if (listReportData.Count > selectIdx)
        {
            ViewResultData(listReportData[selectIdx]);
            FirebaseManager.Instance.LogEvent("br_report_pre");
        }
        else
        {
            if (isMaxReport == false)
            {
                AllBrushResult(selectIdx);
                FirebaseManager.Instance.LogEvent("br_report_pre");
            }
            else
            {
                selectIdx--;
            }
        }
    }

    void SetNotUsedMessage()
    {
        //GameObject Dialog = Instantiate(Resources.Load("Prefabs/Panel-Dialog")) as GameObject;
        //Dialog.transform.SetParent(GameObject.Find("/UI").transform, false);
        //DialogController controller = Dialog.GetComponent<DialogController>();
        //controller.Title.text = LocalizationManager.Instance.GetLocalizedValue("report_dialog_not_used_title");
        //controller.Body.text = LocalizationManager.Instance.GetLocalizedValue("report_dialog_not_used_msg");
        //controller.setActions(null, null);
        //controller.SetButtonName(LocalizationManager.Instance.GetLocalizedValue("common_button_done"), null);
        //controller.setButtonVisible(true, false);

        //BMUtil.Instance.AddOpenDialogList(controller);

        UserVo userVo = ConfigurationData.Instance.GetValueFromJson<UserVo>("User");

#if !OFFLINE
        if (NativePlugin.Instance.GetNativeCountryCode() == "KR" && userVo.app_language_text == "KR")
        {
            BMUtil.Instance.OpenDialog(LocalizationManager.Instance.GetLocalizedValue("report_dialog_not_used_title"), LocalizationManager.Instance.GetLocalizedValue("report_dialog_not_used_msg"),
                LocalizationManager.Instance.GetLocalizedValue("common_button_done"), "칫솔 구매하기",
                true, true,
                null, delegate { Application.OpenURL("http://bit.ly/app_no_brush"); }
                );
        }
        else
        {
            BMUtil.Instance.OpenDialog(LocalizationManager.Instance.GetLocalizedValue("report_dialog_not_used_title"), LocalizationManager.Instance.GetLocalizedValue("report_dialog_not_used_msg"),
                LocalizationManager.Instance.GetLocalizedValue("common_button_done"), null,
                true, false,
                null, null
                );
        }
#else
        BMUtil.Instance.OpenDialog(LocalizationManager.Instance.GetLocalizedValue("report_dialog_not_used_title"), LocalizationManager.Instance.GetLocalizedValue("report_dialog_not_used_msg"),
                LocalizationManager.Instance.GetLocalizedValue("common_button_done"), null,
                true, false,
                null, null
                );
#endif
    }
}