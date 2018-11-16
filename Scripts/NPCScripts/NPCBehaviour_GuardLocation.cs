using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour_GuardLocation : NPCBehaviour {

	public Vector3 locationToGuard;
	public Quaternion rotationToFace;
	public GameObject marker;

	public override void Initialise ()
	{
		myType = behaviourType.guardLoc;
		myController = this.GetComponent<NPCController> ();
		if (myController.memory.guardPos == Vector3.zero) {
			locationToGuard = this.transform.position;
			rotationToFace = Quaternion.Euler (this.transform.rotation.eulerAngles);
			marker = (GameObject)Instantiate (new GameObject (), locationToGuard, rotationToFace);
			myController.memory.guardPos = locationToGuard;
			myController.memory.guardRot = rotationToFace;
		} else {
			locationToGuard = myController.memory.guardPos;
			rotationToFace = myController.memory.guardRot;
			marker = (GameObject)Instantiate (new GameObject (), locationToGuard, rotationToFace);

		}
		if (areWeNearGuardPos () == false) {
			myController.pf.getPath (this.gameObject, marker);
		}
		isInitialised = true;
	}


	public override void OnUpdate ()
	{
		//////////Debug.LogError ("Calling guard location on Update");
		if (isInitialised == false) {
			Initialise ();
			isInitialised = true;
			return;
		}
			
		if (areWeNearGuardPos () == false) {
			//////////Debug.LogError ("Not near location to guard, moving to");
			if (myController.pf.currentPath == null ||myController.pf.target!=marker ) {
				myController.pf.getPath (this.gameObject, marker);

			}

			moveToCurrentPoint ();
		} else {
			this.transform.position = locationToGuard;
			this.transform.rotation = rotationToFace;
		}
	}


	bool areWeNearGuardPos()
	{
		if (Vector2.Distance (this.transform.position, marker.transform.position) < 1.5f) {
			return true;
		}
		return false;
	}

	void moveToCurrentPoint()
	{
		myController.pmc.rotateToFacePosition (myController.pf.getCurrentPoint());
		myController.pmc.moveToDirection (myController.pf.getCurrentPoint());
	}

	void OnDestroy()
	{
		if (marker == null) {

		} else {
			Destroy (marker);
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


			RoomScript rs = LevelController.me.getRoomObjectIsIn (marker);

			if (rs == null) {
				PhoneTab_RadioHack.me.setNewText ("This is "+this.gameObject.name+", standing guard until you need me.",h);

			} else {
				PhoneTab_RadioHack.me.setNewText ("This is "+this.gameObject.name+ ", Standing guard in " + rs.roomName,h);

			}
		}
	}

}
