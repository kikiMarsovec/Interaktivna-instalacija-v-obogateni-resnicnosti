using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleHydrogen : MonoBehaviour {
	public void toggleAtoms() {
		if (gameObject.activeSelf) {
			gameObject.SetActive(false);
		}
		else { 
			gameObject.SetActive(true);
		}
	}
}
