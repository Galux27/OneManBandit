using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
public class ThreadedPathfindInterface : MonoBehaviour {
	public static ThreadedPathfindInterface me;
	public List<ThreadedPathfindJob> jobsToDo;
	public ThreadedPathfindJob currentJob,currentJobT2;
	public Thread t,t2;

	public ThreadedPathfindNode[,] nodes;
	public int numberOfJobsToDo=0;
	public string currentJobTarget="",latestJobRequestedBy="";
	public float timeLastPathTook = 0.0f;
	public int lengthOfLastPath=0;
	float timePathStarted=0.0f;
	public GameObject latestRequestedBy,currentPathRequested;
	public List<string> editorPathDisplay;

	void Debug_ShowPaths()
	{
		if (Application.isEditor) {
			editorPathDisplay = new List<string> ();
			foreach (ThreadedPathfindJob tp in jobsToDo) {
				if (tp.returnTo == null) {
					continue;
				}
				editorPathDisplay.Add (tp.returnTo.gameObject.name + " to " + tp.target.ToString ());
			}
		}
	}

	void Awake()
	{
		if (me == null) {
			me = this;
		}
		jobsToDo = new List<ThreadedPathfindJob> ();
	}

	// Use this for initialization
	void Start () {
		
	}

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
		Debug_ShowPaths ();
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

	void setModifiersForThreadedNodes()
	{
		for (int x = 0; x < nodes.GetLength (0); x++) {
			for (int y = 0; y < nodes.GetLength (1); y++) {

			}
		}
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
				//countTime = true;
				timePathStarted = Time.time;
				jobsToDo.Remove (currentJob);
				////////Debug.Log ("Doing Job Path To " + currentJob.target.transform.position + " For " + currentJob.returnTo.gameObject.name);
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
					////////Debug.Log ("Current Job is Done, moving to next");
					/// 
					/// 
					if (timeLastPathTook > 1.0f) {
					//	Debug.Break ();
					}
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


		/*if (jobsToDo.Count > 0) {
			if (currentJob == null) {
				Debug.Log ("Using thread 1 to get path");

				currentJob = jobsToDo [0];
				jobsToDo.Remove (currentJob);
				////////Debug.Log ("Doing Job Path To " + currentJob.target.transform.position + " For " + currentJob.returnTo.gameObject.name);
				t = new Thread (currentJob.doJob);
				t.Start ();

			} else {
				if (currentJob.jobIsDone == true) {
					Debug.Log ("Thread 1 is done with path");

					////////Debug.Log ("Current Job is Done, moving to next");
					currentJob = null;
				}
			}
		}

		if (jobsToDo.Count > 0) {
			if (currentJobT2 == null) {
				Debug.Log ("Using thread 2 to get path");
				currentJobT2 = jobsToDo [0];
				jobsToDo.Remove (currentJobT2);
				t2 = new Thread (currentJobT2.doJob);
				t2.Start ();
			} else {
				if (currentJobT2.jobIsDone == true) {
					currentJobT2 = null;
					Debug.Log ("Thread 2 is done with path");
									
				}
			}
		}*/
	}

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
		////////Debug.Log ("Going from " + nodes [currentJob.sX, currentJob.sY].worldPos + " to " + nodes [currentJob.fX, currentJob.fY]);
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
			////////Debug.Log ("Current node has " + currentNode.myNeighbours.Count);

			if (currentNode.Equals (nodes [currentJob.fX, currentJob.fY])) {
				////////Debug.Log ("Found path, returning from thread, length was " + foundPath.Count);
				foundPath = retracePath (nodes [currentJob.sX, currentJob.sY], currentNode);
				break;
			}

			foreach (ThreadedPathfindNode neighbour in currentNode.myNeighbours) {
				//!neighbour.walkable && neighbour.worldPos != nodes [currentJob.fX, currentJob.fY].worldPos ||
				if (neighbour.walkable == false && neighbour!=nodes [currentJob.fX, currentJob.fY] || closedSet.Contains (neighbour) || neighbour == null ) {
					continue;
				}

			//	if (currentNode.walkable == false) {
				//	if (neighbour.walkable == false) {
					//	continue;
				//	}
				//}

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
		////////Debug.Log ("INTHREAD Found path with " + foundPath.Count);
		return foundPath;

	}

	private List<ThreadedPathfindNode> retracePath(ThreadedPathfindNode start,ThreadedPathfindNode end)
	{
		List<ThreadedPathfindNode> foundPath = new List<ThreadedPathfindNode> ();
		ThreadedPathfindNode currentNode = end;
		while (currentNode != start) {
			////////Debug.Log ("Retracing path " + currentNode.worldPos);

			currentNode.gCost = 0;
			foundPath.Add (currentNode);
			currentNode = currentNode.parent;
		}
		////////Debug.Log ("Retracing path was " + foundPath.Count);

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
