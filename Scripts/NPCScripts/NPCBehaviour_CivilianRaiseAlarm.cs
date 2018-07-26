using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour_CivilianRaiseAlarm : NPCBehaviour {
	GameObject phoneToGoTo;
	ProgressBar myProgress;
	float timer = 2.5f;
	public override void OnUpdate ()
	{
		if (isInitialised == false) {
			Initialise ();
		}

		if (phoneToGoTo == null) {
			myController.currentBehaviour = this.gameObject.AddComponent<NPCBehaviour_ExitLevel> ();
			myController.npcB.alarmed = false;
			myController.npcB.suspisious = false;
			myController.memory.objectThatMadeMeSuspisious = null;
			myController.myText.setText ("Fuck this shit I'm out.");

			Destroy (this);
		}

		if (isNearLoc () == false) {
			moveToCurrentPoint ();
		} else {
			//OnComplete ();
			if (myProgress == null) {
				createProgressBar ();
			} else {
				if (timer > 0) {
					timer -= Time.deltaTime;
					myProgress.currentValue = (myProgress.maxValue - timer);
					myController.pmc.rotateToFacePosition (phoneToGoTo.transform.position);

				} else {
					myProgress.currentValue = myProgress.maxValue;
					OnComplete ();
				}
			}
		}
	}

	public override void Initialise ()
	{
		if (phoneToGoTo == null) {
			phoneToGoTo = LevelController.me.getNearestPhone (this.transform.position);
		}

	

		myType = behaviourType.raiseAlarm;
		myController = this.gameObject.GetComponent<NPCController> ();

		if (phoneToGoTo == null) {
			myController.currentBehaviour = this.gameObject.AddComponent<NPCBehaviour_ExitLevel> ();
			myController.npcB.alarmed = false;
			myController.npcB.suspisious = false;
			myController.memory.objectThatMadeMeSuspisious = null;
			Destroy (this);
		} else {
			myController.myText.setText ("Gotta call the cops...");
			myController.pf.getPath (this.gameObject, phoneToGoTo);
			isInitialised = true;
		}
	}

	bool isNearLoc()
	{
		if (Vector3.Distance (this.transform.position, phoneToGoTo.transform.position) < 1.5f) {
			return true;
		} else {
			return false;
		}
	}

	void moveToCurrentPoint()
	{
		myController.pmc.rotateToFacePosition (myController.pf.getCurrentPoint());
		myController.pmc.moveToDirection (myController.pf.getCurrentPoint());
	}

	public override void OnComplete ()
	{
		myController.myText.setText ("Gotta get out of here");
		PoliceController.me.callPolice (this.gameObject);

		LevelController.me.alertLevel = 2;
		NPCBehaviourDecider.globalAlarm = true;
		//NPCBehaviourDecider.copAlarm = true;
		myController.npcB.alarmed = false;
		myController.npcB.suspisious = false;
		myController.memory.objectThatMadeMeSuspisious = null;
		Destroy (this);
		//////Debug.Log ("Set off global alarm");

	}

	public void createProgressBar()
	{
		myController.myText.setText ("Hello Police?");

		GameObject g = (GameObject)Instantiate (CommonObjectsStore.me.progressBar, Vector3.zero, Quaternion.Euler (0, 0, 0));
		g.transform.parent = GameObject.FindGameObjectWithTag ("MainCamera").GetComponentInChildren<Canvas> ().gameObject.transform;
		myProgress = g.GetComponent<ProgressBar> ();
		myProgress.maxValue = timer;
		myProgress.myObjectToFollow = this.gameObject;
		g.transform.localScale = new Vector3 (1, 1, 1);
	}

	void OnDestroy()
	{
		if (myProgress == null) {

		} else {
			Destroy (myProgress.gameObject);
		}
	}
}