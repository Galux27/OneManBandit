using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for controlling a police car, follows a set path and then creates police at the end of it 
/// </summary>
public class PoliceCarScript : MonoBehaviour {
	public List<Transform> copSpawnPoints;
	public List<GameObject> pointsToRaycastFrom;
	public float timeTillStop = 10.0f;
	public bool spawnCops = false, spawnedCops = false,swat=false;

	public List<Transform> myRoute;
	public Transform destination;
	public int counter =0;
	public float timer = 5.0f;

	public AudioClip engine,braking,siren;
	AudioController au;
	bool setEngine=false;

	Rigidbody2D rid;
	// Use this for initialization
	void Start () {
		rid = this.GetComponent<Rigidbody2D> ();
		au = this.GetComponent<AudioController> ();
		destination = myRoute [myRoute.Count - 1];
		au.playSoundOnLoop (siren);
		if (LevelController.me.copRaiseAlarmLoc == null) {
			LevelController.me.copRaiseAlarmLoc = copSpawnPoints [0];
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (counter > myRoute.Count-1) {
			counter = myRoute.Count-1;
		}

		//if the police car gets stuck on anything then it counts down a timer and when it reaches 0 it spawns the police regardless of whether its reached the end of its path.
		rayObjectDetect ();
		if (raysHitAnything == true && spawnCops==false && inBoundsOfMap()==true) {
			timeTillStop -= Time.deltaTime;
		}

		if (timeTillStop <= 0 || Vector3.Distance(destination.position,this.transform.position)<5) {
			spawnCops = true;
			au.stopLoopingAudio (siren);
		}

		if (spawnCops == true && spawnedCops == false) {
			spawnCopsAtCar ();
		}
		timer -= Time.deltaTime;

		try{
			if (areWeAtDestination () == false && inBoundsOfMap()) {
				setNodesNearMeToUnwalkable ();
			}
		}
		catch{

		}
			
		if (areWeAtDestination () == false && spawnedCops == false && raysHitAnything == false) {
			moveToPoint ();

			if (setEngine == false) {
				au.playSoundOnLoop (engine);
				setEngine = true;
			}
		} else {
			moveToPoint ();

			if (setEngine == true) {
				
				au.stopLoopingAudio (engine);
				au.playSound (braking);
				setEngine = false;
			}
			brake ();
		}
	}


	void moveToPoint()
	{
		Vector3 dir = myRoute[counter].position - transform.position;
		//dirToMove = dir;
		//transform.Translate (dir.normalized * carSpeed() * Time.deltaTime,Space.World);
		rid.velocity = new Vector2(dir.normalized.x * carSpeed(),dir.normalized.y*carSpeed());

		Vector3 rot = new Vector3(0, 0, Mathf.Atan2((myRoute[counter].position.y - transform.position.y),myRoute[counter].position.x - transform.position.x)) * Mathf.Rad2Deg;
		rot = new Vector3(rot.x, rot.y, rot.z-90);//add 90 to make the player face the right way (yaxis = up)
		//rid.transform.eulerAngles = rot;
		this.transform.rotation =Quaternion.Euler(rot);// Quaternion.Slerp(this.transform.rotation,Quaternion.Euler(rot),5.0f*Time.deltaTime);// Quaternion.Euler(rot); //(INSTA ROTATION)

		if (areWeAtPoint () == true) {
			counter++;
		}
	}

	bool inBoundsOfMap()
	{
		Vector3 pos = Pathfinding.me.pathNodes [0, 0].worldPos;
		
		if (this.transform.position.x > pos.x + 3.0f && this.transform.position.y > pos.y + 3.0f) {
			return true;
		} else {
			return false;
		}
	}

	bool areWeAtPoint()
	{
		return Vector3.Distance (this.transform.position, myRoute [counter].position) < 2;
	}

	bool areWeAtDestination()
	{
		return Vector3.Distance (this.transform.position, destination.position) < 2;
	}

	void spawnCopsAtCar()
	{		
		//setNodesNearMeToUnwalkable ();
		rid.bodyType=RigidbodyType2D.Static;

		reEnableNodeNearSpawns ();
		foreach (Transform t in copSpawnPoints) {
			if (swat == false) {
				GameObject g = (GameObject) Instantiate (CommonObjectsStore.me.cop, t.transform.position, this.transform.rotation);
				PoliceController.me.copsInLevel.Add (g);
				g.SetActive (true);

			} else {
				GameObject g = (GameObject) Instantiate (CommonObjectsStore.me.swat, t.transform.position, this.transform.rotation);
				PoliceController.me.swatInLevel.Add (g);
				g.SetActive (true);
			}
		}
		spawnedCops = true;
	}

	bool raysHitAnything = false;
	void rayObjectDetect()
	{
		raysHitAnything = false;
		foreach (GameObject g in pointsToRaycastFrom) {
			RaycastHit2D ray = Physics2D.Raycast (g.transform.position, g.transform.position - (g.transform.position - transform.up),2.0f);
			Debug.DrawRay (g.transform.position, g.transform.position - (g.transform.position - transform.up), Color.cyan);
			if (ray.collider == null) {

			} else {
				//if (ray.collider.gameObject.tag == "Car" ) {
				raysHitAnything = true;
				//}
			}
		}
	}
	public List<WorldTile> nodesISetToUnwalkable;

	void resetNodes()
	{
		foreach (WorldTile wt in nodesISetToUnwalkable) {
			wt.walkable = true;
			wt.GetComponent<SpriteRenderer> ().color = Color.white;
			ThreadedPathfindInterface.me.nodes [wt.gridX, wt.gridY].walkable = true;

		}
		nodesISetToUnwalkable.Clear ();
	}

	void reEnableNodeNearSpawns()
	{
		foreach (Transform t in copSpawnPoints) {
			WorldTile wt = WorldBuilder.me.getNearest (this.transform.position);
			if (wt.walkable == false) {
				//Debug.Log (wt.gameObject.name + " set to unwalkable");
				wt.GetComponent<SpriteRenderer> ().color = Color.cyan;
				if (wt.modifier >= 10000) {
					wt.modifier -= 10000;
					ThreadedPathfindInterface.me.nodes [wt.gridX, wt.gridY].modifier -=10000;

				}
				//nodesISetToUnwalkable.Add (wt);
			}
		}
	}

	void setNodesNearMeToUnwalkable()
	{
		try{
			if (nodesISetToUnwalkable == null) {
				nodesISetToUnwalkable = new List<WorldTile> ();
			} else {
				resetNodes ();
			}
			WorldTile pos = WorldBuilder.me.getNearest (this.transform.position);

			for (int x = -2; x < 2; x++) {
				for (int y = -2; y < 2; y++) {
					WorldTile wt = WorldBuilder.me.worldTiles [pos.gridX + x, pos.gridY + y].GetComponent<WorldTile>();

					if (wt.walkable == true) {
						//Debug.Log (wt.gameObject.name + " set to unwalkable");
						wt.GetComponent<SpriteRenderer> ().color = Color.blue;
						wt.modifier += 10000;
						nodesISetToUnwalkable.Add (wt);
						ThreadedPathfindInterface.me.nodes [wt.gridX, wt.gridY].modifier +=10000;
					}
				}
			}
		}catch{
			reEnableNodeNearSpawns ();
		}
	}


	float speed = 0.0f;
	public float carSpeed()
	{
		if (setEngine == true) {
			if (speed < 15.0f) {
				speed += Time.deltaTime;
			}
		}
		return speed;
	}
	float brakeTimer = 0.1f;
	void brake()
	{
		if (speed > 0) {
			brakeTimer -= Time.deltaTime;
			if (brakeTimer <= 0) {
				speed -= 5;
				brakeTimer = 0.1f;
			}
		} else {
			speed = 0;
		}
	}
}
