using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilianController : MonoBehaviour {
	public static CivilianController me;
	public List<GameObject> civiliansInWorld;
	public List<Transform> spawnPoints;
	public int civilianLimit = 40;
	public int count = 0;
	void Awake()
	{
		me = this;
		civiliansInWorld = new List<GameObject> ();
	}

	// Use this for initialization
	void Start () {
		pointsForSpawn = new List<Vector3Int> ();
		StartCoroutine(setCivilianPositions ());
		initialiseCivilians ();
	}
	
	// Update is called once per frame
	void Update () {
		count = pointsForSpawn.Count;
		RemoveCivilianPositions ();
		moniterCivilians ();
		spawnCivilians ();
	}

	public void initialiseCivilians()
	{
		for (int x = 0; x <civilianLimit; x++) {
			GameObject g = Instantiate (CommonObjectsStore.me.civilian,	spawnPoints[Random.Range(0,spawnPoints.Count)].position, Quaternion.Euler (0, 0, 0));
			civiliansInWorld.Add (g);
		}
	}

	void spawnCivilians()
	{
		if (civiliansInWorld.Count < civilianLimit) {
			GameObject g =(GameObject) Instantiate (CommonObjectsStore.me.civilian,	pointsForSpawn[Random.Range(0,pointsForSpawn.Count)], Quaternion.Euler (0, 0, 0));
			civiliansInWorld.Add (g);
		}
	}

	void moniterCivilians()
	{
		foreach (GameObject g in civiliansInWorld) {
			if (g == null) {
				civiliansInWorld.Remove (g);
				return;
			}
		}
	}

	public void addSpawnPoint(GameObject g)
	{
		if (spawnPoints == null) {
			spawnPoints = new List<Transform> ();
		}

		spawnPoints.Add (g.transform);
	}


	public List<Vector3Int> pointsForSpawn;
	public List<NPCController> civsCreated;


	IEnumerator setCivilianPositions()
	{
		for (int x = -17; x < 17; x++) {
			for (int y = -17; y < 17; y++) {
				Vector3 pos = CommonObjectsStore.player.transform.position + new Vector3 (x, y, 0);
				float d = Vector2.Distance (pos, CommonObjectsStore.player.transform.position);
				if (d > 10) {
					if (pointsForSpawn.Contains (new Vector3Int(Mathf.RoundToInt(pos.x),Mathf.RoundToInt(pos.y),0)) == false) {
						WorldTile wt = WorldBuilder.me.getNearest (pos);//add a check for whether its in a forbiddon room
						if (wt == null || wt.walkable == false || wt.modifier > 300) {
							continue;
						} else {
							pointsForSpawn.Add (new Vector3Int(Mathf.RoundToInt(pos.x),Mathf.RoundToInt(pos.y),0));
						}
					}

				}
				if (y % 5 == 0) {
					yield return new WaitForEndOfFrame ();
				}
			}

		}
		StartCoroutine (setCivilianPositions ());
	}


	void RemoveCivilianPositions()
	{
		for (int x = 0; x < pointsForSpawn.Count; x++) {
			Vector2 p = new Vector2(pointsForSpawn[x].x,pointsForSpawn[x].y);
			float d = Vector2.Distance (p, CommonObjectsStore.player.transform.position);
			if (d > 25 || d < 10) {
				pointsForSpawn.RemoveAt (x);
				return;
			}
		}
	}
}

