using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProfileEditSelectController : MonoBehaviour {

    public GameObject ContentsLayout;
    private List<ProfileVo> profileList;
    // private string FromScene;
    private TokenVo tokenVo;
    // private UserVo userVo;
    private ProfileVo currentProfileVo;
    public GameObject BackButton;
    
    void Start () {
        // FromScene = SceneDTO.Instance.GetValue ("FromScene");
        tokenVo = ConfigurationData.Instance.GetValueFromJson<TokenVo> ("Token");
        // userVo = ConfigurationData.Instance.GetValueFromJson<UserVo> ("User");
        currentProfileVo = ConfigurationData.Instance.GetValueFromJson<ProfileVo> ("CurrentProfile");
        BackButton.SetActive(true);
        RestService.Instance.GetProfileList (tokenVo, result => {
            ConfigurationData.Instance.SetJsonValue ("ProfileList", result);
            List<ProfileVo> list = ConfigurationData.Instance.GetListFromJson<ProfileVo> ("ProfileList");
            foreach (ProfileVo item in list) {
                GameObject Character = Instantiate (Resources.Load ("Prefabs/Profile/" + item.character_name)) as GameObject;
                Toggle toggle = Character.GetComponent<Toggle> ();
                Text label = Character.GetComponentInChildren<Text> ();
                toggle.group = ContentsLayout.GetComponent<ToggleGroup> ();
                label.text = item.name;
                if (item.profile_idx == currentProfileVo.profile_idx)
                    toggle.isOn = true;
                Character.transform.SetParent (ContentsLayout.transform, false);
                TargetObjectVo target = Character.AddComponent<TargetObjectVo>();
                target.setObject<ProfileVo>(item);
                toggle.onValueChanged.AddListener (OnProfileChanged);
            }
        }, exception => { });
    }

    
    private void OnProfileChanged (bool isChecked) {
        if (isChecked) {
            Toggle theActiveToggle = ContentsLayout.GetComponent<ToggleGroup> ().ActiveToggles ().FirstOrDefault ();
            ConfigurationData.Instance.SetJsonValue<ProfileVo> ("SelectedProfile", theActiveToggle.GetComponent<TargetObjectVo>().getObject<ProfileVo>());
            BrushMonSceneManager.Instance.LoadScene (SceneNames.ProfileEdit.ToString());
        }
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
        BrushMonSceneManager.Instance.LoadScene (SceneNames.Setting.ToString());
    }

    public void BtnHome()
    {
        BMUtil.Instance.OpenTitle();
    }
}