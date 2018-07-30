using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for the threaded pathfinding nodes 
/// </summary>
public class ThreadedPathfindNode  {

	public int gCost;
	public int hCost;
	public int gridX, gridY;
	public bool walkable=true;
	public List<ThreadedPathfindNode> myNeighbours;
	public ThreadedPathfindNode parent;
	public Vector3 worldPos;
	public int modifier = 10;
	public int tempModifiers = 0;
	public int getModifier()
	{
		return modifier + tempModifiers;
	}
	public int fCost
	{
		get{
			return gCost + hCost;
		}
	}
}
