using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ClothingUIControl : MonoBehaviour {
	public GameObject clothingParent,equipmentParent;
	public Text disp;
	//public ClothingItemUI head,face,torso,lShol,rShol,lFore,rFore,lHand,rHand,lThigh,rThigh,lCalf,rCalf,lFoot,rFoot;
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
