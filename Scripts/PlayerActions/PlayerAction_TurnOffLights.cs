using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction_TurnOffLights : PlayerAction {
	Lightswitch l;

	public override void doAction ()
	{
		l.turnOffLights ();
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
		return "Turn off lights";
	}

	public override string getType ()
	{
		return "Turn light off";
	}
}
