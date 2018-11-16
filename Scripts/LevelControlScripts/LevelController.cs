using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour {

	/// <summary>
	/// Class that controls misc date about the level, all police related stuff has been moved to the "PoliceController" class so its not used here and needs to be removed at some point.
	/// </summary>


	public static LevelController me;
	public List<PatrolRoute> patrolRoutesInLevel;
	public RoomScript[] roomsInLevel;
	public RoomScript roomPlayerIsIn;
	public Transform itemDepositLoc,raiseAlarmLoc,copRaiseAlarmLoc;
	public string roomName="";
	public int alertLevel = 0;//0 = fine, 1 = extra searches, 2= cops called
	public List<GameObject> suspects;
	public List<Transform> levelExits;
	public List<CivilianAction> actionsInWorld;
	public List<GameObject> phonesInLevel = new List<GameObject>();
	public float copSpawnTimer = 60;
	public bool spawnedCops=false;
	public bool spawnedSwat = false;
	public float swatSpawnTimer = 60;
	public Transform levelEntry;
	public FloorScript[] floors;
	public FloorScript playerFloor;
	public BuildingScript[] buildings;
	public List<Container> containersForEssentialItems;
	public List<Transform> pointsToExitLevel;
	void Awake()
	{
		me = this;
		roomsInLevel = FindObjectsOfType<RoomScript> ();
		buildings = FindObjectsOfType<BuildingScript> ();
		floors = FindObjectsOfType<FloorScript> ();
	}

	// Use this for initialization
	void Start () {
		setActions ();	
		setNodesInRoom ();
	}

	void setActions()
	{
		actionsInWorld = new List<CivilianAction> ();
		CivilianAction[] c = FindObjectsOfType<CivilianAction> ();
		foreach (CivilianAction ca in c) {
			actionsInWorld.Add (ca);
		}
	}
	
	// Update is called once per frame
	void Update () {
		workOutPlayerRoom ();
		workOutPlayerFloor ();
	}

	void countDownCopTimer()
	{
		copSpawnTimer -= Time.deltaTime;
		if (Mathf.RoundToInt (copSpawnTimer) % 25 == 0) {
			PhoneTab_RadioHack.me.setNewText ("Police ETA: " + Mathf.RoundToInt (copSpawnTimer) + " seconds",radioHackBand.cop);
		}

		//////////Debug.Log ("Time till cops = " + copSpawnTimer.ToString ());
		if (copSpawnTimer <= 0) {
			for(int x = 0;x<8;x++)
			{
				Instantiate (CommonObjectsStore.me.cop, levelEntry.transform.position, Quaternion.Euler (Vector3.zero));
			}
			PhoneTab_RadioHack.me.setNewText ("Officers are on the scene now.",radioHackBand.cop);

			spawnedCops = true;
		}
	}

	void countDownSwatTimer()
	{
		swatSpawnTimer -= Time.deltaTime;
		//////////Debug.Log ("Time till Swat = " + swatSpawnTimer.ToString ());
		if (Mathf.RoundToInt (swatSpawnTimer) % 25 == 0) {
			PhoneTab_RadioHack.me.setNewText ("Armed Response ETA: " + Mathf.RoundToInt (copSpawnTimer) + " seconds",radioHackBand.cop);
		}
		if (swatSpawnTimer <= 0) {
			for(int x = 0;x<5;x++)
			{
				Instantiate (CommonObjectsStore.me.swat, levelEntry.transform.position, Quaternion.Euler (Vector3.zero));
			}
			PhoneTab_RadioHack.me.setNewText ("Armed Response on the scene.",radioHackBand.cop);

			spawnedSwat = true;
		}
	}

	public RoomScript getRoomObjectIsIn(GameObject obj)
	{
		foreach (RoomScript r in roomsInLevel) {
			if (r.isObjectInRoom (obj) == true) {
				return r;
			}
		}
		return null;
	}

	public RoomScript getRoomPosIsIn(Vector3 pos)
	{
		foreach (RoomScript r in roomsInLevel) {
			if (r.isPosInRoom(pos) == true) {
				return r;
			}
		}
		return null;
	}

	public BuildingScript getBuildingPosIsIn(Vector3 pos)
	{
		if (buildings==null || buildings.Length == 0) {
			buildings = FindObjectsOfType<BuildingScript> ();
		}

		foreach (BuildingScript b in buildings) {
			if (b.isPosInRoom (pos) == true) {
				return b;
			}
		}
		return null;
	}

	public PatrolRoute getRandomRoute()
	{
		return patrolRoutesInLevel [Random.Range (0, patrolRoutesInLevel.Count)];
	}

	void workOutPlayerRoom()
	{
		foreach (RoomScript r in roomsInLevel) {
			if (r.isObjectInRoom (CommonObjectsStore.player) == true) {
				roomPlayerIsIn = r;
				roomName = r.roomName;
			} 
		}
	}

	void workOutPlayerFloor()
	{
		foreach (FloorScript f in floors) {
			if (f.isObjectInRoom (CommonObjectsStore.player) == true) {
				playerFloor = f;
				return;
			}
		}
		playerFloor = null;
	}



	public void addSuspect(GameObject toAdd)
	{
		if (suspects == null) {
			suspects = new List<GameObject> ();
		}

		suspects.Add (toAdd);
	}

	public CivilianAction getCivilianAction()
	{
		List<CivilianAction> actionsAvailable = new List<CivilianAction> ();
		foreach (CivilianAction ca in actionsInWorld) {
			if (ca.actionAvailable == true) {
				actionsAvailable.Add (ca);
			}
		}

		return actionsAvailable [Random.Range (0, actionsAvailable.Count)];
	}

	public CivilianAction getActionOfSameType(string actionType)
	{
		foreach (CivilianAction ca in actionsInWorld) {
			if (ca.actionName == actionType && ca.actionAvailable == true && ca.doingAction == null) {
				return ca;
			}
		}

		return getCivilianAction ();
	}

	public Transform getRandomExit()
	{
		return levelExits [Random.Range (0, levelExits.Count)];
	}

	public GameObject getNearestPhone(Vector3 pos)
	{
		GameObject nearest = null;
		float dist = 9999999.0f;

		foreach (GameObject g in phonesInLevel) {

			//if (PoliceController.me.underSiege == true) {
			//	if (PoliceController.me.buildingUnderSiege.isPosInRoom (g.transform.position)) {
			//		continue;
			//	}
			//}

			float dist2 = Vector3.Distance(this.transform.position,pos);

			if (dist2 < dist) {
				nearest = g;
				dist = dist2;
			}
		}

		return nearest;
	}

	public void setNodesInRoom()
	{
		foreach (GameObject g in WorldBuilder.me.worldTiles) {
			if (g == null) {

			} else {
				WorldTile wt = g.GetComponent<WorldTile> ();
				if (wt.walkable == true) {
					RoomScript r = getRoomObjectIsIn (g);
					if (r == null) {

					} else {
						r.addNodesToRoom (g);
					}
				}
			}
		}
	}

	public bool canWeLeaveLevel(){
	/*	if (PoliceController.me.copsHere == true || PoliceController.me.backupHere == true || PoliceController.me.swatHere == true) {
			if (PlayerCarController.inCar == true) {
				return true;
			} else {
				foreach (Transform t in pointsToExitLevel) {
					if (Vector2.Distance (t.transform.position, CommonObjectsStore.player.transform.position) < 10) {
						return true;
					}
				}

				return false;
			}

		} else {
			return true;
		}*/
		return true;
	}
}
