using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMemory : MonoBehaviour {

	/// <summary>
	/// Class that stores information that the NPC has seen e.g. have we seen a corpse, hostage, armed player, whats my patrol route, have I been attacked etc... 
	/// </summary>


	public bool raiseAlarm=false,suspisious=false;
	public GameObject objectThatMadeMeSuspisious;
	//public bool objectWasTraspassing = false;

	public Vector3 guardPos;
	public Quaternion guardRot;

	//these variables are just things to remember e.g. what my patrol route was
	public PatrolRoute myRoute;
	public Vector3 noiseToInvestigate;
	public List<GameObject> peopleThatHaveAttackedMe=new List<GameObject>();

	public bool beenAttacked,seenHostage,seenCorpse,seenArmedSuspect,seenSuspect;


}

