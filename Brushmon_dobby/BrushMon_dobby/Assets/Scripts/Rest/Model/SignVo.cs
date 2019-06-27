using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SignVo
{
//#if HANWHA
//    public SignVo(string email, string password)
//    {
//        this.email = email;
//        this.password = password;
//    }

//    private string _email;
//    private string _password;

//    public string email
//    {
//        get { return Base64.base64Decode(Utility.MD5Manager.DESDecrypt(_email)); }
//        set { _email = Utility.MD5Manager.DESEncrypt(Base64.base64Encode(value)); }
//    }
//    public string password
//    {
//        get { return Base64.base64Decode(Utility.MD5Manager.DESDecrypt(_password)); }
//        set { _password = Utility.MD5Manager.DESEncrypt(Base64.base64Encode(value)); }
//    }

//#else
    public string email;
    public string password;

    public SignVo(string email, string password)
    {
        this.email = email;
        this.password = password;
    }

//#endif
    public SignVo() { }
    
    
}