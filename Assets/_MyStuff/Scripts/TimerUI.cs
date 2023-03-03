using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour {
	// koliko sekund ze drzimo atom
	private float currentHoldTime = 0f;
	// koliko sekund rabimo drzati atom, da se timer v celoti napolni
	public float maxHoldTIme = 3f;

	// Timer iz scene, dodamo ga v editorju
	public Image timer = null;

	private void OnEnable() {
		// ko se skripto ponovno aktiviramo (iz AtomiInterakcija.cs) moramo resetirati currentHoldTIme
		currentHoldTime = 0f;
	}

	private void Update() {
		currentHoldTime += Time.deltaTime;
		timer.fillAmount = currentHoldTime / maxHoldTIme;
	}
}
