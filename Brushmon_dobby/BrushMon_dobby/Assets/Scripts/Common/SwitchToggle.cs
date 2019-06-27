using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

public class SwitchToggle : MonoBehaviour {

	private SkeletonGraphic Switch;
	public bool isOn = false;

	[System.Serializable]
	public class OnValueChanged : UnityEvent<Boolean> { };
	public OnValueChanged onValueChanged;

	void Start () {
		Switch = GetComponentInChildren<SkeletonGraphic> ();
		Initailize ();
	}

	void Initailize () {
		if (isOn) {
			SwitchAnimation ("Idle_on");
		} else {
			SwitchAnimation ("Idle_off");
		}
	}

	public void Toggle () {
		if (isOn) {
			isOn = false;
			SwitchAnimation ("Idle_off");
		} else {
			isOn = true;
			SwitchAnimation ("Idle_on");
		}
	}

	public void SetToggle (bool isOn) {
		if (isOn) {
			isOn = false;
			SwitchAnimation ("Idle_on");
		} else {
			isOn = true;
			SwitchAnimation ("Idle_off");
		}
	}

	private void SwitchAnimation (string toggle) {

		isOn = "Idle_on".Equals (toggle);
		onValueChanged.Invoke (isOn);
		Switch.AnimationState.SetAnimation (0, toggle, false);
	}

}