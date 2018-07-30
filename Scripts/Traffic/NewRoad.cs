using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that stores all the road sections in a leve
/// </summary>
public class NewRoad : MonoBehaviour {
	public List<NewRoadJunction> sectionsInTheRoad;

	public NewRoadJunction getNearestRoad(Vector3 pos)
	{
		float dist = 999999.0f;
		NewRoadJunction retVal = null;

		foreach (NewRoadJunction nj in sectionsInTheRoad) {
			float d = Vector3.Distance (nj.startPoint.position, pos);
			if (d < dist) {
				dist = d;
				retVal = nj;
			}
		}
		return retVal;
	}
}
/// <summary>
/// Class that is a base for a single road section. 
/// </summary>
public class NewRoadSection : MonoBehaviour{
	public Transform startPoint;
	public NewRoadSection nextPoint;

	public virtual bool isJunction()
	{
		return false;
	}

	public virtual List<Transform> getPoints(){
		List<Transform> retVal = new List<Transform> ();
		retVal.Add (nextPoint.startPoint);
		return retVal;
	}
}

