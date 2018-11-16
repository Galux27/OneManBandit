using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeenPlayerUI : MonoBehaviour {
	NPCController npc;
	bool seen=false;
	void Awake()
	{
		npc = this.GetComponentInParent<NPCController> ();
		this.GetComponent<SpriteRenderer> ().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (shouldWeDisable () == false) {
			if (npc.memory.objectThatMadeMeSuspisious == CommonObjectsStore.player || npc.npcB.myType == AIType.swat && LevelController.me.suspects.Contains (CommonObjectsStore.player) || npc.npcB.myType == AIType.swat && LevelController.me.suspects.Contains (CommonObjectsStore.player)) {
				if (seen == false) {
					this.GetComponent<SpriteRenderer> ().enabled = true;
					seen = true;
				}
			}
		} else {
			this.GetComponent<SpriteRenderer> ().enabled = false;
			seen = false;
			//this.gameObject.SetActive (false);
		}
	}

	bool shouldWeDisable()
	{
		if (npc.myHealth.healthValue <= 0 || npc.knockedDown==true) {
			return true;
		}
		return false;
	}
}
