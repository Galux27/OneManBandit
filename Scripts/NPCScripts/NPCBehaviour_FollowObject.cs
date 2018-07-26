using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour_FollowObject : NPCBehaviour {
	public GameObject target;

	public override void Initialise ()
	{
		myType = behaviourType.followTarget;
		myController = this.gameObject.GetComponent<NPCController> ();
		isInitialised = true;
	}

	public override void OnUpdate ()
	{
		if (isInitialised == false) {
			Initialise ();
		}
		myController.pmc.rotateToFacePosition (target.transform.position);
		if (Vector3.Distance (this.transform.position, target.transform.position)>3.0f) {
			myController.pmc.moveToDirection (target.transform.position);
		}
	}

	public override void OnComplete ()
	{
		
	}

	public override void passInGameobject (GameObject passIn)
	{
		target = passIn;
	}
}
