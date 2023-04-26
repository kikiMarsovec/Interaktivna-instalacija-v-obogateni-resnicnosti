using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomEyeTrackingTarget : MonoBehaviour, IMixedRealityFocusHandler {

	private ToolTipPrikazovanje toolTipPrikazovanje;

	private void OnEnable() {
		toolTipPrikazovanje = gameObject.GetComponent<ToolTipPrikazovanje>();
	}

	public void OnFocusEnter(FocusEventData eventData) {
		// preverimo, ali je  event sprozil EyeGaze
		// if (eventData.Pointer.InputSourceParent.SourceType == InputSourceType.Eyes) {
		//	toolTipPrikazovanje.ShowToolTip();
		// }

		// ne glede na to ali je focusEvent sprozil EyeGaze  ali handPoiinter bomo prikazali tooltip
		toolTipPrikazovanje.ShowToolTip();
	}

	public void OnFocusExit(FocusEventData eventData) {
		// // preverimo, ali je  event sprozil EyeGaze
		// if (eventData.Pointer.InputSourceParent.SourceType == InputSourceType.Eyes) {
		// 	Debug.Log("Stopped Looking"); // TODO DELETE
		// 	toolTipPrikazovanje.HideToolTipWithDelay();
		// }

		// skrijemo tooltip ker izgubimo fokus
		toolTipPrikazovanje.HideToolTipWithDelay();
	}
}
