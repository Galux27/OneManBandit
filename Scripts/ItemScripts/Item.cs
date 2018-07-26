using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

	/// <summary>
	/// Base class for items, the child classes overwrite the virtuals to add their individual functionality. 
	/// </summary>

	public string itemName,itemDescription,briefDescription;
	public float itemWeight;
	public Sprite itemTex,inWorldUnequiped;
	public itemEquipSlot slot;
	public bool canEquip, canConsume,mustEquip;
	public Container myContainer;
	public bool illigal = false;
	public bool inUse = false;
	public int myID=-1,price=0;
	void OnDestroy()
	{
		ItemMoniter.me.itemsInWorld.Remove (this);
	}

	void OnEnable()
	{
		if (this.GetComponent<CommonObjectsStore> () == true) {
			return;
		}

		if (ItemMoniter.me.itemsInWorld.Contains (this) == false) {
			ItemMoniter.me.itemsInWorld.Add (this);
		}
		if (this.transform.parent == null) {
			this.gameObject.GetComponent<SpriteRenderer> ().sprite = inWorldUnequiped;
		}

	}

	public virtual void equipItem()
	{
		//equips item to relevant slot
	}

	public virtual void unequipItem()
	{
		
	}

	public virtual void useItem()
	{
		//uses item e.g. consumables
	}

	public virtual void itemPassiveEffect()
	{
		//does effect on call e.g. increases capacity
	}

	public virtual string getDescription()
	{
		return itemDescription;
	}

	public virtual string getBriefDescription()
	{
		return briefDescription;
	}

	public virtual float getCapacityIncrease()
	{
		return 0.0f;
	}

	public virtual float getArmourIncrease()
	{
		return 0.0f;
	}

	public void dropItem()
	{
		this.gameObject.transform.parent = null;
		this.gameObject.SetActive (true);
		inUse = false;
	}

	public virtual string getItemName()
	{
		return itemName;
	}
}

public enum itemEquipSlot{
	leftHand,
	rightHand,
	head,
	torso,
	leftLeg,
	rightLeg,
	bothHands,
	backpack,
	none
}
