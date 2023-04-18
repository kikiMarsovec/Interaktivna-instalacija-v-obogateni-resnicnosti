using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class AtomPodatki : MonoBehaviour {
	public string emso = "";

	[SerializeField]
	private ToolTip toolTip;

	public string pridobiIme() {
		string parentName = gameObject.transform.parent.transform.parent.name;
		// Od  parentName (e.g. Carbon_mesh)  odstranimo zadnjih 5 char-ov (_mesh)
		return parentName.Substring(0, parentName.Length - 5);
	}

	public string pridobiSimbol() {
		switch (pridobiIme()) {
			case "Carbon":
				return "C";
			case "Holmium":
				return "Ho";
			case "Hydrogen":
				return "H";
			case "Nitrogen":
				return "N";
			case "Oxygen":
				return "O";
			case "Phosphorus":
				return "P";
			default:
				return null;
		}
	}

	public string pridobiStatus() {
		if (emso.Length > 0) {
			return "Taken";
		}
		return "Available";
	}

	public bool niZaseden() {
		if (emso.Length > 0) {
			return false;
		}
		return true;
	}

	public void UpdateToolTipText(string newText) { // Posodobimo tekst v Tooltip-u tega atoma
		toolTip.ToolTipText = newText;
	}
}
