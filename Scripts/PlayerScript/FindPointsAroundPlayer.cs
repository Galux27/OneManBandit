using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO: LOOKINTO WHY AI WASNT RAISING THE ALARM WHEN LOOKING AT THE PLAYER (MIGHT BE BECAUSE IM MANUALLY SETTING THE PLAYER AS A TARGET RATHER THAN THEM BEING DISCOVERED) + SOMETIMES THE COLOR OF THE FOV IS WRONG WHEN ATTACKING
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
		findPoints ();
		//yield return new WaitForSeconds (0.1f);
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

	void findPoints()
	{
//		//////Debug.Log ("Finding points around player");

	}


	public bool lineOfSightToTargetWithNoColliderForPathfin(Vector3 target){
		Vector3 origin =this.transform.position;
	



		Vector3 heading = target - origin;
		RaycastHit2D ray = Physics2D.Raycast (origin, heading,Vector3.Distance(this.transform.position,target));

		if (ray.collider == null) {
			//			//////Debug.Log ("No ray hit");
			Debug.DrawRay (origin, heading,Color.green);

			return true;
		} else {
			if (ray.collider.gameObject.tag == "Player") {
				return true;
			}


			////Debug.Log ("Ray hit " + ray.collider.gameObject.name);
			Debug.DrawRay (origin, heading,Color.red);

			//if (ray.collider.gameObject.tag == "WallCollider") {
			//	//////Debug.Log ("We hit a wall " + ray.collider.gameObject.name);
			//}

			////////Debug.Log (ray.collider.gameObject.name);
			return false;
		}
	}
}
