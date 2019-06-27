using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneDTO : GlobalMonoSingleton<SceneDTO>
{

	private Dictionary<string, string> data;
	
    void Awake()
	{
		if(data == null)
			data = new Dictionary<string, string>();
	}
	void Start()
	{
		if(data == null)
			data = new Dictionary<string, string>();
	}
	public string GetValue(string key) 
    {
		if(data.ContainsKey(key))
		{
			string returnValue = data[key].ToString();
			data.Clear();
			return returnValue;
		} else {
			return "";
		}
    }
	public string GetValueKeep(string key) 
    {
		if(data.ContainsKey(key)){
			return data[key];
		} else {
			return "";
		}
    }
	public void SetValue(string key, string value)
	{
		data[key] = value;
	}
}
