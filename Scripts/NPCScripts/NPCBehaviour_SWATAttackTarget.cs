using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour_SWATAttackTarget : NPCBehaviour {

	public GameObject target;
	public override void Initialise ()
	{
		myType = behaviourType.attackTarget;
		myController = this.gameObject.GetComponent<NPCController> ();

		pointNearTarget = FindPointsAroundPlayer.me.getRandomPoint ();
	
		if (myController.pf.target != pointNearTarget) {
			myController.pf.getPath (this.gameObject, pointNearTarget);
		}

		if (myController.pwc.currentWeapon == null) {
			if (myController.inv.doWeHaveAWeaponWeCanUse () == true) {
				Weapon w = myController.inv.getWeaponWeCanUse ();
				w.equipItem ();
			}
		}

		if (target == null) {
			target = PoliceController.me.swatTarget;
		}

		//setAllSwatToTarget ();
		isInitialised = true;
	}

	public override void OnUpdate ()
	{
		if (isInitialised == false) {
			Initialise ();
		}

		if (target == null) {

		} else {
			if (myController.detect.fov.visibleTargts.Contains(target.transform)) {
				myController.memory.noiseToInvestigate = target.transform.position;
				if (myController.pwc.currentWeapon == null) {

					////////Debug.Log ("ACTUALLY TRYING TO ATTACK WITH BARE HANDS");
					//rotateToTarget ();
					meleeMoveToTarget();
					if (Vector3.Distance (this.transform.position, target.transform.position)<1.0f) {
						fireWeapon();
					}
				} else {
					if (myController.pwc.currentWeapon.melee == false) {
						rotateToTarget ();
						if (myController.detect.fov.visibleTargts.Contains(target.transform)) {
							myController.pwc.aimDownSight = true;
							//if (doWeHaveAShot () == true) {
							fireWeapon();
							//} else {
								//moveToTarget ();

								//strafe left or right whilst facing npc
								//myController.pmc.moveToDirection(this.transform.right);
							//}
						} 
					} else {
						rotateToTarget ();
						meleeMoveToTarget();
						if (Vector3.Distance (this.transform.position, target.transform.position)<1.0f) {
							fireWeapon();
						}
					}
				}
			} else {
				//myController.pwc.aimDownSight = false;
				moveToTarget ();
			}
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

	bool doWeHaveAShot()
	{

		/*foreach (GameObject npc in NPCManager.me.npcsInWorld) {
			if (myController.detect.getDotProduct(target) >= 4.5f) {
				return true;
			}
		}*/

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
		RaycastHit2D ray = Physics2D.Raycast (origin, transform.up,12.0f);
		//Debug.DrawRay (origin, transform.up*12.0f,Color.green);

		if (ray.collider == null) {
			return false;
		} else {
//			////////Debug.Log (ray.collider.gameObject);
			if (ray.collider.gameObject == target || ray.collider.gameObject.tag=="Projectile") {
				return true;
			} else {
				return false;
			}
		}


	}

	void rotateToTarget()
	{
		myController.pmc.rotateToFacePosition (target.transform.position);

	}

	void meleeMoveToTarget()
	{
		myController.pf.target = target;

		if (myController.detect.detect == true) {
			myController.pmc.rotateToFacePosition (target.transform.position);
			if (Vector3.Distance (this.transform.position, target.transform.position) > 0.5f) {
				myController.pmc.moveToDirection (target.transform.position);
			}
		} else {
			if (myController.pf.followPath == false) {
				myController.pf.getPath (this.gameObject, target);
			}

			myController.pmc.rotateToFacePosition (myController.pf.getCurrentPoint());
			myController.pmc.moveToDirection (myController.pf.getCurrentPoint());
		}
	}

	void moveToTarget()
	{
		myController.pf.target = target;
		if (myController.detect.detect == true) {
			myController.pmc.rotateToFacePosition (target.transform.position);
			if (Vector3.Distance (this.transform.position, target.transform.position) > 3.0f) {
				myController.pmc.moveToDirection (target.transform.position);
			}
		} else {
			if (myController.pf.followPath == false) {
				myController.pf.getPath (this.gameObject, target);
			}

			myController.pmc.rotateToFacePosition (myController.pf.getCurrentPoint());
			myController.pmc.moveToDirection (myController.pf.getCurrentPoint());

		}
	}

	void setAllSwatToTarget()
	{
		NPCController[] allControllers = FindObjectsOfType<NPCController> ();
		foreach (NPCController npc in allControllers) {
			if (npc.npcB.myType == AIType.swat) {
				if (npc != myController) {
					if (npc.currentBehaviour.myType != behaviourType.attackTarget) {
						Destroy (npc.currentBehaviour);
						NPCBehaviour nb = npc.gameObject.AddComponent<NPCBehaviour_SWATAttackTarget> ();
						npc.currentBehaviour = nb;
						nb.passInGameobject (myController.memory.objectThatMadeMeSuspisious);
					}
				}
			}
		}
		radioMessageOnStart ();
	}

	public override void OnComplete ()
	{

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
			} else if (myController.npcB.myType == AIType.swat) {
				h = radioHackBand.swat;
			}


			PhoneTab_RadioHack.me.setNewText ("SWAT Team engaging target. ",h);


		}
	}
	public bool canWeStillSeeTarget()
	{
		return myController.detect.fov.visibleTargts.Contains (PoliceController.me.swatTarget.transform);
	}

}