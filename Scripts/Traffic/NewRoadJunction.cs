using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for a road section with multile potential points to go to from it. 
/// </summary>
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
