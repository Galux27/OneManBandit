using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Payphone : MonoBehaviour {

	/// <summary>
	/// Class that marks an object as a payphone that a civilian can use to call the police. 
	/// </summary>

	void Start () {
		LevelController.me.phonesInLevel.Add (this.gameObject);
	}
	

}
