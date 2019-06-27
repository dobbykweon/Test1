using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProfileSelectController : MonoBehaviour {

    public GameObject ContentsLayout;
    private List<ProfileVo> profileList;
    private string FromScene;
    private TokenVo tokenVo;
    // private UserVo userVo;
    private ProfileVo currentProfileVo;
    public GameObject BackButton;
    
    void Start () {
        FromScene = SceneDTO.Instance.GetValue ("FromScene");
        tokenVo = ConfigurationData.Instance.GetValueFromJson<TokenVo> ("Token");
        // userVo = ConfigurationData.Instance.GetValueFromJson<UserVo> ("User");
        currentProfileVo = ConfigurationData.Instance.GetValueFromJson<ProfileVo> ("CurrentProfile");

        LogManager.Log("ProfileSelectController__FromScene : "+ FromScene);

        RestService.Instance.GetProfileList(tokenVo, result =>
        {
            ConfigurationData.Instance.SetJsonValue("ProfileList", result);
            List<ProfileVo> list = ConfigurationData.Instance.GetListFromJson<ProfileVo>("ProfileList");
            foreach (ProfileVo item in list)
            {
                GameObject Character = Instantiate(Resources.Load("Prefabs/Profile/" + item.character_name)) as GameObject;
                Toggle toggle = Character.GetComponent<Toggle>();
                Text label = Character.GetComponentInChildren<Text>();
                toggle.group = ContentsLayout.GetComponent<ToggleGroup>();
                label.text = item.name;
                if (currentProfileVo != null && item.profile_idx == currentProfileVo.profile_idx)
                {
                    toggle.isOn = true;
                }
                Character.transform.SetParent(ContentsLayout.transform, false);
                TargetObjectVo target = Character.AddComponent<TargetObjectVo>();
                target.setObject<ProfileVo>(item);
                toggle.onValueChanged.AddListener(OnProfileChanged);
            }

            if (currentProfileVo != null && FromScene.Equals(SceneNames.Welcome.ToString()))
                BackButton.SetActive(true);
            else
                BackButton.SetActive(false);

            GameObject AddUserRoot = Instantiate(Resources.Load("Prefabs/Profile/Button-AddUser")) as GameObject;
            AddUserRoot.transform.SetParent(ContentsLayout.transform, false);
            AddUserRoot.GetComponentInChildren<Button>().onClick.AddListener(AddUser);
        }, exception =>
        {
            onBackPress();
        });
    }


    private void OnProfileChanged(bool isChecked)
    {
        if (isChecked)
        {
            BMUtil.Instance.OpenLoading();

            Toggle theActiveToggle = ContentsLayout.GetComponent<ToggleGroup>().ActiveToggles().FirstOrDefault();
            ConfigurationData.Instance.SetJsonValue<ProfileVo>("CurrentProfile", theActiveToggle.GetComponent<TargetObjectVo>().getObject<ProfileVo>());

            if (currentProfileVo != null)
                TrackingManager.Instance.Tracking("change_profile", "profile_idx : " + currentProfileVo.profile_idx + "=>" + theActiveToggle.GetComponent<TargetObjectVo>().getObject<ProfileVo>().profile_idx);
            else
                TrackingManager.Instance.Tracking("change_profile", "profile_idx : -1=>" + theActiveToggle.GetComponent<TargetObjectVo>().getObject<ProfileVo>().profile_idx);

            currentProfileVo = theActiveToggle.GetComponent<TargetObjectVo>().getObject<ProfileVo>();

            //BrushMonSceneManager.Instance.LoadScene (SceneNames.Welcome.ToString());
            GetPreSticker();
        }
    }

    void GetPreSticker()
    {
        if (currentProfileVo == null) return;

        RestService.Instance.GetProfile(tokenVo, currentProfileVo.profile_idx,
            result =>
            {
                ConfigurationData.Instance.SetJsonValue("CurrentProfile", result);

                currentProfileVo = ConfigurationData.Instance.GetValueFromJson<ProfileVo>("CurrentProfile");

                RestService.Instance.GetPreSticker(tokenVo, currentProfileVo.profile_idx, stickerResult =>
                {
                    BMUtil.Instance.CloseLoading();

                    ConfigurationData.Instance.SetJsonValue("PreSticker", stickerResult);

                    if (currentProfileVo.name.Length > 8)
                        BMUtil.Instance.OpenNextSceneToast(string.Format(LocalizationManager.Instance.GetLocalizedValue("msg_profile_change"),  (currentProfileVo.name.Substring(0, 8))));
                    else
                        BMUtil.Instance.OpenNextSceneToast(string.Format(LocalizationManager.Instance.GetLocalizedValue("msg_profile_change"), currentProfileVo.name));

                    BrushMonSceneManager.Instance.LoadScene(SceneNames.Welcome.ToString());
                }, exception =>
                {
                    LogManager.LogError("exception: " + exception.Message);
                });
            },
            exception =>
            {
                LogManager.Log("GetProfile exception: " + exception.Message);
            });

        
    }

    public void AddUser () {
        BrushMonSceneManager.Instance.LoadScene (SceneNames.ProfileCreate.ToString());
    }
    public void Update () {
        //if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyUp (KeyCode.Escape))
            {
                onBackPress ();
                return;
            }
        }
    }

    public void onBackPress () {
        if (FromScene.Equals(SceneNames.Welcome.ToString()))
        {
            BrushMonSceneManager.Instance.LoadScene (SceneNames.Welcome.ToString());
        }
    }
}