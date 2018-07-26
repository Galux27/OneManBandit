using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonColliderDecider : MonoBehaviour {
	public Collider2D twoHandADS,normal,down;
	PersonWeaponController pwc;
	NPCController npc;
	bool isNPC = false;
	void Awake()
	{
		//twoHandADS = this.GetComponent<BoxCollider2D> ();
		//normal = this.GetComponent<PolygonCollider2D> ();
		pwc = this.GetComponent<PersonWeaponController> ();

		if (this.gameObject.GetComponent<NPCController> () == true) {
			isNPC = true;
			npc = this.gameObject.GetComponent<NPCController> ();
		} else {
			isNPC = false;
		}
		CapsuleCollider2D[] cols = this.gameObject.GetComponents<CapsuleCollider2D> ();
		foreach (CapsuleCollider2D c in cols) {
			if (c.direction == CapsuleDirection2D.Vertical) {
				twoHandADS = c;
			} else {
				normal = c;
			}
		}
	}

	void Update()
	{
		if (this.gameObject.tag == "Dead/Knocked") {
			if (down == null) {
				down = this.gameObject.AddComponent<CircleCollider2D> ();
				down.enabled = true;
				down.isTrigger = true;
				disableColliders ();
				return;
			} else {
				return;
			}

		} else {
			if (down == null) {

			}
			else{
				down.enabled = false;
			}
		}

		if (isNPC == true) {
			if (npc.npcB.myType == AIType.hostage && npc.head.activeInHierarchy == false) {
				if (down == null) {
					down = this.gameObject.AddComponent<CircleCollider2D> ();
					down.enabled = true;
					down.isTrigger = true;
					disableColliders ();
					return;
				} else {
					down.enabled = true;
					disableColliders ();

					return;
				}
			}
		}

		if (pwc.currentWeapon == null) {
			twoHandADS.enabled = false;
			normal.enabled = true;
		} else {
			if (pwc.currentWeapon.slot == itemEquipSlot.bothHands) {
				twoHandADS.enabled = true;
				normal.enabled = false;
			} else {

				twoHandADS.enabled = false;
				normal.enabled = true;
			}
		}
	}

	public void setTrigger()
	{
		twoHandADS.isTrigger = true;
		normal.isTrigger = true;
	}

	public void setSolid()
	{
		twoHandADS.isTrigger = false;
		normal.isTrigger = false;
	}

	public void disableColliders()
	{
		twoHandADS.enabled = false;
		normal.enabled = false;
	}



	public void onDeath()
	{
		disableColliders ();
		if (down == null) {
			down = this.gameObject.AddComponent<CircleCollider2D> ();
			down.isTrigger = true;
		} else {
			down.enabled = true;
			down.isTrigger = true;
		}
		this.enabled = false;
	}
}
