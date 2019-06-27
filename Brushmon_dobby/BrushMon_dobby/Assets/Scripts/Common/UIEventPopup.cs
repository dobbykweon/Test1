using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class EventInfo
{
    public long idx;
    public long start_date;
    public long end_date;
    public string img_url;
    public string link_url;
    public string content;
    //public string country;

    public bool isShow = true;
}

public enum EventCountry
{
    AL,//알바니아
    AT,//오스트리아
    AU,//호주
    BN,//브루나이
    BR,//브라질
    CA,//캐나다
    CH,//스위스
    CN,//중국
    CO,//콜롬비아
    CY,//사이프러스
    CZ,//체코
    DE,//독일
    EC,//에콰도르
    ES,//스페인
    FR,//프랑스
    GB,//영국
    GG,//건지
    GR,//그리스
    HK,//홍콩
    HN,//온두라스
    IE,//아일랜드
    IL,//이스라엘
    IN,//인도
    IT,//이탈리아
    JP,//일본
    KR,//대한민국
    LV,//라트비아
    NL,//네덜란드
    NZ,//뉴질랜드
    PL,//폴란드
    RO,//루마니아
    RS,//세르비아
    RU,//러시아
    SG,//싱가포르
    TR,//터키
    TW,//대만
    US,//미국
    VN,//베트남
    ZA,//남아공
    All,
}

public class NotLookAgainData
{
    public long date;
    public List<long> idxs;

    public NotLookAgainData()
    {
        idxs = new List<long>();
    }
}

public class UIEventPopup : MonoBehaviour
{
    [SerializeField] RawImage img;
    [SerializeField] Toggle toggle;
    [SerializeField] GameObject obj;

    int showIdx = 0;

    List<EventInfo> listEvent;

    int maxWidth = 1000;
    int maxHeight = 1500;

    // 테스트용 데이터 셋팅
    //private void Start()
    //{
    //    ConfigurationData.Instance.DeleteKey("event_not_look_again");

    //    listEvent = new List<EventInfo>();

    //    for (int i = 0; i < 3; i++)
    //    {
    //        EventInfo info = new EventInfo();

    //        info.idx = 100 + i;
    //        info.end_date = BMUtil.Instance.GetGMTinMs(DateTime.Today.AddDays(i));
    //        info.img_url = "https://scontent-icn1-1.xx.fbcdn.net/v/t1.0-9/55596913_2340016962896832_626273164527140864_n.png?_nc_cat=109&_nc_ht=scontent-icn1-1.xx&oh=89ef94eea0fbf010f18677cd0c2f4e50&oe=5D334285";
    //        info.link_url = "https://www.naver.com/";
    //        info.country = EventCountry.All.ToString();
            
    //        listEvent.Add(info);
    //    }

    //    Init(listEvent);
    //}

    public void Init(string data)
    {
        LogManager.Log("UIEventPopup : " + data);

        listEvent = JsonHelper.getJsonArray<EventInfo>(data);

        if (listEvent.Count > 0)
        {
            CheckData();
            CheckNotLookAgain();
        }

        Next();
    }

    public void Init(List<EventInfo> _listEvent)
    {
        listEvent = _listEvent;

        if (listEvent.Count > 0)
        {
            CheckData();
            CheckNotLookAgain();
        }

        Next();
    }

    void CheckData()
    {
        int cnt = 0;

        while(cnt < listEvent.Count)
        {
            //// 나라 체크
            ////if (NativePlugin.Instance.GetNativeCountryCode() != listEvent[cnt].country && listEvent[cnt].country != "All")
            //{
            //    listEvent.RemoveAt(cnt);
            //}
            //// 이벤트 종료일 체크
            //else 

            if (listEvent[cnt].start_date >= BMUtil.Instance.GetGMTinMs(DateTime.Today) || listEvent[cnt].end_date <= BMUtil.Instance.GetGMTinMs(DateTime.Today))
            {
                listEvent.RemoveAt(cnt);
            }
            else
            {
                cnt++;
            }
        }
    }

    void CheckNotLookAgain()
    {
        if (ConfigurationData.Instance.HasKey("event_not_look_again") == true)
        {
            NotLookAgainData againData = ConfigurationData.Instance.GetValueFromJson<NotLookAgainData>("event_not_look_again");

            if (BMUtil.Instance.local_create_date(againData.date) != DateTime.Today)
            {
                ConfigurationData.Instance.DeleteKey("event_not_look_again");
            }
            else
            {
                for (int i = 0; i < listEvent.Count; i++)
                {
                    listEvent[i].isShow = !againData.idxs.Contains(listEvent[i].idx);
                }
            }
        }
    }

    void Next()
    {
        if(listEvent.Count > showIdx)
        { 
            if(listEvent[showIdx].isShow == true)
            {
                StartCoroutine(SetImage(listEvent[showIdx].img_url));
            }
            else
            {
                showIdx++;
                Next();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator SetImage(string url)
    {
        if (string.IsNullOrEmpty(url) == false)
        {
            WWW www = new WWW(url);
            yield return www;

            Texture texture = www.texture;

            if (texture.width > maxWidth || texture.height > maxHeight)
            {
                if (texture.width > texture.height)
                {
                    img.rectTransform.sizeDelta = new Vector2(texture.width * ((float)maxHeight / texture.height), texture.height * ((float)maxHeight / texture.height));
                }
                else
                {
                    img.rectTransform.sizeDelta = new Vector2(texture.width * ((float)maxWidth / texture.width), texture.height * ((float)maxWidth / texture.width));
                }
            }
            else
            {
                img.rectTransform.sizeDelta = new Vector2(texture.width, texture.height);
            }

            img.texture = texture;

            img.enabled = true;
            obj.SetActive(true);
        }
    }

    public void GoLink()
    {
        LogManager.Log("GoLink : " + listEvent[showIdx].link_url);

        if (string.IsNullOrEmpty(listEvent[showIdx].link_url) == false)
        {
            LogManager.Log("GoLink : " + listEvent[showIdx].link_url);

            Application.OpenURL(listEvent[showIdx].link_url);
        }
    }

    public void Close()
    {
        listEvent[showIdx].isShow = !toggle.isOn;

        if (toggle.isOn == true)
        {
            NotLookAgainData againData;

            if (ConfigurationData.Instance.HasKey("event_not_look_again") == false)
            {
                againData = new NotLookAgainData();
                againData.date = BMUtil.Instance.GetGMTinMs(DateTime.Today);
                againData.idxs.Add(listEvent[showIdx].idx);
            }
            else
            {
                againData = ConfigurationData.Instance.GetValueFromJson<NotLookAgainData>("event_not_look_again");

                if (BMUtil.Instance.local_create_date(againData.date) != DateTime.Today)
                {
                    againData.idxs.Clear();
                    againData.date = BMUtil.Instance.GetGMTinMs(DateTime.Today);
                    againData.idxs.Add(listEvent[showIdx].idx);
                }
                else
                {
                    againData.idxs.Add(listEvent[showIdx].idx);
                }
            }

            ConfigurationData.Instance.SetJsonValue("event_not_look_again", againData);
        }

        toggle.isOn = false;
        img.enabled = false;
        obj.SetActive(false);

        showIdx++;
        Next();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Close();
        }
    }

}
