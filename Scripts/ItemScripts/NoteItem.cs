using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteItem : Item {
	public string noteText;

	public override string getDescription ()
	{
		return "The note says : " +  noteText;
	}

}
