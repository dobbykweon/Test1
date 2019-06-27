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
    
    //����Ʈĩ�� ��� ����
    public bool has_brush;
    //���������� ���ŵ���
    public bool agree_marketing;
    //�޽��� ���� ����
    public bool agree_message;
    //�ܸ� ���� ����
    public string device_country;
    //�ܸ� ���� ���
    public string device_language;
    //�� ���� ���(����)
    public string app_language_text;
    //�� ���� ���(����)
    public string app_language_voice;
    //�� ����
    public string app_version;
}