using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RewardStickerVo {
    //public long reward_idx;
    //public long profile_idx;
    public long sticker_idx;
    //public long sticker_pack_idx;
    public string sticker_name;
    public string accessory_name;
    public string accessory_type;
    public int brushing_count;
    //public DateTime local_create_date { 
    //    get
    //    { 
    //        var posixTime = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);
    //        var time = posixTime.AddMilliseconds(create_date);
    //        return time.ToLocalTime();
    //    } 
    //}
    //public long create_date;    
}