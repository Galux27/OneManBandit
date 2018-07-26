using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour_SwatFormUp: NPCBehaviour {
	public Transform exit;
	public override void Initialise ()
	{
		myType = behaviourType.formUp;
		myController = this.gameObject.GetComponent<NPCController> ();

		if (exit == null) {
			exit = PoliceController.me.swatFormUpPoint;
		}
		myController.pf.getPath (this.gameObject,exit.gameObject);

		if (myController.pwc.currentWeapon == null) {
			if (myController.inv.doWeHaveAWeaponWeCanUse () == true) {
				Weapon w = myController.inv.getWeaponWeCanUse ();
				w.equipItem ();
			}
		}

		isInitialised = true;
	}

	public override void OnUpdate ()
	{
		if (isInitialised == false) {
			Initialise ();
		}

		if (areWeAtPosition () == false) {
			moveToPosition ();
		} else {
			//Destroy (this.gameObject);

		}
	}

	bool areWeAtPosition()
	{
		if (Vector3.Distance (this.transform.position, exit.position) > 2.0f) {
			return false;
		} else {
			return true;
		}
	}

	void moveToPosition(){
		myController.pmc.rotateToFacePosition (myController.pf.getCurrentPoint());
		if (areWeAtPosition()==false) {
			myController.pmc.moveToDirection (myController.pf.getCurrentPoint());
		}
	}

	public override void OnComplete ()
	{

	}
}