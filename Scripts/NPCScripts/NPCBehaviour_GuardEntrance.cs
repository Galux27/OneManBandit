using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour_GuardEntrance : NPCBehaviour {

	public Vector3 locationToGuard;
	public GameObject nearestDoor;
	public Transform locationNode;
	public Quaternion rotationToFace;
	public GameObject marker;
	bool requestedPath=false;
	Rigidbody2D rid;
	HeadController hc;
	public override void Initialise ()
	{
		myType = behaviourType.guardEntrance;
		hc = this.gameObject.GetComponentInChildren<HeadController> ();
		rid = this.gameObject.GetComponent<Rigidbody2D> ();
		myController = this.GetComponent<NPCController> ();
		locationNode = PoliceController.me.getPointToGuard ();
		if (locationNode == null) {
			noGuardFallback ();
			rotationToFace = Quaternion.Euler (0, 0, Random.Range (0, 360));
			locationToGuard = locationNode.transform.position;

		} else {
			setNearest ();
			rotationToFace = Quaternion.Euler (0, 0, Vector3.Angle (locationToGuard, nearestDoor.transform.position));
			locationToGuard = locationNode.transform.position;

		}


		//rotationToFace = Quaternion.Euler (0, 0, Random.Range(0,360));


		myController.pf.getPath (this.gameObject, locationNode.gameObject);
		isInitialised = true;
	}

	void noGuardFallback(){
		//float dist = 999999.0f;
		//foreach(CivilianAction c in LevelController.me.actionsInWorld)
		//{
		//	float d2 = Vector2.Distance (this.transform.position, c.positionForAction.position);
		//	if (d2 < dist) {
		////		dist = d2;
				//ca = c;
		//	}
		//}
		locationNode = LevelController.me.getCivilianAction().positionForAction;
		nearestDoor = locationNode.gameObject;
	}

	void setNearest(){
		if (PoliceController.me.buildingUnderSiege == null || PoliceController.me.buildingUnderSiege.entrances == null || PoliceController.me.buildingUnderSiege.entrances.Count == 0) {
			noGuardFallback ();
		} else {
			nearestDoor = PoliceController.me.buildingUnderSiege.entrances [0].gameObject;
			float dist = 999999.0f;
			foreach (Transform t in PoliceController.me.buildingUnderSiege.entrances) {
				float d = Vector3.Distance (t.position, locationNode.position);
				if (d < dist) {
					dist = d;
					nearestDoor = t.gameObject;
				}
			}
		}
	}

	public void rotateToFacePosition(Vector3 pos)
	{
		hc.rotateToFacePosition (pos);
		Vector3 rot = new Vector3(0, 0, Mathf.Atan2((pos.y - transform.position.y),pos.x - transform.position.x)) * Mathf.Rad2Deg;
		rot = new Vector3(rot.x, rot.y, rot.z-90);//add 90 to make the player face the right way (yaxis = up)
		//rid.transform.eulerAngles = rot;
		rid.transform.rotation =Quaternion.Slerp(this.transform.rotation,Quaternion.Euler(rot),5*Time.deltaTime);// Quaternion.Euler(rot); //(INSTA ROTATION)

		//rotates player on Z axis to face cursor position
	}

	public override void OnUpdate ()
	{
		////////Debug.LogError ("Calling guard location on Update");
		if (isInitialised == false) {
			Initialise ();
			isInitialised = true;

		}

		if (areWeNearGuardPos () == false) {
			////////Debug.LogError ("Not near location to guard, moving to");

			if (requestedPath == false) {
				if (myController.pf.currentPath == null || myController.pf.currentPath.Count == 0) {
					myController.pf.getPath (this.gameObject, locationNode.gameObject);

				}
				requestedPath = true;
			}
			moveToCurrentPoint ();
		} else {
			this.transform.position = locationToGuard;
			rotateToFacePosition (nearestDoor.transform.position);
		}
	}


	bool areWeNearGuardPos()
	{
		if (Vector3.Distance (this.transform.position, locationToGuard) < 1.5f) {
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


			PhoneTab_RadioHack.me.setNewText ("This is "+this.gameObject.name+", Entrance secured.",h);

		}
	}

}
