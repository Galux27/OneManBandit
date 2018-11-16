using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour_ArrestTarget : NPCBehaviour {
	public GameObject target;
	public PersonWeaponController pwc;
	Vector3 targetStartLoc = Vector3.zero;
	public bool setText = false;
	public override void Initialise ()
	{
		myType = behaviourType.arrestTarget;
		myController = this.gameObject.GetComponent<NPCController> ();
		pwc = target.GetComponent<PersonWeaponController> ();
		myController.detect.target = target;
		targetStartLoc = target.transform.position;

		if (myController.memory.objectThatMadeMeSuspisious == null) {
		} else {
			if (myController.memory.objectThatMadeMeSuspisious.tag == "NPC") {
				NPCController npc = myController.memory.objectThatMadeMeSuspisious.GetComponent<NPCController> ();
				if (myController.npcB.freindlyIDs.Contains (npc.npcB.myID) == true || myController.npcB.myID != npc.npcB.myID) {
					myController.npcB.suspisious = false;
					////////Debug.LogError ("NPC " + this.gameObject.name + " wanted to set alert on freindly npc " + myController.memory.objectThatMadeMeSuspisious);

					myController.memory.objectThatMadeMeSuspisious = null;
					Destroy (this);
				}
			}
		}
		radioMessageOnStart ();
		isInitialised = true;
	}

	public override void OnUpdate ()
	{
		if (isInitialised == false) {
			Initialise ();
		}
		myController.pmc.rotateToFacePosition (target.transform.position);
		if (Vector3.Distance (this.transform.position, target.transform.position) > 3.0f) {
			myController.pmc.moveToDirection (target.transform.position);
			myController.pwc.aimDownSight = false;
		} else {
			if (setText == false) {
				myController.myText.setText ("Hands up! You're under arrest!");
				setText = true;
			}
			myController.pwc.aimDownSight = true;
		}

		if (shouldWeStillArrestTarget () == false) {
			myController.npcB.attemptToArrest = false;
			myController.npcB.alarmed = true;
			radioMessageOnFinish ();
		}
	}

	bool shouldWeStillArrestTarget()
	{
		if (pwc.currentWeapon == null && myController.detect.fov.visibleTargts.Contains(target.transform)==true && Vector3.Distance(target.transform.position,targetStartLoc)<2.0f) {
			return true;
		} else {
			return false;
		}
	}

	public override void OnComplete ()
	{

	}

	public override void passInGameobject (GameObject passIn)
	{
		target = passIn;
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
			if (shouldWeStillArrestTarget () == false) {
				PhoneTab_RadioHack.me.setNewText ("Suspect hostile, I repeat, suspect hostile, take them down!", h);
			} else {
				PhoneTab_RadioHack.me.setNewText ("Suspect surrendered, we're taking them in.", h);
			}
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

			PhoneTab_RadioHack.me.setNewText ("This is " + this.gameObject.name + ", moving to arrest suspect.",h);
		}
	}
}
