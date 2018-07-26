using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour_InvestigateObject : NPCBehaviour {
	public GameObject target;
	bool falseAlarm=false;
	bool atTarget=false;
	Inventory myInv;
	bool confiscate = true;
	bool item=false;
	public override void Initialise()
	{

		myType = behaviourType.investigate;

		if (target.GetComponent<Item> () == true) {
			item = true;
		} else {
			item = false;
		}

		myController.pf.getPath (this.gameObject, target);
		radioMessageOnStart ();
		isInitialised = true;
	}

	public override void OnUpdate()
	{

		if (isInitialised == false) {
			myController = this.gameObject.GetComponent<NPCController> ();
			myController.skipAiCheckOnFrame = true;
			Initialise ();
		}

		if (target == null) {
			
		} else {
			if (atTarget == false) {
				if (myController.pf.target != target) {
					myController.pf.getPath (this.gameObject, target);
				}
				if (Vector3.Distance (this.transform.position, target.transform.position) > 1.0f) {
					moveToCurrentPoint ();
				} else {
					atTarget = true;
					atItem ();
				}

			} else {
				if (confiscate == true) {
					moveToCurrentPoint ();

					if (Vector3.Distance (this.transform.position, LevelController.me.itemDepositLoc.position) < 2.0f) {
						OnComplete ();

					}
				}
			}

			objectMoniter ();
		}
		//do some shit
	}
	bool lookedAtObject=false;

	void decideWhatToDoWithObject()
	{
		if (target == null) {
			confiscate = false;
			falseAlarm = true;
		} else if (target.gameObject.tag == "NPC" || target.gameObject.tag == "Player") {
			confiscate = false;
			//not sure what to do with this
		} else if (target.GetComponent<Item> () == true) {
			if (target.GetComponent<ThrowableItem> () == true) {
				falseAlarm = true;

				myController.memory.raiseAlarm = true;

			} else if (target.gameObject.GetComponent<Weapon> () == true || target.gameObject.GetComponent<AmmoItem> () == true || target.gameObject.GetComponent<HighValueItem> () == true) {
				//found an item that should set off the alarm,
				falseAlarm = false;
				confiscate = true;
				//myController.npcB.doing = whatAiIsDoing.raisingAlarm;
			} else if (target.gameObject.GetComponent<PortableContainerItem> () == true) {
				PortableContainerItem pc = target.GetComponent<PortableContainerItem> ();
				foreach (Item i in pc.myContainer.itemsInContainer) {
					if (i.gameObject.GetComponent<Weapon> () == true || i.gameObject.GetComponent<AmmoItem> () == true || i.gameObject.GetComponent<HighValueItem> () == true) {
						//found an item in a container that should set off the alarm,
						falseAlarm = false;
						confiscate = true;
						//myController.memory.raiseAlarm = true;
						//myController.npcB.doing = whatAiIsDoing.raisingAlarm;

					}
				}
			} else {
				confiscate = true;
				falseAlarm = true;
			}
		} else {
			confiscate = false;
			falseAlarm = true;
		}


	}

	void objectMoniter()
	{
		if (item == true) {
			if (myController.memory.objectThatMadeMeSuspisious == null) {

			} else {
				if (myController.memory.objectThatMadeMeSuspisious.activeInHierarchy == false && myController.memory.objectThatMadeMeSuspisious.transform.root != this.gameObject.transform) {
					myController.memory.objectThatMadeMeSuspisious = null;
					Destroy (this);
				}
			}
		} else {
			myController.memory.objectThatMadeMeSuspisious = null;

			Destroy (this);
		}

		//add some kind of progress moniter to the investigating of the item
		/*if (atTarget == false) {
			if (Vector3.Distance (target.transform.position, this.transform.position) < 1.2f) {
				////////Debug.Log ("Near target to investigate");

				if (lookedAtObject == false) {
					if (target.gameObject.tag == "Dead/Knocked" || target.gameObject.tag == "Dead/Guarded") {
						confiscate = false;
					} else if (target.gameObject.tag == "Player" || target.gameObject.tag == "NPC") {
						confiscate = false;
						if (LevelController.me.suspects.Contains (target) == true) {
							myController.npcB.alarmed = true;
						} else {
							NPCBehaviour search = this.gameObject.AddComponent<NPCBehaviour_SearchPerson> ();
							search.passInGameobject (target);
							myController.currentBehaviour = search;
							Destroy (this);
						}
					}
					else if (target.GetComponent<Item> ().illigal == true) {
					
					} else if (target.gameObject.GetComponent<Weapon> () == true || target.gameObject.GetComponent<AmmoItem> () == true || target.gameObject.GetComponent<HighValueItem> () == true) {
						//found an item that should set off the alarm,
						//////Debug.Log ("Found suspious item");
						falseAlarm = true;
						myController.memory.raiseAlarm = true;

					} else if (target.gameObject.GetComponent<PortableContainerItem> () == true) {
						PortableContainerItem pc = target.GetComponent<PortableContainerItem> ();
						foreach (Item i in pc.myContainer.itemsInContainer) {
							if (i.gameObject.GetComponent<Weapon> () == true || i.gameObject.GetComponent<AmmoItem> () == true || i.gameObject.GetComponent<HighValueItem> () == true) {
								//found an item in a container that should set off the alarm,
								//////Debug.Log ("Found suspious item");
								falseAlarm = true;
								myController.memory.raiseAlarm = true;
							}
						}
					} else {
						falseAlarm = true;
						confiscate = false;
						//////Debug.Log ("Item was a false alarm");
					}
					lookedAtObject = true;
				}


				if (confiscate == true) {
					if (pickedUp () == false) {
						return;
					}

					atItem ();
				} else {
					OnComplete ();
				}
			}
		} else {
			atPoint ();
		}*/
	}

	void moveToCurrentPoint()
	{
		myController.pmc.rotateToFacePosition (myController.pf.getCurrentPoint());
		myController.pmc.moveToDirection (myController.pf.getCurrentPoint());
	}

	void atPoint()
	{
		if (Vector3.Distance (this.transform.position, LevelController.me.itemDepositLoc.position) < 1.0f) {
			myInv.dropItem(target.GetComponent<Item>());
			OnComplete ();
		}
	}

	float pickUpTimer = 1.0f;
	ProgressBar myProgress;//will need to work out way of destroying progress bar if the NPC dies during the search

	bool pickedUp()
	{
		if (myProgress == null) {
			myController.myText.setText ("Whats this?");
			createProgressBar();
		}
		myProgress.currentValue = 1.0f - pickUpTimer;
		pickUpTimer -= Time.deltaTime;
		if (pickUpTimer <= 0) {
			Destroy (myProgress.gameObject);
			return true;
		} else {
			return false;
		}
	}

	public void createProgressBar()
	{
		GameObject g = (GameObject)Instantiate (CommonObjectsStore.me.progressBar, Vector3.zero, Quaternion.Euler (0, 0, 0));
		g.transform.parent = GameObject.FindGameObjectWithTag ("MainCamera").GetComponentInChildren<Canvas> ().gameObject.transform;
		myProgress = g.GetComponent<ProgressBar> ();
		myProgress.maxValue = 1.0f;
		myProgress.myObjectToFollow = this.gameObject;
		g.transform.localScale = new Vector3 (1, 1, 1);
	}

	void atItem()
	{
		decideWhatToDoWithObject ();
		//////Debug.Log ("Calling investigate on complete");
		atTarget=true;

		if (falseAlarm == true) {
			//go back to patrol
			//LevelController.me.getRoomObjectIsIn (this.gameObject).itemsInRoomAtStart.Add (target.GetComponent<Item>());
			//////Debug.LogError ("Adding item to room alarm");

			//NPCBehaviour_PatrolRoute newBehaviour = this.gameObject.AddComponent<NPCBehaviour_PatrolRoute>();
			//myController.currentBehaviour = newBehaviour;

			//myController.memory.objectThatMadeMeSuspisious = null;
			myController.memory.suspisious = false;
			myController.npcB.suspisious=false;
			//Destroy (this);
			RoomScript r = LevelController.me.getRoomObjectIsIn (myController.memory.objectThatMadeMeSuspisious);
			if (r == null) {
				confiscate = false;
				OnComplete ();
				Debug.Log ("Item result 1");

			} else {
				if (r.itemsInRoomAtStart.Contains (myController.memory.objectThatMadeMeSuspisious.GetComponent<Item> ()) == false) {
					myInv = this.gameObject.GetComponent<Inventory> ();
					myInv.addItemToInventory (target.GetComponent<Item> ());
					myController.pf.getPath (this.gameObject, LevelController.me.itemDepositLoc.gameObject);
					Debug.Log ("Item result 3");

				} else {//should hopefully eliminate the case where they would keep picking up the same object when 
					OnComplete ();
					Debug.Log ("Item result 2");

				}
				myController.myText.setText ("Nothing to worry about, better take it to lost & found.");
			}
		} else {
			//take item to security room then raise alarm based on item found

			//get rid of this when actual behaviours are added
			//LevelController.me.getRoomObjectIsIn (this.gameObject).itemsInRoomAtStart.Add (target.GetComponent<Item>());
			//////Debug.LogError ("Item is being confiscated");
			//NPCBehaviour_PatrolRoute newBehaviour = this.gameObject.AddComponent<NPCBehaviour_PatrolRoute>();
			//myController.currentBehaviour = newBehaviour;

			//myController.memory.objectThatMadeMeSuspisious = null;
			//myController.memory.suspisious = false;
			//myController.npcB.suspisious=false;

			RoomScript r = LevelController.me.getRoomObjectIsIn (myController.memory.objectThatMadeMeSuspisious);
			if (r == null) {
				confiscate = false;
				OnComplete ();
				Debug.Log ("Item result 1");
			} else {
				if (r.itemsInRoomAtStart.Contains (myController.memory.objectThatMadeMeSuspisious.GetComponent<Item> ()) == false) {
					myInv = this.gameObject.GetComponent<Inventory> ();
					myInv.addItemToInventory (target.GetComponent<Item> ());
					myController.pf.getPath (this.gameObject, LevelController.me.itemDepositLoc.gameObject);
				} else {//should hopefully eliminate the case where they would keep picking up the same object when 
					OnComplete ();
					Debug.Log ("Item result 2");

				}
				myController.myText.setText ("This shouldn't be here.");
			}

		}
	}

	public override void OnComplete()
	{
		if (confiscate == true) {
			RoomScript r = LevelController.me.getRoomObjectIsIn (myController.memory.objectThatMadeMeSuspisious);
			if (r.itemsInRoomAtStart.Contains (myController.memory.objectThatMadeMeSuspisious.GetComponent<Item> ()) == false) {
				r.itemsInRoomAtStart.Add (myController.memory.objectThatMadeMeSuspisious.GetComponent<Item> ());
			}

		}
		if (target == null) {

		} else {
			if (target.GetComponent<PortableContainerItem> () == true) {
				Debug.Log ("Dropping portable container");
				myController.inv.unequipItem (target.GetComponent<Item> ());
				myController.inv.dropItem (target.GetComponent<Item> ());
				//	myController.memory.objectThatMadeMeSuspisious.GetComponent<Item> ().unequipItem ();
				target.GetComponent<Item> ().dropItem ();

			} else if (target.GetComponent<Item> () == true) {
				myController.inv.dropItem (target.GetComponent<Item> ());

				target.GetComponent<Item> ().dropItem ();
			}
		}

		if (falseAlarm == false) {
			myController.memory.raiseAlarm = true;

		}
		//myController.memory.suspisious = false;
		//myController.npcB.suspisious=false;
		myController.memory.objectThatMadeMeSuspisious = null;
		radioMessageOnFinish ();
		Debug.Log ("destroyed item on finish");
		Destroy (this);

	}

	public override void passInGameobject(GameObject passIn)
	{
		target = passIn;
		//////Debug.Log ("Passed in " + passIn.name + " to investigate");
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

			RoomScript rs = LevelController.me.getRoomObjectIsIn (myController.memory.objectThatMadeMeSuspisious);

			if (rs == null) {
				PhoneTab_RadioHack.me.setNewText ("This is "+this.gameObject.name+", I've found something odd, checking it out.",h);

			} else {
				PhoneTab_RadioHack.me.setNewText ("This is "+this.gameObject.name+ ", I'm moving to " + rs.roomName,h);

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


			if (confiscate == true) {
				PhoneTab_RadioHack.me.setNewText ("This is " + this.gameObject.name + ", I've confiscated the object, its in the security office.", h);
			} else {
				PhoneTab_RadioHack.me.setNewText ("This is " + this.gameObject.name + ", It was nothing to worry about.", h);

			}


		}
	}


}
