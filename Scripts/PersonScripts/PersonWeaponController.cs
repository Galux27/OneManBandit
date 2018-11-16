using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that controls the weapons of a human character
/// </summary>
public class PersonWeaponController : MonoBehaviour {
	PersonAnimationController pac;
	public Weapon currentWeapon;
	public SpriteRenderer sr;
	public bool aimDownSight=false,reloading=false;
	ArtemAnimationController ac;
	public Transform bulletSpawn_oneHandHip,bulletSpawn_oneHandADS,bulletSpawn_twoHandHip,bulletSpawn_twoHandADS,dropWeaponSpawn,throwWeaponSpawn;
	public float fireRateTimer = 0.0f, reloadTimer=0.0f;
	public Inventory myInv;

	public bool animationConnected=false,startedAttack=false,isPunch=false;
	void Awake()
	{
		sr=this.GetComponent<SpriteRenderer>();
		pac = this.GetComponent<PersonAnimationController> ();
		myInv = this.GetComponent<Inventory> ();
		ac = this.GetComponentInChildren<ArtemAnimationController> ();
	}


	public bool blocking = false;
	public void block()
	{
		if (currentWeapon == null || currentWeapon.melee == true) {
			blocking = true;
			ac.block ();
		}
	}

	public void stopBlocking()
	{
		blocking = false;
		ac.stopBlock ();
	}
	
	// Update is called once per frame
	void Update () {
		newMelee ();
		counDownFirerateTimer ();

		if (throwingWeapon == true) {
			throwWeaponCheck ();
		} else if (droppingWeapon == true) {
			dropWeaponCheck ();
		} else {
			ammoItemCheck ();
			areWeAimingDownSight ();
			//decideMyTorsoAnimation ();
			if (reloading == true) {
				reload ();
			}
		}

	}

	void ammoItemCheck()
	{
		if (currentWeapon == null || currentWeapon.melee==true) {

		} else {
			if (currentWeapon.ammoItem == null) {
				reloading = true;
				//Debug.Log ("Reloading true 1");
			} else {
				if (currentWeapon.ammoItem.gameObject.transform.parent == null) {
					currentWeapon.ammoItem = null;
					reloadTimer = currentWeapon.reloadTime;
					reloading = true;
					//Debug.Log ("Reloading true 2");

					////////Debug.LogError ("Ammo item was not in inventory, setting to null");
				}
			}
		}
	}


	public void setWeapon(Weapon w)
	{
		
		if (currentWeapon == null) {
			
		} else {
			if (w == null) {

			} else {
				currentWeapon.unequipItem ();
			}
		}

		if (w == null) {
			currentWeapon=null;
		} else {

			currentWeapon = w;
			reloadTimer = currentWeapon.reloadTime;

		}
		if (currentWeapon == null) {

		} else {
			ac.changeGunState ();
		}
	}



	void areWeAimingDownSight(){
		if (currentWeapon == null) {

		} else {
			if (myInv.leftArm == null) {

			} else {
				if (currentWeapon.slot != itemEquipSlot.bothHands) {
					aimDownSight = false;
				}
			}
		}
	}



	public void Punch()
	{
		
		if (this.gameObject.tag == "NPC") {
			fireRateTimer = 1.0f;
		}
		animationConnected = false;
		startedAttack = false;
	}

	public List<GameObject> objectsRayHitInSwing;

	void newMeleeDealDamage(GameObject g)
	{
		if (g.tag == this.gameObject.tag) {
			//Debug.Log ("hit freindly, returning");
			return;
		}


		if (currentWeapon == null) {
			//foreach (GameObject g in objectsRayHitInSwing) {
				PersonHealth ph = g.GetComponent<PersonHealth> ();
				if (ph == null) {
				if (g.tag == "Door") {
					//Debug.Log ("Melee hit " + g.gameObject.name);
					DoorScript ds = g.GetComponent<DoorScript> ();
					if (ds == null) {
						if (g.transform.parent == null) {
							//Debug.Log ("Could not find door script in parent");

						} else {
							ds = g.transform.root.gameObject.GetComponent<DoorScript> ();
						}
					}


					if (ds == null) {
						//Debug.Log ("Could not find door script");
					} else {
						ds.kickInDoor ();
					}
					////////Debug.Log ("Door " + ds.gameObject + " kicked in");
					return;
				} else if (g.tag == "Window") {
					Window w = g.GetComponent<Window> ();
					//Debug.Log (g.name);
					if (w == null) {

					} else {
						w.smashWindow ();
					}

					WindowNew w2 = g.GetComponentInParent<WindowNew> ();
					if (w2 == null) {
					} else {
						w2.destroyWindow ();
					}

					return;
				} else if (g == this.gameObject) {
					return;

				}  else {

				}
				} else {
					if (g == this.gameObject || ph.healthValue <= 0) {
					return;

					}

				if (g.tag != "Player") {
					Inventory i = ph.gameObject.GetComponent<Inventory> ();
					if (i.leftArm == null) {

					} else {
						Item toDrop = i.leftArm;
						i.unequipItem (toDrop);

						i.dropItem (toDrop);
					}

					if (i.rightArm == null) {

					} else {
						Item toDrop = i.rightArm;
						i.unequipItem (toDrop);

						i.dropItem (toDrop);
					}
				}
					NPCController npc = g.GetComponent<NPCController> ();
					if (npc == null) {
						ph.gameObject.GetComponent<BleedingEffect> ().bloodImpact (g.transform.position, Quaternion.Euler (0, 0, this.transform.eulerAngles.z - 90));

						//bool inFront = detect.isTargetInFrontOfUs (this.gameObject);
						ph.dealMeleeDamage (500, false);
						punchNoise ();


					} else {


						ph.gameObject.GetComponent<BleedingEffect> ().bloodImpact (g.transform.position, Quaternion.Euler (0, 0, this.transform.eulerAngles.z - 90));

						CanWeDetectTarget detect = g.GetComponent<CanWeDetectTarget> ();
						//bool inFront = detect.isTargetInFrontOfUs (this.gameObject);
						ph.dealMeleeDamage (500, false);
						punchNoise ();
						if (ph.healthValue > 0) {

						if (isTargetFacingAway (npc.gameObject)) {
							npc.knockOutNPC ();
						}
							ph.setAttacked (this.gameObject);
						npc.stunTimer = 3.0f;
							////////Debug.Break ();
						}

						//if (inFront == true) {
					if (this.gameObject.tag == "Player") {//TODO may need to rewrite this in the far future 
						npc.memory.peopleThatHaveAttackedMe.Add (this.gameObject);
					}

						//}
						if (g.tag == "NPC") {
							npc.memory.objectThatMadeMeSuspisious = this.gameObject;
							npc.npcB.onHostageRelease ();
						}
					}
				}
			}
			else{
			PersonHealth ph = g.GetComponent<PersonHealth> ();
			if (ph == null) {
				if (g.tag == "Door") {
					//Debug.Log ("Melee hit " + g.gameObject.name);
					DoorScript ds = g.GetComponent<DoorScript> ();
					if (ds == null) {
						if (g.transform.parent == null) {
							//Debug.Log ("Could not find door script in parent");

						} else {
							ds = g.transform.root.gameObject.GetComponent<DoorScript> ();
						}
					}


					if (ds == null) {
						//Debug.Log ("Could not find door script");
					} else {
						ds.kickInDoor ();
					}
					////////Debug.Log ("Door " + ds.gameObject + " kicked in");
					return;

				} else if (g.tag == "Window") {
					Window w = g.GetComponent<Window> ();
					//Debug.Log (g.name);
					if (w == null) {

					} else {
						w.smashWindow ();
					}

					WindowNew w2 = g.GetComponentInParent<WindowNew>();
					if (w2 == null) {
					} else {
						w2.destroyWindow ();
					}

					return;

				}else if (g.GetComponent<PlayerCarController> () == true) {
					g.GetComponent<PlayerCarController> ().dealDamage (500);
					return;
				}
				else if (g == this.gameObject) {
					return;

				}
			} else {
				if (g== this.gameObject || ph.healthValue<=0) {
					return;

				}

				NPCController npc = g.GetComponent<NPCController> ();
				if (npc == null) {
					ph.gameObject.GetComponent<BleedingEffect> ().bloodImpact (g.transform.position, Quaternion.Euler (0, 0, this.transform.eulerAngles.z - 90));

					//bool inFront = detect.isTargetInFrontOfUs (this.gameObject);
					ph.dealMeleeDamage (currentWeapon.meleeDamage, currentWeapon.bladed);
					punchNoise ();

				} else {


					ph.gameObject.GetComponent<BleedingEffect> ().bloodImpact (g.transform.position, Quaternion.Euler (0, 0, this.transform.eulerAngles.z - 90));

					CanWeDetectTarget detect = g.GetComponent<CanWeDetectTarget> ();
					//bool inFront = detect.isTargetInFrontOfUs (this.gameObject);
					ph.dealMeleeDamage (currentWeapon.meleeDamage, currentWeapon.bladed);
					punchNoise ();
					if (ph.healthValue > 0) {
						if (currentWeapon.bladed == false && isTargetFacingAway (npc.gameObject)) {
							npc.knockOutNPC ();
						}
						ph.setAttacked (this.gameObject);

						////////Debug.Break ();
					}

					//if (inFront == true) {
					if (this.gameObject.tag == "Player") {//TODO may need to rewrite this in the far future 
						npc.memory.peopleThatHaveAttackedMe.Add (this.gameObject);
					}

					//}
					if (g.tag == "NPC") {
						npc.memory.objectThatMadeMeSuspisious = this.gameObject;
						npc.npcB.onHostageRelease ();
					}
				}
				//if (g != this.gameObject) {
				//	ph.dealMeleeDamage (100, false);
				//}
			}
		}

	}
	bool stopAttack=false;
	Vector3 getMeleeDirection()
	{
		if (currentWeapon == null) {
			return (this.transform.position - ac.rightHand.transform.position)*-1;
		} else if (currentWeapon.melee == true) {
			return (this.transform.position -currentWeapon.transform.position)*-1;
		} else {
			return Vector3.zero;
		}
	}

	float getRayDistance()
	{
		if (currentWeapon == null) {
			return 1.5f;
		} else {
			if (currentWeapon.WeaponName == "Hammer") {
				return 2.0f;
			} else if (currentWeapon.WeaponName == "Knife") {
				return 1.5f;
			} else if (currentWeapon.WeaponName == "Baton") {
				return 2.0f;
			}
		}
		return 0.0f;
	}

	void newMelee()
	{
		
		if (startedAttack == true && animationConnected == false) {
			if (objectsRayHitInSwing == null) {
				objectsRayHitInSwing = new List<GameObject> ();
			}
			if (currentWeapon == null) {
				Vector3 rayDir = getMeleeDirection ();

				RaycastHit2D[] rays = Physics2D.RaycastAll (this.transform.position, rayDir.normalized, getRayDistance(),CommonObjectsStore.me.maskForMelee);
				//Debug.DrawRay (this.transform.position, rayDir.normalized * getRayDistance(), Color.red,10.0f);

				int i = 0;
				foreach (RaycastHit2D ray in rays) {
					if (ray.collider == null) {

					} else {
						////Debug.LogError ("Punch hit "  + ray.collider.gameObject.name + i.ToString ());
						i++;
					}
				}

				foreach (RaycastHit2D ray in rays) {
					if (ray.collider == null) {

					} else {
						
						if (objectsRayHitInSwing.Contains (ray.collider.gameObject) == false && stopAttack==false) {
							objectsRayHitInSwing.Add (ray.collider.gameObject);
							if (ray.collider.gameObject.tag == "NPC" && ray.collider.gameObject != this.gameObject || ray.collider.gameObject.tag == "Player" && ray.collider.gameObject != this.gameObject) {
								newMeleeDealDamage (ray.collider.gameObject);
							} else if (ray.collider.gameObject == this.gameObject) { 


							} else if (ray.collider.GetComponent<DoorScript> () == true) {
								newMeleeDealDamage (ray.collider.gameObject);
								stopAttack = true;
							} else if (ray.collider.tag == "Window") {
								newMeleeDealDamage (ray.collider.gameObject);
								stopAttack = true;
							} 
							else{


								
								////Debug.LogError ("Melee hit " + ray.collider.gameObject.name + " stopping melee attack");
								stopAttack = true;
							}
						}
					}
				}

			} else {
				Vector3 rayDir = getMeleeDirection ();
				RaycastHit2D[] rays = Physics2D.RaycastAll (this.transform.position, rayDir.normalized, getRayDistance(),CommonObjectsStore.me.maskForMelee);
				//Debug.DrawRay (this.transform.position, rayDir.normalized * getRayDistance(), Color.red,10.0f);
				foreach (RaycastHit2D ray in rays) {
					if (ray.collider == null) {

					} else {
						if (objectsRayHitInSwing.Contains (ray.collider.gameObject) == false && stopAttack==false) {
							objectsRayHitInSwing.Add (ray.collider.gameObject);
							if (ray.collider.gameObject.tag == "NPC" && ray.collider.gameObject != this.gameObject || ray.collider.gameObject.tag == "Player" && ray.collider.gameObject != this.gameObject) {
								newMeleeDealDamage (ray.collider.gameObject);
							}else if (ray.collider.gameObject == this.gameObject) { 


							} else if (ray.collider.GetComponent<DoorScript> () == true) {
								newMeleeDealDamage (ray.collider.gameObject);
								stopAttack = true;
							} else if (ray.collider.tag=="Window") {
								newMeleeDealDamage (ray.collider.gameObject);
								stopAttack = true;
							}else if (ray.collider.gameObject.layer == 28) {
								newMeleeDealDamage (ray.collider.gameObject);
								stopAttack = true;
							}else {
								////Debug.LogError ("Melee hit " + ray.collider.gameObject.name + " stopping melee attack");

								stopAttack = true;
							}
						}
					}
				}
			}
		} else if (startedAttack == true && animationConnected == true) {
			objectsRayHitInSwing.Clear ();
			stopAttack = false;
		}
	}


	public void fireWeapon()
	{
		if (currentWeapon == null) {
			if (canWeFireWeapon () == false) {
				return;
			}

			if (canWeFireWeapon () == true) {
				//Punch ();
				if (startedAttack == false) {
					this.gameObject.GetComponent<AudioController> ().playSound (SFXDatabase.me.swing);
				}

				ac.setMeleeAttack ();

				isPunch=true;
				startedAttack = true;
			}
		} else {
			if (currentWeapon.melee == false) {
				//Debug.Log ("Trying to fire gun");
				if (canWeFireWeapon () == false) {
					//fireRateTimer -= Time.deltaTime;
					if (this.gameObject.tag == "NPC") {

						if (currentWeapon == null) {
						} else {
							if (currentWeapon.ammoItem == null) {

							} else {
								if (currentWeapon.ammoItem.ammoCount <= 0) {
									playGunClick ();

									reloading = true;
									//Debug.Log ("Reloading true 3");
								}
							}
						}
					}
					return;
				}

				if (currentWeapon.ammoItem == null  || currentWeapon.ammoItem.ammoCount==0) {
					playGunClick ();
					if (myInv.canWeReloadGun (currentWeapon.WeaponName) || this.gameObject.tag=="NPC") {
						reloading = true;
						//Debug.Log ("Reloading true 4");

					}
					////Debug.Break ();
					return;
					//currentWeapon.ammoItem = myInv.getAmmoForGun (currentWeapon.WeaponName);
				}

				if (reloading == true) {
					playGunClick ();

					return;
				}

				if (canWeFireWeapon () == false || areWeDrawingWeapon () == true || currentWeapon.WeaponName == "Unarmed" || currentWeapon.ammoItem == null) {
					playGunClick ();

					return;
				}

				if (currentWeapon.ammoItem.ammoCount > 0) {

					Quaternion rotation = Quaternion.Euler (this.transform.rotation.x, this.transform.rotation.y, Random.Range (this.transform.rotation.eulerAngles.z - currentWeapon.recoilMax, this.transform.rotation.eulerAngles.z + currentWeapon.recoilMax));

					if (aimDownSight == true) {
						rotation = Quaternion.Euler (this.transform.rotation.x, this.transform.rotation.y, Random.Range (this.transform.rotation.eulerAngles.z - (currentWeapon.recoilMax / 2), this.transform.rotation.eulerAngles.z + (currentWeapon.recoilMax / 2)));
					} else {
						rotation = Quaternion.Euler (this.transform.rotation.x, this.transform.rotation.y, Random.Range (this.transform.rotation.eulerAngles.z - currentWeapon.recoilMax, this.transform.rotation.eulerAngles.z + currentWeapon.recoilMax));
					}


						GameObject g = (GameObject)Instantiate (currentWeapon.projectile,currentWeapon.bulletSpawnPos.position, currentWeapon.gameObject.transform.rotation);
						Bullet b = g.GetComponent<Bullet> ();
						b.shooter = this.gameObject;
						if (this.gameObject.tag != "Player") {
							b.isAiBullet = true;
						}
						currentWeapon.playMeleeAnim = true;
						currentWeapon.meleeCounter = 0;
						bulletFire ();

					//}
					currentWeapon.ammoItem.decrementAmmo ();
					this.gameObject.GetComponent<AudioController> ().playSound (currentWeapon.attackNoise);
					CameraController.me.bulletRecoilEffect (currentWeapon);
					fireRateTimer = currentWeapon.fireRate;
				} else {

					if (reloading == false) {
						playGunClick ();

						AmmoItem ai = myInv.getAmmoForGun (currentWeapon.WeaponName);
						if (ai == null) {
						} else {
						}
						reloadTimer = currentWeapon.reloadTime;
						reloading = true;
						//Debug.Log ("Reloading true 5");

					}
				}
			} else {

				if (canWeFireWeapon () == true) {

					ac.setMeleeAttack ();
					isPunch = false;
					if (startedAttack == false) {
						this.gameObject.GetComponent<AudioController> ().playSound (currentWeapon.attackNoise);

					}

					startedAttack = true;

					////////Debug.Break ();
				}
				//meleeAttack ();
			}
		}
	}

	float lastPlayedTimer = 0.0f;
	void playGunClick()
	{
		//Debug.Log ("Calling gun click");
			lastPlayedTimer -= Time.deltaTime;

		if (lastPlayedTimer <= 0) {
			this.gameObject.GetComponent<AudioController> ().playSound (currentWeapon.gunEmpty);
			lastPlayedTimer = 1.0f;
		}
	}


	public bool playedReloadAnim=false;
	public void reload()
	{
		if (currentWeapon == null) {
			//Debug.LogError ("reloading set false 1");
			reloading = false;
			return;
		}

		if (this.gameObject.tag == "Player") {
			if (myInv.canWeReloadGun (currentWeapon.WeaponName)) {
				reloadTimer -= Time.deltaTime;

				if (playedReloadAnim == false) {
					

					playedReloadAnim = true;
				}

				//////////Debug.Log ("Reloading");
				if (reloadTimer <= 0) {
					//need to add some kind of timer, have it on update and just have the ammo reaching 0 setting the bool to start reloading to true
					if (this.gameObject.tag == "NPC") {
						GameObject g = (GameObject)Instantiate (ItemDatabase.me.getAmmoItem (currentWeapon), this.transform.position, this.transform.rotation);
						AmmoItem ai = g.GetComponent<AmmoItem> ();
						ai.ammoCount = 0;
						currentWeapon.ammoItem.ammoCount = currentWeapon.ammoItem.maxAmmo;
						RoomScript r = LevelController.me.getRoomObjectIsIn (g);
						if (r == null) {

						} else {
							r.itemsInRoomAtStart.Add (ai);
						}
						//myInv.dropItem (currentWeapon.ammoItem);
					} else {
						currentWeapon.ammoItem = myInv.getAmmoForGun (currentWeapon.WeaponName);
					}
					////////Debug.Log ("Setting new ammo to be " + currentWeapon.ammoItem.getItemName ());
					reloading = false;
					//Debug.LogError ("reloading set false 2");
					reloadTimer = currentWeapon.reloadTime;
					playedReloadAnim = false;
				}
			} else {
				////////Debug.Log ("Couldnt find ammo for weapon, cancelling reload");
				reloading = false;
				//Debug.LogError ("reloading set false 3");

			}
		} else {
			reloadTimer -= Time.deltaTime;

			if (playedReloadAnim == false) {
				playedReloadAnim = true;
			}

			//////////Debug.Log ("Reloading");
			if (reloadTimer <= 0) {
				//need to add some kind of timer, have it on update and just have the ammo reaching 0 setting the bool to start reloading to true
				if (this.gameObject.tag == "NPC") {
					GameObject g = (GameObject)Instantiate (ItemDatabase.me.getAmmoItem (currentWeapon), this.transform.position, this.transform.rotation);
					AmmoItem ai = g.GetComponent<AmmoItem> ();
					ai.ammoCount = 0;
					currentWeapon.ammoItem.ammoCount = currentWeapon.ammoItem.maxAmmo;
					RoomScript r = LevelController.me.getRoomObjectIsIn (g);
					if (r == null) {

					} else {
						r.itemsInRoomAtStart.Add (ai);
					}
					//myInv.dropItem (currentWeapon.ammoItem);
				} else {
					currentWeapon.ammoItem = myInv.getAmmoForGun (currentWeapon.WeaponName);
				}
				reloading = false;
				//Debug.LogError ("reloading set false 2");
				reloadTimer = currentWeapon.reloadTime;
				playedReloadAnim = false;
			}
		}
	}


	public void manualReload(AmmoItem ai)
	{
		reloadTimer -= Time.deltaTime;
		reloading = true;
		//Debug.Log ("Reloading true 6");

		if (playedReloadAnim == false) {
			playedReloadAnim = true;
		}

		if (reloadTimer <= 0) {
			if (this.gameObject.tag == "NPC") {
				GameObject g = (GameObject)Instantiate (ItemDatabase.me.getAmmoItem (currentWeapon), this.transform.position, this.transform.rotation);
				ai.ammoCount = 0;
				currentWeapon.ammoItem.ammoCount = currentWeapon.ammoItem.maxAmmo;
				RoomScript r = LevelController.me.getRoomObjectIsIn (g);
				if (r == null) {

				} else {
					r.itemsInRoomAtStart.Add (ai);
				}
				//myInv.dropItem (currentWeapon.ammoItem);
			} else {
				currentWeapon.ammoItem = ai;
			}
			////////Debug.Log ("Setting new ammo to be " + currentWeapon.ammoItem.getItemName ());
			reloading = false;
			//Debug.LogError ("reloading set false 2");
			reloadTimer = currentWeapon.reloadTime;
			playedReloadAnim = false;
			ai.gameObject.GetComponent<SpriteRenderer> ().enabled = true;
			ai.gameObject.SetActive (false);
		}
	}
	public bool meleeDone = true;
	public void setMeleeDone()
	{
		meleeDone = true;
		startedAttack = false;
		animationConnected = false;
	}


	void counDownFirerateTimer(){
		if (fireRateTimer > 0) {
			fireRateTimer -= Time.deltaTime;
		}
	}

	bool canWeFireWeapon()
	{
		if (this.gameObject.tag != "Player") {
			if (reloading == true) {
				return false;
			}

			if (currentWeapon==null|| currentWeapon == null &&  fireRateTimer <= 0) {
				if (meleeDone == true) {
					return true;
				} else {
					return false;
				}
			} 
			else if(currentWeapon.melee == true && fireRateTimer <= 0){
				if (meleeDone == true) {
					return true;
				} else {
					return false;
				}
			}
			else {
				if (reloading==false && fireRateTimer <= 0) {
					return true;
				} else {
					return false;
				}
			}
		} else {

			if (reloading == true) {
				return false;
			}

			if (currentWeapon == null || currentWeapon.melee == true) {
				if (meleeDone == true) {
					return true;
				} else {
					return false;
				}
			} else {
				if (reloading==false && fireRateTimer <= 0) {
					return true;
				} else {
					return false;
				}
			}
		}
	}

	public bool droppingWeapon = false,throwingWeapon=false;

	bool areWeDrawingWeapon()
	{
		if (currentWeapon == null) {
			return false;
		} else {
			return false;
		}
	}

	public void dropWeapon()
	{
		if (currentWeapon == null || droppingWeapon==true || throwingWeapon==true) {
			return;
		}
		reloading=false;
		droppingWeapon = true;
	}

	public void dropWeaponCheck()
	{
		if (currentWeapon == null) {
			droppingWeapon = false;
			return;
		} else {
			currentWeapon.inUse = false;

			Inventory.playerInventory.dropItem (currentWeapon);
			currentWeapon = null;
			ItemMoniter.me.refreshItems ();
			droppingWeapon = false;
		}
	}

	public void throwWeaponCheck()
	{
			WeaponStore.me.createWeaponProjectile (currentWeapon, throwWeaponSpawn.position,this.transform.rotation);
			Item i = currentWeapon;
			currentWeapon.unequipItem ();
			myInv.removeItemWithoutDrop (i);
			Destroy (i.gameObject);
			throwingWeapon = false;
			setWeapon (null);

	}
	//problem seems to be related to the gear slots still being occupied when items are dropped
	public void throwWeapon()
	{
		if (currentWeapon == null|| droppingWeapon==true || throwingWeapon==true) {
			return;
		}
	
		throwingWeapon = true;


	}

	public void punchNoise()
	{
		if (this.gameObject.tag == "Player") {
			//NPCController[] npcs = FindObjectsOfType<NPCController> (); //need to store this in some place + make it so that npcs can do it too somehow?
			foreach (GameObject g in NPCManager.me.npcsInWorld) {
				if (g == null) {
					continue;
				}

				NPCController npc = g.GetComponent<NPCController> ();
				npc.setHearedGunshot (this.transform.position, 5.0f);
			}
		}
	}

	public void bulletFire()
	{
		if (this.gameObject.tag == "Player") {
			//NPCController[] npcs = FindObjectsOfType<NPCController> (); //need to store this in some place + make it so that npcs can do it too somehow?
			foreach (GameObject g in NPCManager.me.npcsInWorld) {
				if (g == null) {
					continue;
				}

				NPCController npc = g.GetComponent<NPCController> ();
				npc.setHearedGunshot (this.transform.position, currentWeapon.noiseRange);
			}
			PoliceController.me.setNoiseHeard (this.transform.position,currentWeapon.noiseRange);
		}
	}

	bool isTargetFacingAway(GameObject target)
	{
		NPCController npc = target.GetComponent<NPCController> ();
		if (npc == null) {
			return false;
		} else {
			if (npc.detect.isTargetInFrontOfUs (this.gameObject) == true) {
				return false;
			} else {
				return true;
			}
		}
	}
}
