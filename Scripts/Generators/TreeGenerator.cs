using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour {

	/// <summary>
	/// Genrates random trees using tree components to work out where to create branches & leaves. 
	/// </summary>

	public static TreeGenerator me;
	public List<GameObject> baseBranches;
	public List<GameObject> baseLeaves;
	public List<GameObject> baseTrunks;
	public int baseSortingOrder=11;
	public List<TreeComponent> myComponents;
	public List<Transform> components;

	void Awake()
	{
		if (me == null) {
			me = this;
		}
	}

	void Start()
	{
		setMyComponents ();
		try{
			WorldTile wt = WorldBuilder.me.getNearest (this.transform.position);
			if (wt == null) {

			}
			else{
				wt.walkable = false;
				WorldBuilder.me.threadedGetNearest (this.transform.position).walkable = false;
			}
		}catch{

		}
	}

	public void setMyComponents()
	{
		
		foreach (Transform t in components) {

			GameObject g = (GameObject)Instantiate (TreeGenerator.me.baseBranches [Random.Range (0, TreeGenerator.me.baseBranches.Count)], t.transform.position,Quaternion.Euler(0,0,0));
			if (myComponents == null) {
				myComponents = new List<TreeComponent> ();
			}
			g.transform.parent = this.transform;
			g.transform.position = new Vector3(g.transform.position.x,g.transform.position.y, Random.Range(g.transform.position.z,g.transform.position.z - 1.5f));
			g.transform.rotation = Quaternion.Euler(0,0,Random.Range(0,360));

			TreeComponent tc = g.GetComponent<TreeComponent>();
			tc.setMyComponents (baseSortingOrder+1);
			myComponents.Add (tc);

		}


	}
}
