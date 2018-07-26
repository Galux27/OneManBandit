using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour_GuardCorpse : NPCBehaviour {

	GameObject person;
	bool complete = false;
	bool guarded = false;
	//public static List<GameObject> objectsThatAreGuarded = new List<GameObject>();
	//public List<GameObject> corpseDisplay;
	//public bool iAddedToList = false;
	public override void Initialise ()
	{

		myType = behaviourType.guardCorpse;
		myController = this.gameObject.GetComponent<NPCController> ();
		person = myController.memory.objectThatMadeMeSuspisious;

		if (doesCorpseNeedGuarding()==false) {
			//////Debug.Log (this.gameObject.name + " Tried to guard an already guarded corpse");
			corpseAlreadyGuarded ();
		}
		//corpseDisplay = objectsThatAreGuarded;

		myController.pf.getPath (this.gameObject, person);
		isInitialised = true;
	}

	public override void OnUpdate ()
	{
		if (isInitialised == false) {
			Initialise ();
		}
	//	corpseDisplay = objectsThatAreGuarded;
		///if (myController.memory.objectThatMadeMeSuspisious.tag ==  "Dead/Guarded") {
		//	corpseAlreadyGuarded ();
		//}

		if (person.gameObject.tag == "NPC") {
			Destroy (this);
		}

		if (isNearLoc () == false) {
			moveToCurrentPoint ();
		} else {
			if (complete == false) {
				OnComplete ();
			}
		}
	}

	public override void OnComplete ()
	{

		if (doesCorpseNeedGuarding()==false) {

			corpseAlreadyGuarded ();
		} else {
			//if (myController.memory.objectThatMadeMeSuspisious.tag == "Dead/Knocked") {
			person.gameObject.tag = "Dead/Guarded";
			NPCManager.me.corpsesInWorld.Remove (person);
			myController.myText.setText ("Better watch this till homocide get here");

				//myController.memory.objectThatMadeMeSuspisious.tag = "Dead/Guarded";
			radioMessageOnFinish();
			//NPCManager.me.refreshArrays ();
			complete = true;
			//}// else {
			//	corpseAlreadyGuarded ();
			//}
		}


		//myController.npcB.suspisious = false;
		//myController.memory.objectThatMadeMeSuspisious = null;
		//if (NPCBehaviourDecider.globalAlarm == false) {
		//	myController.npcB.alarmed = true;
		//}

	}

	void corpseAlreadyGuarded()
	{
		guarded = true;
		myController.npcB.suspisious = false;
		myController.memory.suspisious = false;
		myController.memory.objectThatMadeMeSuspisious = null;
		//////Debug.Break ();
		Destroy (this);
	}

	bool isNearLoc()
	{
		if (Vector3.Distance (this.transform.position, person.transform.position) < 1.5f) {
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

	void OnDestroy()
	{
		if (guarded == false) {
			//objectsThatAreGuarded.Remove (person);
			NPCManager.me.corpsesInWorld.Add (person);

			person.gameObject.tag = "Dead/Knocked";
			//////Debug.Log ("reseting corpse tag");
		}
	}

	bool doesCorpseNeedGuarding()
	{
		List<NPCBehaviour_GuardCorpse> instances = new List<NPCBehaviour_GuardCorpse> ();
		foreach (GameObject g in NPCManager.me.npcsInWorld) {
			if (g.GetComponent<NPCBehaviour_GuardCorpse> () == true) {
				instances.Add (g.GetComponent<NPCBehaviour_GuardCorpse> ());
			}
		}

	//	NPCBehaviour_GuardCorpse[] instances = FindObjectsOfType<NPCBehaviour_GuardCorpse> ();
		foreach (NPCBehaviour_GuardCorpse behav in instances) {
			if (behav != this) {
				if (behav.person == person) {
					return false;
				}
			}
		}
		return true;
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

			PhoneTab_RadioHack.me.setNewText ("We've got a trespasser in ",h);
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


			RoomScript rs = LevelController.me.getRoomObjectIsIn (person);

			if (rs == null) {
				PhoneTab_RadioHack.me.setNewText ("This is "+this.gameObject.name+", I'm watching the corpse until the building is secure.",h);

			} else {
				PhoneTab_RadioHack.me.setNewText ("This is "+this.gameObject.name+ ", I'm watching the corpse in " + rs.roomName,h);

			}
		}
	}
}



