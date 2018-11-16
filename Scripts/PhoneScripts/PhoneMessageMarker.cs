using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class for displaying a message via the PhoneAlert when the player is nearby and has a line of sight to the player 
/// </summary>
public class PhoneMessageMarker : MonoBehaviour {
	public string messageToAdd = "";
	public bool addedMessage=false;


	void OnEnable()
	{
		if (PhoneTab_Notes.me == null) {
			PhoneTab_Notes.me = FindObjectOfType<PhoneTab_Notes> ();
		}
		PhoneTab_Notes.me.messagesToAdd.Add (this);
	}

	public bool shouldWeAddMessage()
	{
		if (Vector2.Distance (this.transform.position, CommonObjectsStore.player.transform.position) < 4.0f) {
			return lineOfSightToTarget (CommonObjectsStore.player);
		} else {
			return false;
		}
	}

	public string getMessage()
	{
		RoomScript r = LevelController.me.getRoomObjectIsIn (this.gameObject);
		if (r == null) {
			return messageToAdd;
		} else {
			return messageToAdd + " Location: " + r.roomName;
		}
	}

	public bool lineOfSightToTarget(GameObject target)
	{
		Vector3 origin = this.transform.position;
		Vector3 heading = target.transform.position - origin;
		RaycastHit2D ray = Physics2D.Raycast (origin, heading);
		//Debug.DrawRay (origin, heading,Color.cyan);

		if (ray.collider == null) {

		} else {
			if (ray.collider.gameObject == target) {
				return true;
			}
		}
		return false;
	}
}
