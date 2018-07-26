using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapItem : Item {

	public GameObject prefab;
	public ThrowableItem ti;
	float fuseLength;
	public bool created = false,destroyOnExplode=false,placed=false;
	Vector3 endPointOfWire;
	LineRenderer lr;
	SpriteRenderer sr;
	void Start()
	{
		ti = prefab.GetComponent<ThrowableItem> ();
		lr = this.GetComponent<LineRenderer> ();
		fuseLength = ti.fuseLength;
		sr = this.gameObject.GetComponent<SpriteRenderer> ();
	}

	public override void equipItem()
	{
		//equips item to relevant slot
		//this.gameObject.GetComponentInParent<PersonWeaponController>().setWeapon(this);

		Inventory i = this.gameObject.GetComponentInParent<Inventory> ();
		placed = false;
		i.equipItem (this, slot);
		this.gameObject.SetActive (true);
		this.gameObject.GetComponent<SpriteRenderer> ().sprite = ti.inHand;
		this.GetComponent<SpriteRenderer> ().sortingOrder = 3;

		//	ammoItem = i.getAmmoForGun (WeaponName);
		/*

		
		this.gameObject.SetActive (true);
		inUse = true;
		this.GetComponent<SpriteRenderer> ().sortingOrder = 3;
		//////Debug.Log ("Root is " +  this.transform.root.gameObject.name);*/
		AnimationController ac = this.gameObject.transform.parent.gameObject.GetComponentInChildren<AnimationController> ();
		ac = this.transform.root.gameObject.GetComponentInChildren<AnimationController> ();
		this.gameObject.transform.parent = ac.rightHand.transform;
		this.gameObject.transform.position = ac.rightHand.transform.position;
		this.transform.rotation = ac.rightHand.transform.rotation;
		if (i == Inventory.playerInventory) {
			inUse = true;
		} 

	}

	public override void unequipItem()
	{
		Inventory myInv = this.gameObject.GetComponentInParent<Inventory> ();
		//this.gameObject.GetComponentInParent<PersonWeaponController>().setWeapon(null);
		//this.gameObject.GetComponent<SpriteRenderer> ().sprite =pickup;
		AnimationController ac =  this.gameObject.transform.parent.transform.parent.gameObject.GetComponentInChildren<AnimationController> ();

		this.gameObject.transform.parent = ac.gameObject.transform.parent;
		this.gameObject.SetActive (false);
		inUse = false;

		this.GetComponent<SpriteRenderer> ().sortingOrder = 1;

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
	}

	void Update()
	{
		if (inUse == true) {
			if (placed == false) {
				if (validTrap ()) {
					//////Debug.Log ("Trap placement is valid");
					if (Input.GetMouseButtonDown (0)) {
						Inventory myInv = this.gameObject.GetComponentInParent<Inventory> ();
						if (myInv == null || myInv != Inventory.playerInventory) {
						} else {
							endPointOfWire = CommonObjectsStore.me.mainCam.ScreenToWorldPoint (Input.mousePosition);


							placed = true;
							inUse = false;

							myInv.leftArm = null;
							if (myInv == Inventory.playerInventory) {
								InventoryUI.me.leftArm.resetGearSlot ();
							}
							myInv.removeItemWithoutDrop (this);


							this.gameObject.transform.parent = null;
							this.gameObject.transform.position = CommonObjectsStore.player.transform.position + (CommonObjectsStore.player.transform.up / 2);
							this.gameObject.GetComponent<SpriteRenderer> ().sprite = ti.inHand;
						}
					}
				}
			}
		} else {
			if (placed == true) {

					if (hasTrapBeenTriggered () == true) {
						//////Debug.Log ("Trap triggered");
						detonate ();
					} else {

						//////Debug.Log ("Trap has not been triggered");
					}

			}
		}
	
	}

	void OnEnable()
	{
		this.GetComponent<SpriteRenderer> ().sprite = itemTex;
	}

	void OnDisable()
	{
		placed = false;
		List<Vector3> pos = new List<Vector3> ();
		pos.Add (this.transform.position);
		pos.Add (this.transform.position);

		if (lr == null) {

		} else {

			lr.SetPositions (pos.ToArray ());
			lr.startColor = Color.clear;
			lr.endColor = Color.clear;
		}
	}

	bool hasTrapBeenTriggered()
	{

		Vector3 origin = this.transform.position;

		List<Vector3> pos = new List<Vector3> ();

		pos.Add (origin);
		pos.Add (endPointOfLine);
		lr.SetPositions (pos.ToArray ());
		lr.endColor = Color.grey;
		lr.startColor = Color.grey;

		RaycastHit2D ray = Physics2D.Raycast (origin, dirOfLine,lineDist);

		if (ray.collider == null) {
			//			//////Debug.Log ("No ray hit");
			Debug.DrawRay (origin,dirOfLine*lineDist,Color.green);

			return false;
		} else {

			//if (ray.collider.gameObject.tag == "WallCollider") {
			//	//////Debug.Log ("We hit a wall " + ray.collider.gameObject.name);
			//}

			////////Debug.Log (ray.collider.gameObject.name);
			if (ray.collider.gameObject.tag == "NPC" || ray.collider.gameObject == CommonObjectsStore.player) {
				Debug.DrawRay (origin, dirOfLine*lineDist,Color.red);

				return true;
			} else {
				Debug.DrawRay (origin, dirOfLine*lineDist,Color.green);

				return false;
			}
		}
	}

	Vector3 endPointOfLine = Vector3.zero;
	Vector3 dirOfLine = Vector3.zero;
	float lineDist = 0.0f;
	public bool lineOfSightToTargetWithNoCollider(){

		Vector3 mouse =CommonObjectsStore.me.mainCam.ScreenToWorldPoint (Input.mousePosition);

		List<Vector3> pos = new List<Vector3> ();

		Vector3 origin = CommonObjectsStore.player.transform.position;
		Vector3 dir = CommonObjectsStore.player.transform.up;
		dirOfLine = dir;
		RaycastHit2D[] rays = Physics2D.RaycastAll (origin, dir,5);
		foreach (RaycastHit2D ray in rays) {
			if (ray.collider == null || ray.collider.gameObject == CommonObjectsStore.player) {
				//			//////Debug.Log ("No ray hit");
				//Debug.DrawRay (origin, dir * 5, Color.green);
				//lr.endColor = Color.green;
				//lr.startColor = Color.green;
				//return true;
				endPointOfLine = origin + (dir*5);
				lineDist = 5.0f;
			} else {
				Debug.LogError (ray.collider.gameObject.name + " was in the way of the ray");
				lr.endColor = Color.red;
				lr.startColor = Color.red;
				Debug.DrawRay (origin, dir * ray.distance, Color.green);
				endPointOfLine = ray.point;
				lineDist = ray.distance;
				pos.Add (origin);
				pos.Add (endPointOfLine);
				lr.SetPositions (pos.ToArray ());
				lr.endColor = Color.green;
				lr.startColor = Color.green;

				//if (ray.collider.gameObject.tag == "WallCollider") {
				//	//////Debug.Log ("We hit a wall " + ray.collider.gameObject.name);
				//}

				////////Debug.Log (ray.collider.gameObject.name);

				return true;
			}
		}
		lineDist = 5.0f;

		pos.Add (origin);
		pos.Add (endPointOfLine);
		lr.SetPositions (pos.ToArray ());
		lr.endColor = Color.green;
		lr.startColor = Color.green;
		Debug.DrawRay (origin, dir * 5, Color.green);
		return true;
	}
	bool validTrap()
	{
		return lineOfSightToTargetWithNoCollider ();
	}

	void countDownFuse()
	{
		fuseLength -= Time.deltaTime;
		if (fuseLength <= 0) {
			detonate ();
		}
	}

	public void detonate()
	{


		if (created == false) {
			Instantiate (ti.toCreateOnDetonation, this.transform.position, this.transform.rotation);
			created = true;
		}
		if (destroyOnExplode == true) {
			Destroy (this.gameObject);
		}
	}

}
