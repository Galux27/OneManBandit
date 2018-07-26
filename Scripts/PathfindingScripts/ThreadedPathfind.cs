using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
public class ThreadedPathfind  {
	private object myLock = new object();
	bool isDone = false;
	Vector3Int posToGoTo,posFrom;
	List<ThreadedWorldTile> finalPath;
	PathFollower wantedPath;
	Thread myThread;
	public void initialise(Vector3Int to,Vector3Int start,PathFollower wanted)
	{
		posToGoTo = to;
		posFrom = start;
		wantedPath = wanted;
		finalPath = new List<ThreadedWorldTile> ();
	}

	public bool IsDone
	{
		get{
			bool tmp;
			lock (myLock) {
				tmp = isDone;
			}
			return tmp;
		}
		set{
			lock (myLock) {
				isDone = value;
			}
		}
	}

	public void Start()
	{
		IsDone = false;
		//if (myThread == null) {
			
		//}

		myThread = new Thread (Run);
		myThread.IsBackground = true;
		myThread.Start ();
		//myThread.Join ();
	}

	public void Stop()
	{
		myThread.Abort ();
	}

	public bool Update()
	{
		if (IsDone) {
			OnFinished ();
			return true;
		}
		return false;
	}
		
	void getPath()
	{
		//finalPath = Pathfinding.me.getPath(posFrom.x,posFrom.y,posToGoTo.x,posToGoTo.y);
		List<ThreadedWorldTile> list = new List<ThreadedWorldTile> ();
		//Pathfinding.me.findPath (s.gridX, s.gridY, e.gridX, e.gridY, ref list);
		//setNewPath (list);

		findPath(posFrom.x,posFrom.y,posToGoTo.x,posToGoTo.y,ref list);
		finalPath = list;
		wantedPath.setNewPath (finalPath,(Vector3)posFrom);

	}

	public void Run()
	{
		//get the path
		getPath();
		IsDone = true;
	}

	public void OnFinished()
	{
		//finalPath.Reverse ();
		//wantedPath.setNewPath (finalPath);

	}

	public void findPath(int sX,int sY,int eX,int eY, ref List<ThreadedWorldTile> store)
	{

		//try{
		ThreadedWorldTile startNode =  Pathfinding.me.pathNodes[sX,sY]; //had a bug where the method would go through all the tiles in the grid causing a lag spike, just added a condititon to check for a nearby walkable tile, if its null after this it just abandons the path

		/*	if (startPos.GetComponent<ThreadedWorldTile> () == true) {
			startNode = startPos.GetComponent<ThreadedWorldTile> ();
		} else {
			startNode = WorldBuilder.me.findNearestThreadedWorldTile (startPos.transform.position);
		}*/

		//= WorldBuilder.me.findNearestThreadedWorldTile (startPos.transform.position); //WorldBuilder.me.tileFromWorldPos (WorldBuilder.me.findNearestThreadedWorldTile( startPos.transform.position));
		////////Debug.Log(startNode.name);

		ThreadedWorldTile endNode = Pathfinding.me.pathNodes[eX,eY];

		/*if (endPos.GetComponent<ThreadedWorldTile> () == true) {
			endNode = endPos.GetComponent<ThreadedWorldTile> ();
		} else {
			endNode = WorldBuilder.me.findNearestThreadedWorldTile (endPos.transform.position);
		}*/


		if (startNode.walkable == false) {
			startNode =  threaded_getNearestWalkableTiles (startNode.gridX, startNode.gridY);
		}

		if (endNode.walkable == false) {
			endNode =  threaded_getNearestWalkableTiles (endNode.gridX, endNode.gridY);
		}

		if (startNode == null || endNode == null) {
			////////Debug.LogError ("One of the nodes was null, can't get path");
			return;
		}

		// =WorldBuilder.me.findNearestThreadedWorldTile( endPos.transform.position);// WorldBuilder.me.tileFromWorldPos (endPos.transform.position);
		//ThreadedWorldTile startNode = .NodeFromWorldPoint(startPos);
		//Node targetNode = grid.NodeFromWorldPoint(targetPos);


		List<ThreadedWorldTile> openSet = new List<ThreadedWorldTile>();
		HashSet<ThreadedWorldTile> closedSet = new HashSet<ThreadedWorldTile>();
		openSet.Add(startNode);

		while (openSet.Count > 0) {
			ThreadedWorldTile node = openSet[0];
			////////Debug.Log ("Using thread to try and find path " + node.worldPos.ToString());

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

				int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
				if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
					neighbour.gCost = newCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, endNode);
					neighbour.parent = node;

					if (!openSet.Contains(neighbour))
						openSet.Add(neighbour);
				}
			}
		}
		//catch{
		//	//////Debug.LogError ("Something Went Wrong with the pathfinding");
		//}
	}






	ThreadedWorldTile threaded_getNearestWalkableTiles(int gridX,int gridY)
	{
		ThreadedWorldTile invalidTile =  Pathfinding.me.pathNodes[gridX,gridY];

		foreach (ThreadedWorldTile t in invalidTile.myNeighbours) {
			if (t.walkable == true) {
				return t;
			}
		}

		return null;
	}


	void threaded_RetracePath(ThreadedWorldTile startNode,ThreadedWorldTile targetNode,ref List<ThreadedWorldTile> store)
	{
		//////Debug.Log ("Retracing threaded path length: " + store.Count);
		List<ThreadedWorldTile> path = new List<ThreadedWorldTile>();
		ThreadedWorldTile currentNode = targetNode;

		while (currentNode != startNode) {
			path.Add(currentNode);
			//if (highlightPathFound == true) {
				//currentNode.gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
			//}
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
}
