using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour : MonoBehaviour {

	/// <summary>
	/// This is the base class for all NPC Behaviours, to get NPCs to do something the child class will just overwrite one or more of these methods to get the required functionality.
	/// </summary>

	public bool isInitialised;
	public NPCController myController;
	public behaviourType myType;
	public virtual void Initialise()
	{

	}

	public virtual void OnUpdate()
	{
		//do some shit
	}

	public virtual void OnComplete()
	{

	}

	public virtual void passInGameobject(GameObject passIn)
	{

	}

	public virtual void passInMultipleGameobjects(List<GameObject> toPassIn)
	{

	}

	public virtual void radioMessageOnStart()
	{

	}

	public virtual void radioMessageOnFinish()
	{

	}
}
/// <summary>
/// The type of the NPC behaviour, used when working out whether the behaviour should be changed. 
/// </summary>
public enum behaviourType{
	none,
	getWeapon,
	followTarget,
	attackTarget,
	getAmmo,
	patrol,
	investigate,
	searchPerson,
	guardLoc,
	raiseAlarm,
	searchRooms,
	arrestTarget,
	guardCorpse,
	updateSuspects,
	traspassing,
	civilianAction,
	exitLevel,
	freeHostage,
	guardEntrance,
	formUp,
	evacuate,
	shopkeeper,
	investigatePlayer
}
