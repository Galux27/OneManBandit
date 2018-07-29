using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class MeshSortingOrderSetter : MonoBehaviour {

	/// <summary>
	/// Class to set a meshs sorting order as it can't be done via the inspector. 
	/// </summary>

	public int sortingOrder;
	void Awake()
	{
		MeshRenderer m = this.gameObject.GetComponent<MeshRenderer> ();
		m.sortingOrder = sortingOrder;

	}
}
