using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetiranjeAplikacije : MonoBehaviour {

	[SerializeField]
	private GameObject dialogSmallPrefab;

	[SerializeField]
	private GameObject resetAppNearMenu;

	public void KoncajSprehod() {
		// prikazemo uporabniku dialog, ki ga obvesti, da je sprehod koncan.
		Dialog endWalkDialog = Dialog.Open(dialogSmallPrefab, DialogButtonType.Close, "Thank you for using the app", "The walk among atoms is now finished. Please press Close and pass the HoloLens to the next person.", true);
		if (endWalkDialog != null) {
			endWalkDialog.OnClosed += DialogClose;
		}
	}

	private void DialogClose(DialogResult obj) { 
		if (obj.Result == DialogButtonType.Close) {
			// prikazemo gumb za resetiranje aplikacije
			resetAppNearMenu.SetActive(true);
			resetAppNearMenu.transform.position = CameraCache.Main.transform.position + CameraCache.Main.transform.forward * 0.5f;
			// skrijemo nanocevko
			gameObject.transform.localScale = Vector3.zero;
			gameObject.SetActive(false);
		}
	}

	public void ResetirajAplikacijo() {
		gameObject.SetActive(true);
		// izklopimo near menu za resetiranje aplikacije
		resetAppNearMenu.SetActive(false);
		// zopet prikazemo nanocevko (z animacijo)
		Vector3 ciljnaPozicija = CameraCache.Main.transform.position + CameraCache.Main.transform.forward * 1.5f;
		Quaternion ciljnaRotacija = transform.rotation;
		Vector3 ciljnaVelikost = Vector3.one *  0.5f;
		gameObject.GetComponent<AtomiUpdate>().enabled = true;
		gameObject.GetComponent<AtomiUpdate>().zacniAnimacijoCevke(ciljnaPozicija, ciljnaRotacija, ciljnaVelikost, false);
		// gremo cez vse atome in izklopimo EnableDisableEyeTrackingTarget, nato EyeTrackingTarget in resetiramo ToolTipPrikazovanje
		GameObject atomi = gameObject.transform.GetChild(0).gameObject;
		for (int i = 0; i < atomi.transform.childCount; i++) {
			GameObject atomMesh = atomi.transform.GetChild(i).gameObject;
			for (int j = 0; j < atomMesh.transform.childCount; j++) {
				GameObject atom = atomMesh.transform.GetChild(j).GetChild(0).gameObject;
				string currentEmso = atom.GetComponent<AtomPodatki>().emso;
				if (currentEmso.Length > 0) {
					atom.GetComponent<EnableDisableEyeTrackingTarget>().enabled = false;
					atom.GetComponent<EyeTrackingTarget>().enabled = false;
					atom.GetComponent<ToolTipPrikazovanje>().ResetirajSkripto();

				}
			}
		}

		// v vsaki skripti klicemo ResetirajSkripto

		// TODO v AtomiUpdate se mi zdi da ne rabimo

		// TODO v ObdelavaGlasovnihUkazov se mi zdi da tudi ne rabimo

		// Resetiramo NastavitevTunela
		gameObject.GetComponent<NastavitevTunela>().ResetirajSkripto();
		
		// Resetiramo AtomiInterakcija 
		gameObject.GetComponent<AtomiInterakcija>().ResetirajSkripto();

		// Naloadamo podatke
		gameObject.GetComponent<SaveLoadAtoms>().LoadAtomData();

		// uporabniku damo gumb za resetiranje aplikacije, ko ga klikne, se nanocevka  zmanjsa  in pozicionira pred njega

	}
}
