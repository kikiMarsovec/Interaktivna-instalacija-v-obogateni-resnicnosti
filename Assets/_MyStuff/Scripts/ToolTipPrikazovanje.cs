using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTipPrikazovanje : MonoBehaviour {

	[SerializeField]
	private GameObject toolTip;

	// ce gledamo v tooltip,, nastavimo na true, da toolTip ne izgine prehitro
	private bool gledamoVTooltip = false;

	// s to spremenljivko preprecimo, da eyeGaze vpliva na prikazovanje  in skrivanje toolTip-a
	private bool showToolTipNoMatterWhat = false;

	public Vector3 toolTipVelikost = new Vector3(1f,1f,1f);
	public Vector3 toolTipPozicija = new Vector3(0f,1f,0f);
	public Vector3 toolTipBackgroundVelikost = new Vector3(0.125f,0.023f,1f);

	public void ShowToolTip() {
		if (showToolTipNoMatterWhat)
			return;
		// nastavimo velikost in  pozicijo tooltip-a in velikost TipBackground-a
		float velikostNanocevke = gameObject.transform.parent.transform.parent.transform.parent.transform.parent.localScale.x;
		toolTip.transform.GetChild(1).transform.localScale = toolTipVelikost;
		toolTip.transform.GetChild(1).transform.position = gameObject.transform.position + toolTipPozicija * velikostNanocevke;
		toolTip.transform.GetChild(1).transform.GetChild(0).transform.GetChild(1).transform.localScale = toolTipBackgroundVelikost;
		// prikazemo toolTip
		gledamoVTooltip = true;
		toolTip.SetActive(true);
	}

	public void HideToolTipWithDelay() {
		if (showToolTipNoMatterWhat)
			return;
		gledamoVTooltip = false;
		// po 10 sekundah poklicemo funkcijo, ki skrije toolTip
		Invoke("HideToolTip", 10f);
		
		// TODO Invoke naj bi slabo vplival  na performance  (poglej  dokumentacijo), baje da je bolje nadomestit s Coroutine, testiraj na HoloLens ocalih, ce dejansko slabo vpliva na performance
	}

	private void HideToolTip() { // to metodo poklicemo z Invoke(...)
		if (!gledamoVTooltip)
			toolTip.SetActive(false);
	}

	// s to metodo preprecimo, da EyeGaze vpliva  na prikazovanje in skrivanje ToolTip-a. S spremenljivko show povemo ali zelimo ToolTip prikazati  ali skriti
	public void VsiliToolTipShow(bool show) {
		if (show) {
			// nastavimo velikost in  pozicijo tooltip-a in velikost TipBackground-a
			float velikostNanocevke = gameObject.transform.parent.transform.parent.transform.parent.transform.parent.localScale.x;
			toolTip.transform.GetChild(1).transform.localScale = toolTipVelikost;
			toolTip.transform.GetChild(1).transform.position = gameObject.transform.position + toolTipPozicija * velikostNanocevke;
			toolTip.transform.GetChild(1).transform.GetChild(0).transform.GetChild(1).transform.localScale = toolTipBackgroundVelikost;
			// prikazemo ToolTip
			showToolTipNoMatterWhat = true;
			toolTip.SetActive(true);
		} else {
			showToolTipNoMatterWhat = false;
			toolTip.SetActive(false);
		}
	}


	public void ResetirajSkripto() {
		gledamoVTooltip = false;
		showToolTipNoMatterWhat = false;
		toolTip.SetActive(false);
	}
}
