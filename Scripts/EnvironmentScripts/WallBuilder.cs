using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class WallBuilder : MonoBehaviour {
	/// <summary>
	/// World builder tool built on the old wall builder, not used ingame, just for creating the aesthetics of levels (walls, doors, buildings, rooms, misc 3d objects) 
	/// </summary>


	public static WallBuilder me;
	public List<GameObject> wallSectionsAdded,objectsInLevel;
	public Tile block;
	public List<GameObject> wallTypes;
	public List<GameObject> objectsWeCanPlace;
	public GameObject roomPrefab, buildingPrefab;
	public List<BuildingScript> buildingsICreated;
	public List<RoomScript> roomsICreated;
	public static WallBuilder getBuilder()
	{
		if (me == null) {
			me = FindObjectOfType<WallBuilder> ();
		}
		return me;
	}
	public GameObject corner,wallOneTex,wallTwoTex,windowSection,cornerSmall;

	public void createWallSection(GameObject g, bool corner,bool tSection)
	{
		if (wallSectionsAdded == null) {
			wallSectionsAdded = new List<GameObject> ();
		}
		wallSectionsAdded.Add (g);
		g.transform.position = new Vector3 (g.transform.position.x, g.transform.position.y, g.transform.position.z);
		g.transform.parent = this.transform;
		makeNodesWalkable (g,corner,tSection);
	}

	void makeNodesWalkable(GameObject g, bool corner,bool tSection)
	{
		LevelTilemapController tilemapCont = FindObjectOfType<LevelTilemapController> ();
		Vector3Int v = new Vector3Int (Mathf.RoundToInt( g.transform.position.x), Mathf.RoundToInt( g.transform.position.y), 0);
		tilemapCont.nonWalkable.SetTile (v, block);
		v = new Vector3Int (Mathf.RoundToInt( g.transform.position.x-1), Mathf.RoundToInt( g.transform.position.y), 0);
		tilemapCont.nonWalkable.SetTile (v, block);
		v = new Vector3Int (Mathf.RoundToInt( g.transform.position.x-1), Mathf.RoundToInt( g.transform.position.y-1), 0);
		tilemapCont.nonWalkable.SetTile (v, block);
		v = new Vector3Int (Mathf.RoundToInt( g.transform.position.x), Mathf.RoundToInt( g.transform.position.y-1), 0);
		tilemapCont.nonWalkable.SetTile (v, block);
		/*if (tSection == true) {
			if (g.transform.rotation.eulerAngles.z <= 1) {

				Vector3Int v = new Vector3Int ((int)g.transform.position.x - 1, (int)g.transform.position.y - 1, 0);
				tilemapCont.nonWalkable.SetTile (v, block); //TODO doesnt seem to work, work out why
				//TODO redo the two tone wall so its center is in the right place and it matches up with the other wall


				v = new Vector3Int ((int)g.transform.position.x - 1, (int)g.transform.position.y , 0);
				tilemapCont.nonWalkable.SetTile (v, block); 

				v = new Vector3Int ((int)g.transform.position.x-2, (int)g.transform.position.y - 1, 0);
				tilemapCont.nonWalkable.SetTile (v, block);


				//v = new Vector3Int ((int)g.transform.position.x+2, (int)g.transform.position.y - 1, 0);
				//tilemapCont.nonWalkable.SetTile (v, block);


					
			}
			else if(g.transform.rotation.eulerAngles.z >= 179 && g.transform.rotation.eulerAngles.z <= 181)
			{
				Vector3Int v = new Vector3Int ((int)g.transform.position.x - 1, (int)g.transform.position.y - 1, 0);
				tilemapCont.nonWalkable.SetTile (v, block); //TODO doesnt seem to work, work out why
				//TODO redo the two tone wall so its center is in the right place and it matches up with the other wall


				v = new Vector3Int ((int)g.transform.position.x - 1, (int)g.transform.position.y , 0);
				tilemapCont.nonWalkable.SetTile (v, block); 

				v = new Vector3Int ((int)g.transform.position.x, (int)g.transform.position.y - 1, 0);
				tilemapCont.nonWalkable.SetTile (v, block);


				v = new Vector3Int ((int)g.transform.position.x+1, (int)g.transform.position.y - 1, 0);
				tilemapCont.nonWalkable.SetTile (v, block);

			}
			else if (g.transform.rotation.eulerAngles.z >= 89 && g.transform.rotation.eulerAngles.z <= 91 ) {
				

				Vector3Int v = new Vector3Int ((int)g.transform.position.x - 1, (int)g.transform.position.y - 1, 0);
				tilemapCont.nonWalkable.SetTile (v, block); //TODO doesnt seem to work, work out why
				//TODO redo the two tone wall so its center is in the right place and it matches up with the other wall


				v = new Vector3Int ((int)g.transform.position.x , (int)g.transform.position.y-1, 0);
				tilemapCont.nonWalkable.SetTile (v, block);
				v = new Vector3Int ((int)g.transform.position.x - 1, (int)g.transform.position.y, 0);
				tilemapCont.nonWalkable.SetTile (v, block);
				v = new Vector3Int ((int)g.transform.position.x - 1, (int)g.transform.position.y+1, 0);
				tilemapCont.nonWalkable.SetTile (v, block);
			}
			else if(g.transform.rotation.eulerAngles.z >= 269 && g.transform.rotation.eulerAngles.z <= 271)
			{
				Vector3Int v = new Vector3Int ((int)g.transform.position.x - 1, (int)g.transform.position.y - 1, 0);
				tilemapCont.nonWalkable.SetTile (v, block); //TODO doesnt seem to work, work out why
				//TODO redo the two tone wall so its center is in the right place and it matches up with the other wall


				v = new Vector3Int ((int)g.transform.position.x , (int)g.transform.position.y-1, 0);
				tilemapCont.nonWalkable.SetTile (v, block);

				v = new Vector3Int ((int)g.transform.position.x - 1, (int)g.transform.position.y-1, 0);
				tilemapCont.nonWalkable.SetTile (v, block);

				v = new Vector3Int ((int)g.transform.position.x - 1, (int)g.transform.position.y-2, 0);
				tilemapCont.nonWalkable.SetTile (v, block);
			}
		} else {

			if (corner == true) {
				Vector3Int v = new Vector3Int ((int)g.transform.position.x, (int)g.transform.position.y - 1, 0);
				tilemapCont.nonWalkable.SetTile (v, block);
			} else {
				if (g.transform.rotation.eulerAngles.z <= 1 || g.transform.rotation.eulerAngles.z >= 179 && g.transform.rotation.eulerAngles.z <= 181) {
					Vector3Int v = new Vector3Int ((int)g.transform.position.x - 1, (int)g.transform.position.y - 1, 0);
					tilemapCont.nonWalkable.SetTile (v, block); //TODO doesnt seem to work, work out why
					//TODO redo the two tone wall so its center is in the right place and it matches up with the other wall



					if (corner == false) {
						v = new Vector3Int ((int)g.transform.position.x, (int)g.transform.position.y - 1, 0);
						tilemapCont.nonWalkable.SetTile (v, block);

						//v = new Vector3Int ((int)g.transform.position.x, (int)g.transform.position.y, 0);
						//tilemapCont.nonWalkable.SetTile (v,block);

						//v = new Vector3Int ((int)g.transform.position.x-1, (int)g.transform.position.y, 0);
						//tilemapCont.nonWalkable.SetTile (v,block);
					}
				} else if (g.transform.rotation.eulerAngles.z >= 89 && g.transform.rotation.eulerAngles.z <= 91 || g.transform.rotation.eulerAngles.z >= 269 && g.transform.rotation.eulerAngles.z <= 271) {
					Vector3Int v = new Vector3Int ((int)g.transform.position.x - 1, (int)g.transform.position.y - 1, 0);
					tilemapCont.nonWalkable.SetTile (v, block); //TODO doesnt seem to work, work out why
					//TODO redo the two tone wall so its center is in the right place and it matches up with the other wall



					if (corner == false) {
					
						v = new Vector3Int ((int)g.transform.position.x - 1, (int)g.transform.position.y, 0);
						tilemapCont.nonWalkable.SetTile (v, block);

						//v = new Vector3Int ((int)g.transform.position.x, (int)g.transform.position.y, 0);
						//tilemapCont.nonWalkable.SetTile (v,block);

						//v = new Vector3Int ((int)g.transform.position.x, (int)g.transform.position.y-1, 0);
						//tilemapCont.nonWalkable.SetTile (v,block);
					}
				}

			}
		}*/

	}

	void makeNodesUnwalkable(GameObject g, bool corner)
	{
		LevelTilemapController tilemapCont = FindObjectOfType<LevelTilemapController> ();

		if (corner == true) {
			Vector3Int v = new Vector3Int ((int)g.transform.position.x , (int)g.transform.position.y-1, 0);
			tilemapCont.nonWalkable.SetTile (v, null);
		} else {
			if (g.transform.rotation.eulerAngles.z <= 1 || g.transform.rotation.eulerAngles.z >= 179 && g.transform.rotation.eulerAngles.z <= 181) {
				Vector3Int v = new Vector3Int ((int)g.transform.position.x - 1, (int)g.transform.position.y - 1, 0);
				tilemapCont.nonWalkable.SetTile (v, null); //TODO doesnt seem to work, work out why
				//TODO redo the two tone wall so its center is in the right place and it matches up with the other wall



				if (corner == false) {
					v = new Vector3Int ((int)g.transform.position.x, (int)g.transform.position.y - 1, 0);
					tilemapCont.nonWalkable.SetTile (v, null);

					//v = new Vector3Int ((int)g.transform.position.x, (int)g.transform.position.y, 0);
					//tilemapCont.nonWalkable.SetTile (v,block);

					//v = new Vector3Int ((int)g.transform.position.x-1, (int)g.transform.position.y, 0);
					//tilemapCont.nonWalkable.SetTile (v,block);
				}
			} else if (g.transform.rotation.eulerAngles.z >= 89 && g.transform.rotation.eulerAngles.z <= 91 || g.transform.rotation.eulerAngles.z >= 269 && g.transform.rotation.eulerAngles.z <= 271) {
				Vector3Int v = new Vector3Int ((int)g.transform.position.x - 1, (int)g.transform.position.y - 1, 0);
				tilemapCont.nonWalkable.SetTile (v, null); //TODO doesnt seem to work, work out why
				//TODO redo the two tone wall so its center is in the right place and it matches up with the other wall



				if (corner == false) {

					v = new Vector3Int ((int)g.transform.position.x - 1, (int)g.transform.position.y, 0);
					tilemapCont.nonWalkable.SetTile (v, null);

					//v = new Vector3Int ((int)g.transform.position.x, (int)g.transform.position.y, 0);
					//tilemapCont.nonWalkable.SetTile (v,block);

					//v = new Vector3Int ((int)g.transform.position.x, (int)g.transform.position.y-1, 0);
					//tilemapCont.nonWalkable.SetTile (v,block);
				}
			}

		}
	}

	public GameObject getNearest(Vector3 pos){
		GameObject nearest = null;
		float distance = 9999999.0f;
		foreach (GameObject g in wallSectionsAdded) {
			if (g == null) {
				continue;
			}

			float d = Vector3.Distance (pos, g.transform.position);
			if (d < distance) {
				nearest = g;
				distance = d;
			}
		}
		return nearest;
	}

	public void destroyWallSection(GameObject g)
	{
		if (wallSectionsAdded.Contains (g)) {
			wallSectionsAdded.Remove (g);
		}
		makeNodesUnwalkable (g, g == corner);



		DestroyImmediate (g);
	}

	public void addNewObject(GameObject g)
	{
		if (objectsInLevel == null) {
			objectsInLevel = new List<GameObject> ();
		}
		objectsInLevel.Add (g);
	}


	public void addNewRoom(RoomScript r)
	{
		if (roomsICreated == null) {
			roomsICreated = new List<RoomScript> ();
		}

		roomsICreated.Add (r);
	}

	public void addNewBuilding(BuildingScript b)
	{
		if (buildingsICreated == null) {
			buildingsICreated = new List<BuildingScript> ();
		}
		buildingsICreated.Add (b);

	}


}
