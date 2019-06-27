using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogController : MonoBehaviour {

    [SerializeField]
	private Text title;
    public Text Title { get { return title; } }

    [SerializeField]
    private Text body;
    public Text Body { get { return body; } }

	public GameObject Done;
	public GameObject Cancel;
    public GameObject Detail;

    private Action done;
	private Action cancel;
    private Action detail;

    int minWidth = 700;
    int maxWidth = 1000;

    int minHeight = 300;
    int maxHeight = 1000;

	void Start()
	{
		// setButtonVisible();
	}

    public void SetButtonName(string done = "", string cancel = "", string detail = "")
    {
        if (!string.IsNullOrEmpty(done))
        {
            Done.GetComponent<Text>().text = done;
        }
        else
        {
            Done.GetComponent<Text>().text = LocalizationManager.Instance.GetLocalizedValue("common_button_done");
        }

        if (!string.IsNullOrEmpty(cancel))
        {
            Cancel.GetComponent<Text>().text = cancel;
        }
        else
        {
            Cancel.GetComponent<Text>().text = LocalizationManager.Instance.GetLocalizedValue("common_button_cancel");
        }

        Detail.GetComponent<Text>().text = detail;
    }

    public void setButtonVisible(bool isDone = true, bool isCancel = false, bool isDetail = false)
    {
        Done.SetActive(isDone);
        Cancel.SetActive(isCancel);
        Detail.SetActive(isDetail);
    }

    public void setActions(Action done, Action cancel, Action detail = null)
    {
        this.done = done;
        this.cancel = cancel;
        this.detail = detail;
    }
    public void onCancelClick()
    {
        if (cancel != null)
            cancel();

        BMUtil.Instance.CloseDialog();
        Destroy(gameObject);
    }

    public void onDoneClick()
    {
        if (done != null)
            done();

        BMUtil.Instance.CloseDialog();
        Destroy(gameObject);
    }

    public void OnClickDetail()
    {
        if (detail != null)
            detail();
    }
}