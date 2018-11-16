using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEffect : MonoBehaviour {

    public static List<FireEffect> instances;
	//TODO smoke,damage to entities, affecting pathfinding, spreading, potentially fading
	// Use this for initialization

	/// <summary>
	/// Controls the fire effect.
	/// </summary>

	bool pathInitialised = false;
	public List<GameObject> mySmoke;
	float smokeTimer= 0.5f;
	float spreadTimer = 20.0f;
	public int fireToCreate = 10;
	public GameObject fireEffect;
	void Start () {
        if(FireEffect.instances==null)
        {
            FireEffect.instances = new List<FireEffect>();
        }
        FireEffect.instances.Add(this);
		mySmoke = new List<GameObject> ();
		createFire ();
		this.GetComponent<AudioController> ().playSoundOnLoop (SFXDatabase.me.fire);
        LevelIncidentController.me.addIncident("Fire", this.transform.position);
	}


	// Update is called once per frame
	void Update () {
		if (ThreadedPathfindInterface.me.nodes == null) {
			return;
		}

        if(FireEffect.instances.Count>25)
        {
            Destroy(FireEffect.instances[0].gameObject);
        }

		if (pathInitialised == false) {
			changeNodesNearMe ();
			pathInitialised = true;
		}
		countdownSpreadTimer ();
		SmokeMoniter ();
	}

	void SmokeMoniter()
	{
		if (smokeTimer > 0) {
			smokeTimer -= Time.deltaTime;
		} else {

			if (mySmoke.Count ==0) {
				GameObject g = (GameObject)Instantiate (CommonObjectsStore.me.smokeEffect, getPosForSmoke (), Quaternion.Euler (0, 0, 0));
				SmokeEffect s = g.GetComponent<SmokeEffect> ();
				s.myManager = this.GetComponent<SmokeManager> ();
				mySmoke.Add (g);
				smokeTimer = 0.5f;
			}
		}

		foreach (GameObject g in mySmoke) {
			if (g == null) {
				mySmoke.Remove (g);
				return;
			}
		}
	}

	void createFire()
	{
		bool q = Physics2D.queriesStartInColliders;
		Physics2D.queriesStartInColliders = false;

		for (int x = 0; x < fireToCreate; x++) {
			Vector3 pos = new Vector3 (Random.Range ((size/2) * -1, (size/2)), Random.Range ((size/2) * -1, (size/2)), 0);
			pos = pos + this.transform.position;
			RaycastHit2D ray = Physics2D.Raycast (this.transform.position, pos - this.transform.position,size/2);
			if (ray.collider == null) {
				GameObject g = (GameObject)Instantiate (fireEffect, pos, Quaternion.Euler (0, 0, 0));
				g.transform.parent = this.transform;
			} else {
				GameObject g = (GameObject) Instantiate (fireEffect, ray.point, Quaternion.Euler (0, 0, 0));
				g.transform.parent = this.transform;
			}
		}
		Physics2D.queriesStartInColliders=q;

	}

	public float size=1.5f;
	Vector3 getPosForSmoke()
	{
		return new Vector3 (this.transform.position.x + Random.Range ((size / 2) * -1, size / 2), this.transform.position.y + Random.Range ((size / 2) * -1, size / 2), 0); 
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.transform.root.tag == "NPC" || other.transform.root.tag == "Player") {
			if (other.transform.root.gameObject.GetComponentInChildren<PersonOnFireEffect> () == false) {
				ArtemAestheticController aac = other.transform.root.GetComponentInChildren<ArtemAestheticController> ();
				GameObject g = (GameObject)Instantiate (CommonObjectsStore.me.fireEffect, aac.torsoObj.transform);
				g.transform.localPosition = Vector3.zero;
			}
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.transform.root.tag == "NPC" || other.transform.root.tag == "Player") {
			if (other.transform.root.gameObject.GetComponentInChildren<PersonOnFireEffect> () == false) {
				ArtemAestheticController aac = other.transform.root.GetComponentInChildren<ArtemAestheticController> ();
				GameObject g = (GameObject)Instantiate (CommonObjectsStore.me.fireEffect, aac.torsoObj.transform);
				g.transform.localPosition = Vector3.zero;
			}
		}
	}

	void OnTriggerStay2D(Collider2D other)
	{
		//Debug.Log ("Fire triggered by " + other.gameObject.name);
		PersonHealth ph = other.gameObject.transform.root.GetComponent<PersonHealth> ();
		if (ph == null) {

		} else {
			ph.dealDamageFromFire ();
		}

		PlayerCarController pcc = other.GetComponent<PlayerCarController> ();
		if (pcc == null) {

		} else {
			pcc.dealDamage (10);
		}
	}

	Vector3 getPosForSpread()
	{
		WorldTile pos = WorldBuilder.me.getNearest (this.transform.position);

		if (pos == null) {

			return new Vector3 (this.transform.position.x + Random.Range ((3) * -1,3), this.transform.position.y + Random.Range ((3) * -1, 3), 0); 

		} else {
			int r = Random.Range (0, pos.myNeighbours.Count);
			return pos.myNeighbours [r].transform.position;
		}

	}

	int oddsOfSpread = 50;

	void countdownSpreadTimer()
	{
        if(oddsOfSpread>0)
        {
            spreadTimer -= Time.deltaTime;

        }
        if (spreadTimer <= 0) {
			int r = Random.Range (0, 100);
			if (r < oddsOfSpread) {
				//spread
				GameObject g = Instantiate(CommonObjectsStore.me.fire,getPosForSpread(),this.transform.rotation);
				spreadTimer = 20.0f;
				oddsOfSpread -= 10;
			} else {
				//put out
				//setNodesToNormal();
				//foreach (GameObject g in mySmoke) {
				//	g.GetComponent<SmokeEffect> ().enabled = false;
				//}
				//Destroy (this.gameObject);
			}
		}
	}

	void changeNodesNearMe()
	{
		
		WorldTile pos = WorldBuilder.me.getNearest (this.transform.position);

		for (int x = -2; x < 2; x++) {
			for (int y = -2; y < 2; y++) {
				WorldTile wt = WorldBuilder.me.worldTiles [pos.gridX + x, pos.gridY + y].GetComponent<WorldTile>();

				if (wt.walkable == true) {
					////Debug.Log (wt.gameObject.name + " set to unwalkable");
					wt.GetComponent<SpriteRenderer> ().color = Color.blue;
					//wt.walkable = false;

					float modifier = 100 + (1000- (Vector2.Distance (this.transform.position, wt.gameObject.transform.position) * 100));
					//Debug.Log (modifier);
					wt.tempModifiers = Mathf.RoundToInt (modifier);
					//nodesISetToUnwalkable.Add (wt);
					try{
						ThreadedPathfindInterface.me.nodes [wt.gridX, wt.gridY].tempModifiers = Mathf.RoundToInt (modifier);
					}
					catch{
						//Debug.LogError ("Could not find threaded tiles");
						continue;
					}
				}
			}
		}
	}

	void setNodesToNormal()
	{
		
		WorldTile pos = WorldBuilder.me.getNearest (this.transform.position);

		for (int x = -2; x < 2; x++) {
			for (int y = -2; y < 2; y++) {
				WorldTile wt = WorldBuilder.me.worldTiles [pos.gridX + x, pos.gridY + y].GetComponent<WorldTile>();

				if (wt.walkable == true) {
					////Debug.Log (wt.gameObject.name + " set to unwalkable");
					wt.GetComponent<SpriteRenderer> ().color = Color.white;
					//wt.walkable = false;
					float modifier = 100  + (1000- (Vector2.Distance (this.transform.position, wt.gameObject.transform.position) * 100));


					wt.tempModifiers -= Mathf.RoundToInt (modifier);

					if (wt.tempModifiers < 0) {
						wt.tempModifiers = 0;
					}

					//nodesISetToUnwalkable.Add (wt);
					try{
						ThreadedPathfindInterface.me.nodes [wt.gridX, wt.gridY].tempModifiers -= Mathf.RoundToInt (modifier);

						if(ThreadedPathfindInterface.me.nodes [wt.gridX, wt.gridY].tempModifiers<0)
						{
							ThreadedPathfindInterface.me.nodes [wt.gridX, wt.gridY].tempModifiers=0;
						}
					}
					catch{
						continue;
					}

				}
			}
		}
	}

    private void OnDestroy()
    {
        FireEffect.instances.Remove(this);
    }
}
