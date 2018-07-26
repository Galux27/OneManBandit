using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour : MonoBehaviour {
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
	evacuate
}
