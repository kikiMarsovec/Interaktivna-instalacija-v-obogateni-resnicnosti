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

	public void ShowToolTip() {
		if (showToolTipNoMatterWhat)
			return;
		// prikazemo toolTip in ga nastavimo na ustrezno pozicijo (malo nad atom)
		gledamoVTooltip = true;
		toolTip.SetActive(true);
		// toolTip.transform.GetChild(1).transform.position = gameObject.transform.position + new Vector3(0f, 0.25f, 0f); // TODO to ne dela, ce imamo zelo  majhno nanocevko, ker je ToolTip prevec zgoraj
		// toolTip.transform.GetChild(1).transform.position = gameObject.transform.position + new Vector3(0f, toolTip.transform.GetChild(1).transform.position.y, 0f);
		toolTip.transform.GetChild(1).transform.position = gameObject.transform.position + new Vector3(0f, 1.25f * gameObject.transform.lossyScale.y, 0f); // pozicijo nastavimo glede na globalno velikost atoma
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
			// nastavimo pivot na ustrezno globalno pozicijo (nad atom) in prikazemo ToolTip
			showToolTipNoMatterWhat = true;
			toolTip.SetActive(true);
			toolTip.transform.GetChild(1).transform.position = gameObject.transform.position + new Vector3(0f, 1.25f * gameObject.transform.lossyScale.y, 0f); // pozicijo nastavimo glede na globalno velikost atoma
		} else {
			showToolTipNoMatterWhat = false;
			toolTip.SetActive(false);
		}
	}
}
