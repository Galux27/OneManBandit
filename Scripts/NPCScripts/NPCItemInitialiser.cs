using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCItemInitialiser : MonoBehaviour {

	/// <summary>
	/// Class that adds items to an NPC at the start of the game. 
	/// </summary>

	NPCController myController;

	public string weaponToGet;
	public List<string> itemsToAdd,randomItems;

	void Awake()
	{
		myController = this.GetComponent<NPCController> ();
	}

	void Start () {
		createItems ();
	}


	void createItems()
	{
		if (ItemDatabase.me.getItem (weaponToGet) == null) {
		} else {
			GameObject pistolObj = (GameObject)Instantiate (ItemDatabase.me.getItem (weaponToGet), this.transform.position, this.transform.rotation);
			Weapon w = pistolObj.GetComponent<Weapon> ();
			if (w.melee == false) {
				GameObject ammoObj = (GameObject)Instantiate (ItemDatabase.me.getAmmoItem (w), this.transform.position, this.transform.rotation);
				AmmoItem ai = ammoObj.GetComponent<AmmoItem> ();
				myController.inv.addItemToInventory (ai);
			}
			myController.inv.addItemToInventory (w);
		}

		foreach (string st in itemsToAdd) {
			if (ItemDatabase.me.getItem (st) == null) {

			} else {
				GameObject obj = (GameObject)Instantiate (ItemDatabase.me.getItem (st), this.transform.position, this.transform.rotation);
				Item i = obj.GetComponent<Item> ();
				myController.inv.addItemToInventory (i);
			}
		}

		foreach (string st in randomItems) {
			if (ItemDatabase.me.getItem (st) == null) {

			} else {
				int r = Random.Range (0, 100);
				if (r < 50) {
					GameObject obj = (GameObject)Instantiate (ItemDatabase.me.getItem (st), this.transform.position, this.transform.rotation);
					Item i = obj.GetComponent<Item> ();
					myController.inv.addItemToInventory (i);
				}
			}
		}
	}
}
