using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour_RaiseAlarm : NPCBehaviour {
	public float distanceToPoint = 0.0f;
	public override void OnUpdate ()
	{
		if (isInitialised == false) {
			Initialise ();
		}

		if (isNearLoc () == false) {
			moveToCurrentPoint ();
		} else {
			OnComplete ();
		}
	}

	public override void Initialise ()
	{
		myType = behaviourType.raiseAlarm;
		myController = this.gameObject.GetComponent<NPCController> ();
		myController.pf.getPath (this.gameObject, LevelController.me.raiseAlarmLoc.gameObject);
		radioMessageOnStart ();
		isInitialised = true;
	}

	bool isNearLoc()
	{
		distanceToPoint = Vector2.Distance (this.transform.position, LevelController.me.raiseAlarmLoc.position);
		if (distanceToPoint < 1.6f) {
			return true;
		} else {
			return false;
		}
	}

	void moveToCurrentPoint()
	{
		if (myController.pf.target != LevelController.me.raiseAlarmLoc.gameObject) {
			myController.pf.getPath (this.gameObject, LevelController.me.raiseAlarmLoc.gameObject);
		}

		myController.pmc.rotateToFacePosition (myController.pf.getCurrentPoint());
		myController.pmc.moveToDirection (myController.pf.getCurrentPoint());
	}

	public override void OnComplete ()
	{
		LevelController.me.alertLevel = 2;
		if (myController.memory.objectThatMadeMeSuspisious == null) {

		} else {
			if (myController.memory.objectThatMadeMeSuspisious.tag == "Player" || myController.memory.objectThatMadeMeSuspisious.tag == "NPC") {
				LevelController.me.suspects.Add (myController.memory.objectThatMadeMeSuspisious);
			}
		}
		NPCBehaviourDecider.globalAlarm = true;
		myController.npcB.alarmed = false;
		myController.npcB.suspisious = false;
		PoliceController.me.callPolice (this.gameObject);
		myController.memory.objectThatMadeMeSuspisious = null;
		radioMessageOnFinish ();
		Destroy (this);
		//////Debug.Log ("Set off global alarm");
	}

	public override void radioMessageOnStart ()
	{
		radioHackBand h = radioHackBand.buisness;

		if (myController.npcB.myType == AIType.civilian) {

		} else {
			if (myController.npcB.myType == AIType.cop) {
				h = radioHackBand.cop;
			} else if (myController.npcB.myType == AIType.swat) {
				h = radioHackBand.swat;
			}


			PhoneTab_RadioHack.me.setNewText ("This is "+this.gameObject.name+ ", I'm going to call the cops.",h);


		}
	}

	public override void radioMessageOnFinish ()
	{
		radioHackBand h = radioHackBand.buisness;

		if (myController.npcB.myType == AIType.civilian) {

		} else {
			if (myController.npcB.myType == AIType.cop) {
				h = radioHackBand.cop;
			} else if (myController.npcB.myType == AIType.swat) {
				h = radioHackBand.swat;
			}


		
			PhoneTab_RadioHack.me.setNewText ("This is " + this.gameObject.name + ", Cops are on the way.", h);


		}
	}
}
