using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeComponent : MonoBehaviour {
	/// <summary>
	/// Part of a tree, 
	/// </summary>
	public TreeComponentType myType;
	public List<TreeComponent> components;
	public SpriteRenderer sr;
	public List<Transform> ComponentPoints;
	void Awake(){
		sr = this.GetComponent<SpriteRenderer> ();
	}



	public void setMyComponents(int sortingOrder)
	{
		TreeGenerator tg = TreeGenerator.me;
		foreach (Transform t in ComponentPoints) {
			int r = Random.Range (0, 100);
			if (r < 25) {
				continue;
			}

			if (myType == TreeComponentType.branch) {
				GameObject g = (GameObject)Instantiate (tg.baseLeaves [Random.Range (0, tg.baseLeaves.Count)], t.transform.position,Quaternion.Euler(0,0,0));
				if (components == null) {
					components = new List<TreeComponent> ();
				}
				g.transform.parent = this.transform;
				g.transform.rotation = Quaternion.Euler(0,0,Random.Range(0,360));
				TreeComponent tc = g.GetComponent<TreeComponent>();
				tc.setMyComponents (sortingOrder+1);
				components.Add (tc);
				g.GetComponent<SpriteRenderer> ().sortingOrder = sortingOrder;

			} else if (myType == TreeComponentType.trunk) {
				GameObject g = (GameObject)Instantiate (tg.baseBranches [Random.Range (0, tg.baseBranches.Count)], t.transform.position,Quaternion.Euler(0,0,0));
				if (components == null) {
					components = new List<TreeComponent> ();
				}
				g.transform.parent = this.transform;

				g.transform.rotation = Quaternion.Euler(0,0,Random.Range(0,360));
				TreeComponent tc = g.GetComponent<TreeComponent>();
				tc.setMyComponents (sortingOrder+1);
				components.Add (tc);
				g.GetComponent<SpriteRenderer> ().sortingOrder = sortingOrder;

			}
		}
	}
}

public enum TreeComponentType{
	trunk,
	branch,
	leaves
}
