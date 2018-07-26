using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightswitch : MonoBehaviour {
	/// <summary>
	/// Controls a lightswitch in the game, lightswitches and lights paired via the level editor. 
	/// </summary>

	public RoomScript roomIAmIn;
	public List<LightSource> sourcesIAlter;
	SpriteRenderer sr;
	public Sprite on,off;

	bool initailised=false;
	// Use this for initialization
	void Start () {
		sr = this.gameObject.GetComponent<SpriteRenderer> ();
		if (initailised == false) {
			roomIAmIn = LevelController.me.getRoomObjectIsIn (this.gameObject);
			//sourcesIAlter = new List<LightSource> ();
			LightSource[] sources = FindObjectsOfType<LightSource> ();

			initailised = true;
		}
	}

	public void addLightToSource(LightSource l)
	{
		if (sourcesIAlter == null) {
			sourcesIAlter = new List<LightSource> ();
		}
		sourcesIAlter.Add (l);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void turnOffLights()
	{
		foreach (LightSource l in sourcesIAlter) {
			l.lightOn = false;
		}
		LightSource.UpdateLightMeshes ();
		sr.sprite = off;
	}

	public void turnOnLights()
	{
		foreach (LightSource l in sourcesIAlter) {
			l.lightOn = true;
		}
		LightSource.UpdateLightMeshes ();

		sr.sprite = on;
	}


}
