using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction_TieUpHostage : PlayerAction {
	bool setRot=false;
	NPCController hostage;
	void Awake()
	{
		Destroy(this);
	}

	//need to add way of reversing hostage taking,add behaviour to cops, guards to free hostages
	public override void doAction()
	{
		illigal = true;		
		//this.transform.parent = CommonObjectsStore.player.transform;
		//this.gameObject.transform.rotation = Quaternion.Euler (0, 0, 0);
		PlayerActionUI.me.lastObjectSelected=null;
		if (hostage == null) {
			hostage = this.GetComponent<NPCController> ();
			if (hostage.currentBehaviour == null) {

			} else {
				Destroy (hostage.currentBehaviour);
			}
		}

		foreach (GameObject g in NPCManager.me.npcsInWorld) {
			if (g == null) {
				continue;
			}
			NPCController npc = g.GetComponent<NPCController> ();
			npc.setHearedGunshot (this.transform.position, 3.0f);
		}

		Item i = Inventory.playerInventory.getItem ("Cable Ties");
		Inventory.playerInventory.removeItemWithoutDrop (i);
		if (hostage.npcB.myType != AIType.hostage) {
			hostage.pwc.dropWeapon ();
			hostage.npcB.myType = AIType.hostage;
			this.gameObject.tag = "NPC";
			hostage.detect.fov.viewMeshFilter.gameObject.SetActive (false);
			//hostage.gameObject.GetComponent<SpriteRenderer>().sprite = hostage.ac.myAesthetic.tiedLying;
			hostage.ac.setTied();
			//PersonAnimationController pac = this.gameObject.GetComponent<PersonAnimationController> ();
			//pac.animationsToPlay.Clear ();
			//pac.playAnimation ("TiedUp", true);
			//pac.playing = null;
			//pac.forceFinishCurrentAnim ();
			//hostage.head.gameObject.SetActive (false);
			//hostage.knockedDown = false;
			this.gameObject.GetComponent<Collider2D> ().enabled = true;
			this.gameObject.GetComponent<PersonWeaponController> ().enabled = true;
			this.gameObject.GetComponent<PersonMovementController> ().enabled = true;
		}

		if (hostage.detect.fov.viewMeshFilter.gameObject.activeInHierarchy == true) {
			hostage.detect.fov.viewMeshFilter.gameObject.SetActive (false);
		}
		this.gameObject.GetComponentInChildren<CircleCollider2D> ().enabled = true;

		//this.gameObject.AddComponent<PlayerAction_DragCorpse> ();//will probbably have to add some specific hostage order script
		PlayerAction.currentAction=null;
		PlayerActionUI.me.lastObjectSelected = null;

		//if (setRot == false) {
		//this.transform.position = Vector3.zero;
		//this.gameObject.transform.position = CommonObjectsStore.player.transform.position - Vector3.up;

		//	setRot = true;
		//}
	}

	public override bool canDo ()
	{
		if (hostage == null) {
			hostage = this.gameObject.GetComponent<NPCController> ();
		}

		if (Inventory.playerInventory.doWeHaveItem ("Cable Ties") == false) {
			return false;
		}

		if (hostage.knockedDown == true) {
			if (Vector3.Distance (this.transform.position, CommonObjectsStore.player.transform.position) < 2) {
				return true;
			} else {
				return false;
			}
		} else {
			return false;
		}
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

	public override string getType()
	{
		return "Tie Up Hostage";
	}

	public override string getDescription()
	{
		return "Tie up a knocked down person, requires cable ties.";
	}
}
