using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCorpse : MonoBehaviour {

	// Use this for initialization
	void Start () {
		NPCManager.me.corpsesInWorld.Add (this.gameObject);

	}
}
