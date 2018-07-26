using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BleedOutTexStore : MonoBehaviour {
	public static RawImage bleedOutDisplay;
	// Use this for initialization
	void Start () {
		BleedOutTexStore.bleedOutDisplay = this.gameObject.GetComponent<RawImage> ();
	}
	

}
