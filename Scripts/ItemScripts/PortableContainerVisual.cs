using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortableContainerVisual : MonoBehaviour {
	public SpriteRenderer oneHanded, unarmed;
	public PersonWeaponController pwc;
	public PortableContainerItem myItem;

	void Awake()
	{
		pwc = this.GetComponentInParent<PersonWeaponController> ();
	}


	// Use this for initialization
	void Start () {
		
	}

	void isContainerDropped()
	{
		if (myItem == null) {

		} else {
			if (myItem.gameObject.activeInHierarchy == true) {
				myItem = null;
			}
		}
	}

	// Update is called once per frame
	void Update () {
		drawItem ();
		isContainerDropped ();
	}

	void drawItem()
	{

		if (myItem == null) {
			unarmed.gameObject.SetActive (false);
			oneHanded.gameObject.SetActive (false);

		} else {

			if (pwc.currentWeapon==null) {
				oneHanded.gameObject.SetActive (false);
				unarmed.gameObject.SetActive (true);
				unarmed.sprite = myItem.itemTex;



			} else {
				if (pwc.currentWeapon.oneHanded == true) {
					oneHanded.gameObject.SetActive (true);
					unarmed.gameObject.SetActive (false);
					oneHanded.sprite = myItem.itemTex;
				} else {
					myItem.dropItem ();
				}
			}
		}
	}
}
