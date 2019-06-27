using UnityEngine;

public class LogManager : MonoBehaviour
{
	public static void Log(object msg)
	{
		if(IsEnable()) Debug.Log(msg);
	}

	public static void LogError(object msg)
	{
		Debug.LogError(msg);
	}

	public static void LogWarning(object msg)
	{
        if (IsEnable()) Debug.LogWarning("Warning : " + msg);
	}
    
	static bool IsEnable()
    {
#if (UNITY_EDITOR)
		return true;
#else
        return false;
#endif
    }
}
