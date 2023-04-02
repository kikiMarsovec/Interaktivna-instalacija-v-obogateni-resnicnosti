using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtomPodatki : MonoBehaviour {
	public string emso = "";

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
}
