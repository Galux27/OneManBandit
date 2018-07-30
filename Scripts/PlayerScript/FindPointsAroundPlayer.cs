using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class that maintains a list of walkable nodes around the player, done here for AI that are trying to combat a player with a human shield so they can move to points around the player to see if they have a better shot.
///  Its done here rather than on the individual NPC to hopefully get a performance increase.
/// </summary>
public class FindPointsAroundPlayer : MonoBehaviour {
	public static FindPointsAroundPlayer me;
	public List<GameObject> myNodesNearMe;
	// Use this for initialization

	void Awake()
	{
		me = this;
	}

	void Start () {
		myNodesNearMe = new List<GameObject> ();
		StartCoroutine ("findPointsRoundPlayer");
	}
	
	// Update is called once per frame
	void Update () {
		//findPoints ();

	}

	public IEnumerator findPointsRoundPlayer(){
		foreach (GameObject g in myNodesNearMe) {
			g.GetComponent<SpriteRenderer> ().color += Color.cyan;

		}

		List<GameObject> nodesThatWeWillCheck = new List<GameObject> ();

		nodesThatWeWillCheck.Add (WorldBuilder.me.findNearestWorldTile (CommonObjectsStore.player.transform.position + (Vector3.up*3)).gameObject);
		yield return new WaitForEndOfFrame ();
		nodesThatWeWillCheck.Add (WorldBuilder.me.findNearestWorldTile (CommonObjectsStore.player.transform.position + ((Vector3.up*-1)*3)).gameObject);
		yield return new WaitForEndOfFrame ();

		nodesThatWeWillCheck.Add (WorldBuilder.me.findNearestWorldTile (CommonObjectsStore.player.transform.position + (Vector3.left*3)).gameObject);
		yield return new WaitForEndOfFrame ();

		nodesThatWeWillCheck.Add (WorldBuilder.me.findNearestWorldTile (CommonObjectsStore.player.transform.position + ((Vector3.left*-1)*3)).gameObject);
		yield return new WaitForEndOfFrame ();

		nodesThatWeWillCheck.Add (WorldBuilder.me.findNearestWorldTile (CommonObjectsStore.player.transform.position + ((Vector3.up + Vector3.left)*3)).gameObject);
		yield return new WaitForEndOfFrame ();

		nodesThatWeWillCheck.Add (WorldBuilder.me.findNearestWorldTile (CommonObjectsStore.player.transform.position + ((Vector3.up+Vector3.right)*3)).gameObject);
		yield return new WaitForEndOfFrame ();

		nodesThatWeWillCheck.Add (WorldBuilder.me.findNearestWorldTile (CommonObjectsStore.player.transform.position + (((Vector3.up*-1)+Vector3.left)*3)).gameObject);
		yield return new WaitForEndOfFrame ();

		nodesThatWeWillCheck.Add (WorldBuilder.me.findNearestWorldTile (CommonObjectsStore.player.transform.position + (((Vector3.up*-1)+Vector3.right)*3)).gameObject);

		nodesThatWeWillCheck.Add (WorldBuilder.me.findNearestWorldTile (CommonObjectsStore.player.transform.position + (Vector3.up*5)).gameObject);
		yield return new WaitForEndOfFrame ();

		nodesThatWeWillCheck.Add (WorldBuilder.me.findNearestWorldTile (CommonObjectsStore.player.transform.position + ((Vector3.up*-1)*5)).gameObject);
		yield return new WaitForEndOfFrame ();

		nodesThatWeWillCheck.Add (WorldBuilder.me.findNearestWorldTile (CommonObjectsStore.player.transform.position + (Vector3.left*5)).gameObject);
		yield return new WaitForEndOfFrame ();

		nodesThatWeWillCheck.Add (WorldBuilder.me.findNearestWorldTile (CommonObjectsStore.player.transform.position + ((Vector3.left*-1)*5)).gameObject);
		yield return new WaitForEndOfFrame ();


		nodesThatWeWillCheck.Add (WorldBuilder.me.findNearestWorldTile (CommonObjectsStore.player.transform.position + ((Vector3.up + Vector3.left)*5)).gameObject);
		yield return new WaitForEndOfFrame ();

		nodesThatWeWillCheck.Add (WorldBuilder.me.findNearestWorldTile (CommonObjectsStore.player.transform.position + ((Vector3.up+Vector3.right)*5)).gameObject);
		yield return new WaitForEndOfFrame ();

		nodesThatWeWillCheck.Add (WorldBuilder.me.findNearestWorldTile (CommonObjectsStore.player.transform.position + (((Vector3.up*-1)+Vector3.left)*5)).gameObject);
		yield return new WaitForEndOfFrame ();

		nodesThatWeWillCheck.Add (WorldBuilder.me.findNearestWorldTile (CommonObjectsStore.player.transform.position + (((Vector3.up*-1)+Vector3.right)*5)).gameObject);
		yield return new WaitForEndOfFrame ();


		myNodesNearMe.Clear ();
		foreach (GameObject g in nodesThatWeWillCheck) {
			if (lineOfSightToTargetWithNoColliderForPathfin (g.transform.position) == true) {
				myNodesNearMe.Add (g);
				g.GetComponent<SpriteRenderer> ().color -= Color.cyan;
			}
			yield return new WaitForEndOfFrame ();

		}

		////Debug.Log ("Looking for points");
		StartCoroutine ("findPointsRoundPlayer");
	}

	public GameObject getRandomPoint()
	{
		if (myNodesNearMe.Count == 0) {
			return null;
		}

		return myNodesNearMe [Random.Range (0, myNodesNearMe.Count)];

	}




	public bool lineOfSightToTargetWithNoColliderForPathfin(Vector3 target){
		Vector3 origin =this.transform.position;
		Vector3 heading = target - origin;
		RaycastHit2D ray = Physics2D.Raycast (origin, heading,Vector3.Distance(this.transform.position,target));

		if (ray.collider == null) {
			Debug.DrawRay (origin, heading,Color.green);

			return true;
		} else {
			if (ray.collider.gameObject.tag == "Player") {
				return true;
			}
			Debug.DrawRay (origin, heading,Color.red);

			return false;
		}
	}
}
