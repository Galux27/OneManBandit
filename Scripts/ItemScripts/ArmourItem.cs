using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmourItem : Item {
	public float armourValue = 15.0f;
	public override void equipItem ()
	{
		this.gameObject.GetComponentInParent<Inventory> ().equipItem (this, slot);

	}

	public override void itemPassiveEffect ()
	{
		
	}

	public override float getArmourIncrease ()
	{
		return armourValue;
	}

	public override void unequipItem ()
	{
		
	}
}
