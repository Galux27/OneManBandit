using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction_OpenWithKeycard : PlayerAction {
	DoorScript myDoor;
	//need to add way of reversing hostage taking,add behaviour to cops, guards to free hostages
	public override void doAction()
	{
		illigal = false;		

		myDoor.locked = false;
		myDoor.interactWithDoor (CommonObjectsStore.player);
		PlayerAction.currentAction = null;
	}

	public override bool canDo ()
	{
		if (myDoor == null) {
			myDoor = this.GetComponent<DoorScript> ();
		}
		if (myDoor.locked == false) {
			//Destroy (this);
			return false;

		}
		//will need to add some condition for checking the security clearance of the keycard. will need to create a new item child class for keycards
		if (Vector3.Distance (this.transform.position, CommonObjectsStore.player.transform.position) <2 && Inventory.playerInventory.doWeHaveItem(myDoor.myKey) && myDoor.locked==true) {
			return true;
		} else {
			return false;
		}
	}

	public override string getType ()
	{
		return "Unlock with keycard";
	}

	public override void onComplete()
	{
		//this.transform.parent = null;
	}

	public override float getMoveModForAction ()
	{
		return -0.0f;
	}

	public override float getRotationModForAction ()
	{
		return -0.0f;
	}

	public override string getDescription()
	{
		return "Open locked door with keycard.";
	}
}