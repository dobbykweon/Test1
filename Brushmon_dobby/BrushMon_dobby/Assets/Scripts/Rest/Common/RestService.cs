using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class RestService
{

#if HANWHA
    //private static string ApiPath = @"https://dev-api.brushmon.com";
    private static string ApiPath = @"https://api.brushmon.com";
#else
    private static string ApiPath = @"https://dev-api.brushmon.com";
    //private static string ApiPath = @"https://api.brushmon.com";
#endif

    public static string ServiceUrl { get { return ApiPath; } }
    public static RestService Instance
    {
        get { return new RestService(); }
    }

    public void CheckConnectServer(Action<bool> result)
    {
#if OFFLINE
        OffDataManager.Instance.CheckConnectServer(result);
#else
        try
        {
            RestBase restBase = new RestBase();
            restBase.CheckConnectServer(
                isConnect => { result(isConnect); },
                exception =>
                {
                    LogManager.Log("CheckConnectServer : " + exception.Message);
                    result(false);
                });
        }
        catch (Exception e)
        {
            LogManager.LogError(e.Message);
            result(false);
        }
#endif
    }

    public void SignUp(SignVo signVo, Action<string> result, Action<Exception> exception)
    {
#if OFFLINE
        OffDataManager.Instance.SignUp(signVo, result);
#else
        IHttpContent requestData = makeRequstData(signVo);
        RestBase restBase = new RestBase();
        restBase.Post(new Uri(ApiPath + "/sign/up/email"), requestData, (response) =>
        {
            if (response.Exception == null)
            {
                result(response.Data);
            }
            else
            {
                exception(response.Exception);
            }
        });
#endif
    }

    public void SignIn(SignVo signVo, Action<string> result, Action<Exception> exception)
    {
#if OFFLINE
        OffDataManager.Instance.SignIn(signVo, result);
#else
        IHttpContent requestData = makeRequstData(signVo);
        RestBase restBase = new RestBase();
        restBase.Post(new Uri(ApiPath + "/sign/email"), requestData, (response) =>
        {
            if (response.Exception == null)
            {
                result(response.Data);
            }
            else
            {
                LogManager.Log(response.Exception.Message);
                exception(response.Exception);
            }
        });
#endif
    }

    public void RefreshToken(TokenVo tokenVo, Action<string> result, Action<Exception> exception)
    {
#if OFFLINE
        OffDataManager.Instance.RefreshToken(result);
#else
        IHttpContent requestData = makeRequstData(tokenVo);
        RestBase restBase = new RestBase();
        restBase.Post(new Uri(ApiPath + "/sign/refresh"), requestData, (response) =>
        {
            if (response.Exception == null)
            {
                result(response.Data);
            }
            else
            {
                LogManager.Log(response.Exception.Message);
                exception(response.Exception);
            }
        });
#endif
    }
    public void GetUser(TokenVo token, Action<string> result, Action<Exception> exception)
    {
#if OFFLINE
        OffDataManager.Instance.GetUser(result);
#else
        RestBase restBase = new RestBase();
        restBase.currentTokenVo = token;
        restBase.Get(new Uri(ApiPath + "/user/me"), (response) =>
        {
            if (response.Exception == null)
            {
                LogManager.Log("/user/me : " + response.Data);

                result(response.Data);
            }
            else
            {
                LogManager.Log(response.Exception.Message);
                exception(response.Exception);
            }
        });
#endif
    }

    public void GetProfileList(TokenVo token, Action<string> result, Action<Exception> exception)
    {
#if OFFLINE
        OffDataManager.Instance.GetProfileList(result);
#else
        RestBase restBase = new RestBase();
        restBase.currentTokenVo = token;
        restBase.Get(new Uri(ApiPath + "/user/profile"), (response) =>
        {
            if (response.Exception == null)
            {
                result(response.Data);
            }
            else
            {
                exception(response.Exception);
            }
        });
#endif
    }

    public void RegisterProfile(TokenVo token, ProfileVo profileVo, Action<long> result, Action<Exception> exception)
    {
#if OFFLINE
        OffDataManager.Instance.RegisterProfile(profileVo, result);
#else
        IHttpContent requestData = makeRequstData(profileVo);
        RestBase restBase = new RestBase();
        restBase.currentTokenVo = token;
        restBase.Post(new Uri(ApiPath + "/user/profile"), requestData, (response) =>
        {
            if (response.Exception == null)
            {
                ProfileIdx data = JsonUtility.FromJson<ProfileIdx>(response.Data);
                result(data.profile_idx);
            }
            else
            {
                LogManager.Log(response.Exception);
                exception(response.Exception);
            }
        });
#endif
    }

    public void Tracking(TokenVo token, TrackingVo trackingVo)
    {
#if OFFLINE

#else
        IHttpContent requestData = makeRequstData(trackingVo);
        RestBase restBase = new RestBase();
        restBase.currentTokenVo = token;
        restBase.Post(new Uri(ApiPath + "/user/action"), requestData, (response) =>
        {
            if (response.Exception == null)
            {
                //LogManager.Log("trackingVo : " + trackingVo.action + "/" + trackingVo.description);
            }
            else
            {
                LogManager.Log(response.Exception);
            }
            BMUtil.Instance.CloseLoading();
        });
#endif
    }

    public void UpdateProfile(TokenVo token, ProfileVo profileVo, Action result, Action<Exception> exception)
    {
#if OFFLINE
        OffDataManager.Instance.UpdateProfile(profileVo, result);
#else
        IHttpContent requestData = makeRequstData(profileVo);
        RestBase restBase = new RestBase();
        restBase.currentTokenVo = token;
        restBase.Patch(new Uri(ApiPath + "/user/profile/" + profileVo.profile_idx), requestData, (response) =>
        {
            if (response.Exception == null)
            {
                result();
            }
            else
            {
                exception(response.Exception);
            }
        });
#endif
    }

    public void UpdateUserSetting(TokenVo token, UserVo userVo, Action result, Action<Exception> exception)
    {
#if OFFLINE
        OffDataManager.Instance.UpdateUserSetting(userVo, result);
#else
        IHttpContent requestData = makeRequstData(userVo);
        RestBase restBase = new RestBase();
        restBase.currentTokenVo = token;
        LogManager.Log("token: " + token.access_token);
        restBase.Patch(new Uri(ApiPath + "/user/setting"), requestData, (response) =>
        {
            if (response.Exception == null)
            {
                result();
            }
            else
            {
                exception(response.Exception);
            }
        });
#endif
    }

    public void GetProfile(TokenVo token, long profile_idx, Action<string> result, Action<Exception> exception)
    {
#if OFFLINE
        OffDataManager.Instance.GetProfile(profile_idx, result);
#else
        RestBase restBase = new RestBase();
        restBase.currentTokenVo = token;
        restBase.Get(new Uri(ApiPath + "/user/profile/" + profile_idx), (response) =>
        {
            if (response.Exception == null)
            {
                LogManager.Log(response.Data);
                result(response.Data);
            }
            else
            {
                exception(response.Exception);
            }
        });
#endif
    }

    public void DeleteProfile(TokenVo token, long profile_idx, Action result, Action<Exception> exception)
    {
#if OFFLINE
        OffDataManager.Instance.DeleteProfile(profile_idx, result);
#else
        RestBase restBase = new RestBase();
        restBase.currentTokenVo = token;
        restBase.Delete(new Uri(ApiPath + "/user/profile/" + profile_idx), (response) =>
        {
            if (response.Exception == null)
            {
                LogManager.Log(response.Data);
                result();
            }
            else
            {
                exception(response.Exception);
            }
        });
#endif
    }

    public void GetLatestBrushResult(TokenVo token, long profile_idx, Action<string> result, Action<Exception> exception)
    {
#if OFFLINE
        OffDataManager.Instance.GetLatestBrushResult(profile_idx, result);
#else
        RestBase restBase = new RestBase();
        restBase.currentTokenVo = token;
        restBase.Get(new Uri(ApiPath + "/brush/detail/latest?profile_idx=" + profile_idx), (response) => {
            if (response.Exception == null)
            {
                LogManager.Log(response.Data);
                result(response.Data);
            }
            else
            {
                exception(response.Exception);
            }
        });
#endif
    }

    public void GetBrushResult(TokenVo token, long profile_idx, Action<string> result, Action<Exception> exception)
    {
#if OFFLINE
        OffDataManager.Instance.GetBrushResult(profile_idx, result);
#else
        RestBase restBase = new RestBase();
        restBase.currentTokenVo = token;
        restBase.Get(new Uri(ApiPath + "/brush/detail/all?profile_idx=" + profile_idx), (response) => {
            if (response.Exception == null)
            {
                LogManager.Log(response.Data);
                result(response.Data);
            }
            else
            {
                exception(response.Exception);
            }
        });
#endif
    }

    public void GetBrushResult(TokenVo token, long profile_idx, int limit, int offset, bool use_brush_only, Action<string> result, Action<Exception> exception)
    {
#if OFFLINE
        OffDataManager.Instance.GetBrushResult(profile_idx, limit, offset, use_brush_only, result);
#else
        string value = "profile_idx=" + profile_idx + "&limit=" + limit + "&offset=" + offset + "&use_brush_only=" + use_brush_only;

        RestBase restBase = new RestBase();
        restBase.currentTokenVo = token;

        restBase.Get(new Uri(ApiPath + "/brush/detail/all?" + value), (response) => {
            if (response.Exception == null)
            {
                LogManager.Log(response.Data);
                result(response.Data);
            }
            else
            {
                exception(response.Exception);
            }
        });
#endif
    }

    public void GetPreSticker(TokenVo token, long profile_idx, Action<string> result, Action<Exception> exception)
    {
#if OFFLINE
        OffDataManager.Instance.GetPreSticker(profile_idx, result);
#else
        RestBase restBase = new RestBase();
        restBase.currentTokenVo = token;
        LogManager.Log("token: " + restBase.currentTokenVo);
        LogManager.Log("uri : " + ApiPath + "/sticker/next?profile_idx=" + profile_idx);
        restBase.Get(new Uri(ApiPath + "/sticker/next?profile_idx=" + profile_idx), (response) =>
        {
            if (response.Exception == null)
            {
                LogManager.Log(response.Data);
                result(response.Data);
            }
            else
            {
                exception(response.Exception);
            }
        });
#endif
    }
    public void GetRewardList(TokenVo token, long profile_idx, Action<string> result, Action<Exception> exception)
    {
#if OFFLINE
        OffDataManager.Instance.GetRewardList(profile_idx, result);
#else
        RestBase restBase = new RestBase();
        restBase.currentTokenVo = token;
        restBase.Get(new Uri(ApiPath + "/sticker/pack/mine?profile_idx=" + profile_idx), (response) =>
        {
            if (response.Exception == null)
            {
                LogManager.Log(response.Data);
                result(response.Data);
            }
            else
            {
                exception(response.Exception);
            }
        });
#endif
    }
    public void RegisterBrushingData(TokenVo token, BrushingResultDTO brushingResultDTO, Action result, Action<Exception> exception)
    {
#if OFFLINE
        OffDataManager.Instance.RegisterBrushingData(brushingResultDTO, result);
#else
        IHttpContent requestData = makeRequstData(brushingResultDTO);
        RestBase restBase = new RestBase();
        restBase.currentTokenVo = token;
        restBase.Post(new Uri(ApiPath + "/brush"), requestData, (response) =>
        {
            if (response.Exception == null)
            {
                result();
            }
            else
            {
                LogManager.Log(response.Exception);
                exception(response.Exception);
            }
        });
#endif
    }

    public void FindPasswordEmail(UserEmailVo email, Action result, Action<Exception> exception)
    {
#if OFFLINE
        OffDataManager.Instance.FindPasswordEmail(email, result);
#else
        IHttpContent requestData = makeRequstData(email);
        RestBase restBase = new RestBase();
        restBase.Post(new Uri(ApiPath + "/sign/refresh/account/mail"), requestData, (response) =>
        {
            if (response.Exception == null)
            {
                result();
            }
            else
            {
                LogManager.Log(response.Exception);
                exception(response.Exception);
            }
        });
#endif
    }

    public void RegisterFcmToken(TokenVo token, FcmToken fcmToken, Action result, Action<Exception> exception)
    {
#if OFFLINE
        OffDataManager.Instance.RegisterFcmToken(fcmToken, result);
#else
        LogManager.Log("RegisterFcmToken : " + fcmToken.fcm_token);

        IHttpContent requestData = makeRequstData(fcmToken);
        RestBase restBase = new RestBase();
        restBase.currentTokenVo = token;
        restBase.Post(new Uri(ApiPath + "/user/fcm-token"), requestData, (response) =>
        {
            if (response.Exception == null)
            {
                LogManager.Log("RegisterFcmToken response : " + response.Data);
                result();
            }
            else
            {
                LogManager.Log(response.Exception);
                exception(response.Exception);
            }
        });
#endif
    }

    public void GetFcmTokenList(TokenVo token, Action<string> result, Action<Exception> exception)
    {
#if OFFLINE
        OffDataManager.Instance.GetFcmTokenList(result);
#else
        RestBase restBase = new RestBase();
        restBase.currentTokenVo = token;
        restBase.Get(new Uri(ApiPath + "/user/fcm-token"), (response) =>
        {
            if (response.Exception == null)
            {
                LogManager.Log("GetFcmToken : " + response.Data);
                result(response.Data);
            }
            else
            {
                exception(response.Exception);
            }
        });
#endif
    }

    public void GetEventList(TokenVo token, Action<string> result, Action<Exception> exception)
    {
#if OFFLINE
        OffDataManager.Instance.GetEventList(result);
#else
        RestBase restBase = new RestBase();
        restBase.currentTokenVo = token;
        restBase.Get(new Uri(ApiPath + "/event/all"), (response) =>
        {
            if (response.Exception == null)
            {
                result(response.Data);
            }
            else
            {
                exception(response.Exception);
            }
        });
#endif
    }

    private IHttpContent makeRequstData(object obj)
    {
        string json = JsonUtility.ToJson(obj);
        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
        return content;
    }
}

