using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BrushMonSceneManager : GlobalMonoSingleton<BrushMonSceneManager>{

	public string BeforeScene;
	public void LoadScene(string name, object transitionData = null){
        LogManager.Log("LoadScene : " + name);
        StartCoroutine(LoadScene(name));
	}

	IEnumerator LoadScene(string name){
		yield return new WaitForEndOfFrame();
		BeforeScene = SceneManager.GetActiveScene().name;
		SceneDTO.Instance.SetValue ("FromScene", BeforeScene);
        BMUtil.Instance.ClearDialog();
        SceneManager.LoadScene(name);
	}



	public void LoadSceneDelay(string name, object transitionData = null){
		StartCoroutine(LoadSceneDelay(name));
	}

	IEnumerator LoadSceneDelay(string name){
		yield return new WaitForSeconds (1.5f);
		BeforeScene = SceneManager.GetActiveScene().name;
		SceneDTO.Instance.SetValue ("FromScene", BeforeScene);
		SceneManager.LoadScene (name);
		StopAllCoroutines();
	}
	
}
