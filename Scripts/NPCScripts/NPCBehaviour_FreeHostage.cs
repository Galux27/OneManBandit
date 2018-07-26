using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour_FreeHostage  : NPCBehaviour {
	GameObject hostage;
	NPCController hostageController;
	ProgressBar myProgress;
	float timer = 2.5f;
	public override void OnUpdate ()
	{
		if (isInitialised == false) {
			Initialise ();
		}
		freedCheck ();
		if (isNearLoc () == false) {
			moveToCurrentPoint ();
		} else {
			//OnComplete ();
			if (myProgress == null) {
				createProgressBar ();
			} else {
				if (timer > 0) {
					timer -= Time.deltaTime;
					myProgress.currentValue = (myProgress.maxValue - timer);
					myController.pmc.rotateToFacePosition (hostage.transform.position);

				} else {
					myProgress.currentValue = myProgress.maxValue;
					OnComplete ();
				}
			}
		}
	}

	void freedCheck()
	{
		if (hostageController.npcB.myType != AIType.hostage && hostageController.knockedDown==false) {
			OnComplete ();
		}
	}

	public override void Initialise ()
	{
		myType = behaviourType.freeHostage;

		myController = this.gameObject.GetComponent<NPCController> ();

		//if (myController.memory.objectThatMadeMeSuspisious == null) {
		//	Destroy (this);
		//}

		hostage = myController.memory.objectThatMadeMeSuspisious;
		hostageController = hostage.GetComponent<NPCController> ();
		myController.pf.getPath (this.gameObject, 	hostage);
		radioMessageOnStart ();
		isInitialised = true;
	}

	bool isNearLoc()
	{
		if (Vector3.Distance (this.transform.position, hostage.transform.position) < 1.5f) {
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

	public override void OnComplete ()
	{
		myController.memory.objectThatMadeMeSuspisious = null;
		//myController.myText.setText ("Gotta get out of here");
		hostageController.untieHostage();
		hostageController.knockedDownTimer = -1.0f;
		//LevelController.me.alertLevel = 2;
		//NPCBehaviourDecider.globalAlarm = true;
		//NPCBehaviourDecider.copAlarm = true;
		//myController.npcB.alarmed = true;
		//myController.npcB.suspisious = false;
		//myController.memory.objectThatMadeMeSuspisious = null;
		radioMessageOnFinish();
		//if (myController.npcB.myType == AIType.guard && NPCBehaviourDecider.globalAlarm == false) {
			//Destroy (myController.currentBehaviour);
			//NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_RaiseAlarm> ();
			//myController.currentBehaviour = nb;
		//} else if (myController.npcB.myType == AIType.cop && NPCBehaviourDecider.copAlarm == false) {
			//Destroy (myController.currentBehaviour);
			//NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_CopRaiseAlarm> ();
			//myController.currentBehaviour = nb;
		//} else {
			Destroy (this);
		//}

		////////Debug.Log ("Set off global alarm");

	}

	public void createProgressBar()
	{
		if (myController.knockedDown == false) {
			myController.myText.setText ("Stay still, I'll untie you.");
		} else {
			myController.myText.setText ("Hey, Wake up.");

		}

		GameObject g = (GameObject)Instantiate (CommonObjectsStore.me.progressBar, Vector3.zero, Quaternion.Euler (0, 0, 0));
		g.transform.parent = GameObject.FindGameObjectWithTag ("MainCamera").GetComponentInChildren<Canvas> ().gameObject.transform;
		myProgress = g.GetComponent<ProgressBar> ();
		myProgress.maxValue = timer;
		myProgress.myObjectToFollow = this.gameObject;
		g.transform.localScale = new Vector3 (1, 1, 1);
	}

	void OnDestroy()
	{
		if (myProgress == null) {

		} else {
			Destroy (myProgress.gameObject);
		}
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
			RoomScript rs = LevelController.me.getRoomObjectIsIn (hostage);

			if (rs == null) {
				if (hostageController.knockedDown == false) {
					PhoneTab_RadioHack.me.setNewText ("This is " + this.gameObject.name + ", I've got a hostage here", h);
				} else {
					PhoneTab_RadioHack.me.setNewText ("This is " + this.gameObject.name + ", I've got an injured person here.", h);

				}
			} else {

				if (hostageController.knockedDown == false) {
					PhoneTab_RadioHack.me.setNewText ("This is "+this.gameObject.name+"We've got a hostage in " + rs.roomName,h);
				} else {
					PhoneTab_RadioHack.me.setNewText ("This is "+this.gameObject.name+"We've got an injured person in " + rs.roomName,h);

				}


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

		
			PhoneTab_RadioHack.me.setNewText ("This is "+this.gameObject.name+", The hostage is free", h);


		}
	}
}
