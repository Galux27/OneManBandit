using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BloodbagItem :Item {

	public override void useItem ()
	{

		if (canWeUseBloodBag () == false) {
			return;
		}

		Inventory inventoryIAmIn = this.gameObject.GetComponentInParent<Inventory> ();
		if (myContainer == null) {
		} else {
			myContainer.removeItemFromContainer (this);
		}


		if (inventoryIAmIn == null) {

		} else {
			inventoryIAmIn.removeItemWithoutDrop (this);
		}

		dropItem ();
		CommonObjectsStore.player.GetComponent<PersonHealth> ().healthValue = 5200.0f;

		TimeScript.me.bloodBagSkip ();
		Destroy (this.gameObject);

	}

	bool canWeUseBloodBag()
	{
		if (PoliceController.me.copsHere == true || PoliceController.me.backupHere == true || PoliceController.me.swatHere == true) {
			return false;
		}

		foreach (NPCController npc in NPCManager.me.npcControllers) {
			if (npc.npcB.alarmed == true) {
				return false;
			}
		}

		return true;
	}
}
