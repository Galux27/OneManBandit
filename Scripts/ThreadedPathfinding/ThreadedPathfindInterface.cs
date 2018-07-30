using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

/// <summary>
/// This class acts as an interface for the threaded pathfinding and NPCs that want a path
/// works by having NPCs pass in their PathFollower script and a location they want to go, this creates a threaded pathfind job and gets added to a list which is done one at a time until the list is empty 
/// </summary>
public class ThreadedPathfindInterface : MonoBehaviour {
	public static ThreadedPathfindInterface me;
	public List<ThreadedPathfindJob> jobsToDo;
	public ThreadedPathfindJob currentJob,currentJobT2;
	public Thread t,t2;

	public ThreadedPathfindNode[,] nodes;


	//Variables below this are just for displaying in the editor to see what object is asking for paths, not actually used to calculate them
	public int numberOfJobsToDo=0;
	public string currentJobTarget="",latestJobRequestedBy="";
	public float timeLastPathTook = 0.0f;
	public int lengthOfLastPath=0;
	float timePathStarted=0.0f;
	public GameObject latestRequestedBy,currentPathRequested;



	void Awake()
	{
		if (me == null) {
			me = this;
		}
		jobsToDo = new List<ThreadedPathfindJob> ();
	}


	/// <summary>
	/// When NPCs are destroyed if they have a path request then it gets removed here. 
	/// </summary>
	void removeNonNeededPaths()
	{
		foreach (ThreadedPathfindJob tp in jobsToDo) {
			if (tp.returnTo == null) {
				jobsToDo.Remove (tp);

				return;
			}
		}
	}


	// Update is called once per frame
	void Update () {
		removeNonNeededPaths ();
		if (countTime == true) {
			timer += Time.deltaTime;
		}

		if (currentJob == null) {

		} else {
			if (currentJob.returnTo == null) {
				jobsToDo.Remove (currentJob);
				return;
			}

			currentJobTarget = currentJob.returnTo.gameObject.name;
		}

		if (jobsToDo.Count > 0) {
			if (jobsToDo [jobsToDo.Count - 1].returnTo == null) {

			} else {
				latestRequestedBy = jobsToDo [jobsToDo.Count - 1].returnTo.gameObject;
				latestJobRequestedBy = jobsToDo [jobsToDo.Count - 1].returnTo.gameObject.name;
			}
		}

		numberOfJobsToDo = jobsToDo.Count;
	}



	void LateUpdate(){
		if (nodes == null) {
			initialiseNodes ();
		}
		doStuff ();
	}


	ThreadedPathfindJob getJobToDo()
	{
		foreach (ThreadedPathfindJob tp in jobsToDo) {
			if (tp.returnTo.myController.npcB.myType != AIType.civilian) {
				return tp;
			}
		}
		return jobsToDo [0];
	}
	public bool countTime=false;
	float timer = 0.0f;
	void doStuff()
	{
		if (jobsToDo.Count > 0) {
			if (currentJob == null) {
				currentJob = getJobToDo ();
				timePathStarted = Time.time;
				jobsToDo.Remove (currentJob);
				t = new Thread (currentJob.doJob);
				t.Start ();

			} else {
				if (currentJob.jobIsDone == true) {
					if (currentJob.returnTo == null) {

					} else {
						lengthOfLastPath = currentJob.returnedPath.Count;
						currentPathRequested = currentJob.returnTo.gameObject;
					}
					countTime = false;
					timeLastPathTook = Time.time - timePathStarted;
					timePathStarted = 0.0f;
					timer = 0.0f;


					currentJob = null;
				}
			}
		} else {
			if (currentJob == null) {
				countTime = false;

			} else {
				if (currentJob.jobIsDone == true) {
					if (currentJob.returnTo == null) {
					}else{
						lengthOfLastPath = currentJob.returnedPath.Count;
						currentPathRequested = currentJob.returnTo.gameObject;
					}
					countTime = false;
					timeLastPathTook = Time.time - timePathStarted;
					timePathStarted = 0.0f;
					timer = 0.0f;
					////////Debug.Log ("Current Job is Done, moving to next");
					/// 
					/// 
					if (timeLastPathTook > 1.0f) {
						//Debug.Break ();
					}
					currentJob = null;
				}
			}
		}


	
	}

	/// <summary>
	/// Creates the threaded pathfinding nodes based on the non threaded nodes taken from the tilemaps 
	/// </summary>
	void initialiseNodes()
	{
		WorldBuilder wb = FindObjectOfType<WorldBuilder> ();
		nodes = new ThreadedPathfindNode[WorldBuilder.me.worldTiles.GetLength (0), WorldBuilder.me.worldTiles.GetLength (1)];
		for (int x = 0; x < nodes.GetLength (0); x++) {
			for (int y = 0; y < nodes.GetLength (1); y++) {
//				//////Debug.Log(nodes.GetLength(0));
				if (CreateNodesFromTilemaps.me.nodes [x, y] == null) {
				} else {

					WorldTile wt = CreateNodesFromTilemaps.me.nodes [x, y].GetComponent<WorldTile> ();
					nodes [x, y] = new ThreadedPathfindNode ();
					nodes [x, y].gridX = x;
					nodes [x, y].gridY = y;
					nodes [x, y].walkable = wt.walkable;
					nodes [x, y].worldPos = wt.gameObject.transform.position;
					nodes [x, y].modifier = wt.modifier;

				}
			}
		}

		for (int x = 0; x < nodes.GetLength (0); x++) {
			for (int y = 0; y < nodes.GetLength (1); y++) {
				if (CreateNodesFromTilemaps.me.nodes [x, y] == null) {
				} else {
					WorldTile wt = CreateNodesFromTilemaps.me.nodes [x, y].GetComponent<WorldTile> ();
					nodes [x, y].myNeighbours = new List<ThreadedPathfindNode> ();
					foreach (WorldTile w in wt.myNeighbours) {
						nodes [x, y].myNeighbours.Add (nodes [w.gridX, w.gridY]);
					}
				}
			}
		}
	}

	void OnDestroy()
	{
		if (t == null) {

		} else {
			t.Abort ();
		}
	}

	public List<ThreadedPathfindNode> getPath()
	{
		List<ThreadedPathfindNode> foundPath = new List<ThreadedPathfindNode> ();

		List<ThreadedPathfindNode> openSet = new List<ThreadedPathfindNode> ();
		HashSet<ThreadedPathfindNode> closedSet = new HashSet<ThreadedPathfindNode> ();
		openSet.Add (nodes [currentJob.sX, currentJob.sY]);
		while (openSet.Count > 0) {
			ThreadedPathfindNode currentNode = openSet [0];

			for (int i = 0; i < openSet.Count; i++) {
				if (openSet [i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)) {
					if (currentNode.Equals (openSet [i]) == false) {
						currentNode = openSet [i];
					}
				}
			}

			openSet.Remove (currentNode);
			closedSet.Add (currentNode);

			if (currentNode.Equals (nodes [currentJob.fX, currentJob.fY])) {
				foundPath = retracePath (nodes [currentJob.sX, currentJob.sY], currentNode);
				break;
			}

			foreach (ThreadedPathfindNode neighbour in currentNode.myNeighbours) {
				if (neighbour.walkable == false && neighbour!=nodes [currentJob.fX, currentJob.fY] || closedSet.Contains (neighbour) || neighbour == null ) {
					continue;
				}

				//took out modifier
				int newMoveCost = currentNode.gCost + GetDistance (currentNode, neighbour) + neighbour.getModifier();

				if (newMoveCost < neighbour.gCost || openSet.Contains (neighbour) == false) {
					neighbour.gCost = newMoveCost;
					neighbour.hCost = GetDistance (neighbour, nodes [currentJob.fX, currentJob.fY]);
					neighbour.parent = currentNode;

					if (openSet.Contains (neighbour) == false) {
						openSet.Add (neighbour);
					}
				}
			}
		}
		return foundPath;

	}

	private List<ThreadedPathfindNode> retracePath(ThreadedPathfindNode start,ThreadedPathfindNode end)
	{
		List<ThreadedPathfindNode> foundPath = new List<ThreadedPathfindNode> ();
		ThreadedPathfindNode currentNode = end;
		while (currentNode != start) {

			currentNode.gCost = 0;
			foundPath.Add (currentNode);
			currentNode = currentNode.parent;
		}

		foundPath.Reverse ();
		return foundPath;
	}

	int GetDistance(ThreadedPathfindNode nodeA,ThreadedPathfindNode nodeB)
	{
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
	}

}
