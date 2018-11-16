using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class to control cars that move around the level following the roads and making random decisions when they reach junctions
/// They also increase the weight of nearby nodes to help civilians avoid them. 
/// </summary>
public class NewRoadFollower : MonoBehaviour {
	public bool hasDriver=true;
	NewRoad r;
	NewRoadJunction nr;
	Rigidbody2D rid;
	NewRoadJunction nextJunct;
	public float CarSpeed = 0,maxSpeed=5.0f;
	public List<Transform> carRayPoints;

    public bool ignoreObstacles = false;
	// Use this for initialization
	void Start () {
		if (hasDriver == false) {
			foreach (Light l in headlights) {
				l.enabled = false;	
			}

			foreach (Light l in brakelights) {
				l.enabled = false;
			}
		}
		r = FindObjectOfType<NewRoad> ();
		rid = this.GetComponent<Rigidbody2D> ();
		nr = r.getNearestRoad (this.transform.position);
		nextJunct = nr.potentialPoints [Random.Range (0, nr.potentialPoints.Count)];
	}



	// Update is called once per frame
	void Update () {
		if (hasDriver == true && crashed == false) {
			speedControl ();
			if (Vector2.Distance (this.transform.position, nextJunct.startPoint.position) < 1.0f) {
				atPosition ();
			} else {
				moveToDirection (nextJunct.startPoint.position);
			}
			headlightControls ();
		} else if (hasDriver == true && crashed==true) {
			rid.velocity = Vector2.zero;
			crashedReset ();
		}



		setNodesNearMeToUnwalkable ();

	}

	public bool areBothJunctionsPlayerAccessable()
	{
		if (nr.playerCarSpawn == true && nextJunct.playerCarSpawn == true) {
			return true;
		}
		return false;
	}

	void atPosition()
	{
		nr = nextJunct;
		nextJunct = nr.potentialPoints [Random.Range (0, nr.potentialPoints.Count)];

	}

	public void moveToDirection(Vector3 pos)
	{
		if (pos == null || pos == Vector3.zero) {
			return;
		}
		////////Debug.Log (this.gameObject.name + " is moving at " + getMovementSpeed ().ToString());
		Vector3 dir = pos - transform.position;
		//dirToMove = dir;
		rid.velocity = new Vector2(dir.normalized.x * carSpeed(),dir.normalized.y*carSpeed());

		//transform.Translate (dir.normalized *carSpeed() * Time.deltaTime,Space.World);
		rotateToFacePosition (pos);
	}

	public void rotateToFacePosition(Vector3 pos)
	{
		Vector3 rot = new Vector3(0, 0, Mathf.Atan2((pos.y - transform.position.y),pos.x - transform.position.x)) * Mathf.Rad2Deg;
		rot = new Vector3(rot.x, rot.y, rot.z-90);//add 90 to make the player face the right way (yaxis = up)
		//rid.transform.eulerAngles = rot;
		this.transform.rotation =Quaternion.Slerp(this.transform.rotation,Quaternion.Euler(rot),carSpeed()*2*Time.deltaTime);//Quaternion.Slerp(this.transform.rotation,Quaternion.Euler(rot),5*Time.deltaTime);// Quaternion.Euler(rot); //(INSTA ROTATION)

		//rotates player on Z axis to face cursor position
	}
	float carSpeed()
	{
		return CarSpeed;
	}

	bool raysHitAnything = false;

	void rayObjectDetect()
	{
		raysHitAnything = false;

        if(ignoreObstacles==false)
        {
        
            foreach (Transform t in carRayPoints)
            {
                RaycastHit2D ray = Physics2D.Raycast(t.position, t.position - (t.position - transform.up), 5.0f);
                //Debug.DrawRay(t.position, t.position - (t.position - transform.up), Color.cyan);
                if (ray.collider == null)
                {
                }
                else
                {
                    if (ray.collider.gameObject.tag == "Car" || ray.collider.gameObject.GetComponent<PersonMovementController>() == true || ray.collider.gameObject.layer == 29)
                    {
                        raysHitAnything = true;
                    }
                }
            }
        }

	}

	void speedControl()
	{
		rayObjectDetect ();

		if (raysHitAnything == true) {
			if (carSpeed() > 0.0f) {
				CarSpeed-=Time.deltaTime*5;
			}
		} else {
			if (carSpeed () < maxSpeed) {
				CarSpeed += Time.deltaTime;
			}
		}
	}

	public List<WorldTile> nodesISetToUnwalkable;

	void resetNodes()
	{
		foreach (WorldTile wt in nodesISetToUnwalkable) {
			wt.GetComponent<SpriteRenderer> ().color = Color.cyan;
			if (wt.modifier >= 100000) {
				wt.modifier -= 100000;
				//wt.walkable = true;
				try{
					ThreadedPathfindInterface.me.nodes [wt.gridX, wt.gridY].modifier -=10000;
					//ThreadedPathfindInterface.me.nodes [wt.gridX, wt.gridY].walkable=true;
				}
				catch{

				}
			}
		}
		nodesISetToUnwalkable.Clear ();
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
						////Debug.Log (wt.gameObject.name + " set to unwalkable");
						wt.GetComponent<SpriteRenderer> ().color = Color.blue;
						wt.modifier += 100000;
						//wt.walkable = false;

						nodesISetToUnwalkable.Add (wt);
						ThreadedPathfindInterface.me.nodes [wt.gridX, wt.gridY].modifier +=100000;
					//	ThreadedPathfindInterface.me.nodes [wt.gridX, wt.gridY].walkable=false;

					}
				}
			}
		}
		catch{
			resetNodes ();
		}
	}

	bool crashed=false;

	float crashTimer = 5.0f;

	void crashedReset()
	{
		crashTimer -= Time.deltaTime;
		if (crashTimer <= 0) {
			crashed = false;
		}
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		crashTimer = 5.0f;
		crashed = true;
	}


	public Light[] headlights,brakelights;

	bool shouldLightsBeOn()
	{
		if (Vector2.Distance (CommonObjectsStore.player.transform.position, Camera.main.transform.position) > 12) {
			return false;
		}
		return true;
	}

	bool lightsOn=false;
	void headlightControls()
	{
		if (shouldLightsBeOn () == true) {
			if (TimeScript.me.hour > 20 || TimeScript.me.hour < 6) {
				if (lightsOn == false) {
					foreach (Light l in headlights) {
						l.enabled = true;
					}
					lightsOn = true;
				}

			} else {
				if (lightsOn == true) {
					foreach (Light l in headlights) {
						l.enabled = false;
					}
					lightsOn = false;
				}
			}
			breakLightsControl ();
		} else {
			if (lightsOn == true) {
				foreach (Light l in headlights) {
					l.enabled = false;
				}

				foreach (Light l in brakelights) {
					l.enabled = false;
				}
				lightsOn = false;
			}
		}
	}

	void breakLightsControl()
	{
		if (crashed == true || raysHitAnything == true) {
			foreach (Light l in brakelights) {
				l.enabled = true;
			}
		} else {
			foreach (Light l in brakelights) {
				l.enabled = false;
			}
		}
	}
}
