using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomClothesGenerator : MonoBehaviour {
	public List<string> clothes1, clothes2, clothes3, clothes4, clothes5;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public List<string> getRandomClothes()
	{
		int r = Random.Range (0, 5);
		if (r == 0) {
			return clothes1;
		} else if (r == 1) {
			return clothes2;
		} else if (r == 2) {
			return clothes3;
		} else if (r == 3) {
			return clothes4;
		} else if (r == 4) {
			return clothes5;
		}
		return clothes1;
	}
}
