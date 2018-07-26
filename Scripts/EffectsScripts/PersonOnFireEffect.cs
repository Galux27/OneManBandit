using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonOnFireEffect : MonoBehaviour {

	/// <summary>
	/// Added to a person once they are on fire, deals damage and creates a smoke trail behind them. 
	/// </summary>

	public float smokeTimer = 0.0f;
	public List<GameObject> mySmoke;

	PersonHealth ph;
	public float damageTimer=0.0f,damageReset = 0.1f,lifetime = 5.0f;
	void Start()
	{
		ph = this.GetComponentInParent<PersonHealth> ();
	}

	void Update(){
		control ();
		SmokeMoniter ();
	}

	void control()
	{
		damageTimer -= Time.deltaTime;
		lifetime -= Time.deltaTime;
		if (damageTimer <= 0) {
			ph.dealDamage (50, false);
			damageTimer = damageReset;
		}

		if (lifetime <= 0) {
			Destroy (this.gameObject);
		}
	}

	void SmokeMoniter()
	{
		if (smokeTimer > 0) {
			smokeTimer -= Time.deltaTime;
		} else {

			if (mySmoke.Count < 5) {
				GameObject g = (GameObject)Instantiate (CommonObjectsStore.me.smokeEffect, getPosForSmoke (), Quaternion.Euler (0, 0, 0));
				SmokeEffect s = g.GetComponent<SmokeEffect> ();
				s.myManager = this.GetComponent<SmokeManager> ();
				mySmoke.Add (g);
				smokeTimer = 0.3f;
			}
		}

		foreach (GameObject g in mySmoke) {
			if (g == null) {
				mySmoke.Remove (g);
				return;
			}
		}
	}
	public float size=1.5f;
	Vector3 getPosForSmoke()
	{
		return new Vector3 (this.transform.position.x + Random.Range ((size / 2) * -1, size / 2), this.transform.position.y + Random.Range ((size / 2) * -1, size / 2), 0); 
	}


}
