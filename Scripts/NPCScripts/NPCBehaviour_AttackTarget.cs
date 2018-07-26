using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour_AttackTarget : NPCBehaviour {

	public GameObject target;
	public float loseTargetTimer = 5.0f;
	bool lostTarget = false;
	GameObject marker = null;
	public override void Initialise ()
	{
		myType = behaviourType.attackTarget;
		myController = this.gameObject.GetComponent<NPCController> ();

		if (myController.pwc.currentWeapon == null) {
			if (myController.inv.doWeHaveAWeaponWeCanUse () == true) {
				Weapon w = myController.inv.getWeaponWeCanUse ();
				w.equipItem ();
			}
		}
		//if (target == null) {
		target = myController.memory.objectThatMadeMeSuspisious;
		//}
		radioMessageOnStart ();
		isInitialised = true;
	}

	public override void OnUpdate ()
	{
		if (isInitialised == false) {
			Initialise ();
		}



		if (target == null) {

		} else {
			if (target.tag == "NPC") {
				target = CommonObjectsStore.player;
			}

			if (lostTarget == false) {
				if (canWeSeeTarget ()) {
					myController.memory.noiseToInvestigate = target.transform.position;
					if (myController.pwc.currentWeapon == null) {
						if (PlayerAction.currentAction == null || PlayerAction.currentAction.getType () != "Take Hostage") {

							meleeMoveToTarget ();
							if (Vector3.Distance (this.transform.position, target.transform.position) < 1.0f) {
								fireWeapon ();
							}
						} else {


							meleeMoveToTarget ();
							if (Vector3.Distance (this.transform.position, target.transform.position) < 1.0f) {
								fireWeapon ();
							}
				
						}

					} else {
						if (myController.pwc.currentWeapon.melee == false) {
							rotateToTarget ();
							if (myController.detect.fov.visibleTargts.Contains (target.transform)) {
								myController.pwc.aimDownSight = true;
								fireWeapon ();							
							} 
						} else {
							if (PlayerAction.currentAction == null) {
								rotateToTarget ();
								meleeMoveToTarget ();
								if (Vector3.Distance (this.transform.position, target.transform.position) < 1.0f) {
									fireWeapon ();
								}
							} else if (PlayerAction.currentAction.getType () == "Take Hostage") {
								if (canWeShoot () == true) {
									rotateToTarget ();
									meleeMoveToTarget ();
									if (Vector3.Distance (this.transform.position, target.transform.position) < 1.0f) {
										fireWeapon ();
									}
								} else {
									moveAroundTarget ();
								}
							} else {
								rotateToTarget ();
								meleeMoveToTarget ();
								if (Vector3.Distance (this.transform.position, target.transform.position) < 1.0f) {
									fireWeapon ();
								}
							}
						}
					}
				} else {
					loseTargetTimer -= Time.deltaTime;
					if (loseTargetTimer <= 0) {
						lostTarget = true;
					}
					moveToTarget ();
				}
			} else {
				if (marker == null) {
					marker = new GameObject ();
					marker.transform.position = myController.memory.noiseToInvestigate;
					marker.name = "Attack Search Marker";
				}

				if (canWeSeeTarget ()) {
					lostTarget = false;
				}

				if (myController.pf.target != marker) {
					myController.pf.getPath (this.gameObject, marker);
				}

				if (Vector3.Distance (this.transform.position, marker.transform.position) > 1.0f) {
					myController.pmc.moveToDirection (myController.pf.getCurrentPoint ());
					myController.pmc.rotateToFacePosition (target.transform.position);

				} else {
					OnComplete ();
				}
			}
		}

			
	}

	void OnDestroy()
	{
		if (marker == null) {

		} else {
			Destroy (marker);
		}
	}

	bool canWeSeeTarget()
	{
		if (target == null) {
			return false;
		}

		if (myController.detect.fov.visibleTargts.Contains (target.transform)) {
			loseTargetTimer = 5.0f;
			return true;
		} else {
			return false;
		}
	}

	GameObject pointNearTarget;
	void moveAroundTarget()
	{
		if (pointNearTarget == null) {
			pointNearTarget = FindPointsAroundPlayer.me.getRandomPoint ();
		} else {
			if (myController.pf.target != pointNearTarget) {
				myController.pf.getPath (this.gameObject, pointNearTarget);
			}
			if (Vector3.Distance (this.transform.position, pointNearTarget.transform.transform.position) > 1.0f) {
				myController.pmc.moveToDirection (myController.pf.getCurrentPoint ());
			}
			myController.pmc.rotateToFacePosition (target.transform.position);

			if (Vector3.Distance (target.transform.position, pointNearTarget.transform.position) > 5.0f) {
				pointNearTarget = FindPointsAroundPlayer.me.getRandomPoint ();
			}
		}
	}

	public void fireWeapon()
	{
		if (target.tag == "Player") {
			if (PlayerAction.currentAction == null) {
				myController.pwc.fireWeapon ();

			} else {
				if (PlayerAction.currentAction.getType () == "Take Hostage") {
					if (canWeShoot () == true) {
						myController.pwc.fireWeapon ();
					} else {
						moveAroundTarget ();
					}
				} else {
					myController.pwc.fireWeapon ();

				}
			}
		} else {
			myController.pwc.fireWeapon ();

		}
	}

	bool canWeShoot()
	{
		Vector3 heading = target.transform.position - this.transform.position;
		float dot = Vector3.Dot (heading, target.transform.up);

		if (dot > 0) {
			return true;
		}
		return false;
	}
		

	void rotateToTarget()
	{
		myController.pmc.rotateToFacePosition (target.transform.position);

	}

	void meleeMoveToTarget()
	{
		if (myController.pf.target != target) {
			myController.pf.target = target;
			myController.pf.getPath (this.gameObject, target);

		}

		if (myController.detect.fov.visibleTargts.Contains(target.transform) == true || myController.detect.lineOfSightToTarget(target) && Vector2.Distance(this.transform.position,target.transform.position)<7) {
			//////Debug.Log ("Melee move 1");
				myController.pf.currentPath.Clear ();

			myController.pmc.rotateToFacePosition (target.transform.position);
			if (Vector2.Distance (this.transform.position, target.transform.position) > 0.75f) {
				myController.pmc.moveToDirection (target.transform.position);
			}
		} else {
			if (myController.pf.followPath == false || myController.pf.currentPath == null || myController.pf.currentPath.Count==0) {
				myController.pf.getPath (this.gameObject, target);
			}
			//////Debug.Log ("Melee move 2");

			myController.pmc.rotateToFacePosition (myController.pf.getCurrentPoint());
			myController.pmc.moveToDirection (myController.pf.getCurrentPoint());
		}
	}

	void moveToTarget()
	{
		if (myController.pf.target != target) {
			myController.pf.target = target;
			myController.pf.getPath (this.gameObject, target);

		}
		if (myController.detect.fov.visibleTargts.Contains(target.transform) == true|| myController.detect.lineOfSightToTarget(target)&& Vector2.Distance(this.transform.position,target.transform.position)<7) {
			myController.pmc.rotateToFacePosition (target.transform.position);
			myController.pf.currentPath.Clear ();
			if (Vector3.Distance (this.transform.position, target.transform.position) > 3.0f) {
				myController.pmc.moveToDirection (target.transform.position);
			}
		} else {
			if (myController.pf.followPath == false || myController.pf.currentPath == null || myController.pf.currentPath.Count==0) {
				myController.pf.getPath (this.gameObject, target);
			}

			myController.pmc.rotateToFacePosition (myController.pf.getCurrentPoint());
			myController.pmc.moveToDirection (myController.pf.getCurrentPoint());

		}
	}
	bool doWeHaveAShot()
	{

		Vector3 origin = Vector3.zero;
		if (myController.pwc.currentWeapon == null) {
			origin = this.transform.position;
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

		Vector3 heading = transform.forward - origin;
		RaycastHit2D ray = Physics2D.Raycast (origin, transform.up,6.0f);
		Debug.DrawRay (origin, transform.up*6.0f,Color.green);

		if (ray.collider == null) {
			return false;
		} else {
			if (ray.collider.gameObject == CommonObjectsStore.player) {
				return true;
			} else {
				return false;
			}
		}


	}
	public override void OnComplete ()
	{
		myController.npcB.doing = whatAiIsDoing.raisingAlarm;
		Destroy (this);
	}

	public override void passInGameobject (GameObject passIn)
	{
		target = passIn;
	}

	public override void radioMessageOnStart ()
	{
		radioHackBand h = radioHackBand.buisness;

		if (myController.npcB.myType == AIType.civilian) {

		} else {
			if (myController.npcB.myType == AIType.cop) {
				h = radioHackBand.cop;
				PhoneTab_RadioHack.me.setNewText ("Suspect sighted, taking him down.",h);

			} else if (myController.npcB.myType == AIType.swat) {
				h = radioHackBand.swat;
				PhoneTab_RadioHack.me.setNewText ("Alpha team engaging suspect.",h);

			} else {
				PhoneTab_RadioHack.me.setNewText ("Taking down the intruder.",h);

			}

		}
	}
}
