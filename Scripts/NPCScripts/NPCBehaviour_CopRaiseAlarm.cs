using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour_CopRaiseAlarm : NPCBehaviour {

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

    GameObject raiseAlarmLoc;

	public override void Initialise ()
	{
        if (LevelController.me.copRaiseAlarmLoc == null)
        {
            raiseAlarmLoc = LevelController.me.raiseAlarmLoc.gameObject;
        }
        else
        {
            raiseAlarmLoc = LevelController.me.copRaiseAlarmLoc.gameObject;

        }

        myType = behaviourType.raiseAlarm;
		myController = this.gameObject.GetComponent<NPCController> ();
		myController.pf.getPath (this.gameObject,raiseAlarmLoc);
//		////////Debug.Log ("Cop trying to raise alarm due to " + myController.memory.objectThatMadeMeSuspisious);
		myController.myText.setText ("Better call for backup");

		if (myController.memory.objectThatMadeMeSuspisious == null) {
		} else {
			if (myController.memory.objectThatMadeMeSuspisious.tag == "NPC") {
				NPCController npc = myController.memory.objectThatMadeMeSuspisious.GetComponent<NPCController> ();
				if (myController.npcB.freindlyIDs.Contains (npc.npcB.myID) == true || myController.npcB.myID != npc.npcB.myID) {
					myController.npcB.suspisious = false;
					////////Debug.LogError ("NPC " + this.gameObject.name + " wanted to set alert on freindly npc " + myController.memory.objectThatMadeMeSuspisious);

					myController.memory.objectThatMadeMeSuspisious = null;
					Destroy (this);
				}
			}
		}
		radioMessageOnStart ();
		isInitialised = true;
	}

	bool isNearLoc()
	{
		if (Vector3.Distance (this.transform.position,raiseAlarmLoc.transform.position) < 1.5f) {
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
		
		if (myController.memory.objectThatMadeMeSuspisious == null) {
			NPCBehaviourDecider.copAlarm = true;
			radioMessageOnFinish ();
		} else {
			LevelController.me.alertLevel = 2;
			if (myController.memory.objectThatMadeMeSuspisious.tag == "Player" && myController.memory.seenSuspectsFace==true || myController.memory.objectThatMadeMeSuspisious.tag == "NPC") {
				LevelController.me.suspects.Add (myController.memory.objectThatMadeMeSuspisious);
			}
			NPCBehaviourDecider.copAlarm = true;
			myController.npcB.alarmed = false;
			myController.npcB.suspisious = false;
			radioMessageOnFinish ();
		}
		PoliceController.me.callPolice (this.gameObject);

		Destroy (this);
		//myController.memory.objectThatMadeMeSuspisious = null;
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

			PhoneTab_RadioHack.me.setNewText ("This is " + this.gameObject.name + ", I'm calling for backup.",h);
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

			PhoneTab_RadioHack.me.setNewText ("Alright, backup is on the way guys.", h);

		}
	}
}
