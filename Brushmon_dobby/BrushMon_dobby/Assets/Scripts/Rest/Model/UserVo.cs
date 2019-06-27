using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]  
public class UserVo
{
    public long idx;
    public int vibration;
    public string user_id;
    public string login_type;
    public int profile_count;
    public bool paste_exist;
    public bool clean_up_exist;
    
    //스마트칫솔 사용 유무
    public bool has_brush;
    //마케팅정보 수신동의
    public bool agree_marketing;
    //메시지 수신 동의
    public bool agree_message;
    //단말 설정 국가
    public string device_country;
    //단말 설정 언어
    public string device_language;
    //앱 설정 언어(문자)
    public string app_language_text;
    //앱 설정 언어(음성)
    public string app_language_voice;
    //앱 버전
    public string app_version;
}