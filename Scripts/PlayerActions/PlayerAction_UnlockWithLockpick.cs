using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction_UnlockWithLockpick : PlayerAction {
	ProgressBar myProgress;
	DoorScript myDoor;
	float timer = 3.0f;
	bool doing=false;
	//need to add way of reversing hostage taking,add behaviour to cops, guards to free hostages
	public override void doAction()
	{
		illigal = true;		

		if (canDo () == true) {
			doing = true;
		} else {
			doing = false;
		}

	}

	void Update()
	{
		if (doing == true) {
			if (Vector3.Distance (this.transform.position, CommonObjectsStore.player.transform.position) <= 1.5f) {
				if (myProgress == null) {
					createProgressBar ();
				}

				if (timer > 0) {
					timer -= Time.deltaTime;
					myProgress.currentValue = (myProgress.maxValue - timer);
				} else {
					myProgress.currentValue = myProgress.maxValue;
					myDoor.locked = false;
					Destroy (myProgress.gameObject);
					PlayerAction.currentAction = null;
					this.enabled = false;
				}
			} else {
				timer = 3.0f;
				doing = false;
				if (myProgress == null) {

				} else {
					myProgress.currentValue = (myProgress.maxValue - timer);
					Destroy (myProgress.gameObject);
					PlayerAction.currentAction = null;
				}
			}
		} else {
			if (myProgress == null) {

			} else {
				myProgress.currentValue = (myProgress.maxValue - timer);
				Destroy (myProgress.gameObject);
				PlayerAction.currentAction = null;
			}
		}

	}

	public override bool canDo ()
	{
		if (myDoor == null) {
			myDoor = this.gameObject.GetComponent<DoorScript> ();
		}

		if (myDoor == null) {
			return false;
		}

		if (myDoor.locked == false) {
			Destroy (this);
		}



		if (Inventory.playerInventory.doWeHaveItem ("Lockpick") == true && Vector3.Distance (this.transform.position, CommonObjectsStore.player.transform.position) <= 1.5f && myDoor.locked==true && myDoor.wayIAmLocked==lockedWith.key) {
			return true;
		} else {
			if (myProgress == null) {

			} else {
				Destroy (myProgress.gameObject);
				timer = 3.0f;
				if (PlayerAction.currentAction == this) {
					PlayerAction.currentAction = null;

				}
			}

			return false;
		}
	}

	public void createProgressBar()
	{
		GameObject g = (GameObject)Instantiate (CommonObjectsStore.me.progressBar, Vector3.zero, Quaternion.Euler (0, 0, 0));
		g.transform.parent = GameObject.FindGameObjectWithTag ("MainCamera").GetComponentInChildren<Canvas> ().gameObject.transform;
		myProgress = g.GetComponent<ProgressBar> ();
		myProgress.maxValue = timer;
		myProgress.myObjectToFollow = this.gameObject;
		g.transform.localScale = new Vector3 (1, 1, 1);
	}

	public override void onComplete()
	{
		//this.transform.parent = null;
	}

	public override string getType ()
	{
		return "Unlock with lockpick";
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
		return "Open locked door with a lockpick.";
	}
}
