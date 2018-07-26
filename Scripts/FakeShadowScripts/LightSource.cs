using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class LightSource : MonoBehaviour {

	/// <summary>
	/// Lightsource in the level, methods need abit of renaming to make what they do clearer. 
	/// </summary>

	public static LightSource sun;
	public bool lightOn = true;
	public float sourceSize;
	public Color sourceColor,offColor;

	public Transform bottomLeft,topRight;
	public RoomScript roomIAmIn;
	public SpriteRenderer sr;
	public GameObject myLightRenderer;
	[Range(0.01f,0.5f)]
	public float lightIntensity = 0.5f;
	public PolygonCollider2D myCol;

	public List<TileBase> tilesInRangeOfLight;
	public List<Vector3> shadowPoints;
	public Sprite bulbOn, bulbOff;


	public void setSwitches()
	{
		Lightswitch[] l = FindObjectsOfType<Lightswitch> ();
		foreach (Lightswitch ls in l) {
			if (shouldIBeUsedBySwitch (ls.gameObject) == true) {
				ls.addLightToSource (this);
			}
		}
	}

	bool shouldIBeUsedBySwitch(GameObject target)
	{
		Vector3 origin =target.transform.position;
		//Physics2D.queriesStartInColliders = true;

		Vector3 heading = this.transform.position - origin;
		RaycastHit2D ray = Physics2D.Raycast (origin, heading,Vector3.Distance(this.transform.position,target.transform.position));
		//BuildingScript b = LevelController.me.getBuildingPosIsIn (this.transform.position);




		if (ray.collider == null || Vector2.Distance(ray.point,target.transform.position)<1 && ray.collider.gameObject.tag != "Walls" && ray.collider.gameObject.tag!="Door" && Vector2.Distance(this.transform.position,target.transform.position)<5) {
			//			//////Debug.Log ("No ray hit");

			Debug.DrawRay (origin, heading,Color.blue,1.0f);
			//////Debug.Break ();
			return true;
		} else {

			if (ray.collider.gameObject.tag == "Walls" || ray.collider.gameObject.tag=="Door") {
				
				return false;
			}
			//Debug.DrawRay (origin, heading,Color.black,5.0f);
			//////Debug.Break ();
			//Debug.DrawRay (origin, heading, Color.yellow,5.0f);

			return false;


			////////Debug.Log (ray.collider.gameObject.name);
		}
	}

	public void addTileToLightsource(TileBase t)
	{
		if (tilesInRangeOfLight == null) {
			tilesInRangeOfLight = new List<TileBase> ();
		}
		tilesInRangeOfLight.Add (t);
	}

	/// <summary>
	/// Gets all the light sources and updates them in order (unlit, then lit) done in a coroutine so we can have a frames delay between each so the lag spike is less noticable. 
	/// </summary>
	public static void UpdateLightMeshes()
	{
		FindObjectOfType<LightSource> ().StartCoroutine ("updateMesh");
	}

	public void debugSwitch()
	{
		lightOn = !lightOn;

		StartCoroutine ("updateMesh");
	}


	IEnumerator updateMesh()
	{
		LightSource[] lights = FindObjectsOfType<LightSource> ();

		List<LightSource> inOrder = new List<LightSource> ();

		if (LightSource.sun.lightOn == true) {

			foreach (LightSource l in lights) {
				if (l.lightOn == false && l.myType == lightType.lightbulb) {
					inOrder.Add (l);
				}
			}

			foreach (LightSource l in lights) {
				if (l.myType == lightType.sun && l.lightOn == true) {
					inOrder.Add (l);
				}
			}

			foreach (LightSource l in lights) {
				if (l.lightOn == true && l.myType == lightType.lightbulb) {
					inOrder.Add (l);
				}
			}



		} else {
			
			foreach (LightSource l in lights) {
				if (l.lightOn == false && l.myType == lightType.lightbulb) {
					inOrder.Add (l);
				}
			}

			foreach (LightSource l in lights) {
				if (l.myType == lightType.sun && l.lightOn == false) {
					inOrder.Add (l);
				}
			}

			foreach (LightSource l in lights) {
				if (l.lightOn == true && l.myType == lightType.lightbulb) {
					inOrder.Add (l);
				}
			}

		



		}



		lights = inOrder.ToArray ();



		//yield return new WaitForEndOfFrame ();

		foreach (LightSource l in lights) {
			l.createRoomMesh ();
		}
		yield return new WaitForEndOfFrame ();

	}

	void Awake()
	{
		Vector3 pos = new Vector3 (Mathf.Round (this.transform.position.x), Mathf.Round (this.transform.position.y), this.transform.position.z);
		pos = new Vector3 (pos.x + 0.5f, pos.y, pos.z);
		this.transform.position = pos;


		if (sun == null) {
			if (myType == lightType.sun) {
				sun = this;

			}
		}

		if (lightOn == true) {
			
			this.gameObject.tag = "OnLight";
			this.gameObject.layer = 19;

		} else {
			this.gameObject.tag = "OffLight";
			this.gameObject.layer = 18;
		}
		sr = this.GetComponentInChildren<SpriteRenderer> ();
		/*if (myLightRenderer == null) {
			myLightRenderer = (GameObject)Instantiate (new GameObject (), this.transform);
			myLightRenderer.name = this.gameObject.name + " light texture display";
			sr = myLightRenderer.AddComponent<SpriteRenderer> ();
			sr.sortingOrder = 13;
			sr.sprite = getSprite ();
			float xScale = Mathf.Abs (bottomLeft.position.x - topRight.position.x);
			float yScale = Mathf.Abs (bottomLeft.position.y - topRight.position.y);
			myLightRenderer.transform.localScale = new Vector3 (xScale, yScale, 1);
			myLightRenderer.transform.position = bottomLeft.position + (topRight.position - bottomLeft.position) / 2;
		}*/
	}

	void Start()
	{
		if (myType == lightType.sun) {
			curAlpha = TimeScript.me.getSunColor ().a;
			//StartCoroutine ("sunTileChange");

		}
		createRoomMesh ();
		//setSwitches ();
	}

	public void setSun()
	{
		Color c = getFilteredColor ();
		manualLightMarker.color = c;
	}

	IEnumerator refreshColliderDetails()
	{
		myCol.enabled = false;
		myCol.enabled = true;
		yield return new WaitForSeconds (1.0f);
		StartCoroutine ("refreshColliderDetails");
	}

	public void initialise()
	{
		if (bottomLeft == null) {
			bottomLeft = Instantiate (new GameObject (), this.gameObject.transform).transform;
			bottomLeft.transform.position = transform.position + new Vector3 (-1, -1, 0);
		}

		if (topRight == null) {
			topRight = Instantiate (new GameObject (), this.gameObject.transform).transform;
			topRight.transform.position = transform.position + new Vector3 (1, 1, 0);

		}
	}

	void Update()
	{
		
		if (lightOn == true) {
			sr.sprite = bulbOn;
		} else {
			sr.sprite = bulbOff;

		}

	}

	public RoomScript getMyRoom()
	{
		if (roomIAmIn == null) {
			roomIAmIn = LevelController.me.getRoomObjectIsIn (this.gameObject);
		}
		return roomIAmIn;
	}



	public Vector3 getDirectionFromSource(GameObject wantingDir)
	{
		Vector3 targetDir = wantingDir.transform.position - transform.position;
		return targetDir;

	}

	public float distanceFromSource(GameObject target)
	{
		return Vector3.Distance (this.transform.position, target.transform.position);
	}



	public bool lineOfSightToTarget(GameObject target)
	{
		Vector3 origin = this.transform.position;
		Vector3 heading = target.transform.position - origin;
		RaycastHit2D ray = Physics2D.Raycast (origin, heading, Vector3.Distance(this.transform.position,target.transform.position));
		Debug.DrawRay (origin, heading,Color.yellow);

		if (ray.collider == null) {
			return true;
			////////Debug.Log("Light ray hit nothing");

		} else {
			//			//////Debug.Log ("Ray hit object with tag " + ray.collider.gameObject.tag);
			////////Debug.Log("Light ray hit something + " +ray.collider.gameObject.name);
			if (ray.collider.gameObject.tag == "Walls") {
				return false;
			} else {
				return true;

			}
		}
	}




	public bool areWeInRangeOfSource(GameObject target)
	{
		if (distanceFromSource (target) < (sourceSize / 2)) {
			return true;
		} else {
			return false;
		}
	}

	public bool shouldWeAlterShadow(GameObject target){

		if (lightOn == false) {
			return false;
		}

		if (areWeInRangeOfSource (target)==true && lineOfSightToTarget (target)==true) {
			////////Debug.Log ("Can draw shadow");
			return true;
		} else {

		//	//////Debug.Log ("Can't draw shadow");
			return false;
		}
	}

	//testing for working out the shape of the room
	public Material shaderMaterial;
	MeshRenderer m;
	public MeshFilter viewMeshFilter;
	public Mesh viewMesh;

	public float viewRadius;

	[Range(0,360)]
	public float viewAngle=360;
	public float meshResolution = 0.5f;
	public float maxLightDistance=15;
	public LayerMask targetMask;
	public LayerMask obstacleMask,obstacleMask2;
	public List<Vector2> points;
	public lightType myType;

	public float curAlpha=0.0f,targetAlpha=0.0f;


	public float alphaTimer = 0.01f;
	Color getFilteredColor()
	{
		Color c = TimeScript.me.getSunColor ();
		alphaTimer -= Time.deltaTime;
		if (alphaTimer <= 0) {
			targetAlpha = c.a;
			if (curAlpha < targetAlpha) {
				curAlpha += 0.01f;
			} else if (curAlpha > targetAlpha) {
				curAlpha -= 0.01f;
			}
			alphaTimer = 0.01f;
		}
		return new Color (c.r, c.g, c.b, curAlpha);
	}



	void createRoomMesh()
	{
		//decideWhichTilesAreInRangeOfLight ();
		if (myType==lightType.lightbulb) {
			if (lightOn == true) {
				setTilesToLit ();
			} else {
				setTilesToDark ();
			}
		} else if(myType==lightType.sun){
			setTilesToSun ();
		}

	}

	public void setTilesToSun()
	{
		foreach (Vector3 pos in lightingTilePositions) {
			Vector3Int conv = new Vector3Int (Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), 0);
			LevelTilemapController.me.lighting.SetTile (conv, LevelTilemapController.me.outdoorLight);
		}
	}




	void recursivlyDecideTilesInRange()
	{
		tilesInRangeOfLight = new List<TileBase> ();
		shadowPoints = new List<Vector3> ();
		lightingTilePositions = new List<Vector3> ();

		if (myType == lightType.sun) {
			setSunTiles ();
		} else {
			decideTile (new Vector3Int (Mathf.RoundToInt (this.transform.position.x), Mathf.RoundToInt (this.transform.position.y), 0));
		}
		Debug.Log ("calculated lights recursivly");
	}
	public Tilemap manualLightMarker;
	void setSunTiles()
	{
		LightSource[] sources = FindObjectsOfType<LightSource> ();
		if (LevelController.me == null) {
			LevelController.me = FindObjectOfType<LevelController> ();
		}



		for (float x = this.transform.position.x - 500; x < this.transform.position.x + 500; x += 1) {
			for (float y = this.transform.position.y - 500; y < this.transform.position.y + 500; y += 1) {
				Vector3Int pos = new Vector3Int (Mathf.RoundToInt (x), Mathf.RoundToInt (y), 0);
				if (manualLightMarker.GetTile (pos) == null) {

				} else {
					lightingTilePositions.Add (pos);

				}


			}
		}
		Debug.Log ("There are " + lightingTilePositions.Count + " tiles affected by the sun");
	}

	public bool lineOfSightToLightSource(Vector3 pos)
	{
		bool physicStore = Physics2D.queriesStartInColliders;
		Physics2D.queriesStartInColliders = false;
		//
		if(lineOfSightToTargetWithNoColliderForPathfin (pos) ==false )
		{
			return false;
		}
		Physics2D.queriesStartInColliders = physicStore;

		return true;
	}

	void decideTile(Vector3Int pos)
	{

		//
		if (Vector3Int.Distance (pos, (new Vector3Int (Mathf.RoundToInt (this.transform.position.x), Mathf.RoundToInt (this.transform.position.y), 0))) > maxLightDistance||lineOfSightToLightSource(pos)==false){
			Debug.Log (pos.ToString () + " is not a valid position");
			//return;
		} else {
			if (myType == lightType.lightbulb) {
				TileBase tb = LevelTilemapController.me.floor.GetTile (pos);
				if (LevelController.me == null) {
					LevelController.me = FindObjectOfType<LevelController> ();
					RoomScript[] rooms = FindObjectsOfType<RoomScript> ();
					LevelController.me.roomsInLevel = rooms;
				}
				RoomScript r2 = LevelController.me.getRoomPosIsIn (pos);

				if (tb == null ) {

				} else {

					Vector3 lastPosAdded = new Vector3 (pos.x , pos.y, 0);

					if (lightingTilePositions.Contains (lastPosAdded) == true) {
						//return;
						Debug.Log (pos.ToString () + " Tile was already in");
					} else {
						TileBase t = LevelTilemapController.me.walls.GetTile (pos);


						tilesInRangeOfLight.Add (tb);
						lightingTilePositions.Add (new Vector3 (pos.x , pos.y, 0));


						//if (t == null ) {
							//decideTile (pos + Vector3Int.up);
							//decideTile (pos - (Vector3Int.up));

							//decideTile (pos + Vector3Int.right);
							//decideTile (pos- (Vector3Int.right));

							decideTile (new Vector3Int (pos.x + 1, pos.y, pos.z));
							decideTile (new Vector3Int (pos.x - 1, pos.y, pos.z));
							decideTile (new Vector3Int (pos.x, pos.y + 1, pos.z));
							decideTile (new Vector3Int (pos.x, pos.y - 1, pos.z));
					//	} 
					}
					

				}
			} else {

			}
		}
	}

	bool loopBreak = false;
	public List<Vector3> lightingTilePositions;
	public void editorGetTilesLightTouches()
	{
		LevelTilemapController.me = FindObjectOfType<LevelTilemapController>();

		recursivlyDecideTilesInRange ();
		if (lightOn == true) {
			setTilesToLit ();
		} else {
			setTilesToDark ();
		}
		return;

		tilesInRangeOfLight = new List<TileBase> ();
		shadowPoints = new List<Vector3> ();
		lightingTilePositions = new List<Vector3> ();
		List<Vector3> bounds = new List<Vector3> ();
		//RoomScript r = LevelController.me.getRoomObjectIsIn (this.gameObject);
		//BuildingScript b = LevelController.me.getBuildingPosIsIn (this.transform.position);
		LevelTilemapController.me = FindObjectOfType<LevelTilemapController>();
		Vector3 lastPosAdded = Vector3.zero;
		if (myType == lightType.lightbulb) {
			for (float x = this.transform.position.x - (maxLightDistance); x < this.transform.position.x + (maxLightDistance-0.5f); x += 0.5f) {
				for (float y = this.transform.position.y - (maxLightDistance); y < this.transform.position.y + (maxLightDistance); y += 0.5f) {
					Vector3 pos = new Vector3 (x, y, 0);

					//RoomScript r2 = LevelController.me.getRoomPosIsIn (pos);
					//BuildingScript b2 = LevelController.me.getBuildingPosIsIn (pos);
					//if (b2 == null) {
					//LevelTilemapController.me.lighting.SetTile (new Vector3Int (Mathf.RoundToInt (x), Mathf.RoundToInt (y), 0), LevelTilemapController.me.evening);
					///	continue;
					//} 
					if (lineOfSightToTargetWithNoColliderForPathfin (pos) == true) {

						TileBase tb = LevelTilemapController.me.floor.GetTile (new Vector3Int (Mathf.RoundToInt(x),Mathf.RoundToInt (y), 0));

						if (tb == null) {

						} else {
							//	if(new Vector3 (Mathf.RoundToInt(x),Mathf.RoundToInt (y), 0)!=lastPosAdded)
							//{
							tilesInRangeOfLight.Add (tb);
							lightingTilePositions.Add (new Vector3 (Mathf.RoundToInt(x-0.5f),Mathf.RoundToInt (y), 0));
							lastPosAdded = new Vector3 (Mathf.RoundToInt (x-0.5f), Mathf.RoundToInt (y), 0);
							//}
						}
						//////Debug.Break ();

					} 

					//if (loopBreak == true) {
					//	loopBreak = false;
					//	break;
					//}
				}
			}
		} else if (myType == lightType.sun) {
			for (float x = this.transform.position.x - 500; x < this.transform.position.x + 500; x += 1) {
				for (float y = this.transform.position.y - 500; y < this.transform.position.y + 500; y += 1) {
					Vector3Int pos = new Vector3Int (Mathf.RoundToInt (x), Mathf.RoundToInt (y), 0);
					BuildingScript b2 = LevelController.me.getBuildingPosIsIn (pos);

					if (LevelTilemapController.me.floor.GetTile (pos) == null) {

					} else {
						if (b2==null) {
							lightingTilePositions.Add (pos);
						}
					}
				}
			}
		}


		if (lightOn == true) {
			setTilesToLit ();
		} else {
			setTilesToDark ();
		}
	}


	public bool lineOfSightToTargetWithNoColliderForPathfin(Vector3 target){
		Vector3 origin =target;
		Physics2D.queriesStartInColliders = true;
		origin = new Vector3 (target.x +0.5f , target.y+0.5f , 0.0f);

		Vector3 heading = this.transform.position - origin;
		RaycastHit2D ray = Physics2D.Raycast (origin, heading,Vector3.Distance(this.transform.position,target),obstacleMask);
		//BuildingScript b = LevelController.me.getBuildingPosIsIn (this.transform.position);

		Vector3Int p = new Vector3Int (Mathf.RoundToInt (target.x), Mathf.RoundToInt (target.y), 0);
		TileBase t = LevelTilemapController.me.nonWalkable.GetTile (p);


		if (ray.collider == null) {
			//			//////Debug.Log ("No ray hit");

			Debug.DrawRay (origin, heading,Color.blue,10.0f);
			//////Debug.Break ();
			return true;
		} else {
			Debug.DrawRay (origin, heading,Color.red,10.0f);

			if (ray.collider.gameObject.tag == "Walls" || ray.collider.gameObject.tag=="Door") {


				Vector3Int pos = new Vector3Int (Mathf.RoundToInt( target.x),Mathf.RoundToInt( target.y), 0);

				return false;
			}

			return false;

		}
	}

	public void setTilesToLit()
	{
		foreach (Vector3 pos in lightingTilePositions) {
			Vector3Int conv = new Vector3Int (Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), 0);
			LevelTilemapController.me.lighting.SetTile (conv, LevelTilemapController.me.lit);
		}
	}

	public void setTilesToDark()
	{
		foreach (Vector3 pos in lightingTilePositions) {
			Vector3Int conv = new Vector3Int (Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), 0);
			if (myType != lightType.sun) {
				if (sun.manualLightMarker.GetTile (conv) == null) {
					LevelTilemapController.me.lighting.SetTile (conv, LevelTilemapController.me.dark);

				} else {
					LevelTilemapController.me.lighting.SetTile (conv, null);

				}
			}

		}
	}


}
public enum lightType{
	lightbulb,
	sun
}