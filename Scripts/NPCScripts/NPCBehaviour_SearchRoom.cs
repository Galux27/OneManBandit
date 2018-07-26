using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour_SearchRoom : NPCBehaviour {
	public RoomScript roomToGoTo;
	//public Vector3 midPoint;
	public GameObject point;
	bool searchingRoom = false;
	public override void Initialise ()
	{
		myType = behaviourType.searchRooms;
		myController = this.gameObject.GetComponent<NPCController> ();

		//int r = Random.Range(0,100);
		//if (r < 25) {
			if (roomToGoTo == null) {
				roomToGoTo = LevelController.me.roomsInLevel [Random.Range (0, LevelController.me.roomsInLevel.Length)];
			}
			point = roomToGoTo.getRandomPoint ();
			searchingRoom = true;
		//} else {
			//searchingRoom = false;
		//	point = LevelController.me.actionsInWorld [Random.Range (0, LevelController.me.actionsInWorld.Count)].positionForAction.gameObject;
		//}
		//midPoint = roomToGoTo.bottomLeft.position + (( roomToGoTo.topRight.position - roomToGoTo.bottomLeft.position )/2);

		myController.pf.getPath (this.gameObject, point);
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
			myController.pwc.aimDownSight = true;
		} else {
			OnComplete ();
		}
	}

	public override void OnComplete ()
	{
		myController.npcB.suspisious = false;
		myController.npcB.alarmed = false;
		radioMessageOnFinish ();
		Destroy (this);
	}


	bool isNearLoc()
	{
		if (Vector3.Distance (this.transform.position, point.transform.position) < 2.0f) {
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

			if (searchingRoom == true) {
				PhoneTab_RadioHack.me.setNewText ("This is " + this.gameObject.name + ", going to search " + roomToGoTo.roomName, h);
			} else {
				PhoneTab_RadioHack.me.setNewText ("This is " + this.gameObject.name + ", Checking somewhere else.", h);

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

			if (searchingRoom == true) {
				
				PhoneTab_RadioHack.me.setNewText (this.gameObject.name + " here, " + roomToGoTo.roomName + " was clear.", h);

			} else {
				PhoneTab_RadioHack.me.setNewText (this.gameObject.name + " here, didn't see anything.", h);

			}
		}
	}

}
