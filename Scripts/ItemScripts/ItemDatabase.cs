using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour {
	/// <summary>
	/// List of all items in the game, used to look up items when creating new instances at runtime. 
	/// </summary>
	public static ItemDatabase me;
	public GameObject[] items;
	void Awake()
	{
		me = this;
	}

	public GameObject getItem(string name)
	{
		foreach (GameObject g in items) {
			Item i = g.GetComponent<Item> ();
			if (i == null) {
				continue;
			}else{
				if (i.itemName == name) {
					return g;
				}
			}
		}
		return null;
	}

	public GameObject getAmmoItem(Weapon w)
	{
		foreach (GameObject g in items) {
			AmmoItem ai = g.GetComponent<AmmoItem> ();
			if (ai == null) {

			} else {
				if (ai.canWeUseAmmoInWeapon (w.itemName) == true) {
					return g;
				}
			}
		}
		return null;
	}
}
