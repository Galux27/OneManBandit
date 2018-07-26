using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour {

	/// <summary>
	/// Controls the spawning of cars, will need to add a way to change the car that is spawned once there are more sprites.
	/// </summary>

	public GameObject carPrefab;
	public List<GameObject> carsInWorld;
	public static CarSpawner me;
	bool carRespawn=false;
	public float timerForCarRespawn=30.0f;

	[Range(1,100)]
	public int carDensity = 30;
	NewRoad r;
	void Awake()
	{
		me = this;
		carsInWorld = new List<GameObject> ();
		r = FindObjectOfType<NewRoad> ();
	}

	// Use this for initialization
	void Start () {
		newSpawnCars ();
		//SpawnCars ();
	}
	
	// Update is called once per frame
	void Update () {
		if (carRespawn == true) {
			timerForCarRespawn -= Time.deltaTime;
			if (timerForCarRespawn <= 0) {
				newSpawnCars ();
				//	SpawnCars ();
				carRespawn = false;
				timerForCarRespawn = 30.0f;
			}
		}
	}

	void newSpawnCars()
	{
		foreach (NewRoadJunction rj in r.sectionsInTheRoad) {
			int ran = Random.Range (0, 100);
			if (ran <carDensity) {
				if (rj.startPoint == null) {

				} else {
					Debug.Log ("The start point for " + rj.ToString () + " was null");
				}
				bool spawnCar = true;
				foreach (GameObject g2 in carsInWorld) {
					if (Vector2.Distance (g2.transform.position, rj.startPoint.position) < 20.0f) {
						spawnCar = false;
					}
				}
				if (spawnCar == true) {
					GameObject g = (GameObject)Instantiate (carPrefab, rj.startPoint.position, Quaternion.Euler (0, 0, 90));
					carsInWorld.Add (g);
				}
			}
		}
	}
		

	public void destroyCars()
	{
		Debug.Log ("Destroyed cars");
		foreach (GameObject g in carsInWorld) {
			if (g == null) {
				continue;
			}

			if (Vector2.Distance (g.transform.position, CommonObjectsStore.player.transform.position) > 20.0f) {
				Destroy (g);
			}
		}
		carsInWorld = new List<GameObject> ();
		newSpawnCars ();
		//RoadFollower[] roads = FindObjectsOfType<RoadFollower> ();
		///foreach (RoadFollower r in roads) {
		//	carsInWorld.Add (r.gameObject);
		//}
		carRespawn = true;
	}
}
