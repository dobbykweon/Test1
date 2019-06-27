using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SignUpController : MonoBehaviour {
    public TweenInputField UserID;
    public TweenInputField Password;
    public TweenInputField PasswordCheck;

    public Text txtCheckPasswordLength;
    public Text txtCheckPassword;

    public GameObject objToggleViewPass;

    public GameObject objLogo;
    public Button btnSignUp;

    public Toggle Terms;

    public GameObject UIRoot;

    public Image toggleBack;

    private void Start()
    {
        FirebaseManager.Instance.LogEvent("br_login_signup");

        btnSignUp.interactable = false;
        btnSignUp.GetComponent<Image>().color = Color.white;

        UserID.SetSelectAction(delegate { LogoPlayTween(true); }, delegate { LogoPlayTween(false); });
        Password.SetSelectAction(delegate { LogoPlayTween(true); }, delegate { LogoPlayTween(false); });
        PasswordCheck.SetSelectAction(delegate { LogoPlayTween(true); }, delegate { LogoPlayTween(false); });

        txtCheckPasswordLength.text = LocalizationManager.Instance.GetLocalizedValue("sign_up_pass_length");
        objToggleViewPass.SetActive(false);
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
        FirebaseManager.Instance.LogEvent("br_signup_back");
        BrushMonSceneManager.Instance.LoadScene (SceneNames.SignIn.ToString ());
    }

    public void SignUp () {
        if (!IsValidEmail(UserID.text))
        {
            // OpenDialog ("이메일 입력 오류", "이메일 형식이 맞지 않습니다.");
            BMUtil.Instance.OpenDialog(
                LocalizationManager.Instance.GetLocalizedValue("alert_dialog_sign_title_email_format"),
                LocalizationManager.Instance.GetLocalizedValue("alert_dialog_sign_body_email_format")
            );
            return;
        }

        if (string.IsNullOrEmpty(Password.text))
        {
            // OpenDialog ("비밀번호 입력 오류", "비밀번호를 입력해주세요.");
            BMUtil.Instance.OpenDialog(
                LocalizationManager.Instance.GetLocalizedValue("alert_dialog_sign_title_password_empty"),
                LocalizationManager.Instance.GetLocalizedValue("alert_dialog_sign_body_password_empty")
            );
            return;
        }

        if (Password.text.Length < 4)
        {
            // OpenDialog ("비밀번호 입력 오류", "비밀번호는 4자리 문자를 초과하여 입력하셔야 합니다. 다시 입력해주세요.");
            BMUtil.Instance.OpenDialog(
                LocalizationManager.Instance.GetLocalizedValue("alert_dialog_sign_title_password_length"),
                LocalizationManager.Instance.GetLocalizedValue("alert_dialog_sign_body_password_length")
            );
            return;
        }
        
        //if (string.IsNullOrEmpty(PasswordCheck.text))
        //{
        //    // OpenDialog ("비밀번호 입력 오류", "확인용 비밀번호를 입력해주세요.");
        //    //OpenDialog(
        //    //    LocalizationManager.Instance.GetLocalizedValue("alert_dialog_sign_title_password_empty"),
        //    //    LocalizationManager.Instance.GetLocalizedValue("alert_dialog_sign_body_password_empty")
        //    //);
        //    return;
        //}

        if(Password.text.Equals(PasswordCheck.text) == false)
        {
            //OpenDialog("비밀번호 입력 오류", "비밀번호가 일치하지 않습니다.");

            BMUtil.Instance.OpenDialog(
                LocalizationManager.Instance.GetLocalizedValue("alert_dialog_sign_title_password_empty"),
                LocalizationManager.Instance.GetLocalizedValue("alert_dialog_sign_message_password_check")
            );
            return;
        }
        
        if (!Terms.isOn)
        {
            // OpenDialog ("약관 동의 오류", "이용약관 동의를 체크해주세요.");
            BMUtil.Instance.OpenDialog(
                LocalizationManager.Instance.GetLocalizedValue("alert_dialog_sign_title_term"),
                LocalizationManager.Instance.GetLocalizedValue("alert_dialog_sign_body_term")
            );
            return;
        }
        

        RestService.Instance.SignUp (new SignVo (UserID.text, Password.text), token =>
        {
            ConfigurationData.Instance.SetJsonValue("Token", token);
            TokenVo tokenVo = ConfigurationData.Instance.GetValueFromJson<TokenVo>("Token");
            RestService.Instance.GetUser(tokenVo, user =>
            {
                //Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventSignUp);
                //FirebaseManager.Instance.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventSignUp);

                FirebaseManager.Instance.LogEvent("br_signup_done");

                BMUtil.Instance.SaveUserAccount(new SignVo(UserID.text, Password.text));

                ConfigurationData.Instance.SetJsonValue("User", user);

                RestService.Instance.GetProfileList(tokenVo, result =>
                {
                    ConfigurationData.Instance.SetJsonValue("ProfileList", result);
                    List<ProfileVo> list = ConfigurationData.Instance.GetListFromJson<ProfileVo>("ProfileList");
                    int count = list.Count;
                    UserVo userVo = ConfigurationData.Instance.GetValueFromJson<UserVo>("User");
                    
                    userVo.profile_count = count;
                    userVo.device_country = NativePlugin.Instance.GetNativeCountryCode();
                    userVo.device_language = NativePlugin.Instance.GetNativeLanguageCode();
                    userVo.app_language_text = BMUtil.Instance.GetAppLanguage(Application.systemLanguage).ToString();
                    userVo.app_language_voice = BMUtil.Instance.GetAppVoice(Application.systemLanguage).ToString();
                    userVo.app_version = Application.version;

                    ConfigurationData.Instance.SetJsonValue("User", userVo);
                    ConfigurationData.Instance.SetValue<bool>("isSkip", false);
                    TrackingManager.Instance.Tracking("signUp", userVo.user_id);

                    LocalizationManager.Instance.SetLanguage((AppLanguage)System.Enum.Parse(typeof(AppLanguage), userVo.app_language_text));

                    //회원 가입시 디바이스의 나라, 언어, 앱언어, 앱음성, 앱 버전 업데이트
                    RestService.Instance.UpdateUserSetting(tokenVo, userVo, delegate
                    {
                        switch (count)
                        {
                            case 0:
                                BrushMonSceneManager.Instance.LoadScene("ProfileCreate");
                                break;
                            case 1:
                                ConfigurationData.Instance.SetJsonValue<ProfileVo>("CurrentProfile", list[0]);
                                BrushMonSceneManager.Instance.LoadScene(SceneNames.Welcome.ToString());
                                break;
                            default:
                                BrushMonSceneManager.Instance.LoadScene(SceneNames.ProfileSelect.ToString());
                                break;
                        }
                    }, exception =>
                    {
                        LogManager.Log(exception.Message);
                        BrushMonSceneManager.Instance.LoadScene(SceneNames.Welcome.ToString());
                    });

                }, exception => { });
            }, exception => { });

        }, exception =>
        {
            if (exception.Message.Equals("email duplication"))
            {
                // OpenDialog ("이메일 중복 오류", "이미 가입된 이메일 입니다.");
                BMUtil.Instance.OpenDialog(
                    LocalizationManager.Instance.GetLocalizedValue("alert_dialog_sign_title_email_duplicated"),
                    LocalizationManager.Instance.GetLocalizedValue("alert_dialog_sign_body_email_duplicated")
                );
            }
        });
    }

    public void ViewTerm () {
        Application.OpenURL ("http://www.brushmon.com/terms");
    }

    public bool IsValidEmail (string email) {
        Regex regex = new Regex (@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        return regex.Match (email).Success;
    }

    public void CheckField()
    {
        UserID.text = BMUtil.Instance.CheckText(UserID.text);

        if (UserID.text.Length > 0 && string.IsNullOrEmpty(Password.text) == false && Password.text.Length >= 4 && Password.text.Equals(PasswordCheck.text) == true)
        {
            btnSignUp.interactable = true;
            btnSignUp.GetComponent<Image>().color = new Color32(0x4A, 0xBD, 0xC6, 0xFF);
        }
        else
        {
            btnSignUp.interactable = false;
            btnSignUp.GetComponent<Image>().color = Color.white;
        }

        if(Password.text.Length > 0 && PasswordCheck.text.Length > 0 && Password.text.Equals(PasswordCheck.text) != true)
        {
            ////비밀번호가 일치하지 않습니다.
            txtCheckPassword.text = LocalizationManager.Instance.GetLocalizedValue("alert_dialog_sign_message_password_check");
        }
        else
        {
            txtCheckPassword.text = "";
        }

        if (Password.text.Length < 8 || Password.text.Length > 15)
        {
            txtCheckPasswordLength.text = LocalizationManager.Instance.GetLocalizedValue("sign_up_pass_length");
            objToggleViewPass.SetActive(false);
        }
        else
        {
            txtCheckPasswordLength.text = "";
            objToggleViewPass.SetActive(true);
        }
    }

    public void LogoPlayTween(bool isFront)
    {
        if (isFront == true)
        {
            iTween.ScaleTo(objLogo, Vector3.zero, 0.3f);
        }
        else
        {
            iTween.ScaleTo(objLogo, Vector3.one, 0.3f);
        }
    }

    public void SetEnablePassword(bool isEnable)
    {
        toggleBack.enabled = !isEnable;
    }
}