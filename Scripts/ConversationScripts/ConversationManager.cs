using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationManager : MonoBehaviour {
	/// <summary>
	/// Controller for a conversation, has who you are talking to and the initial choices.
	/// </summary>
	public string openingLine="",personSpeakingTo="";
	public List<ConversationChoice> initialOptions;
	NPCController npc;
    InteractIcon i;
	public int ID = -1;

	void Start()
	{
		GameObject icon = (GameObject)Instantiate (CommonObjectsStore.me.speechIcon, this.transform.position, Quaternion.Euler (0, 0, 0));
		i = icon.GetComponent<InteractIcon> ();
		i.setToFollow (this.gameObject);
		i.isShopkeeper = false;
		npc = this.gameObject.GetComponent<NPCController> ();
	}

	public void addChoice(ConversationChoice c)
	{
		if (initialOptions == null) {
			initialOptions = new List<ConversationChoice> ();
		}

		initialOptions.Add (c);
	}

	public List<ConversationChoice> getChoices()
	{
		if (initialOptions == null) {
			initialOptions = new List<ConversationChoice> ();
		}
		return initialOptions;
	}

	public void setID()
	{
		ID = FindObjectOfType<IDManager> ().getEditorID ();
	}

	void Update(){
		if (npc == null) {

		} else {

			if (npc.npcB.alarmed == true || npc.myHealth.healthValue <= 0||npc.knockedDown==true || npc.npcB.myType==AIType.cop || npc.npcB.myType==AIType.aggressive|| npc.npcB.myType==AIType.patrolCop|| npc.npcB.myType==AIType.hostage || npc.npcB.myType==AIType.uniqueHostile) {
                i.gameObject.SetActive(false);
            } else{
				if (Vector2.Distance (CommonObjectsStore.player.transform.position, this.transform.position) < 2) {
					if (Input.GetKeyDown (KeyCode.E)) {
						ConversationUI.me.enableConvo (this);
					}
				}
			}
		}
	}

	public List<string> getDataFromConvo()
	{
		List<string> retVal = new List<string> ();
		ConversationChoice[] myChoices = this.GetComponents<ConversationChoice> ();
		foreach (ConversationChoice cm in myChoices) {
			if (cm.done == true) {
				retVal.Add ("1");
			} else {
				retVal.Add ("0");
			}
		}
		return retVal;
	}

	public void setDataFromFile(List<string> data)
	{
		ConversationChoice[] myChoices = this.GetComponents<ConversationChoice> ();
		for (int x = 0; x < myChoices.Length; x++) {
			if (data [x] == "1") {
				myChoices [x].done = true;
			} else {

				myChoices [x].done = false;
			}
		}
	}
}
