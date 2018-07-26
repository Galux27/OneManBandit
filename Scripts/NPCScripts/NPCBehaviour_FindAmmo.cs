using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour_FindAmmo : NPCBehaviour {

	public GameObject target;
	bool triedToFindAmmo = false;
	public override void Initialise ()
	{
		myType = behaviourType.getAmmo;
		myController = this.GetComponent<NPCController> ();
		//getAmmo ();
		isInitialised = true;
	}

	public override void OnUpdate ()
	{
		if (isInitialised == false) {
			Initialise ();
		}

		if (triedToFindAmmo == false) {
			getAmmo ();
			triedToFindAmmo = true;
		}

		if (target == null) {
			noAmmo ();
		} else {
			goToAmmo ();
		}
	}

	void noAmmo()
	{
		NPCBehaviour_FindGear newBehaviour = this.gameObject.AddComponent<NPCBehaviour_FindGear> ();
		myController.currentBehaviour = newBehaviour;
		//////Debug.Log ("No ammo,dropping weapon");
		myController.inv.unequipItem (myController.pwc.currentWeapon.ammoItem);
		myController.inv.dropItem (myController.pwc.currentWeapon.ammoItem);

		//Item i = myController.pwc.currentWeapon;
		//i.unequipItem ();
		//i.dropItem ();
		myController.pwc.dropWeapon();
		//myController.inv.dropItem (myController.pwc.currentWeapon);
		//myController.inv.unequipItem (myController.pwc.currentWeapon);

		Destroy (this);
	}

	void getAmmo()
	{
		AmmoItem ai = myController.inv.getAmmoForGun (myController.pwc.currentWeapon.WeaponName);
		if (ai == null) {
			target = ItemMoniter.me.getAmmoForWeapon (myController.pwc.currentWeapon, this.transform.position);
		} else {
			target = ai.gameObject;
		}

		if (ai == null) {

		} else {
			//////Debug.Log ("DROPPING EMPTY MAG");
			myController.inv.dropItem (myController.pwc.currentWeapon.ammoItem);
		}
	}

	void goToAmmo()
	{

		if (myController.pf.followPath == false) {
			myController.pf.getPath (this.gameObject, target);
		}

		myController.pmc.rotateToFacePosition (myController.pf.getCurrentPoint());
		myController.pmc.moveToDirection (myController.pf.getCurrentPoint());

		//myController.pmc.rotateToFacePosition (target.transform.position);
		if (Vector3.Distance (this.transform.position, target.transform.position) > 1.0f) {
			//myController.pmc.moveToDirection (target.transform.position);
		} else {
			Item i = target.GetComponent<Item> ();
			myController.inv.addItemToInventory (i);
			myController.pwc.reloadTimer = myController.pwc.currentWeapon.reloadTime;
			myController.pwc.reloading = true;
		}
	}



	public override void OnComplete ()
	{
		NPCBehaviour rep = this.gameObject.AddComponent<NPCBehaviour_FollowObject> ();
		rep.passInGameobject (CommonObjectsStore.player);
		myController.currentBehaviour = rep;
		Destroy (this);
	}
}
