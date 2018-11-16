using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for finding NPCs a path to a target and moving them along it.
/// </summary>
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
	public bool addedLast=false;
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

		if (waitingForPath==false) {

		} else {
			if (addedLast == false) {
				if (target == null) {
				} else if (target.tag == "Player" || target.tag == "NPC") {

				} else {
					currentPath.Add (target.transform.position);
//					//Debug.Log ("Added " + target.transform.position + " to path for " + this.gameObject.name);
					////Debug.Break ();
				}
				addedLast = true;
			}
		}

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
		
		if (target.gameObject.tag == "Player" || target.gameObject.tag == "NPC" || Vector3.Distance (pathOrigin, target.transform.position) > 2.5f || myController.currentBehaviour.myType == behaviourType.exitLevel && myController.npcB.myType == AIType.civilian || myController.npcB.myType == AIType.swat|| myController.npcB.myType == AIType.cop && myController.currentBehaviour.myType == behaviourType.guardEntrance  || Vector2.Distance(target.transform.position,currentPath[currentPath.Count-1])>3.0f && waitingForPath==false && askedForNewPath==false) {
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
		waitingForPath = true;
		ThreadedPathfindInterface.me.addPathJob (target, this);

		//////Debug.Log (this.gameObject.name + " wants a path ");
		//ThreadedPathfindInterface.me.jobsToDo.Add (new ThreadedPathfindJob (e, this));
	}

	public void threaded_GetPath(Vector3 start, Vector3 end)
	{

		if (target == null) {
			target = WorldBuilder.me.findNearestWorldTile (end).gameObject;
		}
		waitingForPath = true;
		ThreadedPathfindInterface.me.addPathJob (target, this);
		//ThreadedPathfindInterface.me.jobsToDo.Add (new ThreadedPathfindJob (target, this));
	
	}

	Vector3 getMidPointOfNextTwoPoints()
	{


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
		addedLast = false;
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


			////////Debug.Break ();
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

	public void ForceNewPath()
	{
		if (target == null) {
			return;
		}

		followPath = true;
		threaded_GetPath (this.transform.position , target.transform.position);
		pathOrigin = target.transform.position;
		waitingForPath = true;

	}

	public void getPath(GameObject start,GameObject end)
	{
		if (end == null || start == null || Vector2.Distance (this.transform.position, end.transform.position) < 1.0f) {
			return;
		}
		addedLast = false;

		if (useMultiThreading == true) {
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
			////////Debug.Break ();
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
				//Debug.DrawRay (this.transform.position,(heading/distance)*Vector2.Distance(this.transform.position,currentPath[i]), Color.cyan,20.0f);

				break;
			}



			RaycastHit2D ray = Physics2D.Raycast (this.transform.position,heading/distance ,Vector2.Distance(this.transform.position,currentPath[i]),CommonObjectsStore.me.maskForPathSmoothing);

			if (ray.collider==null) {
				newIndex = i;
			} else {
				//Debug.DrawRay (this.transform.position,(heading/distance)*Vector2.Distance(this.transform.position,currentPath[i]), Color.red,20.0f);
			
				break;
			}
		}
		counter = newIndex;
	}

	void pathMoniter()
	{
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
				////////Debug.Break ();
			}
		} else {
			if (Vector2.Distance (this.transform.position, getCurrentPoint ()) < 0.4f) {
				if (counter < currentPath.Count-1) {
					smoothPath ();
					//counter++;
//					////////Debug.LogError ("Incrementing counter to " + counter);

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
		if (pathTimer > 0.0f || askedForNewPath==true) {
			pathTimer -= Time.deltaTime;
			return;
		}

		if (target == null) {
			return;
		}

		if (Vector2.Distance (target.transform.position, pathOrigin) > 1.5f &&pathTimer <= 0  || counter >= currentPath.Count-3 && pathTimer<=0 ) {
			getPath (this.gameObject, target);
			askedForNewPath = true;

			pathTimer = pathResetRate;
		}
	
	}
	bool gotNewPath=false;
	public void setNewPath(List<ThreadedWorldTile> list,Vector3 startPos)
	{
		

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
			
			} else {
				
				followPath = true;
				currentPath.Reverse ();
			}
			gotNewPath = true;


			if (tp == null) {

			} else {
				tp.Stop ();
				//tp = null;
			}
		}



	}


	public void setNewPath(List<Vector3> list)
	{
		waitingForPath = false;

		askedForNewPath = false;
		if (list.Count > 0) {
			//Vector3 startPos = this.transform.position
			List<Vector3> path = list;
			currentPath = path;
			counter = 0;

			if (Vector2.Distance (myPos, currentPath [currentPath.Count - 1]) > Vector2.Distance (myPos, currentPath [0])) {

				followPath = true;

			} else {
				
				followPath = true;
				currentPath.Reverse ();
			}
			gotNewPath = true;
		}



	}

	IEnumerator checkIfStuck()
	{
		rayWallCollider ();
		yield return new WaitForSeconds (1.1f);
	}

	public LayerMask wallMask;
	/// <summary>
	/// If the NPC gets stuck on a wall for long enough this tells it to get a new path.
	/// </summary>
	void rayWallCollider()
	{
		RaycastHit2D rayForward = Physics2D.Raycast (this.transform.position, transform.up, 1.0f,wallMask);
		//Debug.DrawRay (this.transform.position, transform.up*1.0f, Color.cyan);

		if (rayForward.collider == null) {

		} else {

			fixWallBump (new Vector3 (this.transform.position.x, this.transform.position.y, 0) + (transform.up*-1));
		}
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
	}

    public void workOutPositionOnPath()
    {
        int tempCount = 0;
        if (Vector2.Distance(this.transform.position, getCurrentPoint()) < 0.7f)
        {
            for (int x = counter + 1; x < currentPath.Count - 1; x++)
            {
                if (myController.detect.lineOfSightToTargetWithNoColliderForPathfin(currentPath[x]) == true)
                {
                    if (counter < currentPath.Count - 1)
                    {
                        tempCount++;
                    }
                    else
                    {
                        tempCount = currentPath.Count - 1;
                    }
                }
                else
                {
                    break;
                }
            }
            counter = tempCount;
        }
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
	public GameObject obstacle;
	public Vector3 dirMod;
	float limitTimer = 0.5f;
}
