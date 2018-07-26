using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour {
	/// <summary>
	/// Old 2d windows, remove at some point. 
	/// </summary>
	public Sprite[] openAnimation;
	public Sprite smashedSprite;
	public SpriteRenderer sr;
	public int counter = 0;
	public float timer = 0.1f;
	public bool open=false;
	public bool smashed=false;
	void Awake()
	{
		sr = this.GetComponent<SpriteRenderer> ();
		if (this.GetComponent<AudioController> () == false) {
			this.gameObject.AddComponent<AudioController> ();
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (smashed == false) {
			if (open == true && counter < openAnimation.Length - 1) {
				openWindow ();
			} else if (open == false && counter > 0) {
				closeWindow ();
			}
		}
	}

	public void setOpen()
	{
		open = true;
	}

	public void setClosed()
	{
		open = false;
	}

	public void openWindow()
	{
		timer -= Time.deltaTime;
		if (timer > 0) {
			return;
		}
		if (counter < openAnimation.Length - 1) {
			counter++;
			sr.sprite = openAnimation [counter];
			sr.color = new Color (1, 1, 1, 1 -  (counter *0.1f));
			timer = 0.1f;
		} 

		if (counter == openAnimation.Length - 1) {
			this.gameObject.GetComponent<Collider2D> ().isTrigger = true;
		}
	}

	public void closeWindow()
	{
		timer -= Time.deltaTime;

		if (timer > 0) {
			return;
		}
		if (counter > 0) {
			counter--;
			sr.sprite = openAnimation [counter];
			sr.color = new Color (1, 1, 1, 1);
			timer = 0.2f;
		} 
		this.gameObject.GetComponent<Collider2D> ().isTrigger = false;

			
	}

	public void smashWindow()
	{
		smashed = true;
		sr.sprite = smashedSprite;
		sr.color = new Color (1, 1, 1, 1);
		sr.sortingOrder = 1;
		//if (counter == openAnimation.Length - 1) {
			this.gameObject.GetComponent<Collider2D> ().enabled = false;
		//}
		this.gameObject.GetComponent<AudioController> ().playSound (SFXDatabase.me.smashGlass);

		foreach (GameObject g in NPCManager.me.npcsInWorld) {
			if (g == null) {
				continue;
			}
			NPCController npc = g.GetComponent<NPCController> ();
			npc.setHearedGunshot (this.transform.position, 14.0f);
		}
		PoliceController.me.setNoiseHeard (this.transform.position, 14.0f);


		PlayerAction pa = this.gameObject.GetComponent<PlayerAction> ();
		Destroy (pa);
		HighlightObjectWithPlayerActions h = this.gameObject.GetComponent<HighlightObjectWithPlayerActions> ();
		Destroy (h);
		SpriteOutline s = this.gameObject.GetComponent<SpriteOutline> ();
		Destroy (s);
	}
}
