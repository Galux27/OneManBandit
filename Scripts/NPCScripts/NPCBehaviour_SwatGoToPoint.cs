using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour_SwatGoToPoint : NPCBehaviour {
	public GameObject point;
	public override void Initialise ()
	{
		myType = behaviourType.investigate;
		myController = this.gameObject.GetComponent<NPCController> ();
		if (point == null) {
			point = PoliceController.me.pointsToGoTo [0];
		}
		myController.pf.getPath (this.gameObject,point);

		isInitialised = true;
	}

	public override void OnUpdate ()
	{
		if (isInitialised == false) {
			Initialise ();
		}

		if (areWeAtPosition () == false) {
			moveToPosition ();
		} else {
			facePosition ();
		}
		haveWeSeenSuspiciousObject ();

	}
	Vector3 pos = Vector3.zero;
	void facePosition()
	{
		if (isInitialised == false || PoliceController.me.currentRoomSwat==null) {
			return;
		}

		if (pos == Vector3.zero) {

			if (PoliceController.me.buildingUnderSiege == null) {
				pos = this.transform.position + this.transform.forward;
			} else {

				roomRect r = PoliceController.me.currentRoomSwat.getRectIAmIn (this.transform.position);
				if (r == null) {
					pos = PoliceController.me.currentRoomSwat.getCenter ();
				} else {
					pos = r.getCenterPoint ();
				}
			}
		}
		myController.pmc.rotateToFacePosition (pos);

	}



	public void haveWeSeenSuspiciousObject()
	{
		foreach (Transform t in myController.detect.fov.visibleTargts) {
			if (t == null) {
				continue;
			} else {
				if (LevelController.me.suspects.Contains (t.gameObject) == true) {
					PoliceController.me.seenHostile = true;
					PoliceController.me.swatTarget = t.gameObject;
				} else if (t.gameObject == CommonObjectsStore.player) {
					PoliceController.me.seenHostile = true;
					PoliceController.me.swatTarget = t.gameObject;
				}

			}
		}
	}


	public bool areWeAtPosition()
	{
		if (Vector3.Distance (this.transform.position, point.transform.position) > 1.5f) {
			return false;
		} else {
			return true;
		}
	}

	void moveToPosition(){
		myController.pmc.rotateToFacePosition (myController.pf.getCurrentPoint());
		if (areWeAtPosition()==false) {
			myController.pmc.moveToDirection (myController.pf.getCurrentPoint());
		}
	}

	public override void OnComplete ()
	{

	}
}
