using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStore : MonoBehaviour {
	/// <summary>
	/// Redundent, moved all items to the item database, remove at some point.
	/// </summary>

	public static WeaponStore me;
	public List<Weapon> weapons;
	public List<GameObject> weaponPickupsInWorld;
	void Awake(){
		me = this;
	}

	public Weapon getWeapon(string name)
	{
		foreach (Weapon w in weapons) {
			if (w.WeaponName == name) {
				return w;
			}
		}
		return null;
	}

	//need to fix these, maybe just instatiate instance of the weapon passed in?? will need to get rid of the stuff in weapon pickup if so

	public void createWeaponPickup(Weapon w,Vector3 position)
	{
		if (w.name == "Unarmed") {
			return;
		}
			GameObject g = Instantiate(w.gameObject,null);
			g.transform.position = position;
			SpriteRenderer sr = g.GetComponent<SpriteRenderer> ();
			sr.sprite = w.itemTex;
			g.SetActive (true);
			g.transform.parent = null;
		/*GameObject g = new GameObject ();
		g.transform.position = position;
		SpriteRenderer sr = g.AddComponent<SpriteRenderer> ();
		WeaponPickup wp = g.AddComponent<WeaponPickup> ();
		wp.myWeapon = w.WeaponName;

		wp.pickupWeapon = w;
		sr.sprite = w.pickup;*/
	}

	public void createWeaponProjectile(Weapon w,Vector3 position,Quaternion rotation)
	{
		if (w.name == "Unarmed") {
			return;
		}
		GameObject g = Instantiate(w.gameObject,null);
		g.transform.position = position;
		g.transform.rotation = rotation;
		g.SetActive (true);
		g.transform.parent = null;
		SpriteRenderer sr = g.GetComponent<SpriteRenderer> ();
		//WeaponPickup wp = g.AddComponent<WeaponPickup> ();
		//wp.myWeapon = w.WeaponName;
		g.AddComponent<WeaponProjectile> ();
		//wp.pickupWeapon = w;
		sr.sprite = w.pickup;
	}

}
