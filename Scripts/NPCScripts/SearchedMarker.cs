using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchedMarker : MonoBehaviour {

	/// <summary>
	/// Class thats added to the player when they are searched by an NPC so that the NPC doesn't repeatedly search the player.
	/// </summary>

	public List<GameObject> searchedBy;

	public void addToSearchedBy(GameObject g)
	{
		if (searchedBy == null) {
			searchedBy = new List<GameObject> ();
		}

		if (searchedBy.Contains (g) == false) {
			searchedBy.Add (g);
		}
	}
}
