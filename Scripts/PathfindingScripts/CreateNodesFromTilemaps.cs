using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class CreateNodesFromTilemaps : MonoBehaviour {
	//did some stuff to the actions in npc so they can get closer to the Nodes without the glitchyness
	public static CreateNodesFromTilemaps me;
	//changed execution order for this and world builder
	public Grid gridBase;
	public Tilemap floor;//floor of world
	public List<Tilemap> obstacleLayers,weightIncreaseLayers; //all layers that contain objects to navigate around
	public GameObject nodePrefab;

	public Tilemap cover,walls;

	//these are the bounds of where we are searching in the world for tiles, have to use world coords to check for tiles in the tile map
	public int scanStartX=-250,scanStartY=-250,scanFinishX=250,scanFinishY=250;


	public List<GameObject> unsortedNodes;//all the nodes in the world
	public GameObject[,] nodes; //sorted 2d array of nodes, may contain null entries if the map is of an odd shape e.g. gaps

	public bool createNodesOnAwake=false;
	public GameObject wallColliderPrefab;
	// Use this for initialization
	void Awake () {
		unsortedNodes = new List<GameObject> ();
		me = this;
		////////Debug.Log ("Floor is size "+floor.size);
		//foreach (Tilemap t in obstacleLayers) {
		//	//////Debug.Log ("Obstacle " + t.name + " Is size " + t.size);
		//}
		//if (createNodesOnAwake == true) {
		//	generateNodes ();
		//}
	}

	void Start()
	{
		
	}

	public void generateNodes()
	{

		createNodes ();
		FindObjectOfType<WorldBuilder>().passInTilemapPathfinding (nodes, gridBoundX, gridBoundY);
	}

	// Update is called once per frame
	void Update () {
		
	}
	public 	int gridBoundX = 0, gridBoundY = 0;

	void createWallColliders()
	{
		GameObject colliderParent = (GameObject)Instantiate (new GameObject (),Vector3.zero, Quaternion.Euler (0, 0, 0));
		colliderParent.name = "Wall collider parent";
		for (int x = scanStartX; x < scanFinishX; x++) {
			for (int y = scanStartY; y < scanFinishY; y++) {
				TileBase tb = walls.GetTile (new Vector3Int (x, y, 0)); //check if we have a floor tile at that world coords

				if (tb == null) {

				} else {
					GameObject g = (GameObject) Instantiate (wallColliderPrefab, new Vector3 (x+0.5f, y+0.5f, 0), Quaternion.Euler (0, 0, 0));
					g.transform.parent = colliderParent.transform;
					g.name = "Wall Collider " + g.transform.position.ToString ();
				}

			}
		}
	}

	void createNodes()
	{
		int gridX = 0; //use these to work out the size and where each node should be in the 2d array we'll use to store our nodes so we can work out neighbours and get paths
		int gridY = 0;

		bool foundTileOnLastPass = false;
		Road[] roads = FindObjectsOfType<Road> ();
		//scan tiles and create nodes based on where they are
		for(int x = scanStartX;x<scanFinishX;x++)
		{
			for (int y = scanStartY; y < scanFinishY; y++) {
				//go through our world bounds in increments of 1
				TileBase tb = floor.GetTile (new Vector3Int (x, y, 0)); //check if we have a floor tile at that world coords
				if (tb == null) {
				} else {
					//if we do we go through the obstacle layers and check if there is also a tile at those coords if so we set founObstacle to true
					bool foundObstacle = false;
					foreach (Tilemap t in obstacleLayers) {
						TileBase tb2 = t.GetTile (new Vector3Int (x, y, 0));

						if (tb2 == null) {

						} else {
							foundObstacle = true;
						}



						//if we want to add an unwalkable edge round our unwalkable nodes then we use this to get the neighbours and make them unwalkable
						if (unwalkableNodeBorder > 0) {
							List<TileBase> neighbours = getNeighbouringTiles (x, y, t);
							foreach(TileBase tl in neighbours)
							{
								if (tl == null) {

								} else {
									foundObstacle = true;
								}
							}
						}
					}

					int weight = 10;
					foreach (Tilemap t in weightIncreaseLayers) {
						TileBase tb2 = t.GetTile (new Vector3Int (x, y, 0));

						if (tb2 == null) {

						} else {
							weight += 150;
						}
					}

					if (foundObstacle == false) {
						//if we havent found an obstacle then we create a walkable node and assign its grid coords
						GameObject node = (GameObject)Instantiate (nodePrefab, new Vector3 (x + 0.5f + gridBase.transform.position.x, y + 0.5f+ gridBase.transform.position.y, 0), Quaternion.Euler (0, 0, 0));
						WorldTile wt = node.GetComponent<WorldTile> ();
						wt.gridX = gridX;
						wt.gridY = gridY;
						foundTileOnLastPass = true; //say that we have found a tile so we know to increment the index counters
						unsortedNodes.Add (node); 
						wt.modifier = weight;
						foreach (Road r in roads) {
							if (r.isPosInRoadBounds (node.transform.position) == true) {
								//wt.modifier += 100;
								wt.gameObject.GetComponent<SpriteRenderer>().color = Color.cyan;
							//	Debug.Break ();
							}
						}
						node.name = "NODE " + gridX.ToString () + " : " + gridY.ToString ();
						node.transform.parent = this.transform;
					} else {
						//if we have found an obstacle then we do the same but make the node unwalkable
						GameObject node = (GameObject)Instantiate (nodePrefab, new Vector3 (x + 0.5f+ gridBase.transform.position.x, y + 0.5f+ gridBase.transform.position.y, 0), Quaternion.Euler (0, 0, 0));
						//we add the gridBase position to ensure that the nodes are ontop of the tile they relate too
						node.GetComponent<SpriteRenderer> ().color = Color.red;
						WorldTile wt = node.GetComponent<WorldTile> ();
						wt.gridX = gridX;
						wt.gridY = gridY;
						wt.walkable = false;
						wt.modifier = weight;

						foundTileOnLastPass = true;
						unsortedNodes.Add (node);
						node.name = "UNWALKABLE NODE " + gridX.ToString () + " : " + gridY.ToString ();
						node.transform.parent = this.transform;


					}
					gridY++; //increment the y counter


					if (gridX > gridBoundX) { //if the current gridX/gridY is higher than the existing then replace it with the new value
						gridBoundX = gridX;
					}

					if (gridY > gridBoundY) {
						gridBoundY = gridY;
					}
				}
			}
			if (foundTileOnLastPass == true) {//since the grid is going from bottom to top on the Y axis on each iteration of the inside loop, if we have found tiles on this iteration we increment the gridX value and 
				//reset the y value
				gridX++;
				gridY = 0;
				foundTileOnLastPass = false;
			}
		}

		//put nodes into 2d array based on the 
		nodes = new GameObject[gridBoundX+1,gridBoundY+1];//initialise the 2d array that will store our nodes in their position 
		foreach (GameObject g in unsortedNodes) { //go through the unsorted list of nodes and put them into the 2d array in the correct position
			WorldTile wt = g.GetComponent<WorldTile> ();
			////////Debug.Log (wt.gridX + " " + wt.gridY);
			nodes [wt.gridX, wt.gridY] = g;
		}

		//assign neighbours to nodes
		for (int x = 0; x < gridBoundX; x++) { //go through the 2d array and assign the neighbours of each node
			for (int y = 0; y < gridBoundY; y++) {
				if (nodes [x, y] == null) { //check if the coords in the array contain a node

				}
				else{
					WorldTile wt = nodes [x, y].GetComponent<WorldTile> (); //if they do then assign the neighbours
					//if (wt.walkable == true) {
						wt.myNeighbours = getNeighbours (x, y, gridBoundX, gridBoundY);
					//}
				}
			}
		}

		foreach (GameObject g in unsortedNodes) {
			//WorldTile wt = g.GetComponent<WorldTile> ();
			//wt.modifier += getModiferForNode (wt);

		}
		foreach (GameObject g in unsortedNodes) {
			WorldTile wt = g.GetComponent<WorldTile> ();
			//wt.modifier = getModifierFalloff (wt);

			float b = (float)wt.modifier / 255.0f;
			b *= 10.5f;
//			//////Debug.Log (b);
			if (wt.walkable == true) {
				if (wt.modifier != 10) {
					g.GetComponent<SpriteRenderer> ().color = new Color (1, 0, b, 1.0f);
				} else {
					g.GetComponent<SpriteRenderer> ().color = Color.white;

				}
			} else{
				g.GetComponent<SpriteRenderer> ().color = Color.black;
			}
		}
		//after this we have our grid of nodes ready to be used by the astar algorigthm
		createWallColliders();
	}

	int getModiferForNode(WorldTile node)
	{
		int mod = 10;
		/*foreach (WorldTile w in node.myNeighbours) {
			if (w.walkable == false) {
				mod += 5;
			}

			TileBase t = cover.GetTile (new Vector3Int (Mathf.RoundToInt( w.gameObject.transform.position.x),Mathf.RoundToInt( w.gameObject.transform.position.y),0));
			if (t == null) {
				
			} else {
				mod -= 100;
			}

			TileBase t2 = walls.GetTile (new Vector3Int (Mathf.RoundToInt( w.gameObject.transform.position.x),Mathf.RoundToInt( w.gameObject.transform.position.y),0));

			if (t2 == null) {

			} else {
				mod += 50;
			}
		}

		if (node.gridX == 0 || node.gridX== gridBoundX - 1 || node.gridY == 0 || node.gridY == gridBoundY - 1) {
			node.walkable = false;
			mod += 50;
		}

		if (mod < 0) {
			mod = 1;
		}*/

		return mod;
	}

	int getModifierFalloff(WorldTile node)
	{
		int mod = node.modifier;
		foreach (WorldTile w in node.myNeighbours) {
			if (w.modifier > 10) {
				//mod += Mathf.RoundToInt( w.modifier*0.2f);
			}
		}
		return mod;
	}



	//gets neighbours of a tile at x/y in a specific tilemap, can also have a border
	public int unwalkableNodeBorder = 1;
	public List<TileBase> getNeighbouringTiles(int x,int y,Tilemap t)
	{
		List<TileBase> retVal = new List<TileBase>();

		for (int i = x - unwalkableNodeBorder; i < x + unwalkableNodeBorder; i++) {
			for (int j = y - unwalkableNodeBorder; j < y + unwalkableNodeBorder; j++) {
				TileBase tile = t.GetTile(new Vector3Int(i,j,0));
				if (tile == null) {

				} else {
					retVal.Add (tile);
				}
			}
		}

		return retVal;
	}

	//gets the neighbours of the coords passed in
	public List<WorldTile> getNeighbours(int x, int y,int width,int height)
	{

		List<WorldTile> myNeighbours = new List<WorldTile> ();
		GameObject node = nodes [x, y];
		//needs the width & height to work out if a tile is not on the edge, also needs to check if the nodes is null due to the accounting for odd shapes
		WorldTile wto = nodes[x,y].GetComponent<WorldTile>();
		//if (wto.walkable == false) {
		//	return new List<WorldTile> ();
		//}

		if (x > 0 && x < width-1) {
			//can get tiles on both left and right of the tile

			if (y > 0 && y < height - 1) {
				//top and bottom
				if (nodes [x + 1, y] == null) {

				} else {
					
					WorldTile wt1 = nodes [x + 1, y].GetComponent<WorldTile> ();
					if (wt1 == null) {
					} else {
						myNeighbours.Add (wt1);
					}
				}

				if (nodes [x - 1, y] == null) {

				} else {
					WorldTile wt2 = nodes [x - 1, y].GetComponent<WorldTile> ();

					if (wt2 == null) {

					} else {
						myNeighbours.Add (wt2);

					}
				}

				if (nodes [x, y + 1] == null) {

				} else {
					WorldTile wt3 = nodes [x, y + 1].GetComponent<WorldTile> ();
					if (wt3 == null) {

					} else {
						myNeighbours.Add (wt3);

					}
				}

				if (nodes [x, y - 1] == null) {

				} else {
					
					WorldTile wt4 = nodes [x, y - 1].GetComponent<WorldTile> ();
					if (wt4 == null) {

					} else {
						myNeighbours.Add (wt4);
					}
				}

			} else if (y == 0) {
				//just top
				if (nodes [x + 1, y] == null) {

				} else {

					WorldTile wt1 = nodes [x + 1, y].GetComponent<WorldTile> ();
					if (wt1 == null) {
					} else {
						myNeighbours.Add (wt1);
					}
				}

				if (nodes [x - 1, y] == null) {

				} else {
					WorldTile wt2 = nodes [x - 1, y].GetComponent<WorldTile> ();

					if (wt2 == null) {

					} else {
						myNeighbours.Add (wt2);

					}
				}
				if (nodes [x, y + 1] == null) {

				} else {
					WorldTile wt3 = nodes [x, y + 1].GetComponent<WorldTile> ();
					if (wt3 == null) {

					} else {
						myNeighbours.Add (wt3);

					}
				}
			} else if (y == height - 1) {
				//just bottom
				if (nodes [x, y - 1] == null) {

				} else {

					WorldTile wt4 = nodes [x, y - 1].GetComponent<WorldTile> ();
					if (wt4 == null) {

					} else {
						myNeighbours.Add (wt4);
					}
				}
				if (nodes [x + 1, y] == null) {

				} else {

					WorldTile wt1 = nodes [x + 1, y].GetComponent<WorldTile> ();
					if (wt1 == null) {
					} else {
						myNeighbours.Add (wt1);
					}
				}

				if (nodes [x - 1, y] == null) {

				} else {
					WorldTile wt2 = nodes [x - 1, y].GetComponent<WorldTile> ();

					if (wt2 == null) {

					} else {
						myNeighbours.Add (wt2);

					}
				}
			}


		} else if (x == 0) {
			//can't get tile on left
			if (y > 0 && y < height - 1) {
				//top and bottom
			
				if (nodes [x + 1, y] == null) {

				} else {

					WorldTile wt1 = nodes [x + 1, y].GetComponent<WorldTile> ();
					if (wt1 == null) {
					} else {
						myNeighbours.Add (wt1);
					}
				}

				if (nodes [x, y - 1] == null) {

				} else {

					WorldTile wt4 = nodes [x, y - 1].GetComponent<WorldTile> ();
					if (wt4 == null) {

					} else {
						myNeighbours.Add (wt4);
					}
				}
				if (nodes [x, y + 1] == null) {

				} else {
					WorldTile wt3 = nodes [x, y + 1].GetComponent<WorldTile> ();
					if (wt3 == null) {

					} else {
						myNeighbours.Add (wt3);

					}
				}
			} else if (y == 0) {
				//just top
				if (nodes [x + 1, y] == null) {

				} else {

					WorldTile wt1 = nodes [x + 1, y].GetComponent<WorldTile> ();
					if (wt1 == null) {
					} else {
						myNeighbours.Add (wt1);
					}
				}

				if (nodes [x, y + 1] == null) {

				} else {
					WorldTile wt3 = nodes [x, y + 1].GetComponent<WorldTile> ();
					if (wt3 == null) {

					} else {
						myNeighbours.Add (wt3);

					}
				}
			} else if (y == height - 1) {
				//just bottom
				if (nodes [x + 1, y] == null) {

				} else {

					WorldTile wt1 = nodes [x + 1, y].GetComponent<WorldTile> ();
					if (wt1 == null) {
					} else {
						myNeighbours.Add (wt1);
					}
				}
				if (nodes [x, y - 1] == null) {

				} else {

					WorldTile wt4 = nodes [x, y - 1].GetComponent<WorldTile> ();
					if (wt4 == null) {

					} else {
						myNeighbours.Add (wt4);
					}
				}
			}
		} else if (x == width-1) {
			//can't get tile on right
			if (y > 0 && y < height - 1) {
				//top and bottom
				if (nodes [x - 1, y] == null) {

				} else {
					WorldTile wt2 = nodes [x - 1, y].GetComponent<WorldTile> ();

					if (wt2 == null) {

					} else {
						myNeighbours.Add (wt2);

					}
				}

				if (nodes [x, y + 1] == null) {

				} else {
					WorldTile wt3 = nodes [x, y + 1].GetComponent<WorldTile> ();
					if (wt3 == null) {

					} else {
						myNeighbours.Add (wt3);

					}
				}
				if (nodes [x, y - 1] == null) {

				} else {

					WorldTile wt4 = nodes [x, y - 1].GetComponent<WorldTile> ();
					if (wt4 == null) {

					} else {
						myNeighbours.Add (wt4);
					}
				}
			} else if (y == 0) {
				//just top
				if (nodes [x - 1, y] == null) {

				} else {
					WorldTile wt2 = nodes [x - 1, y].GetComponent<WorldTile> ();

					if (wt2 == null) {

					} else {
						myNeighbours.Add (wt2);

					}
				}
				if (nodes [x, y + 1] == null) {

				} else {
					WorldTile wt3 = nodes [x, y + 1].GetComponent<WorldTile> ();
					if (wt3 == null) {

					} else {
						myNeighbours.Add (wt3);

					}
				}
			} else if (y == height - 1) {
				//just bottom
				if (nodes [x - 1, y] == null) {

				} else {
					WorldTile wt2 = nodes [x - 1, y].GetComponent<WorldTile> ();

					if (wt2 == null) {

					} else {
						myNeighbours.Add (wt2);

					}
				}
				if (nodes [x, y - 1] == null) {

				} else {

					WorldTile wt4 = nodes [x, y - 1].GetComponent<WorldTile> ();
					if (wt4 == null) {

					} else {
						myNeighbours.Add (wt4);
					}
				}
			}
		}
		List<WorldTile> retVal = new List<WorldTile> ();
		foreach (WorldTile wt in myNeighbours) {
			if ( Vector2.Distance(wt.gameObject.transform.position,node.transform.position)<1.1f) {
				retVal.Add (wt);
			}
		}

		Debug.Log ("Node had " + retVal.Count);
		myNeighbours = retVal;

		return myNeighbours;
	}
}
