using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;

public class ObdelavaGlasovnihUkazov : MonoBehaviour {

	// sem bomo shranjevali EMSO, ki ga uporabnik vpisuje
	public string emso = "";

	// tukaj hranimo atom, v katerega vpisujemo EMSO. Izbrani atom nastavimo v AtomiInterakcija skripti (v funkciji dialogZaprt())
	public GameObject atom;

	private ToolTipPrikazovanje toolTipPrikazovanje;
	private AtomPodatki atomPodatki;

	[SerializeField]
	private GameObject dialogSmallPrefab;

	[SerializeField]
	private GameObject voiceOrKeyboardNearMenu;

	private bool vpisujemoZVoiceCommand = false;

	[SerializeField]
	private GameObject atomi;

	private AtomiUpdate atomiUpdate;
	
	private void OnEnable() {
		Debug.Log("ENABLING ObdelavaGlasovnihUkazov"); // TODO DELETE

		atomiUpdate = atomi.GetComponent<AtomiUpdate>();

		// shranimo komponenti ToolTipPrikazovanje in AtomPodatki  atoma, saj bomo do njih  pogosto dostopali
		toolTipPrikazovanje = atom.GetComponent<ToolTipPrikazovanje>();
		atomPodatki = atom.GetComponent<AtomPodatki>();

		// Prikazemo toolTip atoma, ne glede na EyeGaze in nastavimo tekst  ToolTipa  na neko default vrednost
		toolTipPrikazovanje.VsiliToolTipShow(true);
		atomPodatki.UpdateToolTipText("UserID:");
	}

	// stevko shranimo v emso in posodobimo ToolTip
	public void SaveNumber(int number) {
		if (!vpisujemoZVoiceCommand)
			return;
		emso += number.ToString();
		atomPodatki.UpdateToolTipText(emso);
	}

	// odstranimo zadnjo stevilko iz stringa emso (ce ta obstaja) in posodobimo ToolTip
	public void DeleteNumber() {
		if (!vpisujemoZVoiceCommand)
			return;
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
		if (!vpisujemoZVoiceCommand)
			return;
		// preverimo, da emso ni slucajno prazen
		if (emso.Length <= 0)
			return;

		// Uporabniku prikazemo dialog, ki preveri ali je pravilen EMSO
		Dialog endSpeechDialog = Dialog.Open(dialogSmallPrefab, DialogButtonType.Yes | DialogButtonType.No, "Is this your ID?","UserID: " + emso, true);
		if (endSpeechDialog != null) {
			endSpeechDialog.OnClosed += DialogClose;
		}
	}

	private void DialogClose(DialogResult obj) { // koncamo vpisovanje EMSA
		if (obj.Result == DialogButtonType.Yes) {
			// Shranimo emso v izbrani  atom in posodobimo ToolTip
			atomPodatki.emso = emso;
			atomPodatki.UpdateToolTipText(emso);
			toolTipPrikazovanje.VsiliToolTipShow(false); // izklopimo toolTip override

			vpisujemoZVoiceCommand = false;
			atomi.GetComponent<SaveLoadAtoms>().SaveAtomData();
			// TODO tukaj klicemo ResetiranjeAplikacije !!!
			emso = "";
			gameObject.SetActive(false); // izklopimo ta gameObject

		} else if (obj.Result == DialogButtonType.No) {
			emso = "";
			// vprasamo uporabnika ali zeli poskusiti ponovno z voiceCommand, ali zeli poskusiti s SystemKeyboard
			vpisujemoZVoiceCommand = false;
			PrikaziDialogZNavodili();
		}
	}

	public void PrikaziDialogZNavodili() {
		// prikazemo NearMenu, kjer uporabnik izbira ali bo vpisal EMSO z glasovnimi ukazi ali s tipkovnico
		voiceOrKeyboardNearMenu.SetActive(true);
		voiceOrKeyboardNearMenu.transform.position = CameraCache.Main.transform.position + CameraCache.Main.transform.forward * 0.5f;
	}

	public void VklopiVoiceCommand() {
		voiceOrKeyboardNearMenu.SetActive(false);

		Dialog navodilaDialog = Dialog.Open(dialogSmallPrefab, DialogButtonType.Close, "Read your UserID", "Read your UserId (from your ID card) loudly and slowly, digit by digit. To delete the last digit, say \"Delete\". When you finish, say \"Over\".", true);
		vpisujemoZVoiceCommand = true;
	}

	public TouchScreenKeyboard tipkovnica;

	public void VklopiSystemKeyboard() {
		voiceOrKeyboardNearMenu.SetActive(false);

		Dialog navodilaDialog = Dialog.Open(dialogSmallPrefab, DialogButtonType.Confirm, "Type your UserID", "Type your UserID via the System Keyboard. Once you are finished, press \"Confirm\".", true);
		if (navodilaDialog != null) {
			navodilaDialog.OnClosed += EndKeyboard;
		}
		tipkovnica = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.NumberPad, false, false, true, false);  // TODO for some reason mi tuki ne da NumberPad ampak cel keyboard WHAT THE FUCKING SHIT (HOLOLENS JE SHIT)
	}

	private void EndKeyboard(DialogResult obj) {
		// TODO PREVERIT MORMO, ALI JE EMSO LENGTH > 1, SICER MORAMO SE ENKRAT KLICAT FUNKCIJO "VklopiSystemKeyboard" IN SPOROCIT UPORABNIKU, DA MORA EMSO BIT VSAJ 1 CHAR DOLG (JEBISE)
		if (obj.Result == DialogButtonType.Confirm) {
			emso = tipkovnicaTekst;
			tipkovnica.active = false;
			Dialog endSpeechDialog = Dialog.Open(dialogSmallPrefab, DialogButtonType.Yes | DialogButtonType.No, "Is this your ID?", "UserID: " + emso, true);
			if (endSpeechDialog != null) {
				endSpeechDialog.OnClosed += DialogClose;
			}
		}
	}

	private string tipkovnicaTekst;

	void Update() {

		// TODO DELETE FROM HERE (zaenkrat uporabljam samo za testiranje v Unity Editor)
		
		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			SaveNumber(Random.Range(0, 10));
		} else if (Input.GetKeyDown(KeyCode.Alpha2)) {
			DeleteNumber();
		} else if (Input.GetKeyDown(KeyCode.Alpha3)) {
			EndSpeech();
		}
		// TODO DELETE TO HERE (zaenkrat uporabljam samo za testiranje v Unity Editor)

		if (tipkovnica != null) {
			tipkovnicaTekst = tipkovnica.text;
			if (tipkovnicaTekst.Length < 1) {
				atomPodatki.UpdateToolTipText("UserID:");
			} else {
				atomPodatki.UpdateToolTipText(tipkovnicaTekst);
			}
		}
	}
}
