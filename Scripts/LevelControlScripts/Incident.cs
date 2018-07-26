using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Incident : MonoBehaviour {

	public string incidentName = "";
	public GameObject objectToCreate;
	public List<GameObject> optionalObjectsToCreate;
	public int timeLimit=1;

	public GameObject board;

	bool shouldWeReactToIncident(DateTimeStore date)
	{
		bool retVal=true;

		int value = workOutNumberOfDaysSince (date);
		Debug.Log ("Incident occured " + value + " days ago");
		return value<=timeLimit;
	}


	int workOutNumberOfDaysSince(DateTimeStore date)
	{
		int numberOfDays = 0;
		int month = date.month;
		int day = date.day;
		if (date.month < TimeScript.me.month) {
			while (month != TimeScript.me.month) {
				numberOfDays += TimeScript.me.daysInMonth [month];
				day = 0;
				month++;
			}
		}


		if (date.month > TimeScript.me.month) {
			if (date.year < TimeScript.me.year) {
				numberOfDays += 365;
			}
		}

		if (date.month == TimeScript.me.month) {
			if (day < TimeScript.me.day) {
				while (day < TimeScript.me.day) {
					numberOfDays++;
					day++;
				}
			} 
		}

		return numberOfDays;
	}

	public void createIncidentReaction(int min,int hour,int day,int month,int year,Vector3 pos)
	{
		DateTimeStore date = new DateTimeStore ();
		date.min = min;
		date.hour = hour;
		date.day = day;
		date.month = month;
		date.year = year;
		if (shouldWeReactToIncident (date) == true) {
			LevelIncidentController.me.reAddIncident (incidentName, pos, date);
			GameObject g = getRelevantGameObject (pos);
			GameObject obj = (GameObject)Instantiate (getObjectToCreate (), g.transform.position, Quaternion.Euler(0,0,g.transform.rotation.eulerAngles.z));
			//create the stuff, find area for misc objects

			if (incidentName == "Window") {
				obj.GetComponent<DoorScript> ().doorOpenSpeed = 0.0f;
				obj.GetComponent<DoorScript> ().doorSoundEffect = null;
			} else if (incidentName == "FireExit" || incidentName == "GlassDoor" || incidentName == "WoodDoor") {
				DoorScript ds = obj.GetComponent<DoorScript> ();
				ds.doorOpenSpeed /= 2;
			}
			if (objectToCreate == null) {

			} else {
				createObject (pos);
			}

		} else {
			//destoy save date relating to incident and the destroyed stuff is reset.
			if (incidentName == "Window") {
				GameObject g = getRelevantGameObject (pos);
				WindowNew wn = g.GetComponent<WindowNew> ();
				//stop it displaying the smashed window
				//re enable window script, mesh and collider
				wn.window.SetActive(true);
				wn.destroyed.SetActive (false);
				wn.windowDestroyed = false;
				wn.windowClosedCol.SetActive (true);
				wn.initilised = false;
				wn.callStart ();
			} else if (incidentName == "FireExit" || incidentName == "GlassDoor" || incidentName == "WoodDoor") {
				GameObject g = getRelevantGameObject (pos);
				DoorScript ds = g.GetComponent<DoorScript> ();
				ds.enabled = true;
				ds.doorMesh.SetActive (true);
				Destroy(g.GetComponent<SpriteRenderer> ());
				ds.doorHealth = 5;
				Shadow s = g.GetComponent<Shadow> ();
				g.GetComponent<BoxCollider2D> ().enabled = true;

				foreach(GameObject gs in s.myShadows)
				{
					Destroy (gs.gameObject);
				}
				Destroy (s);
				ds.setDoorActions ();
			}
		}
	}

	GameObject getRelevantGameObject(Vector3 pos)
	{
		GameObject retVal=null;

		if (incidentName == "Window") {
			WindowNew[] windows = FindObjectsOfType<WindowNew> ();
			float distance = 999999.0f;
			foreach (WindowNew w in windows) {
				float d2 = Vector2.Distance (w.transform.position, pos);
				if (d2 < distance) {
					retVal = w.gameObject;
					distance = d2;
				}
			}
		} else if (incidentName == "FireExit" || incidentName == "GlassDoor" || incidentName == "WoodDoor") {
			DoorScript[] doors = FindObjectsOfType<DoorScript> ();
			float distance = 9999998.0f;
			foreach (DoorScript d in doors) {
				float d2 = Vector2.Distance (d.transform.position, pos);
				if (d2 < distance) {
					distance = d2;
					retVal = d.gameObject;
				}
			}
		}

		return retVal;
	}


	void createObject(Vector3 pos)
	{
		BuildingScript b = LevelController.me.getBuildingPosIsIn (pos);
		if (b == null) {
			Debug.LogError ("No building found for object position");
		} else {
			CommonObjectsStore c = FindObjectOfType<CommonObjectsStore> ();
			List<GameObject> tilesForPoint = new List<GameObject> ();
			b.getPoints ();
			foreach (WorldTile wt in b.tilesInBuilding) {
				if (wt.walkable == false) {
					continue;
				}

				float d = Vector2.Distance (wt.transform.position, pos);
				if (d > 12 || d<5) {
					continue;
				}

				tilesForPoint.Add (wt.gameObject);
			}

			if (tilesForPoint.Count == 0) {
				Debug.LogError ("No tiles were found in building");
				return;
			} else {
				GameObject pointToSpawn = null;
				float d = 999999.0f;
				bool posValid = true;
				foreach (GameObject g in tilesForPoint) {
					List<Vector3> dir = new List<Vector3> ();
					dir.Add (new Vector3 (-1, 0, 0));
					dir.Add (new Vector3 (1, 0, 0));
					dir.Add (new Vector3 (0, -1, 0));
					dir.Add (new Vector3 (0, 1, 0));

					foreach (Vector3 direction in dir) {

						RaycastHit2D ray = Physics2D.Raycast (g.transform.position, direction, 0.5f,c.maskForMelee);
						Debug.DrawRay (g.transform.position, direction * 1.0f, Color.green, 20.0f);
						if (ray.collider == null) {

						} else {
							//if (ray.collider.tag == "Walls" || ray.collider.tag == "CivilianObject" || ray.collider.tag == "Door") {
							//	posValid = false;
							//}
						}
					}
					if (posValid == true) {
						float d2 = Vector2.Distance (g.transform.position, pos);
						if (d2 < d) {
							pointToSpawn = g;
							d = d2;
						}
					}
				}
				if (pointToSpawn == null) {

				} else {
					Instantiate (objectToCreate, new Vector3 (pointToSpawn.transform.position.x, pointToSpawn.transform.position.y, 0), Quaternion.Euler (0, 0, 0));
				}
			}
		}
	}

	GameObject getObjectToCreate()
	{
		GameObject retVal=null;

		if (incidentName == "Window") {
			return board;
		} else if (incidentName == "FireExit") {
			return board;

		} else if (incidentName == "GlassDoor") {
			return board;

		} else if (incidentName == "WoodDoor") {
			return board;

		}
		return retVal;
	}
}
