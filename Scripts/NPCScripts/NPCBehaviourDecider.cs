using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviourDecider : MonoBehaviour {
	//fix bug where the NPC will sometimes think that another cop is a suspect, might be summit to do with when they're knocked out

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

	public bool lostTarget=false;
	public float loseTargetTimer = 5.0f;

	public bool attackedRecently = false;
	public float lastTimeIWasAttacked = 0.0f;

	public bool inLitRoom = true;
	public whatAiIsDoing doing = whatAiIsDoing.starting;

	public int decision=0;
	void Awake()
	{
		myController = this.gameObject.GetComponent<NPCController> ();
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update()
	{
		if (lostTarget == true) {
			loseTargetTimer -= Time.deltaTime;
		} else {
			loseTargetTimer = 5.0f;
		}
		decideFOVColor ();

	}



	public void OnUpdate () {
		if (myController.knockedDown == true || PersonHealth.playerHealth.healthValue<=0) {
			return;
		}

		GameObject g = isCorpseNearby ();
		if (g == null) {

		} else {
			Debug.Log (this.gameObject.name + " can see a corpse");
		}

		GameObject g2 = canSeeHostage ();
		if (g2 == null) {

		} else {
			Debug.Log (this.gameObject.name + " can see a hostage/knocked person");
		}

		////////Debug.Log (myController.detect.getDotProduct (CommonObjectsStore.player));
		inLitRoom = LevelTilemapController.me.areWeLit(this.transform.position);
		if (myType == AIType.guard) {
			unfreezeNPC ();
			decideViewRadius ();

			guardDoBehaviour ();
			 

			shouldNPCBeArmed ();
		} else if (myType == AIType.cop) {
			unfreezeNPC ();

			decideViewRadius ();
			copDoBehaviour ();

			if (PoliceController.me.underSiege == true) {
				alarmed = true;
			}

			shouldNPCBeArmed ();
		} else if (myType == AIType.swat) {
			unfreezeNPC ();

			decideViewRadius ();

			if (PoliceController.me.underSiege == true) {
				alarmed = true;
			}

			if (attackedRecently == true) {
				if (myController.memory.objectThatMadeMeSuspisious == null) {
					myController.memory.objectThatMadeMeSuspisious = CommonObjectsStore.player;
				}

				if (Time.time - lastTimeIWasAttacked >= 5.0f&& myController.detect.fov.visibleTargts.Contains(myController.memory.objectThatMadeMeSuspisious.transform)==false) {
					attackedRecently = false;
					alarmed = true;
				}
				//return;
			}
			//swatDoBehaviour ();
			//swatPassive ();
		} else if (myType == AIType.civilian) {
			decideViewRadius ();
			unfreezeNPC ();

			if (alarmed==false && PoliceController.me.backupHere==false && PoliceController.me.swatHere==false) {
				civilianPassive ();
			} else {
				civilianAlarmed ();
			}
		} else if (myType == AIType.hostage) {

		}
	}

	GameObject isCorpseNearby()
	{
		foreach (GameObject g in NPCManager.me.corpsesInWorld) {
			if (g.tag == "Dead/Knocked") {

				if (myController.detect.fov.visibleTargts.Contains (g.transform) == true) {
					myController.memory.seenCorpse = true;
					return g;
				}
				else if (Vector3.Distance(this.transform.position,g.transform.position)<myController.detect.fov.viewRadius) {
					if (myController.detect.isTargetInFrontOfUs (g) == true) {
						if (myController.detect.lineOfSightToTargetWithNoCollider (g) == true) {
							myController.memory.seenCorpse = true;
							return g;
						}
					}
				}
			}
		}
		return null;
	}

	GameObject isPersonTraspassing()
	{
		foreach (Transform t in myController.detect.fov.visibleTargts) {
			if (t == null) {
				continue;
			} else if (t.gameObject.tag == "Player") {
				RoomScript r = LevelController.me.getRoomObjectIsIn (t.gameObject);
				if (r == null) {

				} else {
					if (r.traspassing == true) {
						return t.gameObject;
					}
				}
			}
		}
		return null;
	}

	GameObject isItemNearby(){
		RoomScript myRoom = LevelController.me.getRoomObjectIsIn (this.gameObject);

		if (myRoom == null) {

		} else {
			foreach (Item i in ItemMoniter.me.itemsInWorld) {
				if (i.transform.parent == null) {
					RoomScript itemRoom = LevelController.me.getRoomObjectIsIn (i.gameObject);
					if (i.gameObject.activeInHierarchy == true) {
						if (itemRoom == null || itemRoom.itemsInRoomAtStart.Contains (i) == false) {
							if (myController.detect.fov.visibleTargts.Contains (i.transform) == true) {
								//myController.memory.seenCorpse = true;
								return i.gameObject;
							}
							else if (myController.detect.areWeNearTarget (i.gameObject)) {

								if (myController.detect.isTargetInFrontOfUs (i.gameObject)) {

									if (myController.detect.lineOfSightToTargetWithNoCollider (i.gameObject) || myController.detect.lineOfSightToTarget (i.gameObject)) {

										return i.gameObject;
									} 
								}
							}
						}
					}
				}
			}
		}
		return null;
	}


	public GameObject isHostileTargetNearby()
	{
		bool isExistingNPCAttacking = false;
		foreach (NPCController npc in NPCManager.me.npcControllers) {
			if (npc.currentBehaviour == null) {

			} else {
				if (npc.currentBehaviour.myType == behaviourType.attackTarget && npc.detect.fov.visibleTargts.Contains (CommonObjectsStore.player.transform)) {
					isExistingNPCAttacking = true;
				}
			}
		}

		foreach (Transform t in myController.detect.fov.visibleTargts) {
			if (t == null || t.gameObject.tag == "NPC") {
				continue;
			} else {
				if (t.gameObject.tag == "Player") {
					bool returnObj = false;

					PersonWeaponController pwc = t.GetComponent<PersonWeaponController> ();
					if (LevelController.me.suspects.Contains (t.gameObject) || myController.memory.peopleThatHaveAttackedMe.Contains (t.gameObject) || myController.memory.seenSuspect==true) {
						myController.memory.seenSuspect = true;
						returnObj = true;
						//return t.gameObject;
						Debug.Log ("Hostile 2");

					} 

					if (isExistingNPCAttacking == true) {
						myController.memory.seenSuspect = true;
						returnObj = true;
					}

					if (pwc == null || pwc.currentWeapon == null) {
						if (decideHowSuspiciousObjectIs (t.gameObject) >= 7) {
							Debug.Log ("Hostile 1");
							myController.memory.seenSuspect = true;
							returnObj = true;
						}
					} else {
						if (pwc.currentWeapon.illigal == true) {
							myController.memory.seenSuspect = true;
							myController.memory.seenArmedSuspect = true;
							returnObj = true;
							Debug.Log ("Hostile 3");

						} else {
							myController.memory.seenSuspect = true;
							myController.memory.seenArmedSuspect = true;
							returnObj = true;
							Debug.Log ("Hostile 4");

						}
					}

					if (returnObj == true) {
						Debug.Log ("returning hostile " + t.gameObject.name);
						return t.gameObject;
					}
				}
			}
		}
		return null;
	}

	GameObject suspiciousTarget()
	{
		foreach (Transform t in myController.detect.fov.visibleTargts) {
			if (t == null || t.gameObject.tag=="NPC") {
				continue;
			}

			if (t.gameObject.tag == "Player") {
				float suspicion = decideHowSuspiciousObjectIs (t.gameObject);
				if (suspicion > 1 && suspicion < 9) {
					return t.gameObject;
				}

				//if (suspicion >=0) {
				//	return t.gameObject;
				//}
			}
		}
		return null;
	}

	GameObject canSeeHostage()
	{
		foreach (Transform t in myController.detect.fov.visibleTargts) {
			if (t == null) {
				continue;
			}


			//if (t.gameObject.tag == "NPC") {
			NPCController npc = t.gameObject.GetComponent<NPCController> ();
			if (npc == null) {

			} else {
				if (npc.npcB.myType == AIType.hostage || npc.knockedDown == true) {
					//myController.memory.objectThatMadeMeSuspisious = t.gameObject;
					myController.memory.seenHostage = true;
					return t.gameObject;
				}
			}
		//	}
		}
		return null;
	}

	bool canWeSeeIncapedPersonAndPlayer()
	{
		bool seenPlayer = false, seenIncaped = false;

		foreach (Transform t in myController.detect.fov.visibleTargts) {
			if (t == null) {
				continue;
			}

			if (t.gameObject.tag == "Player") {
				seenPlayer = true;
			} else if (t.gameObject.tag == "Dead/Knocked") {
				seenIncaped = true;
			}
		}

		if (seenPlayer && seenIncaped) {
			return true;
		} else {
			return false;
		}
	}

	bool shouldWeRaiseAlarm()
	{

		//if (canWeSwitchBehaviour (whatAiIsDoing.raisingAlarm) == true) {

			if (myController.memory.seenCorpse == true && PoliceController.me.knowDead == false) {
				return true;
			}

			if (myController.memory.seenArmedSuspect == true && PoliceController.me.knowArmed == false) {
				return true;
			}

			if (myController.memory.seenHostage == true && PoliceController.me.knowHostage == false) {
				return true;
			}

			if (myController.memory.seenSuspect == true && PoliceController.me.knowSuspect == false) {
				return true;
			}

		if (myController.memory.beenAttacked == true && PoliceController.me.copsCalled == false || myController.memory.raiseAlarm==true && PoliceController.me.copsCalled==false) {
			return true;
		}
	//	}

		return false;
	}

	public void cop_decideBehaviour(){



		if (PoliceController.me.underSiege == false && PoliceController.me.swatCalled==false) {
			GameObject nearbyHostile = isHostileTargetNearby ();

			if (nearbyHostile == null) {
				GameObject nearbySuspicious = suspiciousTarget ();
				//	//////Debug.LogError ("Trying to find person to search");

				if (nearbySuspicious == null) {
					
					//myController.memory.noiseToInvestigate != Vector3.zero
					if (Vector2.Distance(myController.memory.noiseToInvestigate,Vector3.zero)>1) {
						if (canWeSwitchBehaviour (whatAiIsDoing.investigatingLocation) == true) {
							doing = whatAiIsDoing.investigatingLocation;
						}
					} else {
						GameObject nearbyHostage = canSeeHostage ();

						if (nearbyHostage == null) {
							GameObject nearbyCorpse = isCorpseNearby ();

							if (nearbyCorpse == null) {
								if (shouldWeRaiseAlarm()==true){
									doing = whatAiIsDoing.raisingAlarm;
								} else if (canWeSwitchBehaviour (whatAiIsDoing.starting) == true) {
									doing = whatAiIsDoing.starting;
								}
							} else {
								if (shouldWeRaiseAlarm()==false) {
									if (canWeSwitchBehaviour (whatAiIsDoing.guardingCorpse) == true) {
										myController.memory.objectThatMadeMeSuspisious = nearbyCorpse;
										doing = whatAiIsDoing.guardingCorpse;
									}
								} else {
									doing = whatAiIsDoing.raisingAlarm;
								}
							}

						} else {
							//////Debug.LogError (this.gameObject.name + " found hostage " + nearbyHostage);
							if (canWeSwitchBehaviour (whatAiIsDoing.freeingHostage) == true) {
								myController.memory.objectThatMadeMeSuspisious = nearbyHostage;
								doing = whatAiIsDoing.freeingHostage;
							}
						}
					}
				} else {
					if (canWeSwitchBehaviour (whatAiIsDoing.searchingPerson)) {
						doing = whatAiIsDoing.searchingPerson;
						myController.memory.objectThatMadeMeSuspisious = nearbySuspicious;
					}
				}

			} else {
				if (attemptToArrest == true) {
					if (canWeSwitchBehaviour (whatAiIsDoing.arresting) == true) {
						myController.memory.objectThatMadeMeSuspisious = nearbyHostile;
						doing = whatAiIsDoing.arresting;
					}
				} else {
					if (canWeSwitchBehaviour (whatAiIsDoing.attacking) == true) {
						myController.memory.objectThatMadeMeSuspisious = nearbyHostile;
						doing = whatAiIsDoing.attacking;
					}
				}
			}
		} else {
			GameObject nearbyHostile = isHostileTargetNearby ();

			if (nearbyHostile == null) {
				if (canWeSwitchBehaviour (whatAiIsDoing.guardingEntrance)) {
					doing = whatAiIsDoing.guardingEntrance;
				}
			} else {
				myController.memory.objectThatMadeMeSuspisious = nearbyHostile;
				if (attemptToArrest == true) {
					if (canWeSwitchBehaviour (whatAiIsDoing.arresting) == true) {
						myController.memory.objectThatMadeMeSuspisious = nearbyHostile;
						doing = whatAiIsDoing.arresting;
					}
				} else {
					if (canWeSwitchBehaviour (whatAiIsDoing.attacking) == true) {
						myController.memory.objectThatMadeMeSuspisious = nearbyHostile;
						doing = whatAiIsDoing.attacking;
					}
				}
			}
		}
	}

	void copDoBehaviour()
	{
		//////Debug.Log (this.gameObject.name + " Is doing " + doing.ToString ());
		cop_decideBehaviour ();

		if (doing == whatAiIsDoing.searchingPerson) {
			if (myController.currentBehaviour == null) {

			} else {
				if (doing != whatAiIsDoing.attacking && myController.currentBehaviour.myType == behaviourType.attackTarget) {
					doing = whatAiIsDoing.attacking;
				}
			}
		}

		if (doing == whatAiIsDoing.starting) {
			if (myController.currentBehaviour == null || myController.currentBehaviour.myType != behaviourType.searchRooms) {
				if (myController.currentBehaviour == null) {

				} else {
					Destroy (myController.currentBehaviour);
				}
				NPCBehaviour npcb = this.gameObject.AddComponent<NPCBehaviour_SearchRoom> ();
				myController.currentBehaviour = npcb;
				//PhoneTab_RadioHack.me.setNewText ("Searching room for suspects",radioHackBand.cop);

			}

		} else if (doing == whatAiIsDoing.attacking) {
			alarmed = true;

			if (myController.currentBehaviour == null || myController.currentBehaviour.myType != behaviourType.attackTarget) {
				if (myController.currentBehaviour == null) {

				} else {
					Destroy (myController.currentBehaviour);
				}
				NPCBehaviour_AttackTarget nb = this.gameObject.AddComponent<NPCBehaviour_AttackTarget> ();
				nb.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
				myController.currentBehaviour = nb;
			}

			/*if (myController.memory.objectThatMadeMeSuspisious == null) {
				myController.memory.objectThatMadeMeSuspisious = CommonObjectsStore.player;
				//if I'm going to add a system of AI commiting crimes 
			}
			if (myController.detect.fov.visibleTargts.Contains (myController.memory.objectThatMadeMeSuspisious.transform)) {
				myController.memory.noiseToInvestigate = myController.memory.objectThatMadeMeSuspisious.transform.position;


				lostTarget = false;

			} else {
				lostTarget = true;
				if (loseTargetTimer <= 0) {
					doing = whatAiIsDoing.raisingAlarm;
					myController.memory.noiseToInvestigate = Vector3.zero;
					loseTargetTimer = 5.0f;
					lostTarget = false;
				} else {
					if (myController.detect.fov.visibleTargts.Contains (myController.memory.objectThatMadeMeSuspisious.transform) == false && Vector3.Distance (this.transform.position, myController.memory.objectThatMadeMeSuspisious.transform.position) > myController.detect.fov.viewRadius) {
						if (myController.currentBehaviour == null || myController.currentBehaviour.myType != behaviourType.investigate) {
							if (myController.currentBehaviour == null) {

							} else {
								Destroy (myController.currentBehaviour);
							}

							if (myController.memory.noiseToInvestigate != Vector3.zero) {
								RoomScript r = LevelController.me.getRoomObjectIsIn (this.gameObject);
								NPCBehaviour_InvesigateLocation nb = this.gameObject.AddComponent<NPCBehaviour_InvesigateLocation> ();

								//if (r == null) {
								//	doing = whatAiIsDoing.starting;
								//} else {

								nb.location = myController.memory.noiseToInvestigate;
								//}

								//nb.location = 
								myController.currentBehaviour = nb;
							} 
						}
					} else {
						if (myController.currentBehaviour == null || myController.currentBehaviour.myType != behaviourType.attackTarget) {
							if (myController.currentBehaviour == null) {

							} else {
								Destroy (myController.currentBehaviour);
							}
							NPCBehaviour_AttackTarget nb = this.gameObject.AddComponent<NPCBehaviour_AttackTarget> ();
							nb.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
							myController.currentBehaviour = nb;
							lostTarget = false;

						}
					}
				}
			}
			if (myController.currentBehaviour == null) {
				if (shouldWeRaiseAlarm()==false) {
					doing = whatAiIsDoing.starting;
				} else {
					doing = whatAiIsDoing.raisingAlarm;
				}
			}*/
		} else if (doing == whatAiIsDoing.raisingAlarm) {
			if (alarmed == false) {
				suspisious = true;
			}
			//////Debug.Log ("Cop is raising alarm, alarm is " + NPCBehaviourDecider.copAlarm);
			//if (NPCBehaviourDecider.copAlarm == true) {
			//	doing = whatAiIsDoing.starting;
			//}

			if (myController.currentBehaviour.myType != behaviourType.raiseAlarm && shouldWeRaiseAlarm()) {
				if (myController.currentBehaviour == null) {

				} else {
					Destroy (myController.currentBehaviour);
				}
				NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_CopRaiseAlarm> ();
				myController.currentBehaviour = nb;
				//PhoneTab_RadioHack.me.setNewText ("I'm going to call the cops",radioHackBand.buisness);

				//myController.memory.noiseToInvestigate = Vector3.zero;
			}

			if (myController.currentBehaviour == null) {
				doing = whatAiIsDoing.starting;
			}

			if (shouldWeRaiseAlarm()==false) {
				doing = whatAiIsDoing.starting;

			}

		} else if (doing == whatAiIsDoing.investigatingLocation) {
			if (alarmed == false) {
				suspisious = true;
			}
			if (myController.currentBehaviour == null || myController.currentBehaviour.myType != behaviourType.investigate) {
				if (myController.currentBehaviour == null) {

				} else {
					Destroy (myController.currentBehaviour);
				}
				NPCBehaviour_InvesigateLocation nb = this.gameObject.AddComponent<NPCBehaviour_InvesigateLocation> ();


				//nb.location = 
				myController.currentBehaviour = nb;
			}
			if (myController.currentBehaviour == null) {
				doing = whatAiIsDoing.starting;
			}
		} else if (doing == whatAiIsDoing.searching) {
			if (alarmed == false) {
				suspisious = true;
			}
			if (myController.currentBehaviour == null) {
				if (shouldWeRaiseAlarm() == false) {
					doing = whatAiIsDoing.starting;
				} else {
					doing = whatAiIsDoing.raisingAlarm;
					raiseAlarmOnSearch = false;
				}
			}

			if (myController.currentBehaviour == null || myController.currentBehaviour.myType != behaviourType.investigate) {
				if (myController.currentBehaviour == null) {

				} else {
					Destroy (myController.currentBehaviour);
				}
				RoomScript r = LevelController.me.getRoomObjectIsIn (this.gameObject);
				NPCBehaviour_InvesigateLocation nb = this.gameObject.AddComponent<NPCBehaviour_InvesigateLocation> ();

				if (r == null) {
					doing = whatAiIsDoing.starting;
				} else {
					nb.pointToInvestigate = r.getRandomPoint ();
					myController.currentBehaviour = nb;
				}

				//nb.location = 

			}

		} else if (doing == whatAiIsDoing.confiscating) {
			//////Debug.Break ();
			if (alarmed == false) {
				suspisious = true;
			}
			if (myController.currentBehaviour == null && Vector3.Distance (this.transform.position, LevelController.me.itemDepositLoc.position) < 2.5f) {
				doing = whatAiIsDoing.starting;
			}

			if (myController.currentBehaviour == null || myController.currentBehaviour.myType != behaviourType.investigate) {
				if (myController.memory.objectThatMadeMeSuspisious == null) {
					return;
				}

				if (myController.currentBehaviour == null) {

				} else {
					Destroy (myController.currentBehaviour);
				}
				NPCBehaviour_InvestigateObject newBehaviour = this.gameObject.AddComponent<NPCBehaviour_InvestigateObject> ();
				newBehaviour.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
				myController.currentBehaviour = newBehaviour;
				//PhoneTab_RadioHack.me.setNewText ("Seen something odd, going to check it out",radioHackBand.buisness);
			}
		} else if (doing == whatAiIsDoing.searchingPerson) {
			//////Debug.LogError ("Wanting to search person");//need to fix the deciding whether object is suspicious or not
			SearchedMarker suspisiousOf = null;
			if (alarmed == false) {
				suspisious = true;
			}

			if (myController.memory.objectThatMadeMeSuspisious == null) {
				doing = whatAiIsDoing.starting;

			}

			if (myController.memory.objectThatMadeMeSuspisious.GetComponent<SearchedMarker> () == null) {
				doing = whatAiIsDoing.starting;
			} else {
				suspisiousOf = myController.memory.objectThatMadeMeSuspisious.GetComponent<SearchedMarker> ();
				if (suspisiousOf.searchedBy.Contains (this.gameObject) == true && myController.currentBehaviour == null) {
					doing = whatAiIsDoing.starting;

				} else if (suspisiousOf.searchedBy.Contains (this.gameObject) == false) {
					if (myController.currentBehaviour.myType != behaviourType.searchPerson) {
						//////Debug.Log ("Wanting to search person " + myController.memory.objectThatMadeMeSuspisious.name);
						if (myController.currentBehaviour == null) {

						} else {
							Destroy (myController.currentBehaviour);
						}
						NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_SearchPerson> ();
						myController.currentBehaviour = nb;
						nb.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
						suspisiousOf.addToSearchedBy (this.gameObject);
					}
				}
			}


			if (myController.currentBehaviour == null) {
				if (myController.memory.objectThatMadeMeSuspisious == null) {
					doing = whatAiIsDoing.starting;
				} else {
					if (attemptToArrest == false) {
						doing = whatAiIsDoing.attacking;
					} else {
						doing = whatAiIsDoing.arresting;
					}
				}
			}
		} else if (doing == whatAiIsDoing.arresting) {
			if (myController.currentBehaviour.myType != behaviourType.arrestTarget) {
				if (myController.currentBehaviour == null) {

				} else {
					Destroy (myController.currentBehaviour);
				}
				NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_ArrestTarget> ();
				nb.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
				myController.currentBehaviour = nb;
				//PhoneTab_RadioHack.me.setNewText ("Moving to arrest target.",radioHackBand.cop);

			}
		} else if (doing == whatAiIsDoing.freeingHostage) { 
			
			if (myController.currentBehaviour == null) {
				if (myController.memory.objectThatMadeMeSuspisious == null) {
					if (shouldWeRaiseAlarm()==true) {
						doing = whatAiIsDoing.raisingAlarm;
					} else {
						doing = whatAiIsDoing.starting;
					}

				} else {
					NPCController hostage = myController.memory.objectThatMadeMeSuspisious.GetComponent<NPCController> ();
					if (hostage == null) {
						if (shouldWeRaiseAlarm()==true) {
							doing = whatAiIsDoing.raisingAlarm;
						} else {
							doing = whatAiIsDoing.starting;
						}
					} else {
						if (hostage.npcB.myType != AIType.hostage) {
							if (shouldWeRaiseAlarm()==true) {
								doing = whatAiIsDoing.raisingAlarm;
							} else {
								doing = whatAiIsDoing.starting;
							}

						}
					}
				}
			}
		
			if (myController.currentBehaviour.myType != behaviourType.freeHostage) {
				if (myController.currentBehaviour == null) {
					if (myController.memory.objectThatMadeMeSuspisious == null) {
						if (shouldWeRaiseAlarm()==true) {
							doing = whatAiIsDoing.raisingAlarm;
						} else {
							doing = whatAiIsDoing.starting;
						}

					} else {
						NPCController hostage = myController.memory.objectThatMadeMeSuspisious.GetComponent<NPCController> ();
						if (hostage == null) {
							if (shouldWeRaiseAlarm()==true) {
								doing = whatAiIsDoing.raisingAlarm;
							} else {
								doing = whatAiIsDoing.starting;
							}
						} else {
							if (hostage.npcB.myType != AIType.hostage) {
								if (shouldWeRaiseAlarm()==true) {
									doing = whatAiIsDoing.raisingAlarm;
								} else {
									doing = whatAiIsDoing.starting;
								}

							}
						}
					}
				} else {
					Destroy (myController.currentBehaviour);
				}
				NPCBehaviour npcb = this.gameObject.AddComponent<NPCBehaviour_FreeHostage> ();
				myController.currentBehaviour = npcb;
				//PhoneTab_RadioHack.me.setNewText ("Someone got tied up, freeing them now",radioHackBand.buisness);

			}
		} else if (doing == whatAiIsDoing.guardingCorpse) {
			if (myController.currentBehaviour.myType != behaviourType.guardCorpse) {
				if (myController.currentBehaviour == null) {

				} else {
					Destroy (myController.currentBehaviour);
				}
				NPCBehaviour newBehaviour = this.gameObject.AddComponent<NPCBehaviour_GuardCorpse> ();
				//newBehaviour.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
				myController.currentBehaviour = newBehaviour;
				//PhoneTab_RadioHack.me.setNewText ("Guarding a body untill backup gets here.",radioHackBand.cop);

			}
		} else if (doing == whatAiIsDoing.guardingEntrance) {
			if (myController.currentBehaviour==null || myController.currentBehaviour.myType != behaviourType.guardEntrance) {
				if (myController.currentBehaviour == null) {

				} else {
					Destroy (myController.currentBehaviour);
				}
				NPCBehaviour newBehaviour = this.gameObject.AddComponent<NPCBehaviour_GuardEntrance> ();
				//newBehaviour.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
				myController.currentBehaviour = newBehaviour;
			}
		}

	}

	public void guard_decideBehaviour()
	{
		decision = 0;
		GameObject nearbyHostile = isHostileTargetNearby ();
		if (PoliceController.me.swatHere == false) {
			if (nearbyHostile == null) {
				decision++;
				GameObject nearbySuspicious = null;// suspiciousTarget ();
				////////Debug.LogError ("Trying to find person to search");

				if (nearbySuspicious == null) {
					decision++;

					GameObject traspasser = isPersonTraspassing ();
					if (traspasser == null) {
						decision++;

						if (myController.memory.noiseToInvestigate != Vector3.zero) {
							if (canWeSwitchBehaviour (whatAiIsDoing.investigatingLocation) == true) {
								doing = whatAiIsDoing.investigatingLocation;
							}
						} else {
							decision++;

							GameObject nearbyHostage = canSeeHostage ();

							if (nearbyHostage == null) {
								decision++;

								GameObject nearbyCorpse = isCorpseNearby ();

								if (nearbyCorpse == null) {
									decision++;

									GameObject nearbyItem = isItemNearby ();

									if (nearbyItem == null) {
										decision++;

										if (shouldWeRaiseAlarm () == true) {
											decision++;
											if (canWeSwitchBehaviour (whatAiIsDoing.raisingAlarm)) {
												doing = whatAiIsDoing.raisingAlarm;
											}
											////Debug.Log ("Setting alarm 1");
										} else {
											if (canWeSwitchBehaviour (whatAiIsDoing.starting)) {
												doing = whatAiIsDoing.starting;
											}
										}
									} else {
										if (canWeSwitchBehaviour (whatAiIsDoing.confiscating) == true) {
											doing = whatAiIsDoing.confiscating;
											myController.memory.objectThatMadeMeSuspisious = nearbyItem;

										}
									}

								} else {
									if (shouldWeRaiseAlarm () == true) {
										doing = whatAiIsDoing.raisingAlarm;
										////Debug.Log ("setting alarm 2");
										myController.memory.objectThatMadeMeSuspisious = nearbyCorpse;

									} else {
										doing = whatAiIsDoing.starting;
									}
								}
							} else {
								//////Debug.LogError (this.gameObject.name + " found a hostage, attempting to free");
								if (canWeSwitchBehaviour (whatAiIsDoing.freeingHostage) == true) {
									doing = whatAiIsDoing.freeingHostage;
									myController.memory.objectThatMadeMeSuspisious = nearbyHostage;
								}
							}
						}
					} else {
						if (canWeSwitchBehaviour (whatAiIsDoing.escortingTraspass) == true) {
							doing = whatAiIsDoing.escortingTraspass;
							myController.memory.objectThatMadeMeSuspisious = traspasser;
						}
					}
				} else {
					//////Debug.LogError ("Found person to search");

					if (canWeSwitchBehaviour (whatAiIsDoing.searchingPerson)) {
						doing = whatAiIsDoing.searchingPerson;
						myController.memory.objectThatMadeMeSuspisious = nearbySuspicious;
					}
				}
			} else {
				doing = whatAiIsDoing.attacking;
				myController.memory.objectThatMadeMeSuspisious = nearbyHostile;
			}
		} else {
			if (nearbyHostile == null) {
				if (canWeSwitchBehaviour (whatAiIsDoing.evacuate) == true) {
					doing = whatAiIsDoing.evacuate;
				}
			} else {
				doing = whatAiIsDoing.attacking;
				myController.memory.objectThatMadeMeSuspisious = nearbyHostile;
			}
		}
	}


	public bool raiseAlarmOnSearch = false;
	void guardDoBehaviour()
	{
//		//////Debug.Log (this.gameObject.name + " Is doing " + doing.ToString ());

		guard_decideBehaviour ();
		if (myController.currentBehaviour == null) {
			doing = whatAiIsDoing.starting;
		}

		if (doing == whatAiIsDoing.starting) {
			if (patrol == true) {
				if (myController.currentBehaviour == null || myController.currentBehaviour.myType != behaviourType.patrol) {
					if (myController.currentBehaviour == null) {

					} else {
						Destroy (myController.currentBehaviour);
					}
					NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_PatrolRoute> ();
					myController.currentBehaviour = nb;

				}
			} else {
				if (myController.currentBehaviour == null || myController.currentBehaviour.myType != behaviourType.guardLoc) {
					if (myController.currentBehaviour == null) {

					} else {
						Destroy (myController.currentBehaviour);
					}					
					NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_GuardLocation> ();
					myController.currentBehaviour = nb;

				}
			}
			if (myController.currentBehaviour == null) {
				if (shouldWeRaiseAlarm () == true) {
					doing = whatAiIsDoing.raisingAlarm;
				} else {
					doing = whatAiIsDoing.starting;
					////Debug.Log ("Setting alarm 3");
				}
			}
		} else if (doing == whatAiIsDoing.attacking) {
			alarmed = true;
			if (myController.currentBehaviour == null || myController.currentBehaviour.myType != behaviourType.attackTarget) {
				if (myController.currentBehaviour == null) {

				} else {
					Destroy (myController.currentBehaviour);
				}

				NPCBehaviour_AttackTarget nb = this.gameObject.AddComponent<NPCBehaviour_AttackTarget> ();
				nb.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
				myController.currentBehaviour = nb;
			}


			/*if (myController.memory.objectThatMadeMeSuspisious == null) {
				myController.memory.objectThatMadeMeSuspisious = CommonObjectsStore.player;
				//if I'm going to add a system of AI commiting crimes 
			}

			if (myController.detect.fov.visibleTargts.Contains (myController.memory.objectThatMadeMeSuspisious.transform)) {
				loseTargetTimer = 5.0f;

				myController.memory.noiseToInvestigate = myController.memory.objectThatMadeMeSuspisious.transform.position;

				if (myController.currentBehaviour == null || myController.currentBehaviour.myType != behaviourType.attackTarget) {
					if (myController.currentBehaviour == null) {

					} else {
						Destroy (myController.currentBehaviour);
					}
					//////Debug.Log ("Adding attack 2");

					NPCBehaviour_AttackTarget nb = this.gameObject.AddComponent<NPCBehaviour_AttackTarget> ();
					nb.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
					myController.currentBehaviour = nb;
				}
				lostTarget = false;

			} else {
				
				lostTarget = true;

				if (loseTargetTimer <= 0) {
					doing = whatAiIsDoing.raisingAlarm;
					////Debug.Log ("Setting alarm 4");
					myController.memory.noiseToInvestigate = Vector3.zero;
					loseTargetTimer = 5.0f;
				} else {
					if (myController.detect.fov.visibleTargts.Contains (myController.memory.objectThatMadeMeSuspisious.transform) == false && Vector3.Distance (this.transform.position, myController.memory.objectThatMadeMeSuspisious.transform.position) > myController.detect.fov.viewRadius) {
						if (myController.currentBehaviour == null || myController.currentBehaviour.myType != behaviourType.investigate) {
							if (myController.currentBehaviour == null) {

							} else {
								Destroy (myController.currentBehaviour);
							}

							if (myController.memory.noiseToInvestigate != Vector3.zero) {
								RoomScript r = LevelController.me.getRoomObjectIsIn (this.gameObject);
								NPCBehaviour_InvesigateLocation nb = this.gameObject.AddComponent<NPCBehaviour_InvesigateLocation> ();

								//if (r == null) {
								//	doing = whatAiIsDoing.starting;
								//} else {

								nb.location = myController.memory.noiseToInvestigate;
								//}

								//nb.location = 
								myController.currentBehaviour = nb;
							} 
						}
					} else {
						if (myController.currentBehaviour == null || myController.currentBehaviour.myType != behaviourType.attackTarget) {
							if (myController.currentBehaviour == null) {

							} else {
								Destroy (myController.currentBehaviour);
							}
							//////Debug.Log ("Adding attack 1");
							NPCBehaviour_AttackTarget nb = this.gameObject.AddComponent<NPCBehaviour_AttackTarget> ();
							nb.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
							myController.currentBehaviour = nb;
							lostTarget = false;

						}
					}
				}
			}
			if (myController.currentBehaviour == null) {
				if (shouldWeRaiseAlarm () == false) {
					doing = whatAiIsDoing.starting;
				} else {
					doing = whatAiIsDoing.raisingAlarm;
					////Debug.Log ("Setting alarm 5");
				}
			}*/
		} else if (doing == whatAiIsDoing.raisingAlarm) {
			if (alarmed == false) {
				suspisious = true;
			}

			if (myController.currentBehaviour == null && shouldWeRaiseAlarm () || myController.currentBehaviour.myType != behaviourType.raiseAlarm && shouldWeRaiseAlarm ()) {
				if (myController.currentBehaviour == null) {

				} else {
					Destroy (myController.currentBehaviour);
				}
				NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_RaiseAlarm> ();
				myController.currentBehaviour = nb;
				//PhoneTab_RadioHack.me.setNewText ("I'm going to call the cops",radioHackBand.buisness);

				//myController.memory.noiseToInvestigate = Vector3.zero;
			}

			if (myController.currentBehaviour == null) {
				doing = whatAiIsDoing.starting;
			}


			if (shouldWeRaiseAlarm () == false) {
				if (myController.currentBehaviour == null) {

				} else {
					Destroy (myController.currentBehaviour);
				}
				doing = whatAiIsDoing.starting;
			}
		} else if (doing == whatAiIsDoing.investigatingLocation) {
			if (alarmed == false) {
				suspisious = true;
			}
			if (myController.currentBehaviour == null || myController.currentBehaviour.myType != behaviourType.investigate) {
				if (myController.currentBehaviour == null) {

				} else {
					Destroy (myController.currentBehaviour);
				}
				NPCBehaviour_InvesigateLocation nb = this.gameObject.AddComponent<NPCBehaviour_InvesigateLocation> ();


				//nb.location = 
				myController.currentBehaviour = nb;
			}
			if (myController.currentBehaviour == null) {
				doing = whatAiIsDoing.starting;
			}
		} else if (doing == whatAiIsDoing.searching) {
			if (alarmed == false) {
				suspisious = true;
			}
			if (myController.currentBehaviour == null) {
				if (shouldWeRaiseAlarm () == false) {
					doing = whatAiIsDoing.starting;
				} else {
					doing = whatAiIsDoing.raisingAlarm;
					////Debug.Log ("Setting alarm 6");
					raiseAlarmOnSearch = false;
				}
			}

			if (myController.currentBehaviour == null || myController.currentBehaviour.myType != behaviourType.investigate) {
				if (myController.currentBehaviour == null) {

				} else {
					Destroy (myController.currentBehaviour);
				}
				RoomScript r = LevelController.me.getRoomObjectIsIn (this.gameObject);
				NPCBehaviour_InvesigateLocation nb = this.gameObject.AddComponent<NPCBehaviour_InvesigateLocation> ();

				if (r == null) {
					doing = whatAiIsDoing.starting;
				} else {
					nb.pointToInvestigate = r.getRandomPoint ();
				}

				//nb.location = 
				myController.currentBehaviour = nb;
			}

		} else if (doing == whatAiIsDoing.confiscating) {
			//////Debug.Break ();
			if (alarmed == false) {
				suspisious = true;
			}
			if (myController.currentBehaviour == null && Vector3.Distance (this.transform.position, LevelController.me.itemDepositLoc.position) < 2.5f) {
				doing = whatAiIsDoing.starting;
			}

			if (myController.currentBehaviour == null || myController.currentBehaviour.myType != behaviourType.investigate) {
				if (myController.memory.objectThatMadeMeSuspisious == null) {
					return;
				}

				if (myController.currentBehaviour == null) {

				} else {
					Destroy (myController.currentBehaviour);
				}
				NPCBehaviour_InvestigateObject newBehaviour = this.gameObject.AddComponent<NPCBehaviour_InvestigateObject> ();
				newBehaviour.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
				myController.currentBehaviour = newBehaviour;
				//PhoneTab_RadioHack.me.setNewText ("Seen something odd, going to check it out",radioHackBand.buisness);
			}
		} else if (doing == whatAiIsDoing.searchingPerson) {
			//////Debug.LogError ("Wanting to search person");//need to fix the deciding whether object is suspicious or not
			SearchedMarker suspisiousOf = null;
			if (alarmed == false) {
				suspisious = true;
			}

			if (myController.memory.objectThatMadeMeSuspisious == null) {
				doing = whatAiIsDoing.starting;

			}

			if (myController.memory.objectThatMadeMeSuspisious == null) {

				myController.memory.objectThatMadeMeSuspisious = CommonObjectsStore.player;
			}

			if (myController.memory.objectThatMadeMeSuspisious.GetComponent<SearchedMarker> () == null) {
				doing = whatAiIsDoing.starting;
			} else {
				suspisiousOf = myController.memory.objectThatMadeMeSuspisious.GetComponent<SearchedMarker> ();
				if (suspisiousOf.searchedBy.Contains (this.gameObject) == true && myController.currentBehaviour == null) {
					doing = whatAiIsDoing.starting;

				} else if (suspisiousOf.searchedBy.Contains (this.gameObject) == false) {
					if (myController.currentBehaviour.myType != behaviourType.searchPerson) {
						//////Debug.Log ("Wanting to search person " + myController.memory.objectThatMadeMeSuspisious.name);
						if (myController.currentBehaviour == null) {

						} else {
							Destroy (myController.currentBehaviour);
						}
						NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_SearchPerson> ();
						myController.currentBehaviour = nb;
						nb.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
						suspisiousOf.addToSearchedBy (this.gameObject);
					}
				}
			}

		
			if (myController.currentBehaviour == null) {
				if (myController.memory.objectThatMadeMeSuspisious == null) {
					doing = whatAiIsDoing.starting;
				} else {
					doing = whatAiIsDoing.attacking;
				}
			}
		} else if (doing == whatAiIsDoing.freeingHostage) {

			if (myController.currentBehaviour == null) {
				if (myController.memory.objectThatMadeMeSuspisious == null) {
					if (shouldWeRaiseAlarm () == true) {
						doing = whatAiIsDoing.raisingAlarm;
						////Debug.Log ("Setting alarm 7");
					} else {
						doing = whatAiIsDoing.starting;
					}

				} else {
					NPCController hostage = myController.memory.objectThatMadeMeSuspisious.GetComponent<NPCController> ();
					if (hostage == null) {
						if (shouldWeRaiseAlarm () == true) {
							doing = whatAiIsDoing.raisingAlarm;
							////Debug.Log ("Setting alarm 8");
						} else {
							doing = whatAiIsDoing.starting;
						}
					} else {
						if (hostage.npcB.myType != AIType.hostage) {
							if (shouldWeRaiseAlarm () == true) {
								doing = whatAiIsDoing.raisingAlarm;
								////Debug.Log ("Setting alarm 9");
							} else {
								doing = whatAiIsDoing.starting;
							}

						}
					}
				}

			}

			if (myController.currentBehaviour.myType != behaviourType.freeHostage) {


				if (myController.currentBehaviour == null) {
					if (myController.memory.objectThatMadeMeSuspisious == null) {
						if (shouldWeRaiseAlarm () == true) {
							doing = whatAiIsDoing.raisingAlarm;
							////Debug.Log ("Setting alarm 10");
						} else {
							doing = whatAiIsDoing.starting;
						}

					} else {
						NPCController hostage = myController.memory.objectThatMadeMeSuspisious.GetComponent<NPCController> ();
						if (hostage == null) {
							if (shouldWeRaiseAlarm () == true) {
								doing = whatAiIsDoing.raisingAlarm;
								////Debug.Log ("Setting alarm 11");
							} else {
								doing = whatAiIsDoing.starting;
							}
						} else {
							if (shouldWeRaiseAlarm () == true) {
								doing = whatAiIsDoing.raisingAlarm;
								////Debug.Log ("Setting alarm 12");
							} else {
								doing = whatAiIsDoing.starting;

							}
						}
					}

				} else {
					Destroy (myController.currentBehaviour);
				}
				NPCBehaviour npcb = this.gameObject.AddComponent<NPCBehaviour_FreeHostage> ();
				myController.currentBehaviour = npcb;


				//PhoneTab_RadioHack.me.setNewText ("Someone got tied up, freeing them now",radioHackBand.buisness);

			}
		} else if (doing == whatAiIsDoing.escortingTraspass) {
			if (myController.currentBehaviour.myType != behaviourType.traspassing) {
				Destroy (myController.currentBehaviour);
				NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_EscortOutOfRestrictedArea> ();
				myController.currentBehaviour = nb;
				//PhoneTab_RadioHack.me.setNewText ("Found a trespasser, escorting them out now.",radioHackBand.buisness);

				//suspisiousOf.searchedBy.Add (this.gameObject);
			}

			if (myController.memory.seenSuspect == true && myController.currentBehaviour.myType == null) {
				Debug.LogError ("SET TO ATTACK AFTER ESCORT FAILED");
				doing = whatAiIsDoing.attacking;
				if (myController.currentBehaviour == null || myController.currentBehaviour.myType != behaviourType.attackTarget) {
					if (myController.currentBehaviour == null) {

					} else {
						Destroy (myController.currentBehaviour);
					}
					//////Debug.Log ("Adding attack 2");

					NPCBehaviour_AttackTarget nb = this.gameObject.AddComponent<NPCBehaviour_AttackTarget> ();
					nb.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
					myController.currentBehaviour = nb;
				}
			}
		} else if (doing == whatAiIsDoing.evacuate) {
			if (myController.currentBehaviour == null || myController.currentBehaviour.myType != behaviourType.exitLevel) {
				if (myController.currentBehaviour == null) {

				} else {
					Destroy (myController.currentBehaviour);
				}
				NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_EvacuateBuilding> ();
				myController.currentBehaviour = nb;
			}
		}
	}

	void swatBehaviourDecide()
	{
		GameObject nearbyHostile = isHostileTargetNearby ();

		if (nearbyHostile == null) {
			if (myController.memory.objectThatMadeMeSuspisious == null) {
				if (PoliceController.me.buildingSurrounded == true) {
					//swat raid building
					doing=whatAiIsDoing.swatRaidBuilding;
				} else {
					//swat go to form up point
					doing=whatAiIsDoing.swatFormUp;

				}
			} else {
				if (doAnySwatHaveEyesOnTarget () == true) {
					doing = whatAiIsDoing.swatAttack;
				} else {
					if (PoliceController.me.buildingSurrounded == true) {
						//swat raid building
						doing=whatAiIsDoing.swatRaidBuilding;
					} else {
						//swat go to form up point
						doing=whatAiIsDoing.swatFormUp;

					}
				}
			}


		} else {
			//attack nearby hostile
			myController.memory.objectThatMadeMeSuspisious=nearbyHostile;
			doing=whatAiIsDoing.swatAttack;

		}
	}

	bool doAnySwatHaveEyesOnTarget()
	{
		if (myController.memory.objectThatMadeMeSuspisious == false) {
			return false;
		}

		foreach (NPCController npc in NPCManager.me.npcControllers) {
			if (npc.npcB.myType != AIType.swat) {
				continue;
			} else {
				if (npc.detect.fov.visibleTargts.Contains (myController.memory.objectThatMadeMeSuspisious.transform) == true) {
					return true;
				}
			}

		}
		return false;
	}

	void swatDoBehaviour()
	{
		swatBehaviourDecide ();

		if (doing == whatAiIsDoing.swatRaidBuilding) {
			if (myController.currentBehaviour==null|| myController.currentBehaviour.myType !=behaviourType.searchRooms) {
				setAllSwatToSearch ();
			//	if (myController.currentBehaviour == null) {

			//	} else {
			//		Destroy (myController.currentBehaviour);
			//	}


			}
		} else if (doing == whatAiIsDoing.swatFormUp) {
			if (myController.currentBehaviour==null||myController.currentBehaviour.myType !=behaviourType.formUp ) {
				setAllSwatToFormUp ();

			}
		} else if( doing == whatAiIsDoing.swatAttack) {
			if (myController.currentBehaviour==null||myController.currentBehaviour.myType != behaviourType.attackTarget ) {
				setAllSwatToAttack ();
							
			}
		}
	}

	bool canWeSwitchBehaviour(whatAiIsDoing toSwitchTo)
	{
		if (doing == whatAiIsDoing.attacking) {
			return false;
		}

		if (doing == whatAiIsDoing.raisingAlarm) {
			if (toSwitchTo == whatAiIsDoing.attacking||toSwitchTo==whatAiIsDoing.searchingPerson) {
				return true;
			} else {
				return false;
			}
		}

		if (doing == whatAiIsDoing.investigatingLocation) { 
			if (toSwitchTo == whatAiIsDoing.attacking ||  toSwitchTo == whatAiIsDoing.investigatingLocation || toSwitchTo == whatAiIsDoing.raisingAlarm||toSwitchTo==whatAiIsDoing.searchingPerson|| toSwitchTo==whatAiIsDoing.arresting || toSwitchTo==whatAiIsDoing.freeingHostage || toSwitchTo==whatAiIsDoing.guardingEntrance||toSwitchTo== whatAiIsDoing.evacuate) {
				return true;
			} else {
				return false;
			}
		}

		if (doing == whatAiIsDoing.confiscating ) {
			if (toSwitchTo == whatAiIsDoing.attacking || toSwitchTo == whatAiIsDoing.investigatingLocation || toSwitchTo == whatAiIsDoing.raisingAlarm||toSwitchTo==whatAiIsDoing.searchingPerson|| toSwitchTo==whatAiIsDoing.guardingEntrance||toSwitchTo== whatAiIsDoing.evacuate) {
				return true;
			} else {
				return false;
			}
		}


		if (doing == whatAiIsDoing.searchingPerson) {
			if (toSwitchTo == whatAiIsDoing.attacking || toSwitchTo == whatAiIsDoing.investigatingLocation || toSwitchTo == whatAiIsDoing.raisingAlarm ||toSwitchTo== whatAiIsDoing.evacuate) {
				return true;
			} else {
				return false;
			}
		}

		if (doing == whatAiIsDoing.searching) {
			if (toSwitchTo == whatAiIsDoing.attacking || toSwitchTo == whatAiIsDoing.investigatingLocation || toSwitchTo == whatAiIsDoing.raisingAlarm||toSwitchTo==whatAiIsDoing.searchingPerson || toSwitchTo==whatAiIsDoing.arresting|| toSwitchTo==whatAiIsDoing.guardingEntrance || toSwitchTo==whatAiIsDoing.escortingTraspass||toSwitchTo== whatAiIsDoing.evacuate) {
				return true;
			} else {
				return false;
			}
		}

		if (doing == whatAiIsDoing.arresting) {
			if (toSwitchTo == whatAiIsDoing.attacking  || toSwitchTo == whatAiIsDoing.raisingAlarm||toSwitchTo== whatAiIsDoing.evacuate) {
				return true;
			} else {
				return false;
			}
		}

		if (doing == whatAiIsDoing.freeingHostage) {
			if (toSwitchTo == whatAiIsDoing.attacking  || toSwitchTo == whatAiIsDoing.raisingAlarm) {
				return true;
			} else {
				return false;
			}
		}

		if (doing == whatAiIsDoing.guardingEntrance) {
			if (toSwitchTo == whatAiIsDoing.arresting || toSwitchTo == whatAiIsDoing.attacking) {
				return true;
			} else {
				return false;
			}
		}

		if (doing == whatAiIsDoing.escortingTraspass) {
			if (toSwitchTo == whatAiIsDoing.attacking  || toSwitchTo == whatAiIsDoing.raisingAlarm || toSwitchTo == whatAiIsDoing.arresting) {
				return true;
			} else {
				return false;
			}
		}

		if (doing == whatAiIsDoing.starting) {
			return true;
		}


		return false;
	}


	void decideViewRadius()
	{

		if (myType == AIType.guard) {
			if (alarmed == true) {
				if (myController.detect.fov.viewRadius <= myController.detect.fov.startingViewRadius + 5) {
					myController.detect.fov.viewRadius = myController.detect.fov.startingViewRadius + 5;
				}

				if (LevelController.me.suspects.Contains (CommonObjectsStore.player)) {
					//myController.detect.fov.shaderMaterial = CommonObjectsStore.me.aggressive;
				}
			} else if (suspisious == true|| globalAlarm==true) {
				if (myController.detect.fov.viewRadius <= myController.detect.fov.startingViewRadius + 3) {
					myController.detect.fov.viewRadius = myController.detect.fov.startingViewRadius + 3;
				//	myController.detect.fov.shaderMaterial = CommonObjectsStore.me.suspicious;
				}
			} else {
				if (myController.detect.fov.viewRadius <= myController.detect.fov.startingViewRadius) {
					myController.detect.fov.viewRadius = myController.detect.fov.startingViewRadius;
				//	myController.detect.fov.shaderMaterial = CommonObjectsStore.me.passive;
				}
			}
		} else if (myType == AIType.cop) {
			if (alarmed == true) {
				if (myController.detect.fov.viewRadius <= myController.detect.fov.startingViewRadius + 5) {
					myController.detect.fov.viewRadius = myController.detect.fov.startingViewRadius + 5;
				}

				if (LevelController.me.suspects.Contains (CommonObjectsStore.player)) {
					//myController.detect.fov.shaderMaterial = CommonObjectsStore.me.aggressive;
				}
			} else if (suspisious == true|| copAlarm==true) {
				if (myController.detect.fov.viewRadius <= myController.detect.fov.startingViewRadius + 3) {
					myController.detect.fov.viewRadius = myController.detect.fov.startingViewRadius + 3;
					//myController.detect.fov.shaderMaterial = CommonObjectsStore.me.suspicious;
				}
			} else {
				if (myController.detect.fov.viewRadius <= myController.detect.fov.startingViewRadius) {
					myController.detect.fov.viewRadius = myController.detect.fov.startingViewRadius;
					//myController.detect.fov.shaderMaterial = CommonObjectsStore.me.passive;
				}
			}
		} else if (myType == AIType.swat) {
			if (alarmed == true ) {
				if (myController.detect.fov.viewRadius <= myController.detect.fov.startingViewRadius + 5) {
					myController.detect.fov.viewRadius = myController.detect.fov.startingViewRadius + 5;
				}

				if (LevelController.me.suspects.Contains (CommonObjectsStore.player)) {
					//myController.detect.fov.shaderMaterial = CommonObjectsStore.me.aggressive;
				}
			} else{
				if (myController.detect.fov.viewRadius <= myController.detect.fov.startingViewRadius + 3) {
					myController.detect.fov.viewRadius = myController.detect.fov.startingViewRadius + 3;
					//myController.detect.fov.shaderMaterial = CommonObjectsStore.me.suspicious;
				}
			}
		} else if (myType == AIType.civilian) {
			
			if (myController.detect.fov.viewRadius <= myController.detect.fov.startingViewRadius) {
				myController.detect.fov.viewRadius = myController.detect.fov.startingViewRadius;
				//myController.detect.fov.shaderMaterial = CommonObjectsStore.me.passive;
			}
		}
	}
	public float slerpTimer = 0.0f;
	int lastCol = 0;
	void decideFOVColor()
	{
		//works now, will do a different color for civilians, maybe some other color for searches or something, also need to make it so that the timer is shorter if the NPC is aware of the player.
		Color c = myController.detect.fov.shaderMaterial.color;
			
		if (alarmed == true || myController.memory.seenSuspect == true || myController.memory.seenArmedSuspect == true || myController.memory.peopleThatHaveAttackedMe.Contains (CommonObjectsStore.player) || LevelController.me.suspects.Contains (CommonObjectsStore.player)) {
			myController.detect.fov.shaderMaterial.color = new Color (1.0f, 0.2f, 0.2f, 0.4f);
			if (myController.detect.fov.detectionLimitBase > 0.5f) {
				myController.detect.fov.detectionLimitBase = 0.5f;
			}
		} else {
			if (doWeChangeColor ()) {
				myController.detect.fov.shaderMaterial.color = colorSlerp (new Color (1.0f, 1.0f, 1.0f, 0.4f), new Color (1.0f, 0.2f, 0.2f, 0.4f), getPlayerValue ());
			}
		}
	}

	Color colorSlerp(Color c1,Color c2,float t)
	{
		//Vector3 s = new Vector3 (c1.r, c1.g, c1.b);
		//Vector3 f = new Vector3 (c2.r, c2.g, c2.b);
		//Vector3 col = Vector3.Slerp (s, f, t);
		//Debug.Log("Sun Color Slerp is " + col.ToString());
		return Color.Lerp(c1,c2,t);
	}

	public bool doWeChangeColor()
	{
		bool retVal = false;

		if (CommonObjectsStore.pwc.currentWeapon == null) {

		} else {
			if (CommonObjectsStore.pwc.currentWeapon.illigal == true) {
				retVal = true;
			}
		}

		if (retVal == true) {
			return retVal;
		}

		RoomScript r = LevelController.me.getRoomPosIsIn (CommonObjectsStore.player.transform.position);
		if (r == null) {

		} else if (r.traspassing == true) {
			retVal = true;
		}

		BuildingScript b = LevelController.me.getBuildingPosIsIn (CommonObjectsStore.player.transform.position);

		if (b == null) {

		} else if (b.traspassing == true) {
			retVal = true;
		}

		if (PlayerAction.currentAction == null) {

		} else if (PlayerAction.currentAction.illigal == true) {
			retVal = true;
		}

		return retVal;
	}

	float getPlayerValue()
	{
		return (1-( myController.detect.fov.detectionLimit-myController.detect.fov.playerViewTimer)/myController.detect.fov.detectionLimit);
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

//			//////Debug.Log ("Suspision of player = " + suspision);
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
					//			//////Debug.Log ("Suspision of player = " + suspision);
					if (suspision < 1.0f) {
						//everything good
						if (myType == AIType.guard) {
							if (npcRoom.traspassing == true) {
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

				//			//////Debug.Log ("Suspision of player = " + suspision);
				if (suspision < 1.0f) {
					//everything good
					if (myType == AIType.guard) {
						if (npcRoom==null || npcRoom.traspassing == true) {
							suspisious = true;
							myController.memory.objectThatMadeMeSuspisious = CommonObjectsStore.player;
						}
					}

				} else if (suspision < 7.0f) {
					if (myType == AIType.guard) {

						if (npcRoom==null || npcRoom.traspassing == true) {
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
					Debug.Log ("Set default alarmed");
					myController.memory.objectThatMadeMeSuspisious = CommonObjectsStore.player;

				}

				//////Debug.Log (this.gameObject.name + " is looking at the player " + suspision.ToString ());
					
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
						Debug.Log ("Set alarmed by NPC");
					}


				}
			} else if (t.gameObject.tag == "Player") {
				PersonWeaponController pwc = CommonObjectsStore.player.GetComponent<PersonWeaponController> ();

				if (pwc.currentWeapon == null) {

				} else {
					alarmed = true;
					Debug.Log ("Alarmed cause player had a weapon");
				}

				if (PlayerAction.currentAction == null) {
				} else if (PlayerAction.currentAction.illigal == true) {
					Debug.Log ("Civilian " + this.gameObject.name + " was alarmed by " + PlayerAction.currentAction.getType () + " the action was " + PlayerAction.currentAction.illigal);
					alarmed = true;
				}

			}
		}

		foreach (GameObject g in NPCManager.me.corpsesInWorld) { //should probbably find a way for corpses to be detected by the FOV
			if (g.tag != "Dead/Guarded") {
				if (Vector3.Distance(this.transform.position,g.transform.position)<myController.detect.fov.viewRadius) {
					if (myController.detect.isTargetInFrontOfUs (g) == true) {
						if (myController.detect.lineOfSightToTargetWithNoCollider (g) == true) {
							Debug.Log ("Set alarmed by corpse");
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
									////////Debug.Log ("Item " + i.itemName + " was detected");
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
		if (g.GetComponent<PersonWeaponController>()==false) {
			return 0.0f;
		}

		float retVal = 0.0f;
		Inventory i = g.GetComponent<Inventory> ();
		PersonWeaponController pwc = g.GetComponent<PersonWeaponController> ();

		if (pwc==null|| pwc.currentWeapon == null) {

		} else {
			return 999;//player is visibly armed
		}

		if (shouldWeAttackTarget (g)) {
			return 999;
		}

		if (g.tag == "Player") {
			if (canWeSeeIncapedPersonAndPlayer ()) {
				retVal += 10;
			}

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


			/*RoomScript r = LevelController.me.getRoomObjectIsIn (CommonObjectsStore.player);
			if (r == null) {
				//player probs outside
			} else {
				if (r.traspassing == true) {
					retVal += 5.0f;
				}
			}*/

			retVal += i.inventoryItems.Count/2;

			//if (i.getInventoryWeightSum () > i.inventoryCapacity - 5) {
			//	retVal += 1;
			//}

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
				if (r.traspassing == true) {
					retVal += 5.0f;
				}
			}

			if (i.inventoryItems.Count > 5) {
				retVal += 2;
			}

			if (i.getInventoryWeightSum () > i.inventoryCapacity - 5) {
				retVal += 1;
			}
		}
		Debug.Log ("Players suspicion was " + retVal);
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
					//////Debug.Log ("Wanting to investigate location 1");

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
						//////Debug.Log ("Wanting to investigate object");

						Destroy (myController.currentBehaviour);
						NPCBehaviour_InvestigateObject newBehaviour = this.gameObject.AddComponent<NPCBehaviour_InvestigateObject> ();
						newBehaviour.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
						myController.currentBehaviour = newBehaviour;
						//PhoneTab_RadioHack.me.setNewText ("Seen something odd, going to check it out",radioHackBand.buisness);

					}
				} else if (myController.memory.noiseToInvestigate != Vector3.zero) {
					//////Debug.Log ("Wanting to investigate location");

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
					//////Debug.Log ("Wanting to investigate Corpse");

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
							//////Debug.Log ("Wanting to search person " + myController.memory.objectThatMadeMeSuspisious.name);
							Destroy (myController.currentBehaviour);
							NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_SearchPerson> ();
							myController.currentBehaviour = nb;
							nb.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
							suspisiousOf.addToSearchedBy (this.gameObject);
							//PhoneTab_RadioHack.me.setNewText ("Got a suspicious person, going to search them.",radioHackBand.buisness);

						}
					} else {
						RoomScript npcRoom = LevelController.me.getRoomObjectIsIn (myController.memory.objectThatMadeMeSuspisious);

						if (npcRoom==null|| npcRoom.traspassing == true) {
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
						//////Debug.LogError("RAISE ALARM 1");
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
							//////Debug.LogError("RAISE ALARM 2");

							//myController.memory.noiseToInvestigate = Vector3.zero;
							//PhoneTab_RadioHack.me.setNewText ("I'm going to call the cops",radioHackBand.buisness);

						}
					}
				} else {
					////////Debug.LogError ("Post hostage 1");
					if (PlayerAction.currentAction == null) {
						if (myController.currentBehaviour==null || myController.currentBehaviour.myType != behaviourType.raiseAlarm) {
							Destroy (myController.currentBehaviour);
							NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_RaiseAlarm> ();
							myController.currentBehaviour = nb;
							////////Debug.LogError("RAISE ALARM 3");

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
						////////Debug.Log ("Updating suspects");
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
						////////Debug.Log (this.gameObject.name);
						//////Debug.Break ();
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
								//////Debug.Log ("Passive cop investigating location " + myController.memory.objectThatMadeMeSuspisious.name);
								Destroy (myController.currentBehaviour);
								NPCBehaviour newBehaviour = this.gameObject.AddComponent<NPCBehaviour_InvestigateObject> ();
								newBehaviour.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
								myController.currentBehaviour = newBehaviour;
								//PhoneTab_RadioHack.me.setNewText ("Potential suspect sighted, going to investigate",radioHackBand.cop);

							}
						} else {
							if (myController.currentBehaviour.myType != behaviourType.searchPerson) {
								//////Debug.Log ("Wanting to search person " + myController.memory.objectThatMadeMeSuspisious.name);
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
						//////Debug.Log ("Passive cop investigating location " + myController.memory.objectThatMadeMeSuspisious.name);
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
						//////Debug.Log ("Wanting to search person " + myController.memory.objectThatMadeMeSuspisious.name);
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
						//////Debug.Log ("Investigating location on Suspicious" );

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
							//////Debug.Log ("Wanting to investigate object");

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
									//////Debug.Log ("COP SEARCH ONE Wanting to search person " + myController.memory.objectThatMadeMeSuspisious.name);
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
										//////Debug.Log ("COP SEARCH TWO Wanting to search person " + myController.memory.objectThatMadeMeSuspisious.name);
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
							//////Debug.Log ("Guarding Corpse");

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
						//////Debug.Log ("Wanting to search person " + myController.memory.objectThatMadeMeSuspisious.name);
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
						//////Debug.Log ("Updating suspects");
						Destroy (myController.currentBehaviour);
						NPCBehaviour newBehaviour = this.gameObject.AddComponent<NPCBehaviour_UpdateSuspects> ();
						//newBehaviour.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
						myController.currentBehaviour = newBehaviour;
						//PhoneTab_RadioHack.me.setNewText ("Going to update suspects description, standby",radioHackBand.cop);

					}
				} else {
					if (myController.memory.noiseToInvestigate != Vector3.zero) {
						if (myController.currentBehaviour.myType != behaviourType.investigate) {
							//////Debug.Log ("Investigating location on alarmed");
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

	void setAllSwatToFormUp(){
		foreach (NPCController npc in NPCManager.me.npcControllers) {
			if (npc.npcB.myType == AIType.swat) {
				if (npc.currentBehaviour == null) {

				} else {
					if (npc.currentBehaviour.myType == behaviourType.formUp) {
							
					} else {
						Destroy (npc.currentBehaviour);
						NPCBehaviour nb = npc.gameObject.AddComponent<NPCBehaviour_SwatFormUp> ();
						npc.currentBehaviour = nb;
					}
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
					npc.memory.objectThatMadeMeSuspisious = myController.memory.objectThatMadeMeSuspisious;
					Destroy (npc.currentBehaviour);
					NPCBehaviour nb = g.gameObject.AddComponent<NPCBehaviour_SWATAttackTarget> ();
					npc.currentBehaviour = nb;
					nb.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
					nb.Initialise ();
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
				newBehaviour.Initialise ();
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
				newBehaviour.Initialise ();

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
			if (t == null) {
				continue;
			}


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
				//////Debug.Log ("Set all swat to attack");
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
						//////Debug.Log ("Investigating location on alarmed");
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
							//////Debug.Log ("Investigating location on alarmed");
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
				//////Debug.Log ("Civilian exiting level");
				NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_ExitLevel> ();
				myController.currentBehaviour = nb;
			}
		}


	}

	void civilianAlarmed()
	{
		if (myController.currentBehaviour == null) {
			NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_ExitLevel> ();
			myController.currentBehaviour = nb;
		}

		if (PoliceController.me.copsCalled==false) {
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
			////////Debug.Log ("Hostage release");
			if (myController.currentBehaviour.myType != behaviourType.attackTarget) {
				Destroy (myController.currentBehaviour);
				NPCBehaviour nb = this.gameObject.AddComponent<NPCBehaviour_AttackTarget> ();
				myController.currentBehaviour = nb;
				nb.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
				//PhoneTab_RadioHack.me.setNewText ("Target is hostile, taking him down.",radioHackBand.cop);
				////////Debug.Log("Adding attack target");
			}
			return;
			//alarmed = true;
			//////Debug.Break();
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
				////////Debug.Log("Adding attack target");
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


public enum whatAiIsDoing
{
	starting,
	attacking,
	searching,
	searchingPerson,
	confiscating,
	investigatingLocation,
	raisingAlarm,
	searchingRooms,
	freeingHostage,
	guardingCorpse,
	arresting,
	guardingEntrance,
	escortingTraspass,
	swatFormUp,
	swatRaidBuilding,
	swatAttack,
	evacuate
}


