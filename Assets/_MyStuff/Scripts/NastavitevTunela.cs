using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class NastavitevTunela : MonoBehaviour {
	// AtomiUpdate.cs skript
	AtomiUpdate atomiUpdate;

	// ciljna pozicija, rotacija in velikost cevke
	private Vector3 tunelPozicija;
	private Quaternion tunelRotacija;
	private Vector3 tunelVelikost;

	// da preverimo ali je tunel nastavljen preden vanj vstopimo
	private bool tunelNastavljen = false;

	// ce veckrat pritisnemo na gumb za nastavljanje tunela, moramo dobiti samo en Dialog
	private bool tunelVNastavljanju = false;

	// nastavimo na true, ko uporabnik vstopi v tunel
	public bool smoVTunelu = false;

    [SerializeField]
	[Tooltip("Dodaj DialogSmall_192x96 prefab")]
	private GameObject dialogPrefab;
	
	[SerializeField]
	[Tooltip("Dodaj DialogLarge prefab")]
	private GameObject dialogLargePrefab;

	[SerializeField]
	private GameObject gumbBringGWireBack;

	public bool vpisujemoEmso = false;

	private void Start() {
		atomiUpdate = GetComponent<AtomiUpdate>();
		// Takoj na zacetku deaktiviramo atomiUpdate, da se Update funkcija ne bo izvajala po nepotrebnem
		atomiUpdate.enabled = false;
	}

	public void NastaviTunel() {
		// Ce je tunel ze v nastavljanju, ne odpremo novega Dialog-a
		if (tunelVNastavljanju) {
			// Debug.Log("ERROR: Tunel je ze v nastavljanju.");
			return;
		}
		tunelVNastavljanju = true;

		// ustvari se nov dialog
		Dialog nastavljanjeTunelaDialog = Dialog.Open(dialogPrefab, DialogButtonType.Confirm | DialogButtonType.Cancel, "Setting up tunnel", "Set up the G-wire to your desired position, rotation and size and press Confirm.", true);

		// vklopimo komponente: box collider, object manipulator in near interaction grabbable (ce je slucajno cevka ze nastavljena v tunel)
		GetComponent<BoxCollider>().enabled = true;
		GetComponent<ObjectManipulator>().enabled = true;
		GetComponent<NearInteractionGrabbable>().enabled = true;

		// zaklenemo rotacijo nanocevke po Z osi
		atomiUpdate.enabled = true;
		atomiUpdate.nastaviZaklenjenostRotacijeZ(true);

		// ko se dialog zapre klicemo funkcijo dialogZaprt
		if (nastavljanjeTunelaDialog != null)
			nastavljanjeTunelaDialog.OnClosed += dialogZaprt;
	}

	private void dialogZaprt(DialogResult obj) {
		if (obj.Result == DialogButtonType.Confirm) {
			// ce uporabnik klikne na gumb za potrditev, shranimo 
			tunelPozicija = transform.position;
			tunelRotacija = transform.rotation;
			tunelVelikost = transform.localScale;
		}
		// spet omogocimo rotacijo po Z osi
		atomiUpdate.nastaviZaklenjenostRotacijeZ(false);
		atomiUpdate.enabled = false;
		tunelNastavljen = true;
		tunelVNastavljanju = false;
	}

	public void VstopiVTunel() {
		if (!tunelNastavljen) {
			// ce tunel se nima nastavljene ciljne pozicije, rotacije in velikosti ter kliknemo gumb za vstop, se pojavi ERROR
			// TODO prikazi error uporabniku
			// Debug.Log("ERROR: Prej je treba nastaviti tunel");
			return;
		}
		if (tunelVNastavljanju) {
			// ce je tunel ze bil predhodno nastavljen in ga zopet nastavljamo ter kliknemo gumb za vstop, se pojavi  ERROR
			// TODO prikazi error uporabniku
			// Debug.Log("ERROR: Dokler nastavljate tunel, vanj ni mogoce vstopiti.");
			return;
		}
		// izklopimo komponente: box collider, object manipulator in near interaction grabbable
		GetComponent<BoxCollider>().enabled = false;
		GetComponent<ObjectManipulator>().enabled = false;
		GetComponent<NearInteractionGrabbable>().enabled = false;

		smoVTunelu = true;
		gumbBringGWireBack.GetComponent<ButtonConfigHelper>().MainLabelText = "Exit tunnel";

		// Klicemo funkcioj, ki zacne premikati cevko v Update funkciji
		atomiUpdate.enabled = true;
		atomiUpdate.zacniAnimacijoCevke(tunelPozicija, tunelRotacija, tunelVelikost, false);
	}

	public void BringNanotubeBack() {
		if (vpisujemoEmso) {
			return;
		}
		if (smoVTunelu) {
			// izstopimo iz tunela tako, da resetiramo aplikacijo
			GetComponent<ResetiranjeAplikacije>().ResetirajAplikacijo();
			return;
		}
		else if (GetComponent<AtomiUpdate>().vpisujemoPin) {
			// ce vpisujemo pin, ne smemo aktivirati te funkcije (ker zapre skripto  AtomiUpdate)
			return;
		}
		// aktiviramo s klikom na gumb v meniju
		// ce se nanocevka izgubi nekje v  prostoru  (npr uporabnik jo po pomoti da za kaksno steno in jo ne more doseci) jo z  animacijo prikazemo pred uporabnikom
		Vector3 ciljnaPozicija = CameraCache.Main.transform.position + CameraCache.Main.transform.forward * 1.5f;
		Quaternion ciljnaRotacija = transform.rotation;
		Vector3 ciljnaVelikost = Vector3.one * 0.5f;
		bool atomiUpdateStayActivated = gameObject.GetComponent<AtomiUpdate>().enabled; // ce je AtomiUpdate ze "enabled", ga ne ugasnemo na koncu
		gameObject.GetComponent<AtomiUpdate>().enabled = true;
		gameObject.GetComponent<AtomiUpdate>().zacniAnimacijoCevke(ciljnaPozicija, ciljnaRotacija, ciljnaVelikost, atomiUpdateStayActivated);
	}

	private bool dialogInformacijePrikazan = false;
	public void PrikaziInformacije() {
		// ce je ze prikazan dialog, ga ne prikazemo se enkrat
		if (dialogInformacijePrikazan)
			return;

		dialogInformacijePrikazan = true;
		Dialog informacijeDialog = Dialog.Open(dialogLargePrefab, DialogButtonType.Close, "Information", "Before you is a 3D model of a G-wire. You can manipulate the model with your hands, move it around the room, rotate it and scale it. If the model goes out of reach, you can bring it back in front of you by pressing the 'Bring G-wire back' button. When you want to walk through the tunnel, press the 'Enter the tunnel' button. Inside the tunnel, you can select atoms with your hands. If you hold an atom for less than 3 seconds, information about the selected atom will be displayed. If you hold an atom for more than three seconds, you will be able to input your universal identifier into it.", true);
		if (informacijeDialog != null) {
			informacijeDialog.OnClosed += InfoDialogClosed;
		}
	}

	private void InfoDialogClosed(DialogResult obj) {
		if (obj.Result == DialogButtonType.Close)
			dialogInformacijePrikazan = false;
	}

	public void ResetirajSkripto() {
		GetComponent<BoxCollider>().enabled = true;
		GetComponent<ObjectManipulator>().enabled = true;
		GetComponent<NearInteractionGrabbable>().enabled = true;
		smoVTunelu = false;
		vpisujemoEmso = false;
		gumbBringGWireBack.GetComponent<ButtonConfigHelper>().MainLabelText = "Bring G-wire back";
	}
}