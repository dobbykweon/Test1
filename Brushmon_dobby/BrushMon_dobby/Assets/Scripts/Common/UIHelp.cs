using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

using System.ComponentModel;
using UnityEngine.EventSystems;

enum HelpTap
{
    FAQ,
    Question,
}


[Serializable]
public class FAQData
{
    public string title;
    public string msg;
    public string linkTitle;
    public string url;
}

public class UIHelp : MonoBehaviour
{

    [SerializeField] GameObject objFAQ;
    [SerializeField] GameObject objQuestion;

    [SerializeField] GameObject objItemFAQ;
    [SerializeField] GameObject objItemUserEmailButton;

    [SerializeField] Dropdown dropdownCategory;
    [SerializeField] Text txtInputLimit;
    [SerializeField] Toggle toggle;

    [SerializeField] InputField inputMessage;
    [SerializeField] InputField inputPhoneNum;

    [SerializeField] Text txtCategory;

    [SerializeField] Font defaultFont;
    [SerializeField] Font selectFont;

    Action closeAction;

    Mopsicus.Plugins.MobileInputField mInputMessage;
    Mopsicus.Plugins.MobileInputField mInputPhoneNumber;

    private void Start()
    {
        objFAQ.SetActive(true);
        objQuestion.SetActive(false);

        mInputMessage = inputMessage.transform.GetComponent<Mopsicus.Plugins.MobileInputField>();
        mInputPhoneNumber = inputPhoneNum.transform.GetComponent<Mopsicus.Plugins.MobileInputField>();

        txtInputLimit.text = inputMessage.text.Length + "/" + inputMessage.characterLimit;

        StartCoroutine(LoadFAQ());
    }


    public void SetCloseCallback(Action action)
    {
        closeAction = action;
    }



    public void BtnQuestion()
    {
        BMUtil.Instance.OpenToast("question");
        Debug.Log("question");
        objFAQ.SetActive(false);
        objQuestion.SetActive(true);

        dropdownCategory.value = 0;

        txtCategory.font = defaultFont;
        txtCategory.color = new Color((float)0xc1 / (float)0xff, (float)0xc1 / (float)0xff, (float)0xc1 / (float)0xff);

        toggle.isOn = false;

        mInputMessage.ResetInputField();
        mInputPhoneNumber.ResetInputField();

        txtInputLimit.text = inputMessage.text.Length + "/" + inputMessage.characterLimit;
    }

    public void SelectCategory(int value)
    {
        if(value == 0)
        {
            txtCategory.font = defaultFont;
            txtCategory.color = new Color((float)0xc1 / (float)0xff, (float)0xc1 / (float)0xff, (float)0xc1 / (float)0xff);
        }
        else
        {
            txtCategory.font = selectFont;
            txtCategory.color = new Color((float)0x33 / (float)0xff, (float)0x33 / (float)0xff, (float)0x33 / (float)0xff);
        }
    }

    public void CheckText(string text)
    {
        txtInputLimit.text = text.Length + "/" + inputMessage.characterLimit;
    }

    public void SendEmail()
    {
        mInputMessage.SetVisible(false);
        mInputPhoneNumber.SetVisible(false);

        if (inputMessage.text.Length == 0 || inputPhoneNum.text.Length == 0 || dropdownCategory.value == 0)
        {
            BMUtil.Instance.OpenDialog("미입력 항목 존재", "작성하지 않은 항목이 있습니다.\n항목을 확인해주세요.", "확인", "", true);
        }
        else if (toggle.isOn == true)
        {
            BMUtil.Instance.OpenLoading();
            StartCoroutine(SetEmail());
        }
        else
        {
            BMUtil.Instance.OpenDialog("정보동의항목 미체크", "개인정보처리방침에 동의를 해주세요.", "확인", "", true);
        }
    }

    IEnumerator SetEmail()
    {
        yield return new WaitForSeconds(0.1f);

        string userInfo;

        UserVo userVo = ConfigurationData.Instance.GetValueFromJson<UserVo>("User");

        if (userVo != null)
        {
            ProfileVo currentProfileVo = ConfigurationData.Instance.GetValueFromJson<ProfileVo>("CurrentProfile");

            userInfo = "\n\n" +
                "아이디 : " + userVo.user_id + "\n" +
                "프로필 : " + currentProfileVo.name + "\n" +
                "Device Model : " + SystemInfo.deviceModel + "\n" +
                "Device OS : " + SystemInfo.operatingSystem + "\n";
        }
        else
        {
            userInfo = "\n\n" +
                "아이디 : " + "None" + "\n" +
                "프로필 : " + "None" + "\n" +
                "Device Model : " + SystemInfo.deviceModel + "\n" +
                "Device OS : " + SystemInfo.operatingSystem + "\n";
        }

        try
        {

            //if (txtPhoneNumber.text.Length > 0) userInfo += ("\n연락처 : " + txtPhoneNumber.text);
            if (inputPhoneNum.text.Length > 0) userInfo += ("\n연락처 : " + inputPhoneNum.text);

            MailMessage mail = new MailMessage();

            mail.From = new MailAddress("brushmonster.help@gmail.com", "Kittenplanet", System.Text.Encoding.UTF8); // 보내는사람
            mail.To.Add(new MailAddress("brushmonster.help@gmail.com", "Kittenplanet", System.Text.Encoding.UTF8)); // 받는 사람
            mail.Subject = "[" + dropdownCategory.captionText.text + "]";
            mail.Body = inputMessage.text + userInfo;
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = false;

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;

            smtpClient.Credentials = new System.Net.NetworkCredential("brushmonster.help@gmail.com", "KPtree0405~") as ICredentialsByHost; // 보내는사람 주소 및 비밀번호 확인
            ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            { return true; };

            //smtpClient.Send(mail);

            smtpClient.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
            smtpClient.SendAsync(mail, "kittenplanet");


        }
        catch (Exception e)
        {
            LogManager.Log("Email Send Error : " + e.Message);

            BMUtil.Instance.CloseLoading();
            BMUtil.Instance.OpenToast("문의 / 의견 보내기 등록실패");
        }
    }

    void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
    {
        // Get the unique identifier for this asynchronous operation.
        string token = (string)e.UserState;

        if (e.Cancelled)
        {
            //Console.WriteLine("[{0}] Send canceled.", token);
            LogManager.Log("[" + token + "] Send canceled.");

            BMUtil.Instance.CloseLoading();
            BMUtil.Instance.OpenToast("문의 / 의견 보내기 등록실패");
        }

        if (e.Error != null)
        {
            //Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
            LogManager.Log("[" + token + "] " + e.Error.ToString());

            BMUtil.Instance.CloseLoading();
            BMUtil.Instance.OpenToast("문의 / 의견 보내기 등록실패");
        }
        else
        {
            //Console.WriteLine("Message sent.");
            LogManager.Log("Message sent.");

            BMUtil.Instance.CloseLoading();
            BMUtil.Instance.OpenToast("문의 / 의견 보내기 등록완료");
            objFAQ.SetActive(true);
            objQuestion.SetActive(false);
        }

       

    }

    IEnumerator LoadFAQ()
    {
        string fileName = "FAQ_kr.json";

        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        string dataAsJson;

        if (filePath.Contains("://"))
        {
            UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(filePath);
            yield return www.SendWebRequest();
            dataAsJson = www.downloadHandler.text;
        }
        else
            dataAsJson = System.IO.File.ReadAllText(filePath);

        List<FAQData> faqDataList = JsonHelper.getJsonArray<FAQData>(dataAsJson);

        for (int i = 0; i < faqDataList.Count; i++)
        {
            GameObject item = Instantiate(objItemFAQ) as GameObject;
            item.transform.SetParent(objItemFAQ.transform.parent, false);
            item.SetActive(true);

            UIItemFAQ itemFAQ = item.GetComponent<UIItemFAQ>();
            itemFAQ.Set(faqDataList[i]);
            itemFAQ.gameObject.SetActive(true);
        }

        GameObject objEmpty0 = new GameObject();
        objEmpty0.transform.SetParent(objItemFAQ.transform.parent, false);
        objEmpty0.AddComponent<LayoutElement>().preferredHeight = 60;

        GameObject itemButton = Instantiate(objItemUserEmailButton) as GameObject;
        itemButton.transform.SetParent(objItemFAQ.transform.parent, false);
        itemButton.SetActive(true);

        GameObject objEmpty1 = new GameObject();
        objEmpty1.transform.SetParent(objItemFAQ.transform.parent, false);
        objEmpty1.AddComponent<LayoutElement>().preferredHeight = 120;
        
        yield return new WaitForEndOfFrame();
    }

    public void Close()
    {
        if (BMUtil.Instance.IsDialog == true) return;

        if (objQuestion.activeInHierarchy == true)
        {
            mInputMessage.SetVisible(false);
            mInputPhoneNumber.SetVisible(false);

            BMUtil.Instance.OpenDialog("입력취소", "입력을 취소하시겠습니까?", "예", "아니오", true, true, 
                delegate
                {
                    objFAQ.SetActive(true);
                    objQuestion.SetActive(false);
                });
        }
        else
        {
            Destroy(gameObject);
            closeAction();
        }
    }

    /// <summary>
    /// 버튼형식으로 핸드폰번호 입력 Text Object 선택시 호출되는 함수
    /// </summary>
    public void BtnSelectInputPhoneNumber()
    {
        StartCoroutine(SelectInputPhoneNumber());
    }

    IEnumerator SelectInputPhoneNumber()
    {
        yield return new WaitForFixedUpdate();
        mInputPhoneNumber.SetVisible(true);
        yield return new WaitForFixedUpdate();
        mInputPhoneNumber.SetFocus(true);
    }

    /// <summary>
    /// 버튼형식으로 이메일 내용 입력 Text Object 선택시 호출되는 함수
    /// </summary>
    public void BtnSelectInputMessage()
    {
        StartCoroutine(SelectMessageInput());
    }

    IEnumerator SelectMessageInput()
    {
        yield return new WaitForFixedUpdate();
        mInputMessage.SetVisible(true);
        yield return new WaitForFixedUpdate();
        mInputMessage.SetFocus(true);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Close();
        }
    }
}