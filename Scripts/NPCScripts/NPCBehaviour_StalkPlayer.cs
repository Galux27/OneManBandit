using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour_StalkPlayer : NPCBehaviour {

	public bool stalkingPlayer=true,hasPathToCivilianAction=false;
	Vector3 playerPoint;
	public CivilianAction actionToDo;

	public override void Initialise ()
	{
		myType = behaviourType.investigatePlayer;
		myController = this.gameObject.GetComponent<NPCController> ();
		myController.pf.getPath (this.gameObject, CommonObjectsStore.player);
		playerPoint = CommonObjectsStore.player.transform.position;
		isInitialised = true;
	}

	public override void OnUpdate ()
	{
		if (isInitialised == false) {
			Initialise ();
		}

		if (stalkingPlayer == true) {
			if (shouldWeFollowPlayer () == true) {
				move ();

				if (refreshPathToPlayer ()) {
					myController.pf.getPath (this.gameObject, CommonObjectsStore.player);

				}

			} else {
				if (areWeTooCloseToPlayer ()) {
					stalkingPlayer = false;
				}
			}
		} else {
			if (myController.npcB.alarmed == false) {
				if (hasPathToCivilianAction == false) {
					actionToDo = LevelController.me.getCivilianAction ();
					if (myController.pf.target == null || myController.pf.target != actionToDo.positionForAction.gameObject) {
						myController.pf.getPath (this.gameObject, actionToDo.positionForAction.gameObject);
					}
					hasPathToCivilianAction = true;
				}
				move ();
				avoidPlayerTimer -= Time.deltaTime;
				if (goBackToStalkingPlayer () == true) {
					stalkingPlayer = true;
					hasPathToCivilianAction = false;
				}
			} else {
				myController.npcB.doing = whatAiIsDoing.attacking;
			}
		}
	}

	public override void OnComplete ()
	{
		
	}

	void move()
	{	
		if (stalkingPlayer == true) {
			if (myController.detect.lineOfSightToTarget (CommonObjectsStore.player) == false) {
				myController.pmc.rotateToFacePosition (myController.pf.getCurrentPoint ());
			} else {
				myController.pmc.rotateToFacePosition (CommonObjectsStore.player.transform.position);

			}
		} else {
			myController.pmc.rotateToFacePosition (myController.pf.getCurrentPoint ());

		}

		myController.pmc.moveToDirection (myController.pf.getCurrentPoint());


	}

	bool shouldWeFollowPlayer()
	{
		if (myController.detect.lineOfSightToTarget (CommonObjectsStore.player) == true) {
			if (Vector2.Distance (this.transform.position, CommonObjectsStore.player.transform.position) < 8.0f) {
				//Debug.LogError ("NOT FOLLOWING PLAYER");
				return false;
			} else {
				//Debug.LogError ("FOLLOWING PLAYER");

				return true;
			}
		} else {
			if (myController.detect.fov.visibleTargts.Contains (CommonObjectsStore.player.transform)) {
				return false;
			} else {
				if (Vector2.Distance (this.transform.position, CommonObjectsStore.player.transform.position) > 8.0f) {
					//Debug.LogError ("FOLLOWING PLAYER");

					return true;
				}
			}
		}
		return true;
	}

	bool areWeTooCloseToPlayer()
	{
		if (PlayerFieldOfView.me.visibleTargts.Contains (this.gameObject.transform)) {
			//Debug.LogError ("TOO CLOSE TO PLAYER");

			return true;
		} else {
			return false;
		}
	}

	bool refreshPathToPlayer()
	{
		if (Vector2.Distance (CommonObjectsStore.player.transform.position, playerPoint) > 5.0f) {
			//Debug.LogError ("REFRESHING PATH TO PLAYER");

			playerPoint = CommonObjectsStore.player.transform.position;
			return true;
		} else {
			return false;
		}
	}

	float avoidPlayerTimer = 10.0f;
	bool goBackToStalkingPlayer(){
		if (avoidPlayerTimer >= 0) {
			return false;
		} else {
			if (areWeTooCloseToPlayer () == false) {
				avoidPlayerTimer = 10.0f;
				return true;
			}
			return false;
		}
	}
}
