using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]  
public class TokenVo
{
    public string access_token;
    public string refresh_token;
    public double expire_time;

    public override string ToString(){
        return "access_token : "+access_token+" : "+"refresh_token : "+refresh_token+" : "+"expire_time : "+expire_time;
    }
}