using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonAnimationController : MonoBehaviour {
	public string ID;
	SpriteRenderer sr;
	public List<SpriteAnimation> animationsToPlay;
	public SpriteAnimation playing;
	public int counter=0;
	public float timer=0.0f;

	void Awake()
	{
		sr = this.GetComponent<SpriteRenderer> ();
		animationsToPlay = new List<SpriteAnimation> ();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		playAnimation ();
	}

	public bool playingAnimation(string animName)
	{
		return areWePlayingAnimation (ID, animName);
	}

	public bool areWePlayingAnimation(string ID,string animName)
	{
		if (playing == null) {
			return false;
		}

		SpriteAnimation sa = AnimationStore.me.getAnimation (ID, animName);
		if (sa == null) {
		} else {
			if (animationsToPlay.Contains (sa)) {
				return true;
			}
		}

		return playing.animWereLookingFor (ID, animName);
	}

	public void playAnimation(string _AnimName,bool forceFinish)
	{
		return;//added whilst testing the new animation system

		SpriteAnimation anim = AnimationStore.me.getAnimation (ID, _AnimName);
		if (anim == null) {
			//////Debug.Log ("Could not find " + ID + " || " + _AnimName);
		} else {
			animationsToPlay.Add (anim);
		}

		if (forceFinish == true && playing !=null) {
			forceFinishCurrentAnim ();
		}
		playAnimation ();
		counter = 0;
	}

	public void forceFinishCurrentAnim()
	{
		counter = playing.spritesInAnimation.Length - 1;
		timer = 0.0f;
		//////Debug.Break ();
	}

	void playAnimation()
	{
		if (playing == null) {
			if (animationsToPlay.Count == 0) {
				return;
			} else {
				animationsToPlay.Remove (playing);
				playing = animationsToPlay [0];
			}
		}

		timer -= Time.deltaTime;
		sr.sprite = playing.spritesInAnimation [counter];

		if (timer <= 0) {
			if (counter < playing.spritesInAnimation.Length-1) {
				counter++;
				timer = playing.timePerFrame;

			} else {
				if (animationsToPlay.Count > 1) {
					if (animationsToPlay.Contains (playing)) {
						animationsToPlay.Remove (playing);
					}

					playing = animationsToPlay [0];
					counter=0;
					timer = playing.timePerFrame;
				} else {
					counter=0;
					timer = playing.timePerFrame;
				}
			}
		}
	}
}
