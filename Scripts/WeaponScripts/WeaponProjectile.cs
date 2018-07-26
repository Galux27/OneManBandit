using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProjectile : MonoBehaviour {
	Rigidbody2D rid;
	BoxCollider2D bc2d;

	float timer = 5.0f;
	public int damage = 300;
	// Use this for initialization
	void Start () {
		rid = this.gameObject.AddComponent<Rigidbody2D> ();
		bc2d = this.gameObject.AddComponent<BoxCollider2D> ();
		rid.gravityScale = 0;
		rid.isKinematic = false;
		rid.drag = 1.5f;
		rid.AddForce (transform.up * 8, ForceMode2D.Impulse);
//		this.gameObject.GetComponent<WeaponPickup> ().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;

		this.transform.rotation = Quaternion.Euler( new Vector3 (0, 0, timer * rid.velocity.magnitude));


		if (timer <= 0 || rid.velocity.magnitude < 1) {
			stopMovement ();
		}
	}

	void stopMovement()
	{
		rid.velocity = Vector2.zero;
		Destroy (rid);
		Destroy (bc2d);
		//	this.GetComponent<WeaponPickup> ().enabled = true;
		Destroy (this);
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject == null) {

		} else {
			stopMovement ();
			//////Debug.Log ("WEAPON HIT SOMETHING" + col.gameObject.name);
			if (col.gameObject.tag == "Player") {
				CameraController.me.hitByBullet (new Vector2 (this.transform.position.x, this.transform.position.y));
			}

			PersonHealth ph = col.gameObject.GetComponent<PersonHealth> ();
			if (ph == null) {

			} else {
				ph.dealDamage (damage,false);
				//stun npc

				if (ph.healthValue > 0 && this.gameObject.tag=="NPC") {
					NPCController npc = col.collider.gameObject.GetComponent<NPCController> ();
					npc.knockOutNPC ();
				}

				Inventory i = ph.gameObject.GetComponent<Inventory> ();
				if (i.leftArm == null) {

				} else {
					Item toDrop = i.leftArm;
					i.unequipItem (toDrop);

					i.dropItem (toDrop);
				}

				if (i.rightArm == null) {

				} else {
					Item toDrop = i.rightArm;
					i.unequipItem (toDrop);

					i.dropItem (toDrop);
				}
			}

			//Destroy (this.gameObject);
		}
	}
}
