using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDisableEyeTrackingTarget : MonoBehaviour, IMixedRealityFocusHandler {

	private EyeTrackingTarget eyeTrackingTarget;
	private ToolTipPrikazovanje toolTipPrikazovanje;

	private void OnEnable() {
		eyeTrackingTarget = gameObject.GetComponent<EyeTrackingTarget>();
		toolTipPrikazovanje = gameObject.GetComponent<ToolTipPrikazovanje>();
	}

	public void OnFocusEnter(FocusEventData eventData) {
		// Ko je EyeTrackingTarget vklopljen, ne moremo interaktirati z atomom.
		// ko je atom v fokusu preverimo, kaj je sprozilo fokus. Ce je fokus sprozil HandPointer, potem izklopimo EyeTrackingTarget.
		if (eventData.Pointer.InputSourceParent.SourceType == InputSourceType.Hand) {
			Debug.Log("Focus Enter");
			eyeTrackingTarget.enabled = false;
			toolTipPrikazovanje.HideToolTipWithDelay();
		}
	}

	public void OnFocusExit(FocusEventData eventData) {
		// Ko atom izgubi fokus, vklopimo EyeTrackingTarget nazaj.
		Debug.Log("Focus Exit");
		eyeTrackingTarget.enabled = true;
	}
}
