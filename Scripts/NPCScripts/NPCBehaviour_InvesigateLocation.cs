using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour_InvesigateLocation : NPCBehaviour {
	public Vector3 location;
	float waitTimer = 5.0f;
	public bool gotToLocation = false,investigated=false,gotRoom=false;
	RoomScript room;
	public GameObject pointToInvestigate;
	public int pointsGoneTo=0;
	GameObject marker;
	public override void Initialise ()
	{
		myType = behaviourType.investigate;
		myController = this.gameObject.GetComponent<NPCController> ();
		if (location == Vector3.zero) {
			location = myController.memory.noiseToInvestigate;
		}
		if (marker == null) {
			marker = new GameObject ("investigate marker");//(GameObject)Instantiate (, location, Quaternion.Euler (0, 0, 0));
			marker.transform.position = location;
			pointToInvestigate = marker;
		}
		//Debug.Break ();
		myController.pf.getPath (this.gameObject, marker);
		radioMessageOnStart ();
		isInitialised = true;
	}

	public override void OnUpdate ()
	{
		if (isInitialised == false) {
			Initialise ();
		}

		if (pointToInvestigate == null) {
			
			myController.npcB.suspisious = false;
			myController.memory.noiseToInvestigate = Vector3.zero;
			myController.npcB.doing = whatAiIsDoing.starting;
			Destroy (this);
		}

		if (isNearLoc () == false && gotToLocation==false) {
			moveToCurrentPoint ();
			myController.pwc.aimDownSight = false;
		} else {
			if (investigated == false) {
				if (gotRoom == false) {
					room = LevelController.me.getRoomObjectIsIn (this.gameObject);
					gotRoom = true;
				}

				if (room == null) {
					
					myController.npcB.suspisious = false;
					myController.memory.noiseToInvestigate = Vector3.zero;
					Destroy (this);
				} else {
					if (room.pointsInRoom.Count == 0) {
						
						myController.npcB.suspisious = false;
						myController.memory.noiseToInvestigate = Vector3.zero;
						Destroy (this);
					} else {
						if (pointsGoneTo < 4) {
							if (pointToInvestigate == null || myController.pf.target != pointToInvestigate) {
								pointToInvestigate = room.getRandomPoint ();
								myController.pf.getPath (this.gameObject, pointToInvestigate);
							}

							if (Vector2.Distance (this.transform.position, pointToInvestigate.transform.position) >1.5f) {
								moveToCurrentPoint ();
							} else {
								pointToInvestigate = room.getRandomPoint ();
								myController.pf.getPath (this.gameObject, pointToInvestigate);

								pointsGoneTo++;
							}
						} else {
							
							myController.npcB.suspisious = false;
							myController.memory.noiseToInvestigate = Vector3.zero;
							myController.npcB.doing = whatAiIsDoing.starting;
							Destroy (this);
						}
					}
				}
			}

			/*	if (waitTimer <= 0) {
					myController.npcB.suspisious = false;
					myController.memory.noiseToInvestigate = Vector3.zero;
					Destroy (this);
				} else {
					waitTimer -= Time.deltaTime;
					myController.pmc.rotateToFacePosition (location);

				}*/
		}
	}


	bool isNearLoc()
	{
		if (Vector2.Distance (this.transform.position, location) < 1.0f ||Vector2.Distance (this.transform.position, location) < 2.5f && lineOfSightToTarget()) {
			gotToLocation = true;
			return true;
		} else {
			return false;
		}
	}

	public bool lineOfSightToTarget()
	{


		Vector3 origin = pointToInvestigate.transform.position;


		Vector3 heading = this.transform.position - origin;
		RaycastHit2D ray = Physics2D.Raycast (origin, heading);
		Debug.DrawRay (origin, heading,Color.cyan);

		if (ray.collider == null) {

		} else {
			//			//////Debug.Log ("Ray hit object with tag " + ray.collider.gameObject.tag);
			if (ray.collider.gameObject.transform.root == this.transform.root) {
				return true;
			}
		}
		return false;
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



				PhoneTab_RadioHack.me.setNewText ("This is "+this.gameObject.name+ ", I'm on the move. ",h);

		}
	}
}
