using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour {
	public string containerName="";
	public List<Item> itemsInContainer;
	public float maxWeight = 10.0f;
	public SpriteAnimation openContainerAnim,closeContainerAnim,anim;
	public bool open = false;
	public bool animate = false;
	public bool locked = false;
	public bool initialised = false;
	public AudioClip openSfx;
	void Awake()
	{
		sr = this.GetComponent<SpriteRenderer> ();
		this.gameObject.AddComponent<AudioController> ();

	}

	void Start()
	{
		actionsToAdd ();
	}

	float timer = 0.0f;
	int counter = 0;
	SpriteRenderer sr;
	public void playAnimation()
	{
		if (anim == null) {
			return;
		}

		if (counter >= anim.spritesInAnimation.Length) {
			counter = anim.spritesInAnimation.Length - 1;
		}

		timer -= Time.deltaTime;
		sr.sprite = anim.spritesInAnimation [counter];

		if (timer <= 0) {
			if (counter < anim.spritesInAnimation.Length-1) {
				counter++;
				timer = anim.timePerFrame;

			} else {
				counter=0;
				timer = anim.timePerFrame;
				animate = false;
			}
		}
	}

	void Update()
	{
		if (animate == true) {
			playAnimation ();
		}
	}

	public void actionOpenContainer()
	{
		if (open == false) {
			this.gameObject.GetComponent<AudioController> ().playSound (openSfx);
			openContainer ();
			if (Inventory.playerInventory.inventoryGUI.activeInHierarchy == false) {
				Inventory.playerInventory.enableInv ();
			} 
		} else {
			this.gameObject.GetComponent<AudioController> ().playSound (openSfx);

			closeContainer ();
			if (Inventory.playerInventory.inventoryGUI.activeInHierarchy == true) {
				Inventory.playerInventory.disableInv ();
			}
		}
	}

	void actionsToAdd()
	{
		if (this.gameObject.tag != "Player" && this.gameObject.tag != "NPC" && this.GetComponent<PlayerAction_SearchContainer>()==false) {
			this.gameObject.AddComponent<PlayerAction_SearchContainer> ();
		}
	}

	public void openContainer()
	{
		if (locked == true) {
			return;
		}


		//play opening animation
		anim = openContainerAnim;
		open = true;
		animate = true;
		ItemMoniter.me.setOpenContainer(this);
	}

	public void closeContainer()
	{
		anim = closeContainerAnim;
		open = false;
		animate = true;
	}

	public float getCurrentWeight()
	{
		float retVal = 0.0f;

		foreach (Item i in itemsInContainer) {
			retVal += i.itemWeight;
		}

		return retVal;
	}

	public bool canWeAddItemToContainer(Item i)
	{
		if (getCurrentWeight () + i.itemWeight <= maxWeight) {
			return true;
		} else {
			return false;
		}
	}

	public string getDisplay(){
		return getCurrentWeight ().ToString() + "/" + maxWeight + " KG";
	}

	public void addItemToContainer(Item i)
	{
		if (canWeAddItemToContainer (i)) {
			if (itemsInContainer == null) {
				itemsInContainer = new List<Item> ();
			}

			itemsInContainer.Add (i);
			i.gameObject.SetActive (false);
			i.myContainer = this;
			i.gameObject.transform.parent = this.transform;
			i.transform.localPosition = new Vector3 (0, 0, 0);
		}
	}



	public Item removeItemFromContainer(Item i)
	{

		if (i.gameObject.GetComponent<ClothingItem> () == true) {
			i.gameObject.GetComponent<ClothingItem> ().unequipItem ();
		}

		itemsInContainer.Remove (i);
		i.gameObject.SetActive (true);
		i.transform.parent = null;
		return i;
	}
}
