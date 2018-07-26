using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeManager : MonoBehaviour {
	/// <summary>
	/// Manages smoke effects so there  are only a reasonable amount,  
	/// </summary>
	public List<GameObject> smokeFromMe;
	public Vector3 smokeOriginPoint;
	public float maxSmokeLength = 20.0f;
	// Use this for initialization
	void Start () {
		smokeFromMe = new List<GameObject> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (smokeFromMe.Count > 0) {
			if (maxSmokeLength >= 0) {
				maxSmokeLength -= Time.deltaTime; 
			}
		}

	}

	public bool canWeCreateSmoke()
	{
		if (maxSmokeLength <= 0) {
			return false;
		}

		if (smokeFromMe.Count > 30) {
			return false;
		}
		return true;
	}

	public Vector3 getPositionForSmoke(){
		return smokeOriginPoint + new Vector3 (Random.Range (-2.5f, 2.5f), Random.Range (-2.5f, 2.5f), 0);
	}
}
