using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour {
	public GameObject target;
	public float pathResetRate = 1.0f, pathTimer = 1.0f;
	public List<Vector3> currentPath;
	public int counter = 0;
	public bool followPath = false;
	ThreadedPathfind tp;
	public bool waitingForPath = true;
	public NPCController myController;

	public bool useMultiThreading=false,pathSmoothing=true;
	public float distToNextPoint=0.0f;
	public Vector3 pathOrigin = Vector3.zero;
	Vector3 myPos;
	public bool askedForNewPath=false;

	// Use this for initialization
	void Start () {
		myController = this.GetComponent<NPCController> ();
		useMultiThreading = true;
		pathSmoothing = false;
		StartCoroutine ("checkIfStuck");
	}

	public void clearPath()
	{
		currentPath = null;
		counter = 0;
		target = null;
	}

	// Update is called once per frame
	void Update () {
		if (currentPath == null && target != null) {
			getPath (this.gameObject, target);
		}

		if (gotNewPath == true) {
			workOutPositionOnNewPath ();
			gotNewPath = false;
		}

		distToNextPoint = Vector3.Distance (this.transform.position, getCurrentPoint ());
		myPos = this.transform.position;

	/*	shouldWeAvoidObstacle ();
		if (obstacle == true && limitTimer <= 0) {
			avoidObstacle ();
		} else {
			if (limitTimer > 0) {
				limitTimer -= Time.deltaTime;
			}

			if (limitTimer <= 0 && obstacle == true ) {
				workOutPositionOnNewPath ();
				obstacle = false;
			}

		}*/

		if (useMultiThreading == false) {
			if (followPath == true) {
			//	rayWallCollider ();
				pathMoniter ();
				shouldWeBeRefreshingPath ();
		
			} else {
				
			}
		}else{
			if (currentPath == null) {
			} else {
				if (currentPath.Count > 0) {
					//rayWallCollider ();
					pathMoniter ();
					shouldWeBeRefreshingPath ();			
				}

				if (tp == null) {
					return;
				} else {
					if (tp.Update ()) {
						tp.Stop ();
						//	tp = null;
					} else {

					}
				}
			}
		}

	}



	void shouldWeBeRefreshingPath()
	{
		if (target == null) {

		} else  {
			if (shouldWeRefreshPath () == true) {
				pathRefresh ();
			}
		}
	}

	bool shouldWeRefreshPath()
	{
		//if (myController.npcB.myType == AIType.civilian) {
		//	return false;
		//}
		//|| Vector3.Distance(this.transform.position,target.transform.position)>5.0f && counter ==currentPath.Count-1 && askedForNewPath==false || stuckCounter >= 50 || currentPath.Count==0 && askedForNewPath==false


		if (target.gameObject.tag == "Player" || target.gameObject.tag == "NPC" || Vector3.Distance (pathOrigin, target.transform.position) > 2.5f || myController.currentBehaviour.myType == behaviourType.exitLevel && myController.npcB.myType == AIType.civilian || myController.npcB.myType == AIType.swat|| myController.npcB.myType == AIType.cop && myController.currentBehaviour.myType == behaviourType.guardEntrance || target.GetComponent<RefreshPathMarker>()==true || Vector2.Distance(target.transform.position,currentPath[currentPath.Count-1])>3.0f && waitingForPath==false && askedForNewPath==false) {
			stuckCounter = 0;
			return true;
		} else {
			return false;
		}
	}

	public void threaded_GetPath(GameObject start, GameObject end)
	{

		//GameObject s = WorldBuilder.me.getNearest (start.transform.position).gameObject;
		GameObject e = WorldBuilder.me.getNearest (end.transform.position).gameObject;


		if (target == null) {
			target = WorldBuilder.me.findNearestWorldTile (end.transform.position).gameObject;
		}
		////Debug.Log (this.gameObject.name + " wants a path ");
		ThreadedPathfindInterface.me.jobsToDo.Add (new ThreadedPathfindJob (e, this));
	}

	public void threaded_GetPath(Vector3 start, Vector3 end)
	{
		/*WorldTile s = WorldBuilder.me.findNearestWorldTile (start);
		WorldTile e = WorldBuilder.me.findNearestWorldTile (end);

		if (target == null) {
			target = e.gameObject;
		}

		//////Debug.Log (this.gameObject.name + " is trying to get a path to " + end);
		ThreadedWorldTile ts = Pathfinding.me.pathNodes [s.gridX, s.gridY];
		ThreadedWorldTile te = Pathfinding.me.pathNodes [e.gridX, e.gridY];

		if (tp == null) {
			tp = new ThreadedPathfind ();
		}
		waitingForPath = true;

		tp.initialise (new Vector3Int (s.gridX, s.gridY, 0), new Vector3Int (e.gridX, e.gridY, 0), this);
		tp.Start ();*/

		if (target == null) {
			target = WorldBuilder.me.findNearestWorldTile (end).gameObject;
		}
		////Debug.Log (this.gameObject.name + " wants a path ");
		ThreadedPathfindInterface.me.jobsToDo.Add (new ThreadedPathfindJob (target, this));

		//List<ThreadedWorldTile> list = new List<ThreadedWorldTile> ();
		//Pathfinding.me.findPath (s.gridX, s.gridY, e.gridX, e.gridY, ref list);
		//setNewPath (list,this.transform.position);
	}

	Vector3 getMidPointOfNextTwoPoints()
	{
		//go through each node from the current and check if you can get a clear line of sight to them, if so then go to the next one, if not then get the last one checked


		if (currentPath == null) {
			return Vector3.zero;
		}

		if (counter < currentPath.Count - 1) {
			//roomToGoTo.bottomLeft.position + (( roomToGoTo.topRight.position - roomToGoTo.bottomLeft.position )/2);
			return currentPath [counter] + ((currentPath [counter + 1] - currentPath [counter]) / 2);
		} else if (counter == currentPath.Count - 1) {
			return currentPath [counter];
		} else {
			return Vector3.zero;
		}
	}

	public Vector3 getCurrentPoint()
	{
		//return getMidPointOfNextTwoPoints ();
		if (currentPath == null) {//have some way to tell we need a new path
			return this.transform.position;
		}

		if (counter < currentPath.Count && currentPath != null) {
			return currentPath [counter];
		} else {
			//followPath = false;
			return this.gameObject.transform.position;
		}
	}

	public void getPath(Vector3 start,Vector3 end)
	{

		if (Vector2.Distance (this.transform.position, end) < 1.0f) {
			return;
		}

		if (useMultiThreading == true) {

			target = WorldBuilder.me.findNearestWorldTile (end).gameObject;
			currentPath = Pathfinding.me.findPath (WorldBuilder.me.findNearestWorldTile (start).gameObject,target);
			counter = 0;
			followPath = true;

			threaded_GetPath (start, end);
			pathOrigin = target.transform.position;
			return;

		}

		target = WorldBuilder.me.findNearestWorldTile (end).gameObject;
		currentPath = Pathfinding.me.findPath (WorldBuilder.me.findNearestWorldTile (start).gameObject,target);
		counter = 0;
		followPath = true;

		int tempCount = counter;

		counter = tempCount;

		if (Vector3.Distance (this.transform.position, getCurrentPoint ()) < 0.7f) {
			for (int x = counter+1; x < currentPath.Count - 1; x++) {
				if (myController.detect.lineOfSightToTargetWithNoColliderForPathfin (currentPath [x])==true) {
					if (counter < currentPath.Count-1) {
						tempCount++;
					} else {
						tempCount = currentPath.Count - 1;
					}
				} else {
					break;
				}
			}


			counter = tempCount;


			//////Debug.Break ();
		}
	}
	public int stuckCounter = 0;
	public void increaseStuckCounter()
	{
		stuckCounter++;
		stuck ();
	}

	public void stuck()
	{
		if (target == null) {
			return;
		}

		if (stuckCounter >= 50) {
			if (useMultiThreading == true) {
				followPath = true;
				threaded_GetPath (this.transform.position , target.transform.position);
				pathOrigin = target.transform.position;

				stuckCounter = 0;
			}
		}
	}

	public void getPath(GameObject start,GameObject end)
	{
		if (end == null || start == null || Vector2.Distance (this.transform.position, end.transform.position) < 1.0f) {
			return;
		}


		////Debug.Log ("trying to get path to " + end.name);
		if (useMultiThreading == true) {
			//GameObject s = WorldBuilder.me.getNearest (start.transform.position).gameObject;
			//GameObject e = WorldBuilder.me.getNearest (end.transform.position).gameObject;


			target = end;
			followPath = true;
			threaded_GetPath (start, end);
			pathOrigin = target.transform.position;

			return;
		}


		target = end;
		currentPath = Pathfinding.me.findPath (start, end);
		counter = 0;
		followPath = true;

		int tempCount = counter;

		counter = tempCount;

		if (Vector3.Distance (this.transform.position, getCurrentPoint ()) < 0.7f) {
			for (int x = counter+1; x < currentPath.Count - 1; x++) {
				if (myController.detect.lineOfSightToTargetWithNoColliderForPathfin (currentPath [x])==true) {
					if (counter < currentPath.Count-1) {
						tempCount++;
					} else {
						tempCount = currentPath.Count - 1;
					}
				} else {
					break;
				}
			}
			counter = tempCount;
			//////Debug.Break ();
		}
	}

	void smoothPath()
	{
		int curIndex = counter;
		int newIndex = counter+1;

		for (int i = newIndex; i < currentPath.Count-1; i++) {
			Vector3 heading = currentPath [i] - this.transform.position;
			float distance = heading.magnitude;
			if ((i - curIndex) > 3) {
				Debug.DrawRay (this.transform.position,(heading/distance)*Vector2.Distance(this.transform.position,currentPath[i]), Color.cyan,20.0f);

				break;
			}



			RaycastHit2D ray = Physics2D.Raycast (this.transform.position,heading/distance ,Vector2.Distance(this.transform.position,currentPath[i]),CommonObjectsStore.me.maskForPathSmoothing);

			if (ray.collider==null) {
				newIndex = i;
			} else {
				Debug.DrawRay (this.transform.position,(heading/distance)*Vector2.Distance(this.transform.position,currentPath[i]), Color.red,20.0f);
				//Debug.Break ();
//				Debug.Log ("Path smoothing hit object " + ray.collider.gameObject.name);
				break;
			}
		}
		counter = newIndex;
	}

	void pathMoniter()
	{
		

		/*for (int x = counter+1; x < currentPath.Count - 1; x++) {
			if (myController.detect.lineOfSightToTargetWithNoColliderForPathfin (currentPath [x])==true) {
				if (counter < currentPath.Count-1) {
					tempCount++;
				} else {
					tempCount = currentPath.Count - 1;
				}
			} else {
				break;
			}
		}*/
		////////Debug.Log (this.gameObject.name + " went from " + currentPath [counter] + " to " + currentPath [tempCount] + " Skipping " + (tempCount - counter) + " Nodes.");

		////if (counter == tempCount) {
		//	tempCount++;
		//}
		if (pathSmoothing == true) {
			int tempCount = counter;

			counter = tempCount;

			if (Vector2.Distance (this.transform.position, getCurrentPoint ()) < 0.3f) {
				for (int x = counter + 1; x < currentPath.Count - 1; x++) {
					if (myController.detect.lineOfSightToTargetWithNoColliderForPathfin (currentPath [x]) == true) {
						if (counter < currentPath.Count - 1) {
							tempCount++;
						} else {
							tempCount = currentPath.Count - 1;
						}
					} else {
						break;
					}
				}
				if (tempCount > counter + 2) {
					tempCount--;
				}

				counter = tempCount;

				if (counter == currentPath.Count - 1) {
					pathTimer = 0.0f;
				}
				//////Debug.Break ();
			}
		} else {
			if (Vector2.Distance (this.transform.position, getCurrentPoint ()) < 0.4f) {
				if (counter < currentPath.Count-1) {
					smoothPath ();
					//counter++;
//					//////Debug.LogError ("Incrementing counter to " + counter);

				} else {
					counter = currentPath.Count - 1;
				}
			}

			if (counter == currentPath.Count - 1) {
				pathTimer = 0.0f;
			}
		}
	}



	void pathRefresh()
	{
		


		/*if (counter >= currentPath.Count - 2) {			
			//myController.pmc.setStopped ();

			getPath (this.gameObject, target);

			askedForNewPath = true;
			return;
		}*/


		if (pathTimer > 0.0f || askedForNewPath==true) {
			pathTimer -= Time.deltaTime;
			return;
		}

		if (target == null) {
			return;
		}

		if (Vector2.Distance (target.transform.position, pathOrigin) > 1.5f &&pathTimer <= 0  || counter >= currentPath.Count-3 && pathTimer<=0 ) {
//			Debug.Log (this.gameObject.name + " requested a new path to " + target.name);
			getPath (this.gameObject, target);
			askedForNewPath = true;

			pathTimer = pathResetRate;
		}
		/*pathTimer -= Time.deltaTime;
		if (pathTimer <= 0) {
			getPath (this.gameObject, target);
			pathTimer = pathResetRate;
		}*/
	}
	bool gotNewPath=false;
	public void setNewPath(List<ThreadedWorldTile> list,Vector3 startPos)
	{
		////////Debug.Log ("Path set");
		////////Debug.Log (list.Count);

		askedForNewPath = false;
		waitingForPath = false;
		if (list.Count > 0) {
			List<Vector3> path = new List<Vector3> ();

			foreach (ThreadedWorldTile tw in list) {
				path.Add (tw.worldPos);
			}

			currentPath = path;
			counter = 0;
			if (Vector2.Distance (myPos, currentPath [currentPath.Count - 1]) > Vector2.Distance (myPos, currentPath [0])) {
				currentPath.Reverse ();

				followPath = true;
			//	waitingForPath = true;
				//currentPath.Reverse ();
			} else {
				
				followPath = true;
			//	waitingForPath = true;
				currentPath.Reverse ();
			}
			gotNewPath = true;
			//workOutPositionOnNewPath ();

			//if (Vector3.Distance (myPos, currentPath [0]) > Vector3.Distance (myPos, currentPath [currentPath.Count - 1])) {
			//	currentPath.Reverse ();

			//}

			//counter = 0;
			//followPath = true;
			//waitingForPath = false;

			if (tp == null) {

			} else {
				tp.Stop ();
				//tp = null;
			}
		}



	}


	public void setNewPath(List<Vector3> list)
	{
//		//////Debug.Log ("Path set " + list.Count + " Nodes");
		////////Debug.Log (list.Count);
		waitingForPath = false;

		askedForNewPath = false;
		if (list.Count > 0) {
			//Vector3 startPos = this.transform.position
			List<Vector3> path = list;
			currentPath = path;
			counter = 0;

			if (Vector2.Distance (myPos, currentPath [currentPath.Count - 1]) > Vector2.Distance (myPos, currentPath [0])) {
				//currentPath.Reverse ();
			
				followPath = true;
				//waitingForPath = true;
				//currentPath.Reverse ();
			} else {
				
				followPath = true;
				//waitingForPath = true;
				currentPath.Reverse ();
			}
			gotNewPath = true;
			//workOutPositionOnNewPath ();
		//	if (tp == null) {

		//	} else {
			//	tp.Stop ();
			//	//tp = null;
			//}
		}



	}

	IEnumerator checkIfStuck()
	{
		rayWallCollider ();
		yield return new WaitForSeconds (1.1f);
	}

	public LayerMask wallMask;
	void rayWallCollider()
	{
		RaycastHit2D rayForward = Physics2D.Raycast (this.transform.position, transform.up, 1.0f,wallMask);
		Debug.DrawRay (this.transform.position, transform.up*1.0f, Color.cyan);

		if (rayForward.collider == null) {

		} else {
			//bug.Log ("Fixing wall bumb, moving down");

			fixWallBump (new Vector3 (this.transform.position.x, this.transform.position.y, 0) + (transform.up*-1));
		}

		/*RaycastHit2D rayBackward = Physics2D.Raycast (this.transform.position, transform.up*-1, 1.5f,wallMask);
		Debug.DrawRay (this.transform.position, transform.up*-1.5f, Color.cyan);

		if (rayBackward.collider == null) {

		} else {
			//////Debug.Log ("Fixing wall bumb, moving up");

			fixWallBump (new Vector3 (this.transform.position.x, this.transform.position.y+1, 0));
		}

		RaycastHit2D rayRight = Physics2D.Raycast (this.transform.position, transform.right, 1.5f,wallMask);
		Debug.DrawRay (this.transform.position, transform.right*1.5f, Color.cyan);

		if (rayRight.collider == null) {

		} else {
			//////Debug.Log ("Fixing wall bumb, moving left");
			fixWallBump (new Vector3 (this.transform.position.x-1, this.transform.position.y, 0));
		}

		RaycastHit2D rayLeft = Physics2D.Raycast (this.transform.position, transform.right*-1, 1.5f,wallMask);
		Debug.DrawRay (this.transform.position, transform.right*-1.5f, Color.cyan);

		if (rayLeft.collider == null) {

		} else {
			//////Debug.Log ("Fixing wall bumb, moving right");

			fixWallBump (new Vector3 (this.transform.position.x+1, this.transform.position.y, 0));
		}*/
	}

	void fixWallBump(Vector3 start)
	{
		//target = end;

		if (target == null) {
			return;
		}

		currentPath = Pathfinding.me.findPath (WorldBuilder.me.findNearestWorldTile(start).gameObject, target);
		counter = 0;
		followPath = true;

		int tempCount = counter;

		counter = tempCount;

		if (Vector2.Distance (this.transform.position, getCurrentPoint ()) < 0.7f) {
			for (int x = counter+1; x < currentPath.Count - 1; x++) {
				if (myController.detect.lineOfSightToTargetWithNoColliderForPathfin (currentPath [x])==true) {
					if (counter < currentPath.Count-1) {
						tempCount++;
					} else {
						tempCount = currentPath.Count - 1;
					}
				} else {
					break;
				}
			}
			counter = tempCount;
		}
		//////Debug.Log (this.gameObject.name + " got stuck & needed a new path");
		////Debug.Break ();

	}

	public void workOutPositionOnNewPath()
	{
		int closestIndex = 0;
		float dist = 999999.0f;

		for (int x = 0; x < currentPath.Count; x++) {
			RaycastHit2D ray = Physics2D.Raycast (currentPath [x], (myPos - currentPath [x]).normalized, Vector2.Distance (currentPath [x], myPos));

			if (ray.collider == null || ray.collider.gameObject == this.gameObject) {
				float d = Vector2.Distance (myPos, currentPath [x]);
				if (d < dist) {
					closestIndex = x;
					dist = d;
				}
			}
		}
		counter = closestIndex;
	}

	/*void OnCollisionStay2D(Collision2D other)
	{
		if (other == null) {
			return;
		}

		if (other.transform.root.tag == "NPC" || other.gameObject.tag=="ImpassableObject") {
			shouldWeAvoidObstacle ();
			Debug.Log ("Collided with NPC, calling collision avoidance");
		}
	}*/


	public bool obstacle=false;
	void shouldWeAvoidObstacle()
	{
		Vector3 heading = this.transform.up.normalized;
		bool startInCol = Physics2D.queriesStartInColliders;
		Physics2D.queriesStartInColliders = false;
		RaycastHit2D hit = Physics2D.Raycast (this.transform.position,heading, 1.5f,CommonObjectsStore.me.maskForObjectAvoidance);
		Debug.DrawRay (this.transform.position, heading, Color.green);
		if (hit.collider == null) {
			//dirMod = Vector3.zero;
			objInWay=null;
			obstacle = false;
		} else {
			//Debug.Log (this.gameObject.name + " was blocked by " + hit.collider.gameObject.name);
			if (hit.collider.transform.root.tag == "Door" ||hit.collider.gameObject.tag != "NPC")  {
				return;
			}
			/*if (hit.collider.gameObject.tag != "NPC") {
				if (objInWay == null) {
					pathRefresh ();
					objInWay = hit.collider.gameObject;

				} else if (hit.collider.gameObject.tag != objInWay.gameObject.tag) {
					pathRefresh ();

				}
			}*/
			obstacle = true;
		}
		Physics2D.queriesStartInColliders = startInCol;
	}
	public Vector3 dirMod=Vector3.zero;
	void avoidObstacle()
	{
		Vector3 headingWithMostRoom = Vector3.zero;
		List<float> dist = new List<float> ();
		List<Vector3> dir = new List<Vector3> ();

		bool startInCol = Physics2D.queriesStartInColliders;
		Physics2D.queriesStartInColliders = false;

		for (int x = -90; x < 90; x += 10) {
			Vector3 noAndle = this.transform.up.normalized;
			Quaternion spreadAngle = Quaternion.AngleAxis (x, new Vector3 (0, 0, 1));
			Vector3 vec = spreadAngle * noAndle;
			RaycastHit2D ray = Physics2D.Raycast (this.transform.position, vec,CommonObjectsStore.me.maskForObjectAvoidance);
			Debug.DrawRay (this.transform.position, vec);
			dist.Add (ray.distance);
			dir.Add (vec.normalized);
		}
		float longest = 0.0f;
		int ind = 0;

		float d = 9999999.0f;

		//foreach (Vector3 pos in dir) {
		//	if (Vector2.Distance (pos, getMidPointOfNextTwoPoints ()) < d) {
		//		d = Vector2.Distance (pos, getMidPointOfNextTwoPoints ());
		//		ind = dir.IndexOf (pos);
		//	}
		//}

		foreach (float f in dist) {
			if (f > longest) {
				longest = f;
				ind = dist.IndexOf (f);
			}
		}
		Debug.DrawLine (this.transform.position, this.transform.position + (dirMod*dist[ind]), Color.red);
		limitTimer = 1.5f;
		dirMod = dir [ind];
		Physics2D.queriesStartInColliders = startInCol;
		//Debug.Break ();
	}
	GameObject objInWay=null;

	float limitTimer = 0.5f;
}
