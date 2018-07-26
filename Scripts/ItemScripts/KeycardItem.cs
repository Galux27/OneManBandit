using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeycardItem : Item {
	public int securityClearance = 0;

	public override string getDescription ()
	{
		return itemDescription + " Security clearance level: " + securityClearance.ToString ();
	}

	public override string getBriefDescription ()
	{
		return briefDescription + securityClearance.ToString ();
	}
}
