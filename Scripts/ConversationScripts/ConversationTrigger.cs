﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationTrigger : MonoBehaviour {
	/// <summary>
	/// Parent for conversation triggers, used to execute code when a conversation choice is selected e.g. opening a shop, turning an NPC hostile.
	/// </summary>

	public typeOfTrigger myType;
	public virtual void OnOptionSelect()
	{

	}
}

public enum typeOfTrigger{
	None,
	Add_Item,
	Start_Mission,
	Give_Money,
    setIDActive
}
