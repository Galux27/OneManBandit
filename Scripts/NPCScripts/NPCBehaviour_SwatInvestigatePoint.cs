using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour_SwatInvestigatePoint : NPCBehaviour {
	public GameObject point;
	public override void Initialise ()
	{
		myType = behaviourType.investigate;
		myController = this.gameObject.GetComponent<NPCController> ();

		point = new GameObject();
		point.transform.position = PoliceController.me.pointToInvestigate;
		myController.pf.getPath (this.gameObject,point);
		PhoneTab_RadioHack.me.setNewText ("SWAT Team investigating disturbance standby. ",radioHackBand.swat);
		////Debug.Break ();
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
	}
	Vector3 pos = Vector3.zero;
	void facePosition()
	{
		if (pos == Vector3.zero) {

			if (PoliceController.me.currentRoomSwat == null) {
				pos = CommonObjectsStore.player.transform.position;
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

	public bool areWeAtPosition()
	{
		if (point == null) {
			return true;
		}

		if (Vector3.Distance (this.transform.position, point.transform.position) > 2.0f) {
			return false;
		} else {
			return true;
		}
	}

	public GameObject haveWeSeenSuspiciousObject()
	{
		foreach (Transform t in myController.detect.fov.visibleTargts) {
			if (t == null) {
				continue;
			} else {
				if (LevelController.me.suspects.Contains (t.gameObject) == true) {
					return t.gameObject;
				} else if (t.gameObject == CommonObjectsStore.player) {
					return t.gameObject;
				} else if (t.gameObject.layer == 29) {
					return CommonObjectsStore.player;
				}

			}
		}
		return null;
	}

	void moveToPosition(){
		myController.pmc.rotateToFacePosition (myController.pf.getCurrentPoint());
		if (areWeAtPosition()==false) {
			myController.pmc.moveToDirection (myController.pf.getCurrentPoint());
		}
	}

	void OnDestroy()
	{
		Destroy (point);
	}

	public override void OnComplete ()
	{

	}
}
