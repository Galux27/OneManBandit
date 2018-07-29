using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanWeDetectTarget : MonoBehaviour {

	/// <summary>
	/// Class that is used to work out if an NPC can detect an object, done via distance, raycast and the object begin infront of the NPC
	/// </summary>

	public NPCController myController;
	public GameObject target;
	public bool detect=false;
	public float range = 10.0f;
	public float dotProduct = 0.0f;
	public GameObject head;
	public float coneOfVision = 0.5f;
	public FeildOfView fov;
	void Awake()
	{
		myController = this.gameObject.GetComponent<NPCController> ();
		fov = this.GetComponentInChildren<FeildOfView> ();


	}
	// Use this for initialization
	void Start () {
		try{
			GameObject h =this.GetComponentInChildren<AnimationController> ().head;
			if (h == null) {
				head = this.gameObject;
			} else {
				head = h;
			}
		}catch{
			head = this.gameObject;
		}

		myController = this.gameObject.GetComponent<NPCController> ();

	}
	
	// Update is called once per frame
	void Update () {
		howBigShouldFOVRadiusBe ();
		if (target == null) {
			detect = false;
		} else {
			if (fov.visibleTargts.Contains (target.transform)) {
				detect = true;
			} else {
				detect = false;
			}

			//detect = targetDetect ();
		}
	}

	void howBigShouldFOVRadiusBe()
	{
		if (myController == null) {
			myController = this.gameObject.GetComponent<NPCController> ();
		} else {
			if (myController.npcB.inLitRoom == true) {
				fov.radiusMod = 1.0f;
			} else {
				fov.radiusMod = 0.5f;
			}
		}
	}

	public bool targetDetect()
	{
		if (areWeNearTarget (target) == true) {
			if (isTargetInFrontOfUs (target) == true) {
				if (lineOfSightToTarget (target) == true) {
					return true;
				}
				return false;
			}
			return false;
		}
		return false;
	}

	public bool targetDetect(GameObject g)
	{
		if (g == null) {
			return false;
		}

		if (fov.visibleTargts.Contains (g.transform)) {
			return true;
		} else {
			return false;
		}

		/*if (areWeNearTarget (g) == true) {
			if (isTargetInFrontOfUs (g) == true) {
				if (lineOfSightToTarget (g) == true) {
					return true;
				}
				return false;
			}
			return false;
		}
		return false;*/
	}

	public bool areWeNearTarget(GameObject target)
	{

		if (target == null) {
			return false;
		}

		if (Vector3.Distance (this.transform.position, target.transform.position) <= range) {
			return true;
		} else {
			return false;
		}
	}

	public bool isTargetInFrontOfUs(GameObject target)
	{
		//Vector3 heading = target.transform.position - head.transform.position;
		//dotProduct = Vector3.Dot (heading, head.transform.up);

		if (getDotProduct(target) > coneOfVision) {
			angleTest (target);
			return true;
		} else {
			return false;
		}

	}

	public float getDotProduct(GameObject target)
	{
		Vector3 heading = target.transform.position - head.transform.position;
		dotProduct = Vector3.Dot (heading, head.transform.up);
		return dotProduct;
	}

	public int highAngle=0,lowAngle=999;
	void angleTest(GameObject target)
	{
		Vector3 targetDir = target.transform.position - transform.position;
		float angle = Vector3.Angle (targetDir, head.transform.up);
		if (angle > highAngle) {
			highAngle = (int)angle;
		}

		if (angle < lowAngle) {
			lowAngle = (int)angle;
		}

//		//////Debug.Log ("Angle " + (int)angle);
	}

	public bool hostageTest(GameObject target)
	{
		//if(target.tag=="Player" && PlayerAction.currentAction.getType()==
		//////Debug.LogError ("Target is " + target.gameObject);

		if (target.transform.parent == null && target.tag != "Player") {
			//////Debug.LogError (" target was a mistake ");
			return false;
		} else {
			if (target.transform.parent == null) {
				//////Debug.LogError ("targets parent was null ");

				if (target.tag == "Player" && lineOfSightToTargetWithHostage (target) && areWeNearTarget (target)) {
					//////Debug.LogError ("target was player and in range");

					return true;
				} else {
					//////Debug.LogError ("tag was "+target.tag == "Player" );
					//////Debug.LogError ("line of sight "+lineOfSightToTargetWithHostage (target));
					//////Debug.LogError ("near target "+areWeNearTarget (target));


					return false;
				}
			} else {
				if (target.transform.parent.gameObject.tag == "Player") {
					if (lineOfSightToTarget (target) == true && areWeNearTarget (target) == true) {
						//////Debug.LogError ("target was hostage and in range");

						return true;
					} else {
						return false;
					}
				}
			}
		}
		return false;
	}

	public bool lineOfSightToTargetWithHostage(GameObject target)
	{

		Vector3 origin = Vector3.zero;
		if (myController == null) {
			origin = head.transform.position;
		}else if (myController.pwc.currentWeapon == null) {
			origin = head.transform.position;
		} else if (myController.pwc.currentWeapon.oneHanded == true) {
			if (myController.pwc.aimDownSight == true) {
				origin = myController.pwc.bulletSpawn_oneHandADS.position;
			} else {
				origin = myController.pwc.bulletSpawn_oneHandHip.position;
			}
		} else if (myController.pwc.currentWeapon.oneHanded == false) {
			if (myController.pwc.aimDownSight == true) {
				origin = myController.pwc.bulletSpawn_twoHandADS.position;
			} else {
				origin = myController.pwc.bulletSpawn_twoHandHip.position;
			}
		}

		Vector3 heading = target.transform.position - origin;
		RaycastHit2D[] rays = Physics2D.RaycastAll (origin, heading,Vector3.Distance(this.transform.position,target.transform.position));
		//Debug.DrawRay (origin, heading,Color.cyan);

		foreach (RaycastHit2D ray in rays) {
			if (ray.collider == null) {

			} else {
				//////Debug.LogError ("Ray hit object with tag " + ray.collider.gameObject.name);
				if (ray.collider.gameObject == target ) {
					continue;
				} else {
					if (ray.collider.gameObject.transform.parent == null) {
						return false;
					} else {
						if (ray.collider.gameObject.transform.parent.gameObject == target) {
							continue;
						} else {
							return false;
						}
					}
				}
			}
		}
		return true;
	}

	public bool lineOfSightToTarget(GameObject target)
	{
		

		Vector3 origin = Vector3.zero;
		if (myController == null) {
			origin = head.transform.position;
		}else if (myController.pwc.currentWeapon == null) {
			origin = head.transform.position;
		} else if (myController.pwc.currentWeapon.oneHanded == true) {
			if (myController.pwc.aimDownSight == true) {
				origin = myController.pwc.bulletSpawn_oneHandADS.position;
			} else {
				origin = myController.pwc.bulletSpawn_oneHandHip.position;
			}
		} else if (myController.pwc.currentWeapon.oneHanded == false) {
			if (myController.pwc.aimDownSight == true) {
				origin = myController.pwc.bulletSpawn_twoHandADS.position;
			} else {
				origin = myController.pwc.bulletSpawn_twoHandHip.position;
			}
		}

		Vector3 heading = target.transform.position - origin;
		RaycastHit2D ray = Physics2D.Raycast (origin, heading);
		Debug.DrawRay (origin, heading,Color.cyan);

		if (ray.collider == null) {

		} else {
//			//////Debug.Log ("Ray hit object with tag " + ray.collider.gameObject.tag);
			if (ray.collider.gameObject == target) {
				return true;
			}
		}
		return false;
	}

	public bool lineOfSightToTargetWithNoCollider(GameObject target){
		Vector3 origin = Vector3.zero;
		if (myController.pwc.currentWeapon == null) {
			origin = head.transform.position;
		} else if (myController.pwc.currentWeapon.oneHanded == true) {
			if (myController.pwc.aimDownSight == true) {
				origin = myController.pwc.bulletSpawn_oneHandADS.position;
			} else {
				origin = myController.pwc.bulletSpawn_oneHandHip.position;
			}
		} else if (myController.pwc.currentWeapon.oneHanded == false) {
			if (myController.pwc.aimDownSight == true) {
				origin = myController.pwc.bulletSpawn_twoHandADS.position;
			} else {
				origin = myController.pwc.bulletSpawn_twoHandHip.position;
			}
		}



		Vector3 heading = target.transform.position - origin;
		RaycastHit2D ray = Physics2D.Raycast (origin, heading,Vector3.Distance(this.transform.position,target.transform.position));
		Debug.DrawRay (origin, heading,Color.cyan);

		if (ray.collider == null) {
//			//////Debug.Log ("No ray hit");
			return true;
		} else {
			////Debug.Log (ray.collider.gameObject.name);
			return false;
		}
	}

	public bool lineOfSightToTargetWithNoColliderForPathfin(Vector3 target){
		Vector3 origin =this.transform.position;
		/*if (myController.pwc.currentWeapon == null) {
			origin = head.transform.position;
		} else if (myController.pwc.currentWeapon.oneHanded == true) {
			if (myController.pwc.aimDownSight == true) {
				origin = myController.pwc.bulletSpawn_oneHandADS.position;
			} else {
				origin = myController.pwc.bulletSpawn_oneHandHip.position;
			}
		} else if (myController.pwc.currentWeapon.oneHanded == false) {
			if (myController.pwc.aimDownSight == true) {
				origin = myController.pwc.bulletSpawn_twoHandADS.position;
			} else {
				origin = myController.pwc.bulletSpawn_twoHandHip.position;
			}
		}*/



		Vector3 heading = target - origin;
		RaycastHit2D ray = Physics2D.Raycast (origin, heading,Vector3.Distance(this.transform.position,target));

		if (ray.collider == null) {
			//			//////Debug.Log ("No ray hit");
			Debug.DrawRay (origin, heading,Color.cyan);

			return true;
		} else {

			if (ray.collider.gameObject.tag == "WallCollider") {
				//////Debug.Log ("We hit a wall " + ray.collider.gameObject.name);
			}

			////////Debug.Log (ray.collider.gameObject.name);
			return false;
		}
	}

}
