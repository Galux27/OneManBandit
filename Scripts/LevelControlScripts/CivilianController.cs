using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilianController : MonoBehaviour {
	/// <summary>
	/// Class that controls the spawning of civilians, spawn points are decided by getting nodes that are 7 meters or more from the player & whos weight is not above 100 (stops them spawning on roads, other walkable places that you wouldn't want them(
	///  NPCs are then culled if they are far enough away from the player and are not required (civilians that are trying to raise the alarm are not culled)
	/// </summary>


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
		//initialiseCivilians ();
	}
	
	// Update is called once per frame
	void Update () {
		count = pointsForSpawn.Count;
		RemoveCivilianPositions ();
		moniterCivilians ();
		spawnCivilians ();
		cullCivilians ();
	}

	public void initialiseCivilians()
	{
		for (int x = 0; x <civilianLimit; x++) {
			GameObject g = Instantiate (CommonObjectsStore.me.civilian,	spawnPoints[Random.Range(0,spawnPoints.Count)].position, Quaternion.Euler (0, 0, 0));
			civiliansInWorld.Add (g);
		}
	}

	/// <summary>
	/// Spawns civilians at 1 a second until the limit is reached, time limit is so that the last civilian spawned has time to get a path so we don't have a number of civilians just standing around. 
	/// </summary>
	float timeBetweenCivilians=1.0f;
	void spawnCivilians()
	{
		if (timeBetweenCivilians <= 0) {
			if (pointsForSpawn == null) {
				return;
			}
			if (pointsForSpawn.Count > 0) {
				if (civiliansInWorld.Count < civilianLimit) {
					int r = Random.Range (0, pointsForSpawn.Count);
					if (pointsForSpawn [r] == null) {
						return;
					}
					GameObject g = (GameObject)Instantiate (CommonObjectsStore.me.civilian,	pointsForSpawn [Random.Range (0, pointsForSpawn.Count)], Quaternion.Euler (0, 0, 0));
					civiliansInWorld.Add (g);
					timeBetweenCivilians = 1.0f;
				}
			}
		} else {
			timeBetweenCivilians -= Time.deltaTime;

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
					if (d > 7) {
						if (pointsForSpawn.Contains (new Vector3Int(Mathf.RoundToInt(pos.x),Mathf.RoundToInt(pos.y),0)) == false) {
							try{
									WorldTile wt = WorldBuilder.me.getNearest (pos);//add a check for whether its in a forbiddon room
									if (wt == null || wt.walkable == false || wt.modifier > 100) {
										continue;
									} else {
										pointsForSpawn.Add (new Vector3Int(Mathf.RoundToInt(pos.x),Mathf.RoundToInt(pos.y),0));
									}
							}
							catch{
								continue;
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

	void cullCivilians()
	{
		foreach (NPCController npc in NPCManager.me.npcControllers) {
			if (npc.npcB.myType == AIType.civilian) {
				if (npc.memory.beenAttacked == true || npc.npcB.alarmed == true || npc.memory.peopleThatHaveAttackedMe.Contains (CommonObjectsStore.player) || npc.npcB.doing == whatAiIsDoing.raisingAlarm || npc.pf.waitingForPath==true) {
					continue;
				} else {
					float d = Vector2.Distance (Camera.main.transform.position, npc.gameObject.transform.position);
					if (d > 30.0f) {
						NPCManager.me.npcControllers.Remove (npc);
						NPCManager.me.npcsInWorld.Remove (npc.gameObject);
						Destroy (npc.gameObject);
						return;
					}
				}
			}
		}
	}
}

