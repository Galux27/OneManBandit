using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour {

	/// <summary>
	/// Class that stores all the NPCs and corpses in the world. Also calls the NPC conrollers update function to run the AI, done here rather than in update to see if the performance increase from having X ai run a frame would provide a performance increase. 
	/// </summary>

	public static NPCManager me;
	public List<GameObject> npcsInWorld=new List<GameObject>(), corpsesInWorld=new List<GameObject>();
	public List<NPCController> npcControllers;
	float refreshTimer = 1.0f;
	public bool startedUpdating = false;
	public int maxUpdatePerFrame = 5;

	public List<GameObject> swat,patrolCops;
	void Awake()
	{
		if (me == null) {
			me = this;
			npcControllers = new List<NPCController> ();
		}

		if (me != this) {
			Destroy (this);
		}

		foreach (GameObject g in swat) {
			g.SetActive (false);
		}

		foreach (GameObject g in patrolCops) {
			g.SetActive (false);
		}
	}


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (maxUpdatePerFrame > npcControllers.Count) {
			maxUpdatePerFrame = npcControllers.Count - 1;
		}

		//if (startedUpdating == false) {
		//	StartCoroutine ("npcUpdate");
		//	startedUpdating = true;
		//}
		decideNPCsToUpdateThisFrame();
		//refreshCountdown();
	}


	public int numberOfUpdatesThisFrame = 0;
	public int indexToStartAt = 0;
	public void decideNPCsToUpdateThisFrame()
	{
		for (int x = indexToStartAt; x < npcControllers.Count; x++) {



			NPCController npc = npcControllers [x];
			if (x >= npcControllers.Count-1) {
				indexToStartAt = 0;
			}
			if (numberOfUpdatesThisFrame < maxUpdatePerFrame) {
				npc.npcB.OnUpdate ();
				//Debug.Log ("Calling update for " + npc.gameObject.name);
				numberOfUpdatesThisFrame++;
			} else {
				if (x >= npcControllers.Count - 1) {
					indexToStartAt = 0;
				} else {
					indexToStartAt = x;

				}
				numberOfUpdatesThisFrame = 0;
				break;
			}
		}



		/*foreach (NPCController npc in npcControllers) {
			if (numberOfUpdatesThisFrame < 5) {
				npc.npcB.OnUpdate ();
				//Debug.Log ("Calling update for " + npc.gameObject.name);
				numberOfUpdatesThisFrame++;
			} else {
				break;
			}
		}*/
	}




}
