using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMessageBox : MonoBehaviour
{
    public Transform tItemParent;
    public GameObject prefab;

    public void SetList()
    {
        Message_Data_List dataList = MessageManager.Instance.Data_List;

        if (dataList != null && dataList.datatList != null)
        {
            for (int i = 0; i < dataList.datatList.Count; i++)
            {
                GameObject item = Instantiate(prefab) as GameObject;
                item.transform.SetParent(tItemParent, false);
                item.GetComponent<UIMessageItem>().SetMsg(dataList.datatList[i], ResetList);
                item.SetActive(true);
            }
        }

        if (tItemParent.childCount > 1)
        {
            tItemParent.GetComponent<RectTransform>().sizeDelta = new Vector2(tItemParent.GetComponent<RectTransform>().sizeDelta.x, 200 * (tItemParent.childCount - 1));
        }
        else
        {
            tItemParent.GetComponent<RectTransform>().sizeDelta = new Vector2(tItemParent.GetComponent<RectTransform>().sizeDelta.x, 200);

            Text msg = tItemParent.gameObject.AddComponent<Text>();
            msg.text = "내용이 없습니다.";
            msg.fontSize = 60;
            msg.alignment = TextAnchor.MiddleCenter;
            msg.font = Resources.Load("Fonts/NotoSans-Bold") as Font;
        }
    }

    public void BtnClose()
    {
        Destroy(gameObject);
    }

    public void ResetList()
    {
        tItemParent.GetComponent<RectTransform>().sizeDelta = new Vector2(tItemParent.GetComponent<RectTransform>().sizeDelta.x, 200 * (tItemParent.childCount - 1));
    }


}
