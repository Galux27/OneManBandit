using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadController : MonoBehaviour {
	public string animationName = "HeadDefault";
	Rigidbody2D rid;
	public SpriteAnimation headAnimation;
	public Sprite defaultHead;
	void Awake()
	{
		rid = this.gameObject.GetComponent<Rigidbody2D> ();
		sr = this.GetComponent<SpriteRenderer> ();
	}

	// Use this for initialization
	void Start () {
		//if (headAnimation == null) {
		//	headAnimation = AnimationStore.me.getAnimation (this.gameObject.GetComponentInParent<PersonAnimationController> ().ID, animationName);
		//}
	}
	
	// Update is called once per frame
	void Update () {
		//playAnimation ();
	}

	public void rotateToFacePosition(Vector3 pos)
	{
		Vector3 rot = new Vector3(0, 0, Mathf.Atan2((pos.y - transform.position.y),pos.x - transform.position.x)) * Mathf.Rad2Deg;
		rot = new Vector3(rot.x, rot.y, rot.z-90);//add 90 to make the player face the right way (yaxis = up)
		rid.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.Euler (rot), 6.5f * Time.deltaTime);// Quaternion.Euler(rot); //(INSTA ROTATION)
	}

	float timer = 0.0f;
	int counter = 0;
	SpriteRenderer sr;
	void playAnimation()
	{
		if (headAnimation == null) {
			return;
		}
		timer -= Time.deltaTime;
		sr.sprite = headAnimation.spritesInAnimation [counter];

		if (timer <= 0) {
			if (counter < headAnimation.spritesInAnimation.Length-1) {
				counter++;
				timer = headAnimation.timePerFrame;

			} else {
				counter=0;
				timer = headAnimation.timePerFrame;

			}
		}
	}

}
