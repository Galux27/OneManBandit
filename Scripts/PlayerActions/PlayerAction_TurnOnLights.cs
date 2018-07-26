using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction_TurnOnLights : PlayerAction {
	Lightswitch l;

	public override void doAction ()
	{
		l.turnOnLights ();
		this.gameObject.GetComponent<AudioController> ().playSound (SFXDatabase.me.flickSwitch);

		PlayerActionUI.me.lastObjectSelected = null;
		PlayerAction.currentAction = null;

	}

	public override bool canDo ()
	{
		l = this.gameObject.GetComponent<Lightswitch> ();



		if (Vector3.Distance (this.transform.position, CommonObjectsStore.player.transform.position) < 2) {
			return true;
		} else {
			return false;
		}
	}

	public override string getDescription ()
	{
		return "Turn on lights";
	}

	public override string getType ()
	{
		return "Turn light on";
	}
}
