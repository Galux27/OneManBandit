using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that changes the sprite of a police car based on what it spawns. 
/// </summary>
public class PoliceCarSpriteSet : MonoBehaviour {
	public Sprite policeCar,policeVan;
	// Use this for initialization
	void Start () {
		PoliceCarScript pcs = this.GetComponentInParent<PoliceCarScript> ();

		if (pcs.swat == true) {
			this.GetComponent<SpriteRenderer> ().sprite = policeVan;
		}
		Destroy (this);
	}
}
