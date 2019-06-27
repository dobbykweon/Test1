using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class BrushingController : MonoBehaviour {

	private UserVo userVo;
	void Start () {
		// BrushFSM.Instance.Initailize ();
		// BLEManager.Instance.Initailize ();
		userVo = ConfigurationData.Instance.GetValueFromJson<UserVo> ("User");
	}

	public void onBackPress () {
		BrushFSM.Instance.onBackPress ();
	}

	private int skipCount = 0;
	public void skip () {
		if (userVo != null && userVo.user_id.IndexOf ("temp@kittenpla.net") > -1) {
			if (skipCount == 0) {
				StartCoroutine (coSkip ());
			}
			skipCount++;
			if (skipCount > 3) {
				BrushFSM.Instance.FinishBrusing ();
			}
		}
	}

	IEnumerator coSkip () {
		yield return new WaitForSecondsRealtime (1f);
		skipCount = 0;
	}

}