using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction_HumanShield: PlayerAction {
	bool setRot=false;
	NPCController hostage;
	//need to add way of reversing hostage taking,add behaviour to cops, guards to free hostages
	PersonMovementController pmc;
	PersonMovementController player_pmc;

	public bool wasHostageBefore = false;
	public bool doing=false;	
	public override void doAction()
	{
		illigal = true;		
		//this.transform.parent = CommonObjectsStore.player.transform;
		//this.gameObject.transform.rotation = Quaternion.Euler (0, 0, 0);

		if (hostage == null) {
			hostage = this.GetComponent<NPCController> ();

		}
		if (hostage.currentBehaviour == null) {

		} else {
			Destroy (hostage.currentBehaviour);
		}


		if (hostage.myHealth.healthValue <= 0) {
			onComplete ();
		} else {
			doing = true;
			if (hostage.npcB.myType != AIType.hostage) {
				hostage.npcB.myType = AIType.hostage;
				this.gameObject.tag = "NPC";
				hostage.detect.fov.viewMeshFilter.gameObject.SetActive (false);

				Inventory i = this.gameObject.GetComponent<Inventory> ();
				List<Item> items = i.inventoryItems;
				foreach (Item it in items) {
					it.dropItem ();
				}
				i.inventoryItems.Clear ();
				//PersonAnimationController pac = this.gameObject.GetComponent<PersonAnimationController> ();
				//pac.animationsToPlay.Clear ();
				//pac.playAnimation ("TiedUp", false);
				//pac.playing = null;
				//pac.forceFinishCurrentAnim ();
				//hostage.head.gameObject.SetActive (false);
				//hostage.knockedDown = false;
				//	this.gameObject.GetComponent<Collider2D> ().isTrigger = true;
				PlayerActionUI.me.lastObjectSelected=null;
				this.gameObject.GetComponent<PersonColliderDecider>().setTrigger();

				PersonWeaponController pwc = this.gameObject.GetComponent<PersonWeaponController> ();
				if (pwc.currentWeapon == null) {

				} else {
					pwc.currentWeapon.dropItem ();
					pwc.currentWeapon = null;
				}

				pwc.enabled = false;
				pmc = this.gameObject.GetComponent<PersonMovementController> ();
				this.gameObject.transform.position = CommonObjectsStore.player.transform.position + (CommonObjectsStore.player.transform.up / 2);
				this.gameObject.transform.parent = CommonObjectsStore.player.transform;
				this.gameObject.transform.rotation = Quaternion.Euler (0, 0, CommonObjectsStore.player.transform.eulerAngles.z);
				player_pmc = CommonObjectsStore.player.GetComponent<PersonMovementController> ();
				this.gameObject.GetComponent<SpriteRenderer> ().sortingOrder = 4;
			} else {
				wasHostageBefore = true;

				hostage.npcB.myType = AIType.hostage;
				this.gameObject.tag = "NPC";
				hostage.detect.fov.viewMeshFilter.gameObject.SetActive (false);

				//Inventory i = this.gameObject.GetComponent<Inventory> ();
				//List<Item> items = i.inventoryItems;
				//foreach (Item it in items) {
				//	it.dropItem ();
				//}
				//i.inventoryItems.Clear ();
				////PersonAnimationController pac = this.gameObject.GetComponent<PersonAnimationController> ();
				//pac.animationsToPlay.Clear ();
				//pac.playAnimation ("TiedUp", false);
				//pac.playing = null;
				//pac.forceFinishCurrentAnim ();
				//hostage.head.gameObject.SetActive (false);
				//hostage.knockedDown = false;
				//this.gameObject.GetComponent<Collider2D> ().isTrigger = true;
				this.gameObject.GetComponent<PersonColliderDecider>().setTrigger();

				PersonWeaponController pwc = this.gameObject.GetComponent<PersonWeaponController> ();
				if (pwc.currentWeapon == null) {

				} else {
					pwc.currentWeapon.dropItem ();
					pwc.currentWeapon = null;
				}
				pwc.enabled = false;
				pmc = this.gameObject.GetComponent<PersonMovementController> ();
				this.gameObject.transform.position = CommonObjectsStore.player.transform.position + (CommonObjectsStore.player.transform.up / 2);
				this.gameObject.transform.parent = CommonObjectsStore.player.transform;
				this.gameObject.transform.rotation = Quaternion.Euler (0, 0, CommonObjectsStore.player.transform.eulerAngles.z);
				player_pmc = CommonObjectsStore.player.GetComponent<PersonMovementController> ();
				this.gameObject.GetComponent<SpriteRenderer> ().sortingOrder = 4;
			}
			this.gameObject.transform.position = CommonObjectsStore.player.transform.position + (CommonObjectsStore.player.transform.up/2);
			this.gameObject.GetComponentInChildren<CircleCollider2D> ().enabled = false;

			pmc.movedThisFrame = player_pmc.movedThisFrame;
			hostage.ac.setHumanShield ();
			//pmc.legAnimate ();
			if (hostage.detect.fov.viewMeshFilter.gameObject.activeInHierarchy == true) {
				hostage.detect.fov.viewMeshFilter.gameObject.SetActive (false);
			}

			if (PlayerAction.currentAction != this && transform.parent != null) {
				PlayerAction.currentAction = this;
			}

		}

		//if (Input.GetKeyDown (KeyCode.E)) {
		//	onComplete ();
		//}

		//if (setRot == false) {
		//this.transform.position = Vector3.zero;
		//this.gameObject.transform.position = CommonObjectsStore.player.transform.position - Vector3.up;

		//	setRot = true;
		//}
	}



	public override bool canDo ()
	{
		if (hostage == null) {
			hostage = this.GetComponent<NPCController> ();
		}

		if (hostage.knockedDown == true || hostage.myHealth.healthValue<=0) {
			return false;
		}

		if (PlayerAction.currentAction == null) {
			if (Vector3.Distance (CommonObjectsStore.player.transform.position, this.transform.position) < 2.5f) {
				return true;
			} else {
				return false;
			}
		} else {
			if (PlayerAction.currentAction.getType()!= "Move Body" && PlayerAction.currentAction.getType () != getType () && Vector3.Distance (CommonObjectsStore.player.transform.position, this.transform.position) < 2.5f) {
				return true;
			} else {
				return false;
			}
		}
	}

	public override void onComplete()
	{
		//hostage.untieHostage ();
		//hostage.knockOutNPC ();
		doing = false;
		//hostage.npcB.myType = hostage.myType;
		this.transform.parent = null;
		PlayerAction.currentAction = null;
	}

	void Update()
	{
		if (doing == true) {
			if (hostage.myHealth.healthValue <= 0){
				onComplete ();
			}

			if (PlayerAction.currentAction == null) {
				PlayerAction.currentAction = this;
				this.gameObject.transform.parent = null;
				this.gameObject.transform.position = CommonObjectsStore.player.transform.position + (CommonObjectsStore.player.transform.up / 2);
				this.gameObject.transform.parent = CommonObjectsStore.player.transform;
				//Debug.LogError ("resetting human shield action");
			} else if (PlayerAction.currentAction.getType () == "Release Hostage") {
				onComplete ();
			}
		}
	}

	public override float getMoveModForAction ()
	{
		return -0.5f;
	}

	public override float getRotationModForAction ()
	{
		return 0.5f;
	}

	public override string getType()
	{
		return "Take Hostage";
	}

	public override string getDescription()
	{
		return "Take this person as a human shield.";
	}
}