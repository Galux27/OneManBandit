using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for bullet based projectiles
/// </summary>
public class Bullet : MonoBehaviour {
	public int damage = 1;
	public bool isAiBullet=false,isShrapnal=false;
	public GameObject shooter;
	Rigidbody2D rid;
	void Awake()
	{
		rid = this.GetComponent<Rigidbody2D> ();
	}


	void Update () {
		if (Vector3.Distance (this.transform.position, CommonObjectsStore.player.transform.position) < 75.0f) {
			rid.velocity = new Vector2(transform.up.x * 20,transform.up.y*20);
		} else {
			Destroy (this.gameObject);
		}
	}


	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject == null) {

		} else {
			if (col.gameObject.tag == "Door") {
				DoorScript ds = col.gameObject.GetComponent<DoorScript> ();
				if (ds == null) {
				} else {

					ds.kickInDoor ();
					Instantiate (CommonObjectsStore.me.bulletImpact, this.transform.position - (transform.up / 4), this.transform.rotation);
					Destroy (this.gameObject);
				}
				//if (ds.wayIAmLocked == lockedWith.none || ds.wayIAmLocked == lockedWith.key) {
				//	ds.locked = false;
				//	ds.interactWithDoor (this.gameObject);
				//}
			} else if (col.gameObject.tag == "Window") {
				Window w = col.gameObject.GetComponent<Window> ();
				if (w == null) {

				} else {
					w.smashWindow ();
					return;
				}

				WindowNew w2 = col.gameObject.GetComponentInParent<WindowNew> ();
				if (w2 == null) {

				} else {
					w2.destroyWindow ();
				}
			} else if (col.gameObject.GetComponent<PlayerCarController> () == true) {
				col.gameObject.GetComponent<PlayerCarController> ().dealDamage (damage);

				if (col.gameObject.GetComponent<SpawnCopsOnCall> () == true) {
					col.gameObject.GetComponent<SpawnCopsOnCall> ().spawnCops ();
					if (PoliceController.me.copsCalled == false) {
						PoliceController.me.copsCalled = true;
					}
				}
			}

			if (col.gameObject.tag != "Bullet") {

				if (col.gameObject.tag == "NPC" && isAiBullet==false || col.gameObject.tag == "Player"&&isAiBullet==true) {
					Instantiate (CommonObjectsStore.me.bloodImpact,this.transform.position-(transform.up/4), this.transform.rotation);

				} 

				if(col.transform.root.tag!="Player" && col.transform.root.tag!="NPC"){
					Instantiate (CommonObjectsStore.me.bulletImpact,this.transform.position-(transform.up/4), this.transform.rotation);
				}

				Destroy (this.gameObject);

			}
		}
	}


}
