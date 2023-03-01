using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

// Serializable data class, ki vsebuje atribute atomov (emso)
[System.Serializable]
public class AtomData {
	public string emso;
}

// Unity ne more serializirati seznamov. S tem se izognemo temu problemu.
// TAKEN FROM: https://answers.unity.com/questions/1361721/converting-a-generic-list-to-json-in-unity.html
[System.Serializable]
public class SerializableList<T> {
	public List<T> list;

	public void Add(T item) { // metoda za dodajanje novega elementa na konec seznama
		this.list.Add(item);
	}

	public T First() { // metoda za pridobivanje prvega elementa iz seznama
		return this.list.First();
	}

	public void RemoveFirst() {  // metoda za brisanje prvega elementa iz seznama
		this.list.RemoveAt(0);
	}

	public void Clear() {
		this.list.Clear();
	}
}

public class SaveLoadAtoms : MonoBehaviour {
	// path, kamor bomo shranili datoteko s podatki o atomih
	private string path;
	private void Awake() {
		// pridobimo path,  kjer lahko shranimo podatke o atomih
		path = Application.persistentDataPath + "/atom_data.json";
	}

	[SerializeField] private SerializableList<AtomData> atomDataList; //ustvarimo nov seznam, kamor bomo shranjevali atribute atomov (emso)
	public void Save() {
		// gremo cez vse atome in dodamo njihov emso v seznam
		foreach (Transform group in transform.GetChild(0)) {
			foreach (Transform child in group) {
				AtomData atomData = new AtomData();
				atomData.emso = child.GetChild(0).GetComponent<AtomPodatki>().emso;
				atomDataList.Add(atomData);
			}
		}
		string json = JsonUtility.ToJson(atomDataList); // serializiramo seznam
		File.WriteAllText(path, json); // zapisemo v datoteko (funkcija sama zapre datoteko)

		// seznama ne potrebujemo vec zato ga spraznimo
		atomDataList.Clear();
	}

	public void Load() {
		if (File.Exists(path)) {
			string json = File.ReadAllText(path); // odpremo datoteko in preberemo datoteko (funkcija sama zapre datoteko)
			atomDataList = JsonUtility.FromJson<SerializableList<AtomData>>(json); // json string pretvotimo v seznam

			// gremo cez vse otroke in jim po vrsti dodeljujemo emso iz seznama
			foreach (Transform group in transform.GetChild(0)) {
				foreach (Transform child in group) {
					AtomData atomData = atomDataList.First();
					child.GetChild(0).GetComponent<AtomPodatki>().emso = atomData.emso;
					atomDataList.RemoveFirst();
				}
			}
		}
	}
}
