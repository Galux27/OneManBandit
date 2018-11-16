using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationChoice : MonoBehaviour {
	/// <summary>
	/// Conversation choice, stores what the player says, npc says, the next set of choices & any code to trigger after the choice is selected.
	/// </summary>

	public string playerText="";
	public string NPCResponse="";
	public List<ConversationChoice> nextChoices;
	public ConversationTrigger myTrigger;
	public bool repeatable = false,done=false;

	public void addChoice(ConversationChoice c)
	{
		if (nextChoices == null) {
			nextChoices = new List<ConversationChoice> ();
		}

		nextChoices.Add (c);
	}

	public List<ConversationChoice> getChoices()
	{
		if (nextChoices == null) {
			nextChoices = new List<ConversationChoice> ();
		}
		return nextChoices;
	}


	public void setTrigger()
	{
		if (myTrigger == null) {

		} else {
			myTrigger.OnOptionSelect ();
		}
	}

}

//conversation serialization
//have a seperate id generator for conversations
//use this as the file name & assign this to the conversation manager when its created
//then for each conversation choice store it & whether it has been selected or not and write it to the file when needed.
