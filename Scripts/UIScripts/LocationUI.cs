using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LocationUI : MonoBehaviour {
	public Text myText;
	BuildingScript currentBuilding;
	RoomScript currentRoom;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//setText ();
	}

	void setText()
	{
		currentBuilding = LevelController.me.getBuildingPosIsIn (CommonObjectsStore.player.transform.position);
		currentRoom = LevelController.me.roomPlayerIsIn;
		string st = "";
		st = SceneManager.GetActiveScene ().name + "\n";

		if (currentBuilding == null) {
			st += "Outdoors" + "\n";

		} else {
			st += currentBuilding.buildingName + "\n";
		}
		if (currentRoom == null) {
			st += "Outdoors";
		} else {
			st += currentRoom.roomName;
		}
		myText.text = st;
	}
}
