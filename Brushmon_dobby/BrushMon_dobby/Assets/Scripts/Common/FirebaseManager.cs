using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase.Analytics;

public enum FirebaseUserProperty
{
    age,
    country,
    language,
}

public class FirebaseManager : GlobalMonoSingleton<FirebaseManager>
{
#if OFFLINE
    bool isEnableFirebase = false;
#else
    bool isEnableFirebase = true;
#endif

    private void Awake()
    {
        LogManager.Log("URL : " + RestService.ServiceUrl);
    }

    public void FirebaseInit()
    {
        if (isEnableFirebase == true)
        {
            Firebase.Messaging.FirebaseMessaging.TokenReceived += MessageManager.Instance.OnTokenReceived;
            Firebase.Messaging.FirebaseMessaging.MessageReceived += MessageManager.Instance.OnMessageReceived;
        }
    }

    public void LogEvent(string name)
    {
        if (isEnableFirebase == true)
        {
#if UNITY_EDITOR
            LogManager.Log("Firebase.LogEvent(" + name + ")");
#else
            FirebaseAnalytics.LogEvent(name);
#endif
        }
    }

    public void LogEvent(string name, string valueName, int value)
    {
        if (isEnableFirebase == true)
        {
#if UNITY_EDITOR
            LogManager.Log("Firebase.LogEvent(" + name + ", " + valueName + " : " + value + ")");
#else
            FirebaseAnalytics.LogEvent(name, valueName, value);
#endif
        }
    }

    public void LogEvent(string name, string valueName, double value)
    {
        if (isEnableFirebase == true)
        {
#if UNITY_EDITOR
            LogManager.Log("Firebase.LogEvent(" + name + ", " + valueName + " : " + value + ")");
#else
            FirebaseAnalytics.LogEvent(name, valueName, value);
#endif
        }
    }

    public void LogEvent(string name, string valueName, long value)
    {
        if (isEnableFirebase == true)
        {
#if UNITY_EDITOR
            LogManager.Log("Firebase.LogEvent(" + name + ", " + valueName + " : " + value + ")");
#else
            FirebaseAnalytics.LogEvent(name, valueName, value);
#endif
        }
    }

    public void LogEvent(string name, string valueName, string value)
    {
        if (isEnableFirebase == true)
        {
#if UNITY_EDITOR
            LogManager.Log("Firebase.LogEvent(" + name + ", " + valueName + " : " + value + ")");
#else
            FirebaseAnalytics.LogEvent(name, valueName, value);
#endif
        }
    }

    public void LogEvent(string name, Parameter[] parameters)
    {
        if (isEnableFirebase == true)
        {
#if UNITY_EDITOR
            LogManager.Log("Firebase.LogEvent(" + name + "(parametersCnt : " + parameters.Length + "))");
#else
            FirebaseAnalytics.LogEvent(name, parameters);
#endif
        }
    }

    public void SetUserInfo()
    {
        if (isEnableFirebase == true)
        {
            string country = NativePlugin.Instance.GetNativeCountryCode();
            string language = NativePlugin.Instance.GetNativeLanguageCode();

            //국가
            SetUserProperty(FirebaseUserProperty.country, country);
            //언어
            SetUserProperty(FirebaseUserProperty.language, NativePlugin.Instance.GetNativeLanguageCode());
        }
    }

    void SetUserProperty(FirebaseUserProperty key, string value)
    {
        if (isEnableFirebase == true)
        {
            //LogManager.Log("SetUserProperty_" + key + " : " + value);
            FirebaseAnalytics.SetUserProperty(key.ToString(), value);
        }
    }
}
