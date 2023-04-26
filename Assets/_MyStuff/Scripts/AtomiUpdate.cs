using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// V AtomiUpdate.cs sem prestavil Update funkcijo iz NastavitevTunela.cs (Za optimizacijo).
// Skripto bom aktiviral le takrat, ko jo potrebujem, takoj zatem pa jo bom deaktiviral.

public class AtomiUpdate : MonoBehaviour {

	// ko je true, je Z rotacija cevke zaklenjena
	private bool zakleniRotacijoZ = false;

	// ciljna pozicija, rotacija in velikost cevke
	private Vector3 tunelPozicija;
	private Quaternion tunelRotacija;
	private Vector3 tunelVelikost;

	// spremenljivka za zacetek animacije cevke v tunel
	private bool premakniVTunel = false;

	// spremenljivke za animacijo cevke v tunel
	private Vector3 trenutnaHitrostTranslacije = Vector3.zero;
	private Vector3 trenutnaHitrostSkaliranja = Vector3.zero;
	private float casRotacije = 0.0f;

	[SerializeField]
	private GameObject dialogSmallPrefab;

	public void nastaviZaklenjenostRotacijeZ(bool zakleni) {
		zakleniRotacijoZ=zakleni;
	}

	public void zacniAnimacijoCevke(Vector3 ciljnaPozcijia, Quaternion ciljnaRotacija, Vector3 ciljnaVelikost) {
		tunelPozicija = ciljnaPozcijia;
		tunelRotacija = ciljnaRotacija;
		tunelVelikost = ciljnaVelikost;
		premakniVTunel = true;

		// zaradi premikanja cevke izklopimo dolocene atome za optimizacijo
		gameObject.transform.GetChild(0).transform.Find("Hydrogen_mesh").gameObject.SetActive(false);

		// gremo cez vse atome in tistim, ki so ze zasedeni (imajo EMSO) vklopimo EyeTrackingTarget (da bodo prikazovali Tooltipe) in nastavimo ToolTipText na EMSO
		// TODO ce bo negativno vplivalo na performance, lahko probam izvesti to v Coroutini
		GameObject atomi = gameObject.transform.GetChild(0).gameObject;
		for (int i = 0; i < atomi.transform.childCount; i++) {
			GameObject atomMesh = atomi.transform.GetChild(i).gameObject;
			for (int j = 0; j < atomMesh.transform.childCount; j++) {
				GameObject atom = atomMesh.transform.GetChild(j).GetChild(0).gameObject;
				string currentEmso = atom.GetComponent<AtomPodatki>().emso;
				if (currentEmso.Length > 0) {
					//atom.GetComponent<EyeTrackingTarget>().enabled = true; // NE UPORABLJAM VEC  EYETRACKINGTARGET
					atom.GetComponent<CustomEyeTrackingTarget>().enabled = true;
					atom.GetComponent<AtomPodatki>().UpdateToolTipText(currentEmso);
				}
			}
		}
	}


	// System Keyboard
	public TouchScreenKeyboard tipkovnica;

	private bool vpisujemoPin = false;
	public void OdpriTipkovnicoZaPin() {
		if (vpisujemoPin)
			return;
		vpisujemoPin = true;
		Dialog pinDialog = Dialog.Open(dialogSmallPrefab, DialogButtonType.Yes | DialogButtonType.No, "Setting up tunnel", "Setting up tunnel requires admin rights. Do you want to continue? After pressing Yes, you will be prompted to enter a PIN code.", true);
		if (pinDialog != null) {
			pinDialog.OnClosed += DialogClose;
		}
	}

	private void DialogClose(DialogResult obj) {
		if (obj.Result == DialogButtonType.Yes) {
			// uporanbnik zeli vnesti pin, aktiviramo tipkovnico
			tipkovnica = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.NumberPad, false, false, true, false);
		} else if (obj.Result == DialogButtonType.No) {
			// uporabnik ne zeli vnesti pin-a, zato deaktiviramo to skripto
			vpisujemoPin = false;
			this.enabled = false;
		}
	}

	private void Update() {

		// TODO  DELETE FROM HERE (This is only for testing in  Unity Editor)
		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			gameObject.GetComponent<NastavitevTunela>().NastaviTunel();
		}
		// DELETE  TO HERE (This is only for testing in  Unity Editor)


		if (tipkovnica != null) {
			if (vpisujemoPin) {
				if (tipkovnica.text.Length == 4) {
					if (tipkovnica.text == "2525") {
						// TODO ali se da PIN kodo morda nekam skriti?

						// uporabnik je vnesel pravilen PIN
						// klicemo funkcijo za nastavljanje pin-a
						gameObject.GetComponent<NastavitevTunela>().NastaviTunel();
					} else {
						// uporabnik ni vnesel pravilen PIN
						// odpremo dialog in sporocimo uporabniku, da pin ni pravilen
						Dialog pinDialog = Dialog.Open(dialogSmallPrefab, DialogButtonType.Close, "PIN incorrect", "The PIN code you have entered is incorrect.", true);
					}
					// izklopimo to skripto
					tipkovnica.active = false;
					vpisujemoPin = false;
					this.enabled = false;
				}
			}
		}
		if (zakleniRotacijoZ) {
			transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
		}
		if (premakniVTunel) {
			// preverimo ali je cevka priblizno na pravi poziciji, velikosti in rotaciji
			if (Vector3.Distance(transform.position, tunelPozicija) < 0.1f && Quaternion.Angle(transform.rotation, tunelRotacija) < 1 && Vector3.Distance(transform.localScale, tunelVelikost) < 0.1f) {
				// ko cevka doseze pravilno pozicijo, velikost in rotacijo, prenehamo
				premakniVTunel = false;
				trenutnaHitrostTranslacije = Vector3.zero;
				trenutnaHitrostSkaliranja = Vector3.zero;
				casRotacije = 0;
				// vklopimo nazaj izklopljene atome
				gameObject.transform.GetChild(0).transform.Find("Hydrogen_mesh").gameObject.SetActive(true);
				// izklopimo AtomiUpdate.cs, ker ga ne potrebujemo vec
				this.enabled = false;
			}
			else {
				// cevko transliramo, skaliramo in rotiramo proti ciljni poziciji, velikosti in rotaciji
				transform.position = Vector3.SmoothDamp(transform.position, tunelPozicija, ref trenutnaHitrostTranslacije, 1f);
				transform.localScale = Vector3.SmoothDamp(transform.localScale, tunelVelikost, ref trenutnaHitrostSkaliranja, 1f);
				transform.rotation = Quaternion.Slerp(transform.rotation, tunelRotacija, casRotacije);
				casRotacije += Time.deltaTime * 0.1f;
			}
		}
	}
}
