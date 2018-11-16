using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class CreateNodesFromTilemaps : MonoBehaviour {

	/// <summary>
	/// Class that creates pathfinding nodes based on the Tilemaps provided.
	/// </summary>

	public static CreateNodesFromTilemaps me;
	public Grid gridBase;

	/// <summary>
	/// Floor of the world, for each tile in this tilemap, a node will be created. 
	/// </summary>
	public Tilemap floor;//floor of world
	/// <summary>
	/// Tilemaps where if a tile is found it will either mark the node as unwalkable or increase its weight so that it is avoided by NPCs
	/// </summary>
	public List<Tilemap> obstacleLayers,weightIncreaseLayers; //all layers that contain objects to navigate around
	public GameObject nodePrefab;

	/// <summary>
	/// Currently unused, maybe add some AI that take cover in the future 
	/// </summary>
	public Tilemap cover,walls;

	//these are the bounds of where we are searching in the world for tiles, have to use world coords to check for tiles in the tile map
	public int scanStartX=-250,scanStartY=-250,scanFinishX=250,scanFinishY=250;


	public List<GameObject> unsortedNodes;//all the nodes in the world
	public GameObject[,] nodes; //sorted 2d array of nodes, may contain null entries if the map is of an odd shape e.g. gaps

	public bool createNodesOnAwake=false;
	public GameObject wallColliderPrefab;
	// Use this for initialization
	void Awake () {
		me = this;
	}

	void Start()
	{
		
	}

	public void generateNodes()
	{
		createNodes ();
		FindObjectOfType<WorldBuilder>().passInTilemapPathfinding (nodes, gridBoundX, gridBoundY);
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

	List<WorldTile> nodesToRemove = new List<WorldTile>();

#if UNITY_EDITOR

    public void editorCreateNodes()
    {
        unsortedNodes = new List<GameObject>();

        int gridX = 0, gridY = 0;
        int boundX = 0, boundY = 0;
        bool foundTileOnLastPass = false;

        for (int x = scanStartX; x < scanFinishX; x++)
        {
            for (int y = scanStartY; y < scanFinishY; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                TileBase tb = floor.GetTile(floor.WorldToCell(pos));

                if (tb == null)
                {
                    if (foundTileOnLastPass == true)
                    {
                        gridY++;
                        if (gridX > boundX)
                        {
                            boundX = gridX;
                        }

                        if (gridY > boundY)
                        {
                            boundY = gridY;
                        }
                    }
                }
                else
                {
                    bool foundObstacle = false;
                    foreach (Tilemap t in obstacleLayers)
                    {
                        TileBase tb2 = t.GetTile(t.WorldToCell(pos));
                        if (tb2 == null)
                        {

                        }
                        else
                        {
                            foundObstacle = true;
                        }
                    }




                    if (foundObstacle == true)
                    {
                        GameObject node = (GameObject)Instantiate(nodePrefab, new Vector3(x + 0.5f + gridBase.transform.position.x, y + 0.5f + gridBase.transform.position.y, 0), Quaternion.Euler(0, 0, 0));
                        //we add the gridBase position to ensure that the nodes are ontop of the tile they relate too
                        node.GetComponent<SpriteRenderer>().color = Color.red;
                        WorldTile wt = node.GetComponent<WorldTile>();
                        wt.gridX = gridX;
                        wt.gridY = gridY;
                        wt.walkable = false;
                        wt.modifier = 1000;

                        foundTileOnLastPass = true;
                        unsortedNodes.Add(node);
                        node.name = "UNWALKABLE NODE " + gridX.ToString() + " : " + gridY.ToString();
                        node.transform.parent = this.transform;
                    }
                    else
                    {
                        int weight = 10;
                        foreach (Tilemap t in weightIncreaseLayers)
                        {
                            TileBase tb2 = t.GetTile(new Vector3Int(x, y, 0));

                            if (tb2 == null)
                            {

                            }
                            else
                            {
                                weight += 150;
                            }
                        }
                        GameObject node = (GameObject)Instantiate(nodePrefab, new Vector3(x + 0.5f + gridBase.transform.position.x, y + 0.5f + gridBase.transform.position.y, 0), Quaternion.Euler(0, 0, 0));
                        WorldTile wt = node.GetComponent<WorldTile>();
                        wt.gridX = gridX;
                        wt.gridY = gridY;
                        foundTileOnLastPass = true; //say that we have found a tile so we know to increment the index counters
                        unsortedNodes.Add(node);
                        wt.modifier = weight;

                        node.name = "NODE " + gridX.ToString() + " : " + gridY.ToString();
                        node.transform.parent = this.transform;

                    }

                    gridY++;
                    if (gridX > boundX)
                    {
                        boundX = gridX;
                    }

                    if (gridY > boundY)
                    {
                        boundY = gridY;
                    }
                }
            }

            if (foundTileOnLastPass == true)
            {
                gridX++;
                gridY = 0;
                foundTileOnLastPass = false;
            }
        }

        nodes = new GameObject[boundX + 1, boundY + 1];
        gridBoundX = boundX + 1;
        gridBoundY = boundY + 1;
        //Debug.LogError("Grid length is " + (boundX + 1).ToString() + " :: " + (boundY + 1).ToString());

        foreach (GameObject g in unsortedNodes)
        {
            WorldTile wt = g.GetComponent<WorldTile>();
            nodes[wt.gridX, wt.gridY] = g;
        }

        foreach (GameObject g in unsortedNodes)
        {
            WorldTile wt = g.GetComponent<WorldTile>();
            List<WorldTile> neigbours = new List<WorldTile>();
            foreach (GameObject g2 in unsortedNodes)
            {
                if (g2 == g)
                {
                    continue;
                }

                float d = Vector2.Distance(g.transform.position, g2.transform.position);
                if (d < 1.1f)
                {
                    neigbours.Add(g2.GetComponent<WorldTile>());
                }
            }
            wt.myNeighbours = neigbours;
            EditorUtility.SetDirty(wt);
        }
        EditorUtility.SetDirty(this);

    }
#endif

    public GameObject[,] reassembleNodeArray()
    {
        GameObject[,] retVal = new GameObject[gridBoundX,gridBoundY];
        foreach(GameObject g in unsortedNodes)
        {
            WorldTile wt = g.GetComponent<WorldTile>();
            retVal[wt.gridX, wt.gridY] = g;
        }
        return retVal;
    }

    void createNodesTwo()
    {
        unsortedNodes = new List<GameObject>();

        int gridX = 0, gridY = 0;
        int boundX = 0, boundY = 0;
        bool foundTileOnLastPass = false;

        for (int x = scanStartX;x<scanFinishX;x++)
        {
            for (int y = scanStartY; y < scanFinishY; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                TileBase tb = floor.GetTile(floor.WorldToCell(pos));

                if(tb==null)
                {
                    if(foundTileOnLastPass==true)
                    {
                        gridY++;
                        if (gridX > boundX)
                        {
                            boundX = gridX;
                        }

                        if(gridY>boundY)
                        {
                            boundY = gridY;
                        }
                    }
                }
                else
                {
                    bool foundObstacle = false;
                    foreach(Tilemap t in obstacleLayers)
                    {
                        TileBase tb2 = t.GetTile(t.WorldToCell(pos));
                        if(tb2==null)
                        {

                        }
                        else
                        {
                            foundObstacle = true;
                        }
                    }




                    if(foundObstacle==true)
                    {
                        GameObject node = (GameObject)Instantiate(nodePrefab, new Vector3(x + 0.5f + gridBase.transform.position.x, y + 0.5f + gridBase.transform.position.y, 0), Quaternion.Euler(0, 0, 0));
                        //we add the gridBase position to ensure that the nodes are ontop of the tile they relate too
                        node.GetComponent<SpriteRenderer>().color = Color.red;
                        WorldTile wt = node.GetComponent<WorldTile>();
                        wt.gridX = gridX;
                        wt.gridY = gridY;
                        wt.walkable = false;
                        wt.modifier =1000;

                        foundTileOnLastPass = true;
                        unsortedNodes.Add(node);
                        node.name = "UNWALKABLE NODE " + gridX.ToString() + " : " + gridY.ToString();
                        node.transform.parent = this.transform;
                    }
                    else
                    {
                        int weight = 10;
                        foreach (Tilemap t in weightIncreaseLayers)
                        {
                            TileBase tb2 = t.GetTile(new Vector3Int(x, y, 0));

                            if (tb2 == null)
                            {

                            }
                            else
                            {
                                weight += 150;
                            }
                        }
                        GameObject node = (GameObject)Instantiate(nodePrefab, new Vector3(x + 0.5f + gridBase.transform.position.x, y + 0.5f + gridBase.transform.position.y, 0), Quaternion.Euler(0, 0, 0));
                        WorldTile wt = node.GetComponent<WorldTile>();
                        wt.gridX = gridX;
                        wt.gridY = gridY;
                        foundTileOnLastPass = true; //say that we have found a tile so we know to increment the index counters
                        unsortedNodes.Add(node);
                        wt.modifier = weight;
                      
                        node.name = "NODE " + gridX.ToString() + " : " + gridY.ToString();
                        node.transform.parent = this.transform;

                    }

                    gridY++;
                    if(gridX>boundX)
                    {
                        boundX = gridX;
                    }

                    if(gridY>boundY)
                    {
                        boundY = gridY;
                    }
                }
            }

            if(foundTileOnLastPass==true)
            {
                gridX++;
                gridY = 0;
                foundTileOnLastPass = false;
            }
        }

        nodes = new GameObject[boundX+1,boundY+1];
        gridBoundX = boundX + 1;
        gridBoundY = boundY+1;
        //Debug.LogError("Grid length is " + (boundX + 1).ToString()+ " :: " + (boundY + 1).ToString());

        foreach(GameObject g in unsortedNodes)
        {
            WorldTile wt = g.GetComponent<WorldTile>();
            nodes[wt.gridX, wt.gridY] = g;
        }

        foreach(GameObject g in unsortedNodes)
        {
            WorldTile wt = g.GetComponent<WorldTile>();
            List<WorldTile> neigbours = new List<WorldTile>();
            foreach(GameObject g2 in unsortedNodes)
            {
                if (g2 == g)
                {
                    continue;
                }

                float d = Vector2.Distance(g.transform.position, g2.transform.position);
                if(d<1.5f)
                {
                    neigbours.Add(g2.GetComponent<WorldTile>());
                }
            }
            wt.myNeighbours = neigbours;
        }

     /*   for(int x =0;x<nodes.GetLength(0);x++)
        {
            for (int y = 0; y < nodes.GetLength(1); y++)
            {
                if (nodes[x, y] != null)
                {
                    WorldTile wt = nodes[x, y].GetComponent<WorldTile>(); //if they do then assign the neighbours
                    wt.myNeighbours = getNeighbours(x, y, boundX + 1, boundY + 1);

                }
            }
        }
    */
        if (Application.isEditor)
        {
            foreach (GameObject g in unsortedNodes)
            {
                WorldTile wt = g.GetComponent<WorldTile>();
                //wt.modifier = getModifierFalloff (wt);

                float b = (float)wt.modifier / 255.0f;
                b *= 10.5f;
                //			////////Debug.Log (b);
                if (wt.walkable == true)
                {
                    if (wt.modifier != 10)
                    {
                        g.GetComponent<SpriteRenderer>().color = new Color(1, 0, b, 1.0f);
                    }
                    else
                    {
                        g.GetComponent<SpriteRenderer>().color = Color.white;

                    }
                }
                else
                {
                    g.GetComponent<SpriteRenderer>().color = Color.black;
                }
            }
        }
        createWallColliders();
    }

	/// <summary>
	/// Method that creates the nodes, creates all the nodes first then makes a second pass to assign neighbours. 
	/// </summary>
	void createNodes()
	{
        unsortedNodes = new List<GameObject>();

        createNodesTwo();
        return;

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
							//	//Debug.Break ();
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
		nodes = new GameObject[gridBoundX+1,gridBoundY];//initialise the 2d array that will store our nodes in their position 
		foreach (GameObject g in unsortedNodes) { //go through the unsorted list of nodes and put them into the 2d array in the correct position
			WorldTile wt = g.GetComponent<WorldTile> ();
			//////////Debug.Log (wt.gridX + " " + wt.gridY);
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

		//foreach (GameObject g in unsortedNodes) {
			//WorldTile wt = g.GetComponent<WorldTile> ();
			//wt.modifier += getModiferForNode (wt);

		//}
		foreach (GameObject g in unsortedNodes) {
			WorldTile wt = g.GetComponent<WorldTile> ();
			//wt.modifier = getModifierFalloff (wt);

			float b = (float)wt.modifier / 255.0f;
			b *= 10.5f;
//			////////Debug.Log (b);
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


		//code below tries to optimise the pathfinding nodes by merging similar neighbours, don't remove, just commented out for the time being
		/*try{
			for (int x = 1; x < gridBoundX-1; x++) {
				for (int y = 1; y < gridBoundY-1; y++) {

					if(nodesToRemove.Contains(nodes[x,y].GetComponent<WorldTile>())==true ||nodes[x,y].GetComponent<WorldTile>().isOptimised==true || areWeNearCivilianActions(nodes[x,y].GetComponent<WorldTile>()))
					{
						continue;
					}
					List<WorldTile> nodesToCheck = new List<WorldTile> ();


					nodesToCheck.Add (nodes [x, y].GetComponent<WorldTile> ());

					if (nodes [x+ 1, y ] == null) {
					} else {
						nodesToCheck.Add (nodes [x + 1, y].GetComponent<WorldTile> ());
					}
					if (nodes [x, y + 1] == null) {
					} else {

						nodesToCheck.Add (nodes [x, y + 1].GetComponent<WorldTile> ());
					}

					if (nodes [x+1, y + 1] == null) {
					} else {
						nodesToCheck.Add (nodes [x + 1, y + 1].GetComponent<WorldTile> ());
					}
					int mod = nodesToCheck [0].modifier;
				//	//Debug.Log("Found " + nodesToCheck.Count + " nodes");
					bool doWeModifyNodes = true;
					foreach (WorldTile wt in nodesToCheck) {
						if(wt.modifier>250 || wt.walkable==false || wt.isOptimised==true)
						{
							doWeModifyNodes = false;
						}
					}
					GameObject g = null;
					if (doWeModifyNodes == true && nodesToCheck.Count>=3) {
						g = (GameObject)Instantiate(nodePrefab,nodes[x,y].transform.position+new Vector3(0.5f,0.5f,0),Quaternion.Euler(0,0,0));
						g.transform.parent = this.transform;
						WorldTile newTile = g.GetComponent<WorldTile>();

						newTile.gridX = nodesToCheck[0].gridX;
						newTile.gridY = nodesToCheck[0].gridY;
						newTile.isOptimised=true;
						SpriteRenderer sr = g.GetComponent<SpriteRenderer> ();
						sr.color = Color.green;
						newTile.myNeighbours = new List<WorldTile>();

						List<WorldTile> neighbours = new List<WorldTile>();
						foreach(WorldTile wt in nodesToCheck)
						{
							newTile.modifier += wt.modifier/nodesToCheck.Count;
							foreach(WorldTile n in wt.myNeighbours)
							{
								if(neighbours.Contains(n)==false)
								{
									neighbours.Add(n);
								}
							}
						}

						foreach(WorldTile wt in neighbours){
							foreach(WorldTile w in nodesToCheck)
							{
								wt.myNeighbours.Remove(w);
							}
							wt.myNeighbours.Add(newTile);
							newTile.myNeighbours.Add(wt);
						}

						foreach (WorldTile wt in nodesToCheck) {
							if(g==null || wt==null)
							{

							}
							else{
								nodes[wt.gridX,wt.gridY]=g;
								//wt.GetComponent<SpriteRenderer>().color = Color.clear;
								if(nodesToRemove.Contains(wt)==false && wt.isOptimised==false){
									nodesToRemove.Add(wt);
								}
								//DestroyImmediate(wt.gameObject);
							}


						}
					}
				}

			}
			//Debug.Log("There are " + nodesToRemove.Count);
			foreach(WorldTile wt in nodesToRemove)
			{
				//Theres a bug where some spaces are null and need to carry over the weights of the nodes
				if(wt==null)
				{
					continue;
				}
				if(nodes[wt.gridX,wt.gridY] == wt.gameObject){
				}
				else{
					DestroyImmediate(wt.gameObject);
				}
			}
			for (int x = 0; x < gridBoundX; x++) {
				for (int y = 0; y < gridBoundY; y++) {
					GameObject g = nodes[x,y];
					if(g==null)
					{

					}
					else{
						WorldTile wt = g.GetComponent<WorldTile>();
						List<WorldTile> fn = new List<WorldTile>();
						foreach(WorldTile n in wt.myNeighbours)
						{
							if(n==null){
							}
							else{
								fn.Add(n);
							}
						}
						wt.myNeighbours=fn;
					}
				}
			}

		}
		catch(System.Exception e){
			//Debug.LogError ("Something went wrong optimising grid " + e.ToString());
		}*/
		//after this we have our grid of nodes ready to be used by the astar algorigthm
		createWallColliders();
	}

	CivilianAction[] actions;
	bool areWeNearCivilianActions(WorldTile wt)
	{
		if (actions == null || actions.Length == 0) {
			actions = FindObjectsOfType<CivilianAction> ();
		}
		foreach (CivilianAction ca in actions) {
			if (Vector2.Distance (ca.transform.position, wt.transform.position) < 5) {
				return true;
			}
		}
		return false;
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

	//	//Debug.Log ("Node had " + retVal.Count);
		myNeighbours = retVal;

		return myNeighbours;
	}
}
