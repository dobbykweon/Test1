using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class NativePlugin : GlobalMonoSingleton<NativePlugin>
{

#if (UNITY_IPHONE && !UNITY_EDITOR)
    [DllImport("__Internal")]
    extern static private void _log(string message);

    [DllImport("__Internal")]
    private static extern string _userCountryCode();

    [DllImport("__Internal")]
    private static extern string _userLanguageCode();
#endif



#if (UNITY_ANDROID && !UNITY_EDITOR)
    private AndroidJavaObject activityContext = null;
    private AndroidJavaClass javaClass = null;
    private AndroidJavaObject javaClassInstance = null;
#endif

    private void Awake()
    {
#if (UNITY_ANDROID && !UNITY_EDITOR)
        using (AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        }

        using (javaClass = new AndroidJavaClass("kittenplanet.com.bmonpluginforandroid.UnityPluginMainCalss"))
        {
            if(javaClass != null)
            {
                javaClassInstance = javaClass.CallStatic<AndroidJavaObject>("instance");
                javaClassInstance.Call("SetContext", activityContext);
            }
        }
#endif
    }

    public string GetNativeCountryCode()
    {
        string _countryCode = "KR";

        try
        {
#if (UNITY_IPHONE && !UNITY_EDITOR)
            _countryCode = _userCountryCode();
#elif (UNITY_ANDROID && !UNITY_EDITOR)
            _countryCode = javaClassInstance.Call<string>("GetCountry");
#endif
        }
        catch (Exception e)
        {
            LogManager.Log("GetNativeCountryCode Error : " + e.Message);
        }
        return _countryCode;
    }

    public string GetNativeLanguageCode()
    {
        string _languageCode = "KR";

        try
        {
#if (UNITY_IPHONE && !UNITY_EDITOR)
            _languageCode = _userLanguageCode();
#elif (UNITY_ANDROID && !UNITY_EDITOR)
            _languageCode = javaClassInstance.Call<string>("GetLanguage");
#endif
        }
        catch (Exception e)
        {
            LogManager.Log("GetNativeCountryCode Error : " + e.Message);
        }

        return _languageCode;
    }
}
