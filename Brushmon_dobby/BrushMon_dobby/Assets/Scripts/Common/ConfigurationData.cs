using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
public class ConfigurationData : GlobalMonoSingleton<ConfigurationData> {

    void Awake(){
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    public T GetValue<T> (string key, object defaultValue) {
        object returnValue = null;

#if OFFLINE
        if (typeof (T) == typeof (int)) {
            returnValue = PlayerPrefs.GetInt ("offline_" + key, (int) defaultValue);
            return (T) returnValue;
        } else if (typeof (T) == typeof (float)) {
            returnValue = PlayerPrefs.GetFloat ("offline_" + key, (float) defaultValue);
            return (T) returnValue;
        } else if (typeof (T) == typeof (string)) {
            returnValue = PlayerPrefs.GetString ("offline_" + key, (string) defaultValue);
            return (T) returnValue;
        } else if (typeof (T) == typeof (bool)) {
            returnValue = ((int) PlayerPrefs.GetInt ("offline_" + key, (((bool) defaultValue) ? 1 : 0)) == 1 ? true : false);
            return (T) returnValue;
        } else {
            return (T) defaultValue;
        }
#else
        if (typeof (T) == typeof (int)) {
            returnValue = PlayerPrefs.GetInt (key, (int) defaultValue);
            return (T) returnValue;
        } else if (typeof (T) == typeof (float)) {
            returnValue = PlayerPrefs.GetFloat (key, (float) defaultValue);
            return (T) returnValue;
        } else if (typeof (T) == typeof (string)) {
            returnValue = PlayerPrefs.GetString (key, (string) defaultValue);
            return (T) returnValue;
        } else if (typeof (T) == typeof (bool)) {
            returnValue = ((int) PlayerPrefs.GetInt (key, (((bool) defaultValue) ? 1 : 0)) == 1 ? true : false);
            return (T) returnValue;
        } else {
            return (T) defaultValue;
        }
#endif
    }

    public void SetValue<T> (string key, object value) {
#if OFFLINE
        if (typeof (T) == typeof (int)) {
            PlayerPrefs.SetInt ("offline_" + key, (int) value);
        } else if (typeof (T) == typeof (float)) {
            PlayerPrefs.SetFloat ("offline_" + key, (float) value);
        } else if (typeof (T) == typeof (string)) {
            PlayerPrefs.SetString ("offline_" + key, (string) value);
        } else if (typeof (T) == typeof (bool)) {
            PlayerPrefs.SetInt ("offline_" + key, ((bool) value) ? 1 : 0);
        }
#else
        if (typeof (T) == typeof (int)) {
            PlayerPrefs.SetInt (key, (int) value);
        } else if (typeof (T) == typeof (float)) {
            PlayerPrefs.SetFloat (key, (float) value);
        } else if (typeof (T) == typeof (string)) {
            PlayerPrefs.SetString (key, (string) value);
        } else if (typeof (T) == typeof (bool)) {
            PlayerPrefs.SetInt (key, ((bool) value) ? 1 : 0);
        }
#endif
        PlayerPrefs.Save ();
    }

    public void SetJsonValue (string key, string value) {
        value = BMUtil.Instance.Encode(value);
#if OFFLINE
        PlayerPrefs.SetString ("offline_" + key, value);
#else
        PlayerPrefs.SetString (key, value);
#endif
        PlayerPrefs.Save ();
    }

    public void SetJsonValue<T> (string key, T value) {
        string json = JsonUtility.ToJson (value);

        json = BMUtil.Instance.Encode(json);
#if OFFLINE
        PlayerPrefs.SetString ("offline_" + key, json);
#else
        PlayerPrefs.SetString(key, json);
#endif
        PlayerPrefs.Save ();
    }
    public T GetValueFromJson<T> (string key) {
        try {
#if OFFLINE
            T value = JsonUtility.FromJson<T>(BMUtil.Instance.Decode(PlayerPrefs.GetString("offline_" + key, "")));
#else
            T value = JsonUtility.FromJson<T> (BMUtil.Instance.Decode(PlayerPrefs.GetString (key, "")));
#endif
            return value;
        } catch (Exception exception) {
            string message = exception.Message;
            Debug.LogWarning("GetValueFromJson(" + key + ") : " + message);
            return default (T);
        }
    }

    public List<T> GetListFromJson<T> (string key)
    {
#if OFFLINE
        List<T> value = JsonHelper.getJsonArray<T>(BMUtil.Instance.Decode(PlayerPrefs.GetString("offline_" + key, "")));
#else
        List<T> value = JsonHelper.getJsonArray<T> (BMUtil.Instance.Decode(PlayerPrefs.GetString (key, "")));
#endif
        return value;
    }

    public void SetList<T> (string key, List<T> list) {
        var binaryFormatter = new BinaryFormatter ();
        var memoryStream = new MemoryStream ();
        binaryFormatter.Serialize (memoryStream, list);
        PlayerPrefs.SetString (key, Convert.ToBase64String (memoryStream.GetBuffer ()));
        PlayerPrefs.Save ();
    }

    public List<T> GetList<T> (string key) {
        var data = PlayerPrefs.GetString (key);
        List<T> result = new List<T> ();

        if (!string.IsNullOrEmpty (data)) {
            var binaryFormatter = new BinaryFormatter ();
            var memoryStream = new MemoryStream (Convert.FromBase64String (data));
            result = (List<T>) binaryFormatter.Deserialize (memoryStream);
        }
        return result;
    }

    public void SetDictionary<T> (string key, Dictionary<string, T> dictionary) {
        var binaryFormatter = new BinaryFormatter ();
        var memoryStream = new MemoryStream ();
        binaryFormatter.Serialize (memoryStream, dictionary);
        PlayerPrefs.SetString ("key", Convert.ToBase64String (memoryStream.GetBuffer ()));
        PlayerPrefs.Save ();
    }

    public Dictionary<string, T> GetDictionary<T> (string key) {
#if OFFLINE
        var data = PlayerPrefs.GetString("offline_" + key);
#else
        var data = PlayerPrefs.GetString(key);
#endif
        Dictionary<string, T> result = new Dictionary<string, T> ();

        if (!string.IsNullOrEmpty (data)) {
            var binaryFormatter = new BinaryFormatter ();
            var memoryStream = new MemoryStream (Convert.FromBase64String (data));
            result = (Dictionary<string, T>) binaryFormatter.Deserialize (memoryStream);
        }
        return result;
    }

    public void SetObjectInDictionary<T> (string rootKey, string key, object value) {
        Dictionary<string, T> data = GetDictionary<T> (rootKey);
        if (data == null) {
            data = new Dictionary<string, T> ();
        }

        if (data.ContainsKey (key)) {
            data[key] = (T) value;
        } else {
            data.Add (key, (T) value);
        }
    }

    public T GetObjectInDictionary<T> (string rootKey, string key) {
        return (T) GetDictionary<T> (rootKey) [key];
    }

    public bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    public void CleanValue (string key) {
        PlayerPrefs.SetString (key, null);
    }

    public void CleanAll () {
        //PlayerPrefs.DeleteAll();
        RemoveData();
        RemoveJsonData();
    }

    public void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }

    public void RemoveData()
    {
        // "isFirstEnter"은 처음 실행시 인트로무비를 플레이 하기 위해 체크하므로 삭제하지 않음
        //PlayerPrefs.DeleteKey("isFirstEnter");    
        PlayerPrefs.DeleteKey("isSkip");
        PlayerPrefs.DeleteKey("BleDeviceName");
        PlayerPrefs.DeleteKey("CameraZoom");
        PlayerPrefs.DeleteKey("voice");
        PlayerPrefs.DeleteKey("FromWelcome");
    }

    public void RemoveJsonData()
    {
        PlayerPrefs.DeleteKey("Token");
        PlayerPrefs.DeleteKey("User");
        PlayerPrefs.DeleteKey("PreSticker");
        PlayerPrefs.DeleteKey("CurrentProfile");
        PlayerPrefs.DeleteKey("ProfileList");
    }

}