using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDoorOpener : MonoBehaviour {
	NPCController myController;
	float timer = 0.5f;
	bool madeTrigger = true;
	// Use this for initialization
	void Start () {
		myController = this.GetComponent<NPCController> ();
	}
	
	// Update is called once per frame
	void Update () {
		openDoor ();
		if (madeTrigger == true) {
			timer -= Time.deltaTime;
			if (timer <= 0) {
				this.GetComponent<Collider2D> ().isTrigger = false;

				madeTrigger = false;
			}
		}
	}

	void openDoor()
	{

		Vector3 heading = this.transform.up.normalized;
		RaycastHit2D[] rays = Physics2D.RaycastAll (this.transform.position, heading,1.0f);
		//Debug.DrawRay (this.transform.position, heading* 1.0f,Color.red);

		foreach (RaycastHit2D ray in rays) {
			if (ray.collider == null) {

			} else {
				if (ray.collider.gameObject.tag == "Door") {
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

					if (npc.npcB.myType == myController.npcB.myType) {
						if (npc.pmc.frozen == false && npc.pmc.movedThisFrame == true) {
							myController.pmc.setStopped ();
						} 
					}
				} else if (ray.collider.gameObject.tag == "Walls" || ray.collider.gameObject.tag=="ImpassableObject" || ray.collider.gameObject.tag=="Car") {
					myController.pf.increaseStuckCounter ();
				}
			}
		}
		//Physics2D.queriesStartInColliders = true;

	}
}
