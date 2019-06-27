using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProfileEditController : MonoBehaviour {

    public GameObject UIRoot;
    public Dropdown BirthDropdown;
    public ToggleGroup CharacterGroup;
    public Toggle[] CharacterToggles;
    public ToggleGroup HandTypeGroup;
    public Toggle[] HandTypeToggles;
    private List<string> years = new List<string> ();
    // public GameObject ModifyButtonLayout;

    public InputField Name;
    private ProfileVo selectedProfileVo;
    private ProfileVo currentProfileVo;
    public bool isCreate = false;
    public bool isDebug = false;
    private TokenVo tokenVo;
    private UserVo userVo;
    int birthIndex = 0;
    private string FromScene;

    void Start () {
        FromScene = SceneDTO.Instance.GetValue ("FromScene");
        makeBirthList ();
        BirthDropdown.value = 0;
        tokenVo = ConfigurationData.Instance.GetValueFromJson<TokenVo> ("Token");
        userVo = ConfigurationData.Instance.GetValueFromJson<UserVo> ("User");
        selectedProfileVo = ConfigurationData.Instance.GetValueFromJson<ProfileVo> ("SelectedProfile");
        currentProfileVo = ConfigurationData.Instance.GetValueFromJson<ProfileVo> ("CurrentProfile");

        SetBirthDropdownValue (selectedProfileVo.birth.ToString ());
        SetToggleHandType (selectedProfileVo.hand_type);
        SetCharacter (selectedProfileVo.character_name);
        SetProfileName (selectedProfileVo.name);
    }

    private void SetProfileName (string name) {
        Name.text = name;
    }

    private void SetCharacter (string character_name) {
        CharacterGroup.SetAllTogglesOff ();
        switch (character_name) {
            case "cheese":
                CharacterToggles[0].isOn = true;
                break;
            case "cherry":
                CharacterToggles[1].isOn = true;
                break;
            case "soda":
                CharacterToggles[2].isOn = true;
                break;
        }
    }

    private void SetToggleHandType (string hand_type) {
        HandTypeGroup.SetAllTogglesOff ();
        switch (hand_type) {
            case "left":
                HandTypeToggles[0].isOn = true;
                break;
            case "right":
                HandTypeToggles[1].isOn = true;
                break;
        }
    }

    public void onButtonClick (string type) {
        Toggle CharacterToggle = CharacterGroup.ActiveToggles ().FirstOrDefault ();
        Toggle HandTypeToggle = HandTypeGroup.ActiveToggles ().FirstOrDefault ();
        string chracter = CharacterToggle.gameObject.name;
        string handType = HandTypeToggle.gameObject.name;

        switch (type) {
            case "Modify":

                if(Name.text.Trim().Length == 0)
                {
                    BMUtil.Instance.OpenDialog(
                                LocalizationManager.Instance.GetLocalizedValue("alert_dialog_profile_title_profile_empty"),
                                LocalizationManager.Instance.GetLocalizedValue("alert_dialog_profile_body_profile_empty")
                            );
                    return;
                }

                List<ProfileVo> profileList = ConfigurationData.Instance.GetListFromJson<ProfileVo>("ProfileList");

                for (int i = 0; i < profileList.Count; i++)
                {
                    if ( profileList[i].profile_idx!= selectedProfileVo.profile_idx && profileList[i].name.Equals(Name.text) == true)
                    {
                        BMUtil.Instance.OpenDialog(
                            LocalizationManager.Instance.GetLocalizedValue("alert_dialog_profile_title_profile_duplicated"),
                            LocalizationManager.Instance.GetLocalizedValue("alert_dialog_profile_body_profile_duplicated")
                        );
                        return;
                    }
                }

                TrackingManager.Instance.Tracking("modify_profile", "profile_idx : "+selectedProfileVo.profile_idx);
                selectedProfileVo.birth = Int32.Parse (years[birthIndex]);
                selectedProfileVo.hand_type = HandTypeToggle.gameObject.name;
                selectedProfileVo.name = Name.text;
                selectedProfileVo.character_name = CharacterToggle.gameObject.name;

                ConfigurationData.Instance.SetJsonValue<ProfileVo>("SelectedProfile", selectedProfileVo);

                if (currentProfileVo.profile_idx == selectedProfileVo.profile_idx)
                    ConfigurationData.Instance.SetJsonValue<ProfileVo>("CurrentProfile", selectedProfileVo);

                RestService.Instance.UpdateProfile (tokenVo, selectedProfileVo, delegate {
                BrushMonSceneManager.Instance.LoadScene (FromScene);
                }, exception => { });
                break;
            case "Delete":
                if (userVo.profile_count == 1) {
                    // OpenDialog ("프로필 삭제 오류", "프로필은 한개 이상이어야 합니다.", "확인", "", true, false);
                    BMUtil.Instance.OpenDialog (
                        LocalizationManager.Instance.GetLocalizedValue("alert_dialog_profile_title_profile_one_more_than"), 
                        LocalizationManager.Instance.GetLocalizedValue("alert_dialog_profile_body_profile_one_more_than"),
                        LocalizationManager.Instance.GetLocalizedValue("common_button_done"),
                        "","", true, false, false
                    );
                    return;
                }
                //if (currentProfileVo.profile_idx == selectedProfileVo.profile_idx) {
                //    // OpenDialog ("프로필 삭제 오류", "사용중인 프로필은 삭제할 수 없습니다.", "확인", "", true, false);
                //    BMUtil.Instance.OpenDialog (
                //        LocalizationManager.Instance.GetLocalizedValue("alert_dialog_profile_title_profile_delete_current"), 
                //        LocalizationManager.Instance.GetLocalizedValue("alert_dialog_profile_body_profile_delete_current"),
                //        LocalizationManager.Instance.GetLocalizedValue("common_button_done"),
                //        "", true, false
                //    );
                //    return;
                //}

                bool isUseProfile = (currentProfileVo.profile_idx == selectedProfileVo.profile_idx);

                // OpenDialog ("프로필 삭제", "현재 프로필이 삭제됩니다.\n정말 삭제 하시겠습니까?", "삭제", "취소", true, true, delegate {
                BMUtil.Instance.OpenDialog (
                    LocalizationManager.Instance.GetLocalizedValue("alert_dialog_profile_title_profile_delete"),
                    LocalizationManager.Instance.GetLocalizedValue("alert_dialog_profile_body_profile_delete"),
                    LocalizationManager.Instance.GetLocalizedValue("common_button_delete"),
                    LocalizationManager.Instance.GetLocalizedValue("common_button_cancel"),
                    true, true, delegate
                    {
                        TrackingManager.Instance.Tracking("delete_profile", "profile_idx : " + selectedProfileVo.profile_idx);
                        RestService.Instance.DeleteProfile(tokenVo, selectedProfileVo.profile_idx, delegate
                        {
                            RestService.Instance.GetUser(tokenVo, user =>
                            {
                                ConfigurationData.Instance.SetJsonValue("User", user);

                                if (isUseProfile == true)
                                {
                                    BrushMonSceneManager.Instance.LoadScene("ProfileSelect");
                                }
                                else
                                {
                                    BrushMonSceneManager.Instance.LoadScene(FromScene);
                                }
                            }, exception => { });
                        }, exception => { });
                    }, delegate
                    {
                    }
                );

                break;
        }

    }

    void makeBirthList () {
        int currentYear = DateTime.Now.Year;
        for (int i = 0; i < 100; i++) {
            years.Add ((currentYear - i).ToString ());
        }
        BirthDropdown.AddOptions (years);
    }

    public void BirthDropdownValueChange (int index) {
        LogManager.Log (years[index]);
        birthIndex = index;
    }

    public void SetBirthDropdownValue (string year) {
        BirthDropdown.value = years.FindIndex (s => s.Equals (year));
    }

    public void onChangedCharacter () {

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
        BrushMonSceneManager.Instance.LoadScene (FromScene);
    }

    public void BtnHome()
    {
        BMUtil.Instance.OpenTitle();
    }

    public void CheckInputField(string txt)
    {
        Name.text = BMUtil.Instance.CheckText(txt);
    }

    public void OpenDialog (string title, string body, string doneName = "", string cancelName = "", bool isDone = true, bool isCancel = false, Action Done = null, Action Cancel = null) {
        //GameObject Dialog = Instantiate (Resources.Load ("Prefabs/Panel-Dialog")) as GameObject;
        //Dialog.transform.SetParent (UIRoot.transform, false);
        //DialogController controller = Dialog.GetComponent<DialogController> ();
        //controller.Title.text = title;
        //controller.Body.text = body;
        //controller.setActions (Done, Cancel);
        //controller.SetButtonName (doneName, cancelName);
        //controller.setButtonVisible (isDone, isCancel);

        //BMUtil.Instance.AddOpenDialogList(controller);
    }
}