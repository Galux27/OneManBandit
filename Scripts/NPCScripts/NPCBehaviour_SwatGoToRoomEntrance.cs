using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour_SwatGoToRoomEntrance : NPCBehaviour {
	public GameObject point;
	public override void Initialise ()
	{
		myType = behaviourType.investigate;
		myController = this.gameObject.GetComponent<NPCController> ();

		point = PoliceController.me.currentRoomSwat.entrances[0].gameObject;
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
			facePoint ();
		}
		haveWeSeenSuspiciousObject ();
	}

	void facePoint()
	{
		myController.pmc.rotateToFacePosition (point.transform.position);

	}

	public bool areWeAtPosition()
	{
		if (point == null) {
			return false;
		}


		if (Vector2.Distance (this.transform.position, point.transform.position) > 5.0f) {
			return false;
		} else {
			return true;
		}
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
