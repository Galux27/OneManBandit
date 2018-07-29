using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilianAction : MonoBehaviour {

	/// <summary>
	/// This is a class to give civilians something to do when roaming about the map, they will go to a point, wait for X seconds then find something else to do.
	/// Also has options for sitting & going for a specific action after doing this one e.g. using a toilet then washing hands 
	/// </summary>

	public float timeActionTakes,resetValue;
	public string animationName;
	public Transform positionForAction;
	public Vector3 positionToResetNPC = Vector3.zero;
	public bool doWeHaveNPCDoingAnimation = false;
	public civilianActionType myType;

	public string actionName="";

	public GameObject doingAction;
	public PersonAnimationController pac;


	public bool sitting = false,disableNearestNode=true,actionAvailable=true;

	void Awake(){
		resetValue = timeActionTakes;
		if (positionForAction == null) {
			positionForAction = this.transform;
		}

		if (positionForAction.gameObject.GetComponent<SpriteRenderer> () != null) {
			positionForAction.gameObject.GetComponent<SpriteRenderer> ().enabled = false;
		}


	}

	// Use this for initialization
	void Start () {
		if (disableNearestNode == true) {
			WorldTile wt = WorldBuilder.me.findNearestWorldTile (this.transform.position);
			if (wt == null) {

			} else {
			//	wt.walkable = false;
			//	wt.gameObject.GetComponent<SpriteRenderer> ().color = Color.black;
			}
		}
	}
	
	// Update is called once per frame
	public void OnUpdate () {
		if (doWeHaveNPCDoingAnimation == true) {
			doAction ();
		}
	}

	void doAction()
	{
		//if (pac.areWePlayingAnimation (pac.ID, animationName) == false) {
		//	pac.playAnimation (animationName, true);
		//}

		doingAction.transform.position = positionForAction.position;
		doingAction.transform.rotation = positionForAction.rotation;

		timeActionTakes -= Time.deltaTime;

		if (timeActionTakes <= 0) {
			stopAction ();
		}


		if (sitting == true) {
			if (doingAction == null) {
			} else {

				//doingAction.GetComponentInChildren<Legs> ().myState = legState.sitting;
			}
		}
	}

	public void setNPCAction(GameObject toDoAction)
	{
		timeActionTakes = resetValue;
		positionToResetNPC = toDoAction.transform.position;
		doingAction = toDoAction;
		//pac = doingAction.GetComponent<PersonAnimationController> ();
		doWeHaveNPCDoingAnimation = true;

	
	}

	public void stopAction()
	{
		if (doingAction == null) {
		} else {
			doingAction.transform.position = positionToResetNPC;

		}
		if (pac == null) {

		} else {

			//if (pac.areWePlayingAnimation (pac.ID, animationName) == true) {
			//	pac.animationsToPlay.Clear ();

			//	pac.forceFinishCurrentAnim ();
			//	pac.playing = null;
			//	pac.playAnimation ("Unarmed",true);

			//}
		}
		doingAction = null;
		doWeHaveNPCDoingAnimation = false;
	}
}

public enum civilianActionType{
	general,
	worker
}
