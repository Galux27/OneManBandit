using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction_InteractWithDoor : PlayerAction {
	DoorScript myDoor;
	//need to add way of reversing hostage taking,add behaviour to cops, guards to free hostages
	public override void doAction()
	{

		myDoor.interactWithDoor(CommonObjectsStore.player);

		foreach (GameObject g in NPCManager.me.npcsInWorld) {
			if (g == null) {
				continue;
			}
			NPCController npc = g.GetComponent<NPCController> ();
			if (npc.npcB.myType != AIType.civilian) {
				npc.setHearedGunshot (this.transform.position, 4.0f);
			}
		}
		PoliceController.me.setNoiseHeard (this.transform.position, 4.0f);

		PlayerAction.currentAction = null;
	}

	public override bool canDo ()
	{
		illigal = false;		

		if (myDoor == null) {
			myDoor = this.GetComponent<DoorScript> ();
		}

		if (myDoor.locked == false) {
			if (Vector3.Distance (this.transform.position, CommonObjectsStore.player.transform.position) < 2) {
				return true;
			} else {
				return false;
			}
		} else {
			return false;

		}
	}

	public override string getType ()
	{
		return "Interact with door";
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
		return "Open an unlocked door.";
	}
}

