using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour {
	/// <summary>
	/// Animations controler for explosions, seperate from disposable animations so we can play a sound effect, deal damage, set sounds heard for NPCs and fade out the smoke after the explosion 
	/// </summary>

	public Sprite[] mySprites;
	public SpriteRenderer sr;

	public float timer = 0.01f;

	public float maxSize;
	public float explosionSize = 50.0f;
	public AudioClip explosionSfx;
	int counter = 0;
	// Use this for initialization
	void Start () {
		sr = this.gameObject.GetComponent<SpriteRenderer> ();
		this.gameObject.AddComponent<AudioController> ();
		this.gameObject.GetComponent<AudioController> ().playSound (explosionSfx);

		alertNPCs ();

	}

	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;
		if (timer <= 0) {
			if (counter<mySprites.Length-1) {
				//this.transform.localScale = new Vector3 (this.transform.localScale.x + 0.1f, this.transform.localScale.y + 0.1f, 1);
				sr.sprite = mySprites [counter];
				counter++;
				timer = 0.01f;
			} else {
				float a = sr.color.a;
				a -= 0.05f;
				sr.color = new Color (a, a, a, a);
				sr.sprite = mySprites [counter];

				if (a <= 0) {
					Destroy (this.gameObject);
				}



				timer = 0.01f;
			}
		}
	}

	void alertNPCs()
	{
		foreach (NPCController npc in NPCManager.me.npcControllers) {
			npc.setHearedGunshot (this.transform.position, explosionSize);
		}
		PoliceController.me.setNoiseHeard (this.transform.position,explosionSize);

	}

	void OnTrigger2DEnter(Collider2D other)
	{
		PersonHealth ph = other.gameObject.transform.parent.GetComponent<PersonHealth> ();
		if (ph == null) {

		} else {
			ph.dealDamage (2500,true);
		}
	}
}
