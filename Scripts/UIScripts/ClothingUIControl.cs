using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that controls the switching between the clothing and equipment UIs 
/// </summary>
public class ClothingUIControl : MonoBehaviour {
	public GameObject clothingParent,equipmentParent;
	public Text disp;
	public List<ClothingItemUI> uis;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		makeSureUIisCorrect ();
		if (clothingParent.activeInHierarchy == true) {
			foreach (ClothingItemUI cui in uis) {
				cui.resetGearSlot ();

			}

			foreach (ClothingItem cl in PersonClothesController.playerClothes.clothesBeingWorn) {
				foreach (ClothingItemUI cui in uis) {
					if (cl.slotsITakeUp.Contains (cui.mySlot)==true && cui.itemEquiped!=cl) {
						cui.setItem (cl);
					}
				}
			}
		}
	}

	void makeSureUIisCorrect()
	{
		if (clothingParent.activeInHierarchy == true && equipmentParent.activeInHierarchy == true) {
			clothingParent.SetActive (false);
		}

		if (clothingParent.activeInHierarchy == true) {
			disp.text = "Switch to Equipment";
		} else {
			disp.text = "Switch to Clothes";
		}
	}

	public void buttonPress()
	{
		if (clothingParent.activeInHierarchy == false) {
			disp.text = "Switch to Clothes";
			clothingParent.SetActive (true);
			equipmentParent.SetActive (false);
		} else {
			disp.text = "Switch to Equipment";
			clothingParent.SetActive (false);
			equipmentParent.SetActive (true);
		}
	}
}
