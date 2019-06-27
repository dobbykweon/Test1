using System;

[Serializable]
public class FcmTokenVo
{
    public long date;
    public string user_id;
    public long idx;
    public string token;

    public DateTime RegDateTime
    {
        get
        {
            var posixTime = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);
            var time = posixTime.AddMilliseconds(date);
            return time.ToLocalTime();
        }
    }
}
