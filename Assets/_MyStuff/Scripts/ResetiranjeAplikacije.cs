using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetiranjeAplikacije : MonoBehaviour
{
    public void ResetirajAplikacijo() {
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
