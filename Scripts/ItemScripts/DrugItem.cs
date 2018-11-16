using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrugItem :Item {

	public string drugEffect = "";
	public override void useItem ()
	{
		EffectsManager.me.addEffectToPlayer (drugEffect);
		Inventory inventoryIAmIn = this.gameObject.GetComponentInParent<Inventory> ();
		if (myContainer == null) {
		} else {
			myContainer.removeItemFromContainer (this);
		}


		if (inventoryIAmIn == null) {

		} else {
			inventoryIAmIn.removeItemWithoutDrop (this);
		}

		dropItem ();
		Destroy (this.gameObject);
	}
}
