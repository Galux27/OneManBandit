using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction_UseComputer : PlayerAction {

	public override void doAction()
	{
		if (ComputerUIControl.me.computerBeingUsed == false) {
			if (PhoneController.me.phoneOpen () == true) {
				PhoneController.me.openPhone ();
			}
			ComputerUIControl.me.interactWithComputer ();
		}

		if (ComputerUIControl.me.computerBeingUsed == false) {
			onComplete ();
		}
	}

	public override bool canDo ()
	{
		if (Vector3.Distance (this.transform.position, CommonObjectsStore.player.transform.position) <= 2.0f && PlayerAction.currentAction==null) {
			return true;
		}

		return false;
	}

	public override void onComplete()
	{
		PlayerActionUI.me.lastObjectSelected = null;

		PlayerAction.currentAction = null;
	}

	public override float getMoveModForAction ()
	{
		return -0.5f;
	}

	public override float getRotationModForAction ()
	{
		return -0.5f;
	}

	public override string getType()
	{
		return "Use Computer";
	}

	public override string getDescription()
	{
		return "Use the selected computer.";
	}
}
