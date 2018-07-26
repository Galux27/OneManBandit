using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackpackItem : Item {
	public float capacityIncrease = 5.0f;
	public override void equipItem ()
	{
		this.gameObject.GetComponentInParent<Inventory> ().equipItem (this, slot);

	}

	public override void itemPassiveEffect ()
	{

	}

	public override float getCapacityIncrease ()
	{
		return capacityIncrease;
	}

	public override void unequipItem ()
	{

	}
}
