using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScript : MonoBehaviour {
	//public Transform bottomLeft,topRight;
	public string roomName;
	public List<roomRect> rectsInRoom;
	public bool shootOnSight,traspassing;
	public List<Item> itemsInRoomAtStart;
	public List<GameObject> pointsInRoom;
	// Use this for initialization
	public List<Vector3> pointsForSwat;
	public List<GameObject> nodesForSwat;
	public List<Transform> entrances;
	public int distFromSwatFormUp = 0;

	public bool isOutdoors=false;
	void Awake()
	{
		pointsInRoom = new List<GameObject> ();
		if (rectsInRoom == null || rectsInRoom.Count==0) {
			rectsInRoom = new List<roomRect> ();
			roomRect[] rects = this.gameObject.GetComponentsInChildren<roomRect> ();
			foreach (roomRect r in rects) {
				rectsInRoom.Add (r);
			}
		}

		foreach (roomRect r in rectsInRoom) {
			Vector3 blPos = r.bottomLeft.transform.position;
			Vector3 trPos = r.topRight.transform.position;

			if (blPos.x > trPos.x) {
				r.topRight.transform.position = blPos;
				r.bottomLeft.transform.position = trPos;
			}

		}


		/*foreach (roomRect r in rectsInRoom) {
			GameObject mid = new GameObject ();
			mid.transform.parent = this.transform;
			mid.name = "mid point";
			mid.transform.position = r.bottomLeft.position + ((r.topRight.position - r.bottomLeft.position) / 2);
			pointsInRoom.Add (mid);
		}

		foreach (Vector3 v in pointsForSwat) {
			GameObject mid = new GameObject ();
			mid.transform.parent = this.transform;
			mid.name = "swat point for room";
			mid.transform.position = v;
			pointsInRoom.Add (mid);
		}
		if (nodesForSwat == null || nodesForSwat.Count == 0) {
			setPointsToGoTo ();
		}*/
	}

	public void addNewEntranceToBuilding()
	{
		if (entrances == null) {
			entrances = new List<Transform> ();
		}

		GameObject g = new GameObject ();
		g.name = roomName + " Entrance " + entrances.Count;
		g.transform.position = this.transform.position + new Vector3(0,1,0);
		g.transform.parent = this.transform;
		entrances.Add (g.transform);
	}

	void Start()
	{
		
		itemsInRoomAtStart = itemsInRoom ();
		setPointsToGoTo ();
		distFromSwatFormUp = Pathfinding.me.getPathCost(entrances[0].gameObject,PoliceController.me.swatFormUpPoint.gameObject);

	}

	public void addNewRoomRect()
	{
		if (rectsInRoom == null) {
			rectsInRoom = new List<roomRect> ();
		}

		GameObject myObj = (GameObject)Instantiate (new GameObject (), this.transform);
		myObj.name = roomName + " Room rect " + rectsInRoom.Count;
		roomRect rc = myObj.AddComponent<roomRect>();
		rc.bottomLeft = Instantiate (new GameObject (), this.transform.position - new Vector3 (-1, -1, 0), Quaternion.Euler (0, 0, 0)).transform;
		rc.bottomLeft.transform.parent = myObj.transform;
		rc.topRight = Instantiate (new GameObject (), this.transform.position - new Vector3 (1, 1, 0), Quaternion.Euler (0, 0, 0)).transform;
		rc.topRight.transform.parent = myObj.transform;

		rc.bottomLeft.gameObject.name = roomName + " bottom left " + rectsInRoom.Count;
		rc.topRight.gameObject.name = roomName + " top right " + rectsInRoom.Count;
		rectsInRoom.Add (rc);

	}

	public void findPointsForSwat()
	{
		if (rectsInRoom.Count == 0) {
			roomRect[] rects = gameObject.GetComponentsInChildren<roomRect> ();
			foreach (roomRect rs in rects) {
				if (rectsInRoom.Contains (rs) == false) {
					rectsInRoom.Add (rs);
				}
			}
		}

		pointsForSwat = new List<Vector3> ();

		foreach (roomRect r in rectsInRoom) {
			Vector3 dirToMid = r.getCenterPoint ();

			Vector3 point1 = new Vector3 (r.bottomLeft.transform.position.x, r.bottomLeft.transform.position.y, 0);
			Vector3 point2 = new Vector3 (r.bottomLeft.transform.position.x, r.topRight.transform.position.y, 0);

			Vector3 point3 = new Vector3 (r.topRight.transform.position.x, r.bottomLeft.transform.position.y, 0);
			Vector3 point4 = new Vector3 (r.topRight.transform.position.x, r.topRight.transform.position.y, 0);

			pointsForSwat.Add (point1 + ((dirToMid-point1).normalized*2));
			pointsForSwat.Add (point2+ ((dirToMid-point2).normalized*2));
			pointsForSwat.Add (point3+ ((dirToMid-point3).normalized*2));
			pointsForSwat.Add (point4+ ((dirToMid-point4).normalized*2));



		}
	}


	
	// Update is called once per frame
	void Update () {
		//if (nodesForSwat == null || nodesForSwat.Count == 0) {
			//setPointsToGoTo ();

		//}
	}

	public bool isObjectInRoom(GameObject obj)
	{

		foreach (roomRect r in rectsInRoom) {
			
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

		foreach (roomRect r in rectsInRoom) {
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

	public void addNodesToRoom(GameObject g)
	{
		if (pointsInRoom == null) {
			pointsInRoom = new List<GameObject> ();
			foreach (roomRect r in rectsInRoom) {
				GameObject mid = new GameObject ();
				mid.transform.position = r.bottomLeft.position + ((r.topRight.position - r.bottomLeft.position) / 2);
				mid.transform.parent = this.transform;
				mid.name = "Mid point";
				//= (GameObject)Instantiate (new GameObject (), r.bottomLeft.position + ((r.topRight.position - r.bottomLeft.position) / 2), Quaternion.Euler (0, 0, 0));
				pointsInRoom.Add (mid);
			}
		}
		pointsInRoom.Add (g);
	}

	public GameObject getRandomPoint()
	{
		return pointsInRoom [Random.Range (0, pointsInRoom.Count)];
	}

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

	public void setPointsToGoTo(){
		foreach (Vector3 v in pointsForSwat) {
			float distanceToPoint = 999999.0f;
			GameObject near = null;
			foreach (GameObject g in pointsInRoom) {
				float d = Vector3.Distance (g.transform.position, v);

				if (d < distanceToPoint) {
					distanceToPoint = d;
					near = g;
				}
			}
			nodesForSwat.Add (near);
		}
	}

	public roomRect getRectIAmIn(Vector3 pos)
	{
		foreach (roomRect r in rectsInRoom) {
			if (r.amIInRoomRect (pos)) {
				return r;
			}
		}
		return null;
	}

	public Vector3 getCenter()
	{
		return rectsInRoom [0].getCenterPoint ();
	}
}

