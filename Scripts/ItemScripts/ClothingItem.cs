using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothingItem  : Item {
	public Sprite head,torso,lShoulder,lFore,lHand,rShoulder,rFore,rHand,lThigh,lCalf,lFoot,rThigh,rCalf,rFoot,lbThigh,lbCalf,lbFoot,rbThigh,rbCalf,rbFoot,torFall;
	public Sprite lHandFist,lHandTrigger,lHandHold, rHandFist,rHandTrigger,rHandHold;
	public string clothingName = "";
	public List<ClothingItemSlot> slotsITakeUp;
	public override void equipItem ()
	{
		PersonClothesController pcc = this.gameObject.GetComponentInParent<PersonClothesController> ();
		pcc.equipClothing (this);
		this.GetComponent<SpriteRenderer> ().enabled = false;
		this.gameObject.SetActive (false);
	}

	public override void unequipItem ()
	{
		PersonClothesController pcc = this.gameObject.GetComponentInParent<PersonClothesController> ();
		if (pcc == null) {

		} else {
			pcc.removeClothing (this);

		}
		inUse = false;
	}

	public void dropItem()
	{
		if (inUse == true) {
			unequipItem ();
		}
		PersonClothesController pcc = this.gameObject.GetComponentInParent<PersonClothesController> ();
		pcc.removeClothing (this);
		Inventory i = this.gameObject.GetComponentInParent<Inventory> ();
		i.dropItem (this);
		this.GetComponent<SpriteRenderer> ().enabled = true;
	}
}

public enum ClothingItemSlot
{
	head,
	face,
	torso,
	leftShoulder,
	leftFore,
	leftHand,
	rightShoulder,
	rightFore,
	rightHand,
	leftThigh,
	leftCalf,
	leftFoot,
	rightThigh,
	rightCalf,
	rightFoot
}