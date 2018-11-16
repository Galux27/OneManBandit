using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractIcon : MonoBehaviour {
	bool doWeDraw=false;
	public bool isShopkeeper=false;
	public GameObject toFollow;
	NPCController npc;
	// Update is called once per frame
	void Update () {
		doWeDraw = draw ();
		if (doWeDraw == true) {
			this.transform.position = toFollow.transform.position;
		} 
	}


	public void setToFollow(GameObject g)
	{
		toFollow = g;
		npc = g.GetComponent<NPCController> ();
	}
	bool draw()
	{
		if (toFollow == null) {
			this.GetComponent<SpriteRenderer> ().enabled = false;

			return false;
		}

		if(npc.npcB.alarmed == true || npc.myHealth.healthValue <= 0||npc.knockedDown==true || npc.npcB.myType==AIType.cop || npc.npcB.myType==AIType.aggressive|| npc.npcB.myType==AIType.patrolCop|| npc.npcB.myType==AIType.hostage ){
			this.GetComponent<SpriteRenderer> ().enabled = false;

			return false;
		}

		if (Vector2.Distance (Camera.main.transform.position, toFollow.transform.position) < 10) {
			if (isShopkeeper == true) {
				if (npc.npcB.doing == whatAiIsDoing.shopkeep || npc.npcB.doing == whatAiIsDoing.starting) {
					this.GetComponent<SpriteRenderer> ().enabled = true;
					return true;
				}
			} else {
				if (npc.npcB.alarmed == false) {
					this.GetComponent<SpriteRenderer> ().enabled = true;
					return true;
				}
			}
		}
		this.GetComponent<SpriteRenderer> ().enabled = false;
		return false;
	}
}
