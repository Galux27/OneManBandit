using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Payphone : MonoBehaviour {

	// Use this for initialization
	void Start () {
		LevelController.me.phonesInLevel.Add (this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
