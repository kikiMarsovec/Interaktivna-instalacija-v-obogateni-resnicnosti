using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NastavitevTunela : MonoBehaviour
{
	private bool zakleniRotacijoZ = false;

	private Vector3 tunelPozicija;
	private Quaternion tunelRotacija;
	private Vector3 tunelVelikost;

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
	}

	private void Update() {
		// TODO ALI JE TO OPTIMALEN NACIN ZA ZAKLEPANJE ROTACIJE PO Z OSI
		if (zakleniRotacijoZ) {
			transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
		}
	}
}
