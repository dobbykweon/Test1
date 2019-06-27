using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipWindow : MonoBehaviour {

    public string testMsg = "";

    public RectTransform imgTxtBG;
    public Text txtTip;

    Animation ani;

    private void Awake()
    {
        StartCoroutine(TestStart());
    }

    IEnumerator TestStart()
    {
        yield return new WaitForSeconds(2.0f);

        string msg = "가나다라마바사\n가나다라마바사\n123456789123456789\n가나다라마바사\n가나다라마바사\n123456789123456789";
        SetMsg(msg);
    }

    public  void SetMsg(string msg)
    {
        txtTip.text = msg;
        StartCoroutine(Set());
    }


    IEnumerator Set()
    {
        RectTransform m_RectTransform = imgTxtBG.GetComponent<RectTransform>();
        RectTransform t_RectTransform = txtTip.gameObject.GetComponent<RectTransform>();

        LogManager.Log("width : " + m_RectTransform.rect.width);

        while (t_RectTransform.rect.width <= 0)
        {
            yield return null;
        }

        m_RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, t_RectTransform.rect.width + 50);
        m_RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, t_RectTransform.rect.height + 50);
        m_RectTransform.localPosition = new Vector3(0, m_RectTransform.rect.height / 2, 0);

        LogManager.Log("width : " + m_RectTransform.rect.width);
    }

}
