using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour_EscortOutOfRestrictedArea : NPCBehaviour {
	public GameObject target;
	RoomScript roomStartedIn;

	float timerToGetOut = 10.0f;
	bool firstWarning = false,secondWarning=false,initialWarning=false;
	bool playerNotListening=false;
	public override void Initialise ()
	{
		myType = behaviourType.traspassing;
		myController = this.gameObject.GetComponent<NPCController> ();
		target = myController.memory.objectThatMadeMeSuspisious;
		roomStartedIn = LevelController.me.getRoomObjectIsIn (target);
		radioMessageOnStart ();
		isInitialised = true;
	}

	public override void OnUpdate ()
	{
		if (isInitialised == false) {
			Initialise ();
		}
		myController.pmc.rotateToFacePosition (target.transform.position);
		if (Vector3.Distance (this.transform.position, target.transform.position) > 3.0f) {
			if(myController.pf.target != target)
			{
				myController.pf.getPath (this.gameObject, target);
			}

			myController.pmc.moveToDirection (myController.pf.getCurrentPoint());
		} else {
			if (initialWarning == false) {
				myController.myText.setText ("You're trespassing, leave now");
				initialWarning = true;
			}
		}
		RoomScript newRoom = LevelController.me.getRoomObjectIsIn (target);

		if (playerNotListening == false) {
			if (reduceTimer (newRoom) == true) {
				timerToGetOut -= 5.0f;
				playerNotListening = true;
			}
		}

		if (playerNoLongerTraspassing (newRoom) == true) {
			myController.npcB.suspisious = false;
			myController.memory.objectThatMadeMeSuspisious = null;
			radioMessageOnFinish ();
			Destroy (this);
		}

		timerToGetOut -= Time.deltaTime;

		if (timerToGetOut <= 0) {
			if (myController.npcB.alarmed == false) {
				myController.myText.setText ("You were warned!");
			}
			radioMessageOnFinish ();

			myController.npcB.doing = whatAiIsDoing.attacking;
			myController.memory.seenSuspect = true;
		} else if (timerToGetOut <= 3) {
			if (secondWarning == false) {
				if (myController.detect.fov.visibleTargts.Contains (target.transform) == false) {
					myController.myText.setText ("Don't hide, you need to leave!");
				} else {
					myController.myText.setText ("Last chance!");

				}

			}
			secondWarning = true;
		} else if (timerToGetOut <= 6) {
			if (firstWarning == false) {
				myController.myText.setText ("I told you to leave!");
			}
			firstWarning = true;
		}

	}

	bool reduceTimer(RoomScript newRoom)
	{
		if (newRoom == null) {
			return false;
		}

		if (myController.detect.fov.visibleTargts.Contains (target.transform) == false) {
			return true;
		}

		if (newRoom != roomStartedIn && newRoom.traspassingInRoom() == true) {
			return true;
		} else {
			return false;
		}
	}

	bool playerNoLongerTraspassing(RoomScript newRoom)
	{
		if (newRoom == null) {
			return true;
		}


		if (newRoom != roomStartedIn && newRoom.traspassingInRoom() == false) {
			return true;
		} else {
			return false;
		}
	}


	public override void OnComplete ()
	{

	}

	public override void passInGameobject (GameObject passIn)
	{
		target = passIn;
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

			PhoneTab_RadioHack.me.setNewText ("We've got a trespasser in " + roomStartedIn.roomName,h);
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

			if (timerToGetOut <= 0) {
				PhoneTab_RadioHack.me.setNewText ("They're not listening, I'm taking them down.", h);
			} else {
				PhoneTab_RadioHack.me.setNewText ("Just a misunderstanding, they're gone now.", h);

			}

		}
	}
}

