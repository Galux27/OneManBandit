using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadedPathfindJob  {
	public GameObject target;
	public PathFollower returnTo;
	public List<Vector3> returnedPath=new List<Vector3>();
	public int sX, sY, fX, fY;
	public bool jobIsDone=false;
	public ThreadedPathfindJob(GameObject g,PathFollower r){
		this.target = g;
		returnTo = r;
		WorldTile wt = WorldBuilder.me.findNearestWorldTile (r.gameObject.transform.position);
		WorldTile wt2 = WorldBuilder.me.findNearestWorldTile (g.transform.position);
		sX = wt.gridX;
		sY = wt.gridY;

		//if (wt2 == null || g == null) {
		//	Debug.Log ("Return to " + returnTo.gameObject.name + " got the path error");
		//	Debug.Break ();
		//}

		fX = wt2.gridX;
		fY = wt2.gridY;
	}

	public void returnPath()
	{
		if (returnTo == null) {

		} else {
			returnTo.setNewPath (returnedPath);
		}
		jobIsDone = true;
	}

	public void doJob()
	{
		List<ThreadedPathfindNode> nodes = ThreadedPathfindInterface.me.getPath ();
		getPathAsVectors (nodes);
//		//////Debug.Log ("Nodes in unconverted path " + nodes.Count);
		returnPath ();
	}

	public void getPathAsVectors(List<ThreadedPathfindNode> nodes)
	{
		foreach (ThreadedPathfindNode n in nodes) {
			returnedPath.Add (n.worldPos);
		}
		returnedPath.Reverse ();
	}

}
