using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTVController : MonoBehaviour {

	/// <summary>
	/// Controller class for all CCTVCamera instances in the level
	/// </summary>

	public static CCTVController me;
	public CCTVCamera[] camerasInWorld;


	void Awake()
	{
		
		me = this;
		camerasInWorld = FindObjectsOfType<CCTVCamera> ();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	/// <summary>
	/// Alerts are passed in by individual cameras once they spot the player (if they have been detected by a NPC previously), 
	/// it will they try to find nearby enemies to go to the camera to see if they can catch the player. 
	/// </summary>
	/// <param name="c">C.</param>
	public void cameraAlert(CCTVCamera c)
	{
		List<NPCController> candidatesForInvestigation = new List<NPCController> ();
		foreach (GameObject g in NPCManager.me.npcsInWorld) {
			if (g == null) {
				continue;
			}

			NPCController npc = g.GetComponent<NPCController> ();
			//if (npc.npcB.myType == AIType.guard) {
				candidatesForInvestigation.Add (npc);
			//}
		}

		NPCController finalCandidate = null;
		float closest = 9099999.09f;

		List<NPCController> nearbyCops = new List<NPCController> ();

		foreach (NPCController npc in candidatesForInvestigation) {
			if (npc.npcB.myType == AIType.guard) {
				float dist = Vector3.Distance (npc.gameObject.transform.position, c.gameObject.transform.position);
				if (dist < closest && npc.npcB.alarmed == false && npc.npcB.suspisious == false) {
					closest = dist;
					finalCandidate = npc;
				}
			} else if (npc.npcB.myType == AIType.swat) {
				if (npc.currentBehaviour.myType == behaviourType.searchRooms) {
					RoomScript r = LevelController.me.getRoomObjectIsIn (c.gameObject);
					if (r == null) {

					} else {
						if (NPCBehaviour_SWATSearchMap.roomToGoTo != r) {
							FindObjectOfType<NPCBehaviourDecider> ().setAllSwatToGoToRoom (r); //TODO switch to just making it use the behaviour decider of the npc its currently on
						}
					}
				}
			} else if (npc.npcB.myType == AIType.cop) {
				float dist = Vector3.Distance (npc.gameObject.transform.position, c.gameObject.transform.position);
				if (dist < 20 && npc.npcB.alarmed == false && npc.npcB.suspisious == false) {
					nearbyCops.Add (npc);
				}
			}
		}

		if (finalCandidate == null) {
			//////////Debug.Log ("No guard to invesitgate camera");
		} else {
			finalCandidate.memory.noiseToInvestigate = c.gameObject.transform.position;
			finalCandidate.npcB.suspisious = true;
		}

		foreach (NPCController npc in nearbyCops) {
			npc.memory.noiseToInvestigate = c.gameObject.transform.position;
			npc.npcB.suspisious = true;

			//////////Debug.LogError ("Setting cop " + npc.gameObject.ToString () + " to investigate " + c.gameObject.ToString ());
		}

		if (PoliceController.me.swatHere == true && PoliceController.me.seenHostile==false) {
			RoomScript r = LevelController.me.getRoomObjectIsIn (c.gameObject);
			if (r == null) {

			} else {
				if (PoliceController.me.currentRoomSwat != r) {
					PoliceController.me.lastRoom = r;
					PoliceController.me.currentRoomSwat = null;
					PoliceController.me.goingToRoom = true;
				}
			}
		}
	}
}
