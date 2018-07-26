using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction_KickInDoor  : PlayerAction {
	ProgressBar myProgress;
	DoorScript myDoor;
	float timer = 3.0f;
	//need to add way of reversing hostage taking,add behaviour to cops, guards to free hostages
	public override void doAction()
	{
		illigal = true;		

		myDoor.locked = false;
		//myDoor.interactWithDoor (CommonObjectsStore.player);
		myDoor.kickInDoor();
		CommonObjectsStore.player.GetComponent<PersonWeaponController> ().Punch ();
		foreach (GameObject g in NPCManager.me.npcsInWorld) {
			if (g == null) {
				continue;
			}
			NPCController npc = g.GetComponent<NPCController> ();
			npc.setHearedGunshot (this.transform.position, 7.0f);
		}
		PoliceController.me.setNoiseHeard (this.transform.position, 7.0f);

		PlayerAction.currentAction = null;
	}

	public override bool canDo ()
	{
		return false;
		if (myDoor == null) {
			myDoor = this.GetComponent<DoorScript> ();
		}
		if (Vector3.Distance (this.transform.position, CommonObjectsStore.player.transform.position) <2) {
			return true;
		} else {
			return false;
		}
	}

	public override string getType ()
	{
		return "Break Door";
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
}
