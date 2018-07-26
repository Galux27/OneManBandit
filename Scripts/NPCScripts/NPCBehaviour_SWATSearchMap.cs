using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour_SWATSearchMap : NPCBehaviour {
	public static RoomScript roomToGoTo;
	public static List<GameObject> swatInRoom;
	public Vector3 midPoint;
	public static bool gotNewLocation = false;
	public static GameObject marker;
	public override void Initialise ()
	{
		myType = behaviourType.searchRooms;
		isInitialised = true;

		myController = this.gameObject.GetComponent<NPCController> ();
		if (swatInRoom == null) {
			swatInRoom = new List<GameObject> ();
		}

		if (roomToGoTo == null) {
			roomToGoTo = LevelController.me.roomsInLevel [Random.Range (0, LevelController.me.roomsInLevel.Length)];
		}
		//have some kind of issue with pathfinding, 
		midPoint = roomToGoTo.rectsInRoom[0].bottomLeft.position + (( roomToGoTo.rectsInRoom[0].topRight.position - roomToGoTo.rectsInRoom[0].bottomLeft.position )/2);
		if (marker == null) {
			marker = (GameObject)Instantiate (new GameObject(), midPoint, Quaternion.Euler (0, 0, 0));
			marker.name = "Swat Marker";
		}
		myController.pf.getPath (this.gameObject, marker);


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
		if (swatInRoom.Contains (this.gameObject) == false) {
			swatInRoom.Add (this.gameObject);
		}

		if (areAllSwatInRoom () == true && NPCBehaviour_SWATSearchMap.gotNewLocation==false) {

			setSwatToNewRoom ();
		}
		//myController.npcB.suspisious = false;
		//myController.npcB.alarmed = false;
		//Destroy (this);
	}

	bool areAllSwatInRoom()
	{
		foreach (NPCController npc in NPCManager.me.npcControllers) {
			if (npc == null) {
				continue;
			}

			if (npc.npcB.myType == AIType.swat) {
				RoomScript room = LevelController.me.getRoomObjectIsIn (npc.gameObject);
				if (room == null) {
					//////Debug.Log (g.name + " Is not in a room.");
				} else {
					//////Debug.Log (g.name + " Is in room " + room.roomName);
					//if (room != roomToGoTo) {
					//	return false;
					//}
				}
				if (room != roomToGoTo && Vector3.Distance(midPoint,npc.gameObject.transform.position)>5.0f) {
					return false;
				}
			}

		}
		return true;

		/*NPCBehaviour_SWATSearchMap[] allSwat = FindObjectsOfType<NPCBehaviour_SWATSearchMap> ();

		foreach (NPCBehaviour nb in allSwat) {
			RoomScript room = LevelController.me.getRoomObjectIsIn (nb.gameObject);
			//////Debug.Log ("Swat " + nb.gameObject.name + " Is in room " + room.roomName);
			if (room != roomToGoTo) {
				return false;
			}
		}
		return true;*/
	}

	bool isNearLoc()
	{
		if (Vector3.Distance (this.transform.position, midPoint) < 2.5f) {
			return true;
		} else {
			NPCBehaviour_SWATSearchMap.gotNewLocation = false;
			return false;
		}
	}

	void setSwatToNewRoom()
	{
		/*RoomScript newRoom = LevelController.me.roomsInLevel [Random.Range (0, LevelController.me.roomsInLevel.Length)];
		NPCBehaviour_SWATSearchMap.roomToGoTo = newRoom;
		NPCBehaviour_SWATSearchMap[] allSwat = FindObjectsOfType<NPCBehaviour_SWATSearchMap> ();
		foreach (NPCBehaviour_SWATSearchMap nb in allSwat) {
			nb.midPoint = roomToGoTo.bottomLeft.position + (( roomToGoTo.topRight.position - roomToGoTo.bottomLeft.position )/2);
			nb.Initialise ();
			//nb.myController.pf.getPath (this.gameObject.transform.position, midPoint);
		}*/
		RoomScript newRoom = LevelController.me.roomsInLevel [Random.Range (0, LevelController.me.roomsInLevel.Length)];
		midPoint = newRoom.rectsInRoom[0].bottomLeft.position + (( newRoom.rectsInRoom[0].topRight.position -newRoom.rectsInRoom[0].bottomLeft.position )/2);
		marker.transform.position = midPoint;
		//NPCController[] allNPC = FindObjectsOfType<NPCController> ();
		foreach (NPCController npc in NPCManager.me.npcControllers) {
			if (npc.npcB.myType == AIType.swat) {
				Destroy (npc.currentBehaviour);
				NPCBehaviour_SWATSearchMap nb = npc.gameObject.AddComponent<NPCBehaviour_SWATSearchMap> ();
				NPCBehaviour_SWATSearchMap.roomToGoTo = newRoom;
				npc.currentBehaviour = nb;
			}
		}
		NPCBehaviour_SWATSearchMap.gotNewLocation = true;

		radioMessageOnStart ();
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


			PhoneTab_RadioHack.me.setNewText ("SWAT Team moving to " + NPCBehaviour_SWATSearchMap.roomToGoTo.roomName,h);


		}
	}

}
