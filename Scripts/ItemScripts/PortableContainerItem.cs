using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortableContainerItem : Item {

	public override void equipItem ()
	{
		Inventory i = this.GetComponentInParent<Inventory> ();
		ArtemAnimationController ac = this.transform.root.GetComponentInChildren<ArtemAnimationController> ();
		this.transform.parent = ac.leftHand.transform;
		this.transform.localPosition = new Vector3(0,0,0);
		this.gameObject.SetActive (true);
		this.transform.rotation = ac.leftHand.transform.rotation;
		this.gameObject.GetComponent<SpriteRenderer> ().sortingOrder = ac.leftHand.gameObject.GetComponent<SpriteRenderer> ().sortingOrder - 1;
		if (this.GetComponent<PlayerAction_SearchContainer> () == false) {
			this.gameObject.AddComponent<PlayerAction_SearchContainer> ();
		}
		this.GetComponent<PlayerAction_SearchContainer> ().enabled = false;
		//i.addItemToInventory (this);
		//if (this.gameObject.activeInHierarchy == false) {
		//	i.equipItem (this, slot);
		////////Debug.Break();
			//this.transform.parent.gameObject.GetComponentInChildren<PortableContainerVisual> ().myItem = this;
		//}



	}

	void Update()
	{
		if (this.gameObject.activeInHierarchy == false) {
			this.gameObject.SetActive (true);
		}
	}
	//glitch with unequiping, recursion because of going through inventory UI, fix by moving unequip method to the Inventory and having the inventoryUI call that instead? 
	public override void unequipItem ()
	{
		this.transform.parent = null;
		this.GetComponent<PlayerAction_SearchContainer> ().enabled = true;

		//this.GetComponentInParent<Inventory> ().unequipItem (this);
		//this.transform.parent.gameObject.GetComponentInChildren<PortableContainerVisual> ().myItem = null;

		//dropItem ();
	}
}
