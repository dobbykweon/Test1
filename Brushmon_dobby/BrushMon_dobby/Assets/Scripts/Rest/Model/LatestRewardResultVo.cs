using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LatestRewardResultVo {
    public long idx;
    public long profile_idx;
    public long sticker_pack_idx;
    public string sticker_pack_name;
    public List<string> result_list;
    public DateTime local_create_date { 
        get
        { 
            var posixTime = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);
            var time = posixTime.AddMilliseconds(create_date);
            return time.ToLocalTime();
        } 
    }
    public long create_date;
    public string priority;
    public string is_seasonal;

    public bool use_brush;
    public string os;
}