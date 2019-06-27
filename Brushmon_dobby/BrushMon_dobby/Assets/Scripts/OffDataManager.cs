using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Sticker
{
    cheese,
    cherry,
    soda
}

public class OffDataManager : GlobalMonoSingleton<OffDataManager>
{
    bool isLog = true;

    int[] idxlist = { 211, 212, 213, 214, 215, 216, 217, 176, 177, 178, 179, 180, 181, 182, 120, 121, 122, 123, 124, 125, 126 };

    public void CheckConnectServer(Action<bool> result)
    {
        result(true);
    }

    public void SignUp(SignVo signVo, Action<string> result)
    {
        if (isLog) LogManager.Log("OFF_SignUp");

        //UserVo.user_id = signVo.email;
        //UserVo = UserVo;

        if (CheckData("TokenVo") == true)
        {
            result(LoadData("TokenVo"));
        }
        else
        {
            string data = JsonUtility.ToJson(Token);
            result(data);
        }
    }

    public void SignIn(SignVo signVo, Action<string> result)
    {
        if (isLog) LogManager.Log("OFF_SignIn");

        if (CheckData("TokenVo") == true)
        {
            result(LoadData("TokenVo"));
        }
        else
        {
            string data = JsonUtility.ToJson(Token);
            result(data);
        }
    }

    public void RefreshToken(Action<string> result)
    {
        if (isLog) LogManager.Log("OFF_RefreshToken");

        if (CheckData("UserVo") == true)
        {
            result(LoadData("UserVo"));
        }
        else
        {
            string data = JsonUtility.ToJson(UserVo);
            result(data);
        }
    }

    public void GetUser(Action<string> result)
    {
        if (isLog) LogManager.Log("OFF_GetUser");

        if (CheckData("UserVo") == true)
        {
            result(LoadData("UserVo"));
        }
        else
        {
            string data = JsonUtility.ToJson(UserVo);
            result(data);
        }
    }

    public void GetProfileList(Action<string> result)
    {
        if (isLog) LogManager.Log("OFF_GetProfileList");

        if (CheckData("ProfileList") == true)
        {
            result(LoadData("ProfileList"));
        }
        else
        {
            string data = JsonHelper.ListToJson<ProfileVo>(ProfileList);
            result(data);
        }
    }

    public void RegisterProfile(ProfileVo profileVo, Action<long> result)
    {
        if (isLog) LogManager.Log("OFF_RegisterProfile : "+ ProfileList.Count);

        if (ProfileList.Count == 0)
            profileVo.profile_idx = 0;
        else
            profileVo.profile_idx = ProfileList[ProfileList.Count - 1].profile_idx + 1;

        AddProfileVo(profileVo);
        result(profileVo.profile_idx);
    }

    public void Tracking(TrackingVo trackingVo)
    {
    }

    public void UpdateProfile(ProfileVo profileVo, Action result)
    {
        if (isLog) LogManager.Log("OFF_UpdateProfile");

        for(int i = 0; i < ProfileList.Count; i++)
        {
            if(ProfileList[i].profile_idx == profileVo.profile_idx)
            {
                ProfileList[i] = profileVo;
                SaveData("ProfileList", JsonHelper.ListToJson<ProfileVo>(ProfileList));
                break;
            }
        }

        
        result();
    }

    public void UpdateUserSetting(UserVo userVo, Action result)
    {
        if (isLog) LogManager.Log("OFF_UpdateUserSetting");

        UserVo = userVo;
        result();
    }

    public void GetProfile(long profile_idx, Action<string> result)
    {
        if (isLog) LogManager.Log("OFF_GetProfile");

        for (int i = 0; i < ProfileList.Count; i++)
        {
            if (ProfileList[i].profile_idx == profile_idx)
            {
                result(JsonUtility.ToJson(ProfileList[i]));
                break;
            }
        }
    }

    public void DeleteProfile(long profile_idx, Action result)
    {
        if (isLog) LogManager.Log("OFF_DeleteProfile");

        for (int i = 0; i < ProfileList.Count; i++)
        {
            if(ProfileList[i].profile_idx == profile_idx)
            {
                ProfileList.RemoveAt(i);
                SaveData("ProfileList", JsonHelper.ListToJson<ProfileVo>(ProfileList));
                result();
                break;
            }
        }
    }

    public void GetLatestBrushResult(long profile_idx, Action<string> result)
    {
        if (isLog) LogManager.Log("OFF_GetLatestBrushResult");

        result(LoadData("RewardResult"));
    }

    public void GetBrushResult(long profile_idx, Action<string> result)
    {
        if (isLog) LogManager.Log("OFF_GetBrushResult");

        LogManager.LogError("GetBrushResult No DATA!!!!!!");
    }

    public void GetBrushResult(long profile_idx, int limit, int offset, bool use_brush_only, Action<string> result)
    {
        if (isLog) LogManager.Log("OFF_GetBrushResult");
        result(JsonHelper.ListToJson<LatestRewardResultVo>(ListRewardResult));
    }

    public void GetPreSticker(long profile_idx, Action<string> result)
    {
        if (isLog) LogManager.Log("OFF_GetPreSticker");

        result(JsonUtility.ToJson(PreSticker));
    }

    public void GetRewardList(long profile_idx, Action<string> result)
    {
        if (isLog) LogManager.Log("OFF_GetRewardList");

        BMUtil.Instance.LoadTestStickData(result, "Reward01.json");
    }

    public void RegisterBrushingData(BrushingResultDTO brushingResultDTO, Action result)
    {
        if (isLog) LogManager.Log("OFF_RegisterBrushingData : "+JsonUtility.ToJson(brushingResultDTO));

        LatestRewardResultVo resultVo = new LatestRewardResultVo();

        resultVo.idx = (LatestRewardResultVo == null) ? 0 : LatestRewardResultVo.idx + 1;
        resultVo.profile_idx = brushingResultDTO.profile_idx;
        resultVo.sticker_pack_idx = 0;
        resultVo.sticker_pack_name = "CHEESE";
        resultVo.result_list = brushingResultDTO.result_list;
        resultVo.create_date = BMUtil.Instance.GetGMTinMs(DateTime.Now);
        resultVo.priority = "";
        resultVo.is_seasonal = "";
        resultVo.use_brush = brushingResultDTO.use_brush;
#if UNITY_IOS
        brushingResultDTO.os = "IOS";
#elif UNITY_ANDROID
        brushingResultDTO.os = "Android";
#endif
        SaveBrushResult(resultVo);

        result();
    }

    public void FindPasswordEmail(UserEmailVo email, Action result)
    {
        //LogManager.LogError("FindPasswordEmail No DATA!!!!!!");
    }

    public void RegisterFcmToken(FcmToken fcmToken, Action result)
    {
        //LogManager.LogError("RegisterFcmToken No DATA!!!!!!");
    }

    public void GetFcmTokenList(Action<string> result)
    {
        //LogManager.LogError("GetFcmTokenList No DATA!!!!!!");
    }

    public void GetEventList(Action<string> result)
    {
        //LogManager.LogError("GetEventList No DATA!!!!!!");
    }






    bool CheckData(string key)
    {
        return PlayerPrefs.HasKey("OFF_" + key);
    }

    void SaveData(string key, string value)
    {
        PlayerPrefs.SetString("OFF_" + key, value);
    }

    string LoadData(string key)
    {
        return PlayerPrefs.GetString("OFF_" + key);
    }

#region 데이터 관련

    TokenVo Token
    {
        set
        {
            SaveData("TokenVo", JsonUtility.ToJson(value));
        }
        get
        {
            TokenVo token;

            if (CheckData("TokenVo") == true)
            {
                token = JsonUtility.FromJson<TokenVo>(LoadData("TokenVo"));
            }
            else
            {
                token = new TokenVo();

                token.access_token = "access_token";
                token.refresh_token = "refresh_token";
                token.expire_time = BMUtil.Instance.GetGMTinMs(DateTime.Now);

                SaveData("TokenVo", JsonUtility.ToJson(token));
            }

            return token;
        }
    }

    UserVo userVo = null;
    UserVo UserVo
    {
        set
        {
            SaveData("UserVo", JsonUtility.ToJson(value));
        }
        get
        {
            if (userVo == null)
            {
                if (CheckData("UserVo") == true)
                {
                    userVo = JsonUtility.FromJson<UserVo>(LoadData("UserVo"));
                }
                else
                {
                    userVo = new UserVo();

                    userVo.idx = 901;
                    userVo.vibration = 4;
                    //userVo.admin = false;
                    userVo.user_id = "temp@kittenpla.net";
                    userVo.login_type = "email";
                    userVo.profile_count = 5;
                    userVo.paste_exist = false;
                    userVo.clean_up_exist = false;
                    userVo.has_brush = false;
                    userVo.agree_marketing = true;
                    userVo.agree_message = true;
                    userVo.device_country = "KR";
                    userVo.device_language = "KR";
                    userVo.app_language_text = "KR";
                    userVo.app_language_voice = "KR";
                    userVo.app_version = Application.version;
                    //userVo.create_date = 1559720647860;

                    SaveData("UserVo", JsonUtility.ToJson(userVo));
                }
            }
            return userVo;
        }
    }

    void AddProfileVo(ProfileVo value)
    {
        ProfileList.Add(value);
        SaveData("ProfileList", JsonHelper.ListToJson<ProfileVo>(ProfileList));
    }

    ProfileVo GetProfileVo(int idx)
    {
        return ProfileList[idx];
    }


    List<ProfileVo> profileVos = null;
    List<ProfileVo> ProfileList
    {
        get
        {
            if (profileVos == null)
            {
                if (CheckData("ProfileList") == true)
                {
                    profileVos = JsonHelper.getJsonArray<ProfileVo>(LoadData("ProfileList"));
                }
                else
                {
                    profileVos = new List<ProfileVo>();
                    SaveData("ProfileList", JsonHelper.ListToJson<ProfileVo>(profileVos));
                }
            }

            return profileVos;
        }
    }

    PreStickerVo PreSticker
    {
        get
        {
            PreStickerVo sticker = new PreStickerVo();

            //sticker.order = PlayerPrefs.GetInt("sticker_idx", 0);
            //sticker.idx = idxlist[sticker.order];

            //long charidx = (sticker.idx - 1) / 7 / 3 * 7 + (sticker.idx - 1) % 7 + 1;
            //CharacterType charType = (CharacterType)((sticker.idx - 1) / 7 % 3);

            //sticker.name = "sticker_" + charType.ToString() + "_" + charidx.ToString("00");

            sticker.idx = 2;
            sticker.name = "sticker_cheese_02";

            return sticker;
        }
    }

    List<LatestRewardResultVo> listRewardResult = null;
    List<LatestRewardResultVo> ListRewardResult
    {
        get
        {
            if (listRewardResult == null)
            {
                if (CheckData("ListRewardResult") == true)
                {
                    string data = LoadData("ListRewardResult");

                    listRewardResult = JsonHelper.getJsonArray<LatestRewardResultVo>(data);
                }
                else
                {
                    listRewardResult = new List<LatestRewardResultVo>();

                    for (int i = 0; i < 4; i++)
                    {
                        LatestRewardResultVo reward = new LatestRewardResultVo();

                        reward.idx = i;
                        reward.profile_idx = 0;
                        reward.sticker_pack_idx = 0;
                        reward.sticker_pack_name = "CHEESE";

                        reward.result_list = new List<string>();

                        for (int k = 0; k < 16; k++)
                        {
                            if (i == 0 && k < 4)
                            {
                                reward.result_list.Add("BAD");
                            }
                            else if (i == 1 && k < 8)
                            {
                                reward.result_list.Add("BAD");
                            }
                            else if (i == 2 && k < 12)
                            {
                                reward.result_list.Add("BAD");
                            }
                            else if (i == 3 && k < 16)
                            {
                                reward.result_list.Add("BAD");
                            }
                            else
                            {
                                reward.result_list.Add("GOOD");
                            }
                        }

                        reward.create_date = BMUtil.Instance.GetGMTinMs(DateTime.Now.AddDays(-10 + i));
                        reward.priority = "";
                        reward.is_seasonal = "";
                        reward.use_brush = true;
                        reward.os = SystemInfo.operatingSystem;

                        listRewardResult.Add(reward);
                    }

                    SaveData("ListRewardResult", JsonHelper.ListToJson<LatestRewardResultVo>(ListRewardResult));
                }
            }

            return listRewardResult;
        }
    }

    LatestRewardResultVo LatestRewardResultVo
    {
        get
        {
            if (CheckData("RewardResult"))
            {
                return JsonUtility.FromJson<LatestRewardResultVo>(LoadData("RewardResult"));
            }
            else
            {
                return null;
            }
        }
    }

    void SaveBrushResult(LatestRewardResultVo result)
    {
        while (ListRewardResult.Count > 4)
            ListRewardResult.RemoveAt(ListRewardResult.Count - 1);

        ListRewardResult.Add(result);

        SaveData("ListRewardResult", JsonHelper.ListToJson<LatestRewardResultVo>(ListRewardResult));
        SaveData("RewardResult", JsonUtility.ToJson(result));

        LogManager.Log("?????_result : "+ JsonUtility.ToJson(result));
    }

    #endregion

}
