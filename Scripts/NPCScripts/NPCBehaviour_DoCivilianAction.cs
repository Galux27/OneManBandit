using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour_DoCivilianAction : NPCBehaviour {
	public CivilianAction actionToDo;
	public bool needNewPath = false,getActionOfSameType=false;
	public override void Initialise ()
	{
		myType = behaviourType.civilianAction;
		myController = this.gameObject.GetComponent<NPCController> ();

		if (actionToDo == null) {
			actionToDo = LevelController.me.getCivilianAction ();
		}
		myController.pf.getPath (this.gameObject, actionToDo.positionForAction.gameObject);

		isInitialised = true;
	}

	public override void OnUpdate ()
	{
		if (isInitialised == false) {
			Initialise ();
		}


		if (needNewPath==true || actionToDo.actionAvailable == false) {
			if (getActionOfSameType == false) {
				actionToDo = LevelController.me.getCivilianAction ();
			} else {
				actionToDo = LevelController.me.getActionOfSameType (actionToDo.actionName);
			}
			//if (myController.pf.target != actionToDo.positionForAction.gameObject && myController.pf.waitingForPath==false) {
			//Debug.Log(this.gameObject.name + " is wanting to search for a new path to civilian action!");
			if (myController.pf.target == null || myController.pf.target != actionToDo.positionForAction.gameObject) {
				myController.pf.getPath (this.gameObject, actionToDo.positionForAction.gameObject);
			}
			needNewPath = false;	
			//}
		}



		posImGoingTo = actionToDo.positionForAction.transform.position.ToString ();
		if (areWeAtPosition () == false) {
			if (myController.pf.askedForNewPath == false) {
				moveToPosition ();
			}


		} else {

			if (actionToDo.doingAction == null) {
				actionToDo.setNPCAction (this.gameObject);
				myController.pf.target = null;
				needNewPath = false;


			} else {
				if (actionToDo.doingAction == this.gameObject) {
					if (actionToDo.doWeHaveNPCDoingAnimation == true && actionToDo.doingAction == this.gameObject) {
						//			////////Debug.Log ("Calling update on civilian action " + actionToDo.gameObject.ToString ());
						myController.pf.target = null;

						actionToDo.OnUpdate ();

						if (actionToDo.timeActionTakes <= 0) {
							//myController.pf.clearPath ();
							Destroy (this);//done with action
						}
					}
				} else {
					getActionOfSameType = true;
					needNewPath = true;

				}
			}
			//////////Debug.Log ("Calling update on civilian action in the wrong place " + actionToDo.gameObject.ToString ());

		}



	}
	public string posImGoingTo = "";
	bool areWeAtPosition()
	{
		
		if (Vector2.Distance (this.transform.position, actionToDo.positionForAction.position) > 1.5f) {
			return false;
		} else {
			return true;
		}
	}

	void moveToPosition(){
		myController.pmc.rotateToFacePosition (myController.pf.getCurrentPoint());
		if (areWeAtPosition()==false) {
			myController.pmc.moveToDirection (myController.pf.getCurrentPoint());
		}
	}

	public override void OnComplete ()
	{

	}

	void OnDestroy()
	{
		if (actionToDo == null) {

		} else {
			actionToDo.stopAction ();

		}

	}

}
