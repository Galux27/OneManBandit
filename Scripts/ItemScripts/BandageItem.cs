using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BandageItem : Item {

	public override void useItem()
	{
		this.GetComponentInParent<BleedingEffect> ().stopBleeding ();
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
