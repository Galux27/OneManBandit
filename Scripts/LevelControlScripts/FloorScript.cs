﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorScript : MonoBehaviour {

	/// <summary>
	/// Just here for when I decide to implement multiple floors, has no use at the moment. 
	/// </summary>

	public Transform bottomLeft,topRight;
	public string name;

	public bool isObjectInRoom(GameObject obj)
	{
		if (obj.transform.position.x > bottomLeft.position.x && obj.transform.position.x < topRight.position.x) {
			if (obj.transform.position.y > bottomLeft.position.y && obj.transform.position.y < topRight.position.y) {
				return true;
			}
		}
		return false;
	}
}
