using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomEyeTrackingTarget : MonoBehaviour, IMixedRealityFocusHandler {
	public void OnFocusEnter(FocusEventData eventData) {
		// preverimo, ali je  event sprozil EyeGaze
		if (eventData.Pointer.InputSourceParent.SourceType == InputSourceType.Eyes) {
			Debug.Log("Looking at it");
		}
	}

	public void OnFocusExit(FocusEventData eventData) {
		// preverimo, ali je  event sprozil EyeGaze
		if (eventData.Pointer.InputSourceParent.SourceType == InputSourceType.Eyes) {
			Debug.Log("Stopped Looking");
		}
	}
}
