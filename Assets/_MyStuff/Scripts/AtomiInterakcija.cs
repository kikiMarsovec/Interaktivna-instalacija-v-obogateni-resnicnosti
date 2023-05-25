using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AtomiInterakcija : MonoBehaviour, IMixedRealityPointerHandler {

	// spremenljivka, ki nam pove, ali trenutno interaktiramo z atomom
	private bool trenutnoInteraktiramo = false;

	// Ko imamo odprt Dialog, onemogocimo interakcijo (da uporabnik ne more odpreti prevec dialogov hkrati)
	private bool dialogOdprt = false;

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

	// sem shranimo atom v katerega se bo vpisal emso
	private GameObject izbraniAtom = null;

	// s to spremenljivko  preprecimo, da bi uporabnik med vpisovanjem emsa izbral se en atom
	private bool trenutnoVpisujemoEmso = false;

	[SerializeField]
	[Tooltip("Dodaj DialogMedium prefab")]
	private GameObject dialogMediumPrefab;
	
	[SerializeField]
	[Tooltip("Dodaj DialogSmall prefab")]
	private GameObject dialogSmallPrefab;

	// Tu imamo speechHandler, da ga lahko aktiviramo in mu  podamo atom
	[SerializeField]
	private GameObject speechHandler;

	private void Start() {
		// Na zacetku nalozimo ze shranjene podatke
		gameObject.GetComponent<SaveLoadAtoms>().LoadAtomData();
	}


	// Ta metoda se poklice ko zacnemo interakcijo z nekim objektom
	void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData) {
		if (trenutnoVpisujemoEmso)
			return;

		// Pridobimo GameObject s katerim interaktiramo in preverimo ali gre za atom
		GameObject trenutniGameObject = eventData.Pointer.Result.CurrentPointerTarget as GameObject;
		if (string.Equals(trenutniGameObject.tag, "Atom") && !trenutnoInteraktiramo && !dialogOdprt) {
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
		if (trenutnoVpisujemoEmso)
			return;

		// Pridobimo GameObject s katerim interaktiramo in preverimo ali gre za atom
		GameObject trenutniGameObject = eventData.Pointer.Result.CurrentPointerTarget as GameObject;
		if (string.Equals(trenutniGameObject.tag, "Atom") && trenutnoInteraktiramo && !dialogOdprt) {
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
		// Odpremo Dialog, ki izpiše podatke o izbranem atomu (ime elementa, simbol za element, lokalno pozicijo?, status: zaseden/nezaseden, emso)
		string vsebina = string.Format("Chemical element: {0}\nLocal position:\n x: {4,10:F2}\n y: {5,10:F2}\n z: {6,10:F2}\nSymbol: {1}\nStatus: {2}\nUserID: {3}", 
			atom.GetComponent<AtomPodatki>().pridobiIme(), atom.GetComponent<AtomPodatki>().pridobiSimbol(), atom.GetComponent<AtomPodatki>().pridobiStatus(), atom.GetComponent<AtomPodatki>().emso,
			atom.transform.parent.transform.localPosition.x, atom.transform.parent.transform.localPosition.y, atom.transform.parent.transform.localPosition.z);
		Dialog atomKlikDialog = Dialog.Open(dialogMediumPrefab, DialogButtonType.Close, "Atom Info", vsebina, true);
		//  onemogocimo  interakcijo  z  drugimi  atomi,  dokler je Dailog  odprt
		dialogOdprt = true;
		// ko se dialog zapre klicemo funkcijo dialogZaprt
		if (atomKlikDialog != null)
			atomKlikDialog.OnClosed += dialogZaprt;
	}

	void AtomDrzanje(GameObject atom) {
		string vsebina;
		Dialog atomDrzanjeDialog;
		// preverimo ali je atom ze izbran ali je na voljo, da uporabnik vanj vpise svoj emso
		if (atom.GetComponent<AtomPodatki>().niZaseden()) {
			// ustvarimo dialog kjer user lahko izbere vpis svojega emsa v atom
			vsebina = string.Format("You have chosen {0} ({1}) atom with local position \nx = {2:F2}, y = {3:F2}, z = {4:F2}. The atom is available. After clicking the Accept button, you will enter your identification number inside the atom, which will end your walk among the atoms.",
				atom.GetComponent<AtomPodatki>().pridobiIme(), atom.GetComponent<AtomPodatki>().pridobiSimbol(),
				atom.transform.parent.transform.localPosition.x, atom.transform.parent.transform.localPosition.y, atom.transform.parent.transform.localPosition.z);
			atomDrzanjeDialog = Dialog.Open(dialogMediumPrefab, DialogButtonType.Accept | DialogButtonType.Cancel, "Atom Available", vsebina, true);
			// v izbraniAtom shranimo trenutni atom, da bomo v funkciji dialogZaprt vanj lahko vpisali emso
			izbraniAtom = atom;
		} else {
			// ustvarimo dialog, ki opozori uporabnika, da je atom ze zaseden
			vsebina = string.Format("You have chosen {0} ({1}) atom with local position \nx = {2:F2}, y = {3:F2}, z = {4:F2}. The atom is already taken, please choose another one.",
				atom.GetComponent<AtomPodatki>().pridobiIme(), atom.GetComponent<AtomPodatki>().pridobiSimbol(), 
				atom.transform.parent.transform.localPosition.x, atom.transform.parent.transform.localPosition.y, atom.transform.parent.transform.localPosition.z);
			atomDrzanjeDialog = Dialog.Open(dialogSmallPrefab, DialogButtonType.Close, "Atom Taken", vsebina, true);
		}
		dialogOdprt = true;
		if (atomDrzanjeDialog != null) {
			atomDrzanjeDialog.OnClosed += dialogZaprt;
		}
	}

	private void dialogZaprt(DialogResult obj) {
		// ko se  Dialog zapre spet  omogocimo  interakcijo
		if (obj.Result == DialogButtonType.Close || obj.Result == DialogButtonType.Cancel)
			dialogOdprt = false;
		else if (obj.Result == DialogButtonType.Accept) {
			dialogOdprt = false;
			// TODO USER MUST ENTER HIS OWN EMSO VIA VOICE COMMAND  OR SYSTEM KEYBOARD
			if (izbraniAtom != null) {
				// Aktiviramo SpeechHandler in mu podamo atom, v katerega moramo vpisati emso
				speechHandler.GetComponent<ObdelavaGlasovnihUkazov>().atom = izbraniAtom;
				speechHandler.SetActive(true);
				speechHandler.GetComponent<ObdelavaGlasovnihUkazov>().PrikaziDialogZNavodili();   // uporabniku pokazemo dialog z navodili za Glasovne Ukaze

				// TODO - verjetno bi rabili tudi nek dialog, ki vprasa uporabnika ali bi rad vnesel EMSO z VoiceCommand ali s SystemKeyboard

				// izklopimo interakcijo z atomi
				trenutnoVpisujemoEmso = true;
			} else
				throw new System.Exception("No atom chosen."); // TODO PREVERI, DA SE TO NE MORA ZGODIT
		}
	}

	// Teh metod ne bomo potrebovali, vseeno pa morajo biti implementirane
	void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData) { }
	void IMixedRealityPointerHandler.OnPointerDragged(MixedRealityPointerEventData eventData) { }

}
