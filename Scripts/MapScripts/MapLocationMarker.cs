using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MapLocationMarker : MonoBehaviour {
	public static MapLocationMarker toDisplay;
	public string locationName,sceneName;
	public bool displayIcon = false;
	public GameObject fullParent,displayParent;
	public Text displayNameText,descriptionText;
	public List<MapLocationMarker> neighbours;

	void Awake(){
		toDisplay = this;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (displayIcon == true) {
			fullParent.SetActive (true);
			displayParent.SetActive (false);
			descriptionText.text = getLocationInfo ();
		} else {
			fullParent.SetActive (false);
			displayParent.SetActive (true);
			displayNameText.text = locationName;
		}

		if (toDisplay != this && displayIcon == true) {
			displayIcon = false;
		}
	}

	public void display()
	{
		MapLocationMarker.toDisplay = this;
		displayIcon = true;
	}

	public void hide()
	{
		displayIcon = false;
	}

	public void loadLevel()
	{
		LoadScreen.loadScreen.loadGivenScene (sceneName);
		MapControlScript.me.displayMap = false;
	}

	string getLocationInfo()
	{
		return "Name: " + locationName;
	}
}
