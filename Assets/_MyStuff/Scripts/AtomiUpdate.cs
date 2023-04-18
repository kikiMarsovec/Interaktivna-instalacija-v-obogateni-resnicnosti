using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		// TODO zaenkrat gremo samo cez Holmium atome, naredi da gremo cez  vse
		GameObject holmiumMesh = gameObject.transform.GetChild(0).transform.Find("Holmium_mesh").gameObject;
		for (int i = 0; i < holmiumMesh.transform.childCount; i++) {
			GameObject atom = holmiumMesh.transform.GetChild(i).GetChild(0).gameObject;
			string currentEmso = atom.GetComponent<AtomPodatki>().emso;
			if (currentEmso.Length > 0) {
				atom.GetComponent<EyeTrackingTarget>().enabled = true;
				atom.GetComponent<AtomPodatki>().UpdateToolTipText(currentEmso);
			}
		}
	}

	private void Update() {
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
