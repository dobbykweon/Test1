using UnityEngine;
using UnityEngine.UI;

using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class UIUserMessage : MonoBehaviour {

    [SerializeField] InputField inputTitle;
    [SerializeField] InputField inputMessage;

    public void OnClickSendEmail()
    {
        BMUtil.Instance.OpenLoading();
        SendEmail();
    }

    void SendEmail()
    {
        UserVo userVo = ConfigurationData.Instance.GetValueFromJson<UserVo>("User");
        ProfileVo currentProfileVo = ConfigurationData.Instance.GetValueFromJson<ProfileVo>("CurrentProfile");

        string userInfo = "\n\n" +
            "ID : " + userVo.user_id + "\n" +
            "Profile Name : " + currentProfileVo.name + "\n" +
            "Device Model : " + SystemInfo.deviceModel + "\n" +
            "Device OS : " + SystemInfo.operatingSystem + "\n";

        MailMessage mail = new MailMessage();

        mail.From = new MailAddress("leech0051@gmail.com");
        mail.To.Add("leech0051@gmail.com");
        mail.Subject = inputTitle.text;
        mail.Body = inputMessage.text + "\n\n" + userInfo;


        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
        smtpServer.Port = 587;
        smtpServer.Credentials = new System.Net.NetworkCredential("abydos3755@gmail.com", "1qaz2wsx~") as ICredentialsByHost;
        smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback =
        delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        { return true; };
        smtpServer.Send(mail);
        Debug.Log("success");

        BMUtil.Instance.CloseLoading();
        BMUtil.Instance.OpenToast("문의메일 전송 완료");

        gameObject.SetActive(false);
    }

    public void OnClickClose()
    {
        gameObject.SetActive(false);
    }
}
