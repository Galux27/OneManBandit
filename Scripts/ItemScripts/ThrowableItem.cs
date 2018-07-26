using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableItem : Item {

	public float fuseLength = 5.0f;
	public Sprite inHand,thrown;
	public bool destroyOnExplode = false;
	public bool holdingMouse = false;
	public float hold = 5.0f;
	Rigidbody2D myRid;
	public GameObject toCreateOnDetonation;
	public bool hasBeenThrown=false;
	bool created = false;
	public ArtemAnimationController ac;
	public GameObject thrower;
	public bool isSmoke = false,detonateOnImpact=false;
	Vector3 spawnPos;
	public AudioClip impactSFX;




	public override void equipItem()
	{
		//equips item to relevant slot
		//this.gameObject.GetComponentInParent<PersonWeaponController>().setWeapon(this);
		myRid=this.GetComponent<Rigidbody2D>();
		Inventory i = this.gameObject.GetComponentInParent<Inventory> ();

		i.equipItem (this, slot);
	//	ammoItem = i.getAmmoForGun (WeaponName);
		this.gameObject.GetComponent<SpriteRenderer> ().sprite = inHand;
		this.gameObject.SetActive (true);
		ac = this.gameObject.transform.root.gameObject.GetComponentInChildren<ArtemAnimationController> ();
		this.gameObject.transform.parent = ac.rightHand.transform;
		//this.gameObject.transform.position = ac.rightHand.transform.position;
		//this.gameObject.transform.localPosition = ac.myAesthetic.throwableOffset;
		ac.setHoldingThrowable ();
		this.gameObject.SetActive (true);
		inUse = true;
		this.transform.rotation = ac.rightHand.transform.rotation;
		this.GetComponent<SpriteRenderer> ().sortingOrder = 3;
		//////Debug.Log ("Root is " +  this.transform.root.gameObject.name);
		//ac = this.transform.root.gameObject.GetComponentInChildren<AnimationController> ();
		thrower = this.transform.parent.root.gameObject;
		this.gameObject.GetComponent<Collider2D> ().enabled = false;
		spawnPos = this.transform.root.position;
		myRid.isKinematic = true;
		myRid.freezeRotation = true;
	}

	public override void unequipItem()
	{
		Inventory myInv = this.transform.root.GetComponent<Inventory> ();
		//this.gameObject.GetComponentInParent<PersonWeaponController>().setWeapon(null);
		//this.gameObject.GetComponent<SpriteRenderer> ().sprite =pickup;
		//ArtemAnimationController ac = this.gameObject.transform.root.gameObject.GetComponentInChildren<ArtemAnimationController> ();
		//if (ac == null) {

		//} else {
			this.gameObject.transform.parent = ac.gameObject.transform.root;

		//}
		inUse = false;
		hasBeenThrown = false;
		this.GetComponent<SpriteRenderer> ().sortingOrder = 1;
		this.gameObject.GetComponent<Collider2D> ().enabled = false;
		ThrowableUI.me.displayUI = false;
		ac.setThrowablePutAway ();
		if (slot == itemEquipSlot.bothHands) {
			if (myInv.leftArm == null) {

			} else {
				myInv.leftArm = null;
				if (myInv == Inventory.playerInventory) {
					InventoryUI.me.leftArm.resetGearSlot ();
				}
			}

			if (myInv.rightArm == null) {

			} else {
				myInv.rightArm = null;
				if (myInv == Inventory.playerInventory) {
					InventoryUI.me.rightArm.resetGearSlot ();
				}
			}
		} else if(slot==itemEquipSlot.rightHand){
			myInv.rightArm = null;
			if (myInv == Inventory.playerInventory) {
				InventoryUI.me.rightArm.resetGearSlot ();
			}
		}
		else if(slot==itemEquipSlot.leftHand){
			myInv.leftArm = null;
			if (myInv == Inventory.playerInventory) {
				InventoryUI.me.leftArm.resetGearSlot ();
			}
		}
		this.gameObject.SetActive (false);

	}

	void Update()
	{
		if (ac == null) {
			if (transform.parent == null) {
				return;
			} else if (transform.root.tag == "Player" || transform.root.tag == "NPC") {
				ac = transform.root.GetComponentInChildren<ArtemAnimationController> ();
			} else {
				return;
			}
		}


		if (inUse == true) {
			if (ac == null) {
				ac = this.transform.root.gameObject.GetComponentInChildren<ArtemAnimationController> ();
			}

			if (hasBeenThrown == false) {
				this.transform.position = ac.rightHand.transform.position;

				//ac.setThrowableThrown ();

			} 
		}


		if (inUse == true && hasBeenThrown==false && Inventory.playerInventory.inventoryGUI.activeInHierarchy == false) {
			ThrowableUI.me.setLine (this.transform.root, hold);


			if (Input.GetMouseButtonDown (1)) {
				ac.switchThrow ();
			}

			if (ac.shortThrown == false) {
				hold = 12;

			} else {
				hold = 6;

			}


			if (Input.GetMouseButtonUp (0)) {
				ac.setThrowableThrown ();
				throwItem ();
			}

			//if (ac.thrownAnimDone == true) {
			//}

		}

	

		if (hasBeenThrown == true) {
			ThrowableUI.me.displayUI = false;
			if (Vector3.Distance (this.transform.position, thrower.transform.position) > 1.25f) {
				this.GetComponent<Collider2D> ().enabled = true;
			}
			if (detonateOnImpact == false) {
				countDownFuse ();
			} else {
				detonateOnStop ();
			}
		} 
	}

	public void throwItem()
	{

		Inventory myInv = this.gameObject.GetComponentInParent<Inventory> ();
		this.gameObject.GetComponent<SpriteRenderer> ().sprite = thrown;

		if (myRid == null) {
			myRid = this.gameObject.GetComponent<Rigidbody2D> ();
		}
		myRid.freezeRotation=false;
		myRid.isKinematic = false;

		myRid.AddForce (this.transform.root.up  * hold,ForceMode2D.Impulse);

		this.gameObject.transform.parent = null;

		//this.gameObject.GetComponent<Collider2D> ().enabled = true;
		hasBeenThrown = true;

	//	if (this.gameObject.tag == "Player") {
			myInv.rightArm = null;
			if (myInv == Inventory.playerInventory) {
				InventoryUI.me.rightArm.resetGearSlot ();
			}
		//}
		//ac.thrown ();
		myInv.removeItemWithoutDrop (this);
		//this.gameObject.GetComponent<Collider2D> ().enabled = false;
		//unequipItem ();
	}

	void countDownFuse()
	{
		if (Vector3.Distance (this.transform.position, thrower.transform.position) > 1.5f) {
			this.gameObject.GetComponent<Collider2D> ().enabled = true;

		}

		fuseLength -= Time.deltaTime;
		if (fuseLength <= 0) {
			detonate ();
		}
	}

	public void detonate()
	{
		

		if (created == false) {
			GameObject g = Instantiate (toCreateOnDetonation, this.transform.position, this.transform.rotation);

			if (isSmoke == true) {
				SmokeEffect s = g.GetComponent<SmokeEffect> ();
				s.myManager = this.gameObject.GetComponent<SmokeManager> ();
				this.GetComponent<SmokeManager> ().smokeOriginPoint = this.transform.position;
			}
			if (impactSFX == null) {

			} else {
				GameObject g2 = new GameObject();
				g2.transform.position = this.transform.position;
				AudioController au2 = g2.AddComponent<AudioController>();
				au2.playSound(impactSFX);


			}
			created = true;
		}
		if (destroyOnExplode == true) {
			Destroy (this.gameObject);
		}
	}

	void detonateOnStop()
	{
		if (myRid.velocity.magnitude < 0.5f) {
			detonate ();
		}
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		Debug.Log ("Molotov hit " + col.collider.name);
		if (col == null) {

		} else {
			if (col.gameObject.tag != "Player") {
				if (detonateOnImpact == true) {
					detonate ();
				}
			}
		}
	}

}

