using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerInitialiser : MonoBehaviour {
	public string weaponToGet;
	public List<string> itemsToAdd,randomItems;
	Container c;
	void Awake()
	{
		c = this.GetComponent<Container> ();
	}

	// Use this for initialization
	void Start () {
		if (c.initialised == false) {
			createItems ();
			c.initialised = true;
		}
	}

	// Update is called once per frame
	void Update () {

	}

	void createItems()
	{
		if (ItemDatabase.me.getItem (weaponToGet) == null) {

		} else {
			GameObject pistolObj = (GameObject)Instantiate (ItemDatabase.me.getItem (weaponToGet), this.transform.position, this.transform.rotation);

			Weapon w = pistolObj.GetComponent<Weapon> ();
			if (w.melee == false) {
				GameObject ammoObj = (GameObject)Instantiate (ItemDatabase.me.getAmmoItem (w), this.transform.position, this.transform.rotation);
				AmmoItem ai = ammoObj.GetComponent<AmmoItem> ();
				c.addItemToContainer (ai);
			}
			c.addItemToContainer (w);
		}


		foreach (string st in itemsToAdd) {
			if (ItemDatabase.me.getItem (st) == null) {

			} else {
				GameObject obj = (GameObject)Instantiate (ItemDatabase.me.getItem (st), this.transform.position, this.transform.rotation);

				Item i = obj.GetComponent<Item> ();
				c.addItemToContainer (i);
			}
		}

		foreach (string st in randomItems) {
			int r = Random.Range (0, 100);
			if (r < 25) {
				if (ItemDatabase.me.getItem (st) == null) {

				} else {
					GameObject obj = (GameObject)Instantiate (ItemDatabase.me.getItem (st), this.transform.position, this.transform.rotation);

					Item i = obj.GetComponent<Item> ();
					c.addItemToContainer (i);
				}
			}
		}
	}
}
