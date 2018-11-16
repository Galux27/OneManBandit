using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour_FindGear : NPCBehaviour {

	public GameObject target;
	public List<GameObject> toPickUp;
	public override void Initialise ()
	{
		myType = behaviourType.getWeapon;
		myController = this.GetComponent<NPCController> ();
		toPickUp = ItemMoniter.me.findAWeapon (this.gameObject.transform.position);
		isInitialised = true;
	}

	public override void OnUpdate ()
	{
		if (isInitialised == false) {
			////////Debug.Log ("LOOKING FOR GEAR");
			Initialise ();
		}

		if (toPickUp.Count == 0 || toPickUp==null) {
			////////Debug.Log ("No weapon, attacking with bear hands");
			//could not find a weapon
			NPCBehaviour_AttackTarget newBehaviour = this.gameObject.AddComponent<NPCBehaviour_AttackTarget>();
			myController.currentBehaviour = newBehaviour;
			myController.currentBehaviour.passInGameobject (CommonObjectsStore.player);//need to find some way of keeping track of who an enemy is targeting outside of the behaviours, maybe work it into the suspision system
			myController.skipAiCheckOnFrame=true;
			Destroy (this);
			////////Debug.Break ();
		} else {
			if (toPickUp [0].activeInHierarchy == true) {
				goToWeapon ();
			} else {
				if (myController.pwc.currentWeapon == null) {
					//should probably base this on whether they are near to the target
					//also need to work out why sometimes a weapon can be duplicated 
					toPickUp = ItemMoniter.me.findAWeapon (this.gameObject.transform.position);
				} else {
					if (myController.inv.getAmmoForGun (myController.pwc.currentWeapon.WeaponName) == false) {
						if (toPickUp [1].activeInHierarchy == true) {
							goToAmmo ();
						} else {
							OnComplete ();
						}
					} else {
						OnComplete ();
					}
				}
			}
		}
	}

	void goToWeapon()
	{
		//if (target == null) {
		//	toPickUp = ItemMoniter.me.findAWeapon (this.gameObject.transform.position);

		//} else {
			

			target = toPickUp [0];

		if (target.activeInHierarchy == false) {
			////////Debug.LogError ("Weapon was null,finding new one");
			toPickUp = ItemMoniter.me.findAWeapon (this.gameObject.transform.position);
		} else
		{
			//myController.pmc.rotateToFacePosition (target.transform.position);
			if (myController.pf.followPath == false) {
				myController.pf.getPath (this.gameObject, target);
			}

			myController.pmc.rotateToFacePosition (myController.pf.getCurrentPoint());
			myController.pmc.moveToDirection (myController.pf.getCurrentPoint());

			if (Vector3.Distance (this.transform.position, target.transform.position) > 1.0f) {
				//myController.pmc.moveToDirection (target.transform.position);
			} else {
				
				Item i = target.GetComponent<Item> ();
				myController.inv.addItemToInventory (i);

				//if (myController.myType == AIType.aggressive) {
					myController.inv.equipItem (i, i.slot);
					myController.pwc.setWeapon (target.GetComponent<Weapon> ());
				//}
			}
		}
	}

	void goToAmmo()
	{
		
		if (myController.inv.getAmmoForGun (myController.pwc.currentWeapon.WeaponName) == null) {
			target = toPickUp [1];

			if (myController.pf.followPath == false) {
				myController.pf.getPath (this.gameObject, target);
			}

			myController.pmc.rotateToFacePosition (myController.pf.getCurrentPoint());
			myController.pmc.moveToDirection (myController.pf.getCurrentPoint());

			//myController.pmc.rotateToFacePosition (target.transform.position);
			if (Vector3.Distance (this.transform.position, target.transform.position) > 1.0f) {
				//myController.pmc.moveToDirection (target.transform.position);
			} else {
				myController.inv.addItemToInventory (target.GetComponent<Item> ());
			}
		} else {
			OnComplete ();
		}
	}

	public override void OnComplete ()
	{
		//NPCBehaviour rep = this.gameObject.AddComponent<NPCBehaviour_FollowObject> ();
		//rep.passInGameobject (CommonObjectsStore.player);
		//myController.currentBehaviour = rep;
		myController.currentBehaviour=null;
		Destroy (this);
	}


}
