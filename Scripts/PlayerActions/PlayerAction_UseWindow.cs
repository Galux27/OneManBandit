using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction_UseWindow  : PlayerAction {
	Window w;

	public override void doAction ()
	{
		w.open = !w.open;
		PlayerActionUI.me.lastObjectSelected = null;
		PlayerAction.currentAction = null;

	}

	public override bool canDo ()
	{
		w = this.gameObject.GetComponent<Window> ();

		if (w.smashed == true) {
			Destroy (this);
			return false;
		}

		if (Vector3.Distance (this.transform.position, CommonObjectsStore.player.transform.position) < 2) {
			return !w.smashed;
		} else {
			return false;
		}
	}

	public override string getDescription ()
	{
		return "Interact with the window";
	}

	public override string getType ()
	{
		return "Interact Window";
	}
}
