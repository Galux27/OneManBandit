using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisposableAnimation : MonoBehaviour {
	/// <summary>
	/// Just a script to run through an array of sprites for whatever animation you want, meant for stationary env effects only. Options for looping, time animations takes, destroying the object on finish & starting on a random sprite 
	/// </summary>
	public Sprite[] spritesInAnim;
	public float timer,timerReset;
	public int counter=0;
	SpriteRenderer sr;
	public bool loop=false,destroyOnFinish=false,startRandom=false;
	void Awake()
	{
		sr = this.GetComponent<SpriteRenderer> ();

	}

	// Use this for initialization
	void Start () {
		if (startRandom == false) {
			sr.sprite = spritesInAnim [0];
		} else {
			counter = Random.Range (0, spritesInAnim.Length);
			sr.sprite = spritesInAnim [counter];
		}
	}
	
	// Update is called once per frame
	void Update () {
		//if (destroyOnFinish == true && counter>=spritesInAnim.Length ) {
		//	Destroy (this);
		//}

		timer -= Time.deltaTime;
		if (timer <= 0) {
			if (counter < spritesInAnim.Length-1) {
				counter++;
				sr.sprite = spritesInAnim [counter];
			} else {
				if (loop == true) {
					counter = 0;
					sr.sprite = spritesInAnim [counter];

				} else {
					if (destroyOnFinish == true) {
						Destroy (this.gameObject);
					}
				}
			}
			timer = timerReset;
		}
	}
}
