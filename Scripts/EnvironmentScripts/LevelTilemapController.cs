using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class LevelTilemapController : MonoBehaviour {

	/// <summary>
	/// Stores the various tilemaps that are used for things like pathfinding, minimap generation &  
	/// </summary>

	public static LevelTilemapController me;

	/// <summary>
	///Floor tilemap, used as a base for pathfinding nodes & map generation 
	/// </summary>
	public Tilemap floor;

	/// <summary>
	/// Tilemap that marks where walls are in the level 
	/// </summary>
	public Tilemap walls;

	/// <summary>
	/// Tilemap that contains all the areas that are affected by lights other than the sun
	/// </summary>
	public Tilemap lighting;

	/// <summary>
	/// Another tilemap to define unwalkable tiles, used by the pathfinding 
	/// </summary>
	public Tilemap nonWalkable;

	/// <summary>
	/// Tiles that the lighting can switch between. 
	/// </summary>
	public Tile dark,lit,evening,outdoorLight;
	public LayerMask mask;
	void Awake()
	{
		me = this;
		//Have to set the floor to recive shadows here because it won't let us in the inspector
		floor.gameObject.GetComponent<TilemapRenderer> ().receiveShadows = true;
		calculateLighting ();
		TilemapRenderer t = lighting.gameObject.GetComponent<TilemapRenderer> ();
		//lighting.transform.position = new Vector3 (lighting.transform.position.x - 0.2f, lighting.transform.position.y - 0.2f, lighting.transform.position.z);
		t.sortingOrder = 99;
	}

	// Use this for initialization
	void Start () {
		LightSource.UpdateLightMeshes ();

		Physics2D.queriesStartInColliders = false;


	}
	
	// Update is called once per frame
	void Update () {
		//setColorForSunLighting ();
	}
	public void setColorForSunLighting()
	{
		//outdoorLight.color = TimeScript.me.getSunColor ();
	}

	/// <summary>
	/// Goes through all the lighting positions in each light source and sets it to be the relevent tile based on whether the light is on or off. 
	/// </summary>
	void calculateLighting()
	{
		LightSource[] lights = FindObjectsOfType<LightSource> ();
		/*foreach (LightSource l in lights) {
			if (l.myType == lightType.lightbulb) {
				l.decideWhichTilesAreInRangeOfLight ();
			}
		}

		foreach (LightSource l in lights) {
			if (l.myType == lightType.sun) {
				l.decideWhichTilesAreInRangeOfLight ();
			}
		}*/

		/*for (int x = -500; x < 500; x++) {
			for (int y = -500; y < 500; y++) {
				Vector3Int pos = new Vector3Int (x, y, 0);

				TileBase t = floor.GetTile (pos);

				RoomScript roomTileIsIn = LevelController.me.getRoomPosIsIn (pos);
				BuildingScript b = LevelController.me.getBuildingPosIsIn (pos);
				if (t == null) {
					
				} else {
					//
					////////Debug.Log("Found tile");
					if (roomTileIsIn==null) {
						
						//if (walls.GetTile (pos) == null) {
							foreach (LightSource l in lights) {
							RoomScript r = LevelController.me.getRoomPosIsIn (l.gameObject.transform.position);

								if (l.myType == lightType.sun) {
									if (roomTileIsIn == null ) {
										l.addTileToLightsource (t);
										l.lightingTilePositions.Add (new Vector3 (x, y, 0));
									}
									//if (l.lightOn == true) {
									//	lighting.SetTile (pos, lit);
									//} else {
									//	lighting.SetTile (pos, dark);
									//}
								}
							}
						//}
					} 	
						//Debug.Log (b.buildingName);
						

						//if (walls.GetTile (pos) == null) {
							foreach (LightSource l in lights) {
								if (l.myType == lightType.lightbulb) {
									if (Vector3.Distance (l.transform.position, new Vector3 (x, y, 0)) < l.maxLightDistance) {
								RoomScript r = LevelController.me.getRoomPosIsIn (l.gameObject.transform.position);
								//took out raycast check && r == roomTileIsIn
									if (lineOfSightToLightSoruce (new Vector3 (x, y, 0), l.gameObject.transform.position) == true ) {
											//////Debug.Log ("Calculating lighting");

											l.addTileToLightsource (t);
											l.lightingTilePositions.Add (new Vector3 (x, y, 0));
											//if (l.lightOn == true) {
											//	lighting.SetTile (pos, lit);
											//} else {
											//	lighting.SetTile (pos, dark);
											//}
										}
									}
								}
							}
						//}
				}


			}
		}*/
		setColorForSunLighting ();

		foreach (LightSource l in lights) {
		if (l.lightOn == true && l != LightSource.sun) {
				foreach (Vector3 v in l.lightingTilePositions) {
					Vector3Int pos = new Vector3Int ((int)v.x, (int)v.y, 0);
					lighting.SetTile (pos, lit);
				}
			}
		}

		foreach (LightSource l in lights) {
		if (l.lightOn == false && l != LightSource.sun) {
				foreach (Vector3 v in l.lightingTilePositions) {
					Vector3Int pos = new Vector3Int ((int)v.x, (int)v.y, 0);
					lighting.SetTile (pos, dark);
				}
			}
		}
	}

	/// <summary>
	/// Method for NPC's to work out if they are in a lit area or not, affects their FOV 
	/// </summary>
	/// <returns><c>true</c>, if we lit was ared, <c>false</c> otherwise.</returns>
	/// <param name="pos">Position.</param>
	public bool areWeLit(Vector3 pos)
	{
		TileBase t = lighting.GetTile (new Vector3Int ((int)pos.x, (int)pos.y, 0));
		if (t == null) {
			return true;
		} else if (t == lit || t==outdoorLight && LightSource.sun.lightOn==true) {
			return true;
		} else {
			return false;
		}
	}
}
