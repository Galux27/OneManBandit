using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class for weapons. 
/// </summary>
public class Weapon : Item {

	public string WeaponName = "";
	public Sprite pickup,holding; //replace these with animations once I've written an animation system, may have to make it strings instead so that multiple 
	public float fireRate;
	public int clipCapacity;
	public bool oneHanded,melee;
	public GameObject projectile;
	public float recoilMax;
	public AmmoItem ammoItem;
	public int meleeDamage;
	public bool bladed = false;
	public float reloadTime;
	public Transform bulletSpawnPos;

	/// <summary>
	/// an animation to play when attacking e.g. flash comming of the barral of a gun. 
	/// </summary>
	public Sprite[] animation;
	public bool playMeleeAnim = false;
	public float meleeTimer = 0.05f;
	public int meleeCounter=0;
	public float resetTime = 0.05f;
	public float noiseRange = 5.0f;
	public AudioClip attackNoise,gunEmpty;

	/// <summary>
	/// Value for the Animation controller that decides the animation to play whilst holding the weapon. 
	/// </summary>
	public int gunAnimValue=0;
	void animate()
	{
		if (animation.Length == 0 || animation [meleeCounter]==null ) {
			return;
		}

		meleeTimer -= Time.deltaTime;
		if (meleeTimer <= 0) {
			this.gameObject.GetComponent<SpriteRenderer> ().sprite = animation [meleeCounter];
			if (meleeCounter < animation.Length - 1) {
				meleeCounter++;
			} else {
				meleeCounter = 0;
				playMeleeAnim = false;
			}
			meleeTimer = resetTime;
		}
	}




	public override void equipItem ()
	{
		this.gameObject.GetComponentInParent<PersonWeaponController>().setWeapon(this);

		Inventory i = this.gameObject.GetComponentInParent<Inventory> ();

		i.equipItem (this, slot);
		ammoItem = i.getAmmoForGun (WeaponName);
		this.gameObject.GetComponent<SpriteRenderer> ().sprite = holding;
		this.gameObject.SetActive (true);
		ArtemAnimationController ac = this.gameObject.transform.root.gameObject.GetComponentInChildren<ArtemAnimationController> ();
		if (melee == true) {
			this.transform.rotation = Quaternion.Euler (0, 0,ac.rightHand.transform.rotation.eulerAngles.z + 35);
		} else {
			this.transform.rotation = ac.rightHand.transform.rotation;
		}
		this.gameObject.transform.parent = ac.rightHand.transform;
		this.gameObject.transform.position = ac.rightHand.transform.position;

		this.gameObject.transform.localPosition = ac.myAesthetic.handgunOffset;

		this.gameObject.SetActive (true);
		inUse = true;
		this.GetComponent<SpriteRenderer> ().sortingOrder =3;

	}

	void Update()
	{
		if (playMeleeAnim == true) {
			animate ();
		}
	}

	public override void unequipItem()
	{
		if (this.transform.parent == null) {
			return;
		}

		Inventory myInv = this.gameObject.GetComponentInParent<Inventory> ();
		this.gameObject.transform.root.gameObject.GetComponent<PersonWeaponController>().setWeapon(null);
		this.gameObject.GetComponent<SpriteRenderer> ().sprite =pickup;
		ArtemAnimationController ac = this.gameObject.transform.root.gameObject.GetComponentInChildren<ArtemAnimationController> ();

		this.gameObject.transform.parent = ac.gameObject.transform.parent;
		this.gameObject.SetActive (false);
		//inUse = false;
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
}
