using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
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

    [SerializeField]
	[Tooltip("Dodaj DialogSmall_192x96 prefab")]
	private GameObject dialogPrefab;

	private void Start() {
		atomiUpdate = GetComponent<AtomiUpdate>();
		// Takoj na zacetku deaktiviramo atomiUpdate, da se Update funkcija ne bo izvajala po nepotrebnem
		atomiUpdate.enabled = false;
	}

	public void NastaviTunel() {
		// Ce je tunel ze v nastavljanju, ne odpremo novega Dialog-a
		if (tunelVNastavljanju) {
			Debug.Log("ERROR: Tunel je ze v nastavljanju."); //  TODO
			return;
		}
		tunelVNastavljanju = true;

		// ustvari se nov dialog
		Dialog nastavljanjeTunelaDialog = Dialog.Open(dialogPrefab, DialogButtonType.Confirm | DialogButtonType.Cancel, "Setting up tunnel", "Set up the nanotube to your desired position, rotation and size and press Confirm.", true);

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
			Debug.Log("ERROR: Prej je treba nastaviti tunel");
			return;
		}
		if (tunelVNastavljanju) {
			// ce je tunel ze bil predhodno nastavljen in ga zopet nastavljamo ter kliknemo gumb za vstop, se pojavi  ERROR
			// TODO prikazi error uporabniku
			Debug.Log("ERROR: Dokler nastavljate tunel, vanj ni mogoce vstopiti.");
			return;
		}
		// izklopimo komponente: box collider, object manipulator in near interaction grabbable
		GetComponent<BoxCollider>().enabled = false;
		GetComponent<ObjectManipulator>().enabled = false;
		GetComponent<NearInteractionGrabbable>().enabled = false;


		// Klicemo funkcioj, ki zacne premikati cevko v Update funkciji
		atomiUpdate.enabled = true;
		atomiUpdate.zacniAnimacijoCevke(tunelPozicija, tunelRotacija, tunelVelikost, false);
	}

	public void ResetirajSkripto() {
		GetComponent<BoxCollider>().enabled = true;
		GetComponent<ObjectManipulator>().enabled = true;
		GetComponent<NearInteractionGrabbable>().enabled = true;
	}
}