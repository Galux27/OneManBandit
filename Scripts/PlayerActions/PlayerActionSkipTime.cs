using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionSkipTime : PlayerAction {
	

	public override void doAction()
	{
		TimeSkipUI.me.enableUI ();
	}

	public override bool canDo ()
	{
		if (PoliceController.me.copsHere == true || PoliceController.me.backupHere == true || PoliceController.me.swatHere == true) {
			return false;
		}
		return true;
	}

	public override void onComplete()
	{

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
		return "Pass Time";
	}

	public override string getDescription()
	{
		return "Passes Time";
	}
}
