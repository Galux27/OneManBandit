using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction_ReleaseHostage : PlayerAction {
	bool setRot=false;
	NPCController hostage;
	//need to add way of reversing hostage taking,add behaviour to cops, guards to free hostages
	PersonMovementController pmc;
	PersonMovementController player_pmc;
	PersonHealth ph;
	SpriteRenderer sr;
	public override void doAction()
	{
		illigal = false;		
		//this.transform.parent = CommonObjectsStore.player.transform;
		//this.gameObject.transform.rotation = Quaternion.Euler (0, 0, 0);

		if (hostage == null) {
			hostage = this.GetComponent<NPCController> ();
		}

		onComplete ();

	}

	public override bool canDo ()
	{
		if (ph == null) {
			ph = this.gameObject.GetComponent<PersonHealth> ();
		}


		if (ph.healthValue <= 0) {
			return false;
		}

		if (PlayerAction.currentAction == null) {
			return false;
		} else {
			if (PlayerAction.currentAction == null) {
				return false;
			}

			if (this.transform.parent == null) {
				return false;
			}

			if (this.gameObject.tag == "Dead/Knocked") {
				return false;
			}

			if (PlayerAction.currentAction.getType () == "Take Hostage" && this.transform.root.gameObject.tag=="Player") {
				return true;
			} else {
				return false;
			}
		}
	}

	public override void onComplete()
	{
		if (PlayerAction.currentAction.getType () == "Take Hostage" && this.transform.root.gameObject.tag=="Player") {
			PlayerAction_HumanShield pa = (PlayerAction_HumanShield)PlayerAction.currentAction;
			pa.doing = false;
			pa.onComplete ();
		} 
		hostage.gameObject.transform.parent = null;
		PlayerAction.currentAction = null;
		if (hostage.tied == false) {
			hostage.untieHostage ();
			hostage.memory.objectThatMadeMeSuspisious = CommonObjectsStore.player;
			hostage.npcB.alarmed = true;
			hostage.GetComponent<NPCBehaviourDecider> ().onHostageRelease ();
			hostage.ac.releaseHumanShield ();
		} else {
			Collider2D[] cols = hostage.gameObject.GetComponents<Collider2D> ();
			hostage.ac.setTied ();
			foreach(Collider2D c in cols)
			{
				c.isTrigger = false;
			}
		}
		PlayerActionUI.me.lastObjectSelected = null;
		//hostage.knockOutNPC ();
		//hostage.npcB.myType = hostage.myType;

	}

	public override float getMoveModForAction ()
	{
		return 0.0f;
	}

	public override float getRotationModForAction ()
	{
		return 0.0f;
	}

	public override string getType()
	{
		return "Release Hostage";
	}

	public override string getDescription()
	{
		return "Releases current human shield, will not untie them.";
	}
}