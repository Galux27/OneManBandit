using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that contains an unthreaded & threaded versions of the A* algorithm, 
/// </summary>
public class Pathfinding : MonoBehaviour {



	public static Pathfinding me;
	public GameObject player,target;
	public List<WorldTile> temp;
	public bool highlightPathFound = false;

	public ThreadedWorldTile[,] pathNodes;
	bool doubleCheckForWalkable = false;
	void Awake()
	{
		if (me == null) {
			me = this;
		}
	}

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
	}



	// Update is called once per frame
	void Update () {
		if (doubleCheckForWalkable == false) {
			for (int x = 0; x <pathNodes.GetLength (0) - 1; x++) {
				for (int y = 0; y < pathNodes.GetLength (1) - 1; y++) {
					GameObject g = WorldBuilder.me.worldTiles [x, y];
					if (g == null || pathNodes [x, y] == null) {

					} else {
						WorldTile wt =g.GetComponent<WorldTile> ();
						if (wt == null) {

						} else {
							pathNodes [x, y].walkable = wt.walkable;

						}
					}
						
				}
			}
			doubleCheckForWalkable = true;
		}

	}

	public List<Vector3> findPath(GameObject me,GameObject targetObj)
	{
		List<Vector3> retVal;
		List<WorldTile> tilePath = new List<WorldTile>();
		findPath (me, targetObj, ref tilePath);
		retVal = convertWorldTileListToVectorList (tilePath);
		return retVal;
	}

	public int getPathCost(GameObject me,GameObject targetObj)
	{
		List<WorldTile> tilePath = new List<WorldTile>();
		findPath (me, targetObj, ref tilePath);
	

		return tilePath.Count;
	}

	List<Vector3>convertWorldTileListToVectorList(List<WorldTile> path)
	{
		List<Vector3> retVal = new List<Vector3> ();
		foreach (WorldTile t in path) {
			retVal.Add (t.transform.position);
		}
		return retVal;
	}

	public void findPath(GameObject startPos,GameObject endPos,ref List<WorldTile> store)
	{
		
			WorldTile startNode; //had a bug where the method would go through all the tiles in the grid causing a lag spike, just added a condititon to check for a nearby walkable tile, if its null after this it just abandons the path

			if (startPos.GetComponent<WorldTile> () == true) {
				startNode = startPos.GetComponent<WorldTile> ();
			} else {
				startNode = WorldBuilder.me.findNearestWorldTile (startPos.transform.position);
			}

			
			WorldTile endNode ;

			if (endPos.GetComponent<WorldTile> () == true) {
				endNode = endPos.GetComponent<WorldTile> ();
			} else {
				endNode = WorldBuilder.me.findNearestWorldTile (endPos.transform.position);
			}


		if (startNode.walkable == false) {
			//startNode = getNearestWalkableTiles (startNode.gridX, startNode.gridY);
		}

		if (endNode.walkable == false) {
			endNode = getNearestWalkableTiles (endNode.gridX, endNode.gridY);
		}

		if (startNode == null || endNode == null) {
			return;
		}



			List<WorldTile> openSet = new List<WorldTile>();
			HashSet<WorldTile> closedSet = new HashSet<WorldTile>();
			openSet.Add(startNode);

			while (openSet.Count > 0) {
				WorldTile node = openSet[0];

				for (int i = 1; i < openSet.Count; i ++) {
					if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost) {
						if (openSet[i].hCost < node.hCost)
							node = openSet[i];
					}
				}

				openSet.Remove(node);
				closedSet.Add(node);

				if (node == endNode) {
					RetracePath(startNode,endNode,ref store);
					return;
				}

				foreach (WorldTile neighbour in node.myNeighbours) {
					if (!neighbour.walkable || closedSet.Contains(neighbour) || neighbour==null || node==null) {
						continue;
					}

				int newCostToNeighbour = node.gCost + GetDistance(node, neighbour)+node.getModifier();
					if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
						neighbour.gCost = newCostToNeighbour;
						neighbour.hCost = GetDistance(neighbour, endNode);
						neighbour.parent = node;

						if (!openSet.Contains(neighbour))
							openSet.Add(neighbour);
					}
				}
			}

	}






	WorldTile getNearestWalkableTiles(int gridX,int gridY)
	{
		WorldTile invalidTile = WorldBuilder.me.worldTiles [gridX, gridY].GetComponent<WorldTile> ();

		foreach (WorldTile t in invalidTile.myNeighbours) {
			if (t.walkable == true) {
				return t;
			}
		}

		return null;
	}


	void RetracePath(WorldTile startNode,WorldTile targetNode,ref List<WorldTile> store)
	{
		List<WorldTile> path = new List<WorldTile>();
		WorldTile currentNode = targetNode;

		while (currentNode != startNode) {
			path.Add(currentNode);

			currentNode = currentNode.parent;
		}
		path.Reverse();
		store = path;
	}

	int GetDistance(WorldTile nodeA,WorldTile nodeB)
	{
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
	}


	public void setThreadedNodeArray(GameObject[,] originalNodes)
	{

		pathNodes = new ThreadedWorldTile[originalNodes.GetLength(0),originalNodes.GetLength(1)];

		for (int x = 0; x < originalNodes.GetLength(0)-1; x++) {
			for (int y = 0; y < originalNodes.GetLength (1)-1; y++) {
				GameObject g = originalNodes [x, y];
				if (g == null) {

				} else {
					WorldTile wt = g.GetComponent<WorldTile> ();
					if (wt == null) {

					} else {
						ThreadedWorldTile t = new ThreadedWorldTile ();
						t.gridX = wt.gridX;
						t.gridY = wt.gridY;
						t.worldPos = wt.transform.position;
						t.walkable = wt.walkable;

						pathNodes [x, y] = t;
					}
				}
			}
		}

		for (int x = 0; x < originalNodes.GetLength(0)-1; x++) {
			for (int y = 0; y < originalNodes.GetLength (1)-1; y++) {
				GameObject g = originalNodes [x, y];

				if (g == null) {

				} else {
					WorldTile wt = g.GetComponent<WorldTile> ();
					if (wt == null) {

					} else {
						ThreadedWorldTile t = pathNodes [x, y];
						t.myNeighbours = new List<ThreadedWorldTile> ();
						foreach (WorldTile n in wt.myNeighbours) {
							t.myNeighbours.Add (pathNodes [n.gridX, n.gridY]);
						}
					}
				}
			}
		}

	}

	//THREADED VARIANTS

	public List<ThreadedWorldTile> getPath(int sX,int sY,int eX,int eY)
	{
		List<ThreadedWorldTile> list = new List<ThreadedWorldTile> ();
		findPath (sX, sY, eX, eY, ref list);
		return list;
	}

	public void findPath(int sX,int sY,int eX,int eY, ref List<ThreadedWorldTile> store)
	{

		//try{
		ThreadedWorldTile startNode = pathNodes[sX,sY]; //had a bug where the method would go through all the tiles in the grid causing a lag spike, just added a condititon to check for a nearby walkable tile, if its null after this it just abandons the path


		ThreadedWorldTile endNode = pathNodes[eX,eY];

	


		if (startNode.walkable == false) {
			startNode =  threaded_getNearestWalkableTiles (startNode.gridX, startNode.gridY);
		}

		if (endNode.walkable == false) {
			endNode =  threaded_getNearestWalkableTiles (endNode.gridX, endNode.gridY);
		}

		if (startNode == null || endNode == null) {
			return;
		}

	


		List<ThreadedWorldTile> openSet = new List<ThreadedWorldTile>();
		HashSet<ThreadedWorldTile> closedSet = new HashSet<ThreadedWorldTile>();
		openSet.Add(startNode);

		while (openSet.Count > 0) {
			ThreadedWorldTile node = openSet[0];

			for (int i = 1; i < openSet.Count; i ++) {
				if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost) {
					if (openSet[i].hCost < node.hCost)
						node = openSet[i];
				}
			}

			openSet.Remove(node);
			closedSet.Add(node);

			if (node == endNode) {
				threaded_RetracePath(startNode,endNode,ref store);
				return;
			}

			foreach (ThreadedWorldTile neighbour in node.myNeighbours) {
				
				if (!neighbour.walkable || closedSet.Contains(neighbour) || neighbour==null || node==null) {
					continue;
				}

				int newCostToNeighbour = node.gCost + GetDistance (node, neighbour) + node.getModifier ();
				if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
					neighbour.gCost = newCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, endNode);
					neighbour.parent = node;

					if (!openSet.Contains(neighbour))
						openSet.Add(neighbour);
				}
			}
		}

	}






	ThreadedWorldTile threaded_getNearestWalkableTiles(int gridX,int gridY)
	{
		ThreadedWorldTile invalidTile = pathNodes[gridX,gridY];

		foreach (ThreadedWorldTile t in invalidTile.myNeighbours) {
			if (t.walkable == true) {
				return t;
			}
		}

		return null;
	}


	void threaded_RetracePath(ThreadedWorldTile startNode,ThreadedWorldTile targetNode,ref List<ThreadedWorldTile> store)
	{
		List<ThreadedWorldTile> path = new List<ThreadedWorldTile>();
		ThreadedWorldTile currentNode = targetNode;

		while (currentNode != startNode) {
			path.Add(currentNode);

			currentNode = currentNode.parent;
		}
		path.Reverse();
		store = path;
	}

	int GetDistance(ThreadedWorldTile nodeA,ThreadedWorldTile nodeB)
	{
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
	}

	float tempModResetTimer = 5.0f;





}
