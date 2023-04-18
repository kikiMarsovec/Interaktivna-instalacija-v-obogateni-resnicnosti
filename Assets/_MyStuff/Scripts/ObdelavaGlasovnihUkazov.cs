using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class ObdelavaGlasovnihUkazov : MonoBehaviour {

	// sem bomo shranjevali EMSO, ki ga uporabnik vpisuje
	public string emso = "";

	// tukaj hranimo atom, v katerega vpisujemo EMSO. Izbrani atom nastavimo v AtomiInterakcija skripti (v funkciji dialogZaprt())
	public GameObject atom;

	private ToolTipPrikazovanje toolTipPrikazovanje;
	private AtomPodatki atomPodatki;

	[SerializeField]
	private GameObject dialogSmallPrefab; 
	
	private void OnEnable() {
		// TODO maybe tukaj prikazi dialog z navodili uporabniku ???

		// shranimo komponenti ToolTipPrikazovanje in AtomPodatki  atoma, saj bomo do njih  pogosto dostopali
		toolTipPrikazovanje = atom.GetComponent<ToolTipPrikazovanje>();
		atomPodatki = atom.GetComponent<AtomPodatki>();

		// Prikazemo toolTip atoma, ne glede na EyeGaze in nastavimo tekst  ToolTipa  na neko default vrednost
		toolTipPrikazovanje.VsiliToolTipShow(true);
		atomPodatki.UpdateToolTipText("UserID:");
	}
	private void OnDisable() {
		// TODO tukaj klici funkcijo VsiliToolTipShow(false) AMPAK PREVIDNO!!!! -> Ko se aplikacija zapre, ti bo tukaj metalo ERROR. Nekako se izogni temu.	(Mogoce bi lahko disablal to skripto preden se ugasne program)
	}

	// stevko shranimo v emso in posodobimo ToolTip
	public void SaveNumber(int number) {
		emso += number.ToString();
		atomPodatki.UpdateToolTipText(emso);
	}

	// odstranimo zadnjo stevilko iz stringa emso (ce ta obstaja) in posodobimo ToolTip
	public void DeleteNumber() {
		if (emso.Length > 0) {
			emso = emso.Substring(0, emso.Length - 1);
			if (emso.Length == 0) {
				atomPodatki.UpdateToolTipText("UserID:");
			} else {
				atomPodatki.UpdateToolTipText(emso);
			}
		}
	}

	// preverimo, ali je uporabnik vnesel pravilen EMSO (z Dialogom). Ce je, shranimo emso v atom. Nato naj bi se aplikacija  zaprla.  Ce  ni  vnesel  pravilnega, mu se enkrat  odpremo  dialog ali  bi poskusil z VoiceCommand ali s SystemKeyboard
	public void EndSpeech() {
		// Uporabniku prikazemo dialog, ki preveri ali je pravilen EMSO
		Dialog endSpeechDialog = Dialog.Open(dialogSmallPrefab, DialogButtonType.Yes | DialogButtonType.No, "Is this your ID?","UserID: " + emso, true);
		if (endSpeechDialog != null) {
			endSpeechDialog.OnClosed += DialogClose;
		}
	}

	private void DialogClose(DialogResult obj) {
		if (obj.Result == DialogButtonType.Yes) {
			// Shranimo emso v izbrani  atom in posodobimo ToolTip
			atomPodatki.emso = emso;
			atomPodatki.UpdateToolTipText(emso);
			// TODO aplikacija se mora zapreti (ampak najprej moramo disablati to skripto)
		} else if (obj.Result == DialogButtonType.No) {
			// Nastavimo emso nazaj na prazen string in vprasamo uporabnika ali zeli poskusiti ponovno z voiceCommand, ali zeli poskusiti s SystemKeyboard
			emso = "";
			// TODO Dialog
		}
	}

	public void PrikaziDialogZNavodili() {
		Dialog navodilaDialog = Dialog.Open(dialogSmallPrefab, DialogButtonType.Close, "Read your UserID", "Read your UserId (from your ID card) loudly and slowly, digit by digit. To delete the last digit, say \"Delete\". When you finish, say \"Over\".", true);
	}

	void Update() { // TODO DELETE, zaenkrat uporabljam samo za testiranje v Unity Editor
		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			SaveNumber(Random.Range(0, 10));
		} else if (Input.GetKeyDown(KeyCode.Alpha2)) {
			DeleteNumber();
		} else if (Input.GetKeyDown(KeyCode.Alpha3)) {
			EndSpeech();
		}
    }
}
