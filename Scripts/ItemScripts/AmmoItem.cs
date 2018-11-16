using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoItem : Item {
	public List<string> gunsAmmoFitsIn;
	public int ammoCount,maxAmmo;
	public string itemNameBase = "";

	public bool canWeUseAmmoInWeapon(string curWep)
	{
		if (ammoCount == 0) {
			return false;
		}

		foreach (string s in gunsAmmoFitsIn) {
			if (s == curWep) {
				return true;
			}
		}
		return false;
	}

	public void decrementAmmo() //ammo counter stops displaying on 1 & When ammo is dropped it will unequip the weapon the ammo is in
	{
		if (ammoCount > 0) {
			ammoCount--;
		}
	}


	public PersonWeaponController pwc;
	public bool loadingAmmo = false;

	void Update()
	{
		if (loadingAmmo == true) {
			//Debug.Log ("MANUAL RELOAD");
			CommonObjectsStore.pwc.manualReload (this);
		}
	}

	public override string getItemName ()
	{
		return itemNameBase + " (" + ammoCount + "/" + maxAmmo + ")";
	}

	public override void useItem ()
	{
		Weapon w =CommonObjectsStore.pwc.currentWeapon;
		Inventory i = CommonObjectsStore.player.GetComponent<Inventory> ();

		if (w == null) {
			//Debug.Log ("Weapon was null");
		} else {
			if (canWeUseAmmoInWeapon (w.WeaponName) == true && ammoCount>0 && CommonObjectsStore.pwc.currentWeapon.ammoItem!=this) {
				loadingAmmo = true;
				this.gameObject.SetActive (true);
				this.gameObject.GetComponent<SpriteRenderer> ().enabled = false;
				//Debug.Log ("CAN REALOD");
			} else {
				//Debug.Log ("CANT RELOAD");
			}
		}
	}




}
