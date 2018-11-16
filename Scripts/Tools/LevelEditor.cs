using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditor : MonoBehaviour {
	public GameObject roadPrefab;
	public GameObject civilianSpawnPrefab;
	public GameObject policeCarPrefab;
	public GameObject civilianActionPrefab;
	public GameObject lightPrefab, lightswitchPrefab;
	public GameObject cctvPrefab;
	public List<GameObject> enemies;
	public List<GameObject> peopleToTalkTo;
	public List<GameObject> cctvCameras;
	public List<GameObject> civilianActionsCreated;
	public List<GameObject> outOfBoundsMarkers;

	public List<GameObject> lightsourcesICreated,lightswitchesICreated;
	public List<GameObject> objectsForNature,natureObjectsCreated;
	public List<GameObject> shops;
	public void addCivilanAction(GameObject g)
	{
		if (civilianActionsCreated == null) {
			civilianActionsCreated = new List<GameObject> ();
		}
		civilianActionsCreated.Add (g);
	}


	public List<GameObject> enemiesCreated;

	public void addEnemy(GameObject g)
	{
		if (enemiesCreated == null) {
			enemiesCreated = new List<GameObject> ();
		}

		enemiesCreated.Add (g);
	}

	public void addToTalkTo(GameObject g){
		if (peopleToTalkTo == null) {
			peopleToTalkTo = new List<GameObject> ();
		}
		peopleToTalkTo.Add (g);
	}

	public void addLightSource(GameObject source)
	{
		if (lightsourcesICreated == null) {
			lightsourcesICreated = new List<GameObject> ();
		}
		lightsourcesICreated.Add (source);
	}

	public void addLightSwitch(GameObject g){
		if (lightswitchesICreated == null) {
			lightswitchesICreated = new List<GameObject> ();
		}
		lightswitchesICreated.Add (g);
	}

	public void addCCTVCamera(GameObject g)
	{
		if (cctvCameras == null) {
			cctvCameras = new List<GameObject> ();
		}
		cctvCameras.Add (g);
	}

	public void addOutOfBoundsMarker(GameObject g)
	{
		if (outOfBoundsMarkers == null) {
			outOfBoundsMarkers = new List<GameObject> ();
		}

		outOfBoundsMarkers.Add (g);
	}

	public void addNatureObjects(GameObject g)
	{
		if (natureObjectsCreated == null) {
			natureObjectsCreated = new List<GameObject> ();
		}

		natureObjectsCreated.Add (g);
	}

	public void addShop(GameObject g)
	{
		if (shops == null) {
			shops = new List<GameObject> ();
		}
		shops.Add (g);
	}
}

public enum LevelEditorTask{
	roads,
	enemies,
	patrolRoutes,
	civSpawns,
	civActions,
	police,
	lightSources,
	cctv,
	outOfBounds,
	nature,
	conversations,
	shops,
    items,
    teleport,
    events,
    background
}
