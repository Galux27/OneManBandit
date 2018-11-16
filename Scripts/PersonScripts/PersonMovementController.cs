using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that Controls the movement&rotation speed of a human
/// </summary>
public class PersonMovementController : MonoBehaviour {
	HeadController hc;
	PersonWeaponController pwc;
	//Legs myLegs;
	Rigidbody2D rid;
	public float movementSpeed = 5.0f,movementMod=1.0f;
	public float rotationSpeed = 2.5f,rotationMod = 1.0f;
	public bool movedThisFrame = false, calledThisFrame = false;
	public bool frozen = false,slowedMovement=false;
	GameObject obstacle;
	PathFollower pl;
	void Awake()
	{
		rid = this.GetComponent<Rigidbody2D> ();
		hc = this.GetComponentInChildren<HeadController> ();//this.gameObject.GetComponentInChildren<AnimationController> ().head.GetComponent<HeadController> ();
		pwc = this.gameObject.GetComponent<PersonWeaponController> ();
		if (this.gameObject.GetComponent<PathFollower> () == null) {

		} else {
			pl = this.gameObject.GetComponent<PathFollower> ();
		}
		//myLegs = this.gameObject.GetComponentInChildren<Legs> ();
	}
	public GameObject nearestTile;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	
		shouldWeKillVelocity ();

		if (stopped == true) {
			resetStopped ();
		}

		checkForMovementMods ();
		checkForRotationMods ();

		if (calledThisFrame == true) {
			calledThisFrame = false;
		} else {
			movedThisFrame = false;
		}
        throttleRigidbodyVelocity();
	}

	void throttleRigidbodyVelocity()
	{
		Vector2 v = this.GetComponent<Rigidbody2D> ().velocity;
		float x = 0.0f, y = 0.0f;
		if (v.x > getMovementSpeed ()) {
			x = getMovementSpeed ();
		} else if (v.x < getMovementSpeed () * -1) {
			x = getMovementSpeed () * -1;
		} else {
			x = v.x;
		}

		if (v.y > getMovementSpeed ()) {
			y = getMovementSpeed ();
		} else if (v.y < getMovementSpeed () * -1) {
			y = getMovementSpeed () * -1;
		} else {
			y = v.y;
		}

		rid.velocity = new Vector2 (x, y);
	}

	void FixedUpdate(){
		if (this.gameObject.tag != "Player") {
			physicsMoveTest ();
		}
	}

	float tempStopTimer = 0.5f;
	public bool stopped = false;

	public void setStopped()
	{
		stopped = true;
		frozen = true;
	}

	void resetStopped()
	{
		tempStopTimer -= Time.deltaTime;
		if (tempStopTimer <= 0) {
			stopped = false;
			frozen = false;
			tempStopTimer = 0.5f;
		}
	}

	Vector3 dirToMove = Vector3.zero;


	public void checkForMovementMods()
	{
		float startVal = 1.0f;

		if (pwc.aimDownSight == true) {
			startVal -= 0.5f;
		}

		if (this.gameObject.tag == "Player") {
			if (PlayerAction.currentAction == null) {

			} else {
				startVal += PlayerAction.currentAction.getMoveModForAction ();
			}
			foreach (EffectBase eb in EffectsManager.me.effectsOnPlayer) {
				startVal += eb.getMoveMod ();
			}
		}
		if (slowedMovement == true) {
			startVal -= 3.5f;
		}

	

		if (startVal <= 0 ) {
			startVal = 0.1f;
		}


//		//Debug.Log ("Player movement mod was " + startVal);
		movementMod = startVal;
	}



	public void checkForRotationMods()
	{
		float startVal = 1.0f;

		if (pwc.aimDownSight == true) {
			startVal += 0.25f;
		}

		if (this.gameObject.tag == "Player") {
			if (PlayerAction.currentAction == null) {

			} else {
				startVal += PlayerAction.currentAction.getRotationModForAction();
			}
		}

		if (startVal <= 0) {
			startVal = 0.1f;
		}

		rotationMod = startVal;
	}

	float getMovementSpeed()
	{
		return movementSpeed * movementMod;
	}

	float getRotationSpeed()
	{
		return rotationSpeed * rotationMod;
	}


	public void rotateToFacePosition(Vector3 pos)
	{
		if (this.gameObject.tag != "Player") {
			//if (this.GetComponent<NPCBehaviourDecider> ().myType == AIType.shopkeeper) {
			//	//Debug.Log ("SHOPKEEPER IS FACING " + pos.ToString ());
			//}

			if (pl == null) {

			} else {

				if (pos == null || pos == Vector3.zero || frozen==true || pl.waitingForPath==true) {
					//if (this.GetComponent<NPCBehaviourDecider> ().myType == AIType.shopkeeper) {
						////Debug.Log ("SHOPKEEPER RETURNED NULL");
					//}
					return;
				}
			}

		
		}
			hc.rotateToFacePosition (pos);

		if (pl == null) {
			Vector3 rot = new Vector3(0, 0, Mathf.Atan2((pos.y - (transform.position.y)),pos.x - (transform.position.x))) * Mathf.Rad2Deg;
			rot = new Vector3(rot.x, rot.y, rot.z-90);//add 90 to make the player face the right way (yaxis = up)
			//rid.transform.eulerAngles = rot;
			rid.transform.rotation =Quaternion.Euler(rot);//Quaternion.Slerp(this.transform.rotation,Quaternion.Euler(rot),getRotationSpeed()*Time.deltaTime);
		} else {
			if (pl.obstacle == true) {
				//if (this.GetComponent<NPCBehaviourDecider> ().myType == AIType.shopkeeper) {
					////Debug.Log ("SHOPKEEPER RETURNED NULL 2");
				//}
				return;

			}
			else{
				Vector3 rot = new Vector3(0, 0, Mathf.Atan2((pos.y - (transform.position.y)),pos.x - (transform.position.x))) * Mathf.Rad2Deg;
				rot = new Vector3(rot.x, rot.y, rot.z-90);//add 90 to make the player face the right way (yaxis = up)
				rid.transform.rotation =Quaternion.Euler(rot);//Quaternion.Slerp(this.transform.rotation,Quaternion.Euler(rot),getRotationSpeed()*Time.deltaTime);
			}
		}
			

		//rotates player on Z axis to face cursor position
	}
	public float timeSinceLastMoveCall = 0.0f;
	Vector3 posAIisGoingTo;
	public void moveToDirection(Vector3 pos)
	{
		
		if (pos == null || pos == Vector3.zero || frozen==true || pl.waitingForPath==true) {
			movedThisFrame = false;
			calledThisFrame = false;
			return;
		}
		posAIisGoingTo = pos;
		Vector3 dir = pos - transform.position;


		dirToMove = dir;


		timeSinceLastMoveCall = 0.0f;
		xMove = (dir.normalized);
		yMove =  xMove;
		movedThisFrame = true;
		calledThisFrame = true;
	}

	public void moveUp()
	{
		if (Input.GetAxis ("Vertical") > 0.0f) {

			rid.AddForce (Vector3.up * getMovementSpeed ());

			doWeResetVelocity = true;

			movedThisFrame = true;
			calledThisFrame = true;
		}
	}
	bool doWeResetVelocity = false;


	void shouldWeKillVelocity()
	{
		if (this.gameObject.tag == "Player") {
			return;
		}

		timeSinceLastMoveCall += Time.deltaTime;
		if ( killVel()) {
			rid.velocity = Vector2.zero;
			xMove = Vector3.zero;
			yMove = Vector3.zero;
			doWeResetVelocity = false;
			return;
		}

	}

	public bool killVel()
	{
		if ( timeSinceLastMoveCall > 0.5f || frozen==true ) {
			
			return true;
		}
		if (this.gameObject.tag != "Player") {
			if (pl.target == null || Vector2.Distance (this.transform.position, pl.target.transform.position) < 1) {

				return true;
			}
		}
		return false;
	}

	public void moveDown()
	{

		//transform.Translate (Vector3.down * getMovementSpeed() * Time.deltaTime,Space.World);
		if (Input.GetAxis ("Vertical") < 0.0f) {
			timeSinceLastMoveCall = 0.0f;
			rid.AddForce (Vector3.down * getMovementSpeed ());
			doWeResetVelocity = true;
			movedThisFrame = true;
			calledThisFrame = true;
		}

	}

	public void moveLeft()
	{

		if (Input.GetAxis ("Horizontal") < 0.0f) {
			timeSinceLastMoveCall = 0.0f;

			rid.AddForce (Vector3.left * getMovementSpeed ());
			doWeResetVelocity = true;

			movedThisFrame = true;
			calledThisFrame = true;
		}
	}

	public void moveRight()
	{

		if (Input.GetAxis ("Horizontal") > 0.0f) {
			timeSinceLastMoveCall = 0.0f;

			rid.AddForce (Vector3.right * getMovementSpeed ());
			doWeResetVelocity = true;

			movedThisFrame = true;
			calledThisFrame = true;
		}
	}


	public Vector3 xMove = Vector3.zero;
	public Vector3 yMove = Vector3.zero;

	public void moveDirSet()
	{
		xMove = Vector3.right * Input.GetAxis ("Horizontal") * getMovementSpeed ();
		yMove = Vector3.up* Input.GetAxis ("Vertical")  * getMovementSpeed ();

		if (Input.GetAxis ("Vertical") != 0.0f || Input.GetAxis ("Horizontal") != 0.0f) {

			doWeResetVelocity = true;

			movedThisFrame = true;
			calledThisFrame = true;
		}
	}
	public void physicsMoveTest()
	{

		if (this.gameObject.tag == "Player") {
			rid.velocity = new Vector2 (xMove.x, yMove.y);
		} else {
			if (closeEnough()) {

			}else{
				if (pl == null) {
					rid.MovePosition ((this.transform.position + (new Vector3 (xMove.x, yMove.y, 0)) * getMovementSpeed () * Time.deltaTime));

				} else {

					if (pl.askedForNewPath == true) {
						rid.velocity = new Vector2 (0, 0);
					}


					if (pl.obstacle == true) {
						
						rid.MovePosition ((this.transform.position + (pl.dirMod) * (getMovementSpeed ()/4) * Time.deltaTime));

					} else {
						rid.MovePosition ((this.transform.position + (new Vector3 (xMove.x, yMove.y, 0)) * getMovementSpeed () * Time.deltaTime));

					}
				}
			}
		}

	}

	bool closeEnough()
	{
		if (pl==null|| pl.target==null || Vector2.Distance(pl.target.transform.position,this.transform.position)<1.0f) {
			return true;
		}
		return false;
	}
}
