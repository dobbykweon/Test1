using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour {

	public string key;

	IEnumerator Start(){
		while (!LocalizationManager.Instance.GetIsReady ()) 
        {
            yield return null;
        }
		Text text = GetComponent<Text> ();
		text.text = LocalizationManager.Instance.GetLocalizedValue (key);
	}

	public void ReLoadText()
	{
		StartCoroutine(Start());
	}
}
