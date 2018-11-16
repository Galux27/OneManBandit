using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction_DropBody : PlayerAction {
		bool setRot=false;
		SpriteRenderer sr;
		float timeTillDoAgain = 1.0f;
		bool droppedCorpse=false;
		public override void doAction()
		{
			illigal = true;		
		//this.gameObject.GetComponent<Rigidbody2D> ().WakeUp ();
			PlayerAction.currentAction = this;
			onComplete ();
			//this.gameObject.transform.rotation = Quaternion.Euler (0, 0, 0);

			//if (setRot == false) {
			//this.transform.position = Vector3.zero;
			//this.gameObject.transform.position = CommonObjectsStore.player.transform.position - Vector3.up;

			//	setRot = true;
			//}
		}



		public override bool canDo ()
		{
			if (sr == null) {
				sr = this.gameObject.GetComponent<SpriteRenderer> ();
			}

			//Debug.Log ("Player action drop body root tag is " + this.gameObject.transform.root.tag);
			if (this.gameObject.transform.root.tag == "Player") {
				return true;
			}

			if (PlayerAction.currentAction == null) {

			}
			else if (PlayerAction.currentAction.getType()== "Move Body") {
				return true;
			}
			return false;
		}

		public override void onComplete()
		{
		Destroy(this.gameObject.GetComponent<NPCBeingDragged>());

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
			return "Drop Body";
		}

		public override string getDescription()
		{
			return "Drop the currently held body";
		}
	}
