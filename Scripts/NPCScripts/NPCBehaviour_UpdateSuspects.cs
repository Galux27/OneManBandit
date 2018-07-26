using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour_UpdateSuspects : NPCBehaviour {

	public override void OnUpdate ()
	{
		if (isInitialised == false) {
			Initialise ();
		}

		if (isNearLoc () == false) {
			moveToCurrentPoint ();
			myController.pwc.aimDownSight = false;
		} else {
			OnComplete ();
		}
	}

	public override void Initialise ()
	{
		myType = behaviourType.updateSuspects;
		myController = this.gameObject.GetComponent<NPCController> ();

		if (myController.memory.objectThatMadeMeSuspisious == null) {
		} else {
			if (myController.memory.objectThatMadeMeSuspisious.tag == "NPC") {
				NPCController npc = myController.memory.objectThatMadeMeSuspisious.GetComponent<NPCController> ();
				if (myController.npcB.freindlyIDs.Contains (npc.npcB.myID) == true || myController.npcB.myID != npc.npcB.myID) {
					myController.npcB.suspisious = false;
					//////Debug.LogError ("NPC " + this.gameObject.name + " wanted to set alert on freindly npc " + myController.memory.objectThatMadeMeSuspisious);

					myController.memory.objectThatMadeMeSuspisious = null;
					Destroy (this);
				}
			}
		}
		radioMessageOnStart ();
		myController.pf.getPath (this.gameObject, LevelController.me.raiseAlarmLoc.gameObject);
		isInitialised = true;
	}

	bool isNearLoc()
	{
		if (Vector3.Distance (this.transform.position, LevelController.me.raiseAlarmLoc.position) < 1.5f) {
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
		//LevelController.me.alertLevel = 2;
		if (myController.memory.objectThatMadeMeSuspisious == null) {

		} else {
			if (myController.memory.objectThatMadeMeSuspisious.tag == "Player" || myController.memory.objectThatMadeMeSuspisious.tag == "NPC") {
				LevelController.me.suspects.Add (myController.memory.objectThatMadeMeSuspisious);
			}
		}

		if (myController.npcB.myType == AIType.cop) {
			NPCBehaviourDecider.copAlarm = true;
		} else if (myController.npcB.myType == AIType.guard) {
			NPCBehaviourDecider.globalAlarm = true;
		}

	//	NPCBehaviourDecider.globalAlarm = true;
		myController.npcB.alarmed = false;
		myController.npcB.suspisious = false;
		myController.memory.objectThatMadeMeSuspisious = null;
		radioMessageOnFinish ();
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


			PhoneTab_RadioHack.me.setNewText ("Its " + this.gameObject.name + " prepare for an update on suspects.",h);


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


			PhoneTab_RadioHack.me.setNewText ("Thats the suspect, keep an eye out for them.",h);


		}
	}
}