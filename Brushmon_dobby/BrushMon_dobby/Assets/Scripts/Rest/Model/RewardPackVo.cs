using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RewardPackVo {
    //public int priority;
    //public long package_idx;
    public string package_name;
    public string package_color;
    //public int group_idx;
    
    [SerializeField]
    public List<RewardStickerVo> reward_list;

    //public DateTime local_closing_date { 
    //    get
    //    { 
    //        var posixTime = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);
    //        var time = posixTime.AddMilliseconds(closing_date);
    //        return time.ToLocalTime();
    //    } 
    //}
    //public long closing_date;

}