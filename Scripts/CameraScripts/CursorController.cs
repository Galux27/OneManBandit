using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour {
	/// <summary>
	/// Sets the cursor texture based on whats being done e.g. computer, phone, shooting.
	/// </summary>
	public Texture2D active,normal,computer,phone;
	float timer = 0.05f;
	int counter = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		cursorSet ();
	}

	void cursorSet()
	{
		if (ComputerUIControl.me.computerBeingUsed == true) {
			if (active != computer) {
				active = computer;
				Cursor.SetCursor (active, new Vector2 (8, 8), CursorMode.ForceSoftware);
			}

		} else if (PhoneController.me.displayPhoneController == true) {
			if (active != phone) {
				active = phone;
				Cursor.SetCursor (active, new Vector2 (21, 8), CursorMode.ForceSoftware);
			}

		} else {
			if (active != normal) {
				active = normal;
				Cursor.SetCursor (active, new Vector2 (24, 24), CursorMode.ForceSoftware);
			}
		}

	}



}
