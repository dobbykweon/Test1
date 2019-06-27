using System;
using UnityEngine;
using UnityEngine.UI;

public class GPSController : MonoSingleton<GPSController>
{
    private bool isCheckGPS = true;

    public void TurnOnGPS()
    {
        try
        {
            using (AndroidJavaClass classPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject objActivity = classPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                using (AndroidJavaObject jo = new AndroidJavaObject("kr.lithos.applications.brushmon.location_permission.VerifyLocation"))
                {
                    jo.CallStatic("TurnOnGPS", new object[] { objActivity });
                    LogManager.Log(":: Unity GPS:: GPS Turning on");
                };
            };
        }
        catch (Exception e)
        {
            LogManager.LogError("GPS Turning on Exception : " + e.Message);
        }
    }

    public bool CheckGPS()
    {
#if !UNITY_EDITOR
        try {
            using (AndroidJavaClass classPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                AndroidJavaObject objActivity = classPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                using (AndroidJavaObject jo = new AndroidJavaObject("kr.lithos.applications.brushmon.location_permission.VerifyLocation")) {
                    isCheckGPS = jo.CallStatic<bool>("CheckGPS", new object[] { objActivity });
                    LogManager.Log(":: Unity GPS:: GPS Checking");
                };
            };
        } catch (Exception e) {
            LogManager.LogError("GPS Checking Exception : " + e.Message);
        }
        LogManager.Log(":: Unity GPS :: isCheckGPS = " + isCheckGPS);
#endif
        return isCheckGPS;
    }
}
