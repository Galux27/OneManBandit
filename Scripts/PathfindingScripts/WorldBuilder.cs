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
	public bool nodesCreated=true;
	public bool fromTilemap = false;
	void Awake()
	{
		me = this;

        if(FindObjectOfType<CreateNodesFromTilemaps>().unsortedNodes==null)
        {
            nodesCreated = false;
        }

		if (fromTilemap == false) {
			worldTiles = new GameObject[width, height];
		} else {
			if (nodesCreated == false) {
                //Debug.LogError("Nodes are not created");
				createNodes ();
			} else {
                //Debug.LogError("Nodes already created, seeing if we can use");
				CreateNodesFromTilemaps ct = FindObjectOfType<CreateNodesFromTilemaps> ();
                worldTiles = ct.reassembleNodeArray();
                ct.nodes = worldTiles;
               for(int x= 0; x < worldTiles.GetLength(0); x++)
                {
                    for(int y = 0; y < worldTiles.GetLength(1); y++)
                    {
                        if (worldTiles[x, y] != null)
                        {
                            startX = x;
                            startY = y;
                            return;
                        }
                    }
                }

                /*if (worldTiles == null) {
					//Debug.Log ("There were no world tiles found");
					//DebugFixNodes ();
				}

				if (ct.nodes == null && worldTiles != null) {
					//Debug.Log ("Create nodes from tilemaps had no nodes, passing in");
					ct.nodes = worldTiles;
				}

				Pathfinding p = FindObjectOfType<Pathfinding> ();
				if (p.pathNodes == null) {
					//Debug.Log ("There were no threaded nodes");
				}*/
            }
        }
	}

    private void Start()
    {
        Pathfinding p = FindObjectOfType<Pathfinding>();
        p.setThreadedNodeArray(worldTiles);
    }

    void Update()
	{
		if (Input.GetKeyDown (KeyCode.Y)) {
			for (int x = 0; x < worldTiles.GetLength (0); x++) {
				for (int y = 0; y < worldTiles.GetLength (1); y++) {
					if (worldTiles [x, y] == null) {
						//Debug.Log ("World tile at " + x.ToString () + " || " + y.ToString () + " was null");
					} else {
						//Debug.Log ("World tile at " + x.ToString () + " || " + y.ToString () + " was at " + worldTiles [x, y].transform.position);
					}
				}
			}
		}
	}


	/// <summary>
	/// Method that fixes an issue with baking the pathfinding nodes in the editor, basicly just double checks that they are assigned in the grid.
	/// </summary>
	void DebugFixNodes()
	{
		//Debug.Log ("Had to fix nodes cause summit went wrong " + width + " | " + height);
		worldTiles = new GameObject[width+1, height+1];

		try{
			worldTiles = FindObjectOfType<CreateNodesFromTilemaps> ().nodes;
			//Debug.Log("Found " + worldTiles.Length + " nodes from tilemaps");
		}catch{
			//Debug.Log ("Error getting nodes from tilemaps, recreating");
			WorldTile[] wt = FindObjectsOfType<WorldTile> ();

			foreach (WorldTile w in wt) {
				if (w == null || worldTiles [w.gridX, w.gridY] ==null) {
					continue;
				}
				w.gameObject.transform.position = new Vector3 (w.transform.position.x, w.transform.position.y, 0);
				////Debug.Log (w.gridX + " || " + w.gridY);
				worldTiles [w.gridX, w.gridY] = w.gameObject;
				if (w.isOptimised == true) {
					worldTiles [w.gridX+1, w.gridY] = w.gameObject;
					worldTiles [w.gridX, w.gridY+1] = w.gameObject;
					worldTiles [w.gridX+1, w.gridY+1] = w.gameObject;

				}
			}
			CreateNodesFromTilemaps.me.nodes = worldTiles;
		}
		if (worldTiles == null || worldTiles.Length == 0) {
			//Debug.Log ("Couldn't find nodes from tilemaps, recreating");
			WorldTile[] wt = FindObjectsOfType<WorldTile> ();

			foreach (WorldTile w in wt) {
				////Debug.Log (w.gridX + " || " + w.gridY);
				worldTiles [w.gridX, w.gridY] = w.gameObject;
				if (w.isOptimised == true) {
					worldTiles [w.gridX+1, w.gridY] = w.gameObject;
					worldTiles [w.gridX, w.gridY+1] = w.gameObject;
					worldTiles [w.gridX+1, w.gridY+1] = w.gameObject;

				}
			}
			CreateNodesFromTilemaps.me.nodes = worldTiles;
		}
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
        cnt.editorCreateNodes();
		nodesCreated = true;
	}

#endif
    int startY = 0, startX = 0;


    public WorldTile getNearest(Vector3 pos){

		WorldTile retVal = null;
        
        float distance = 9999999.0f;



        int xInd = -1, yInd = -1;
        for(int x =startX;x<worldTiles.GetLength(0)-1&&xInd<0;x++)
        {
            if (worldTiles[x, 0] == null)
            {
                continue;
            }
            else
            {
                if (worldTiles[x, 0].transform.position.x >= pos.x)
                {
                    xInd = x;
                    break;
                }
            }
        }


        for (int y = startY; y < worldTiles.GetLength(1)-1 && yInd<0; y++)
        {
            if (worldTiles[xInd, y] == null)
            {
                continue;
            }
            else
            {
                if (worldTiles[xInd, y].transform.position.y >= pos.y)
                {
                    yInd = y;
                    break;
                }
            }
        }

       // //Debug.LogError("Xind = " + xInd.ToString() + " Yind = " + yInd.ToString() + " Lengths: " + worldTiles.GetLength(0) + " || " + worldTiles.GetLength(1));

        if(xInd>=worldTiles.GetLength(0)|| yInd>=worldTiles.GetLength(1) ||xInd<0 || yInd<0 || worldTiles[xInd,yInd]==null)
        {
            GameObject nearest = null;
            float dist = 9999999.0f;
            for(int x = xInd - 5; x < xInd + 5; x++)
            {
                if (x < 0 || x > worldTiles.GetLength(0))
                {
                    continue;
                }
                for(int y = yInd-5;y<yInd+5;y++)
                {
                    if (y < 0 || y > worldTiles.GetLength(1))
                    {
                        continue;
                    }
                    if (worldTiles[x, y] != null)
                    {
                        float d = Vector2.Distance(worldTiles[x, y].transform.position, pos);
                        if (d < dist)
                        {
                            dist = d;
                            nearest = worldTiles[x, y];
                        }
                    }
                }
            }
            retVal = nearest.GetComponent<WorldTile>();
        }
        else
        {
            retVal = worldTiles[xInd, yInd].GetComponent<WorldTile>();

        }

        return retVal;
		/*float distance = 9999999.0f;


		WorldTile fallback = null;
		int xInd = 0, yInd = 0;
		bool xDone = false, yDone = false;
		List<WorldTile> potentialAlternateTiles = new List<WorldTile> ();
		bool cantFind = false;

		float xDif = 99999.0f;
		float yDif = 99999.0f;

		while (retVal == null) {
			for (int x = 0; x < worldTiles.GetLength (0); x++) {

				if(worldTiles [x,0]==null){
					continue;
				}

				float diff = Mathf.Abs (worldTiles [x, 0].transform.position.x - pos.x);

				if (diff < xDif) {
					xDif = diff;
					xInd = x;
				}
			}

			for (int y = 0; y < worldTiles.GetLength (1); y++) {
				if(worldTiles [0,y]==null){
					continue;
				}

				float diff = Mathf.Abs (worldTiles [0,y].transform.position.y - pos.y);

				if (diff < yDif) {
					yDif = diff;
					yInd = y;
				}
			}

			retVal = worldTiles [xInd, yInd].GetComponent<WorldTile> ();
            */
			/*if (worldTiles [xInd, yInd] == null) {
				if (xInd < worldTiles.GetLength (0)) {
					xInd++;
				} else {
					if (yInd < worldTiles.GetLength (1)) {
						yInd++;
					} else {
						cantFind=true;
						break;
					}
					xInd = 0;
				}
			} else {
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

				if (xDone == true && yDone == true) {
					retVal = wt;
					fallback = wt;
				}
			}*/
		//}



		/*if (retVal.GetComponent<WorldTile> ().walkable == false) {
			distance = Vector2.Distance (retVal.transform.position, pos);
			for (int x = retVal.gridX - 3; x < retVal.gridX + 3; x++) {
				for (int y = retVal.gridY - 3; y < retVal.gridY + 3; y++) {
					if (x < 0 || x > worldTiles.GetLength (0) || y < 0 || y > worldTiles.GetLength (1)) {
						continue;
					} else {
						if (worldTiles [x, y] == null) {
							continue;
						}

						WorldTile wt2 = worldTiles [x, y].GetComponent<WorldTile> ();
						if (wt2.walkable == true) {
							float d2 = Vector2.Distance (wt2.gameObject.transform.position, pos);
							if (d2 < distance) {
								distance = d2;
								retVal = wt2;
							}
						}
					}
				}
			}
		}*/
			/*} else {
			//Debug.LogError ("Can't get normally, bruteforcing getting nearest node");
			foreach (GameObject g in worldTiles) {
				if (g == null) {
					continue;
				}

				WorldTile wt = g.GetComponent<WorldTile> ();
				if (wt.walkable == true) {
					float d = Vector2.Distance (pos, wt.gameObject.transform.position);
					if (d < distance) {
						retVal = wt;
						distance = d;
					}
				}
				if (distance < 2) {
					break;
				}
			}
		}*/
		//return retVal;
			//float dist = 99999999.0f;
			//WorldTile altRet = null;

			/*if (retVal.walkable == false || retVal.myNeighbours.Count == 0 || retVal==null) {
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
						//Debug.DrawRay (t.transform.position, pos - t.transform.position, Color.blue);

						float d = Vector3.Distance (t.gameObject.transform.position, pos);
						if (d < dist) {
							dist = d;
							altRet = t;
						}
					} else {
						//Debug.DrawRay (t.transform.position, pos - t.transform.position, Color.green);

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
							//Debug.DrawRay (t.transform.position, pos - t.transform.position, Color.cyan);

							float d = Vector3.Distance (t.gameObject.transform.position, pos);
							if (d < dist) {
								dist = d;
								altRet = t;
							}
						} else {
							//Debug.DrawRay (t.transform.position, pos - t.transform.position, Color.magenta);

						}
					}
				}
			retVal = altRet;


			}
		if (retVal == null) {
		//	//Debug.LogError ("Had to go to fallback, returning " + fallback.gameObject.name);
			return fallback;
		} else {
			return retVal;
		}*/
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


	public GameObject bottomLeftNode, topRightNode;


	void DebugNodes()
	{
		foreach (GameObject g in worldTiles) {
			if (g == null) {
				//Debug.Log ("Found null in pathfinding array");
			}
		}
	}
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
			//	//Debug.Log ("There was a null space in the array...");
				continue;
			}


			if (g.transform.position.x > xPos || g.transform.position.y > yPos) {
				topRightNode = g;
				xPos = g.transform.position.x;
				yPos = g.transform.position.y;
			}


		}
		////DebugNodes ();
		bottomLeftCorner = bottomLeftNode.transform.position;//nodeArr [0, 0].transform.position;
		topRightCorner = topRightNode.transform.position;//nodeArr [nodeArr.GetLength(0)-1, nodeArr.GetLength(1)-1].transform.position;
		FindObjectOfType<Pathfinding>().setThreadedNodeArray (worldTiles);
	}


}
