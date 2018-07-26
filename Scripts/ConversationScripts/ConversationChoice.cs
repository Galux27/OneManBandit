using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationChoice : MonoBehaviour {
	/// <summary>
	/// Conversation choice, stores what the player says, npc says, the next set of choices & any code to trigger after the choice is selected.
	/// </summary>

	public string playerText;
	public string NPCResponse;
	public List<ConversationChoice> nextChoices;
	public ConversationTrigger myTrigger;

}
