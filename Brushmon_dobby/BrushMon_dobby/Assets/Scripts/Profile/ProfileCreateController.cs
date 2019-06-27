using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProfileCreateController : MonoBehaviour {

    public GameObject UIRoot;
    public Dropdown BirthDropdown;
    public ToggleGroup CharacterGroup;
    public Toggle[] CharacterToggles;
    public ToggleGroup HandTypeGroup;
    public Toggle[] HandTypeToggles;
    private List<string> years = new List<string> ();
    public InputField Name;
    private ProfileVo profileVo;
    private TokenVo tokenVo;
    // private UserVo userVo;
    private string FromScene;

    public GameObject objBackButton;
    public GameObject objHomeButton;

    void Start () {
        FromScene = SceneDTO.Instance.GetValue ("FromScene");
        
        objBackButton.SetActive(SceneNames.ProfileSelect.ToString().Equals(FromScene));
        objHomeButton.SetActive(SceneNames.ProfileSelect.ToString().Equals(FromScene));

        makeBirthList ();
        BirthDropdown.value = 0;
        tokenVo = ConfigurationData.Instance.GetValueFromJson<TokenVo> ("Token");
        // userVo = ConfigurationData.Instance.GetValueFromJson<UserVo> ("User");
        profileVo = new ProfileVo ();
    }

    private void SetProfileName (string name) {
        Name.text = name;
    }

    private void SetCharacter (string character_name) {
        CharacterGroup.SetAllTogglesOff ();
        switch (character_name) {
            case "cheesse":
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

    public void OnClickCreateProfile () {
        if (string.IsNullOrEmpty (Name.text)) {
            // OpenDialog ("프로필 입력 오류", "프로필 이름을 입력해주세요.");
            BMUtil.Instance.OpenDialog (
                LocalizationManager.Instance.GetLocalizedValue("alert_dialog_profile_title_profile_empty"), 
                LocalizationManager.Instance.GetLocalizedValue("alert_dialog_profile_body_profile_empty")
            );
            return;
        }

        Toggle CharacterToggle = CharacterGroup.ActiveToggles ().FirstOrDefault ();
        Toggle HandTypeToggle = HandTypeGroup.ActiveToggles ().FirstOrDefault ();
        string chracter = CharacterToggle.gameObject.name;
        string handType = HandTypeToggle.gameObject.name;
        profileVo.birth = Int32.Parse (years[birthIndex]);
        profileVo.hand_type = HandTypeToggle.gameObject.name;
        profileVo.name = Name.text;
        profileVo.character_name = CharacterToggle.gameObject.name;
        RestService.Instance.RegisterProfile (tokenVo, profileVo, profile_idx => {
            profileVo.profile_idx = profile_idx;
            ConfigurationData.Instance.SetJsonValue ("CurrentProfile", JsonUtility.ToJson(profileVo));
            TrackingManager.Instance.Tracking("add_profile", "Regist Profile");
            RestService.Instance.GetUser (tokenVo, user => {
                ConfigurationData.Instance.SetJsonValue ("User", user);
                if(SceneNames.ProfileSelect.ToString().Equals(FromScene))
                    BrushMonSceneManager.Instance.LoadScene (FromScene);
                else
                    BrushMonSceneManager.Instance.LoadScene (SceneNames.Welcome.ToString());
            }, exception => { });
        }, exception => {
            if (exception.Message.Equals("profile name duplication"))
            {
                // OpenDialog ("이름 중복 오류", "프로필 이름이 중복입니다.");
                BMUtil.Instance.OpenDialog(
                    LocalizationManager.Instance.GetLocalizedValue("alert_dialog_profile_title_profile_duplicated"),
                    LocalizationManager.Instance.GetLocalizedValue("alert_dialog_profile_body_profile_duplicated")
                );
            }
        });
    }

    public void CheckInputField(string txt)
    {
        Name.text = BMUtil.Instance.CheckText(txt);
    }

    void makeBirthList () {
        int currentYear = DateTime.Now.Year;
        for (int i = 0; i < 100; i++) {
            years.Add ((currentYear - i).ToString ());
        }
        BirthDropdown.AddOptions (years);
    }

    int birthIndex = 0;
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
        if(SceneNames.ProfileSelect.ToString().Equals(FromScene))
            BrushMonSceneManager.Instance.LoadScene (FromScene);
    }

    public void BtnHome()
    {
        BMUtil.Instance.OpenTitle();
    }
}