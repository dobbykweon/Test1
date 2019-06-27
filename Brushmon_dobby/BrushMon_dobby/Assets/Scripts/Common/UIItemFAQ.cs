using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uTools;

public class UIItemFAQ : MonoBehaviour {

    [SerializeField] Text txtTitle;
    [SerializeField] Text txtMsg;
    [SerializeField] Text txtLinkTitle;

    [SerializeField] TweenLayoutElement tween;
    [SerializeField] LayoutElement layoutElement;
    [SerializeField] RectTransform tItemCell;

    [SerializeField] RectTransform rectBG;
    [SerializeField] RectTransform rectBtnTween;

    [SerializeField] Image imgOpen;

    RectTransform rectTransform;
    
    FAQData data;

    int setCnt = 0;

    public void Set(FAQData _data)
    {
        rectTransform = gameObject.GetComponent<RectTransform>();

        data = _data;

        txtTitle.text = data.title;
        txtMsg.text = data.msg;
        txtLinkTitle.text = data.linkTitle;
    }

    private void Update()
    {
        if (gameObject.activeInHierarchy == true && setCnt > 0 && setCnt < 3)
        {
            rectTransform.sizeDelta = new Vector2(0, 60 + txtTitle.rectTransform.sizeDelta.y);

            rectBG.sizeDelta = rectTransform.sizeDelta;
            rectBtnTween.sizeDelta = rectTransform.sizeDelta;

            tween.from = layoutElement.preferredHeight = rectTransform.sizeDelta.y;
            tween.to = tItemCell.sizeDelta.y;

            tween.duration = tween.to / 200 * 0.05f;
            setCnt++;
        }

        if (setCnt == 0) setCnt++;
    }

    public void Btn()
    {
        imgOpen.enabled = !imgOpen.enabled;
    }

    public void OpenLink()
    {
        Application.OpenURL(data.url);
    }
}
