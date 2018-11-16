using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDoorOpener : MonoBehaviour {

	/// <summary>
	/// Class that detects if a door is infront of an NPC and opens it if so. 
	/// </summary>

	NPCController myController;
	float timer = 0.5f;
	// Use this for initialization
	void Start () {
		myController = this.GetComponent<NPCController> ();
	}
	
	// Update is called once per frame
	void Update () {
		openDoor ();

	}

	void openDoor()
	{

		Vector3 heading = this.transform.up.normalized;
		RaycastHit2D[] rays = Physics2D.RaycastAll (this.transform.position, heading,1.5f);
		////Debug.DrawRay (this.transform.position, heading* 1.0f,Color.red);

		foreach (RaycastHit2D ray in rays) {
			if (ray.collider == null) {

			} else {
				if (ray.collider.gameObject.tag == "Car" || ray.collider.transform.root.tag == "Car") {
					NPCController npc = this.gameObject.GetComponent<NPCController> ();

					//if (npc.pmc.frozen == false && npc.pmc.movedThisFrame == true) {
						myController.pmc.setStopped ();
					//} 

					if (myController.pf.waitingForPath == false) {
						myController.pf.ForceNewPath ();
					}
				}
				else if (ray.collider.gameObject.tag == "Door") {
					DoorScript ds = ray.collider.gameObject.GetComponent<DoorScript> ();//TODO work out some method of obstacle avoidance
					if (ds == null) {
						ds = ray.collider.gameObject.GetComponentInParent<DoorScript> ();
						if (ds.doorOpen == false && ds.canNPCOpenDoor (myController.npcB.myID)) {
							ds.interactWithDoor (this.gameObject);
						}					
					} else {
						if (ds.doorOpen == false && ds.canNPCOpenDoor (myController.npcB.myID)) {
							ds.interactWithDoor (this.gameObject);
						}
					}
				} else if (ray.collider.gameObject.tag == "NPC" && ray.collider.gameObject != this.gameObject) {
					NPCController npc = ray.collider.gameObject.GetComponent<NPCController> ();

					//if (npc.npcB.myType == myController.npcB.myType) {
					//	if (npc.pmc.frozen == false && npc.pmc.movedThisFrame == true) {
					//		myController.pmc.setStopped ();
					//	} 
					//}
				} else if (ray.collider.gameObject.tag == "Walls" || ray.collider.gameObject.tag == "ImpassableObject") {
					myController.pf.increaseStuckCounter ();
				} 
			}
		}
		//Physics2D.queriesStartInColliders = true;

	}
}
