using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadFollower : MonoBehaviour {
	public Road currentRoad,lastRoad,nextRoad;
	public RoadJunction myJunction;
	public List<RoadJunction> lastJunctions;

	bool reachedEndOfRoad = false;
	public List<GameObject> pointsToRaycastFrom;

	float timer = 5.0f;

	Rigidbody2D rid;

	public AudioClip engine,braking;
	AudioController au;
	bool setEngine=false;
	// Use this for initialization
	void Start () {
		rid = this.GetComponent<Rigidbody2D> ();
		au = this.GetComponent<AudioController> ();

		lastRoad = currentRoad;
		decideJunction ();
	}

	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;
		rayObjectDetect ();
		if (shouldWeDrive () == true) {
			if (timer <= 0) {
				setNodesNearMeToUnwalkable ();
			}
			if (reachedEndOfRoad == false) {
				moveToDirection (currentRoad.getTopOfRoad ());
			} else {
				moveToDirection (myJunction.getJunctionPoint ());

			}
			if (setEngine == false) {
				au.playSoundOnLoop (engine);
				setEngine = true;
			}
		} else {
			if (timer <= 0) {
				setNodesNearMeToUnwalkable ();
			}

			if (reachedEndOfRoad == false) {
				moveToDirection (currentRoad.getTopOfRoad ());
			} else {
				moveToDirection (myJunction.getJunctionPoint ());

			}


			if (setEngine == true) {
				au.stopLoopingAudio (engine);
				//au.playSound (braking);
				setEngine = false;
			}
			brake ();
		}
		junctionMoniter ();
		directionMoniter ();
	}
	public bool headingRight=false, headingUp = false;


	void directionMoniter(){
		if (Vector3.Distance (this.transform.position, currentRoad.getTopOfRoad()) < 1.5f) {
			reachedEndOfRoad = true;
		}
	}

	void junctionMoniter()
	{
			if (Vector3.Distance (this.transform.position, myJunction.getJunctionPoint ()) < 1.5f) {
				lastRoad = currentRoad;
			reachedEndOfRoad = false;
				currentRoad = myJunction.roadGoingTo;
				decideJunction ();
			}
	}


	void decideJunction()
	{
		headingUp = false;
		headingRight = false;
		if (currentRoad.vertical == false) {
			if (currentRoad.invertRoadDirection == true) {
				if (currentRoad.getRoadDirection ().x > 0) {
					headingRight = false;
				}
			} else {
				if (currentRoad.getRoadDirection ().x > 0) {
					headingRight = true;
				}
			}
		} else {
			if (currentRoad.invertRoadDirection == true) {
				if (currentRoad.getRoadDirection ().y > 0) {
					headingUp = false;
				}
			} else {
				if (currentRoad.getRoadDirection ().y > 0) {
					headingUp = true;
				}
			}

		}


		if (currentRoad.vertical == false) {
			List<RoadJunction> possibleJunctions = new List<RoadJunction> ();
			foreach (RoadJunction r in currentRoad.myJunctions) {
				if (r.canIUseJunction () == false) {
					continue;
				}

				if (headingRight == true) {
					if (r.getJunctionPoint ().x >= this.transform.position.x) {
						possibleJunctions.Add (r);
					}
				} else {
					if (r.getJunctionPoint ().x <= this.transform.position.x) {
						possibleJunctions.Add (r);

					}
				}
			}

			if (possibleJunctions.Count == 0) {
				Debug.LogError ("Could not find any junctions horizontal");
				float dist = 999999.0f;
				RoadJunction nearest = null;
				foreach (RoadJunction j in currentRoad.myJunctions) {
					float d = Vector3.Distance (this.transform.position, j.getJunctionPoint ());
					if (d < dist) {
						nearest = j;
						dist = d;
					}
				}
				myJunction = nearest;
			} else {
				myJunction = possibleJunctions [Random.Range (0, possibleJunctions.Count)];
				lastJunctions = possibleJunctions;

			}
		} else {
			List<RoadJunction> possibleJunctions = new List<RoadJunction> ();
			foreach (RoadJunction r in currentRoad.myJunctions) {
				if (r.canIUseJunction () == false) {
					continue;
				}

				if (headingUp==true) {
					if (r.getJunctionPoint ().y >= this.transform.position.y) {
						possibleJunctions.Add (r);
					}
				} else {
					if (r.getJunctionPoint ().y <= this.transform.position.y) {
						possibleJunctions.Add (r);

					}
				}
			}
			if (possibleJunctions.Count == 0) {
				Debug.LogError ("Could not find any junctions horizontal");
				float dist = 999999.0f;
				RoadJunction nearest = null;
				foreach (RoadJunction j in currentRoad.myJunctions) {
					float d = Vector3.Distance (this.transform.position, j.getJunctionPoint ());
					if (d < dist) {
						nearest = j;
						dist = d;
					}
				}
				myJunction = nearest;
			} else {
				myJunction = possibleJunctions [Random.Range (0, possibleJunctions.Count)];
				lastJunctions = possibleJunctions;
			}
		}
		nextRoad = myJunction.roadGoingTo;


	}

	public void moveToDirection(Vector3 pos)
	{
		if (pos == null || pos == Vector3.zero) {
			return;
		}
		//////Debug.Log (this.gameObject.name + " is moving at " + getMovementSpeed ().ToString());
		Vector3 dir = pos - transform.position;
		//dirToMove = dir;
		rid.velocity = new Vector2(dir.normalized.x * carSpeed(),dir.normalized.y*carSpeed());

		//transform.Translate (dir.normalized *carSpeed() * Time.deltaTime,Space.World);
		rotateToFacePosition (pos);


		if (currentRoad.getRoadDirection ().y == 1) {
			this.transform.position = new Vector3 (currentRoad.getTopOfRoad().x,this.transform.position.y , this.transform.position.z);

		} else {
			this.transform.position = new Vector3 (this.transform.position.x, currentRoad.getTopOfRoad().y, this.transform.position.z);

		}
	}

	public void rotateToFacePosition(Vector3 pos)
	{
		Vector3 rot = new Vector3(0, 0, Mathf.Atan2((pos.y - transform.position.y),pos.x - transform.position.x)) * Mathf.Rad2Deg;
		rot = new Vector3(rot.x, rot.y, rot.z-90);//add 90 to make the player face the right way (yaxis = up)
		//rid.transform.eulerAngles = rot;
		this.transform.rotation =Quaternion.Euler(rot);//Quaternion.Slerp(this.transform.rotation,Quaternion.Euler(rot),5*Time.deltaTime);// Quaternion.Euler(rot); //(INSTA ROTATION)

		//rotates player on Z axis to face cursor position
	}



	public bool shouldWeDrive()
	{
		if (raysHitAnything == false) {
			return true;
		} else {
			return false;
		}
	}

	bool raysHitAnything = false;

	void rayObjectDetect()
	{
		raysHitAnything = false;
		foreach (GameObject g in pointsToRaycastFrom) {
			RaycastHit2D ray = Physics2D.Raycast (g.transform.position, g.transform.position - (g.transform.position - transform.up),15.0f);
			Debug.DrawRay (g.transform.position, g.transform.position - (g.transform.position - transform.up), Color.cyan);
			if (ray.collider == null) {

			} else {
				if (ray.collider.gameObject.tag == "Car" || ray.collider.gameObject.GetComponent<PersonMovementController>()==true) {
					raysHitAnything = true;
				}
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

	void setNodesNearMeToUnwalkable()
	{
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
					wt.walkable = false;
					nodesISetToUnwalkable.Add (wt);
					ThreadedPathfindInterface.me.nodes [wt.gridX, wt.gridY].walkable = false;
				}
			}
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
