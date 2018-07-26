using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour {
	public string myWeapon;
	public Weapon pickupWeapon;
	// Use this for initialization
	void Start () {
		//WeaponStore.me.weaponPickupsInWorld.Add (this.gameObject);
		if (pickupWeapon == null) {
			pickupWeapon = WeaponStore.me.getWeapon (myWeapon);
		}
		if (this.GetComponent<SpriteRenderer> () == null) {
			this.gameObject.AddComponent<SpriteRenderer> ().sprite = pickupWeapon.pickup;
		} else {
			this.gameObject.GetComponent<SpriteRenderer> ().sprite = pickupWeapon.pickup;
		}

		this.gameObject.name = myWeapon;
		Instantiate (pickupWeapon, this.transform);
		transform.GetChild (0).gameObject.SetActive (true);
		ItemMoniter.me.refreshItems ();
		Destroy (this);
	}
	
	// Update is called once per frame
	void Update () {
		if (Vector3.Distance (this.transform.position, CommonObjectsStore.player.transform.position) < 2.0f) {
			if (Input.GetKeyDown (KeyCode.E)) {
				CommonObjectsStore.player.GetComponent<PersonWeaponController> ().setWeapon (pickupWeapon);
				WeaponStore.me.weaponPickupsInWorld.Remove (this.gameObject);
				Destroy (this.gameObject);
			}
		}
	}
}
