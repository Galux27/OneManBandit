using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRoadJunction:NewRoadSection{
	public List<NewRoadJunction> potentialPoints;

	public override bool isJunction ()
	{
		return true;
	}

	public override List<Transform> getPoints(){
		List<Transform> retVal = new List<Transform> ();
		foreach (NewRoadSection nr in potentialPoints) {
			retVal.Add (nr.startPoint);
		}
		return retVal;
	}
}
