using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMemory : MonoBehaviour {
	//Idea of this class is to keep track of what an NPC knows e.g. do they know that the player is not just a civilian, whether an item 
	// Use this for initialization
	public bool raiseAlarm=false,suspisious=false;
	CanWeDetectTarget detect;
	public GameObject objectThatMadeMeSuspisious;
	//public bool objectWasTraspassing = false;

	public Vector3 guardPos;
	public Quaternion guardRot;

	//these variables are just things to remember e.g. what my patrol route was
	public PatrolRoute myRoute;
	public Vector3 noiseToInvestigate;
	public List<GameObject> peopleThatHaveAttackedMe=new List<GameObject>();

	public bool beenAttacked,seenHostage,seenCorpse,seenArmedSuspect,seenSuspect;
	void Start () {
		detect = this.GetComponent<CanWeDetectTarget> ();
	}
	
	// Update is called once per frame
	void Update () {
		//detectStuff ();
	}


	void detectStuff()
	{
		if (suspisious == true) {
			return;//already suspisious, no need to check for stuff
		}


		RoomScript myRoom = LevelController.me.getRoomObjectIsIn (this.gameObject);

		if (myRoom == null) {

		} else {
			foreach (Item i in ItemMoniter.me.itemsInWorld) {
				if (i.gameObject.activeInHierarchy == true) {
					if (myRoom.itemsInRoomAtStart.Contains (i) == false) {
						if (detect.areWeNearTarget (i.gameObject)) {
							if (detect.isTargetInFrontOfUs (i.gameObject)) {
								if (detect.lineOfSightToTargetWithNoCollider (i.gameObject)) {
									////////Debug.Log ("Item " + i.itemName + " was detected");
									objectThatMadeMeSuspisious = i.gameObject;
									suspisious = true;
								} 
							}
						}
					}
				}
			}
		}
		//do the same shit for players & NPCS
	}
}

