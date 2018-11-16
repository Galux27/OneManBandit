using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingScript : MonoBehaviour {
	/// <summary>
	/// Script that defines an area for a building
	/// </summary>


	//public Transform bottomLeft,topRight;
	public string buildingName;
	public List<roomRect> rectsInBuilding;
	public bool shootOnSight,traspassing;
	public List<Item> itemsInRoomAtStart;
	// Use this for initialization
	public List<RoomScript> myRooms;
	public List<Transform> entrances;

	public List<WorldTile> tilesInBuilding;
	public bool isOutdoors=false;
	public static BuildingScript outdoors;

	/// <summary>
	/// Each rect has an invisible object above it that blocks light from hitting the floor inside, to give the effect of there being more building above
	/// </summary>
	public List<GameObject> shadowBlockers;
    public bool buildingClosed = false, closedFromIncident = false;

    public bool buildingHasOpeningTime;
    public int openHour = 0, openMin = 0;
    public int closeHour = 23, closeMin = 59;

    bool shouldWeCloseBuilding()
    {
        if (closedFromIncident == true)
        {
            return true;
        }

        if(TimeScript.me.hour < openHour || TimeScript.me.hour >closeHour)
        {
            return true;
        }else if(TimeScript.me.hour==openHour)
        {
            if(TimeScript.me.minute<openMin)
            {
                return true;
            }
        }else if(TimeScript.me.hour == closeHour)
        {
            if(TimeScript.me.minute>=closeMin)
            {
                return true;
            }
        }
        return false;
    }

    void openCloseBuildingControl()
    {
        if(shouldWeCloseBuilding())
        {
            if(buildingClosed==false)
            {
                closeBuilding();
                //Debug.Log("Building was closed "  + buildingName);
            }
            //Debug.Log("Building should be closed " + buildingName);
        }
        else
        {
            if(buildingClosed==true)
            {
                openBuilding();
                //Debug.Log("Building was opened");
            }
            //Debug.Log("Building should be open " + buildingName);

        }
    }


    void closeBuilding()
    {
        CivilianAction[] actions = FindObjectsOfType<CivilianAction>();
        foreach (CivilianAction ca in actions)
        {
            if (isPosInRoom(ca.positionForAction.position))
            {
                ca.actionAvailable = false;
            }
        }

        DoorScript[] doors = getAllDoorsInLevel().ToArray();
        foreach (DoorScript d in doors)
        {
            if (isPosInRoom(d.transform.position))
            {
                d.locked = true;
                if (d.GetComponent<PlayerAction_UnlockWithLockpick>() == false)
                {
                    d.gameObject.AddComponent<PlayerAction_UnlockWithLockpick>();
                }
            }
        }

        foreach (RoomScript r in myRooms)
        {
            r.closed = true;
        }

        foreach(Shop s in Shop.shopsInWorld)
        {
            s.shopAvailable = false;
            s.myKeeper.SetActive(false);
        }
        buildingClosed = true;
    }

    void openBuilding()
    {
        CivilianAction[] actions = FindObjectsOfType<CivilianAction>();
        foreach (CivilianAction ca in actions)
        {
            if (isPosInRoom(ca.positionForAction.position))
            {
                ca.actionAvailable = true;
            }
        }

        DoorScript[] doors = getAllDoorsInLevel().ToArray();
        foreach (DoorScript d in doors)
        {
            if (isPosInRoom(d.transform.position))
            {
                d.locked = false;
               // if (d.GetComponent<PlayerAction_UnlockWithLockpick>() == true)
              //  {
             //       d.gameObject.AddComponent<PlayerAction_UnlockWithLockpick>();
             //   }
            }
        }

        foreach (RoomScript r in myRooms)
        {
            r.closed = true;
        }

        foreach (Shop s in Shop.shopsInWorld)
        {
            s.shopAvailable = true;
            s.myKeeper.SetActive(true);
        }
        buildingClosed = false;
    }

	void Awake()
	{

		if (rectsInBuilding == null || rectsInBuilding.Count==0) {
			rectsInBuilding = new List<roomRect> ();
			roomRect[] rects = this.gameObject.GetComponentsInChildren<roomRect> ();
			foreach (roomRect r in rects) {
				rectsInBuilding.Add (r);
			}
		}

		if (isOutdoors == true) {
			outdoors = this;
		}

		foreach (roomRect rs in rectsInBuilding) {
			Vector3 blPos = rs.bottomLeft.transform.position;
			Vector3 trPos = rs.topRight.transform.position;

			if (blPos.x > trPos.x) {
				rs.topRight.transform.position = blPos;
				rs.bottomLeft.transform.position = trPos;
			}

		}

		if (myRooms == null || myRooms.Count == 0) {
			RoomScript[] rooms = FindObjectsOfType<RoomScript> ();
			if (isOutdoors == false) {
				foreach (RoomScript r in rooms) {
					if (isObjectInRoom (r.rectsInRoom [0].bottomLeft.gameObject) == true) {
						addRoomToBuilding (r);
					}
				}
			} else {
				foreach (RoomScript r in rooms) {
					if (r.isOutdoors == true) {
						addRoomToBuilding (r);
					}
				}
			}
		}
	}

	/// <summary>
	/// Sets incidents in the building (broken doors/windows), with sufficient incidents the building will be closed till they are fixed.
	/// </summary>
	void checkForIncidentsInBuilding()
	{
		int numOfIncidentsInBuilding = 0;
		foreach (Vector3 ip in LevelIncidentController.me.incidentPositions) {
			if (isPosInRoom (ip)) {
				numOfIncidentsInBuilding++;
			}
		}
		//Debug.Log ("We found " + numOfIncidentsInBuilding + " incidents in the building " + buildingName);
		if (numOfIncidentsInBuilding >= 5) {
			CivilianAction[] actions = FindObjectsOfType<CivilianAction> ();
			foreach (CivilianAction ca in actions) {
				if (isPosInRoom (ca.positionForAction.position)) {
					ca.actionAvailable = false;
				}
			}

			DoorScript[] doors = getAllDoorsInLevel ().ToArray ();
			foreach (DoorScript d in doors) {
				if (isPosInRoom (d.transform.position)) {
					d.locked = true;
					if (d.GetComponent<PlayerAction_UnlockWithLockpick> () == false) {
						d.gameObject.AddComponent<PlayerAction_UnlockWithLockpick> ();
					}
				}
			}

			foreach (RoomScript r in myRooms) {
				r.traspassing = true;
			}

			Item[] items = FindObjectsOfType<Item> ();
			foreach (Item i in items) {
				if(isObjectInRoom(i.gameObject)){
					if (i.myContainer == null && i.isActiveAndEnabled == true && isItemBase (i) == false) {
						i.gameObject.SetActive(false);	
					}
				}
			}
			buildingClosed = true;
            closedFromIncident = true;
		}
	}

	/// <summary>
	/// Checks whether the item is one of the instances that the item database uses (true) or is an instance in the world (false)
	/// </summary>
	/// <returns><c>true</c>, , <c>false</c> otherwise.</returns>
	/// <param name="i">The index.</param>
	bool isItemBase(Item i)
	{
		foreach(GameObject i2 in ItemDatabase.me.items)
		{
			if (i.gameObject == i2) {
				return true;
			}
		}
		return false;
	}

	public void addNewEntranceToBuilding()
	{
		if (entrances == null) {
			entrances = new List<Transform> ();
		}

		GameObject g = (GameObject)Instantiate (new GameObject (), this.transform.position+new Vector3(0,5,0), this.transform.rotation);
		g.name = buildingName + " Entrance " + entrances.Count;
		entrances.Add (g.transform);
	}

	public void addRoomToBuilding(RoomScript r)
	{
		if (myRooms == null) {
			myRooms = new List<RoomScript> ();
		}

		myRooms.Add (r);
	}

	public void addNewRoomRect()
	{
		if (rectsInBuilding == null) {
			rectsInBuilding = new List<roomRect> ();
		}

		GameObject myObj = (GameObject)Instantiate (new GameObject (), this.transform);
		myObj.name = buildingName + " Room rect " + rectsInBuilding.Count;
		roomRect rc = myObj.AddComponent<roomRect>();
		rc.bottomLeft = Instantiate (new GameObject (), this.transform.position - new Vector3 (-1, -1, 0), Quaternion.Euler (0, 0, 0)).transform;
		rc.bottomLeft.transform.parent = myObj.transform;
		rc.topRight = Instantiate (new GameObject (), this.transform.position - new Vector3 (1, 1, 0), Quaternion.Euler (0, 0, 0)).transform;
		rc.topRight.transform.parent = myObj.transform;

		rc.bottomLeft.gameObject.name = buildingName + " bottom left " + rectsInBuilding.Count;
		rc.topRight.gameObject.name = buildingName + " top right " + rectsInBuilding.Count;
		rectsInBuilding.Add (rc);

	}

	/// <summary>
	/// Finds nodes that are in this building.
	/// </summary>
	public void getPoints()
	{
		tilesInBuilding = new List<WorldTile> ();

		foreach (GameObject g in WorldBuilder.me.worldTiles) {
			if (g == null) {
				continue;
			}

			if (isPosInRoom (g.transform.position) == true) {
				WorldTile wt = g.GetComponent<WorldTile> ();
				tilesInBuilding.Add (wt);
			}
		}
	}

	void Start () {
		itemsInRoomAtStart = itemsInRoom ();
		//		////////Debug.Log ("There are " + itemsInRoomAtStart.Count + " Items in " + roomName);
		if (tilesInBuilding == null || tilesInBuilding.Count == 0) {
			tilesInBuilding = new List<WorldTile> ();

			foreach (GameObject g in WorldBuilder.me.worldTiles) {
				if (g == null) {
					continue;
				}

				if (isPosInRoom (g.transform.position) == true) {
					WorldTile wt = g.GetComponent<WorldTile> ();
					tilesInBuilding.Add (wt);
				}
			}
		}

		checkForIncidentsInBuilding ();
	}

    // Update is called once per frame
    private void Update()
    {
        openCloseBuildingControl();
    }
    public bool isObjectInRoom(GameObject obj)
	{

		foreach (roomRect r in rectsInBuilding) {
			if (r.amIInRoomRect (obj) == true) {
				return true;
			}
		}

		//if (obj.transform.position.x > bottomLeft.position.x && obj.transform.position.x < topRight.position.x) {
		//	if (obj.transform.position.y > bottomLeft.position.y && obj.transform.position.y < topRight.position.y) {
		//		return true;
		//	}
		//}
		return false;
	}




	public bool isPosInRoom(Vector3 pos)
	{

		foreach (roomRect r in rectsInBuilding) {
			if (r.amIInRoomRect (pos) == true) {
				return true;
			}
		}
		//if (pos.x > bottomLeft.position.x && pos.x < topRight.position.x) {
		//	if (pos.y > bottomLeft.position.y && pos.y < topRight.position.y) {
		//		return true;
		//	}
		//}
		return false;
	}
		
	/// <summary>
	/// Returns the items in the building that are not held in a container or disabled
	/// </summary>
	/// <returns>The in room.</returns>
	public List<Item> itemsInRoom()
	{
		List<Item> retVal = new List<Item> ();
		foreach (Item i in ItemMoniter.me.itemsInWorld) {
			if (i.gameObject.activeInHierarchy == true) {
				if (isObjectInRoom (i.gameObject) == true) {
					retVal.Add (i);
				}
			}
		}
		return retVal;	
	}

	public void setBuildingUnderSiege()
	{
		//want to make all the nodes undesireable for pathfinding, disable all the civilian actions, get all non hostage, non hostile actors out
		foreach (WorldTile wt in tilesInBuilding) {
			wt.modifier = 150;
			ThreadedPathfindInterface.me.nodes [wt.gridX, wt.gridY].modifier = wt.modifier;
		}


	}

	public void setBuildingWeightsToNormal(){
		foreach (WorldTile wt in tilesInBuilding) {
			wt.modifier = 10;
			ThreadedPathfindInterface.me.nodes [wt.gridX, wt.gridY].modifier = wt.modifier;
		}
	}

	public bool setShadowBlockers()
	{
		if (shadowBlockers == null) {
			shadowBlockers = new List<GameObject> ();
		}

		if (shadowBlockers.Count != rectsInBuilding.Count) {
			foreach (GameObject g in shadowBlockers) {
				DestroyImmediate (g);
			}
			shadowBlockers.Clear ();
			if (isOutdoors == true) {
				return true;
			}

			foreach (roomRect r in rectsInBuilding) {
				GameObject g = GameObject.CreatePrimitive (PrimitiveType.Cube);
				g.transform.position = r.bottomLeft.position + ((r.topRight.position - r.bottomLeft.position) / 2);
				g.transform.parent = this.transform;
				float xScale = r.topRight.position.x - r.bottomLeft.position.x;
				float yScale = r.topRight.position.y - r.bottomLeft.position.y;
				g.transform.localScale = new Vector3 (xScale, yScale, 5.0f);
				g.name = buildingName + " Shadow Blocker";
				g.transform.parent = r.transform;
				MeshRenderer mr = g.GetComponent<MeshRenderer> ();
				mr.receiveShadows = false;
				mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
				DestroyImmediate (g.GetComponent<BoxCollider> ());
				shadowBlockers.Add (g);
			}
			return false;
		} else {
			return true;
		}
	}

	public void setNewShadowBlockers()
	{


		if (shadowBlockers == null) {
			shadowBlockers = new List<GameObject> ();
		}

		foreach (GameObject g in shadowBlockers) {
			DestroyImmediate (g);
		}


		shadowBlockers.Clear ();

		if (isOutdoors == true) {
			return;
		}

		foreach (roomRect r in rectsInBuilding) {
			GameObject g = GameObject.CreatePrimitive (PrimitiveType.Cube);
			g.transform.position = r.bottomLeft.position + ((r.topRight.position - r.bottomLeft.position) / 2);
			g.transform.parent = this.transform;
			float xScale = r.topRight.position.x - r.bottomLeft.position.x;
			float yScale = r.topRight.position.y - r.bottomLeft.position.y;
			g.transform.localScale = new Vector3 (xScale, yScale, 5.0f);
			g.name = buildingName + " Shadow Blocker";
			g.transform.parent = r.transform;
			MeshRenderer mr = g.GetComponent<MeshRenderer> ();
			mr.receiveShadows = false;
			mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
			DestroyImmediate (g.GetComponent<BoxCollider> ());
			shadowBlockers.Add (g);
		}

	}

	List<DoorScript> getAllDoorsInLevel()
	{
		List<DoorScript> retVal = new List<DoorScript> ();
		DoorScript[] doors = Resources.FindObjectsOfTypeAll<DoorScript> ();


		foreach (DoorScript d in doors) {
			if (d.gameObject.scene.IsValid() == false) {

			} else if (d.isTempDoor == true) {

			}else {
				retVal.Add (d);
			}
		}
		return retVal;
	}
}