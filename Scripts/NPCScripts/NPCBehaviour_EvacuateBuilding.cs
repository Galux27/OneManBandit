using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour_EvacuateBuilding : NPCBehaviour {
	public Transform exit;
	float dist = 0.0f;
	public override void Initialise ()
	{
		myType = behaviourType.exitLevel;
		myController = this.gameObject.GetComponent<NPCController> ();

		if (exit == null) {
			exit = PoliceController.me.evacPoint;
		}
		myController.pf.getPath (this.gameObject,exit.gameObject);
		dist = Random.Range (7.5f, 1.5f);
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
			//Destroy (this.gameObject);

		}
	}

	bool areWeAtPosition()
	{
		if (Vector3.Distance (this.transform.position, exit.position) > dist) {
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