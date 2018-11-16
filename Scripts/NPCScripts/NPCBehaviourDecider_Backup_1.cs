using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviourDecider_Backup_1 : MonoBehaviour {
/// <summary>
/// Backup of NPC behaviour decider, unused.
/// </summary>
	public AIType myType;
	public NPCController myController;

	//state booleans
	public bool alarmed = false,combat=false,suspisious=false;

	//world state booleans
	public static bool globalAlarm=false,copAlarm=false;
	//guard booleans
	public bool patrol=false;

	//police booleans
	public bool attemptToArrest = true;

	public string myID;
	public List<string> freindlyIDs;
	public float loseTargetTimer = 5.0f;

	public bool attackedRecently = false;
	public float lastTimeIWasAttacked = 0.0f;
	void Awake()
	{
		myController = this.gameObject.GetComponent<NPCController> ();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (myController.knockedDown == true || PersonHealth.playerHealth.healthValue<=0) {
			return;
		}
		//////////Debug.Log (myController.detect.getDotProduct (CommonObjectsStore.player));
		if (myType == AIType.guard) {
			unfreezeNPC ();
			decideViewRadius ();
			checkForArmedNPCs ();

			if (attackedRecently == true) {
				alarmed = true;
				if (Time.time - lastTimeIWasAttacked >= 5.0f&& myController.detect.fov.visibleTargts.Contains(myController.memory.objectThatMadeMeSuspisious.transform)==false) {
					attackedRecently = false;
					//alarmed = true;
				}
				//return;
			}

			if (suspisious == false && alarmed == false) {
				guardPassive ();
			} else if (alarmed == true) {
				guardAggressive ();
			} else if (suspisious == true) {
				guardSuspisious ();
			}
			shouldNPCBeArmed ();
		} else if (myType == AIType.cop) {
			unfreezeNPC ();

			decideViewRadius ();

			checkForArmedNPCs ();

			if (attackedRecently == true) {
				alarmed = true;
				if (Time.time - lastTimeIWasAttacked >= 5.0f&& myController.detect.fov.visibleTargts.Contains(myController.memory.objectThatMadeMeSuspisious.transform)==false) {
					attackedRecently = false;
					//alarmed = true;
				}
				//return;
			}

			if (suspisious == false && alarmed == false) {
				copPassive ();
			} else if (alarmed == true) {
				copAlarmed ();
			} else if (suspisious == true) {
				copSuspicious ();
			}
			shouldNPCBeArmed ();
		} else if (myType == AIType.swat) {
			unfreezeNPC ();

			decideViewRadius ();

			if (attackedRecently == true) {
				if (Time.time - lastTimeIWasAttacked >= 5.0f&& myController.detect.fov.visibleTargts.Contains(myController.memory.objectThatMadeMeSuspisious.transform)==false) {
					attackedRecently = false;
					alarmed = true;
				}
				//return;
			}

			swatPassive ();
		} else if (myType == AIType.civilian) {
			decideViewRadius ();
			unfreezeNPC ();



			if (alarmed == false) {
				civilianPassive ();
			} else {
				civilianAlarmed ();
			}
		} else if (myType == AIType.hostage) {

		}
	}

	void decideViewRadius()
	{

		if (myType == AIType.guard) {
			if (alarmed == true) {
				if (myController.detect.fov.viewRadius <= myController.detect.fov.startingViewRadius + 5) {
					myController.detect.fov.viewRadius = myController.detect.fov.startingViewRadius + 5;
				}

				if (LevelController.me.suspects.Contains (CommonObjectsStore.player)) {
					myController.detect.fov.shaderMaterial = CommonObjectsStore.me.aggressive;
				}
			} else if (suspisious == true|| globalAlarm==true) {
				if (myController.detect.fov.viewRadius <= myController.detect.fov.startingViewRadius + 3) {
					myController.detect.fov.viewRadius = myController.detect.fov.startingViewRadius + 3;
					myController.detect.fov.shaderMaterial = CommonObjectsStore.me.suspicious;
				}
			} else {
				if (myController.detect.fov.viewRadius <= myController.detect.fov.startingViewRadius) {
					myController.detect.fov.viewRadius = myController.detect.fov.startingViewRadius;
					myController.detect.fov.shaderMaterial = CommonObjectsStore.me.passive;
				}
			}
		} else if (myType == AIType.cop) {
			if (alarmed == true) {
				if (myController.detect.fov.viewRadius <= myController.detect.fov.startingViewRadius + 5) {
					myController.detect.fov.viewRadius = myController.detect.fov.startingViewRadius + 5;
				}

				if (LevelController.me.suspects.Contains (CommonObjectsStore.player)) {
					myController.detect.fov.shaderMaterial = CommonObjectsStore.me.aggressive;
				}
			} else if (suspisious == true|| copAlarm==true) {
				if (myController.detect.fov.viewRadius <= myController.detect.fov.startingViewRadius + 3) {
					myController.detect.fov.viewRadius = myController.detect.fov.startingViewRadius + 3;
					myController.detect.fov.shaderMaterial = CommonObjectsStore.me.suspicious;
				}
			} else {
				if (myController.detect.fov.viewRadius <= myController.detect.fov.startingViewRadius) {
					myController.detect.fov.viewRadius = myController.detect.fov.startingViewRadius;
					myController.detect.fov.shaderMaterial = CommonObjectsStore.me.passive;
				}
			}
		} else if (myType == AIType.swat) {
			if (alarmed == true ) {
				if (myController.detect.fov.viewRadius <= myController.detect.fov.startingViewRadius + 5) {
					myController.detect.fov.viewRadius = myController.detect.fov.startingViewRadius + 5;
				}

				if (LevelController.me.suspects.Contains (CommonObjectsStore.player)) {
					myController.detect.fov.shaderMaterial = CommonObjectsStore.me.aggressive;
				}
			} else{
				if (myController.detect.fov.viewRadius <= myController.detect.fov.startingViewRadius + 3) {
					myController.detect.fov.viewRadius = myController.detect.fov.startingViewRadius + 3;
					myController.detect.fov.shaderMaterial = CommonObjectsStore.me.suspicious;
				}
			}
		} else if (myType == AIType.civilian) {
			
			if (myController.detect.fov.viewRadius <= myController.detect.fov.startingViewRadius) {
				myController.detect.fov.viewRadius = myController.detect.fov.startingViewRadius;
				myController.detect.fov.shaderMaterial = CommonObjectsStore.me.passive;
			}
		}
	}

	bool shouldWeAttackTarget(GameObject g)
	{
		

		if (myController.memory.objectThatMadeMeSuspisious == null && g==null) {
			return false;
		}

		if (g == null) {
			return shouldWeAttackTarget (myController.memory.objectThatMadeMeSuspisious);
		}

		if (myController.memory.peopleThatHaveAttackedMe.Contains (g) == true) {
			return true;
		}
		return false;

	}

	void unfreezeNPC()
	{
		if (myController.pmc.frozen == true) {
			if (PlayerFieldOfView.me.shouldNPCBeUnfrozen (this.transform) || myController.npcB.attackedRecently==true) {
				myController.pmc.frozen = false;
			}
		}
	}

	void detectWhetherWeShouldBeAlerted()
	{
		/*if (myController.detect.targetDetect (CommonObjectsStore.player) == true) {
			float suspision = decideHowSuspiciousObjectIs (CommonObjectsStore.player);
			RoomScript npcRoom = LevelController.me.getRoomObjectIsIn (CommonObjectsStore.player);

//			////////Debug.Log ("Suspision of player = " + suspision);
			if (suspision < 1.0f) {
				//everything good
				if (myType == AIType.guard) {
					if (npcRoom.traspassing == true) {
						suspisious = true;
						myController.memory.objectThatMadeMeSuspisious = CommonObjectsStore.player;
					}
				}

			} else if (suspision < 7.0f) {
				if (myType == AIType.guard) {
					//if (npcRoom.traspassing == true) {

					//} else {
					//follow player
					suspisious = true;
					myController.memory.objectThatMadeMeSuspisious = CommonObjectsStore.player;
					//}
				} else if (myType == AIType.cop) {
					suspisious = true;
					myController.memory.objectThatMadeMeSuspisious = CommonObjectsStore.player;
				}


			} else {
				//order surrender/attack
				alarmed = true;
				myController.memory.objectThatMadeMeSuspisious = CommonObjectsStore.player;

			}
		}*/



		foreach (Transform npc in myController.detect.fov.visibleTargts) {
			if (npc == null) {
				continue;
			}

			if (npc.gameObject.tag == "NPC") {
				NPCController npcC = npc.gameObject.GetComponent<NPCController> ();
				string npcID = npcC.npcB.myID;
				if (npcC.npcB.myType == AIType.hostage) {
					myController.memory.objectThatMadeMeSuspisious = npc.gameObject;
					suspisious = true;
					break;
				}

				if (freindlyIDs.Contains (npcID) == false && npcID != myID) {
					RoomScript npcRoom = LevelController.me.getRoomObjectIsIn (npc.gameObject);

					float suspision = decideHowSuspiciousObjectIs (npc.gameObject);
					//			////////Debug.Log ("Suspision of player = " + suspision);
					if (suspision < 1.0f) {
						//everything good
						if (myType == AIType.guard) {
							if (npcRoom.traspassingInRoom() == true) {
								suspisious = true;
								myController.memory.objectThatMadeMeSuspisious = npc.gameObject;
							}
						}
					
					} else if (suspision < 7.0f) {
						if (myType == AIType.guard) {
							//if (npcRoom.traspassing == true) {

							//} else {
							//follow player
							suspisious = true;
							myController.memory.objectThatMadeMeSuspisious = npc.gameObject;
							//}
						} else if (myType == AIType.cop) {
							suspisious = true;
							myController.memory.objectThatMadeMeSuspisious = npc.gameObject;
						}


					} else {
						//order surrender/attack
						alarmed = true;
						myController.memory.objectThatMadeMeSuspisious = npc.gameObject;
					}
				}
			} else if (npc.gameObject.tag == "Player") {
				float suspision = decideHowSuspiciousObjectIs (CommonObjectsStore.player);
				RoomScript npcRoom = LevelController.me.getRoomObjectIsIn (CommonObjectsStore.player);

				//			////////Debug.Log ("Suspision of player = " + suspision);
				if (suspision < 1.0f) {
					//everything good
					if (myType == AIType.guard) {
						if (npcRoom==null || npcRoom.traspassingInRoom() == true) {
							suspisious = true;
							myController.memory.objectThatMadeMeSuspisious = CommonObjectsStore.player;
						}
					}

				} else if (suspision < 7.0f) {
					if (myType == AIType.guard) {

						if (npcRoom==null || npcRoom.traspassingInRoom() == true) {
							suspisious = true;
							myController.memory.objectThatMadeMeSuspisious = CommonObjectsStore.player;
						}

						//if (npcRoom.traspassing == true) {

						//} else {
						//follow player
						suspisious = true;
						myController.memory.objectThatMadeMeSuspisious = CommonObjectsStore.player;
						//}
					} else if (myType == AIType.cop) {
						suspisious = true;
						myController.memory.objectThatMadeMeSuspisious = CommonObjectsStore.player;
					}


				} else {
					//order surrender/attack
					alarmed = true;
					myController.memory.objectThatMadeMeSuspisious = CommonObjectsStore.player;

				}

				////////Debug.Log (this.gameObject.name + " is looking at the player " + suspision.ToString ());
					
			}
		}
	}

	void shouldCivilianBeAlarmed()
	{
		foreach (Transform t in myController.detect.fov.visibleTargts) {

			if (t == null) {
				continue;
			}

			if (t.gameObject.tag == "NPC") {
				NPCController npc = t.gameObject.GetComponent<NPCController> ();

				if (freindlyIDs.Contains (npc.npcB.myID) == false && npc.npcB.myID != myID) {
					if (npc.pwc.currentWeapon == null) {

					} else {
						alarmed = true;
					}


				}
			} else if (t.gameObject.tag == "Player") {
				PersonWeaponController pwc = CommonObjectsStore.player.GetComponent<PersonWeaponController> ();

				if (pwc.currentWeapon == null) {

				} else {
					alarmed = true;
				}

				if (PlayerAction.currentAction == null) {
				} else if (PlayerAction.currentAction.illigal == true) {
					alarmed = true;
				}

			}
		}

		foreach (GameObject g in NPCManager.me.corpsesInWorld) { //should probbably find a way for corpses to be detected by the FOV
			if (g.tag != "Dead/Guarded") {
				if (Vector3.Distance(this.transform.position,g.transform.position)<myController.detect.fov.viewRadius) {
					if (myController.detect.isTargetInFrontOfUs (g) == true) {
						if (myController.detect.lineOfSightToTargetWithNoCollider (g) == true) {
							myController.memory.objectThatMadeMeSuspisious = g;
							alarmed = true;
							myController.memory.suspisious = true;
						}
					}
				}
			}
		}
	}

	void detectItems()
	{
		RoomScript myRoom = LevelController.me.getRoomObjectIsIn (this.gameObject);

		if (myRoom == null) {

		} else {
			foreach (Item i in ItemMoniter.me.itemsInWorld) {
				RoomScript itemRoom = LevelController.me.getRoomObjectIsIn (i.gameObject);
				if (i.gameObject.activeInHierarchy == true) {
					if (itemRoom == null || itemRoom.itemsInRoomAtStart.Contains(i)==false) {
						if (myController.detect.areWeNearTarget (i.gameObject)) {
							if (myController.detect.isTargetInFrontOfUs (i.gameObject)) {
								if (myController.detect.lineOfSightToTargetWithNoCollider (i.gameObject)) {
									//////////Debug.Log ("Item " + i.itemName + " was detected");
									myController.memory.objectThatMadeMeSuspisious = i.gameObject;
									suspisious = true;
									myController.memory.suspisious = true;
								} 
							}
						}
					}
				}
			}
		}

		/*foreach (GameObject g in NPCManager.me.corpsesInWorld) {
			if (g.tag != "Dead/Guarded") {
				if (myController.detect.areWeNearTarget (g) == true) {
					if (myController.detect.isTargetInFrontOfUs (g) == true) {
						if (myController.detect.lineOfSightToTargetWithNoCollider (g) == true) {
							myController.memory.objectThatMadeMeSuspisious = g;
							suspisious = true;
							myController.memory.suspisious = true;
						}
					}
				}
			}
		}*/
	}

	public float decideHowSuspiciousObjectIs(GameObject g)
	{
		float retVal = 0.0f;
		Inventory i = g.GetComponent<Inventory> ();
		PersonWeaponController pwc = g.GetComponent<PersonWeaponController> ();

		if (pwc.currentWeapon == null) {

		} else {
			return 999;//player is visibly armed
		}

		if (shouldWeAttackTarget (g)) {
			return 999;
		}

		if (g.tag == "Player") {
			if (i.back == null) {

			} else {
				retVal += 0.5f;
			}

			if (i.leftArm == null) {

			} else {
				if (i.leftArm.illigal == true) {
					retVal += 11;
				}
			}

			if (i.rightArm == null) {

			} else {
				if (i.rightArm.illigal == true) {
					retVal += 11;
				}
			}

			if (i.head == null) {

			} else {
				if (i.head.illigal == true) {
					retVal += 11;
				}
			}

			if (pwc.currentWeapon == null) {

			} else {
				retVal += 100;
			}


			RoomScript r = LevelController.me.getRoomObjectIsIn (CommonObjectsStore.player);
			if (r == null) {
				//player probs outside
			} else {
				if (r.traspassingInRoom() == true) {
				//	retVal += 5.0f;
				}
			}

			if (i.getInventoryWeightSum () > i.inventoryCapacity - 5) {
				retVal += 1;
			}

			if (PlayerAction.currentAction == null) {

			} else {
				if (PlayerAction.currentAction.illigal == true) {
					retVal += 100;
				}
			}

		} else {
			NPCController npc = g.GetComponent<NPCController> ();
			if (i.back == null) {

			} else {
				retVal += 0.5f;
			}

			if (i.leftArm == null) {

			} else {
				if (i.leftArm.illigal == true) {
					retVal += 11;
				}
			}

			if (i.rightArm == null) {

			} else {
				if (i.rightArm.illigal == true) {
					retVal += 11;
				}
			}

			if (i.head == null) {

			} else {
				if (i.head.illigal == true) {
					retVal += 11;
				}
			}

			if (pwc.currentWeapon == null) {

			} else {
				retVal += 100;
			}


			RoomScript r = LevelController.me.getRoomObjectIsIn (g);
			if (r == null) {
				//player probs outside
			} else {
				if (r.traspassingInRoom() == true) {
					retVal += 5.0f;
				}
			}

			if (i.getInventoryWeightSum () > i.inventoryCapacity - 5) {
				retVal += 1;
			}
		}
		return retVal;
	}

	void guardPassive()
	{
		if (globalAlarm == false) {
			detectWhetherWeShouldBeAlerted ();
			detectItems ();
			if (patrol == true) {
				if (myController.currentBehaviour == null || myController.currentBehaviour.myType != behaviourType.patrol) {
					Destroy (myController.currentBehaviour);
					NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_PatrolRoute> ();
					myController.currentBehaviour = nb;
					////PhoneTab_RadioHack.me.setNewText ("No problems here, starting patrol.",radioHackBand.buisness);

				}
			} else {
				if (myController.currentBehaviour == null || myController.currentBehaviour.myType != behaviourType.guardLoc) {
					Destroy (myController.currentBehaviour);
					NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_GuardLocation> ();
					myController.currentBehaviour = nb;
					//PhoneTab_RadioHack.me.setNewText ("No problems here, standing guard",radioHackBand.buisness);

				}
			}
		} else {
			detectWhetherWeShouldBeAlerted ();
			if (canWeSeeSuspects () == true) {
				alarmed = true;
				return;
			} else {
				if (patrol == true) {
					if (myController.currentBehaviour == null || myController.currentBehaviour.myType != behaviourType.patrol) {
						Destroy (myController.currentBehaviour);
						NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_PatrolRoute> ();
						myController.currentBehaviour = nb;
						//PhoneTab_RadioHack.me.setNewText ("No problems here, starting patrol.",radioHackBand.buisness);

					}
				} else {
					if (myController.currentBehaviour == null || myController.currentBehaviour.myType != behaviourType.guardLoc) {
						Destroy (myController.currentBehaviour);
						NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_GuardLocation> ();
						myController.currentBehaviour = nb;
						//PhoneTab_RadioHack.me.setNewText ("No problems here, standing guard",radioHackBand.buisness);

					}
				}
			}
		}
	}

	bool canWeSeeSuspects()
	{
		foreach (GameObject g in LevelController.me.suspects) {
			if (myController.detect.fov.visibleTargts.Contains(g.transform)) {
				myController.memory.objectThatMadeMeSuspisious = g;
				return true;
			}

		}
		return false;
	}

	void checkForArmedNPCs(){ //could probbaly write this to be better and not have to go through every npc
		if (myController.detect.fov.visibleTargts.Contains (CommonObjectsStore.player.transform)) {
			PersonWeaponController pwc = CommonObjectsStore.player.GetComponent<PersonWeaponController> ();
			if (pwc.currentWeapon == null) {

			} else {
				if (pwc.currentWeapon.illigal == true) {
					myController.memory.objectThatMadeMeSuspisious = CommonObjectsStore.player;
					alarmed = true;
				}
			}
		}

		foreach (GameObject npc in NPCManager.me.npcsInWorld) {
			if (npc == null) {
				continue;
			}
			if (myController.detect.fov.visibleTargts.Contains (npc.transform)) {
				NPCController nc = npc.GetComponent<NPCController>();
				if (nc.npcB.freindlyIDs.Contains (myID) == false && nc.npcB.myID != myID) {
					PersonWeaponController pwc = npc.GetComponent<PersonWeaponController> ();

					if (pwc.currentWeapon == null) {

					} else {
						myController.memory.objectThatMadeMeSuspisious =npc;
						alarmed = true;
					}
				}
			}


		}
	}

	void guardSuspisious()
	{
		canWeSeeSuspects ();
		detectWhetherWeShouldBeAlerted ();
		//detectItems ();
		SearchedMarker suspisiousOf;

		if (myController.memory.objectThatMadeMeSuspisious == null) {
			 if (myController.memory.noiseToInvestigate != Vector3.zero) {
				if (myController.currentBehaviour.myType != behaviourType.investigate) {
					////////Debug.Log ("Wanting to investigate location 1");

					Destroy (myController.currentBehaviour);
					NPCBehaviour newBehaviour = this.gameObject.AddComponent<NPCBehaviour_InvesigateLocation> ();
					//newBehaviour.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
					myController.currentBehaviour = newBehaviour;
					//PhoneTab_RadioHack.me.setNewText ("Heard something, going to check it out",radioHackBand.buisness);
				}
			}
		} else {
			suspisiousOf = myController.memory.objectThatMadeMeSuspisious.GetComponent<SearchedMarker> ();

			NPCController npc = myController.memory.objectThatMadeMeSuspisious.GetComponent<NPCController> ();
			if (npc == null) {

			} else {
				if (npc.npcB.myType == AIType.hostage && npc.transform.parent.tag != "Player") {
					if (myController.currentBehaviour.myType != behaviourType.freeHostage) {
						Destroy (myController.currentBehaviour);
						NPCBehaviour npcb = this.gameObject.AddComponent<NPCBehaviour_FreeHostage> ();
						myController.currentBehaviour = npcb;
						//PhoneTab_RadioHack.me.setNewText ("Someone got tied up, freeing them now",radioHackBand.buisness);

					}
					return;
				}
				else if(npc.npcB.myType == AIType.hostage && npc.transform.parent.tag == "Player")
				{
					myController.memory.objectThatMadeMeSuspisious = CommonObjectsStore.player;
					alarmed = true;
				}
			}

			if (suspisiousOf == null) {
				//would be an object, go investigate
				if (myController.memory.objectThatMadeMeSuspisious != null) {
					if (myController.currentBehaviour.myType != behaviourType.investigate) {
						////////Debug.Log ("Wanting to investigate object");

						Destroy (myController.currentBehaviour);
						NPCBehaviour_InvestigateObject newBehaviour = this.gameObject.AddComponent<NPCBehaviour_InvestigateObject> ();
						newBehaviour.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
						myController.currentBehaviour = newBehaviour;
						//PhoneTab_RadioHack.me.setNewText ("Seen something odd, going to check it out",radioHackBand.buisness);

					}
				} else if (myController.memory.noiseToInvestigate != Vector3.zero) {
					////////Debug.Log ("Wanting to investigate location");

					if (myController.currentBehaviour.myType != behaviourType.investigate) {
						Destroy (myController.currentBehaviour);
						NPCBehaviour newBehaviour = this.gameObject.AddComponent<NPCBehaviour_InvesigateLocation> ();
						//newBehaviour.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
						myController.currentBehaviour = newBehaviour;
						//PhoneTab_RadioHack.me.setNewText ("Heard something, going to check it out",radioHackBand.buisness);

					}
				}
				//newBehaviour.Initialise ();
			
			} else if (myController.memory.objectThatMadeMeSuspisious.tag == "Dead/Knocked") {
				if (myController.currentBehaviour.myType != behaviourType.investigate) {
					////////Debug.Log ("Wanting to investigate Corpse");

					Destroy (myController.currentBehaviour);
					NPCBehaviour newBehaviour = this.gameObject.AddComponent<NPCBehaviour_InvestigateCorpse> ();
					newBehaviour.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
					myController.currentBehaviour = newBehaviour;
					//PhoneTab_RadioHack.me.setNewText ("Control, I've found a body",radioHackBand.buisness);


				}
			}
			else {//if (suspisiousOf.searchedBy.Contains (this.gameObject) == false) { //stops player getting searched by the same npc over and over
				if (LevelController.me.suspects.Contains (myController.memory.objectThatMadeMeSuspisious) == false) {
					
					if (suspisiousOf.searchedBy.Contains (this.gameObject) == false) { //NEED TO ADD SOME KIND OF SEARCH + ESCORT OUT
						if (myController.currentBehaviour.myType != behaviourType.searchPerson) {
							////////Debug.Log ("Wanting to search person " + myController.memory.objectThatMadeMeSuspisious.name);
							Destroy (myController.currentBehaviour);
							NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_SearchPerson> ();
							myController.currentBehaviour = nb;
							nb.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
							suspisiousOf.addToSearchedBy (this.gameObject);
							//PhoneTab_RadioHack.me.setNewText ("Got a suspicious person, going to search them.",radioHackBand.buisness);

						}
					} else {
						RoomScript npcRoom = LevelController.me.getRoomObjectIsIn (myController.memory.objectThatMadeMeSuspisious);

						if (npcRoom==null|| npcRoom.traspassingInRoom() == true) {
							if (myController.currentBehaviour.myType != behaviourType.traspassing) {
								Destroy (myController.currentBehaviour);
								NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_EscortOutOfRestrictedArea> ();
								myController.currentBehaviour = nb;
								//PhoneTab_RadioHack.me.setNewText ("Found a trespasser, escorting them out now.",radioHackBand.buisness);

								//suspisiousOf.searchedBy.Add (this.gameObject);
							}
						}
					}
				} else {
					alarmed = true;
				}
			}
		}
	}

	public void guardAggressive()
	{



		if (myController.currentBehaviour == null) {
			NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_RaiseAlarm> ();
			myController.currentBehaviour = nb;
		}

		canWeSeeSuspects ();

		if (globalAlarm == false) {
			bool canWeSeeObject = myController.detect.targetDetect (myController.memory.objectThatMadeMeSuspisious);

			if (canWeSeeObject == true) {
				if (shouldWeAttackTarget (myController.memory.objectThatMadeMeSuspisious) == true) {
					if (myController.currentBehaviour.myType != behaviourType.attackTarget) {
						Destroy (myController.currentBehaviour);
						NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_AttackTarget> ();
						myController.currentBehaviour = nb;
						nb.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
						//PhoneTab_RadioHack.me.setNewText ("Engaging target",radioHackBand.buisness);

					}
				}
				if (myController.currentBehaviour.myType == behaviourType.attackTarget) {
					loseTargetTimer -= Time.deltaTime;
					if (loseTargetTimer <= 0) {
						Destroy (myController.currentBehaviour);
						loseTargetTimer = 5.0f;
					}
					return;
				} 

				if (myController.memory.objectThatMadeMeSuspisious.tag == "Player" || myController.memory.objectThatMadeMeSuspisious.tag == "NPC") {
					if (myController.currentBehaviour.myType != behaviourType.attackTarget) {
						Destroy (myController.currentBehaviour);
						NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_AttackTarget> ();
						myController.currentBehaviour = nb;
						nb.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
						//PhoneTab_RadioHack.me.setNewText ("Engaging target",radioHackBand.buisness);

					}

				} else if(myController.memory.noiseToInvestigate!=Vector3.zero){
					if (myController.currentBehaviour.myType != behaviourType.investigate) {
						Destroy (myController.currentBehaviour);
						NPCBehaviour newBehaviour = this.gameObject.AddComponent<NPCBehaviour_InvesigateLocation> ();
						//newBehaviour.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
						myController.currentBehaviour = newBehaviour;
						//PhoneTab_RadioHack.me.setNewText ("Lost the target, searching",radioHackBand.buisness);

					}

				}
				else if(globalAlarm==false){
					if (myController.currentBehaviour.myType != behaviourType.raiseAlarm) {
						Destroy (myController.currentBehaviour);
						NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_RaiseAlarm> ();
						myController.currentBehaviour = nb;
						////////Debug.LogError("RAISE ALARM 1");
						//PhoneTab_RadioHack.me.setNewText ("I'm going to call the cops",radioHackBand.buisness);

						//myController.memory.noiseToInvestigate = Vector3.zero;
					}
				}
			} else {
				if (myController.currentBehaviour.myType == behaviourType.attackTarget) {
					loseTargetTimer -= Time.deltaTime;
					if (loseTargetTimer <= 0) {
						Destroy (myController.currentBehaviour);
						loseTargetTimer = 5.0f;
						RoomScript r = LevelController.me.getRoomObjectIsIn (this.gameObject);
						if (r == null) {
							if (myController.currentBehaviour.myType != behaviourType.raiseAlarm) {
								Destroy (myController.currentBehaviour);
								NPCBehaviour newBehaviour = this.gameObject.AddComponent<NPCBehaviour_RaiseAlarm> ();
								//newBehaviour.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
								myController.currentBehaviour = newBehaviour;
								//PhoneTab_RadioHack.me.setNewText ("Going to call for backup",radioHackBand.cop);
							}
						} else {
							if (myController.currentBehaviour.myType != behaviourType.searchRooms) {
								Destroy (myController.currentBehaviour);
								NPCBehaviour npcb = this.gameObject.AddComponent<NPCBehaviour_SearchRoom> ();
								myController.currentBehaviour = npcb;
								//PhoneTab_RadioHack.me.setNewText ("Searching room for suspects",radioHackBand.cop);

							}
						}

					}
					return;

				} 
				if (myController.memory.objectThatMadeMeSuspisious == null) {
					if (Vector3.Distance (this.transform.position, myController.memory.noiseToInvestigate) > 1.5f && myController.memory.noiseToInvestigate!=Vector3.zero) {
						if (myController.currentBehaviour.myType != behaviourType.investigate && myController.currentBehaviour.myType != behaviourType.raiseAlarm) {
							Destroy (myController.currentBehaviour);
							NPCBehaviour newBehaviour = this.gameObject.AddComponent<NPCBehaviour_InvesigateLocation> ();
							//newBehaviour.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
							myController.currentBehaviour = newBehaviour;
							//PhoneTab_RadioHack.me.setNewText ("Heard something, going to check it out",radioHackBand.buisness);

						}
					} else if(Vector3.Distance (this.transform.position, myController.memory.noiseToInvestigate) <= 1.5f && myController.memory.noiseToInvestigate!=Vector3.zero) {
						if (myController.currentBehaviour.myType != behaviourType.raiseAlarm) {
							Destroy (myController.currentBehaviour);
							NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_RaiseAlarm> ();
							myController.currentBehaviour = nb;
							////////Debug.LogError("RAISE ALARM 2");

							//myController.memory.noiseToInvestigate = Vector3.zero;
							//PhoneTab_RadioHack.me.setNewText ("I'm going to call the cops",radioHackBand.buisness);

						}
					}
				} else {
					//////////Debug.LogError ("Post hostage 1");
					if (PlayerAction.currentAction == null) {
						if (myController.currentBehaviour==null || myController.currentBehaviour.myType != behaviourType.raiseAlarm) {
							Destroy (myController.currentBehaviour);
							NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_RaiseAlarm> ();
							myController.currentBehaviour = nb;
							//////////Debug.LogError("RAISE ALARM 3");

							//PhoneTab_RadioHack.me.setNewText ("I'm going to call the cops",radioHackBand.buisness);

							//myController.memory.noiseToInvestigate = Vector3.zero;
						}
					}
					else if(PlayerAction.currentAction.getType()=="Take Hostage" && myController.detect.hostageTest(myController.memory.objectThatMadeMeSuspisious)==true){
						//test somethign out first of february
						if (myController.currentBehaviour.myType != behaviourType.attackTarget) {
							Destroy (myController.currentBehaviour);
							NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_AttackTarget> ();
							myController.currentBehaviour = nb;
							nb.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
							//PhoneTab_RadioHack.me.setNewText ("Engaging target",radioHackBand.buisness);

						}

					}
				}


			}
		} else {
			bool canWeSeeObject = myController.detect.targetDetect (myController.memory.objectThatMadeMeSuspisious);

			if (canWeSeeObject == true) {
				if (myController.currentBehaviour.myType != behaviourType.attackTarget) {
					Destroy (myController.currentBehaviour);
					NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_AttackTarget> ();
					myController.currentBehaviour = nb;
					nb.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
					//PhoneTab_RadioHack.me.setNewText ("Engaging target",radioHackBand.buisness);

				}
				myController.memory.noiseToInvestigate = myController.memory.objectThatMadeMeSuspisious.transform.position;
			} else {
				if (LevelController.me.suspects.Contains (myController.memory.objectThatMadeMeSuspisious) == false) {
					if (myController.currentBehaviour.myType != behaviourType.updateSuspects) {
						//////////Debug.Log ("Updating suspects");
						Destroy (myController.currentBehaviour);
						NPCBehaviour newBehaviour = this.gameObject.AddComponent<NPCBehaviour_UpdateSuspects> ();
						//newBehaviour.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
						myController.currentBehaviour = newBehaviour;
						//PhoneTab_RadioHack.me.setNewText ("I'm coming with a description of the suspect, standby",radioHackBand.buisness);

					}
				} else {
					if (myController.memory.noiseToInvestigate != Vector3.zero) {
						if (myController.currentBehaviour.myType != behaviourType.investigate) {
							Destroy (myController.currentBehaviour);
							NPCBehaviour newBehaviour = this.gameObject.AddComponent<NPCBehaviour_InvesigateLocation> ();
							//newBehaviour.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
							myController.currentBehaviour = newBehaviour;
							//PhoneTab_RadioHack.me.setNewText ("Investigating location",radioHackBand.buisness);

						}
					}
				}

				if (Vector3.Distance (this.transform.position, myController.memory.noiseToInvestigate) < 1.5f) {
					alarmed = false;
				}
			}
		}


	}


	void shouldNPCBeArmed()
	{
		if (myType == AIType.guard) {
			if (NPCBehaviourDecider.globalAlarm == true) {
				if (myController.pwc.currentWeapon == null && myController.inv.doWeHaveAWeaponWeCanUse () == true) {
					Weapon w = myController.inv.getWeaponWeCanUse ();
					w.equipItem ();
				}
			}
		} else if (myType == AIType.cop) {
			if (myController.pwc.currentWeapon == null && myController.inv.doWeHaveAWeaponWeCanUse () == true) {
				Weapon w = myController.inv.getWeaponWeCanUse ();
				w.equipItem ();
			}
		} else if (myType == AIType.swat) {
			if (myController.pwc.currentWeapon == null && myController.inv.doWeHaveAWeaponWeCanUse () == true) {
				Weapon w = myController.inv.getWeaponWeCanUse ();
				w.equipItem ();
			}
		}
	}

	void detectCorpses()
	{
		foreach (GameObject g in NPCManager.me.corpsesInWorld) {
			if (g.tag != "Dead/Guarded") {
				if (Vector3.Distance(this.transform.position,g.transform.position)<myController.detect.fov.viewRadius) {
					if (myController.detect.isTargetInFrontOfUs (g) == true) {
						if (myController.detect.lineOfSightToTargetWithNoCollider (g) == true) {
							myController.memory.objectThatMadeMeSuspisious = g;
							suspisious = true;
							myController.memory.suspisious = true;
						}
					}
				}
			}
		}

		/*(foreach (GameObject g in NPCManager.me.corpsesInWorld) {
			if (myController.detect.areWeNearTarget (g) == true) {
				if (myController.detect.isTargetInFrontOfUs (g) == true) {
					if (myController.detect.lineOfSightToTargetWithNoCollider (g) == true) {
						myController.memory.objectThatMadeMeSuspisious = g;
						suspisious = true;
						myController.memory.suspisious = true;
						//////////Debug.Log (this.gameObject.name);
						////////Debug.Break ();
					}
				}
			}
		}*/
	}

	public void copPassive()
	{
		//if (myController.currentBehaviour == null) {
		//	NPCBehaviour npcb = this.gameObject.AddComponent<NPCBehaviour_SearchRoom> ();
		//	myController.currentBehaviour = npcb;
		//}

		if (myController.currentBehaviour == null) {
			//if ( myController.currentBehaviour.myType != behaviourType.searchRooms) {
			//Destroy (myController.currentBehaviour);
			NPCBehaviour npcb = this.gameObject.AddComponent<NPCBehaviour_SearchRoom> ();
			myController.currentBehaviour = npcb;
			//PhoneTab_RadioHack.me.setNewText ("Moving to invesigate room",radioHackBand.cop);

			//}
		}

		detectWhetherWeShouldBeAlerted ();

		detectCorpses ();
		if (canWeSeeSuspects () == false) {
			if (myController.memory.objectThatMadeMeSuspisious == null) {
				if (myController.currentBehaviour==null || myController.currentBehaviour.myType != behaviourType.searchRooms) {
					Destroy (myController.currentBehaviour);
					NPCBehaviour npcb = this.gameObject.AddComponent<NPCBehaviour_SearchRoom> ();
					myController.currentBehaviour = npcb;
					//PhoneTab_RadioHack.me.setNewText ("Moving to invesigate room",radioHackBand.cop);

				}
			} else {

				if (myController.memory.objectThatMadeMeSuspisious.tag == "Player" || myController.memory.objectThatMadeMeSuspisious.tag == "NPC") {
					SearchedMarker suspisiousOf = myController.memory.objectThatMadeMeSuspisious.GetComponent<SearchedMarker> ();
					if (LevelController.me.suspects.Contains (myController.memory.objectThatMadeMeSuspisious) == false) {
						if (suspisiousOf == null) {
							if (myController.currentBehaviour.myType != behaviourType.investigate) {
								////////Debug.Log ("Passive cop investigating location " + myController.memory.objectThatMadeMeSuspisious.name);
								Destroy (myController.currentBehaviour);
								NPCBehaviour newBehaviour = this.gameObject.AddComponent<NPCBehaviour_InvestigateObject> ();
								newBehaviour.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
								myController.currentBehaviour = newBehaviour;
								//PhoneTab_RadioHack.me.setNewText ("Potential suspect sighted, going to investigate",radioHackBand.cop);

							}
						} else {
							if (myController.currentBehaviour.myType != behaviourType.searchPerson) {
								////////Debug.Log ("Wanting to search person " + myController.memory.objectThatMadeMeSuspisious.name);
								Destroy (myController.currentBehaviour);
								NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_SearchPerson> ();
								myController.currentBehaviour = nb;
								nb.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
								suspisiousOf.addToSearchedBy (this.gameObject);
								//PhoneTab_RadioHack.me.setNewText ("Potential suspect sighted, going to investigate",radioHackBand.cop);

							}
						}
					} else {
						alarmed = true;
					}
				} else if (myController.memory.objectThatMadeMeSuspisious.tag != "Dead/Knocked" || myController.memory.objectThatMadeMeSuspisious.tag != "Dead/Guarded") {
					if (myController.currentBehaviour.myType != behaviourType.investigate) {
						////////Debug.Log ("Passive cop investigating location " + myController.memory.objectThatMadeMeSuspisious.name);
						Destroy (myController.currentBehaviour);
						NPCBehaviour newBehaviour = this.gameObject.AddComponent<NPCBehaviour_InvestigateObject> ();
						newBehaviour.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
						myController.currentBehaviour = newBehaviour;
						//PhoneTab_RadioHack.me.setNewText ("Found a suspicious object, investigating",radioHackBand.cop);

					}
				} else {
					if ( myController.currentBehaviour.myType != behaviourType.searchRooms) {
						Destroy (myController.currentBehaviour);
						NPCBehaviour npcb = this.gameObject.AddComponent<NPCBehaviour_SearchRoom> ();
						myController.currentBehaviour = npcb;
						//PhoneTab_RadioHack.me.setNewText ("Moving to invesigate room",radioHackBand.cop);

					}
				}
			}
		} else {
			PersonWeaponController pwc = myController.memory.objectThatMadeMeSuspisious.GetComponent<PersonWeaponController> ();
			SearchedMarker suspisiousOf = null;//s = myController.memory.objectThatMadeMeSuspisious.GetComponent<SearchedMarker> ();

			if (pwc == null) {
				//not got a weapon,
			} else {

				//alarmed = true;
				if (pwc.currentWeapon == null && LevelController.me.suspects.Contains(myController.memory.objectThatMadeMeSuspisious)==false) {
					if (myController.currentBehaviour.myType != behaviourType.searchPerson) {
						////////Debug.Log ("Wanting to search person " + myController.memory.objectThatMadeMeSuspisious.name);
						suspisiousOf = myController.memory.objectThatMadeMeSuspisious.GetComponent<SearchedMarker> ();
						Destroy (myController.currentBehaviour);
						NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_SearchPerson> ();
						myController.currentBehaviour = nb;
						nb.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
						suspisiousOf.addToSearchedBy (this.gameObject);
						//PhoneTab_RadioHack.me.setNewText ("Potential suspect sighted, going to investigate",radioHackBand.cop);

					}
				} else {
					alarmed = true;
				}
			}
		}

		if (myController.currentBehaviour == null) {
			//if ( myController.currentBehaviour.myType != behaviourType.searchRooms) {
			//Destroy (myController.currentBehaviour);
			NPCBehaviour npcb = this.gameObject.AddComponent<NPCBehaviour_SearchRoom> ();
			myController.currentBehaviour = npcb;
			//PhoneTab_RadioHack.me.setNewText ("Moving to invesigate room",radioHackBand.cop);

			//}
		}

	}

	public void copSuspicious()
	{
		detectWhetherWeShouldBeAlerted ();

		if (canWeSeeSuspects () == false) {
			if (myController.memory.objectThatMadeMeSuspisious == null) {
				if (myController.memory.noiseToInvestigate == Vector3.zero) {
					//suspisious = false;

				} else {



					//go to location
					if (myController.currentBehaviour.myType != behaviourType.investigate) {
						////////Debug.Log ("Investigating location on Suspicious" );

						Destroy (myController.currentBehaviour);
						NPCBehaviour newBehaviour = this.gameObject.AddComponent<NPCBehaviour_InvesigateLocation> ();
						//newBehaviour.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
						myController.currentBehaviour = newBehaviour;
						//PhoneTab_RadioHack.me.setNewText ("Moving to invesigate location, standby",radioHackBand.cop);

					}

				}


			} else {
				

				// go to object
				if (myController.memory.objectThatMadeMeSuspisious.tag != "Dead/Knocked" && myController.memory.objectThatMadeMeSuspisious.tag != "Dead/Guarded") {
					if (myController.memory.objectThatMadeMeSuspisious.tag != "Player" && myController.memory.objectThatMadeMeSuspisious.tag != "NPC") {
						if (myController.currentBehaviour.myType != behaviourType.investigate) {
							////////Debug.Log ("Wanting to investigate object");

							Destroy (myController.currentBehaviour);
							NPCBehaviour_InvestigateObject newBehaviour = this.gameObject.AddComponent<NPCBehaviour_InvestigateObject> ();
							newBehaviour.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
							myController.currentBehaviour = newBehaviour;
							//PhoneTab_RadioHack.me.setNewText ("Found a suspicious object, investigating",radioHackBand.cop);

						}
					} else {
						//PersonWeaponController pwc = myController.memory.objectThatMadeMeSuspisious.GetComponent<PersonWeaponController> ();
						if (LevelController.me.suspects.Contains (myController.memory.objectThatMadeMeSuspisious) == false) {
							SearchedMarker suspisiousOf = null;//s = myController.memory.objectThatMadeMeSuspisious.GetComponent<SearchedMarker> ();

							NPCController npc = myController.memory.objectThatMadeMeSuspisious.GetComponent<NPCController> ();
							if (npc == null) {
								if (myController.currentBehaviour.myType != behaviourType.searchPerson && myController.currentBehaviour.myType != behaviourType.raiseAlarm) {
									////////Debug.Log ("COP SEARCH ONE Wanting to search person " + myController.memory.objectThatMadeMeSuspisious.name);
									suspisiousOf = myController.memory.objectThatMadeMeSuspisious.GetComponent<SearchedMarker> ();
									Destroy (myController.currentBehaviour);
									NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_SearchPerson> ();
									myController.currentBehaviour = nb;
									nb.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
									suspisiousOf.addToSearchedBy (this.gameObject);
									//PhoneTab_RadioHack.me.setNewText ("Potential suspect spotted, investigating.",radioHackBand.cop);

								}
							} else {
								if (npc.npcB.myType == AIType.hostage && npc.transform.parent.tag != "Player") {
									if (myController.currentBehaviour.myType != behaviourType.freeHostage && npc.transform.parent.gameObject.tag != "Player") {
										Destroy (myController.currentBehaviour);
										NPCBehaviour npcb = this.gameObject.AddComponent<NPCBehaviour_FreeHostage> ();
										myController.currentBehaviour = npcb;
										//PhoneTab_RadioHack.me.setNewText ("Found a suspicious object, investigating",radioHackBand.cop);

									}
									else if(npc.npcB.myType == AIType.hostage && npc.transform.parent.tag == "Player")
									{
										myController.memory.objectThatMadeMeSuspisious = CommonObjectsStore.player;
										alarmed = true;
									}
								} else {
									if (myController.currentBehaviour.myType != behaviourType.searchPerson && myController.currentBehaviour.myType != behaviourType.raiseAlarm) {
										////////Debug.Log ("COP SEARCH TWO Wanting to search person " + myController.memory.objectThatMadeMeSuspisious.name);
										suspisiousOf = myController.memory.objectThatMadeMeSuspisious.GetComponent<SearchedMarker> ();
										Destroy (myController.currentBehaviour);
										NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_SearchPerson> ();
										myController.currentBehaviour = nb;
										nb.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
										suspisiousOf.addToSearchedBy (this.gameObject);
										//PhoneTab_RadioHack.me.setNewText ("Potential suspect spotted, investigating.",radioHackBand.cop);

									}
								}
							}



						}
					}
				} else {
					if (copAlarm == false) {
						if (myController.currentBehaviour.myType != behaviourType.raiseAlarm) {

							Destroy (myController.currentBehaviour);
							NPCBehaviour newBehaviour = this.gameObject.AddComponent<NPCBehaviour_CopRaiseAlarm> ();
							//newBehaviour.passInGameobject (myController.memory.objectThatMadeMeSuspisious);

							myController.currentBehaviour = newBehaviour;
							//PhoneTab_RadioHack.me.setNewText ("Calling for backup.",radioHackBand.cop);

						}
					} else {
						if (myController.currentBehaviour.myType != behaviourType.guardCorpse) {
							////////Debug.Log ("Guarding Corpse");

							Destroy (myController.currentBehaviour);
							NPCBehaviour newBehaviour = this.gameObject.AddComponent<NPCBehaviour_GuardCorpse> ();
							//newBehaviour.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
							myController.currentBehaviour = newBehaviour;
							//PhoneTab_RadioHack.me.setNewText ("Guarding a body untill backup gets here.",radioHackBand.cop);

						}
					}
				}

			}
		} else {



			PersonWeaponController pwc = myController.memory.objectThatMadeMeSuspisious.GetComponent<PersonWeaponController> ();
			SearchedMarker suspisiousOf = null;//s = myController.memory.objectThatMadeMeSuspisious.GetComponent<SearchedMarker> ();

			if (pwc == null) {
				//not got a weapon,
			} else {
				alarmed = true;
				/*if (pwc.currentWeapon == null) {
					if (myController.currentBehaviour.myType != behaviourType.searchPerson) {
						////////Debug.Log ("Wanting to search person " + myController.memory.objectThatMadeMeSuspisious.name);
						suspisiousOf = myController.memory.objectThatMadeMeSuspisious.GetComponent<SearchedMarker> ();
						Destroy (myController.currentBehaviour);
						NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_SearchPerson> ();
						myController.currentBehaviour = nb;
						nb.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
						suspisiousOf.addToSearchedBy (this.gameObject);
					}
				} else {
					alarmed = true;
				}*/
			}
		}


	}
		
	public void copAlarmed()
	{
		//detectWhetherWeShouldBeAlerted ();
		if (myController.currentBehaviour == null) {
			//if ( myController.currentBehaviour.myType != behaviourType.searchRooms) {
			//Destroy (myController.currentBehaviour);
			NPCBehaviour npcb = this.gameObject.AddComponent<NPCBehaviour_SearchRoom> ();
			myController.currentBehaviour = npcb;
			//PhoneTab_RadioHack.me.setNewText ("Searching room for suspects",radioHackBand.cop);

			//}
		}
		bool canWeSeeObject = false;// myController.detect.targetDetect (myController.memory.objectThatMadeMeSuspisious);

		foreach (GameObject g in LevelController.me.suspects) {
			if (myController.detect.fov.visibleTargts.Contains (g.transform)) {
				myController.memory.objectThatMadeMeSuspisious = g;
			}
		}

		if (myController.memory.objectThatMadeMeSuspisious==null || myController.detect.fov.visibleTargts.Contains (myController.memory.objectThatMadeMeSuspisious.transform) == false) {
			canWeSeeObject = false;
		} else {
			canWeSeeObject = true;
			if (LevelController.me.suspects.Contains (myController.memory.objectThatMadeMeSuspisious)) {
				attemptToArrest = false;
			}
		}

		if (canWeSeeObject == true) {
			if (shouldWeAttackTarget (myController.memory.objectThatMadeMeSuspisious) == true) {
				if (myController.currentBehaviour.myType != behaviourType.attackTarget) {
					Destroy (myController.currentBehaviour);
					NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_AttackTarget> ();
					myController.currentBehaviour = nb;
					nb.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
					//PhoneTab_RadioHack.me.setNewText ("Engaging target",radioHackBand.buisness);

				}
			}

			if (attemptToArrest == true) {
				if (myController.currentBehaviour.myType != behaviourType.arrestTarget) {
					Destroy (myController.currentBehaviour);
					NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_ArrestTarget> ();
					nb.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
					myController.currentBehaviour = nb;
					//PhoneTab_RadioHack.me.setNewText ("Moving to arrest target.",radioHackBand.cop);

				}
			} else {
				if (myController.currentBehaviour.myType != behaviourType.attackTarget) {
					Destroy (myController.currentBehaviour);
					NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_AttackTarget> ();
					myController.currentBehaviour = nb;
					nb.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
					//PhoneTab_RadioHack.me.setNewText ("Target is hostile, taking him down.",radioHackBand.cop);

				}
			}
			myController.memory.noiseToInvestigate = myController.memory.objectThatMadeMeSuspisious.transform.position;
		} else {
		/*	if (myController.currentBehaviour.myType == behaviourType.attackTarget) {
				loseTargetTimer -= Time.deltaTime;
				if (loseTargetTimer <= 0) {
					Destroy (myController.currentBehaviour);
					loseTargetTimer = 5.0f;
					RoomScript r = LevelController.me.getRoomObjectIsIn (this.gameObject);
					if (r == null) {
						if (myController.currentBehaviour.myType != behaviourType.raiseAlarm) {
							Destroy (myController.currentBehaviour);
							NPCBehaviour newBehaviour = this.gameObject.AddComponent<NPCBehaviour_CopRaiseAlarm> ();
							//newBehaviour.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
							myController.currentBehaviour = newBehaviour;
							//PhoneTab_RadioHack.me.setNewText ("Going to call for backup",radioHackBand.cop);
						}
					} else {
						if (myController.currentBehaviour.myType != behaviourType.searchRooms) {
							Destroy (myController.currentBehaviour);
							NPCBehaviour npcb = this.gameObject.AddComponent<NPCBehaviour_SearchRoom> ();
							myController.currentBehaviour = npcb;
							//PhoneTab_RadioHack.me.setNewText ("Searching room for suspects",radioHackBand.cop);

						}
					}

				}
				return;

			} */


			if (copAlarm == true) {
				if (LevelController.me.suspects.Contains (myController.memory.objectThatMadeMeSuspisious) == false) {
					if (myController.currentBehaviour.myType != behaviourType.updateSuspects) {
						////////Debug.Log ("Updating suspects");
						Destroy (myController.currentBehaviour);
						NPCBehaviour newBehaviour = this.gameObject.AddComponent<NPCBehaviour_UpdateSuspects> ();
						//newBehaviour.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
						myController.currentBehaviour = newBehaviour;
						//PhoneTab_RadioHack.me.setNewText ("Going to update suspects description, standby",radioHackBand.cop);

					}
				} else {
					if (myController.memory.noiseToInvestigate != Vector3.zero) {
						if (myController.currentBehaviour.myType != behaviourType.investigate) {
							////////Debug.Log ("Investigating location on alarmed");
							Destroy (myController.currentBehaviour);
							NPCBehaviour newBehaviour = this.gameObject.AddComponent<NPCBehaviour_InvesigateLocation> ();
							//newBehaviour.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
							myController.currentBehaviour = newBehaviour;
							//PhoneTab_RadioHack.me.setNewText ("I've heard something, going to investigate",radioHackBand.cop);

						}
					} else {
						if (myController.currentBehaviour.myType != behaviourType.searchRooms) {
							Destroy (myController.currentBehaviour);
							NPCBehaviour npcb = this.gameObject.AddComponent<NPCBehaviour_SearchRoom> ();
							myController.currentBehaviour = npcb;
							//PhoneTab_RadioHack.me.setNewText ("Searching room for suspects",radioHackBand.cop);

						}
					}
				}

				if (Vector3.Distance (this.transform.position, myController.memory.noiseToInvestigate) < 1.5f) {
					alarmed = false;
				}
			} else {
				if (myController.currentBehaviour.myType != behaviourType.raiseAlarm) {
					Destroy (myController.currentBehaviour);
					NPCBehaviour newBehaviour = this.gameObject.AddComponent<NPCBehaviour_CopRaiseAlarm> ();
					//newBehaviour.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
					myController.currentBehaviour = newBehaviour;
					//PhoneTab_RadioHack.me.setNewText ("Going to call for backup",radioHackBand.cop);

				}
			}
		}

	
	}

	void setAllSwatToAttack()
	{
		foreach (GameObject g in NPCManager.me.npcsInWorld) {
			NPCController npc = g.GetComponent<NPCController> ();
			if (npc.npcB.myType == AIType.swat) {
				if (myController.currentBehaviour.myType != behaviourType.attackTarget) {
					Destroy (npc.currentBehaviour);
					NPCBehaviour nb = g.gameObject.AddComponent<NPCBehaviour_SWATAttackTarget> ();
					npc.currentBehaviour = nb;
					nb.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
				}
			}
		}
		//PhoneTab_RadioHack.me.setNewText ("Team Alpha moving in on the target.",radioHackBand.swat);

	}

	public void setAllSwatToGoToRoom(RoomScript r)
	{
		foreach (GameObject g in NPCManager.me.npcsInWorld) {
			NPCController npc = g.GetComponent<NPCController> ();
			if (npc.npcB.myType == AIType.swat) {
				//if (myController.currentBehaviour.myType != behaviourType.searchRooms) {
					Destroy (npc.currentBehaviour);
					NPCBehaviour newBehaviour = g.gameObject.AddComponent<NPCBehaviour_SWATSearchMap> ();
					//newBehaviour.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
				NPCBehaviour_SWATSearchMap.roomToGoTo = r;
					npc.currentBehaviour = newBehaviour;
					npc.npcB.swatLoseTargetTimer = 10.0f;

			//	}
			}
		}
		//PhoneTab_RadioHack.me.setNewText ("Team Alpha moving to " + r.roomName,radioHackBand.swat);

	}

	void setAllSwatToSearch()
	{
		foreach (GameObject g in NPCManager.me.npcsInWorld) {
			NPCController npc = g.GetComponent<NPCController> ();
			if (npc.npcB.myType == AIType.swat) {
			//	if (myController.currentBehaviour.myType != behaviourType.searchRooms) {
					Destroy (npc.currentBehaviour);
					NPCBehaviour newBehaviour = g.gameObject.AddComponent<NPCBehaviour_SWATSearchMap> ();
					//newBehaviour.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
					npc.currentBehaviour = newBehaviour;
					npc.npcB.swatLoseTargetTimer = 10.0f;

			//	}
			}
		}
		//PhoneTab_RadioHack.me.setNewText ("Team Alpha moving to next location",radioHackBand.swat);

	}


	bool hasSwatSeenArmedTarget()
	{
		if (myController.detect.fov.visibleTargts.Contains (CommonObjectsStore.player.transform)) {
			PersonWeaponController pwc = CommonObjectsStore.player.GetComponent<PersonWeaponController> ();
			if (pwc.currentWeapon == null) {

			} else {
				myController.memory.objectThatMadeMeSuspisious = CommonObjectsStore.player;
				alarmed = true;
				return true;
			}
		}

		foreach (GameObject npc in NPCManager.me.npcsInWorld) {
			if (npc == null) {
				continue;
			}
			if (myController.detect.fov.visibleTargts.Contains (npc.transform)) {
				NPCController nc = npc.GetComponent<NPCController>();
				if (nc.npcB.freindlyIDs.Contains (myID) == false && nc.npcB.myID != myID) {
					PersonWeaponController pwc = npc.GetComponent<PersonWeaponController> ();

					if (pwc.currentWeapon == null) {

					} else {
						myController.memory.objectThatMadeMeSuspisious =npc;
						alarmed = true;
						return true;

					}
				}
			}


		}
		return false;
	}

	public float swatLoseTargetTimer = 10.0f;
	public void swatPassive()
	{
		shouldNPCBeArmed ();
		if (myController.currentBehaviour == null) {
			NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_SWATSearchMap> ();
			myController.currentBehaviour = nb;
			//PhoneTab_RadioHack.me.setNewText ("Team Alpha moving in",radioHackBand.swat);

		}

		foreach (Transform t in myController.detect.fov.visibleTargts) {
			if (shouldWeAttackTarget (t.gameObject) == true) {
				myController.memory.objectThatMadeMeSuspisious = t.gameObject;
				setAllSwatToAttack ();
			}
		}

		if (myController.currentBehaviour.myType == behaviourType.attackTarget) {
			if (canWeSeeSuspects () == false) {
				swatLoseTargetTimer -= Time.deltaTime;
				if (swatLoseTargetTimer <= 0) {
					setAllSwatToSearch ();
					swatLoseTargetTimer = 10.0f;
				}
			} else {
				swatLoseTargetTimer = 10.0f;
				myController.memory.noiseToInvestigate = myController.memory.objectThatMadeMeSuspisious.transform.position;
			}
			return;
		}

		if (canWeSeeSuspects () == true || hasSwatSeenArmedTarget()==true) {

			if (myController.currentBehaviour.myType != behaviourType.attackTarget) {
				setAllSwatToAttack ();
				////////Debug.Log ("Set all swat to attack");
			}

		}

		//if (FindObjectOfType<NPCBehaviour_SWATAttackTarget> () == true && myController.currentBehaviour.myType == behaviourType.attackTarget) {
		//	return;
		//}

		/*if (canWeSeeSuspects () == false) {
			if (myController.currentBehaviour.myType != behaviourType.attackTarget) {
				if (myController.memory.noiseToInvestigate == Vector3.zero) {
					if (myController.currentBehaviour.myType != behaviourType.searchRooms) {
						Destroy (myController.currentBehaviour);
						NPCBehaviour newBehaviour = this.gameObject.AddComponent<NPCBehaviour_SWATSearchMap> ();
						//newBehaviour.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
						myController.currentBehaviour = newBehaviour;
					}
				} else {
					if (myController.currentBehaviour.myType != behaviourType.investigate) {
						////////Debug.Log ("Investigating location on alarmed");
						Destroy (myController.currentBehaviour);
						NPCBehaviour newBehaviour = this.gameObject.AddComponent<NPCBehaviour_InvesigateLocation> ();
						//newBehaviour.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
						myController.currentBehaviour = newBehaviour;
					}
				}
			} else {
				swatLoseTargetTimer -= Time.deltaTime;
				if (swatLoseTargetTimer <= 0) {
					swatLoseTargetTimer = 5.0f;
					if (myController.memory.noiseToInvestigate == Vector3.zero) {
						if (myController.currentBehaviour.myType != behaviourType.searchRooms) {
							Destroy (myController.currentBehaviour);
							NPCBehaviour newBehaviour = this.gameObject.AddComponent<NPCBehaviour_SWATSearchMap> ();
							//newBehaviour.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
							myController.currentBehaviour = newBehaviour;
						}
					} else {
						if (myController.currentBehaviour.myType != behaviourType.investigate) {
							////////Debug.Log ("Investigating location on alarmed");
							Destroy (myController.currentBehaviour);
							NPCBehaviour newBehaviour = this.gameObject.AddComponent<NPCBehaviour_InvesigateLocation> ();
							//newBehaviour.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
							myController.currentBehaviour = newBehaviour;
						}
					}
				}
			}
		} else {
			swatLoseTargetTimer = 5.0f;
			if (myController.currentBehaviour.myType != behaviourType.attackTarget) {
				Destroy (myController.currentBehaviour);
				NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_SWATAttackTarget> ();
				myController.currentBehaviour = nb;
				nb.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
			}
			myController.memory.noiseToInvestigate = myController.memory.objectThatMadeMeSuspisious.transform.position;
		}*/
	}


	void civilianPassive()
	{
		shouldCivilianBeAlarmed ();

		if (myController.currentBehaviour == null) {
			int r = Random.Range (0, 100);

			if (r < 90) {

				NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_DoCivilianAction> ();
				myController.currentBehaviour = nb;
			} else {
				////////Debug.Log ("Civilian exiting level");
				NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_ExitLevel> ();
				myController.currentBehaviour = nb;
			}
		}


	}

	void civilianAlarmed()
	{
		if (myController.currentBehaviour == null) {

		}

		if (globalAlarm == false || LevelController.me.copSpawnTimer >= 0) {
			//raise alarm (need to both trigger
			if (myController.currentBehaviour.myType != behaviourType.raiseAlarm) {
				Destroy (myController.currentBehaviour);
				NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_CivilianRaiseAlarm> ();
				myController.currentBehaviour = nb;
			}
		} else {
			//leave map
			if (myController.currentBehaviour.myType != behaviourType.exitLevel) {
				Destroy (myController.currentBehaviour);
				NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_ExitLevel> ();
				myController.currentBehaviour = nb;
			}
		}
	}

	public void onHostageRelease()
	{
		if (myType == AIType.civilian) {

		} else if (myType == AIType.swat) {
			setAllSwatToAttack ();
		}
		else{
			//////////Debug.Log ("Hostage release");
			if (myController.currentBehaviour.myType != behaviourType.attackTarget) {
				Destroy (myController.currentBehaviour);
				NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_AttackTarget> ();
				myController.currentBehaviour = nb;
				nb.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
				//PhoneTab_RadioHack.me.setNewText ("Target is hostile, taking him down.",radioHackBand.cop);
				//////////Debug.Log("Adding attack target");
			}
			return;
			//alarmed = true;
			////////Debug.Break();
		}
	}

	public void onMeleeAttack()
	{

		if (myType == AIType.civilian) {
			alarmed = true;
		} else if (myType == AIType.swat) {
			setAllSwatToAttack ();
		}
		else{
			//if (myController.currentBehaviour.myType != behaviourType.attackTarget) {
			//	Destroy (myController.currentBehaviour);
			//	NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_AttackTarget> ();
			//	myController.currentBehaviour = nb;
			//	nb.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
				//PhoneTab_RadioHack.me.setNewText ("Target is hostile, taking him down.",radioHackBand.cop);
				//////////Debug.Log("Adding attack target");
			//}
			alarmed = true;
		}
		setAttacked ();
		return;
	}

	public void setAttacked()
	{
		lastTimeIWasAttacked = Time.time;
		attackedRecently = true;
	}


}


