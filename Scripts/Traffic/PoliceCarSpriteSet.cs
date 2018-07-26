using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	
	// Update is called once per frame
	void Update () {
		
	}
}
