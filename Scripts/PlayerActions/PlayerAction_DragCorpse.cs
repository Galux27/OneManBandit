using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction_DragCorpse : PlayerAction {
	bool setRot=false;
	SpriteRenderer sr;
	float timeTillDoAgain = 1.0f;
	bool droppedCorpse=false;

	void Awake()
	{
		if (this.GetComponent<PlayerAction_DropBody> () == false) {
			this.gameObject.AddComponent<PlayerAction_DropBody> ();
		}
	}

	public override void doAction()
	{
		illigal = true;		
		if (this.gameObject.GetComponent<NPCBeingDragged> () == false) {
				
			this.gameObject.AddComponent<NPCBeingDragged> ();
		}
		//this.transform.parent = CommonObjectsStore.player.transform;
		//this.transform.localPosition = Vector3.zero;
		sr.sortingOrder = 1;
	//	if (this.transform.parent != null) {
			//if (Input.GetKeyDown (KeyCode.E)) {
			//	onComplete ();
			//	droppedCorpse = true;
			//}
//		}
		//Destroy(this.gameObject.GetComponent<Rigidbody2D>());
		//this.gameObject.GetComponent<Rigidbody2D> ().Sleep();
		PlayerAction.currentAction = this;

		//this.gameObject.transform.rotation = Quaternion.Euler (0, 0, 0);

		//if (setRot == false) {
			//this.transform.position = Vector3.zero;
			//this.gameObject.transform.position = CommonObjectsStore.player.transform.position - Vector3.up;

		//	setRot = true;
		//}
	}

	void Update()
	{
		if (droppedCorpse == true) {
			timeTillDoAgain -= Time.deltaTime;
			if (timeTillDoAgain <= 0) {
				droppedCorpse = false;
				timeTillDoAgain = 1.0f;
			}
		}
	}

	public override bool canDo ()
	{
		if (sr == null) {
			sr = this.gameObject.GetComponent<SpriteRenderer> ();
		}
		if (this.gameObject.tag == "Dead/Knocked" && this.transform.parent==null && droppedCorpse==false && currentAction==null) {
			return true;
		}
		return false;
	}

	public override void onComplete()
	{
		this.transform.parent = null;
		PlayerAction.currentAction = null;
	}

	public override float getMoveModForAction ()
	{
		return -0.65f;
	}

	public override float getRotationModForAction ()
	{
		return -0.65f;
	}

	public override string getType()
	{
		return "Move Body";
	}

	public override string getDescription()
	{
		return "Move this body, will slow movement.";
	}
}
