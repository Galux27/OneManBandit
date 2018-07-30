using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for actions that the player could do in the world.
/// </summary>
public class PlayerAction : MonoBehaviour {


	void Awake()
	{
		
	}
	//to be a class that allows the player to interact with stuff e.g. hide corpses, set up explosives on a gate etc...
	// Use this for initialization
	public static PlayerAction currentAction;
	public bool doingAction=false;
	public bool illigal=false;
	void Start () {
		
	}


	
	// Update is called once per frame
	void Update () {
		if (currentAction == this && doingAction == true) {
			doAction ();
		}
	}

	void inputDetect()
	{
		if (doingAction == false) {
			if (canDo () && Input.GetKeyDown (KeyCode.E) && currentAction==null) {
				doingAction = true;
				currentAction = this;
			}
		} else {
			if (Input.GetKeyDown (KeyCode.E) && currentAction==this) {
				doingAction = false;
				currentAction = null;
				onComplete ();
			}
		}
	}

	public virtual bool canDo()
	{
		if (Vector3.Distance (CommonObjectsStore.player.transform.position, this.transform.position) < 2) {
			return true;
		} else {
			return false;
		}
	}

	public virtual void doAction()
	{

	}

	public virtual void onComplete()
	{

	}

	public virtual float getMoveModForAction()
	{
		return 0.0f;
	}

	public virtual float getRotationModForAction()
	{
		return 0.0f;
	}

	public virtual string getType()
	{
		return "Default";
	}

	public virtual string getDescription()
	{
		return "Action Base";
	}

	void OnDestroy()
	{
		NewPlayerActionUI.me.allActions.Remove (this);
		NewPlayerActionUI.me.myActions.Remove (this);

	}

	void OnEnable()
	{
		NewPlayerActionUI.AddAction (this);
	}
}
