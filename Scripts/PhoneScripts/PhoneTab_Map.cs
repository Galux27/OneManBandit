using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
public class PhoneTab_Map : PhoneTab {
	public static PhoneTab_Map me;
	public RawImage background;
	public Texture2D mapBase,sample;
	public int sizeX,sizeY;
	public Button zoomInB, zoomOutB;

	public Tilemap floor;
	public List<Tilemap> markerLayers;
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

	void generateBackgroundTexture() //takes a while to generate textures, do on level load for all floors and cache
	{
		sizeX = (int)Mathf.Abs (LevelController.me.playerFloor.bottomLeft.position.x - LevelController.me.playerFloor.topRight.position.x) + 1;
		sizeY = (int)Mathf.Abs (LevelController.me.playerFloor.bottomLeft.position.y - LevelController.me.playerFloor.topRight.position.y) + 1;
		Color[,] colors = new Color[sizeX,sizeY];
		int xInd =  0;
		int yInd = 0;
		for (int x = (int)LevelController.me.playerFloor.bottomLeft.position.x; x < sizeX; x++) {
			for(int y = (int)LevelController.me.playerFloor.bottomLeft.position.y; y<sizeY;y++)
			{
				bool setWhite = false;
				foreach (Tilemap t in markerLayers) {
					if (setWhite == false) {
						if (t.GetTile (new Vector3Int (x, y, 0)) == null) {
							if (xInd < sizeX && yInd < sizeY) {	
								//							//////Debug.Log ("x = " + xInd + " y = " + yInd);
								colors [xInd, yInd] = Color.blue;
								//break;
							}
						} else {
							if (xInd < sizeX && yInd < sizeY) {	
								////////Debug.Log ("x = " + xInd + " y = " + yInd);
								colors [xInd, yInd] = Color.white;
								setWhite = true;
								//break;
							}
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
					sample2.SetPixel (x2, y2, Color.blue);
				}
				y2++;
				////////Debug.Log (xInd + " || " + yInd);
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
