using UnityEngine;


public class GlobalMonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null;
    public static T Instance
    {
        get
        {
            _instance = FindObjectOfType(typeof(T)) as T;
            if (_instance == null)
            {
                _instance = new GameObject("@" + typeof(T).ToString(), typeof(T)).GetComponent<T>();
                DontDestroyOnLoad(_instance);
            }
            return _instance;
        }
    }
}
