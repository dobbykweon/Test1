using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LocalizationManager : GlobalMonoSingleton<LocalizationManager> {

	private Dictionary<string, string> localizedText;
	private bool isReady = false;
    public bool IsReady { get { return isReady; } }

    private string missingTextString = "Localized text not found";

	void Awake () 
	{
        StartCoroutine(StartSetLanguage());
	}

    IEnumerator StartSetLanguage()
    {
        while (ConfigurationData.Instance == null)
            yield return null;

        SetLanguage(LoadLanguage());
    }

	public void SetLanguage(SystemLanguage language)
	{
		isReady = false;

		switch (language) {
			case SystemLanguage.Korean:
				StartCoroutine(LoadLocalized ("localizedText_kr.json"));
				break;

			case SystemLanguage.English:
				StartCoroutine(LoadLocalized ("localizedText_en.json"));
				break;

            case SystemLanguage.ChineseTraditional:
                StartCoroutine(LoadLocalized("localizedText_cn_t.json"));
                break;

            case SystemLanguage.ChineseSimplified:
                StartCoroutine(LoadLocalized("localizedText_cn_s.json"));
                break;

            case SystemLanguage.Japanese:
                StartCoroutine(LoadLocalized("localizedText_jp.json"));
                break;

            default:
				StartCoroutine(LoadLocalized("localizedText_en.json"));
				break;
		}
		
		//언어설정 저장
		SaveLanguage(language);
	}

    public void SetLanguage(AppLanguage language)
    {
        isReady = false;

        switch (language)
        {
            case AppLanguage.KR:
                StartCoroutine(LoadLocalized("localizedText_kr.json"));
                break;

            case AppLanguage.EN:
                StartCoroutine(LoadLocalized("localizedText_en.json"));
                break;

            case AppLanguage.TW:
                StartCoroutine(LoadLocalized("localizedText_cn_t.json"));
                break;

            case AppLanguage.CN:
                StartCoroutine(LoadLocalized("localizedText_cn_s.json"));
                break;

            case AppLanguage.JP:
                StartCoroutine(LoadLocalized("localizedText_jp.json"));
                break;

            default:
                StartCoroutine(LoadLocalized("localizedText_en.json"));
                break;
        }
    }

    IEnumerator LoadLocalized(string fileName)
    {
		string filePath = Path.Combine (Application.streamingAssetsPath, fileName);
		string dataAsJson;
		localizedText = new Dictionary<string, string> ();
        if (filePath.Contains("://")) {
            UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(filePath);
            yield return www.SendWebRequest();
            dataAsJson = www.downloadHandler.text;
        } else
            dataAsJson = System.IO.File.ReadAllText(filePath);

		LocalizationData loadedData = JsonUtility.FromJson<LocalizationData> (dataAsJson);

			for (int i = 0; i < loadedData.items.Length; i++) {
				localizedText.Add (loadedData.items[i].key, loadedData.items[i].value);
			}
		isReady = true;
    }

	void SaveLanguage(SystemLanguage language)
	{
        if (ConfigurationData.Instance.HasKey("User") == true)
        {
            UserVo userVo = ConfigurationData.Instance.GetValueFromJson<UserVo>("User");
            userVo.app_language_text = BMUtil.Instance.GetAppLanguage(language).ToString();
            ConfigurationData.Instance.SetJsonValue("User", userVo);
        }

        //PlayerPrefs.SetInt("language", (int)language);
    }

	public SystemLanguage LoadLanguage()
	{
        if (ConfigurationData.Instance.HasKey("User") == true)
        {
            UserVo userVo = ConfigurationData.Instance.GetValueFromJson<UserVo>("User");

            if (string.IsNullOrEmpty(userVo.app_language_text) == true)
            {
                if (PlayerPrefs.HasKey("language") == true)
                {
                    SystemLanguage language = (SystemLanguage)PlayerPrefs.GetInt("language", (int)Application.systemLanguage);
                    userVo.app_language_text = BMUtil.Instance.GetAppLanguage(language).ToString();
                }
                else
                    userVo.app_language_text = BMUtil.Instance.GetAppLanguage(Application.systemLanguage).ToString();

                ConfigurationData.Instance.SetJsonValue("User", userVo);
            }

            return BMUtil.Instance.GetAppLanguage((AppLanguage)System.Enum.Parse(typeof(AppLanguage), userVo.app_language_text));
        }
        else
        {
            return Application.systemLanguage;
        }
	}

    public void SaveVoice(SystemLanguage language)
    {
        if (ConfigurationData.Instance.HasKey("User") == true)
        {
            UserVo userVo = ConfigurationData.Instance.GetValueFromJson<UserVo>("User");
            userVo.app_language_voice = BMUtil.Instance.GetAppVoice(language).ToString();
            ConfigurationData.Instance.SetJsonValue("User", userVo);
        }
        //ConfigurationData.Instance.SetValue<int>("voice", (int)language);
    }

    public SystemLanguage LoadVoice()
    {
        //return (SystemLanguage)ConfigurationData.Instance.GetValue<int>("voice", (int)Application.systemLanguage);

        if (ConfigurationData.Instance.HasKey("User") == true)
        {
            UserVo userVo = ConfigurationData.Instance.GetValueFromJson<UserVo>("User");

            if (string.IsNullOrEmpty(userVo.app_language_voice) == true)
            {
                if (PlayerPrefs.HasKey("voice") == true)
                {
                    SystemLanguage language = (SystemLanguage)PlayerPrefs.GetInt("voice", (int)Application.systemLanguage);
                    userVo.app_language_voice = BMUtil.Instance.GetAppVoice(language).ToString();
                }
                else
                    userVo.app_language_voice = BMUtil.Instance.GetAppVoice(Application.systemLanguage).ToString();

                ConfigurationData.Instance.SetJsonValue("User", userVo);
            }

            //LogManager.Log("LoadVoice : " + userVo.app_language_voice);

            return BMUtil.Instance.GetAppVoice((AppVoice)System.Enum.Parse(typeof(AppVoice), userVo.app_language_voice));
        }
        else
        {
            return Application.systemLanguage;
        }
    }


    public string GetLocalizedValue (string key) {
		string result = missingTextString;
		if (localizedText.ContainsKey (key)) {
			result = localizedText[key];
		}

		return result;

	}

	public bool GetIsReady () {
		return isReady;
	}

    public void SetFirebaseMessagingSubscribe()
    {
        
    }

}