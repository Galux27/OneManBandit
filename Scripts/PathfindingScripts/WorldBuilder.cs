using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBuilder : MonoBehaviour {
	public static WorldBuilder me;
//	public GameObject tilePrefab,rock;
	public GameObject[,] worldTiles;
	public int width=0,height=0;
	public Vector3 bottomLeftCorner,topRightCorner;

	//public GameObject cliffBottom,cliffTop,cliffEdge,cliffBottomCorner,cliffTopCorner;
	public bool nodesCreated=false;
	public bool fromTilemap = false;
	void Awake()
	{
		me = this;
		if (fromTilemap == false) {
			worldTiles = new GameObject[width, height];
		} else {
			if (nodesCreated == false) {
				createNodes ();
			} else {
				CreateNodesFromTilemaps ct = FindObjectOfType<CreateNodesFromTilemaps> ();

				if (worldTiles == null) {
					Debug.Log ("There were no world tiles found");
					debugFixNodes ();
				}

				if (ct.nodes == null && worldTiles != null) {
					Debug.Log ("Create nodes from tilemaps had no nodes, passing in");
					ct.nodes = worldTiles;
				}

				Pathfinding p = FindObjectOfType<Pathfinding> ();
				if (p.pathNodes == null) {
					Debug.Log ("There were no threaded nodes");
				}
			}
		}
	}

	void debugFixNodes()
	{
		Debug.Log ("Had to fix nodes cause summit went wrong " + width + " | " + height);
		worldTiles = new GameObject[width+1, height+1];

		WorldTile[] wt = FindObjectsOfType<WorldTile> ();

		foreach (WorldTile w in wt) {
			//Debug.Log (w.gridX + " || " + w.gridY);
			worldTiles [w.gridX, w.gridY] = w.gameObject;
		}

		CreateNodesFromTilemaps.me.nodes = worldTiles;
		FindObjectOfType<Pathfinding> ().setThreadedNodeArray (worldTiles);
	}

	public void createNodes()
	{
		FindObjectOfType<CreateNodesFromTilemaps> ().generateNodes ();
		nodesCreated = true;
	}

	#if UNITY_EDITOR

	public void editorCreateNodes()
	{
		CreateNodesFromTilemaps cnt = FindObjectOfType<CreateNodesFromTilemaps> ();
		cnt.generateNodes ();
		nodesCreated = true;
	}

	#endif

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	List<WorldTile> getNeighbours(int x,int y)
	{


		List<WorldTile> myNeighbours = new List<WorldTile> ();
	//	WorldTile currentTile = worldTiles [x, y].GetComponent<WorldTile> ();
	//	currentTile.myNeighbours = new List<WorldTile> ();



		if (x > 0 && x < width-1) {
			//can get tiles on both left and right of the tile

			if (y > 0 && y < height - 1) {
				//top and bottom
				myNeighbours.Add(worldTiles[x+1,y].GetComponent<WorldTile>());
				myNeighbours.Add(worldTiles[x-1,y].GetComponent<WorldTile>());
				myNeighbours.Add(worldTiles[x,y+1].GetComponent<WorldTile>());
				myNeighbours.Add(worldTiles[x,y-1].GetComponent<WorldTile>());

			} else if (y == 0) {
				//just top
				myNeighbours.Add(worldTiles[x+1,y].GetComponent<WorldTile>());
				myNeighbours.Add(worldTiles[x-1,y].GetComponent<WorldTile>());
				myNeighbours.Add(worldTiles[x,y+1].GetComponent<WorldTile>());
			} else if (y == height - 1) {
				//just bottom
				myNeighbours.Add(worldTiles[x,y-1].GetComponent<WorldTile>());
				myNeighbours.Add(worldTiles[x+1,y].GetComponent<WorldTile>());
				myNeighbours.Add(worldTiles[x-1,y].GetComponent<WorldTile>());
			}


		} else if (x == 0) {
			//can't get tile on left
			if (y > 0 && y < height - 1) {
				//top and bottom
				myNeighbours.Add(worldTiles[x+1,y].GetComponent<WorldTile>());
				myNeighbours.Add(worldTiles[x,y-1].GetComponent<WorldTile>());
				myNeighbours.Add(worldTiles[x,y+1].GetComponent<WorldTile>());
			} else if (y == 0) {
				//just top
				myNeighbours.Add(worldTiles[x+1,y].GetComponent<WorldTile>());
				myNeighbours.Add(worldTiles[x,y+1].GetComponent<WorldTile>());
			} else if (y == height - 1) {
				//just bottom
				myNeighbours.Add(worldTiles[x+1,y].GetComponent<WorldTile>());
				myNeighbours.Add(worldTiles[x,y-1].GetComponent<WorldTile>());
			}
		} else if (x == width-1) {
			//can't get tile on right
			if (y > 0 && y < height - 1) {
				//top and bottom
				myNeighbours.Add(worldTiles[x-1,y].GetComponent<WorldTile>());
				myNeighbours.Add(worldTiles[x,y+1].GetComponent<WorldTile>());
				myNeighbours.Add(worldTiles[x,y-1].GetComponent<WorldTile>());
			} else if (y == 0) {
				//just top
				myNeighbours.Add(worldTiles[x-1,y].GetComponent<WorldTile>());
				myNeighbours.Add(worldTiles[x,y+1].GetComponent<WorldTile>());
			} else if (y == height - 1) {
				//just bottom
				myNeighbours.Add(worldTiles[x-1,y].GetComponent<WorldTile>());
				myNeighbours.Add(worldTiles[x,y-1].GetComponent<WorldTile>());
			}
		}


		return myNeighbours;
	}


	public WorldTile getNearest(Vector3 pos){
		WorldTile retVal = null;
		WorldTile fallback = null;
			int xInd = 0, yInd = 0;
			bool xDone = false, yDone = false;
		List<WorldTile> potentialAlternateTiles = new List<WorldTile> ();
			while (retVal == null) {
				WorldTile wt = worldTiles [xInd, yInd].GetComponent<WorldTile> ();

			if (Vector2.Distance (wt.transform.position, pos) < 3.0f) {
				potentialAlternateTiles.Add (wt);
			}

				if (wt.transform.position.y < pos.y) {
					yInd++;
				} else {
					yDone = true;
				}

				if (wt.transform.position.x < pos.x) {
					xInd++;
				} else {
					xDone = true;
				}

				if (xDone == true && yDone==true) {
					retVal = wt;
				fallback = wt;
				}
			}

			float dist = 99999999.0f;
			WorldTile altRet = null;

			if (retVal.walkable == false || retVal.myNeighbours.Count == 0 || retVal==null) {
				//List<WorldTile> nearby = new List<WorldTile> ();
				potentialAlternateTiles.Add(worldTiles [xInd+1, yInd].GetComponent<WorldTile> ());
				potentialAlternateTiles.Add(worldTiles [xInd-1, yInd].GetComponent<WorldTile> ());
				potentialAlternateTiles.Add(worldTiles [xInd, yInd+1].GetComponent<WorldTile> ());
				potentialAlternateTiles.Add(worldTiles [xInd, yInd-1].GetComponent<WorldTile> ());

				potentialAlternateTiles.Add(worldTiles [xInd+1, yInd+1].GetComponent<WorldTile> ());
				potentialAlternateTiles.Add(worldTiles [xInd-1, yInd-1].GetComponent<WorldTile> ());
				potentialAlternateTiles.Add(worldTiles [xInd-1, yInd+1].GetComponent<WorldTile> ());
				potentialAlternateTiles.Add(worldTiles [xInd+1, yInd-1].GetComponent<WorldTile> ());
			
				foreach(WorldTile t in potentialAlternateTiles)
				{
					if (t.walkable == false || t==null) {
						continue;
					}
					RaycastHit2D ray = Physics2D.Raycast (t.transform.position, (pos - t.transform.position).normalized, Vector2.Distance (t.transform.position, pos));
					if (ray.collider == null || ray.collider.GetComponent<PersonMovementController> () == true) {
						Debug.DrawRay (t.transform.position, pos - t.transform.position, Color.blue);

						float d = Vector3.Distance (t.gameObject.transform.position, pos);
						if (d < dist) {
							dist = d;
							altRet = t;
						}
					} else {
						Debug.DrawRay (t.transform.position, pos - t.transform.position, Color.green);

					}
				}

				if (altRet == null) {
				//altRet = getNearest(pos + 
					foreach(WorldTile t in potentialAlternateTiles)
					{
						if (t==null) {
							continue;
						}
						RaycastHit2D ray = Physics2D.Raycast (t.transform.position, (pos - t.transform.position).normalized, Vector2.Distance (t.transform.position, pos));

						if (ray.collider == null || ray.collider.GetComponent<PersonMovementController> () == true) {
							Debug.DrawRay (t.transform.position, pos - t.transform.position, Color.cyan);

							float d = Vector3.Distance (t.gameObject.transform.position, pos);
							if (d < dist) {
								dist = d;
								altRet = t;
							}
						} else {
							Debug.DrawRay (t.transform.position, pos - t.transform.position, Color.magenta);

						}
					}
				}
			retVal = altRet;


			}
		if (retVal == null) {
		//	Debug.LogError ("Had to go to fallback, returning " + fallback.gameObject.name);
			return fallback;
		} else {
			return retVal;
		}
	}

	public WorldTile findNearestWorldTile(Vector3 pos) //TODO add better way of getting nearest node
	{
		return getNearest (pos);
	}

	public ThreadedWorldTile threadedGetNearest(Vector3 pos){
		ThreadedWorldTile retVal = null;
		ThreadedWorldTile fallback;
		int xInd = 0, yInd = 0;
		bool xDone = false, yDone = false;
		while (retVal == null) {
			ThreadedWorldTile wt =  Pathfinding.me.pathNodes [xInd, yInd];
			if (wt.worldPos.y < pos.y) {
				yInd++;
			} else {
				yDone = true;
			}

			if (wt.worldPos.x < pos.x) {
				xInd++;
			} else {
				xDone = true;
			}

			if (xDone == true && yDone==true) {
				retVal = wt;
				fallback = wt;
			}
		}

		return retVal;
	}

	public ThreadedWorldTile findNearestThreadedWorldTile(Vector3 pos)
	{
		int xIndex = 0, yIndex = 0;

		ThreadedWorldTile t = threadedGetNearest (pos);
		//RoomScript r = LevelController.me.getRoomPosIsIn (pos);
		ThreadedWorldTile fallback = t;

		if (t.walkable == false) {
			Debug.Log ("Retval was not walkable");
			List<ThreadedWorldTile> otherTiles = new List<ThreadedWorldTile> ();
			otherTiles.Add (Pathfinding.me.pathNodes [xIndex + 1, yIndex]);
			otherTiles.Add (Pathfinding.me.pathNodes [xIndex - 1, yIndex]);
			otherTiles.Add (Pathfinding.me.pathNodes [xIndex , yIndex+ 1]);
			otherTiles.Add (Pathfinding.me.pathNodes [xIndex , yIndex- 1]);

			otherTiles.Add (Pathfinding.me.pathNodes [xIndex + 1, yIndex -1]);
			otherTiles.Add (Pathfinding.me.pathNodes [xIndex - 1, yIndex+1]);
			otherTiles.Add (Pathfinding.me.pathNodes [xIndex +1, yIndex+ 1]);
			otherTiles.Add (Pathfinding.me.pathNodes [xIndex -1, yIndex- 1]);

			List<ThreadedWorldTile> walkable = new List<ThreadedWorldTile> ();

			foreach(ThreadedWorldTile w in otherTiles){
				if (w.walkable == true) {
					walkable.Add (w);
				}
			}

			float nearest = 999999.0f;

			foreach(ThreadedWorldTile w in walkable){
				float d3 = Vector3.Distance (pos,w.worldPos);
				if (d3 < nearest) {
					nearest = d3;
					t = w;
				}
			}


		}


		float d = Vector3.Distance(t.worldPos,pos);

		foreach (ThreadedWorldTile w in t.myNeighbours) {
		//	RoomScript r2 = LevelController.me.getRoomPosIsIn (w.worldPos);
			if (w.walkable == true ) {
				float d2 = Vector3.Distance (w.worldPos, pos);
				if (d2 < d) {
					t = w;
				}
			}
		}

			return t;

	}

	public GameObject bottomLeftNode, topRightNode;

	public void passInTilemapPathfinding(GameObject[,] nodeArr, int highX,int highY)
	{
		worldTiles = nodeArr;
		width = highX;
		height = highY;
		bottomLeftNode = nodeArr [0, 0];
		topRightNode = null;//nodeArr  [nodeArr.GetLength(0)-1, nodeArr.GetLength(1)-1];
		float xPos = nodeArr [0, 0].transform.position.y;
		float yPos = nodeArr [0, 0].transform.position.y;
		foreach (GameObject g in nodeArr) {
			if (g == null) {
				Debug.Log ("There was a null space in the array...");
				continue;
			}


			if (g.transform.position.x > xPos || g.transform.position.y > yPos) {
				topRightNode = g;
				xPos = g.transform.position.x;
				yPos = g.transform.position.y;
			}


		}

		bottomLeftCorner = bottomLeftNode.transform.position;//nodeArr [0, 0].transform.position;
		topRightCorner = topRightNode.transform.position;//nodeArr [nodeArr.GetLength(0)-1, nodeArr.GetLength(1)-1].transform.position;
		FindObjectOfType<Pathfinding>().setThreadedNodeArray (worldTiles);
	}


}
