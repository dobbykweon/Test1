using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EmailField : InputField
{
    Transform tList;
    GameObject item;
    Transform tItemParent;

    Vector3 firstPos;
    GameObject objLogo;
    GameObject objTweenTarget;
    float uiScale;

    protected override void Start()
    {
        //tList = transform.Find("AccountList");
        tList = BMUtil.Instance.UIRoot.Find("AccountList");

        tItemParent = tList.Find("Viewport/Content");
        item = tList.Find("Item").gameObject;
        item.SetActive(false);
        tList.gameObject.SetActive(false);

        firstPos = transform.parent.position;
        objLogo = BMUtil.Instance.UIRoot.Find("Spine-Logo").gameObject;
        objTweenTarget = transform.parent.gameObject;
        uiScale = BMUtil.Instance.UIRoot.localScale.x;
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);

        //저장된 계정정보 가져오기
        List<SignVo> listUserAccount = BMUtil.Instance.LoadUserAccount();

        //저장된 계정이 있는지 체크
        if (listUserAccount!= null && listUserAccount.Count > 0)
        {
            tList.gameObject.SetActive(true);
            OpenList();
        }

        PlayTween(true);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);

        //tList.gameObject.SetActive(false);

        PlayTween(false);
    }

    void PlayTween(bool isFront)
    {
        if (isFront)
        {
            iTween.MoveTo(objTweenTarget, firstPos + Vector3.up * 400 * uiScale, 0.3f);
            iTween.ScaleTo(objLogo, Vector3.zero, 0.3f);
        }
        else
        {
            iTween.MoveTo(objTweenTarget, firstPos, 0.3f);
            iTween.ScaleTo(objLogo, Vector3.one, 0.3f);
        }
    }

    /// <summary>
    /// 저장된 계정 정보 보여주기
    /// </summary>
    public void OpenList(/*string inputTxt*/)
    {
        List<SignVo> listUserAccount = BMUtil.Instance.LoadUserAccount();

        if (listUserAccount == null) return;

        if (tItemParent.childCount > 0)
        {
            bool isActive = false;

            for(int i  = 0; i < tItemParent.childCount; i++)
            {
                Transform t = tItemParent.GetChild(i);

                bool isShow = (string.IsNullOrEmpty(this.text) == true || (t.name.StartsWith(this.text) && t.name.Equals(this.text) == false));

                t.gameObject.SetActive(isShow);

                if(isActive == false && isShow == true)
                {
                    isActive = true;
                }
            }

            tList.gameObject.SetActive(isActive);

        }
        else
        {
            for (int i = 0; i < listUserAccount.Count; i++)
            {
                if (string.IsNullOrEmpty(this.text) == true || listUserAccount[i].email.StartsWith(this.text))
                {
                    GameObject obj = Instantiate(item) as GameObject;
                    obj.transform.SetParent(tItemParent, false);
                    obj.transform.GetChild(0).GetComponent<Text>().text = listUserAccount[i].email;
                    obj.name = listUserAccount[i].email;
                    obj.SetActive(true);
                }
            }
        }

        tItemParent.GetComponent<RectTransform>().sizeDelta = new Vector2(500 * tItemParent.childCount, tItemParent.GetComponent<RectTransform>().sizeDelta.y);

    }

    public void SetField(GameObject obj)
    {
        LogManager.Log("SetField : "+ obj.name);
        this.text = obj.name;

        List<SignVo> listUserAccount = BMUtil.Instance.LoadUserAccount();

        for(int i = 0; i < listUserAccount.Count; i++)
        {
            if(listUserAccount[i].email.Equals(this.text) == true)
            {
                BMUtil.Instance.UIRoot.Find("ObjCenter/Password").GetComponent<InputField>().text = listUserAccount[i].password;
                break;
            }
        }
    }

}
