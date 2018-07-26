using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadJunction : MonoBehaviour {
	/// <summary>
	/// Old road junction, needs removing at some point. 
	/// </summary>
	public Road roadOn,roadGoingTo;
	public bool canUseJunction = true;

	public bool canIUseJunction()
	{
		if (canUseJunction==true && roadOn.roadAccessable == true && roadGoingTo.roadAccessable == true) {
			return true;
		}
		return false;
	}

	public Vector3 getJunctionPoint()
	{
		Vector3 lineVec3 = roadGoingTo.getCenterPointOfRoad () - roadOn.getCenterPointOfRoad ();
		Vector3 crossVec1and2 = Vector3.Cross ( roadOn.getRoadDirection(),roadGoingTo.getRoadDirection());
		Vector3 crossVec2and3 = Vector3.Cross (lineVec3, roadGoingTo.getRoadDirection());

		float planarFactor = Vector3.Dot (lineVec3, crossVec1and2);

		if (Mathf.Abs (planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f) {
			float s = Vector3.Dot (crossVec2and3, crossVec1and2) / crossVec1and2.sqrMagnitude;
			return roadOn.getCenterPointOfRoad() + (roadOn.getRoadDirection()*s);
		} else {
			return Vector3.zero;
		}

		return Vector3.zero;

	}
}
