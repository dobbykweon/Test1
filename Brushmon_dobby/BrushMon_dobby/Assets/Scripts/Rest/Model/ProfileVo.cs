using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]  
public class ProfileVo
{
    public long profile_idx;
    public string name;
    public string character_name;
    public bool terminated;
    public int birth;
    public string hand_type;
}