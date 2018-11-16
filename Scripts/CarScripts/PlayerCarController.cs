using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCarController : MonoBehaviour {

	public string carName = "";

	public bool playerCar=false,stolen = false, stolenFromDriver = false;
	public int dayStolen = 0,monthStolen=0,yearStolen=0;
	public int ID = -1;

	public static bool inCar = false;
	NewRoadFollower myRoadFollower;
	float acceleration=0,accelerationTimer = 0.0f;
	public float maxAccelleration = 18000;
	public float steering=4;
	private Rigidbody2D rb;
	float horizontal = 0.0f;
	float vertical = 0.0f;
	public bool playerInCar=false;
	bool inputOnFrame=false;

	public float carHealth=30000,maxHealth=0;
	public GameObject enginePoint;


	public GameObject driversDoor;
	public List<GameObject> altCarDoors;

	public GameObject boot;
	public Container carContainer;

	public List<GameObject> wheelPoints;


	float originalMass=0.0f;
	float originalDrag=0.0f;
	float originalAngularDrag = 0.0f;

	SpriteRenderer enterIcon;
	public bool canEnterCar=true;
	// Use this for initialization
	void Start () {
		myRoadFollower = this.GetComponent<NewRoadFollower> ();
		rb = this.GetComponent<Rigidbody2D> ();
		carContainer = boot.GetComponent<Container> ();
		maxHealth = carHealth;

		originalMass = rb.mass;
		originalDrag = rb.drag;
		originalAngularDrag = rb.angularVelocity;
		enterIcon = driversDoor.AddComponent<SpriteRenderer> ();
		enterIcon.sortingOrder = 99;
		if (playerCar == true) {
			enterIcon.sprite = CommonObjectsStore.me.keyIcon;
		} else {
			enterIcon.sprite = CommonObjectsStore.me.hotwireIcon;

		}
		enterIcon.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (carExploding == false) {
			enterCarCheck ();
			if (playerInCar == true) {
				CommonObjectsStore.player.transform.position = this.transform.position;
				InputDetect ();
			}
            else if(myRoadFollower.hasDriver==false)
            {
                Vector3 v = rb.transform.InverseTransformDirection(rb.velocity);
                if(v.y>0)
                {
                    braking();
                }
            }
            headlightControls ();
		} else if (playerInCar == true && carExploding == true) {
			playerExitCar ();
		}
		effectsController ();
	}
	float losevelocityTimer=0.5f;
	void InputDetect()
	{
		if (carHealth <= 0 || MapControlScript.me.displayMap==true) {
			return;
		}

		inputOnFrame = false;
		isCarBraking = false;
		if (Input.GetKey (KeyCode.W)) {
			vertical = 1.0f;
			Accelleration ();
			inputOnFrame = true;
		} else if (Input.GetKey (KeyCode.S)) {

			Vector3 v = rb.transform.InverseTransformDirection (rb.velocity);

			if (v.y>0.1f) {
				braking ();
				inputOnFrame = true;
			} else {
				vertical = -1.0f;
				reversing ();
				inputOnFrame = true;
			}
		} else {
			vertical = 0;
		}

	

		if (Input.GetKey (KeyCode.A)) {
			horizontal = 1.0f;
		} else if (Input.GetKey (KeyCode.D)) {
			horizontal = -1.0f;
		} else {
			horizontal = 0;
		}

		if (Input.GetKey (KeyCode.Space)) {
			handbrake ();
		} else {
			handbraking = false;
		}

		if (Input.GetKeyDown (KeyCode.E)) {
			playerLightSwitch = !playerLightSwitch;
		}

		if (inputOnFrame == false) {
			losevelocityTimer -= Time.deltaTime;
		}

	}
	bool isCarBraking=false;
	void braking()
	{
		vertical = 0;
		acceleration -= 100;
		isCarBraking = true;
		if (acceleration <= 0) {
			acceleration = 0;
		}
	}

	void reversing()
	{
		if (acceleration >= maxAccelleration/3) {
			return;
		}
		accelerationTimer -= Time.deltaTime;
		if (accelerationTimer <= 0) {
			acceleration += 1000;
			accelerationTimer = 0.5f;
		}
	}

	bool handbraking=false;
	void handbrake()
	{
		handbraking = true;
		//rb.mass = originalMass * 2;
		//rb.angularDrag = originalAngularDrag / 2;
		//rb.drag = originalDrag / 2;
	}

	void Accelleration(){
		if (acceleration >= maxAccelleration) {
			return;
		}
		accelerationTimer -= Time.deltaTime;
		if (accelerationTimer <= 0) {
			acceleration += 1000;
			accelerationTimer = 0.5f;
		}

	}



	void FixedUpdate()
	{
		


		float h = horizontal;//-Input.GetAxis ("Horizontal");
		float v = vertical;//Input.GetAxis ("Vertical");
		////Debug.Log("Car velocity = " + .ToString());
		if (inputOnFrame == false && losevelocityTimer<=0) {
			if (rb.velocity.magnitude > 1.0f) {
				acceleration -= 1000;
				rb.velocity = rb.velocity / 1.2f;
				losevelocityTimer = 1.0f;
			} else {
				acceleration =0;
				rb.velocity = Vector2.zero;
				losevelocityTimer = 1.0f;
			}
		}
		if (carHealth <= 0) {

			return;
		}

		if (handbraking == true) {
			rb.AddTorque (100.0f);
		}

		if (isCarBraking == false) {

			Vector2 speed = transform.up * (v * acceleration);
			rb.AddForce (speed);
		} else {
			rb.AddForce (rb.velocity.normalized*-1);
		}

		float direction = Vector2.Dot(rb.velocity,rb.GetRelativeVector(Vector2.up));
		if (direction >= 0) {
			rb.rotation+=h*steering*(rb.velocity.magnitude/5);
		} else {
			rb.rotation-=h*steering*(rb.velocity.magnitude/5);
		}

		Vector2 forward = new Vector2 (0.0f, 0.5f);
		float steeringRightAngle;
		if (rb.angularVelocity > 0) {
			steeringRightAngle = -90;
		} else {
			steeringRightAngle = 90;
		}

		Vector2 rightAngleFromForward = Quaternion.AngleAxis (steeringRightAngle, Vector3.forward) * forward;
		//Debug.DrawLine ((Vector3)rb.position, (Vector3)rb.GetRelativePoint (rightAngleFromForward), Color.green);

		float driftForce = Vector2.Dot (rb.velocity, rb.GetRelativeVector (rightAngleFromForward.normalized));
		Vector2 relativeForce = (rightAngleFromForward.normalized * -1.0f) * (driftForce * 10);
		//Debug.DrawLine ((Vector3)rb.position, (Vector3)rb.GetRelativePoint (relativeForce), Color.red);
		rb.AddForce (rb.GetRelativeVector (relativeForce));
	}

	void enterCarCheck()
	{
		if (canEnterCar == false) {
			return;
		}

		if (PlayerCarController.inCar == false) {
			float d = Vector2.Distance (CommonObjectsStore.player.transform.position, driversDoor.transform.position);
			if (d < 3) {
				enterIcon.enabled = true;
				if (Input.GetKeyDown (KeyCode.X)) {
					if (playerInCar == true) {
						playerExitCar ();
					} else {
						playerEnterCar ();
					}
				}
			}
		} else if (PlayerCarController.inCar == true && playerInCar == true) {
			enterIcon.enabled = false;
			if (Input.GetKeyDown (KeyCode.X)) {
				if (playerInCar == true) {
					playerExitCar ();
				} 
			}
		} else if (PlayerCarController.inCar == true) {
			enterIcon.enabled = false;

		}
	}

	public void playerEnterCar()
	{
		//Debug.Log ("PLAYER ENTERED CAR");
		SpriteRenderer[] srs = CommonObjectsStore.player.GetComponentsInChildren<SpriteRenderer> ();
		foreach (SpriteRenderer sr in srs) {
			sr.enabled = false;
		}

		Collider2D[] c2s = CommonObjectsStore.player.GetComponents<Collider2D> ();
		foreach (Collider2D c2 in c2s) {
			c2.enabled = false;
		}
		PersonColliderDecider pcd = CommonObjectsStore.player.GetComponent<PersonColliderDecider> ();
		pcd.twoHandADS.enabled = false;
		pcd.normal.enabled = false;
		PlayerCarController.inCar = true;
		playerInCar = true;

		if (playerCar == false) {
			stolen = true;
			if (myRoadFollower == null) {

			} else {
				if (myRoadFollower.hasDriver == true) {
					stolenFromDriver = true;
				}
			}
			dayStolen = TimeScript.me.day;
			monthStolen = TimeScript.me.month;
			yearStolen = TimeScript.me.year;
		}


		if (myRoadFollower == null) {

		} else {
			if (myRoadFollower.hasDriver == true) {
				GameObject g = (GameObject)Instantiate (CommonObjectsStore.me.civilian, driversDoor.transform.position, Quaternion.Euler (0, 0, 0));
				NPCController npc = g.GetComponent<NPCController> ();
				NPCBehaviourDecider npcb = g.GetComponent<NPCBehaviourDecider> ();
				NPCMemory npcm = g.GetComponent<NPCMemory> ();
				npcm.beenAttacked = true;
				npcm.seenSuspect = true;
				npcm.objectThatMadeMeSuspisious = CommonObjectsStore.player;
				npcm.raiseAlarm = true;
				npcb.alarmed = true;
				CrimeRecordScript.me.addCrime (new Crime (crimeTypes.carTheft, true));

			}

			myRoadFollower.hasDriver = false;
		}

		if (ID == -1) {
			ID = IDManager.me.getID ();
		}
		this.gameObject.layer = 29;
	}

	public void playerExitCar()
	{
		//Debug.Log ("PLAYER LEFT CAR");
		SpriteRenderer[] srs = CommonObjectsStore.player.GetComponentsInChildren<SpriteRenderer> ();
		foreach (SpriteRenderer sr in srs) {
			sr.enabled = true;
		}

		Collider2D[] c2s = CommonObjectsStore.player.GetComponents<Collider2D> ();
		foreach (Collider2D c2 in c2s) {
			c2.enabled = true;
		}
		PlayerCarController.inCar = false;
		this.gameObject.layer = 28;
		playerInCar = false;
		CommonObjectsStore.player.transform.position = driversDoor.transform.position;
	}

	void OnCollisionEnter2D(Collision2D col){
		if (playerInCar == true && rb.velocity.magnitude > 1) {
			//Debug.Log ("CAR COLLIDED WITH " + col.gameObject.name);
			if (col.gameObject.GetComponent<NPCController> () == true) {
				NPCController npc = col.gameObject.GetComponent<NPCController> ();
				npc.knockOutNPC ();
				//Debug.Log ("Dealt " + Mathf.RoundToInt (100 * rb.velocity.magnitude).ToString () + " Damage to NPC");
				npc.myHealth.dealDamage (Mathf.RoundToInt (100 * rb.velocity.magnitude), true);
			} else if (col.gameObject.GetComponent<DoorScript> () == true) {
				DoorScript ds = col.gameObject.GetComponent<DoorScript> ();
				ds.largeImpactOnDoor (100);
			} else if (col.gameObject.transform.parent!=null&& col.gameObject.transform.parent.GetComponent<WindowNew> () == true) {
				WindowNew wn = col.gameObject.transform.parent.GetComponent<WindowNew> ();
				wn.destroyWindow ();
			}
			dealDamage (100);
		}
	}

	bool carSmoking=false,carMinorFire=false,carMajorFire=false,carExploding=false;

	public void dealDamage(int damage)
	{
		if (carHealth < 0) {
			return;
		}

		carHealth -= damage;
		if (carHealth < maxHealth * 0.5f) {
			carSmoking = true;
		}

		if (carHealth < maxHealth * 0.3f) {
			carMinorFire = true;
		}

		if (carHealth < maxHealth * 0.1f) {
			carMajorFire = true;
		}

		if (carHealth <= 0) {
			carExploding = true;
		}
//		//Debug.Log ("Delt car damage, car health = " + carHealth);
	}

	GameObject carSmoke;
	public List<GameObject> carFireEffect;
	float explodeTimer=1.0f;
	SmokeManager sm;
	void effectsController()
	{
		if (carSmoking == true && carSmoke == null) {
			if (sm == null) {
				sm= this.gameObject.AddComponent<SmokeManager> ();
			}
			sm.smokeOriginPoint = enginePoint.transform.position;

			carSmoke = (GameObject)Instantiate (CommonObjectsStore.me.smokeEffect, enginePoint.transform);
			carSmoke.GetComponent<SmokeEffect> ().myManager = sm;
		}
		if (carFireEffect == null) {
			carFireEffect = new List<GameObject> ();
		}
		if (carMinorFire == true && carFireEffect.Count < 2) {
			for (int x = 0; x < 2; x++) {
				GameObject g = (GameObject)Instantiate (CommonObjectsStore.me.carFireEffect, enginePoint.transform);
				g.transform.localPosition = new Vector3 (Random.Range (-0.25f, 0.25f),Random.Range (-0.25f, 0.25f), 0);
				g.transform.localScale = new Vector3 (Random.Range (0.1f,0.4f), Random.Range (0.1f, 0.4f), 0);
				carFireEffect.Add (g);
			}
		}

		if (carMajorFire == true && carFireEffect.Count < 7) {
			while (carFireEffect.Count < 7) {
				GameObject g = (GameObject)Instantiate (CommonObjectsStore.me.carFireEffect, enginePoint.transform);
				g.transform.localPosition = new Vector3  (Random.Range (-0.25f, 0.25f),Random.Range (-0.25f, 0.25f), 0);
				g.transform.localScale = new Vector3 (Random.Range (0.7f,1.0f), Random.Range (0.7f, 1.0f), 0);
				carFireEffect.Add (g);
			}
		}

		if (carExploding == true) {
			explodeTimer -= Time.deltaTime;
			if (explodeTimer <= 0) {
				Vector3 pos = new Vector3 (Random.Range (this.transform.position.x - 1, this.transform.position.x + 1), Random.Range (this.transform.position.y - 1, this.transform.position.y + 1), 0);
				GameObject g = (GameObject) Instantiate (CommonObjectsStore.me.grenadeExplosion, pos, Quaternion.Euler (0, 0, 0));
				g.GetComponent<SpriteRenderer> ().sortingOrder = 12;
				explodeTimer = Random.Range (1.0f, 5.0f);
				this.gameObject.GetComponentInChildren<SpriteRenderer> ().color = new Color (0.5f, 0.5f, 0.5f, 1);

				if (myRoadFollower == null) {

				} else {
					if (myRoadFollower.hasDriver == true) {
						GameObject g2 = (GameObject)Instantiate (CommonObjectsStore.me.civilian, driversDoor.transform.position, Quaternion.Euler (0, 0, 0));
						NPCController npc = g2.GetComponent<NPCController> ();
						NPCBehaviourDecider npcb = g2.GetComponent<NPCBehaviourDecider> ();
						NPCMemory npcm = g2.GetComponent<NPCMemory> ();
						npcm.beenAttacked = true;
						npcm.seenSuspect = true;
						//npcm.objectThatMadeMeSuspisious = CommonObjectsStore.player;
						npcm.raiseAlarm = true;
						npcb.alarmed = true;
					}

					myRoadFollower.hasDriver = false;
				}
			}
		}

	}
	public Light[] headlights,brakelights;

	bool shouldLightsBeOn()
	{
		if (Vector2.Distance (CommonObjectsStore.player.transform.position, Camera.main.transform.position) > 12) {
			return false;
		}
		return true;
	}

	bool playerLightSwitch=false;
	bool lightsOn=false;
	void headlightControls()
	{
		if (shouldLightsBeOn () == true) {
				if (playerLightSwitch==true&& lightsOn == false) {
					foreach (Light l in headlights) {
						l.enabled = true;
					}
					lightsOn = true;
				}

				if (playerLightSwitch==false&&lightsOn == true) {
					foreach (Light l in headlights) {
						l.enabled = false;
					}
					lightsOn = false;
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
		if (isCarBraking==true) {
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
