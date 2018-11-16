using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour_SearchPerson : NPCBehaviour {
	//todo on waking up redo AI , start implementing suspision mechanic (have in the class that does all the raycasting?)


	public GameObject personToFind;
	bool reachedPerson = false,personRunOff=false,personHadIlligalItem=false,personInnocent=false;
	ProgressBar myProgress;
	public float timer = 5.0f;
	public override void Initialise ()
	{
		myType = behaviourType.searchPerson;
		if (personToFind == null) {

		} else {
			myController = this.gameObject.GetComponent<NPCController> ();

			myController.pf.getPath (this.gameObject, personToFind);
			myController.skipAiCheckOnFrame = true;
			radioMessageOnStart ();
			isInitialised = true;
		}
	}

	// Use this for initialization
	void Start () {
		
	}

	public override void OnUpdate ()
	{
		if (isInitialised == false) {
			Initialise ();
			return;
		}

		if (reachedPerson == false) {

			if (nearObject () == false) {
				moveToCurrentPoint ();
				myController.pwc.aimDownSight = false;

			} else {
				reachedPerson = true;
			}

		} else {

			if (nearObject () == false) {
				personRunOff = true;
			}
			if (myProgress == null) {
				myController.myText.setText ("I need to search you.");

				createProgressBar ();
			} else {
				myController.pmc.rotateToFacePosition (personToFind.transform.position);
				myController.pwc.aimDownSight = true;

				if (personRunOff == true) {
					OnComplete ();
				}

				if (timer > 0) {
					timer -= Time.deltaTime;
					myProgress.currentValue = (myProgress.maxValue - timer);
				} else {
					myProgress.currentValue = myProgress.maxValue;
					searchThroughInventory ();
				}
			}
		}
	}
	


	public void searchThroughInventory()
	{
		////////Debug.Log ("Searching through inventory");
		Inventory i = personToFind.GetComponent<Inventory> ();
		PortableContainerItem pt = personToFind.GetComponentInChildren<PortableContainerItem> ();

		foreach (Item it in i.inventoryItems) {
			if (it.illigal == true) {
				personHadIlligalItem = true;
			}
		}

		if (pt == null) {

		} else {
			foreach(Item it in pt.myContainer.itemsInContainer)
			{
				if(it.illigal==true){
					personHadIlligalItem=true;
				}
			}

		}
		OnComplete ();
	}

	public void createProgressBar()
	{
		GameObject g = (GameObject)Instantiate (CommonObjectsStore.me.progressBar, Vector3.zero, Quaternion.Euler (0, 0, 0));
		g.transform.parent = GameObject.FindGameObjectWithTag ("MainCamera").GetComponentInChildren<Canvas> ().gameObject.transform;
		myProgress = g.GetComponent<ProgressBar> ();
		myProgress.maxValue = timer;
		myProgress.myObjectToFollow = this.gameObject;
		g.transform.localScale = new Vector3 (1, 1, 1);
	}

	public override void OnComplete ()
	{
		radioMessageOnFinish ();
		if (personHadIlligalItem == true || personRunOff == true) {
			myController.memory.raiseAlarm = true;
			myController.memory.seenSuspect = true;
			////////Debug.Log ("Illigal item found");
			myController.myText.setText ("On your knees!");

			Destroy (myProgress.gameObject);
			//myController.npcB.alarmed = true;

			//NPCBehaviour_UpdateSuspects npc = this.gameObject.AddComponent<NPCBehaviour_UpdateSuspects> ();
			//myController.currentBehaviour = npc;
			Destroy (this);
			//NPCBehaviour_AttackTarget nat = this.gameObject.AddComponent<NPCBehaviour_AttackTarget> ();
			//nat.passInGameobject (personToFind);
			//myController.currentBehaviour = nat;
			//Destroy (this);
		} else {
			//NPCBehaviour nat = this.gameObject.AddComponent<NPCBehaviour_PatrolRoute> ();
			myController.myText.setText ("Sorry, carry on.");
			Destroy (myProgress.gameObject);
			//myController.npcB.suspisious = false;
			myController.memory.objectThatMadeMeSuspisious=null;
			myController.memory.suspisious = false;
			myController.npcB.suspisious = false;
			//nat.passInGameobject (personToFind);
			//myController.currentBehaviour = nat;
			//Destroy (myProgress.gameObject);
			////////Debug.Log ("No illigal items found");
			Destroy (this);

		}
	}

	void OnDestroy()
	{
		if (myProgress == null) {
			
		} else {
			Destroy (myProgress.gameObject);
		}
	}

	void moveToCurrentPoint()
	{
		myController.pmc.rotateToFacePosition (myController.pf.getCurrentPoint());
		myController.pmc.moveToDirection (myController.pf.getCurrentPoint());
	}
	public bool nearObject()
	{
		if (Vector3.Distance (this.transform.position, personToFind.transform.position) < 2.0f && myController.detect.fov.visibleTargts.Contains(personToFind.transform)) {
			return true;
		}
		return false;
	}

	public override void passInGameobject (GameObject passIn)
	{
		personToFind = passIn;
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


			PhoneTab_RadioHack.me.setNewText ("This is "+this.gameObject.name+ ", Someone looks suspicious, I'm going to search them.",h);


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

			if (personRunOff == true) {
				PhoneTab_RadioHack.me.setNewText ("They ran off, I'm going after them.", h);

			} else if (personHadIlligalItem == true) {
				PhoneTab_RadioHack.me.setNewText ("They had something, I'm taking them down.", h);

			} else {
				PhoneTab_RadioHack.me.setNewText ("This is " + this.gameObject.name + ", false alarm guys.", h);

			}



		}
	}
}
