using UnityEngine;
using System.Collections;
using System;


public class CommonUtil
{
    public static int TrueFalseBoolToZeroOne(bool value)
    {
        if (value)
        {
            return 1;
        }
        return 0;
    }

    public static bool ZeroOneToTrueFalseBool(int value)
    {
        if (value == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static long GetGMTInMS () {
        var unixTime = DateTime.Now.ToUniversalTime () - new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        LogManager.Log("Now : " + DateTime.Now.ToUniversalTime().ToLongDateString());
        LogManager.Log("UTC : " + (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).ToLongDateString());

        return (long) unixTime.TotalMilliseconds;
    }
}
