using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDialog : MonoBehaviour
{
    [SerializeField] Text txtTitle;
    [SerializeField] Text txtMessage;

    [SerializeField] LayoutElement titleLayout;
    [SerializeField] LayoutElement msgLayout;

    [SerializeField] GameObject objBtnDone;
    [SerializeField] GameObject objBtnCancel;
    [SerializeField] GameObject objBtnDetail;

    [SerializeField] Text txtDone;
    [SerializeField] Text txtCancel;
    [SerializeField] Text txtDetail;

    private Action done;
    private Action cancel;
    private Action detail;

    public Text Title { get { return txtTitle; } }
    public Text Body { get { return txtMessage; } }

    private void Update()
    {
        if (txtTitle.rectTransform.sizeDelta.x > 720)
        {
            titleLayout.preferredWidth = 720;
        }

        if (txtMessage.rectTransform.sizeDelta.x > 720)
        {
            msgLayout.preferredWidth = 720;
        }
    }

    public void SetButtonName(string done = "", string cancel = "", string detail = "")
    {
        if (!string.IsNullOrEmpty(done))
        {
            txtDone.text = done;
        }
        else
        {
            txtDone.text = LocalizationManager.Instance.GetLocalizedValue("common_button_done");
        }

        if (!string.IsNullOrEmpty(cancel))
        {
            txtCancel.text = cancel;
        }
        else
        {
            txtCancel.text = LocalizationManager.Instance.GetLocalizedValue("common_button_cancel");
        }

        txtDetail.text = detail;
    }

    public void setButtonVisible(bool isDone = true, bool isCancel = false, bool isDetail = false)
    {
        objBtnDone.SetActive(isDone);
        objBtnCancel.SetActive(isCancel);
        objBtnDetail.SetActive(isDetail);
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
