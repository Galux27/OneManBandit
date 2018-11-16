using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction_OpenDoorWithExplosives : PlayerAction {
	DoorScript myDoor;
	public bool explosivePlaced = false;
	public float timer = 5.0f;


	//need to add way of reversing hostage taking,add behaviour to cops, guards to free hostages
	public override void doAction()
	{
		illigal = true;		

		if (explosivePlaced == false) {
			Inventory.playerInventory.removeItemWithoutDrop (Inventory.playerInventory.getItem ("Explosive"));
			this.gameObject.GetComponent<AudioController> ().playSound (SFXDatabase.me.explosiveBeep);
			explosivePlaced = true;
		}
	}

	void Update(){
		if (explosivePlaced == true) {
			timer -= Time.deltaTime;
			////////Debug.Log ("Explosive Beep");
			if (timer <= 0) {
				Instantiate (CommonObjectsStore.me.explosion, this.transform.position, this.transform.rotation);
				myDoor.locked = false;
				myDoor.interactWithDoor (CommonObjectsStore.player);
				myDoor.doorHealth = 0;
				myDoor.kickInDoor ();
				for (int x = 0; x < 4; x++) {
					Vector3 pos = new Vector3 (Random.Range (-1.0f, 1.0f), Random.Range (-1.0f, 1.0f), 0.0f);
					Instantiate (CommonObjectsStore.me.explosion, this.transform.position+pos, this.transform.rotation);

				}
				PlayerAction.currentAction = null;
				this.enabled = false;
				//create explosion
				foreach (GameObject g in NPCManager.me.npcsInWorld) {
					if (g == null) {
						continue;
					}
					NPCController npc = g.GetComponent<NPCController> ();
					npc.setHearedGunshot (this.transform.position, 50.0f);
				}
				PoliceController.me.setNoiseHeard (this.transform.position, 50.0f);

			//	Destroy (this.gameObject);
			}
		}
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
		if (explosivePlaced == false) {
			//////////Debug.Log ("Does player have explosives = " + Inventory.playerInventory.doWeHaveItem ("Explosive"));
			if (Vector3.Distance (this.transform.position, CommonObjectsStore.player.transform.position) < 2 && Inventory.playerInventory.doWeHaveItem ("Explosive")==true) {
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
		return "Destroy with explosives";
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
		return "Open locked door with explosives, requires explosives.";
	}
}