using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class Road : MonoBehaviour {
	/// <summary>
	/// Old road system, needs removing at some point. 
	/// </summary>


	public Transform bottomLeft,topRight;
	public bool roadInitialised=false;
	public bool vertical = false;
	public bool roadAccessable = true;
	public bool invertRoadDirection = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void initialiseRoad()
	{
		this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y, 0);

		if (bottomLeft == null) {
			GameObject g = new GameObject ();
			g.name = "Bottom left road marker";
			g.transform.position = new Vector3 (this.transform.position.x - 1, this.transform.position.y - 1, 0);
			g.transform.parent = this.transform;
			bottomLeft = g.transform;
		}

		if (topRight == null) {
			GameObject g = new GameObject ();
			g.name = "Top right road marker";
			g.transform.position = new Vector3 (this.transform.position.x + 1, this.transform.position.y + 1, 0);
			g.transform.parent = this.transform;
			topRight = g.transform;
		}

		roadInitialised = true;
	}

	public bool isPosInRoadBounds(Vector3 pos)
	{
		if (pos.x > bottomLeft.position.x && pos.y < topRight.position.x) {
			if (pos.y > bottomLeft.position.y && pos.y < topRight.position.y) {
				return true;
			}

		}
		return false;
	}

	public Vector3 getRoadDirection()
	{
		if (vertical == false) {
			Vector3 roadStart = new Vector3 (bottomLeft.transform.position.x, topRight.transform.position.y - (Mathf.Abs (topRight.transform.position.y - bottomLeft.transform.position.y)) / 2, 0);
			Vector3 roadEnd = new Vector3 (topRight.transform.position.x, topRight.transform.position.y - (Mathf.Abs (topRight.transform.position.y - bottomLeft.transform.position.y)) / 2, 0);
		
			return (roadEnd - roadStart).normalized;

		} else {
			Vector3 roadStart = new Vector3 (bottomLeft.transform.position.x + (Mathf.Abs (topRight.transform.position.x - bottomLeft.transform.position.x)) / 2 , bottomLeft.transform.position.y , 0);
			Vector3 roadEnd = new Vector3 (topRight.transform.position.x - (Mathf.Abs (topRight.transform.position.x - bottomLeft.transform.position.x)) / 2, topRight.transform.position.y , 0);

			return (roadEnd - roadStart).normalized;
		}
	}

	public Vector3 getCenterPointOfRoad()
	{
		return new Vector3 (bottomLeft.transform.position.x + Mathf.Abs (topRight.transform.position.x - bottomLeft.transform.position.x) / 2, bottomLeft.transform.position.y + Mathf.Abs (topRight.transform.position.y - bottomLeft.transform.position.y) / 2, 0);
	}

	public List<Road> roadsIIntersect;
	public List<RoadJunction> myJunctions;
	public Vector3 getTopOfRoad()
	{
		if (invertRoadDirection == false) {
			if (vertical == false) {
				return new Vector3 (topRight.position.x, topRight.position.y - (Mathf.Abs (topRight.position.y - bottomLeft.transform.position.y) / 2), 0);

			} else {
				return new Vector3 (topRight.position.x - (Mathf.Abs (topRight.position.x - bottomLeft.position.x) / 2), topRight.position.y, 0);

			}
		} else {
			if (vertical == false) {
				return new Vector3 (bottomLeft.position.x, bottomLeft.position.y + (Mathf.Abs (topRight.position.y - bottomLeft.transform.position.y) / 2), 0);

			} else {
				return new Vector3 (bottomLeft.position.x + (Mathf.Abs (topRight.position.x - bottomLeft.position.x) / 2), bottomLeft.position.y, 0);

			}
		}
	}

	public Vector3 getBottomOfRoad()
	{
		if (invertRoadDirection == false) {
			if (vertical == false) {
				return new Vector3 (bottomLeft.position.x, bottomLeft.position.y + (Mathf.Abs (topRight.position.y - bottomLeft.transform.position.y) / 2), 0);

			} else {
				return new Vector3 (bottomLeft.position.x + (Mathf.Abs (topRight.position.x - bottomLeft.position.x) / 2), bottomLeft.position.y, 0);

			}
		} else {
			if (vertical == false) {
				return new Vector3 (topRight.position.x, topRight.position.y - (Mathf.Abs (topRight.position.y - bottomLeft.transform.position.y) / 2), 0);

			} else {
				return new Vector3 (topRight.position.x - (Mathf.Abs (topRight.position.x - bottomLeft.position.x) / 2), topRight.position.y, 0);

			}

		}
	}

	public void addRoadIIntersect(Road r)
	{
		if (roadsIIntersect == null) {
			roadsIIntersect = new List<Road> ();
		}
		if (roadsIIntersect.Contains (r) == false) {
			roadsIIntersect.Add (r);
		}
	}

	public void findRoadsIIntersect()
	{
		if (roadsIIntersect == null) {
			roadsIIntersect = new List<Road> ();
		}

		Road[] roadsInWorld = FindObjectsOfType<Road> ();

		for (float x1 = 0; x1 < 5; x1+=0.1f) {
			int x = Mathf.RoundToInt (x1);
			foreach (Road r in roadsInWorld) {
				if (r == this) {
					continue;
				}

				if (invertRoadDirection == true) {
					if (r.isPosInRoadBounds (getBottomOfRoad () + getRoadDirection () * (x1 + 1))) {
						if (roadsIIntersect.Contains (r) == false) {
							roadsIIntersect.Add (r);
						}
						r.addRoadIIntersect (this);
					}

					if (r.isPosInRoadBounds (getTopOfRoad () + (getRoadDirection () * -1) * (x1 + 1))) {
						if (roadsIIntersect.Contains (r) == false) {
							roadsIIntersect.Add (r);
						}
						r.addRoadIIntersect (this);

					}
				} else {
					if (r.isPosInRoadBounds (getTopOfRoad() + getRoadDirection () * (x1 + 1))) {
						if (roadsIIntersect.Contains (r) == false) {
							roadsIIntersect.Add (r);
						}
						r.addRoadIIntersect (this);
					}

					if (r.isPosInRoadBounds (getBottomOfRoad() + (getRoadDirection () * -1) * (x1 + 1))) {
						if (roadsIIntersect.Contains (r) == false) {
							roadsIIntersect.Add (r);
						}
						r.addRoadIIntersect (this);

					}
				}
			}
		}
	}
	#if UNITY_EDITOR

	public void calculateJunctions(){
		if (myJunctions == null) {
			myJunctions = new List<RoadJunction> ();
		}

		foreach (Road r in roadsIIntersect) {
			RoadJunction j = this.gameObject.AddComponent<RoadJunction> ();
			j.roadOn = this;
			j.roadGoingTo = r;
			if (Vector3.Distance (getTopOfRoad (), j.getJunctionPoint ()) > Vector3.Distance (getBottomOfRoad (), j.getJunctionPoint ())) {
				DestroyImmediate (j);
			} else {
				myJunctions.Add (j);
				EditorUtility.SetDirty (j);
			}
				
		}
	}

	public void manualJunction(Road r)
	{
		if (myJunctions == null) {
			myJunctions = new List<RoadJunction> ();
		}

		RoadJunction j = this.gameObject.AddComponent<RoadJunction> ();
		j.roadOn = this;
		j.roadGoingTo = r;
		//if (Vector3.Distance (getTopOfRoad (), j.getJunctionPoint ()) > Vector3.Distance (getBottomOfRoad (), j.getJunctionPoint ())) {
		//	DestroyImmediate (j);
		//} else {
			myJunctions.Add (j);
			EditorUtility.SetDirty (j);
		//}
	}
	#endif
	public bool canObjectGoDownRoad(Vector3 pos){
		if (invertRoadDirection == false) {
			if (vertical == true) {
				if (pos.y < getBottomOfRoad ().y) {
					return true;
				}
			} else {
				if (pos.x < getBottomOfRoad ().x) {
					return true;
				}

			}
		} else {
			if (vertical == true) {
				if (pos.y > getTopOfRoad ().y) {
					return true;
				}
			} else {
				if (pos.x > getTopOfRoad ().x) {
					return true;
				}
			}
		}
		return false;
	}
}
