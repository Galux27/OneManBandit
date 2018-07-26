using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTile : MonoBehaviour {
	public int gCost;
	public int hCost;
	public int gridX, gridY;
	public bool walkable=true;
	public List<WorldTile> myNeighbours;
	public WorldTile parent;
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
