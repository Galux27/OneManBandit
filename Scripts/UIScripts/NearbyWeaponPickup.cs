using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NearbyWeaponPickup : MonoBehaviour {
	public static NearbyWeaponPickup me;
	public Text myText;
	void Awake()
	{
		me = this;
	}


	public void setWeapon(Weapon w)
	{
		this.gameObject.SetActive (true);
		myText.text = "(Z) " + w.getItemName ();
		this.transform.position = CommonObjectsStore.me.mainCam.WorldToScreenPoint (w.gameObject.transform.position);
	}

	public void disable()
	{
		this.gameObject.SetActive (false);
	}
}
