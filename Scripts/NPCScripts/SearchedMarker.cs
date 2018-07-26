using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchedMarker : MonoBehaviour {
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
