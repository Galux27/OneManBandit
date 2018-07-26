using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour {
	public NPCBehaviour currentBehaviour;

	public PersonMovementController pmc;
	public PersonWeaponController pwc;
	public Inventory inv;
	public PathFollower pf;
	public CanWeDetectTarget detect;
	public bool skipAiCheckOnFrame=false;

	public bool knockedDown=false;
	public float knockedDownTimer = 5.0f;
	public AIType myType;
	public NPCMemory memory;
	public NPCBehaviourDecider npcB;
	public PersonTextDisplay myText;
	public PersonColliderDecider myCol;
	public PersonHealth myHealth;
	public ArtemAnimationController ac;
	public PersonClothesController pcc;
	public bool tied = false;
	public GameObject myHalo;
	void Awake()
	{
		pmc = this.GetComponent<PersonMovementController> ();
		pwc = this.GetComponent<PersonWeaponController> ();
		inv = this.GetComponent<Inventory> ();
		detect = this.GetComponent<CanWeDetectTarget> ();
		pf = this.GetComponent<PathFollower> ();
		memory = this.GetComponent<NPCMemory> ();
		npcB = this.GetComponent<NPCBehaviourDecider> ();
		myCol = this.GetComponent<PersonColliderDecider> ();
		myHealth = this.gameObject.GetComponent<PersonHealth> ();
		ac = this.GetComponentInChildren<ArtemAnimationController> ();
		pcc = this.GetComponent<PersonClothesController> ();
	}

	void OnEnable()
	{
		if (NPCManager.me.npcsInWorld.Contains (this.gameObject) == false) {
			NPCManager.me.npcsInWorld.Add (this.gameObject);
		}
		if (NPCManager.me.npcControllers.Contains (this) == false) {
			NPCManager.me.npcControllers.Add (this);
		}
	}



	// Use this for initialization
	void Start () {
		//legs = this.gameObject.GetComponentInChildren<Legs> ().gameObject;
		head = this.gameObject.GetComponentInChildren<ArtemAnimationController> ().getHead();
		myType = npcB.myType;
		GameObject textObj = (GameObject)Instantiate (CommonObjectsStore.me.personText, this.transform.position, Quaternion.Euler (0, 0, 0));
		textObj.transform.SetParent(CommonObjectsStore.me.mainCam.GetComponentInChildren<Canvas> ().gameObject.transform,false);//TODO test if this works, game was coming up with a warning
		myText = textObj.GetComponentInChildren<PersonTextDisplay> ();
		myText.toFollow = this.gameObject;
		textObj.transform.localScale = new Vector3 (1, 1, 1);

		if (NPCManager.me.npcsInWorld.Contains (this.gameObject) == false) {
			NPCManager.me.npcsInWorld.Add (this.gameObject);
		}
		if (NPCManager.me.npcControllers.Contains (this) == false) {
			NPCManager.me.npcControllers.Add (this);
		}

	//	NPCBehaviour test = this.gameObject.AddComponent<NPCBehaviour_SearchPerson> ();
		//test.passInGameobject (CommonObjectsStore.player);
		//currentBehaviour = test;
	}
	public float stunTimer = 0.0f;

	// Update is called once per frame
	void Update () {
		if (currentBehaviour == null) {
			return;
		}

		if (stunTimer > 0) {
			if (myHalo == null) {
				myHalo = (GameObject)Instantiate (CommonObjectsStore.me.stunHalo, ac.getHead ().transform);
				myHalo.transform.localPosition = Vector3.zero;
			}
			stunTimer -= Time.deltaTime;
			return;
		}

		if (knockedDown == false) {
			if (myHalo != null) {
				Destroy (myHalo);
			}
		}
		//if (Time.deltaTime > 0.1f) {
			//Debug.Log (this.gameObject.name + " was doing " + currentBehaviour.myType);
		//}


		if (knockedDown == false && npcB.myType!=AIType.hostage) {
			skipAiCheckOnFrame = false;
			if (currentBehaviour == null) {
			} else {
				currentBehaviour.OnUpdate ();
			}


			//if (npcB.myType == AIType.civilian) {
			//	if (currentBehaviour==null ||  currentBehaviour.myType != behaviourType.civilianAction) {
			//		if (pwc.enabled == true) {
			//			pwc.OnUpdate ();
			//		}
			//	}
			//} else {
			//	if (pwc.enabled == true) {
			//		pwc.OnUpdate ();
			//	}
			//}


			/*if (skipAiCheckOnFrame == false) {
				if (myType == AIType.aggressive) {
					AgressiveAI ();
				} else if (myType == AIType.guard) {
					GuardAI ();
				}
			}*/
		} else if(knockedDown==true){
			knockedOut ();
		}
	}

	void GuardAI()
	{
		if (currentBehaviour == null) {
			if (doWeHaveAWeapon () == true) {
				if (currentBehaviour == null || currentBehaviour.myType != behaviourType.patrol && currentBehaviour.myType != behaviourType.getAmmo) {
					NPCBehaviour_PatrolRoute newBehaviour = this.gameObject.AddComponent<NPCBehaviour_PatrolRoute> ();
					if (currentBehaviour == null) {

					} else {
						Destroy (currentBehaviour);
					}
					currentBehaviour = newBehaviour;
				}
			} else {
				NPCBehaviour_FindGear newBehaviour = this.gameObject.AddComponent<NPCBehaviour_FindGear> ();
				if (currentBehaviour == null) {

				} else {
					Destroy (currentBehaviour);
				}
				currentBehaviour = newBehaviour;
			}
		} else {
			if (doWeHaveAWeapon () == true) {
				if (memory.suspisious == false) {
					if (currentBehaviour == null || currentBehaviour.myType != behaviourType.patrol && currentBehaviour.myType != behaviourType.getAmmo) {
						NPCBehaviour_PatrolRoute newBehaviour = this.gameObject.AddComponent<NPCBehaviour_PatrolRoute> ();
						if (currentBehaviour == null) {

						} else {
							Destroy (currentBehaviour);
						}
						currentBehaviour = newBehaviour;
					}
				} else {
					if (currentBehaviour.myType != behaviourType.investigate) {
						NPCBehaviour_InvestigateObject newBehaviour = this.gameObject.AddComponent<NPCBehaviour_InvestigateObject> ();
						newBehaviour.passInGameobject (memory.objectThatMadeMeSuspisious);
						//newBehaviour.Initialise ();
						if (currentBehaviour == null) {

						} else {
							Destroy (currentBehaviour);
						}
						currentBehaviour = newBehaviour;

					}
				}
			} else {
				NPCBehaviour_FindGear newBehaviour = this.gameObject.AddComponent<NPCBehaviour_FindGear> ();
				if (currentBehaviour == null) {

				} else {
					Destroy (currentBehaviour);
				}
				currentBehaviour = newBehaviour;
			}
		}
	}


	void AgressiveAI()
	{
		if (doWeHaveAWeapon () == true) {
			if (canWeFireWeapon () == true) {
				if (currentBehaviour == null || currentBehaviour.myType != behaviourType.attackTarget) {
					NPCBehaviour_AttackTarget newBehaviour = this.gameObject.AddComponent<NPCBehaviour_AttackTarget> ();
					newBehaviour.passInGameobject (CommonObjectsStore.player);
					if (currentBehaviour == null) {

					} else {
						Destroy (currentBehaviour);
					}
					currentBehaviour = newBehaviour;
				}
			} else {
				if (currentBehaviour == null || currentBehaviour.myType != behaviourType.getAmmo && currentBehaviour.myType != behaviourType.getWeapon) {
					if (pwc.currentWeapon == null) {
						NPCBehaviour_FindGear newBehaviour = this.gameObject.AddComponent<NPCBehaviour_FindGear> ();
						//////Debug.Log ("GEAR FIND 1");
						if (currentBehaviour == null) {

						} else {
							Destroy (currentBehaviour);
						}
						currentBehaviour = newBehaviour;
					} else {
						NPCBehaviour_FindAmmo newBehaviour = this.gameObject.AddComponent<NPCBehaviour_FindAmmo> ();
						if (currentBehaviour == null) {

						} else {
							Destroy (currentBehaviour);
						}
						currentBehaviour = newBehaviour;
					}
				}

				/*if (currentBehaviour == null || currentBehaviour.myType != behaviourType.getWeapon) {
					NPCBehaviour_FindGear newBehaviour = this.gameObject.AddComponent<NPCBehaviour_FindGear> ();
					if (currentBehaviour == null) {

					} else {
						Destroy (currentBehaviour);
					}
					currentBehaviour = newBehaviour;
				}*/
			}
		} else {


			if (currentBehaviour == null || currentBehaviour.myType != behaviourType.getWeapon && currentBehaviour.myType != behaviourType.attackTarget) {
				//////Debug.Log ("GEAR FIND 2");

				NPCBehaviour_FindGear newBehaviour = this.gameObject.AddComponent<NPCBehaviour_FindGear> ();
				if (currentBehaviour == null) {

				} else {
					Destroy (currentBehaviour);
				}
				currentBehaviour = newBehaviour;
			}
		}
	}

	public bool doWeHaveAWeapon()
	{
		if (pwc.currentWeapon == null) {
			return false;
		} else {
			return true;
		}
	}

	public bool canWeFireWeapon()
	{
		if (pwc.currentWeapon == null) {
			return true;
		}else{
			if (pwc.currentWeapon.melee == true) {
				return true;
			} else {
				if (inv.getAmmoForGun (pwc.currentWeapon.WeaponName) == null) {
					return false;
				} else {
					return true;
				}
			}
		}
	}

	public void knockOutNPC()
	{
		if (npcB.myType == AIType.hostage) {
			return;
		}
		ArtemAnimationController ac = this.gameObject.GetComponentInChildren<ArtemAnimationController> ();
		ac.setFallen();
		PersonAnimationController pac = this.GetComponent<PersonAnimationController> ();
		//pac.forceFinishCurrentAnim ();
		pac.animationsToPlay.Clear ();
		pac.playing = null;
		pac.counter = 0;
		pac.playAnimation ("Knock",true);
		knockedDown = true;
		this.gameObject.GetComponent<SpriteRenderer> ().sortingOrder = 1;
		this.gameObject.tag = "Dead/Knocked";
		//ac.disableBodyParts ();
		//legs = this.GetComponentInChildren<Legs> ().gameObject;
		//legs.SetActive (false);
		head = ac.getHead(); //this.GetComponentInChildren<HeadController> ().gameObject;
		//this.GetComponent<SpriteRenderer> ().sprite = ac.myAesthetic.knocked;
		this.GetComponent<Rigidbody2D> ().isKinematic = true;
		this.GetComponent<Rigidbody2D> ().angularVelocity = 0.0f;
		this.GetComponent<Rigidbody2D> ().velocity = new Vector2 (0, 0);
		//head.SetActive (false);
		pmc.enabled = false;
		pwc.enabled = false;
		//this.GetComponent<Collider2D> ().isTrigger = true;
		myCol.setTrigger();
		this.gameObject.GetComponentInChildren<CircleCollider2D> ().enabled = false;

		detect.fov.enabled = false;
		detect.fov.viewMeshFilter.gameObject.SetActive (false);
		//this.gameObject.AddComponent<PlayerAction_TieUpHostage> ();
		if (this.GetComponent<PlayerAction_HumanShield> () == true) {
			this.GetComponent<PlayerAction_HumanShield> ().enabled = false;
		}
		SpriteRenderer[] srs = this.gameObject.GetComponentsInChildren<SpriteRenderer> ();

		foreach (SpriteRenderer sr in srs) {
			sr.sortingOrder = sr.sortingOrder - 10;
		}

		Container c = this.gameObject.AddComponent<Container> ();
		c.containerName = "Unconscious Person";
		c.itemsInContainer = new List<Item> ();
		Inventory i = this.gameObject.GetComponent<Inventory> ();
		foreach (Item it in i.inventoryItems) {
			c.addItemToContainer (it);
		}
		i.inventoryItems.Clear ();
		this.gameObject.AddComponent<PlayerAction_DragCorpse> ();

	}

	public GameObject legs,head;

	public void reviveNPC()
	{
		knockedDownTimer = 0.0f;
	}


	public void knockedOut()
	{
		if (npcB.myType == AIType.hostage) {
			return;
		}

		//knockedDownTimer -= Time.deltaTime;

		if (knockedDownTimer <= 0) {
			PersonAnimationController pac = this.GetComponent<PersonAnimationController> ();
			pac.playAnimation ("Unarmed",true);
			knockedDownTimer = 5.0f;
			knockedDown = false;
			this.gameObject.tag = "NPC";

			//legs.SetActive (true);
			ac.enableBodyParts();
			head.SetActive (true);
			pmc.enabled = true;
			pwc.enabled = true;

			myCol.setSolid();
			this.gameObject.GetComponent<SpriteRenderer> ().sortingOrder = 5;
			this.GetComponent<SpriteRenderer> ().sprite = null;

			detect.fov.enabled = true;
			detect.fov.viewMeshFilter.gameObject.SetActive (true);
			//npcB.
			npcB.alarmed = true;
			myText.setText ("Ughhh my head");
			this.gameObject.GetComponentInChildren<CircleCollider2D> ().enabled = true;
			//ArtemAnimationController ac = this.gameObject.GetComponentInChildren<ArtemAnimationController> ();
			ac.setUp ();
			if (this.GetComponent<PlayerAction_HumanShield> () == true) {
				this.GetComponent<PlayerAction_HumanShield> ().enabled = true;
			}
			SpriteRenderer[] srs = this.gameObject.GetComponentsInChildren<SpriteRenderer> ();

			foreach (SpriteRenderer sr in srs) {
				sr.sortingOrder = sr.sortingOrder + 10;
			}
			Container c = this.GetComponent<Container> ();
			Inventory i = this.GetComponent<Inventory> ();
			foreach(Item item in c.itemsInContainer)
			{
				i.addItemToInventory (item);
			}
			c.itemsInContainer.Clear ();
			Destroy (c);
			if (this.GetComponent<PlayerAction_SearchContainer> () == true) {
				Destroy (this.GetComponent<PlayerAction_SearchContainer> ());
			}

			if (this.GetComponent<PlayerAction_DragCorpse> () == true) {
				Destroy (this.GetComponent<PlayerAction_DragCorpse> ());
			}

			if (this.GetComponent<PlayerAction_DropBody> () == true) {
				Destroy (this.GetComponent<PlayerAction_DropBody> ());
			}
		}
	}


	public void setHearedGunshot(Vector3 pos,float range)
	{
		if (npcB.myType != AIType.civilian) {
			if (Vector3.Distance (this.transform.position, pos) < range) {
				memory.noiseToInvestigate = pos;
				memory.objectThatMadeMeSuspisious = null;
				npcB.suspisious = true;
			}
		} else {
			if (Vector3.Distance (this.transform.position, pos) < range) {
				memory.noiseToInvestigate = pos;
				memory.objectThatMadeMeSuspisious = null;
				npcB.alarmed = true;
			}
		}
	}

	public void untieHostage(){
		if (npcB.myType != AIType.hostage) {
			return;
		}

		if (this.GetComponent<PlayerAction_HumanShield> () == true) {
			this.GetComponent<PlayerAction_HumanShield> ().enabled = true;
		}

		npcB.myType = myType;
		this.gameObject.tag = "NPC";
		detect.fov.viewMeshFilter.gameObject.SetActive (true);
		npcB.myType = myType;
		//PersonAnimationController pac = this.gameObject.GetComponent<PersonAnimationController> ();
		//pac.animationsToPlay.Clear ();
		//pac.playAnimation ("TiedUp", false);
		//pac.playing = null;
		//pac.forceFinishCurrentAnim ();
		//head.gameObject.SetActive (true);
		//pac.playAnimation ("Unarmed",true);
		knockedDownTimer = 5.0f;
		knockedDown = false;
		this.gameObject.tag = "NPC";
		this.GetComponent<SpriteRenderer> ().sprite = null;
		ac.setUntied ();
		//ArtemAnimationController ac = this.gameObject.GetComponentInChildren<ArtemAnimationController> ();
		ac.setUp ();
		//ac.enableBodyParts ();
		//hostage.ac.torso.GetComponent<SpriteRenderer> ().sortingOrder = 1;
		ac.enableLegs();
		//legs = this.GetComponentInChildren<Legs> ();
		//legs.SetActive (true);
		//head = this.GetComponentInChildren<HeadController> ();
		this.gameObject.GetComponentInChildren<CircleCollider2D> ().enabled = true;

		head.SetActive (true);
		pmc.enabled = true;
		pwc.enabled = true;
		//this.GetComponent<Collider2D> ().isTrigger = false;
		myCol.setSolid();
		this.gameObject.GetComponent<SpriteRenderer> ().sortingOrder = 5;
		detect.fov.enabled = true;
		npcB.alarmed = true;
		//knockedDown = false;
		//this.gameObject.GetComponent<Collider2D> ().enabled = true;
	}

	void OnDestroy()
	{
		NPCManager.me.npcsInWorld.Remove (this.gameObject);
		NPCManager.me.npcControllers.Remove (this);

	}


}
public enum AIType{
	aggressive,//just try to kill the player
	guard, //walk around, tries to spot things out of the oridinary
	cop, //gets called by guards, will work as a team to try and secure a building, will call for backup if they lose a member or 
	civilian, //walks around and does activities, will flee if they spot something bad, will try to raise alarm if they are far enough from the player
	swat,
	hostage
}
