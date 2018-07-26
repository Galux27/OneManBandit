using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour_ExitLevel : NPCBehaviour {
	public Transform exit;
	public override void Initialise ()
	{
		myType = behaviourType.exitLevel;
		myController = this.gameObject.GetComponent<NPCController> ();

		if (exit == null) {
			exit = LevelController.me.getRandomExit ();
		}
		myController.pf.getPath (this.gameObject,exit.gameObject);

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
			Destroy (this.gameObject);

		}
	}

	bool areWeAtPosition()
	{
		if (Vector2.Distance (this.transform.position, exit.position) > 2.0f) {
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
