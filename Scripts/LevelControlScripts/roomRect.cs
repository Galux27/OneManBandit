using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class roomRect : MonoBehaviour{
	/// <summary>
	/// Class to store the rect for a room/building 
	/// </summary>


	public Transform bottomLeft,topRight;

	void Awake()
	{
		//bottomLeft.parent = null;
		//topRight.parent = null;
	}

	public bool amIInRoomRect(Vector3 pos)
	{
		if (pos.x > bottomLeft.position.x-1.0f && pos.x < topRight.position.x+1.0f) {
			if (pos.y > bottomLeft.position.y-1.0f && pos.y < topRight.position.y+1.0f) {
				return true;
			}
		}
		return false;
	}

	public bool amIInRoomRect(GameObject obj)
	{
		if (obj.transform.position.x >= bottomLeft.position.x-1.0f && obj.transform.position.x <= topRight.position.x+1.0f) {
			if (obj.transform.position.y >= bottomLeft.position.y-1.0f && obj.transform.position.y <= topRight.position.y+1.0f) {
				return true;
			}
		}
		return false;
	}

	public Vector3 getCenterPoint()
	{
		return Vector3.Lerp (bottomLeft.position, topRight.position, 0.5f);
	}
}
