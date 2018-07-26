using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction_DropWeapon  : PlayerAction {
	NPCController npcToDrop;
	PersonWeaponController playerWeapon;
	//need to add way of reversing hostage taking,add behaviour to cops, guards to free hostages
	public override void doAction()
	{
		illigal = true;		

		npcToDrop.pwc.dropWeapon ();
		npcToDrop.inv.dropItem (npcToDrop.pwc.currentWeapon);
		npcToDrop.pwc.currentWeapon = null;

		PlayerAction.currentAction=null;

	}

	public override bool canDo ()
	{
		if (npcToDrop == null) {
			npcToDrop = this.gameObject.GetComponent<NPCController> ();
		}

		if (playerWeapon == null) {
			playerWeapon = CommonObjectsStore.player.GetComponent<PersonWeaponController> ();
		}

		if (npcToDrop.pwc.currentWeapon == null) {
			return false;
		} else {
			if (playerWeapon.currentWeapon == null) {
				return false;
			} else if (playerWeapon.currentWeapon.melee == true) {
				return false;
			} else {
				return true;
			}

		}
		return false;
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
		return "Make Drop Weapon";
	}

	public override string getDescription()
	{
		return "Gets NPC to drop their weapon, only works when the player has a hostage.";
	}
}
