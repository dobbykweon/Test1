using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionData : GlobalMonoSingleton<SceneTransitionData>
{

    private Dictionary<string, object> data;
	public T GetValueOneTime<T>(string key, object defaultValue) 
    {
		ifNullNew();
		if(data.ContainsKey(key)){
			T obj = default(T);
        	obj =Activator.CreateInstance<T>();
			obj = (T)data[key];
			Clear();
			return obj;
		}else {
			return (T)defaultValue;
		}
    }
	public T GetValueKeep<T>(string key, object defaultValue) 
    {
		ifNullNew();
		if(data.ContainsKey(key)){
			return (T)data[key];
		}else {
			return (T)defaultValue;
		}
    }
	public void SetValue<T>(string key, object value)
	{
		initialize();
        data.Add(key, value);
	}

	void ifNullNew(){
		if(data == null)
			data = new Dictionary<string, object>();
	}

	void initialize(){
		ifNullNew();
		Clear();
	}

	public void Clear(){
		data.Clear();
	}
}
