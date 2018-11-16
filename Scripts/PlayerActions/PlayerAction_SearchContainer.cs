using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction_SearchContainer  : PlayerAction {
	Container myContainer;
	public override void doAction()
	{
		illigal = true;		

		//if (myContainer.open == false) {
			myContainer.actionOpenContainer ();
		//}
		InventorySwitch.me.enable ();

		if (Inventory.playerInventory.inventoryGUI.activeInHierarchy == false) {
			onComplete ();
		}

	}

	public override bool canDo ()
	{
		if (myContainer == null) {
			myContainer = this.GetComponent<Container> ();
		}
		if (this.GetComponent<PortableContainerItem> () == null) {
			if (Vector3.Distance (this.transform.position, CommonObjectsStore.player.transform.position) < 2.5f) {
				return true;
			} else {
				return false;
			}
		} else if (this.transform.parent!=null && this.transform.parent.tag == "Car") {
			if (PlayerCarController.inCar == false) {
				if (Vector3.Distance (this.transform.position, CommonObjectsStore.player.transform.position) < 2.5f) {
					return true;
				} else {
					return false;
				}
			} else {
				return false;
			}
		}else{
			if (Vector3.Distance (this.transform.position, CommonObjectsStore.player.transform.position) < 2.5f && transform.parent == null) {
				return true;
			} else {
				return false;
			}
		}

	}

	public override void onComplete()
	{
		myContainer.closeContainer ();
		PlayerAction.currentAction = null;
		PlayerActionUI.me.lastObjectSelected = null;
	}

	public override float getMoveModForAction ()
	{
		return -0.65f;
	}

	public override float getRotationModForAction ()
	{
		return -0.65f;
	}

	public override string getType()
	{
		return "Search Container";
	}

	public override string getDescription()
	{
		return "Search through the selected container.";
	}
}
