using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushGenerator : MonoBehaviour {
	/// <summary>
	/// Generates misc foliage 
	/// </summary>


	public List<GameObject> bushComponents;
	public int numberOfBushComponents=5;
	// Use this for initialization
	void Start () {
		generateBush ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void generateBush()
	{
		for (int x = 0; x < numberOfBushComponents; x++) {
			GameObject g = Instantiate (bushComponents [Random.Range (0, bushComponents.Count)], this.transform);
			g.transform.localPosition = new Vector3 (Random.Range (-1.0f, 1.0f), Random.Range (-1.0f, 1.0f), 0);
			g.GetComponent<SpriteRenderer> ().sortingOrder = Random.Range (2, 4);
		}
	}
}
