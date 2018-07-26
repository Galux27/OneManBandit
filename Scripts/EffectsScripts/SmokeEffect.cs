using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeEffect : MonoBehaviour {

	/// <summary>
	/// Creates smoke, also has the options to change it to tear gas so NPCs are slowed whilst passing through. 
	/// </summary>

	public float increaseSizeReset,timer;
	public float maxSize;
	public SpriteRenderer sr;
	public Sprite[] potentialSprites;
	public SmokeManager myManager;
	public bool isTearGas = false;
	void Awake()
	{
		sr = this.GetComponent<SpriteRenderer> ();

	}
	// Use this for initialization
	void Start () {
		timer = increaseSizeReset;
		Collider2D col = this.gameObject.AddComponent<PolygonCollider2D> ();
		col.isTrigger = true;
		myManager.smokeFromMe.Add (this.gameObject);
	}

	void OnDestroy()
	{
		myManager.smokeFromMe.Remove (this.gameObject);
	}

	
	// Update is called once per frame
	void Update () {
		increaseSize ();
	}

	void shouldWeCreateMoreSmoke()
	{
		if (myManager.canWeCreateSmoke()==false) {
			return;
		}

		int r = Random.Range (1, 1000);
		if (r > 975) {
			Vector3 pos = myManager.getPositionForSmoke ();
			if (isTearGas == false) {
				GameObject g = (GameObject)Instantiate (CommonObjectsStore.me.smokeEffect, pos, Quaternion.Euler (0, 0, Random.Range (0, 360)));
				SmokeEffect s = g.GetComponent<SmokeEffect> ();
				s.myManager = myManager;
			} else {
				GameObject g = (GameObject)Instantiate (CommonObjectsStore.me.tearGasEffect, pos, Quaternion.Euler (0, 0, Random.Range (0, 360)));
				SmokeEffect s = g.GetComponent<SmokeEffect> ();
				s.myManager = myManager;
			}
		}
	}

	void increaseSize()
	{
		if (this.transform.localScale.x < maxSize) {
			timer -= Time.deltaTime;
			if (timer <= 0) {
				this.transform.localScale = new Vector3 (this.transform.localScale.x + 0.01f, this.transform.localScale.y + 0.01f, 1);
				shouldWeCreateMoreSmoke ();
				timer = increaseSizeReset;
			}
		} else {
			timer -= Time.deltaTime;
			if (timer <= 0) {
				Color c = new Color (sr.color.r, sr.color.g, sr.color.b, sr.color.a - 0.01f);
				sr.color = c;
				timer = increaseSizeReset;

				if (c.a <= 0) {
					Destroy (this.gameObject);
				}
			}

		}
	}

	void OnTriggerStay2D(Collider2D col)
	{
		if (isTearGas == true) {
			if (col.gameObject.tag == "NPC" || col.gameObject.tag == "Player") {
				PersonMovementController pmc = col.gameObject.GetComponent<PersonMovementController> ();
				pmc.slowedMovement = true;
			}
		}
	}

	void OnTriggerExit2D(Collider2D col)
	{
		if (isTearGas == true) {
			if (col.gameObject.tag == "NPC" || col.gameObject.tag == "Player") {
				PersonMovementController pmc = col.gameObject.GetComponent<PersonMovementController> ();
				pmc.slowedMovement = false;
			}
		}
	}
}
