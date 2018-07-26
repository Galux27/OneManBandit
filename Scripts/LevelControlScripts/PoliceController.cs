using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceController : MonoBehaviour {
	public static PoliceController me;

	public bool knowArmed, knowSuspect, knowHostage, knowDead,copsCalled,backupCalled,swatCalled,copsHere,backupHere,swatHere,copsLeave;
	public float policeTimer=60.0f,policeBackup =60.0f,policeSearchArea = 120.0f,swatTimer=60.0f,backupNoResponseTimer=60.0f;

	public int timesPoliceCalled = 0;
	public List<BuildingScript> buildingsCalledFrom;
	public List<GameObject> copsInLevel;
	public List<GameObject> swatInLevel;

	public BuildingScript buildingUnderSiege;
	public bool underSiege = false;

	public List<Transform> pointsToSecure;

	public bool buildingSurrounded = false, actionsDisabled = false;
	public Transform swatFormUpPoint,evacPoint;

	public List<GameObject> copCarsAvailable,copCarsInWorld;

	void Awake()
	{
		me = this;
	}

	// Use this for initialization
	void Start () {
		if (buildingsCalledFrom == null) {
			buildingsCalledFrom = new List<BuildingScript> ();
		}

		if (copsInLevel == null) {
			copsInLevel = new List<GameObject> ();
		}

		if (swatInLevel == null) {
			swatInLevel = new List<GameObject> ();
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.P)) {
			Debug.Log("Path cost test " + Pathfinding.me.getPathCost(CommonObjectsStore.player,swatFormUpPoint.gameObject));
		}

		if (swatCalled == true && swatHere==false) {
			countDownSwat ();
		}

		if (copsCalled == true && copsHere == false) {
			countDownCops ();
		}

		if (backupCalled == true && backupHere == false) {
			countDownBackup ();
		}

		//if (copsHere == true && backupCalled == false && swatCalled == false) {
		//	countDownInvestigation ();
		//}

		if (swatHere == true && buildingSurrounded == false) {
			if (buildingUnderSiege == null) {
				buildingSurrounded = true;
			} else {
				setBuildingSurrounded ();


			}

			if (actionsDisabled == false) {
				disableActionsInBuildingUnderSiege ();
			}
		}

		if (swatHere == true) {
			shouldWeRefreshSWAT ();
		}

		if (swatHere == true && buildingSurrounded == true || swatHere==true && buildingUnderSiege==null) {
			//swatManager ();
			newSwatController();
		}
	}

	void countDownInvestigation()
	{
		policeSearchArea -= Time.deltaTime;
		if (policeSearchArea <= 0) {
			if (shouldBackupBeCalled () == true) {
				backupCalled = true;
			} else {
				copsCalled = false;
				copsHere = false;
				copsLeave = true;
			}
		}
	}

	bool shouldBackupBeCalled()
	{
		foreach (GameObject c in copsInLevel) {
			if (c.gameObject.tag == "Dead/Knocked") {
				return true;
			}
		}
		return false;
	}

	public void callPolice(GameObject calling){
		NPCController npc = calling.GetComponent<NPCController> ();

		if (npc.npcB.myType == AIType.cop || npc.npcB.myType == AIType.guard) {
			if (npc.memory.seenCorpse == true) {
				knowDead = true;
			}

			if (npc.memory.seenHostage == true) {
				knowHostage = true;
			}

			if (npc.memory.seenSuspect == true) {
				knowSuspect = true;
			}

			if (npc.memory.seenArmedSuspect == true) {
				knowArmed = true;
			}
		}

		if (npc.npcB.myType == AIType.cop) {
			if (copsHere == true) {
				backupCalled = true;
			}
			else if (backupHere == true) {
				swatCalled = true;
				setSiege ();

			}
		}

		foreach (BuildingScript b in LevelController.me.buildings) {
			if (b.isObjectInRoom (calling) == true) {
				buildingsCalledFrom.Add (b);
			}
		}
		timesPoliceCalled += 1;
		decideLevelOfPolice ();
	}

	void countDownCops()
	{
		policeTimer -= Time.deltaTime;
		if (policeTimer <= 0) {
			spawnInitialCops ();
		}
	}


	public void countDownBackup()
	{
		policeBackup -= Time.deltaTime;
		if (policeBackup <= 0) {
			spawnBackup ();
		}
	}

	public void countDownSwat()
	{
		swatTimer -= Time.deltaTime;
		if (swatTimer <= 0) {
			spawnSwat ();
		}
	}

	public void spawnInitialCops()
	{
		_spawnCops ();

	}

	public void spawnBackup()
	{
		_spawnBackup ();
	}

	public void spawnSwat()
	{
		_spawnSwat ();
	}

	void decideLevelOfPolice()
	{
		int level = 0;

		if (knowSuspect == true) {
			level += 2;
		}

		if (knowDead == true) {
			level += 4;
		}

		if (knowHostage == true) {
			level += 3;
		}

		if (knowArmed == true) {
			level += 10;
		}

		level += timesPoliceCalled;

		if (level <= 5) {
			copsCalled = true;
		} else if (level <= 10) {
			copsCalled = true;
			backupCalled = true;
		} else {
			copsCalled = true;
			backupCalled = true;
			swatCalled = true;
			setSiege ();
		}
	}

	void setSiege(){
		if (buildingsCalledFrom == null || buildingsCalledFrom.Count==0) {
			buildingUnderSiege = BuildingScript.outdoors;
		} else {
			buildingUnderSiege = buildingsCalledFrom [0];
			buildingUnderSiege.setBuildingUnderSiege ();
		}
		underSiege = true;
		getPointsToSecure ();
	}

	void getPointsToSecure()
	{


		if (buildingUnderSiege==null || buildingUnderSiege.isOutdoors==true) {
			Debug.Log ("Points set for outdoors");
			CivilianAction[] cas = FindObjectsOfType<CivilianAction> ();
			foreach (CivilianAction p in cas) {
				Transform t = p.positionForAction;
				WorldTile nearestPoint = WorldBuilder.me.findNearestWorldTile (t.position);
				/*bool pointAdded = false;
				for (int x = nearestPoint.gridX - 5; x < nearestPoint.gridX + 5; x++) {
					for (int y = nearestPoint.gridY - 5; y < nearestPoint.gridY + 5; y++) {
						WorldTile wt;

						wt = WorldBuilder.me.worldTiles [x, y].GetComponent<WorldTile> ();
						if (wt == null) {
						} else {
							if (pointAdded==false && Vector3.Distance (wt.transform.position, t.position) > 3 && wt.walkable == true) {
								pointsToSecure.Add (wt.transform);
								pointAdded=true;
							}
						}

					}
				}*/

				//if (pointAdded == false) {
					pointsToSecure.Add (t);
				//}
			}



		} else {
			pointsToSecure = new List<Transform> ();
			Debug.Log ("Points set for indoors");

			foreach (Transform t in buildingUnderSiege.entrances) {
				WorldTile nearestPoint = WorldBuilder.me.findNearestWorldTile (t.position);
				for (int x = nearestPoint.gridX - 5; x < nearestPoint.gridX + 5; x++) {
					for (int y = nearestPoint.gridY - 5; y < nearestPoint.gridY + 5; y++) {
						WorldTile wt;



						wt = WorldBuilder.me.worldTiles [x, y].GetComponent<WorldTile> ();
						if (wt == null) {
						} else {
							if (buildingUnderSiege.isPosInRoom (wt.transform.position) == false && Vector3.Distance (wt.transform.position, t.position) > 3 && wt.walkable == true) {
								pointsToSecure.Add (wt.transform);
							}
						}

					}
				}
			}
		}
	}

	void _spawnCops()
	{

		if (copCarsAvailable.Count <= 0) {
			return;
		}
		copsHere = true;

		/*for (int x = 0; x < 4; x++) {
			if (NPCManager.me.patrolCops.Count > 0) {
				GameObject cop = NPCManager.me.patrolCops [0];
				NPCManager.me.patrolCops.RemoveAt (0);
				cop.transform.position = LevelController.me.levelEntry.transform.position;
				cop.SetActive (true);
			} else {
				GameObject cop = (GameObject)Instantiate (CommonObjectsStore.me.cop, LevelController.me.levelEntry.transform.position, Quaternion.Euler (0, 0, 0));
				copsInLevel.Add (cop);
			}
		}*/
		CarSpawner.me.destroyCars ();
		int copsSpawned = 0;
		while (copsSpawned < 4) {
			GameObject policeCar = copCarsAvailable [Random.Range (0, copCarsAvailable.Count)];
			addCopCar (policeCar);
			policeCar.SetActive (true);
			copCarsAvailable.Remove (policeCar);
			copsSpawned=4;
		}

	}

	void _spawnBackup()
	{
		if (copCarsAvailable.Count <= 0) {
			return;
		}
		CarSpawner.me.destroyCars ();

		backupHere = true;
		int copsSpawned = 0;
		while (copsSpawned < 12 ) {
			GameObject policeCar = copCarsAvailable [Random.Range (0, copCarsAvailable.Count)];
			policeCar.SetActive (true);
			addCopCar (policeCar);

			copCarsAvailable.Remove (policeCar);
			copsSpawned+=4;
		}
		/*for (int x = 0; x < 12; x++) {
			if (NPCManager.me.patrolCops.Count > 0) {
				GameObject cop = NPCManager.me.patrolCops [0];
				NPCManager.me.patrolCops.RemoveAt (0);
				cop.transform.position = LevelController.me.levelEntry.transform.position;
				cop.SetActive (true);
				copsInLevel.Add (cop);

			} else {
				GameObject cop = (GameObject)Instantiate (CommonObjectsStore.me.cop, LevelController.me.levelEntry.transform.position, Quaternion.Euler (0, 0, 0));
				copsInLevel.Add (cop);
			}
		}*/
	}

	void _spawnSwat()
	{
		if (copCarsAvailable.Count <= 0) {
			return;
		}
		swatHere = true;
		CarSpawner.me.destroyCars ();

		int copsSpawned = 0;
		while (copsSpawned < 6) {
			GameObject policeCar = copCarsAvailable [Random.Range (0, copCarsAvailable.Count)];
			PoliceController.me.swatFormUpPoint = policeCar.GetComponent<PoliceCarScript> ().copSpawnPoints [0];
			policeCar.GetComponent<PoliceCarScript> ().swat = true;
			policeCar.SetActive (true);
			addCopCar (policeCar);

			copCarsAvailable.Remove (policeCar);
			copsSpawned+=4;
		}

		/*for (int x = 0; x < 6; x++) {
			if (NPCManager.me.swat.Count > 0) {
				GameObject cop = NPCManager.me.swat [0];
				NPCManager.me.swat.RemoveAt (0);
				cop.transform.position = LevelController.me.levelEntry.transform.position;
				cop.SetActive (true);
				swatInLevel.Add (cop);

			} else {
				GameObject swat = (GameObject)Instantiate (CommonObjectsStore.me.swat, LevelController.me.transform.position, Quaternion.Euler (0, 0, 0));
				swatInLevel.Add (swat);
			}
		}*/
	}

	float buildingSurroundTimer = 40.0f;
	void setBuildingSurrounded()
	{
		buildingSurroundTimer -= Time.deltaTime;
		if (buildingSurroundTimer <= 0) {
			buildingUnderSiege.setBuildingWeightsToNormal ();
			buildingSurrounded = true;
		}
	}


	public Transform getPointToGuard()
	{
		if (pointsToSecure == null || pointsToSecure.Count==0) {
			getPointsToSecure ();
		}
		Transform t = pointsToSecure [Random.Range (0, pointsToSecure.Count)];
		pointsToSecure.Remove (t);
		return t;
	}


	public RoomScript currentRoomSwat;
	public List<GameObject> pointsToGoTo,pointsBeenTo;
	public GameObject currentPoint,swatGoingToCurrentPoint;
	public List<GameObject> swatGoingIn;
	List<NPCBehaviour_SwatGoToRoomEntrance> goToEntrance;
	List<NPCBehaviour_SWATAttackTarget> attacking;
	public bool goingToRoom=true,stormingRoom=false,setSwatToAttack=false;
	NPCBehaviour_SwatGoToPoint nb;

	public List<RoomScript> roomsToGoTo,roomsGoingTo;
	public List<int> lengthsFromEntrance;

	public bool investigatePoint = false,seenHostile=false;
	public Vector3 pointToInvestigate = Vector3.zero;
	public GameObject swatInvestigating,swatTarget;
	public NPCBehaviour_SwatInvestigatePoint investigate;
	public RoomScript lastRoom;
	public float swatLoseTargetTimer = 10.0f;
	public void setNoiseHeard(Vector3 pos,float range)
	{
		if (seenHostile == true || swatHere==false) {
			return;
		}

		foreach (GameObject g in swatInLevel) {
			float d = Vector3.Distance (pos, g.transform.position);
			if (d < range) {
				pointToInvestigate = pos;
				if (swatGoingToCurrentPoint == null) {
					swatInvestigating = swatInLevel [0];
				} else {
					swatInvestigating = swatGoingToCurrentPoint;
				}

				NPCController npc = swatInvestigating.GetComponent<NPCController> ();
				if (npc.currentBehaviour == null) {

				} else {
					Destroy (npc.currentBehaviour);
				}

				investigate = swatInvestigating.AddComponent<NPCBehaviour_SwatInvestigatePoint> ();
				npc.currentBehaviour = investigate;
				investigatePoint = true;

				break;
			}
		}
	}

	public void getRoomsToGoTo()
	{
		
		lengthsFromEntrance = new List<int> ();
		List<RoomScript> inOrder = new List<RoomScript> ();
		List<RoomScript> temp = new List<RoomScript> ();

		foreach (RoomScript r in LevelController.me.roomsInLevel) {
			if (buildingUnderSiege.myRooms.Contains(r)) {
				temp.Add (r);
			}
			//lengthsFromEntrance.Add(Pathfinding.me.getPathCost(r.entrances[0].gameObject,swatFormUpPoint.gameObject));
		}

		bool finishedSort = false;
		RoomScript addThisTime = null;
		int dist = 99999999;

		while (finishedSort == false) {
			foreach (RoomScript r in temp) {
				int d = r.distFromSwatFormUp;
				if (d < dist) {
					dist = d;
					addThisTime = r;
				}
			}
			inOrder.Add (addThisTime);
			dist = 99999999;
			temp.Remove (addThisTime);
			if (temp.Count == 0) {
				finishedSort = true;
			}
		}

		roomsToGoTo = inOrder;
		roomsGoingTo = inOrder;
	}


	void setPointsToGoTo(){
		pointsToGoTo = currentRoomSwat.nodesForSwat;
		sortNodeByDistFromEntrance ();
	}

	void sortNodeByDistFromEntrance(){
		List<GameObject> inOrder = new List<GameObject> ();
		List<GameObject> temp = pointsToGoTo;
		bool finishedSort = false;
		float dist = 99999999.0f;
		GameObject addThisTime = null;

		while (finishedSort == false) {
			foreach (GameObject g in temp) {
				float d = Vector3.Distance (g.transform.position, currentRoomSwat.entrances [0].transform.position);
				if (d < dist) {
					dist = d;
					addThisTime = g;
				}
			}
			inOrder.Add (addThisTime);
			dist = 9999999.0f;
			temp.Remove (addThisTime);

			if (temp.Count == 0) {
				pointsToGoTo = inOrder;
				finishedSort = true;
			}
		}
	}

	GameObject getFreeSwatNearPoint(Vector3 pos)
	{
		float distance = 999999.0f;
		GameObject retVal = null;
		foreach (GameObject g in swatInLevel) {
			if (swatGoingIn.Contains (g) == false) {
				float d = Vector3.Distance (this.transform.position, pos);
				if (d < distance) {
					retVal = g;
					distance = d;
				}
			}
		}
		return retVal;
	}
	GameObject swatCivilPoint;
	bool setCivilPoints=false;
	public void swatManager() //TODO need to find a way to sort the points to secure by the distance from the entrance
	{
		if (buildingUnderSiege == null) {

		} else {
			if (roomsToGoTo == null || roomsToGoTo.Count == 0) {
				getRoomsToGoTo ();
			}
		}
		if (seenHostile == false) {
			swatHostileMoniter ();
		}

		if (investigatePoint == false && seenHostile == false) {
			if (buildingUnderSiege == null) {
				if (setCivilPoints == false) {
					swatCivilPoint = LevelController.me.getCivilianAction ().gameObject;
					foreach (GameObject g in swatInLevel) {
						NPCController npcc = g.GetComponent<NPCController> ();
						if (npcc.inv.doWeHaveAWeaponWeCanUse () == true) {
							Weapon w = npcc.inv.getWeaponWeCanUse ();
							w.equipItem ();
						}
						if (npcc.currentBehaviour == null) {

						} else {
							Destroy (npcc.currentBehaviour);
						}
						NPCBehaviour_SwatGoToPoint beh = g.AddComponent<NPCBehaviour_SwatGoToPoint> ();
						beh.point = swatCivilPoint;
						npcc.currentBehaviour = beh;
					}
					setCivilPoints = true;
				}

				foreach (GameObject g in swatInLevel) {
					NPCController npcc = g.GetComponent<NPCController> ();
					if (npcc.GetComponent<NPCBehaviour_SwatGoToPoint> () == false) {
						if (npcc.inv.doWeHaveAWeaponWeCanUse () == true) {
							Weapon w = npcc.inv.getWeaponWeCanUse ();
							w.equipItem ();
						}
						if (npcc.currentBehaviour == null) {

						} else {
							Destroy (npcc.currentBehaviour);
						}
						NPCBehaviour_SwatGoToPoint beh = g.AddComponent<NPCBehaviour_SwatGoToPoint> ();
						beh.point = swatCivilPoint;
						npcc.currentBehaviour = beh;
					}
				}

				bool shouldWeReset = true;
				foreach (GameObject g in swatInLevel) {
					if (Vector2.Distance (g.transform.position, swatCivilPoint.transform.position) > 5.0f) {
						shouldWeReset = false;
					}
				}

				if (shouldWeReset == true) {
					setCivilPoints = false;
				}
			}else{
				if (currentRoomSwat == null) {
					swatGoingIn = new List<GameObject> ();
					goToEntrance = new List<NPCBehaviour_SwatGoToRoomEntrance> ();

					if (roomsToGoTo == null || roomsToGoTo.Count == 0) {
						getRoomsToGoTo ();
					}

					if (lastRoom == null) {
						currentRoomSwat = roomsGoingTo [0];
					} else {
						currentRoomSwat = lastRoom;
						lastRoom = null;
					}
					roomsGoingTo.RemoveAt (0);
					goingToRoom = true;

					if (roomsGoingTo.Count == 0) {
						roomsGoingTo = roomsToGoTo;
					}

					setPointsToGoTo ();
				}

				if (goingToRoom == false) { //add a method to get the nearest swat to the point?
					if (nb == null) {
						currentPoint = pointsToGoTo [0];
						if (swatGoingToCurrentPoint == null || swatGoingIn.Count == 0) {
							swatGoingToCurrentPoint = getFreeSwatNearPoint (currentPoint.transform.position);
							swatGoingIn.Add (swatGoingToCurrentPoint);

						} else {
							foreach (GameObject g in swatInLevel) {
								if (swatGoingIn.Contains (g) == false) {//TODO add checks for if they are dead/going for target etc...
									swatGoingToCurrentPoint = g;
									swatGoingIn.Add (g);

									break;
								}
							}
						}
						if (swatGoingIn.Count == swatInLevel.Count) {
							swatGoingIn.Clear ();
						}

						if (swatGoingToCurrentPoint == null) {
							
						} else {

							NPCController npc = swatGoingToCurrentPoint.GetComponent<NPCController> ();
							if (npc.currentBehaviour == null) {

							} else {
								Destroy (npc.currentBehaviour);
							}
							nb = swatGoingToCurrentPoint.AddComponent<NPCBehaviour_SwatGoToPoint> ();
							npc.currentBehaviour = nb;
						}
					} else if (nb.areWeAtPosition () == true) {
						
						pointsBeenTo.Add (pointsToGoTo [0]);
						pointsToGoTo.RemoveAt (0);
						if (pointsToGoTo.Count == 0) {
							PhoneTab_RadioHack.me.setNewText (currentRoomSwat.roomName + "Has been secured, moving to " + roomsGoingTo [0].roomName, radioHackBand.swat);

							currentRoomSwat = null;
							pointsBeenTo.Clear ();
							swatGoingIn.Clear ();
							goToEntrance.Clear ();
						}
						nb = null;
					} else {
						if (goToEntrance == null || goToEntrance.Count == 0) {
							foreach (GameObject g in swatInLevel) {
								NPCController n = g.GetComponent<NPCController> ();
								if (n.currentBehaviour == null) {

								} else {
									Destroy (n.currentBehaviour);
								}

								NPCBehaviour_SwatGoToRoomEntrance nb = n.gameObject.AddComponent<NPCBehaviour_SwatGoToRoomEntrance> ();
								n.currentBehaviour = nb;
								goToEntrance.Add (nb);
							}
						}

						if (areSwatAtEntrance () == true) {
							PhoneTab_RadioHack.me.setNewText ("SWAT Team secutring " + currentRoomSwat.roomName,radioHackBand.swat);

							goingToRoom = false;
						}
					}
				}
			} 
		} else if (seenHostile == true) {
			if (setSwatToAttack == false) {
				PhoneTab_RadioHack.me.setNewText ("Target spotted, moving to engage.",radioHackBand.swat);

				attacking = new List<NPCBehaviour_SWATAttackTarget> ();
				foreach (GameObject g in swatInLevel) {
					NPCController npc = g.GetComponent<NPCController> ();
					if (npc.currentBehaviour == null) {
						
					} else {
						Destroy (npc.currentBehaviour);
					}
					NPCBehaviour_SWATAttackTarget nb = g.AddComponent<NPCBehaviour_SWATAttackTarget> ();
					npc.currentBehaviour = nb;
					attacking.Add (nb);
				}
				setSwatToAttack = true;
			} else {
				//add some condition to  check if they have lost the target
				if (doSwatStillHaveTarget () == true) {
						lastRoom = LevelController.me.roomPlayerIsIn;
				} else {
					swatLoseTargetTimer -= Time.deltaTime;
					if (swatLoseTargetTimer <= 0) {
						if (lastRoom == null) {

						} else {
							PhoneTab_RadioHack.me.setNewText ("Target lost, moving to " + lastRoom.roomName, radioHackBand.swat);
						}
						seenHostile = false;
						investigatePoint = false;
						currentRoomSwat = null;
						goingToRoom = true;
						swatLoseTargetTimer = 10.0f;
						setSwatToAttack = false;
					}


				}
			}

		} else if (investigatePoint == true) {
			if (investigate.areWeAtPosition () == false) {
				swatTarget = investigate.haveWeSeenSuspiciousObject ();
				if (swatTarget == null) {
					seenHostile = false;
				} else {
					seenHostile = true;
				}
			} else {
				if (seenHostile == false) {

					NPCController npc = swatInvestigating.GetComponent < NPCController> ();

					if (npc.currentBehaviour == null) {

					} else {
						Destroy (npc.currentBehaviour);
					}

					NPCBehaviour nb = swatGoingToCurrentPoint.AddComponent<NPCBehaviour_SwatGoToPoint> ();
					npc.currentBehaviour = nb;

				}
			}
		}
	}

	void swatBugFix()
	{
		foreach (GameObject g in swatInLevel) {
			if(buildingSurrounded==true && g.GetComponent<NPCBehaviour_SwatFormUp>()==true)
			{

			}
		}
	}

	void swatHostileMoniter()
	{
		foreach (GameObject g in swatInLevel) {
			NPCController npc = g.GetComponent<NPCController> ();
			GameObject target = npc.npcB.isHostileTargetNearby();
			if(target==null)
			{

			}
			else{
				npc.memory.objectThatMadeMeSuspisious = target;
				PoliceController.me.swatTarget = target;
				seenHostile = true;
			}
		}
	}


	bool doSwatStillHaveTarget()
	{
		foreach (NPCBehaviour_SWATAttackTarget s in attacking) {
			if (s.canWeStillSeeTarget () == true) {
				return true;
			}

		}
		return false;
	}

	bool areSwatAtEntrance()
	{
		foreach (NPCBehaviour_SwatGoToRoomEntrance nb in goToEntrance) {
			if (nb.areWeAtPosition () == false) {
				return false;
			}
		}
		return true;
	}

	void shouldWeRefreshSWAT()
	{
		bool refresh = false;

		if (swatInLevel == null) {
			return;
		}

		foreach (GameObject g in swatInLevel) {
			NPCController npc = g.GetComponent<NPCController> ();

			if (npc == null) {
				continue;
			}

			if (npc.npcB.myType == AIType.swat) {
				Debug.Log (npc.gameObject.name+ " Fount swat with " + npc.myHealth.healthValue);

				if (npc.gameObject.tag == "Dead/Knocked" && npc.myHealth.healthValue <=0 ) {
					refresh = true;
				}
			}
		}

		if (refresh == true) {
			Debug.Log ("Refreshing swat");
			List<NPCController> swatAvailable = new List<NPCController> ();


			foreach (GameObject g in swatInLevel) {
				NPCController npc = g.GetComponent<NPCController> ();
				if (npc.npcB.myType == AIType.swat) {
					if (npc.myHealth.healthValue > 0) {
						swatAvailable.Add (npc);
					}
				}
			}
			if(buildingSurrounded==true)
			{
				if (investigatePoint == true) {
					NPCController investigatingController = swatInvestigating.GetComponent<NPCController> ();
					if (swatAvailable.Contains (investigatingController) == false) {
						swatInvestigating = swatAvailable [0].gameObject;
						NPCController npc = swatInvestigating.GetComponent<NPCController> ();
						if (npc.currentBehaviour == null) {

						} else {
							Destroy (npc.currentBehaviour);
						}

						investigate = swatInvestigating.AddComponent<NPCBehaviour_SwatInvestigatePoint> ();
						npc.currentBehaviour = investigate;
						investigatePoint = true;
					}
				} else if (investigatePoint == false && seenHostile == false) {
					if (goingToRoom == true) {
						goToEntrance = new List<NPCBehaviour_SwatGoToRoomEntrance> ();

						foreach (NPCController n in swatAvailable) {
							if (n.currentBehaviour == null) {

							} else {
								Destroy (n.currentBehaviour);
							}

							NPCBehaviour_SwatGoToRoomEntrance nb = n.gameObject.AddComponent<NPCBehaviour_SwatGoToRoomEntrance> ();
							n.currentBehaviour = nb;
							goToEntrance.Add (nb);
						}
					} else {
						NPCController investigatingController = swatGoingToCurrentPoint.GetComponent<NPCController> ();
						if (swatAvailable.Contains (investigatingController) == false) {
							swatGoingToCurrentPoint = swatAvailable [0].gameObject;

							NPCController npc = swatGoingToCurrentPoint.GetComponent<NPCController> ();
							if (npc.currentBehaviour == null) {

							} else {
								Destroy (npc.currentBehaviour);
							}
							nb = swatGoingToCurrentPoint.AddComponent<NPCBehaviour_SwatGoToPoint> ();
							npc.currentBehaviour = nb;
						}
					}
				}
			}
			swatInLevel.Clear ();
			foreach (NPCController n in swatAvailable) {
				swatInLevel.Add (n.gameObject);
			}

		}
	}

	void disableActionsInBuildingUnderSiege()
	{
		CivilianAction[] actions = FindObjectsOfType<CivilianAction> ();
		List<CivilianAction> newActionsAvailable = new List<CivilianAction> ();

		foreach (CivilianAction ca in actions) {
			if (buildingUnderSiege == null) {
				newActionsAvailable.Add (ca);

				continue;
			}


			if (buildingUnderSiege.isPosInRoom (ca.transform.position) == true) {
				ca.actionAvailable = false;
			} else {
				newActionsAvailable.Add (ca);
			}
		}
		LevelController.me.actionsInWorld = newActionsAvailable;
		if (CivilianController.me.civilianLimit > 2) {
			CivilianController.me.civilianLimit /= 2;
		}
		actionsDisabled = true;
	}


	bool swatFormedUp=false;
	bool pointsAssigned=false;
	void newSwatController()
	{
		if (buildingUnderSiege == null) {
			if (areWeFormedUp () == true) {

				if (investigatePoint == false && seenHostile == false) {
					if (setCivilPoints == false) {
						swatCivilPoint = LevelController.me.getCivilianAction ().gameObject;
						foreach (GameObject g in swatInLevel) {
							NPCController npcc = g.GetComponent<NPCController> ();
							if (npcc.inv.doWeHaveAWeaponWeCanUse () == true) {
								Weapon w = npcc.inv.getWeaponWeCanUse ();
								w.equipItem ();
							}
							if (npcc.currentBehaviour == null) {

							} else {
								Destroy (npcc.currentBehaviour);
							}
							NPCBehaviour_SwatGoToPoint beh = g.AddComponent<NPCBehaviour_SwatGoToPoint> ();
							beh.point = swatCivilPoint;
							npcc.currentBehaviour = beh;
						}
						setCivilPoints = true;
					}

					foreach (GameObject g in swatInLevel) {
						NPCController npcc = g.GetComponent<NPCController> ();
						if (npcc.GetComponent<NPCBehaviour_SwatGoToPoint> () == false) {
							if (npcc.inv.doWeHaveAWeaponWeCanUse () == true) {
								Weapon w = npcc.inv.getWeaponWeCanUse ();
								w.equipItem ();
							}
							if (npcc.currentBehaviour == null) {

							} else {
								Destroy (npcc.currentBehaviour);
							}
							NPCBehaviour_SwatGoToPoint beh = g.AddComponent<NPCBehaviour_SwatGoToPoint> ();
							beh.point = swatCivilPoint;
							npcc.currentBehaviour = beh;
						}
					}

					bool shouldWeReset = true;
					foreach (GameObject g in swatInLevel) {
						if (Vector2.Distance (g.transform.position, swatCivilPoint.transform.position) > 5.0f) {
							shouldWeReset = false;
						}
					}

					if (shouldWeReset == true) {
						setCivilPoints = false;
					}
				}else if (seenHostile == true) {
					if (setSwatToAttack == false) {
						PhoneTab_RadioHack.me.setNewText ("Target spotted, moving to engage.",radioHackBand.swat);

						attacking = new List<NPCBehaviour_SWATAttackTarget> ();
						foreach (GameObject g in swatInLevel) {
							NPCController npc = g.GetComponent<NPCController> ();
							if (npc.currentBehaviour == null) {

							} else {
								Destroy (npc.currentBehaviour);
							}
							NPCBehaviour_SWATAttackTarget nb = g.AddComponent<NPCBehaviour_SWATAttackTarget> ();
							npc.currentBehaviour = nb;
							attacking.Add (nb);
						}
						setSwatToAttack = true;
					} else {
						//add some condition to  check if they have lost the target
						if (doSwatStillHaveTarget () == true) {
							lastRoom = LevelController.me.roomPlayerIsIn;
						} else {
							swatLoseTargetTimer -= Time.deltaTime;
							if (swatLoseTargetTimer <= 0) {
								if (lastRoom == null) {

								} else {
									PhoneTab_RadioHack.me.setNewText ("Target lost, moving to " + lastRoom.roomName, radioHackBand.swat);
								}
								seenHostile = false;
								investigatePoint = false;
								currentRoomSwat = null;
								goingToRoom = true;
								swatLoseTargetTimer = 10.0f;
								setSwatToAttack = false;
							}


						}
					}

				} else if (investigatePoint == true) {
					if (investigate.areWeAtPosition () == false) {
						swatTarget = investigate.haveWeSeenSuspiciousObject ();
						if (swatTarget == null) {
							seenHostile = false;
						} else {
							seenHostile = true;
						}
					} else {
						if (seenHostile == false) {

							NPCController npc = swatInvestigating.GetComponent < NPCController> ();

							if (npc.currentBehaviour == null) {

							} else {
								Destroy (npc.currentBehaviour);
							}

							NPCBehaviour nb = swatGoingToCurrentPoint.AddComponent<NPCBehaviour_SwatGoToPoint> ();
							npc.currentBehaviour = nb;

						}
					}
				}

				//make the swat move to the entrance of the room

				//if(swat at entrance to room)
				//make swat go to the 
			} else {
				//check for form up stuff
				foreach (GameObject g in swatInLevel) {
					NPCController npc = g.GetComponent<NPCController> ();

					if (npc.currentBehaviour==null || npc.currentBehaviour.myType != behaviourType.formUp) {
						if (npc.currentBehaviour == null) {

						} else {
							Destroy (npc.currentBehaviour);
						}

						NPCBehaviour_SwatFormUp newB = g.AddComponent<NPCBehaviour_SwatFormUp> ();
						npc.currentBehaviour = newB;
					} 
				}
			}
		} else {
			if (buildingSurrounded == true && areWeFormedUp () == true && areAllSwatSpawned()==true) {
				Debug.Log ("Swat surrounded building and are formed up");
				if (investigatePoint == false && seenHostile == false) {

					if (roomsToGoTo == null || roomsToGoTo.Count == 0) {
						getRoomsToGoTo ();
					}

					if (roomsGoingTo == null || roomsGoingTo.Count == 0) {
						roomsGoingTo = roomsToGoTo;
					}

					//if (currentRoomSwat == null) {
					//	currentRoomSwat = roomsGoingTo [0];
					//	roomsGoingTo.Remove (lastRoom);
					//}

					if (currentRoomSwat == null) {
						swatGoingIn = new List<GameObject> ();
						goToEntrance = new List<NPCBehaviour_SwatGoToRoomEntrance> ();



						if (lastRoom == null) {
							currentRoomSwat = roomsGoingTo [0];
							if (currentRoomSwat.nodesForSwat == null || currentRoomSwat.nodesForSwat.Count == 0) {
								currentRoomSwat.setPointsToGoTo ();
							}
						} else {
							currentRoomSwat = lastRoom;
							lastRoom = null;
							if (currentRoomSwat.nodesForSwat == null || currentRoomSwat.nodesForSwat.Count == 0) {
								currentRoomSwat.setPointsToGoTo ();
							}
						}
						roomsGoingTo.RemoveAt (0);
						goingToRoom = true;

						if (roomsGoingTo.Count == 0) {
							roomsGoingTo = roomsToGoTo;
						}

						setPointsToGoTo ();
					}

					if (pointsToSecure == null || pointsToSecure.Count == 0) {
						getPointsToSecure ();
					}

					if (pointsToGoTo==null || pointsToGoTo.Count==0) {
						setPointsToGoTo ();
					}


						if (nb == null && goingToRoom==false) {
						Debug.Log ("Not going to room");
							currentPoint = pointsToGoTo [0];
							if (swatGoingToCurrentPoint == null || swatGoingIn.Count == 0) {
								swatGoingToCurrentPoint = getFreeSwatNearPoint (currentPoint.transform.position);
								swatGoingIn.Add (swatGoingToCurrentPoint);

							} else {
								foreach (GameObject g in swatInLevel) {
									if (swatGoingIn.Contains (g) == false) {//TODO add checks for if they are dead/going for target etc...
										swatGoingToCurrentPoint = g;
										swatGoingIn.Add (g);

										break;
									}
								}
							}
							if (swatGoingIn.Count == swatInLevel.Count) {
								swatGoingIn.Clear ();
							}

							if (swatGoingToCurrentPoint == null) {

							} else {

								NPCController npc = swatGoingToCurrentPoint.GetComponent<NPCController> ();
								if (npc.currentBehaviour == null) {

								} else {
									Destroy (npc.currentBehaviour);
								}
								nb = swatGoingToCurrentPoint.AddComponent<NPCBehaviour_SwatGoToPoint> ();
								npc.currentBehaviour = nb;
							}
						} else if(nb == null && goingToRoom==true){
						Debug.Log ("Going to room");
							if (goToEntrance == null || goToEntrance.Count == 0) {
								goToEntrance = new List<NPCBehaviour_SwatGoToRoomEntrance> ();
								foreach (GameObject g in swatInLevel) {
									NPCController n = g.GetComponent<NPCController> ();
									if (n.currentBehaviour == null) {

									} else {
										Destroy (n.currentBehaviour);
									}

									NPCBehaviour_SwatGoToRoomEntrance nb = n.gameObject.AddComponent<NPCBehaviour_SwatGoToRoomEntrance> ();
									n.currentBehaviour = nb;
									goToEntrance.Add (nb);
								}
							}

							if (areSwatAtEntranceToRoom()== true) {
								PhoneTab_RadioHack.me.setNewText ("SWAT Team secutring " + currentRoomSwat.roomName,radioHackBand.swat);

								goingToRoom = false;
							}
						}
						else if (nb.areWeAtPosition () == true) {

							pointsBeenTo.Add (pointsToGoTo [0]);
							pointsToGoTo.RemoveAt (0);
							if (pointsToGoTo.Count == 0) {
								PhoneTab_RadioHack.me.setNewText (currentRoomSwat.roomName + "Has been secured, moving to " + roomsGoingTo [0].roomName, radioHackBand.swat);

								currentRoomSwat = null;
								pointsBeenTo.Clear ();
								swatGoingIn.Clear ();
								goToEntrance.Clear ();
								goingToRoom = true;
							}
							nb = null;
						}

				}else if (seenHostile == true) {
					if (setSwatToAttack == false) {
						PhoneTab_RadioHack.me.setNewText ("Target spotted, moving to engage.",radioHackBand.swat);

						attacking = new List<NPCBehaviour_SWATAttackTarget> ();
						foreach (GameObject g in swatInLevel) {
							NPCController npc = g.GetComponent<NPCController> ();
							if (npc.currentBehaviour == null) {

							} else {
								Destroy (npc.currentBehaviour);
							}
							NPCBehaviour_SWATAttackTarget nb = g.AddComponent<NPCBehaviour_SWATAttackTarget> ();
							npc.currentBehaviour = nb;
							attacking.Add (nb);
						}
						setSwatToAttack = true;
					} else {
						//add some condition to  check if they have lost the target
						if (doSwatStillHaveTarget () == true) {
							lastRoom = LevelController.me.roomPlayerIsIn;
						} else {
							swatLoseTargetTimer -= Time.deltaTime;
							if (swatLoseTargetTimer <= 0) {
								if (lastRoom == null) {

								} else {
									PhoneTab_RadioHack.me.setNewText ("Target lost, moving to " + lastRoom.roomName, radioHackBand.swat);
								}
								seenHostile = false;
								investigatePoint = false;
								currentRoomSwat = null;
								goingToRoom = true;
								swatLoseTargetTimer = 10.0f;
								setSwatToAttack = false;
							}


						}
					}

				} else if (investigatePoint == true) {
					if (investigate.areWeAtPosition () == false) {
						swatTarget = investigate.haveWeSeenSuspiciousObject ();
						if (swatTarget == null) {
							seenHostile = false;
						} else {
							seenHostile = true;
						}
					} else {
						if (seenHostile == false) {

							NPCController npc = swatInvestigating.GetComponent < NPCController> ();

							if (npc.currentBehaviour == null) {

							} else {
								Destroy (npc.currentBehaviour);
							}

							NPCBehaviour nb = swatGoingToCurrentPoint.AddComponent<NPCBehaviour_SwatGoToPoint> ();
							npc.currentBehaviour = nb;

						}
					}
				}
			} else {
				foreach (GameObject g in swatInLevel) {
					NPCController npc = g.GetComponent<NPCController> ();

					if (npc.currentBehaviour==null || npc.currentBehaviour.myType != behaviourType.formUp) {
						if (npc.currentBehaviour == null) {

						} else {
							Destroy (npc.currentBehaviour);
						}

						NPCBehaviour_SwatFormUp newB = g.AddComponent<NPCBehaviour_SwatFormUp> ();
						npc.currentBehaviour = newB;
					} 
				}
			}
		}
	}

	bool areWeFormedUp()
	{
		//if(areAllSwatSpawned()==false)
		//{
		//	return false;
		//}

		if (swatFormedUp == false ) {
			foreach (GameObject g in swatInLevel) {
				if (Vector2.Distance (g.transform.position, swatFormUpPoint.position) > 5.0f) {
					return false;
				}
			}
			swatFormedUp=true;
			return true;
		} else {
			return true;
		}
	}
								


	bool areSwatAtEntranceToRoom()
	{
		if (currentRoomSwat == null) {
			return false;
		}


		bool isThereSwatNotAtRoom = false;
		foreach (GameObject g in swatInLevel) {
			bool isThisSwatThere=false;
			foreach (Transform t in currentRoomSwat.entrances) {
				if (Vector2.Distance (g.transform.position, t.transform.position) < 5) {
					isThisSwatThere = true;
				}
			}
			if (isThisSwatThere == false) {
				isThereSwatNotAtRoom = true;
			}
		}

		if (isThereSwatNotAtRoom == true) {
			return false;
		} else {
			goingToRoom = false;
			return true;
		}
	

	}
	void addCopCar(GameObject g)
	{
		if (copCarsInWorld == null) {
			copCarsInWorld = new List<GameObject> ();
		}

		copCarsInWorld.Add (g);
	}

	bool areAllSwatSpawned()
	{
		foreach (GameObject g in copCarsInWorld) {
			PoliceCarScript pcs = g.GetComponent<PoliceCarScript> ();
			if (pcs.swat == true && pcs.spawnedCops== false) {
				return false;
			}
		}
		return true;
	}
}
