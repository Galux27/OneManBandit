using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that controls the health of a human character. 
/// </summary>
public class PersonHealth : MonoBehaviour {
	public static PersonHealth playerHealth;
	// Use this for initialization
	public float armourValue=0.0f; //value between 0 & 100
	public float healthValue = 5200.0f;
	public Inventory myInventory;
	bool dead = false;
	public BleedingEffect be;
	public bool playerInvinvible = true,amIInvincible=false;
	void Awake()
	{
		myInventory = this.gameObject.GetComponent<Inventory> ();
	}

	void Start () {
		if (CommonObjectsStore.player == this.gameObject) {
			playerHealth = this;
		}
		be = this.GetComponent<BleedingEffect> ();
	}

	void Update()
	{
		

		armourValue = myInventory.getArmourMod (); 

		//Player is invincible in editor for convinience
		if (this == playerHealth && Application.isEditor==true && playerInvinvible==true|| Application.isEditor==true && amIInvincible==true) {

			//healthValue = 5200.0f;
		}
		if (healthValue <= 0) {
			if (dead == false) {
				die ();
				dead = true;
			}
		}
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject == null) {

		} else {
			Bullet b =col.gameObject.GetComponent<Bullet>();
			if (b==null) {
				
			}else{
				
				if (playerHealth == this && b.isAiBullet==true) {
					CameraController.me.hitByBullet (new Vector2 (this.transform.position.x, this.transform.position.y));
				}
				if (b.isShrapnal == false) {
					if (b.isAiBullet == true && this.gameObject.tag == "Player" || b.isAiBullet == false && this.gameObject.tag == "NPC") {
						dealDamage (b.damage, true);
					}
				} else {
					dealDamage (b.damage, true);
				}

				if (this.gameObject.tag == "NPC") {
					NPCController npc = this.gameObject.GetComponent<NPCController> ();

					npc.memory.beenAttacked = true;

					if (b.shooter == null) {

					} else {
						if (npc.detect.fov.visibleTargts.Contains (b.shooter.transform) == true) {
							npc.memory.peopleThatHaveAttackedMe.Add (b.shooter);
						}
					}

				}
				if (b.shooter == null) {
				} else {
					if (playerHealth != this) {
						this.GetComponent<NPCController> ().npcB.setAttacked ();
					}
				}

				be.bloodImpact (col.otherCollider.gameObject.transform.position, b.gameObject.transform.rotation);

			}
		}
	}

	bool shouldWeBleedFromWound()
	{
		float healthAsPercentage = (healthValue / 5200)*100;
		int chance = Random.Range (0, 200);
		int odds = Mathf.RoundToInt(healthAsPercentage + (armourValue));


		if (chance > odds) {
			return true;
		} else {
			return false;
		}


	}

	public void die()
	{
		//add corpse to some kind of dead person list?
		if (this != playerHealth) {
			NPCManager.me.npcsInWorld.Remove (this.gameObject);
			NPCManager.me.corpsesInWorld.Add (this.gameObject);
			NPCController npc = this.GetComponent<NPCController> ();
			NPCManager.me.npcControllers.Remove (npc);
			if (npc.myText == null) {

			} else {
				npc.myText.fadeOutText = true;
			}
			Destroy (npc.currentBehaviour);
			npc.enabled = false;
			//this.GetComponent<NPCController> ().enabled = false;
			this.GetComponent<PersonWeaponController> ().enabled = false;
			this.GetComponent<PathFollower> ().enabled = false;
			this.GetComponent<CanWeDetectTarget> ().enabled = false;
			//this.gameObject.GetComponent<Pathfinding> ().enabled = false;
			this.gameObject.GetComponent<NPCBehaviourDecider> ().enabled = false;
			this.GetComponent<NPCMemory> ().enabled = false;
			this.GetComponent<PersonMovementController> ().enabled = false;
			npc.detect.fov.viewMeshFilter.gameObject.SetActive (false);
			npc.detect.fov.enabled=false;
			this.gameObject.AddComponent<PlayerAction_DragCorpse> ();
			this.gameObject.layer = 27;
			this.gameObject.GetComponentInChildren<CircleCollider2D> ().gameObject.layer = 27;
			this.transform.parent = null;

			Container c = this.gameObject.AddComponent<Container> ();
			c.containerName = "Corpse";
			c.itemsInContainer = new List<Item> ();
			Inventory i = this.gameObject.GetComponent<Inventory> ();
			foreach (Item it in i.inventoryItems) {
				c.addItemToContainer (it);
			}
			i.inventoryItems.Clear ();
		
			if (npc.myHalo == null) {

			} else {
				Destroy (npc.myHalo);
			}
           // if (LoadingDataStore.me.loadingDone==true)
           // {
                LevelIncidentController.me.addIncident("Murder", this.transform.position);
          //  }
		}

		SpriteRenderer[] srs = this.gameObject.GetComponentsInChildren<SpriteRenderer> ();

		foreach (SpriteRenderer sr in srs) {
			sr.sortingOrder = sr.sortingOrder - 10;
		}




		this.GetComponent<Rigidbody2D> ().isKinematic = false;
		this.GetComponent<Rigidbody2D> ().angularVelocity = 0.0f;
		this.GetComponent<Rigidbody2D> ().velocity = new Vector2 (0, 0);

		PersonAnimationController pac = this.GetComponent<PersonAnimationController> ();
		pac.animationsToPlay.Clear ();
		pac.playing = null;
		pac.counter = 0;
		pac.playAnimation ("Die",true);
		this.gameObject.GetComponent<SpriteRenderer> ().sortingOrder = 1;
		ArtemAnimationController ac = this.gameObject.GetComponentInChildren<ArtemAnimationController> ();
		ac.setFallen ();

		this.gameObject.tag = "Dead/Knocked";

		this.gameObject.GetComponent<PersonColliderDecider> ().onDeath ();
		this.enabled = false;
	}
	PersonWeaponController pwc;
	public void dealMeleeDamage(int damage,bool chanceOfBleed)
	{
		
         
		if (pwc == null) {
			pwc = this.gameObject.GetComponent<PersonWeaponController> ();
		}

		if (damage <= 100) {
			this.gameObject.GetComponent<AudioController> ().playSound (SFXDatabase.me.bluntImpact);


		} else {

			if (chanceOfBleed == false) {
				this.gameObject.GetComponent<AudioController> ().playSound (SFXDatabase.me.bluntImpact);
			} else {
				this.gameObject.GetComponent<AudioController> ().playSound (SFXDatabase.me.bladedImpact);
			}
		}
		float modifier = 1.0f;
        if (EffectsManager.me != null)
        {
            foreach (EffectBase eb in EffectsManager.me.effectsOnPlayer)
            {
                modifier += eb.damageMod;
            }
        }
		float dam = damage * modifier;
		if (pwc.blocking == true) {
			dealDamage (Mathf.RoundToInt(dam) / 5, false);
		} else {
			dealDamage (Mathf.RoundToInt(dam), true);
		}


	}

	public void dealDamage(int damage,bool chanceOfBleed)
	{
		if (this == playerHealth &&  playerInvinvible==true) {
		// return;
		}
		float modifier = armourValue / 100;
		modifier /= 2;
        if (EffectsManager.me != null)
        {
            foreach (EffectBase eb in EffectsManager.me.effectsOnPlayer)
            {
                modifier += eb.damageMod;
            }
        }
		//Debug.Log ("Modifier for player damage was " + modifier);
		//modifier++;
		float finalVal = damage * (1-modifier);
		healthValue -= finalVal;
		this.GetComponentInChildren<ArtemAnimationController> ().doRedFlash();
		if (healthValue <= 0) {
			die ();
		} else if (chanceOfBleed==true &&  shouldWeBleedFromWound () == true) {
			//start bleeding
			be.setBleeding();
		}
	}


	public void setAttacked(GameObject attackedBy)
	{
		if (attackedBy.gameObject.tag == "Player") {
			NPCController npc = this.gameObject.GetComponent<NPCController> ();

			if (npc.detect.fov.visibleTargts.Contains (attackedBy.transform)) {
				npc.memory.beenAttacked = true;
				npc.memory.seenSuspect = true;
				npc.memory.objectThatMadeMeSuspisious = attackedBy;
			} else {
				npc.memory.beenAttacked = true;
			}
		} else {
			return;
		}
	}

	float fireTimer = 0.1f;
	public void dealDamageFromFire()
	{
		
		dealDamage (10, false);

	}
}
