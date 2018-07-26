using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour_InvestigateCorpse : NPCBehaviour {

	GameObject person;

	public override void Initialise ()
	{
		myType = behaviourType.investigate;
		myController = this.gameObject.GetComponent<NPCController> ();
		person = myController.memory.objectThatMadeMeSuspisious;
		myController.pf.getPath (this.gameObject, person);
		radioMessageOnStart ();
		isInitialised = true;
	}

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

	public override void OnComplete ()
	{
		myController.npcB.suspisious = false;
		//myController.memory.objectThatMadeMeSuspisious = null;
		if (NPCBehaviourDecider.globalAlarm == false) {
			myController.npcB.alarmed = true;
		}
		myController.currentBehaviour = null;
		radioMessageOnFinish ();
		Destroy (this);
	}


	bool isNearLoc()
	{
		if (Vector3.Distance (this.transform.position, person.transform.position) < 1.5f) {
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

			RoomScript rs = LevelController.me.getRoomObjectIsIn (person);

			if (rs == null) {
				PhoneTab_RadioHack.me.setNewText ("This is "+this.gameObject.name+", I'm on the move",h);

			} else {
				PhoneTab_RadioHack.me.setNewText ("This is "+this.gameObject.name+ ", I'm moving to " + rs.roomName,h);

			}

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

			RoomScript rs = LevelController.me.getRoomObjectIsIn (person);

			if (rs == null) {
				PhoneTab_RadioHack.me.setNewText ("This is "+this.gameObject.name+", I've found a body.",h);

			} else {
				PhoneTab_RadioHack.me.setNewText ("This is "+this.gameObject.name+ ", I've found a body in " + rs.roomName,h);

			}

		}
	}
}
