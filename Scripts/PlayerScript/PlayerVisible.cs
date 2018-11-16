using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisible : MonoBehaviour {
	public static PlayerVisible me;
	PersonClothesController pcc;

	void Awake()
	{
		me = this;
		pcc = this.GetComponent<PersonClothesController> ();
	}

	public bool isPlayersFaceHidden()
	{
		if (areWeWearingClothes ("Balaclava") == true) {
			return true;
		}

		return false;
	}

	public bool areHandsCovered()
	{
		if (areWeWearingClothes ("Gloves") == true) {
			return true;
		}
		return false;
	}

	bool areWeWearingClothes(string st)
	{
		foreach (ClothingItem c in pcc.clothesBeingWorn) {
			if (c.itemName == st) {
				return true;
			}
		}
		return false;
	}
}
