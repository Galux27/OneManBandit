using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemMoniter : MonoBehaviour {

	/// <summary>
	///  Moniters all instances items in a level & sets items to be displayed in the container UI
	/// </summary>

	public static ItemMoniter me;
	public List<Item> itemsInWorld;
	// Use this for initialization
	public List<ItemUI> myItemUI;
	public List<Item> nearbyItems;
	public int nearIndex = 0;
	public Container[] containersInWorld;
	public bool displayContainer = false;
	public Container toDisplay;

	void Awake()
	{
		me = this;
		refreshItems ();

	}

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		nearbyItemDisplay ();
		nearbyWeaponPickup ();
		drawItemUI ();
		//refreshItemCountdown ();
		//openContainer ();
	}

	public void refreshContainers()
	{
		containersInWorld = FindObjectsOfType<Container> ();
	}

	public void refreshItems()
	{
		itemsInWorld = new List<Item> ();
		Item[] items = FindObjectsOfType<Item> ();

		foreach (Item i in items) {
			if (i.gameObject.activeInHierarchy == true) {
				itemsInWorld.Add (i);
			}
		}

	}
	float refreshTimer = 1.0f;

	void refreshItemCountdown()
	{
		refreshTimer -= Time.deltaTime;
		if (refreshTimer <= 0) {
			refreshItems ();
			refreshContainers ();
			refreshTimer = 1.0f;
		}
	}


	void nearbyItemDisplay()
	{
		//should add some kind of condition for checking if there is an object in the way
		if (displayContainer == false) {
			InventoryUI.me.containerName.text = "Nearby Items";

			nearbyItems = new List<Item> ();
			foreach (Item i in itemsInWorld) {

				if (i == null) {
					refreshItems ();
					return;
				}
				if (i.inUse == false && i.gameObject.activeInHierarchy==true) {
					float distance = Vector3.Distance (CommonObjectsStore.player.transform.position, i.gameObject.transform.position);

					if (distance <= 2.0f) {
						nearbyItems.Add (i);
					}
				}
			}
		} else {
			if (Vector3.Distance (CommonObjectsStore.player.transform.position, toDisplay.gameObject.transform.position) > 2.0f) {
				displayContainer = false;
			}
			InventoryUI.me.containerName.text = toDisplay.containerName;
			nearbyItems = toDisplay.itemsInContainer;
		}
	}


	PersonWeaponController pwc;
	void nearbyWeaponPickup()
	{
		if (pwc == null) {
			pwc = CommonObjectsStore.player.GetComponent<PersonWeaponController> ();
		}
		List<Weapon> nearbyWeapons = new List<Weapon> ();


		if (CommonObjectsStore.player.GetComponent<PersonWeaponController> ().currentWeapon == null) {
			foreach (Item i in nearbyItems) {
				if (i.GetComponent<Weapon> () == true) {
					nearbyWeapons.Add (i.GetComponent<Weapon> ());
				}
			}

			Weapon nearest = null;
			float dist = 999999.0f;
			foreach (Weapon w in nearbyWeapons) {
				if (w.gameObject.activeInHierarchy == true && w.transform.parent == null && w.myContainer == null) {
					float d = Vector3.Distance (w.gameObject.transform.position, CommonObjectsStore.player.transform.position);
					if (d < dist) {
						nearest = w;
						dist = d;
					}
				}
			}

			if (nearest == null) {
				NearbyWeaponPickup.me.disable ();
			} else {
				NearbyWeaponPickup.me.setWeapon (nearest);
				if (Input.GetKeyDown (KeyCode.Z)) {
					if (Inventory.playerInventory.canWeCarryItem (nearest) == true) {
						Inventory.playerInventory.addItemToInventory (nearest);
						nearest.equipItem ();
					}
				}
			}
		} else {
			NearbyWeaponPickup.me.disable ();

		}
	}

	public void setOpenContainer(Container toOpen)
	{
		displayContainer = true;
		toDisplay = toOpen;
	}



	void drawItemUI()
	{
		int counter = nearIndex;
		foreach (ItemUI i in myItemUI) {
			if (counter < nearbyItems.Count) {
				i.gameObject.SetActive (true);
				i.setItem (nearbyItems [counter]);
				counter++;
			} else {
				
				i.gameObject.SetActive (false);
			}
		}

	//	for (int x = 0; x <InventoryUI.me.nearText.Count; x++) {
		//	if (myItemUI [x].gameObject.activeInHierarchy == true) {
			//	InventoryUI.me.nearText [x].gameObject.SetActive (true);
			//	InventoryUI.me.nearText [x].text = (nearIndex + x).ToString();
			//} else {
			//	InventoryUI.me.nearText [x].gameObject.SetActive (false);
		//	}
		//}
	}

	public void incrementCounter()
	{
		if (nearIndex < nearbyItems.Count-5) {
			nearIndex++;
		}
	}

	public void decrementCounter()
	{
		if (nearIndex > 0) {
			nearIndex--;
		}
	}

	public List<GameObject> findAWeapon(Vector3 pos)
	{
		List<GameObject> retVal=new List<GameObject>();

		foreach (Item i in itemsInWorld) {
			if (i.gameObject.activeInHierarchy == true) {
				Weapon w = i.gameObject.GetComponent<Weapon> ();
				if (w == null) {

				} else {
					if (Vector3.Distance (w.gameObject.transform.position, pos) < 20.0f) {
						GameObject g = getAmmoForWeapon (w,pos);
						if (g == null) {

						} else {
							retVal.Add (w.gameObject);
							retVal.Add (g);
							return retVal;
						}
					}
				}
			}
		}
		return retVal;
	}

	public GameObject getAmmoForWeapon(Weapon w,Vector3 pos)
	{

		foreach (Item i in itemsInWorld) {
			if (i.gameObject.activeInHierarchy == true) {
				if (Vector3.Distance (pos, i.gameObject.transform.position) < 20.0f) {
					AmmoItem ai = i.gameObject.GetComponent<AmmoItem> ();
					if (ai == null) {

					} else {
						if (ai.canWeUseAmmoInWeapon (w.WeaponName) == true) {
							return i.gameObject;
						}
					}
				}
			}
		}
		return null;
	}
}
