using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
public class PhoneTab_WorldMaps : PhoneTab {
	public static PhoneTab_WorldMaps me;
	public RawImage background;
	public Texture2D mapBase,sample;
	public int sizeX,sizeY;
	public Button zoomInB, zoomOutB;

	public Tilemap floor;
	public List<Tilemap> markerLayers;

	public List<TileBase> tiles;
	public List<Color> tileColor;

	bool generated=false;
	void Awake()
	{
		me = this;
		generateBackgroundTexture ();
	}

	void OnEnable()
	{
		me = this;
		generateBackgroundTexture ();
	}

	public override void disablePhoneTab()
	{
		//disable all the UI elements needed
		zoomInB.gameObject.SetActive(false);
		zoomOutB.gameObject.SetActive (false);
		background.enabled = false;
		active = false;
	}

	public override void enablePhoneTab()
	{
		//enable all ui elements
		background.enabled=true;
		zoomInB.gameObject.SetActive(true);
		zoomOutB.gameObject.SetActive (true);
		active = true;
	}

	public override void onUpdate()
	{
		if (LevelController.me.playerFloor == null) {

		} else {
			if (mapBase == null) {
				generateBackgroundTexture ();
			}
			sampleMap ();
			background.texture = sample;
		}
	}

	Color getColorFromTile(TileBase tb){
		for(int x = 0;x<tiles.Count;x++)
		{
			TileBase tb2 = tiles [x];
			if (tb.name == tb2.name) {
				return tileColor [x];
			}
		}
		return Color.cyan;
	}

	BuildingScript[] buildings;
	bool foundBuildings=false;

	bool isPosInBuilding(Vector3 pos)
	{
		if (foundBuildings == false) {
			buildings = FindObjectsOfType<BuildingScript> ();
			foundBuildings = true;
		}
		foreach (BuildingScript b2 in buildings) {
			if (b2.isOutdoors == true) {
				continue;
			}


			if (b2.isPosInRoom (pos) == true) {
				return true;
			}
		}
		return false;
	}

	void generateBackgroundTexture() //takes a while to generate textures, do on level load for all floors and cache
	{
		if (generated == true) {
			return;
		}

		sizeX = (int)Mathf.Abs (LevelController.me.playerFloor.bottomLeft.position.x - LevelController.me.playerFloor.topRight.position.x) + 1;
		sizeY = (int)Mathf.Abs (LevelController.me.playerFloor.bottomLeft.position.y - LevelController.me.playerFloor.topRight.position.y) + 1;
		Color[,] colors = new Color[sizeX,sizeY];
		int xInd =  0;
		int yInd = 0;
		float r = Random.Range (0.01f, 0.1f);

		for (int x = (int)LevelController.me.playerFloor.bottomLeft.position.x; x < sizeX; x++) {
			for(int y = (int)LevelController.me.playerFloor.bottomLeft.position.y; y<sizeY;y++)
			{
				Vector3 worldPos = new Vector3 (x, y, 0);
				Vector3Int posInFloor = floor.WorldToCell (worldPos);

				bool setWhite = false;
					if (setWhite == false) {
						if (floor.GetTile (posInFloor) == null) {
							if (xInd < sizeX && yInd < sizeY) {	

								if (colors [xInd, yInd]==Color.white) {
									colors [xInd, yInd] = Color.black;
								}
							}
						} else {
							if (xInd < sizeX && yInd < sizeY) {	

								if (isPosInBuilding (worldPos) == true) {
									if (colors [xInd, yInd]==Color.white) {
										
										colors [xInd, yInd] = Color.grey;
									}

								} else {
									colors [xInd, yInd] = getColorFromTile (floor.GetTile (posInFloor));
									foreach (Tilemap t in markerLayers) {
										if (t.GetTile (posInFloor) == null) {

										} else {
											if (xInd < sizeX && yInd < sizeY) {	
												if (colors [xInd, yInd]==Color.white) {
														
													colors [xInd, yInd] -= new Color (r, r, r, 0.0f);
												}
											}
											break;
										}
									}
								}

								setWhite = true;
									
							}
						}
					}
				

				yInd += 1;

			}
			xInd += 1;
			yInd = 0;

		}
		Texture2D tex = new Texture2D (sizeX, sizeY, TextureFormat.RGBA32, false);
		for (int x = 0; x < sizeX; x++) {
			for (int y = 0; y < sizeY; y++) {
				tex.SetPixel (x, y, colors [x, y]);
			}
		}
		tex.filterMode = FilterMode.Point;
		tex.Apply ();
		mapBase = tex;
		createBuildingIconMap (sizeX,sizeY,tex);
		generated = true;
	}

	Color[,] buildingArr;
	public Texture2D buildingTex;
	void createBuildingIconMap(int sizeX,int sizeY,Texture2D og)
	{
		buildingArr = new Color[sizeX,sizeY];
		int xInd =  0;
		int yInd = 0;
		for (int x = (int)LevelController.me.playerFloor.bottomLeft.position.x; x < sizeX; x++) {
			for (int y = (int)LevelController.me.playerFloor.bottomLeft.position.y; y < sizeY; y++) {
				Vector3 worldPos = new Vector3 (x, y, 0);

				RaycastHit2D[] rays = Physics2D.RaycastAll (worldPos, Vector2.zero, 1.0f);
				foreach (RaycastHit2D ray in rays) {
					if (ray.collider == null) {

					} else {
						if (ray.collider.gameObject.tag == "Walls") {
						/*	MapIcon mi = ray.collider.gameObject.transform.parent.GetComponent<MapIcon> ();
							if (mi == null) {

							} else {
								try {
									buildingArr [xInd, yInd] = mi.getNextPixel ();
								} catch {

								}
							}*/
						} else {
							MapIcon mi = ray.collider.gameObject.transform.root.GetComponent<MapIcon> ();
							if (mi == null) {

							} else {
								try {
									buildingArr [xInd, yInd] = mi.getNextPixel ();
								} catch {

								}
							}
						}
					}
				}
				yInd++;
			}
			xInd++;
			yInd = 0;
		}
		Texture2D tex = new Texture2D (sizeX, sizeY, TextureFormat.RGBA32, false);
		for (int x = 0; x < sizeX; x++) {
			for (int y = 0; y < sizeY; y++) {
				if (buildingArr [x, y] == null || buildingArr[x,y]==Color.clear) {
					tex.SetPixel (x, y, og.GetPixel (x, y));
				} else {
					tex.SetPixel (x, y, buildingArr [x, y]);
				}
			}
		}
		tex.filterMode = FilterMode.Point;
		tex.Apply ();
		mapBase = tex;
	}


	public Vector2Int sampleSize = new Vector2Int (25, 25);
	void sampleMap()
	{
		Texture2D sample2 = new Texture2D (sampleSize.x, sampleSize.y, TextureFormat.RGBA32, false);
		Vector3 center = CommonObjectsStore.player.transform.position;
		int x2 = 0;
		int y2 = 0;
		for (int x = (int) center.x - sampleSize.x/2; x < center.x + sampleSize.x/2 +1; x++) {
			for (int y = (int) center.y - sampleSize.y/2; y < center.y + sampleSize.y/2 +1; y++) {
				//int xInd = (int) center.x -(int) LevelController.me.playerFloor.bottomLeft.position.x;
				//int yInd = (int) center.y -(int) LevelController.me.playerFloor.bottomLeft.position.y;


				int xInd = x- (int) LevelController.me.playerFloor.bottomLeft.position.x;
				int yInd = y-(int) LevelController.me.playerFloor.bottomLeft.position.y;
				if (xInd >= 0 && xInd <= sizeX && yInd >= 0 && yInd <= sizeY) {
					sample2.SetPixel (x2, y2, mapBase.GetPixel (xInd, yInd));
				} else {
					sample2.SetPixel (x2, y2, Color.grey);
				}
				y2++;
				//////////Debug.Log (xInd + " || " + yInd);
			}
			x2++;
			y2 = 0;
		}
		sample2.SetPixel (sampleSize.x/2, sampleSize.y/2, Color.red);
		sample2.filterMode = FilterMode.Point;
		sample2.Apply ();
		sample = sample2;
	}

	public void zoomOut()
	{
		int x = sampleSize.x;
		int y = sampleSize.y;

		if (x + 2 < sizeX && y + 2 < sizeY) {
			x+=2;
			y+=2;
		}

		sampleSize = new Vector2Int (x, y);
	}

	public void zoomIn()
	{
		int x = sampleSize.x;
		int y = sampleSize.y;

		if (x - 2 >10 && y - 2 >10) {
			x-=2;
			y-=2;
		}

		sampleSize = new Vector2Int (x, y);
	}

}