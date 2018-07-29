using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class that used to control the legs of human characters in the game, no longer used. 
/// </summary>
public class Legs : MonoBehaviour {



	public string animationToPlay = "Walk";
	public SpriteAnimation myLegs;
	public legState myState;
	public Sprite sitting;
	void Awake()
	{
		sr = this.gameObject.GetComponent<SpriteRenderer> ();
	}

	void Start()
	{
		if (myLegs == null) {
			myLegs= AnimationStore.me.getAnimation (this.GetComponentInParent<PersonAnimationController> ().ID, animationToPlay);
		}
	}

	void Update()
	{
		if (myState == legState.walking) {

		} else {
			this.GetComponent<SpriteRenderer> ().sprite = sitting;
		}
	}

	float timer = 0.0f;
	int counter = 0;
	SpriteRenderer sr;
	public void playAnimation()
	{
		//if (myState != legState.walking) {
		//	return;
		//}
		myState=legState.walking;
		timer -= Time.deltaTime;
		sr.sprite = myLegs.spritesInAnimation [counter];

		if (timer <= 0) {
			if (counter < myLegs.spritesInAnimation.Length-1) {
				counter++;
				timer = myLegs.timePerFrame;

			} else {
				counter=0;
				timer = myLegs.timePerFrame;

			}
		}
	}
}

public enum legState{
	walking,
	sitting,
	kneeling
}
