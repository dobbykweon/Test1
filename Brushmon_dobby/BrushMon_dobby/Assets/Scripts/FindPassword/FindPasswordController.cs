using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FindPasswordController : MonoBehaviour {

    public GameObject UIRoot;
    public TweenInputField Email;

    //public GameObject objLogo;
    public Button btn;

    private void Start()
    {
        btn.interactable = false;
        btn.GetComponent<Image>().color = Color.white;
        Email.SetSelectAction(delegate { LogoPlayTween(true); }, delegate { LogoPlayTween(false); });
    }

    public void Update () {
        //if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyUp (KeyCode.Escape)) {
                onBackPress ();
                return;
            }
        }
    }

    public void onBackPress () {
        BrushMonSceneManager.Instance.LoadScene ("SignIn");
    }

    public void FindPassward () {
        if (!IsValidEmail (Email.text)) {
            // OpenDialog ("이메일 입력 오류", "이메일 형식이 맞지 않습니다.");
            BMUtil.Instance.OpenDialog (
                LocalizationManager.Instance.GetLocalizedValue("alert_dialog_sign_title_email_format"), 
                LocalizationManager.Instance.GetLocalizedValue("alert_dialog_sign_body_email_format")
            );
            return;
        }
        UserEmailVo emailVo = new UserEmailVo();
        emailVo.email = Email.text;
        RestService.Instance.FindPasswordEmail(emailVo, delegate{
            // OpenDialog ("재설정 메일 발송 완료", "입력하신 메일 주소로 비밀번호\n재설정 메일이 발송 되었습니다.");
            BMUtil.Instance.OpenDialog (
                LocalizationManager.Instance.GetLocalizedValue("alert_dialog_findpw_title_send_email"), 
                LocalizationManager.Instance.GetLocalizedValue("alert_dialog_findpw_body_send_email")
            );

            FirebaseManager.Instance.LogEvent("br_login_discover_pw");
        }, exception => {
            if("not found email".Equals(exception.Message))
            {
                // OpenDialog ("이메일 입력 오류", "해당 메일주소는 존재하지 않습니다.\n다시 입력해 주세요.");
                BMUtil.Instance.OpenDialog (
                    LocalizationManager.Instance.GetLocalizedValue("alert_dialog_findpw_title_email_not_found"), 
                    LocalizationManager.Instance.GetLocalizedValue("alert_dialog_findpw_body_email_not_found")
                );
            }
        });
    }

    public bool IsValidEmail (string email) {
        Regex regex = new Regex (@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        return regex.Match (email).Success;
    }

    public void CheckField()
    {
        Email.text = BMUtil.Instance.CheckText(Email.text);

        if (IsValidEmail(Email.text))
        {
            btn.interactable = true;
            btn.GetComponent<Image>().color = new Color32(0x4A, 0xBD, 0xC6, 0xFF);
        }
        else
        {
            btn.interactable = false;
            btn.GetComponent<Image>().color = Color.white;
        }
    }

    public void LogoPlayTween(bool isFront)
    {
        //if (isFront == true)
        //{
        //    iTween.ScaleTo(objLogo, Vector3.zero, 0.3f);
        //}
        //else
        //{
        //    iTween.ScaleTo(objLogo, Vector3.one, 0.3f);
        //}
    }
}