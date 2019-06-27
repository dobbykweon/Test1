using System.Collections.Generic;
using UnityEngine;

public class JsonHelper {
    //Usage:
    //YouObject[] objects = JsonHelper.getJsonArray<YouObject> (jsonString);
    public static List<T> getJsonArray<T> (string json) {
        string newJson = "{ \"array\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>> (newJson);
        return wrapper.array;
    }

    //Usage:
    //string jsonString = JsonHelper.arrayToJson<YouObject>(objects);
    public static string arrayToJson<T> (List<T> array) {
        Wrapper<T> wrapper = new Wrapper<T> ();
        wrapper.array = array;
        return JsonUtility.ToJson (wrapper);
    }

    public static string ListToJson<T>(List<T> list)
    {
        string jsonData = "[";

        for (int i = 0; i < list.Count; i++)
        {
            if (i > 0) jsonData += ",";

            jsonData += JsonUtility.ToJson(list[i]);
        }

        jsonData += "]";

        return jsonData;
    }

    [System.Serializable]
    private class Wrapper<T> {
        public List<T> array = new List<T>();
    }
}