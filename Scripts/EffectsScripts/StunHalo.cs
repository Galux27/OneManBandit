using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunHalo : MonoBehaviour {
	/// <summary>
	/// Rotates a sprite round an NPCs head to indicate they are stunned. 
	/// </summary>

	
	// Update is called once per frame
	void Update () {
		Vector3 newRot = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z) + new Vector3(0,0,160*Time.deltaTime);
		this.transform.rotation = Quaternion.Euler (newRot);
	}
}
