using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class MeshSortingOrderSetter : MonoBehaviour {
	public int sortingOrder;
	void Awake()
	{
		MeshRenderer m = this.gameObject.GetComponent<MeshRenderer> ();
		m.sortingOrder = sortingOrder;

	}
}
