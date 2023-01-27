using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtomiInterakcija : MonoBehaviour, IMixedRealityPointerHandler {

	// spremenljivka, ki nam pove, ali trenutno interaktiramo z atomom
	// (preprecimo, da bi istocasno iteraktirali z vec atomi hkrati, poleg tega pa jo potrebujemo za prozenje UI timerja - Zato je javna)
	public bool trenutnoInteraktiramo = false;
	// spremenljivka, ki belezi koliko casa interaktiramo z atomom
	// (z njo razlikujemo med klikom in drzanjem atoma)
	private float casInterakcije;

	// Ta metoda se poklice ko zacnemo interakcijo z nekim objektom
	void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData) {
		// Pridobimo GameObject s katerim interaktiramo in preverimo ali gre za atom
		GameObject trenutniGameObject = eventData.Pointer.Result.CurrentPointerTarget as GameObject;
		if (string.Equals(trenutniGameObject.tag, "Atom") && !trenutnoInteraktiramo) {
			trenutnoInteraktiramo = true;
			casInterakcije = Time.time;
		}
		else { // TODO DELETE
			Debug.Log(trenutniGameObject.name);
		}
	}

	// Ta metoda se poklice ko koncamo interakcijo z nekim objektom
	void IMixedRealityPointerHandler.OnPointerUp(MixedRealityPointerEventData eventData) {
		// Pridobimo GameObject s katerim interaktiramo in preverimo ali gre za atom
		GameObject trenutniGameObject = eventData.Pointer.Result.CurrentPointerTarget as GameObject;
		if (string.Equals(trenutniGameObject.tag, "Atom") && trenutnoInteraktiramo) {
			trenutnoInteraktiramo = false;
			casInterakcije = Time.time - casInterakcije;
			if (casInterakcije < 1f) {
				// Atom smo kliknili
				AtomKlik(trenutniGameObject);
			}
			else {
				// Atom smo drzali
				AtomDrzanje(trenutniGameObject);
			}
		}
	}

	void AtomKlik(GameObject atom) {
		// TODO
		Debug.Log("Klik: " + atom.name); // DELETE
		atom.GetComponent<Renderer>().material.color = Color.magenta; // DELETE
	}

	void AtomDrzanje(GameObject atom) {
		// TODO
		Debug.Log("Drzanje: " + atom.name); // DELETE
		atom.GetComponent<Renderer>().material.color = Color.yellow; // DELETE
	}


	// Teh metod ne bomo potrebovali, vseeno pa morajo biti implementirane
	void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData) { }
	void IMixedRealityPointerHandler.OnPointerDragged(MixedRealityPointerEventData eventData) { }

}
