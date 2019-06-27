using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SignInController : MonoBehaviour {
    public TweenInputField UserID;
    public TweenInputField Password;

    public GameObject objLogo;

    public GameObject UIRoot;

    public GameObject objLogin;
    public GameObject objSimpleLogin;

    public Transform tItemParent;
    public GameObject item;

    public Button btnLogin;
    public Button btnSimpleLogin;

    public Text txtPasswordLengthCheckMsg;

    private void Start()
    {
        //LogManager.Log("systemLanguage : " + Application.systemLanguage);
        //BMUtil.Instance.OpenToast("systemLanguage : " + Application.systemLanguage);

        List<SignVo> listUserAccount = BMUtil.Instance.LoadUserAccount();

        //저장된 계정이 있는지 체크
        if (listUserAccount != null && listUserAccount.Count > 0)
        {

            objSimpleLogin.SetActive(true);
            SetIDList(listUserAccount);
        }
        else
        {
            objLogin.SetActive(true);
            objLogin.transform.Find("Button-Back").gameObject.SetActive(false);
        }

        UserID.SetSelectAction(delegate { LogoPlayTween(true); }, delegate { LogoPlayTween(false); });
        Password.SetSelectAction(delegate { LogoPlayTween(true); }, delegate { LogoPlayTween(false); });


        btnLogin.interactable = false;
        btnLogin.GetComponent<Image>().color = Color.white;

        btnSimpleLogin.interactable = false;
        btnSimpleLogin.GetComponent<Image>().color = Color.white;

        txtPasswordLengthCheckMsg.text = LocalizationManager.Instance.GetLocalizedValue("login_password_length_check_msg");
    }

    #region SimpleLogin

    void SetIDList(List<SignVo> listUserAccount)
    {
        for (int i = 0; i < listUserAccount.Count; i++)
        {
            GameObject obj = Instantiate(item) as GameObject;
            obj.transform.SetParent(tItemParent, false);
            obj.transform.Find("ID").GetComponent<Text>().text = listUserAccount[i].email;
            obj.name = i.ToString();
            obj.SetActive(true);
        }

        tItemParent.GetComponent<RectTransform>().sizeDelta = new Vector2(tItemParent.GetComponent<RectTransform>().sizeDelta.x, 95 * tItemParent.childCount);
    }

    public void SelectItem(bool isSelect)
    {
        LogManager.Log("SelectItem : " + isSelect);
        if (isSelect == true && btnSimpleLogin.interactable == false)
        {
            btnSimpleLogin.interactable = true;
            btnSimpleLogin.GetComponent<Image>().color = new Color32(0x4A, 0xBD, 0xC6, 0xFF);
        }
        else
        {
            btnSimpleLogin.interactable = false;
            btnSimpleLogin.GetComponent<Image>().color = Color.white;
        }
    }

    public void OnClickSimpleLogin()
    {
        List<SignVo> listUserAccount = BMUtil.Instance.LoadUserAccount();

        ToggleGroup group = tItemParent.GetComponent<ToggleGroup>();

        foreach (var item in group.ActiveToggles())
        {
            SignVo signVo = FindSignVo(item.transform.Find("ID").GetComponent<Text>().text);
            SignIn(signVo.email, signVo.password);
            break;
        }
    }

    public SignVo FindSignVo(string email)
    {
        List<SignVo> listUserAccount = BMUtil.Instance.LoadUserAccount();

        for (int i = 0; i < listUserAccount.Count; i++)
        {
            if (listUserAccount[i].email.Equals(email))
            {
                return listUserAccount[i];
            }
        }

        return null;
    }

    public void OnClickOrderLogin()
    {
        objSimpleLogin.SetActive(false);
        objLogin.SetActive(true);
    }

    public void OnClickDeleteItem(GameObject obj)
    {
        BMUtil.Instance.RemoveUserAccount(obj.transform.Find("ID").GetComponent<Text>().text);
        BMUtil.Destroy(obj);

        if (tItemParent.childCount == 0)
        {
            objLogin.transform.Find("Button-Back").gameObject.SetActive(false);
            objSimpleLogin.SetActive(false);
            objLogin.SetActive(true);
        }
    }

    #endregion


    public void SignIn()
    {
        SignIn(UserID.text, Password.text);
    }

    public void SignIn (string id, string pass) {
        if (!IsValidEmail (id)) {
            // OpenDialog ("이메일 입력 오류", "이메일 형식이 맞지 않습니다.");
            BMUtil.Instance.OpenDialog (
                LocalizationManager.Instance.GetLocalizedValue ("alert_dialog_sign_title_email_format"),
                LocalizationManager.Instance.GetLocalizedValue ("alert_dialog_sign_body_email_format")
            );
            return;
        }
        if (string.IsNullOrEmpty (pass)) {
            // OpenDialog ("비밀번호 입력 오류", "비밀번호를 입력해주세요.");
            BMUtil.Instance.OpenDialog (
                LocalizationManager.Instance.GetLocalizedValue ("alert_dialog_sign_title_password_empty"),
                LocalizationManager.Instance.GetLocalizedValue ("alert_dialog_sign_body_password_empty")
            );
            return;
        }
        if (pass.Length < 4) {
            // OpenDialog ("비밀번호 입력 오류", "비밀번호는 4자리 문자를 초과하여 입력하셔야 합니다. 다시 입력해주세요.");
            BMUtil.Instance.OpenDialog (
                LocalizationManager.Instance.GetLocalizedValue ("alert_dialog_sign_title_password_length"),
                LocalizationManager.Instance.GetLocalizedValue ("alert_dialog_sign_body_password_length")
            );
            return;
        }

        RestService.Instance.SignIn (new SignVo (id, pass), token =>
        {
            ConfigurationData.Instance.SetJsonValue("Token", token);
            TokenVo tokenVo = ConfigurationData.Instance.GetValueFromJson<TokenVo>("Token");
            RestService.Instance.GetUser(tokenVo, user =>
            {
                //Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventLogin);
                //FirebaseManager.Instance.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventLogin);

                FirebaseManager.Instance.LogEvent("br_login_login");

                BMUtil.Instance.SaveUserAccount(new SignVo(id, pass));

                ConfigurationData.Instance.SetJsonValue("User", user);
                UserVo userVo = ConfigurationData.Instance.GetValueFromJson<UserVo>("User");

                LogManager.Log("userVo.app_language_text : " + userVo.app_language_text);

                LocalizationManager.Instance.SetLanguage((AppLanguage)System.Enum.Parse(typeof(AppLanguage), userVo.app_language_text));

                RestService.Instance.GetProfileList(tokenVo, result =>
                {
                    ConfigurationData.Instance.SetJsonValue("ProfileList", result);
                    List<ProfileVo> list = ConfigurationData.Instance.GetListFromJson<ProfileVo>("ProfileList");
                    int count = list == null ? 0 : list.Count;
                    
                    TrackingManager.Instance.Tracking("signIn", userVo.user_id);
                    userVo.profile_count = count;
                    ConfigurationData.Instance.SetJsonValue("User", userVo);
                    ConfigurationData.Instance.SetValue<bool>("isSkip", false);
                    switch (count)
                    {
                        case 0:
                            BrushMonSceneManager.Instance.LoadScene(SceneNames.ProfileCreate.ToString());
                            break;
                        case 1:
                            ConfigurationData.Instance.SetJsonValue<ProfileVo>("CurrentProfile", list[0]);
                            BrushMonSceneManager.Instance.LoadScene(SceneNames.Welcome.ToString());
                            break;
                        default:
                            BrushMonSceneManager.Instance.LoadScene(SceneNames.ProfileSelect.ToString());
                            break;
                    }

                }, exception => { LogManager.Log("2:" + exception.Message); });
            }, exception => { LogManager.Log("1:" + exception.Message); });

        }, exception =>
        {
            if (exception.Message.Equals("This email does not exist"))
            {
                // OpenDialog ("인증 에러", "등록된 사용자가 아닙니다.");
                BMUtil.Instance.OpenDialog(
                    LocalizationManager.Instance.GetLocalizedValue("alert_dialog_sign_title_not_found_user"),
                    LocalizationManager.Instance.GetLocalizedValue("alert_dialog_sign_body_not_found_user")
                );
            }
            if (exception.Message.Equals("The password is incorrect"))
            {
                // OpenDialog ("인증 에러", "비밀번호가 맞지 않습니다.");
                BMUtil.Instance.OpenDialog(
                    LocalizationManager.Instance.GetLocalizedValue("alert_dialog_sign_title_wrong_password"),
                    LocalizationManager.Instance.GetLocalizedValue("alert_dialog_sign_body_wrong_password")
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

    public void FindPassword () {
        BrushMonSceneManager.Instance.LoadScene (SceneNames.FindPassword.ToString ());
    }

    public void SignUp () {
        BrushMonSceneManager.Instance.LoadScene (SceneNames.SignUp.ToString ());
    }
    public void Skip () {
        ConfigurationData.Instance.SetValue<bool> ("isSkip", true);
        FirebaseManager.Instance.LogEvent("br_login_skip");
        BrushMonSceneManager.Instance.LoadScene (SceneNames.Welcome.ToString ());
    }
    public void SignInByFacebook () {

    }

    public void SignInByGoogle () {

    }

    public void CheckField()
    {
        UserID.text = BMUtil.Instance.CheckText(UserID.text);

        if (UserID.text.Length > 0 && string.IsNullOrEmpty(Password.text) == false && Password.text.Length >= 4)
        {
            btnLogin.interactable = true;
            btnLogin.GetComponent<Image>().color = new Color32(0x4A, 0xBD, 0xC6, 0xFF);
        }
        else
        {
            btnLogin.interactable = false;
            btnLogin.GetComponent<Image>().color = Color.white;
        }

        if(Password.text.Length < 4)
        {
            txtPasswordLengthCheckMsg.text = LocalizationManager.Instance.GetLocalizedValue("login_password_length_check_msg");
        }
        else
        {
            txtPasswordLengthCheckMsg.text = "";
        }
    }

    //public void OpenDialog (string title, string body, string doneName = "", string cancelName = "", bool isDone = true, bool isCancel = false, Action Done = null, Action Cancel = null) {
    //    GameObject Dialog = Instantiate (Resources.Load ("Prefabs/Panel-Dialog")) as GameObject;
    //    Dialog.transform.SetParent (UIRoot.transform, false);
    //    DialogController controller = Dialog.GetComponent<DialogController> ();
    //    controller.Title.text = title;
    //    controller.Body.text = body;
    //    controller.setActions (Done, Cancel);
    //    controller.SetButtonName (doneName, cancelName);
    //    controller.setButtonVisible (true, false);
    //}

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

    /// <summary>
    /// 일반 로그인 화면에서 뒤로가기 눌렀을때 간편로그인으로 돌아가기 기능 추가
    /// </summary>
    public void OnClickBack()
    {
        objSimpleLogin.SetActive(true);
        objLogin.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if(objSimpleLogin.activeInHierarchy == true)
            {
                Skip();
            }
            else if(objLogin.activeInHierarchy == true)
            {
                OnClickBack();
            }
        }
    }
}