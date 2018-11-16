using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for pathfinding nodes from when the pathfinding wasn't multithreaded, now used for more gameplay stuff. 
/// </summary>
public class WorldTile : MonoBehaviour {
	public int gCost;
	public int hCost;
	public int gridX, gridY;
	public bool walkable=true;
	public List<WorldTile> myNeighbours;
	public WorldTile parent;
	public int modifier = 10;
	public int tempModifiers = 0;
	public bool isOptimised=false;
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

/// <summary>
/// Class for multithreaded pathfinding node, identical to WorldTile but modified so it uses nothing unity specific so it can run on a seperate thread. 
/// </summary>
public class ThreadedWorldTile
{

	public int gCost;
	public int hCost;
	public int gridX, gridY;
	public bool walkable=true;
	public List<ThreadedWorldTile> myNeighbours;
	public ThreadedWorldTile parent;
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
