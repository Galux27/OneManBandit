using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MapControlScript : MonoBehaviour {

	/// <summary>
	/// Class that controls the world map (displaying it and movement)
	/// </summary>

	public static MapControlScript me;
	public bool displayMap=false;
	public GameObject mapObj;
	void Awake(){
		me = this;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (displayMap == true) {
			mapObj.SetActive (true);
			moveMap ();
		} else {
			mapObj.SetActive (false);
		}
	}

	Vector3 startPos,endPos;

	void moveMap()
	{
		if (displayMap == true) {
			if (Input.GetKey (KeyCode.W)) {
				mapObj.transform.Translate (Vector3.up * Time.deltaTime * -250);
			}

			if (Input.GetKey (KeyCode.S)) {
				mapObj.transform.Translate (Vector3.up * Time.deltaTime * 250);
			}

			if (Input.GetKey (KeyCode.A)) {
				mapObj.transform.Translate (Vector3.right * Time.deltaTime * 250);
			}

			if (Input.GetKey (KeyCode.D)) {
				mapObj.transform.Translate (Vector3.right * Time.deltaTime * -250);
			}


		}
	}

	void setMousePos()
	{
		mapObj.transform.position = mapObj.transform.position + (endPos-endPos);
	}

	public void mapSwitch()
	{
		if (displayMap == true) {
			displayMap = false;
		} else {
			displayMap = true;
		}
	}
}
