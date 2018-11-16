using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTVCamera : MonoBehaviour {
	/// <summary>
	/// Class that controls a CCTV camera. 
	/// </summary>

	CanWeDetectTarget detect;
	public GameObject leftLineOfSightMarker, rightLineOfSightMarker;
	Rigidbody2D rid;
	public bool sentAlert = false;
	public float startRotation ;
	public FeildOfView fov;
	Vector3 startingRot;

	public bool positiveRotation = false;
	public float rotationTimer = 5.0f;
	public bool cameraActive = true;
	void Awake()
	{
		startRotation = this.transform.rotation.eulerAngles.z;
		rotAdd = new Vector3 (0, 0, startRotation + 35);
		this.transform.rotation = Quaternion.Euler (0, 0, 0);
		detect = this.GetComponent<CanWeDetectTarget> ();
		detect.coneOfVision = 2.5f;
		rid = this.GetComponent<Rigidbody2D> ();
		fov = this.GetComponentInChildren<FeildOfView> ();
	}
	// Use this for initialization
	void Start () {
		this.transform.rotation = Quaternion.Euler (0, 0, startRotation);
		//StartCoroutine (RotateMe (rotAdd, 4.0f));

	}
	
	// Update is called once per frame
	void Update () {
		if (cameraActive == true) {
			if (fov.gameObject.activeInHierarchy == false) {
				fov.gameObject.SetActive (true);
			}
			if (sentAlert == false) {
				lookForSuspects ();
			} else {
				resetCamera ();
			}
			setMaterial ();
			faceSuspect ();
		} else {
			if (fov.gameObject.activeInHierarchy == true) {
				fov.gameObject.SetActive (false);
			}
		}

	}

	void setMaterial()
	{
		if (fov.visibleTargts.Contains (CommonObjectsStore.player.transform) && LevelController.me.suspects.Contains (CommonObjectsStore.player) == true) {
			fov.shaderMaterial = CommonObjectsStore.me.aggressive;
		} else {
			fov.shaderMaterial = CommonObjectsStore.me.suspicious;
		}
	}

	void faceSuspect()
	{
		foreach (Transform t in fov.visibleTargts) {
			if (t == null) {
				continue;
			}


			if (LevelController.me.suspects.Contains (t.gameObject)) {
				rotateToFaceTarget (t.gameObject);
				return;
			}
		}

		//rotationController ();
		//rotateToFaceTarget(new Vector3(0,0,startRotation));
	}

	public void rotateClockwise()
	{
		this.transform.rotation = Quaternion.Euler (0, 0, this.transform.rotation.eulerAngles.z + 5);
	}

	public void rotateCounterClockwise()
	{
		this.transform.rotation = Quaternion.Euler (0, 0, this.transform.rotation.eulerAngles.z - 5);
	}


	float rotTimer = 4.0f;
	Vector3 rotAdd;
	void rotationController()
	{
		rotTimer -= Time.deltaTime;
		if (rotTimer <= 0) {
			StopAllCoroutines ();
			if (rotAdd.z == startRotation + 35) {
				rotAdd.z = startRotation -70;
			} else if (rotAdd.z == startRotation -70) {
				rotAdd.z = startRotation+70;
			} else {
				rotAdd.z = startRotation  -70;
			}
			StartCoroutine (RotateMe (rotAdd, 4.0f));
			rotTimer = 4.0f;
		}
	}

	IEnumerator RotateMe(Vector3 byAngles,float inTime)
	{
		var fromAngle = transform.rotation;
		var toAngle = Quaternion.Euler (transform.eulerAngles + byAngles);

		for (var t = 0f; t < 1; t += Time.deltaTime / inTime) {
			transform.rotation = Quaternion.Lerp (fromAngle, toAngle,t);
			yield return null;
		}
	}

	public int angleD = 0;
	void lookForSuspects()
	{
		
		foreach (Transform t in fov.visibleTargts) {
			if (t == null) {
				continue;
			}

			if (t.gameObject.tag == "Player") {
				if (PlayerAction.currentAction == null) {

				} else {
					if (PlayerAction.currentAction.illigal == true) {
						CCTVController.me.cameraAlert (this);
						sentAlert = true;
					}
				}
				if (PlayerVisible.me.isPlayersFaceHidden () == false) {
					CrimeRecordScript.me.playerSpottedByCCTV = true;
				}
			}

			if (LevelController.me.suspects.Contains (t.gameObject)) {
				if (sentAlert == false) {
//					////////Debug.LogError ("Camera has seen suspect " + t.gameObject.ToString ());
					CCTVController.me.cameraAlert (this);
					sentAlert = true;
				}
			}
		}
			
	}

	void rotateToFaceTarget(Vector3 pos)
	{
		Quaternion rot = Quaternion.Euler (pos); //new Vector3(0, 0, Mathf.Atan2((pos.y - transform.position.y),pos.x - transform.position.x)) * Mathf.Rad2Deg;
		rot = Quaternion.Euler(rot.x, rot.y, rot.z+90);//add 90 to make the player face the right way (yaxis = up)
		//rid.transform.eulerAngles = rot;
		rid.transform.rotation =Quaternion.Slerp(this.transform.rotation,rot,2*Time.deltaTime);// Quaternion.Euler(rot); //(INSTA ROTATION)
	}

	void rotateToFaceTarget(GameObject g)
	{
		Vector3 pos = g.transform.position;
		Vector3 rot = new Vector3(0, 0, Mathf.Atan2((pos.y - transform.position.y),pos.x - transform.position.x)) * Mathf.Rad2Deg;
		rot = new Vector3(rot.x, rot.y, rot.z-90);//add 90 to make the player face the right way (yaxis = up)
		//rid.transform.eulerAngles = rot;
		rid.transform.rotation =Quaternion.Slerp(this.transform.rotation,Quaternion.Euler(rot),5*Time.deltaTime);// Quaternion.Euler(rot); //(INSTA ROTATION)
	}

	float cameraResetTimer = 5.0f;
	void resetCamera()
	{
		cameraResetTimer -= Time.deltaTime;
		if (cameraResetTimer <= 0) {
			sentAlert = false;
			cameraResetTimer = 5.0f;
		}
	}
}
