using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class NastavitevTunela : MonoBehaviour
{
	// ko je true, je Z rotacija cevke zaklenjena
	private bool zakleniRotacijoZ = false;

	// ciljna pozicija, rotacija in velikost cevke
	private Vector3 tunelPozicija;
	private Quaternion tunelRotacija;
	private Vector3 tunelVelikost;

	// da preverimo ali je tunel nastavljen preden vanj vstopimo
	private bool tunelNastavljen = false;

	// spremenljivka za zacetek animacije cevke v tunel
	private bool premakniVTunel = false;

	// spremenljivke za animacijo cevke v tunel
	private Vector3 trenutnaHitrostTranslacije = Vector3.zero;
	private Vector3 trenutnaHitrostSkaliranja = Vector3.zero;
	private float casRotacije = 0.0f;

    [SerializeField]
	[Tooltip("Dodaj DialogSmall_192x96 prefab")]
	private GameObject dialogPrefab;
    public void NastaviTunel() {
		// TODO FAJN BI BLO ZASCITIT TO Z NEKIM PIN-OM
		// ustvari se nov dialog
		Dialog nastavljanjeTunelaDialog = Dialog.Open(dialogPrefab, DialogButtonType.Confirm | DialogButtonType.Cancel, "Nastavljanje tunela", "Nastavite tunel na željeno pozicijo, rotacijo in velikost in potrdite.", true);
		// zaklenemo rotacijo nanocevke po Z osi
		zakleniRotacijoZ = true;
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
		zakleniRotacijoZ = false;
		tunelNastavljen = true;
	}

	private void Update() {
		// TODO ALI JE TO OPTIMALEN NACIN ZA ZAKLEPANJE ROTACIJE PO Z OSI
		if (zakleniRotacijoZ) {
			transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
		}
		// TODO TUDI TO NI NAJBOLJ OPTIMALNO
		// Ideja: prestavi ta Update v nek drug script, ki ga aktiviras, takrat, ko ga potrebujes, potem pa ga takoj deaktiviras
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
			} else {
				// cevko transliramo, skaliramo in rotiramo proti ciljni poziciji, velikosti in rotaciji
				transform.position = Vector3.SmoothDamp(transform.position, tunelPozicija, ref trenutnaHitrostTranslacije, 1f);
				transform.localScale = Vector3.SmoothDamp(transform.localScale, tunelVelikost, ref trenutnaHitrostSkaliranja, 1f);
				transform.rotation = Quaternion.Slerp(transform.rotation, tunelRotacija, casRotacije);
				casRotacije += Time.deltaTime * 0.1f;
			}
		}
	}

	public void VstopiVTunel() {
		if (!tunelNastavljen) {
			// TODO prikazi error uporabniku
			Debug.Log("ERROR: Prej je treba nastaviti tunel");
			return;
		}
		// izklopimo komponente: box collider, object manipulator in near interaction grabbable
		GetComponent<BoxCollider>().enabled = false;
		GetComponent<ObjectManipulator>().enabled = false;
		GetComponent<NearInteractionGrabbable>().enabled = false;

		premakniVTunel = true; // zacne premikati cevko v Update funkciji

		// zaradi premikanja cevke izklopimo dolocene atome za optimizacijo
		gameObject.transform.GetChild(0).transform.Find("Hydrogen_mesh").gameObject.SetActive(false);
	}
}