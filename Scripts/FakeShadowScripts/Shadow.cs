using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour {

	/// <summary>
	/// Creates a "Shadow" for 2d objects by creating a child object with the same sprite but set to be a transparent black color then offsetting it in a direction based on the closest light source.
	/// </summary>

	public List<GameObject> toCreateShadowsFor;

	public List<GameObject> myShadows;
	public LightSource currentLightSource;
	public bool staticShadow=false,isItem=false;

	/// <summary>
	/// On start objects to create a shadow are calculated, if its a person it gets the torso & arms else we just get all children with a sprite renderer. 
	/// </summary>
	void Start () {

		if (this.gameObject.GetComponent<Item> () == true) {
			isItem = true;
		}

		StartCoroutine (setLightSource ());
		if (this.gameObject.GetComponentInChildren<ArtemAnimationController> () == true) {
			ArtemAnimationController ac = this.gameObject.GetComponentInChildren<ArtemAnimationController> ();
			toCreateShadowsFor.Add (ac.myAesthetic.torsoObj);
			toCreateShadowsFor.Add (ac.myAesthetic.lShoulderObj);
			toCreateShadowsFor.Add (ac.myAesthetic.lForeObj);
			toCreateShadowsFor.Add (ac.myAesthetic.lHandObj);

			toCreateShadowsFor.Add (ac.myAesthetic.rShoulderObj);
			toCreateShadowsFor.Add (ac.myAesthetic.rForeObj);
			toCreateShadowsFor.Add (ac.myAesthetic.rHandObj);

		} else {
			if (this.GetComponent<SpriteRenderer> () == true) {
				toCreateShadowsFor.Add (this.gameObject);
			}
			//for (int x = 0; x < transform.childCount; x++) {
			//	GameObject g = transform.GetChild (x).gameObject;
			//	if (g.GetComponent<SpriteRenderer> () == true) {
			//		toCreateShadowsFor.Add (g);
			//	}
			//}
		}

		foreach (GameObject g in toCreateShadowsFor) {
			GameObject shadow = new GameObject ();
			if (staticShadow == true) {
				shadow.transform.position = this.transform.position + new Vector3 (0.1f, 0.1f, 0);
				shadow.transform.rotation = g.transform.rotation;
			} else {
				shadow.transform.position = this.transform.position;
				shadow.transform.rotation = g.transform.rotation;
			}
			shadow.transform.parent = g.transform;
			//GameObject shadow = (GameObject)Instantiate (new GameObject (), g.transform);
			SpriteRenderer sr = shadow.AddComponent<SpriteRenderer> ();
			sr.sortingOrder = g.GetComponent<SpriteRenderer> ().sortingOrder - 1 ;
			sr.color = new Color (0, 0, 0, 0.5f);
			sr.sprite = g.GetComponent<SpriteRenderer> ().sprite;
			myShadows.Add (shadow);
		}
	}

	public void addNewShadow(GameObject add)
	{
		if (toCreateShadowsFor == null) {
			toCreateShadowsFor = new List<GameObject> ();
		}
		toCreateShadowsFor.Add (add);
		GameObject shadow = (GameObject)Instantiate (new GameObject (), add.transform);
		SpriteRenderer sr = shadow.AddComponent<SpriteRenderer> ();
		sr.sortingOrder = add.GetComponent<SpriteRenderer> ().sortingOrder - 1 ;
		sr.color = new Color (0, 0, 0, 0.5f);
		sr.sprite = add.GetComponent<SpriteRenderer> ().sprite;
		myShadows.Add (shadow);
	}

	// Update is called once per frame
	void Update () {
		if (staticShadow == false) {
			if (shouldWeCalculateShadows ()) {
				////Debug.Log (this.gameObject.name + " is looking for a new shadow source");
			
				if (currentLightSource == null) {
					currentLightSource = LightSource.sun;
				} else {
					setShadowPositions ();
				}
			}

		
		} else {
			if (currentLightSource == null) {
				currentLightSource = LightSource.sun;
			} else {
				setShadowPositions ();
			}
			Destroy (this);
		}

		if (isItem == true) {
			if (shouldWeCalculateShadows ()) {
				sortingOrderFix ();
			}
		}
	}

	/// <summary>
	/// Sets the light source for calculating the shadow position every 0.1 seconds, no need to do it every frame.  
	/// </summary>
	/// <returns>The light source.</returns>
	IEnumerator setLightSource()
	{
		if (shouldWeCalculateShadows () == true) {
			LightSource newSource = FakeShadowGenerator.me.getNearestLightSource (this.gameObject);
			if (newSource == null) {
				newSource = LightSource.sun;
			}


			if (newSource.lightOn == true && newSource != currentLightSource) {
				currentLightSource = newSource;
			}

			yield return new WaitForSeconds (0.1f);
			StartCoroutine (setLightSource ());
		} else {
			yield return new WaitForSeconds (0.1f);
			StartCoroutine (setLightSource ());
		}
	}

	void sortingOrderFix()
	{
		for (int x = 0; x < toCreateShadowsFor.Count; x++) {
			SpriteRenderer sr1 = toCreateShadowsFor [x].GetComponent<SpriteRenderer> ();
			SpriteRenderer sr2 = myShadows [x].GetComponent<SpriteRenderer> ();


			if (sr1.sortingOrder <= sr2.sortingOrder) {
				sr2.sortingOrder = sr1.sortingOrder - 1;
			}
		}
	}

	void setShadowPositions()
	{

		if (LevelTilemapController.me.areWeLit(this.transform.position)==true) {
			Vector3 dir = currentLightSource.getDirectionFromSource (this.gameObject).normalized/10;
			for (int x = 0; x <toCreateShadowsFor.Count; x++) {
				GameObject shadow = myShadows [x];
				GameObject shadowSource = toCreateShadowsFor [x];

			
				if(shadowSource.activeInHierarchy==true)
				{
					shadow.SetActive(true);
					SpriteRenderer sourcesr = shadowSource.GetComponent<SpriteRenderer> ();
					SpriteRenderer sr = shadow.GetComponent<SpriteRenderer> ();
					sr.sprite = sourcesr.sprite;
					sr.flipX = sourcesr.flipX;

				}
				else{
					SpriteRenderer sourcesr = shadowSource.GetComponent<SpriteRenderer> ();
					SpriteRenderer sr = shadow.GetComponent<SpriteRenderer> ();
					sr.sprite = sourcesr.sprite;
					sr.flipX = sourcesr.flipX;
					shadow.SetActive(false);

				}
				//shadow.GetComponent<SpriteRenderer> ().sprite = shadowSource.GetComponent<SpriteRenderer> ().sprite;
				shadow.transform.position = shadowSource.transform.position + dir;
			}
		} else {
			for (int x = 0; x < toCreateShadowsFor.Count; x++) {
				GameObject shadow = myShadows [x];
				GameObject shadowSource = toCreateShadowsFor [x];

				if(shadowSource.activeInHierarchy==true)
				{
					shadow.SetActive(true);
					SpriteRenderer sourcesr = shadowSource.GetComponent<SpriteRenderer> ();
					SpriteRenderer sr = shadow.GetComponent<SpriteRenderer> ();
					sr.sprite = sourcesr.sprite;
					sr.flipX = sourcesr.flipX;
				}
				else{
					shadow.SetActive(false);
				}

				//shadow.GetComponent<SpriteRenderer> ().sprite = shadowSource.GetComponent<SpriteRenderer> ().sprite;

				shadow.transform.position = shadowSource.transform.position;
			}
		}


	}

	bool shouldWeCalculateShadows()
	{
		Vector2 myPos = new Vector2 (this.transform.position.x, this.transform.position.y);
		Vector2 camPos = new Vector2 (CommonObjectsStore.me.mainCam.transform.position.x, CommonObjectsStore.me.mainCam.transform.position.y);

		if (Vector2.Distance (myPos, camPos) < 15) {
			return true;
		} else {
			return false;
		}
	}
}
