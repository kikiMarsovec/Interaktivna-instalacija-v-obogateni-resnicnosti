using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AtomiInterakcija : MonoBehaviour, IMixedRealityPointerHandler {

	// spremenljivka, ki nam pove, ali trenutno interaktiramo z atomom
	// (preprecimo, da bi istocasno iteraktirali z vec atomi hkrati, poleg tega pa jo potrebujemo za prozenje UI timerja - Zato je javna)
	public bool trenutnoInteraktiramo = false;
	// spremenljivka, ki nam pove s koliko rokami trenutno drzimo celotno nanocevko
	// (ko drzimo nanocevko z vsaj eno roko, izklopimo dolocene atome, za izboljsanje delovanja)
	private int stevecRok = 0;
	// spremenljivka, ki belezi koliko casa interaktiramo z atomom
	// (z njo razlikujemo med klikom in drzanjem atoma)
	private float casInterakcije;

	// spremenljivka, ki pove, koliko casa moramo drzati atom, da izvedemo AtomDrzanje interakcijo
	public float drzanjeAtomaCas = 3f;

	// sem smo dodali Timer (Image) v editorju
	public Image timer = null;



	// Ta metoda se poklice ko zacnemo interakcijo z nekim objektom
	void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData) {
		// Pridobimo GameObject s katerim interaktiramo in preverimo ali gre za atom
		GameObject trenutniGameObject = eventData.Pointer.Result.CurrentPointerTarget as GameObject;
		if (string.Equals(trenutniGameObject.tag, "Atom") && !trenutnoInteraktiramo) {
			trenutnoInteraktiramo = true;
			casInterakcije = Time.time;

			// aktiviramo skripto za UI Timer in nastavimo koliko casa potrebuje da pride naokrog
			timer.GetComponent<TimerUI>().enabled = true;
			timer.GetComponent<TimerUI>().maxHoldTIme = drzanjeAtomaCas;

		}
		else if (string.Equals(trenutniGameObject.tag, "Nanocev")) {
			stevecRok++;
			// Ce smo zaceli translirati, skalirati ali rotirati celotno Nanocev, izklopimo dolocene atome, da izboljsamo delovanje aplikacije
			gameObject.transform.GetChild(0).transform.Find("Hydrogen_mesh").gameObject.SetActive(false);
			//gameObject.transform.GetChild(0).transform.Find("Carbon_mesh").gameObject.SetActive(false);
		}
	}

	// Ta metoda se poklice ko koncamo interakcijo z nekim objektom
	void IMixedRealityPointerHandler.OnPointerUp(MixedRealityPointerEventData eventData) {
		// Pridobimo GameObject s katerim interaktiramo in preverimo ali gre za atom
		GameObject trenutniGameObject = eventData.Pointer.Result.CurrentPointerTarget as GameObject;
		if (string.Equals(trenutniGameObject.tag, "Atom") && trenutnoInteraktiramo) {
			trenutnoInteraktiramo = false;
			casInterakcije = Time.time - casInterakcije;

			// izklopimo skripto za UI Timer in nastavimo fill amount na 0 (Timerja se ne vidi)
			timer.GetComponent<TimerUI>().enabled = false;
			timer.GetComponent<Image>().fillAmount = 0;

			if (casInterakcije < drzanjeAtomaCas) {
				// Atom smo kliknili
				AtomKlik(trenutniGameObject);
			}
			else {
				// Atom smo drzali
				AtomDrzanje(trenutniGameObject);
			}
		}
		else if (string.Equals(trenutniGameObject.tag, "Nanocev")) {
			stevecRok--;
			// Prenehali smo skalirati, translirati ali rotirati nanocev, zato spet vklopimo vse atome (ce smo izpustili nanocevko z obema rokama)
			if (stevecRok <= 0) {
				stevecRok= 0;
				gameObject.transform.GetChild(0).transform.Find("Hydrogen_mesh").gameObject.SetActive(true);
				//gameObject.transform.GetChild(0).transform.Find("Carbon_mesh").gameObject.SetActive(true);
			}
		}
	}

	void AtomKlik(GameObject atom) {
		// TODO
		// Debug.Log("Klik: " + atom.name); // DELETE
		atom.GetComponent<Renderer>().material.color = Color.magenta; // DELETE

		// TESTING SAVING
		Debug.Log(atom.GetComponent<AtomPodatki>().emso);
	}

	void AtomDrzanje(GameObject atom) {
		// TODO
		// Debug.Log("Drzanje: " + atom.name); // DELETE
		atom.GetComponent<Renderer>().material.color = Color.yellow; // DELETE

		// TESTING SAVING
		string novEmso = "Novo nastavljen emso.";
		atom.GetComponent<AtomPodatki>().emso = novEmso;
	}


	// Teh metod ne bomo potrebovali, vseeno pa morajo biti implementirane
	void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData) { }
	void IMixedRealityPointerHandler.OnPointerDragged(MixedRealityPointerEventData eventData) { }

}
