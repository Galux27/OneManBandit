using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonMovementController : MonoBehaviour {
	HeadController hc;
	PersonWeaponController pwc;
	//Legs myLegs;
	Rigidbody2D rid;
	public float movementSpeed = 5.0f,movementMod=1.0f;
	public float rotationSpeed = 2.5f,rotationMod = 1.0f;
	public bool movedThisFrame = false, calledThisFrame = false;
	public bool frozen = false,slowedMovement=false;

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

		/*GameObject g = 	WorldBuilder.me.findNearestWorldTile (this.transform.position).gameObject;

		if (nearestTile == null) {
			nearestTile = g;
			g.GetComponent<SpriteRenderer> ().color = Color.cyan;
		} else if (nearestTile != g) {
			g.GetComponent<SpriteRenderer> ().color = Color.cyan;
			g.GetComponent<SpriteRenderer> ().sortingOrder = 20;
			nearestTile.GetComponent<SpriteRenderer> ().color = Color.white;
			nearestTile.GetComponent<SpriteRenderer> ().sortingOrder = 1;
			nearestTile = g;
		}*/
		//throttleRigidbodyVelocity ();
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
		//if (this.gameObject.tag != "Player") {
		//	timeSinceLastMoveCall += Time.deltaTime;
		//}
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
	//void FixedUpdate()
	//{
	//	if (movedThisFrame == true) {
	//		transform.Translate (dirToMove.normalized * getMovementSpeed() * Time.deltaTime,Space.World);
	//		movedThisFrame = false;
	//		dirToMove = Vector3.zero;
	//	}
	//}

	public void checkForMovementMods()//check for things that would affect movement speed and calculates the new value
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
		}
		if (slowedMovement == true) {
			startVal -= 3.5f;
		}

		if (startVal <= 0 ) {
			startVal = 0.1f;
		}



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
			if (pl == null) {

			} else {

				if (pos == null || pos == Vector3.zero || frozen==true || pl.waitingForPath==true) {
					
					return;
				}


				//if (pl.currentPath==null || pl.askedForNewPath==true) {
				//	return;
				//}

				//if (pl.currentPath.Count > 0) {
				//	if(pl.counter >= pl.currentPath.Count - 1 && Vector2.Distance(this.transform.position,pl.currentPath[pl.currentPath.Count-1])<1.0f ){
				//		return;
				//	}
				//}
			}

		}

		//if (pl == null) {
			hc.rotateToFacePosition (pos);
		///} else {
			//hc.rotateToFacePosition (pos+pl.dirMod);



		//}
		if (pl == null) {
			Vector3 rot = new Vector3(0, 0, Mathf.Atan2((pos.y - (transform.position.y)),pos.x - (transform.position.x))) * Mathf.Rad2Deg;
			rot = new Vector3(rot.x, rot.y, rot.z-90);//add 90 to make the player face the right way (yaxis = up)
			//rid.transform.eulerAngles = rot;
			rid.transform.rotation =Quaternion.Euler(rot);//Quaternion.Slerp(this.transform.rotation,Quaternion.Euler(rot),getRotationSpeed()*Time.deltaTime);// Quaternion.Euler(rot); //(INSTA ROTATION)
		} else {
			if (pl.obstacle == true) {
				return;

			}
			else{
				Vector3 rot = new Vector3(0, 0, Mathf.Atan2((pos.y - (transform.position.y)),pos.x - (transform.position.x))) * Mathf.Rad2Deg;
				rot = new Vector3(rot.x, rot.y, rot.z-90);//add 90 to make the player face the right way (yaxis = up)
				//rid.transform.eulerAngles = rot;
				rid.transform.rotation =Quaternion.Euler(rot);//Quaternion.Slerp(this.transform.rotation,Quaternion.Euler(rot),getRotationSpeed()*Time.deltaTime);// Quaternion.Euler(rot); //(INSTA ROTATION)
			}
		}
			

		//rotates player on Z axis to face cursor position
	}
	public float timeSinceLastMoveCall = 0.0f;
	Vector3 posAIisGoingTo;
	public void moveToDirection(Vector3 pos)
	{
		//if (pl == null) {

		//} else {
			//if (pl.currentPath==null || pl.askedForNewPath==true) {
			//	xMove = Vector3.zero;
			//	yMove = Vector3.zero;

			//	return;
			//}

			//if (pl.currentPath.Count > 0) {
				//if(pl.counter >= pl.currentPath.Count - 1 && Vector2.Distance(this.transform.position,pl.currentPath[pl.currentPath.Count-1])<1.0f ){
				//	xMove = Vector3.zero;
				//	yMove = Vector3.zero;
				//	movedThisFrame = false;
				//	calledThisFrame = false;
				//	return;
				//}
			//}
		//}



		if (pos == null || pos == Vector3.zero || frozen==true || pl.waitingForPath==true) {
			movedThisFrame = false;
			calledThisFrame = false;
			return;
		}
		posAIisGoingTo = pos;
		//////Debug.Log (this.gameObject.name + " is moving at " + getMovementSpeed ().ToString());
		Vector3 dir = pos - transform.position;

		//if (pl == null) {
			dirToMove = dir;
		//} else {
			//dirToMove = dir + pl.dirMod;
		//}
		//transform.Translate (dir.normalized * getMovementSpeed() * Time.deltaTime,Space.World);

		timeSinceLastMoveCall = 0.0f;
		xMove = (dir.normalized);
		yMove =  xMove;
		movedThisFrame = true;
		calledThisFrame = true;
	}

	public void moveUp()
	{
		//transform.Translate (Vector3.up * getMovementSpeed() * Time.deltaTime ,Space.World);
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

		//transform.Translate (Vector3.left * getMovementSpeed() * Time.deltaTime,Space.World);
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

		//transform.Translate (Vector3.right * getMovementSpeed() * Time.deltaTime ,Space.World);
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
	//	rid.AddForce ();
		//rid.AddForce ();

		//rid.AddForce (new Vector2 (xMove.x, yMove.y));

		//rid.velocity = new Vector2 (xMove.x, yMove.y);
		if (this.gameObject.tag == "Player") {
			rid.velocity = new Vector2 (xMove.x, yMove.y);
		} else {
			if (closeEnough()) {

			}else{
				//rid.MovePosition (this.transform.position + (new Vector3(xMove.x,yMove.y,0) * getMovementSpeed() * Time.deltaTime));
				if (pl == null) {
					rid.MovePosition ((this.transform.position + (new Vector3 (xMove.x, yMove.y, 0)) * getMovementSpeed () * Time.deltaTime));

				} else {

					if (pl.askedForNewPath == true) {
						rid.velocity = new Vector2 (0, 0);
					}

					//rid.MovePosition (this.transform.position + ((new Vector3 (xMove.x, yMove.y, 0) + (pl.dirMod)) * getMovementSpeed () * Time.deltaTime));
					//rid.MovePosition ((this.transform.position + (new Vector3 (xMove.x, yMove.y, 0)) * getMovementSpeed () * Time.deltaTime));

					if (pl.obstacle == true) {
						//rid.MovePosition ((this.transform.position + pl.dirMod) * getMovementSpeed() * Time.deltaTime);
						//rid.AddForce(pl.dirMod * getMovementSpeed());
						//rid.velocity = new Vector2(pl.dirMod.x * getMovementSpeed()/2,pl.dirMod.y *getMovementSpeed()/2);
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
