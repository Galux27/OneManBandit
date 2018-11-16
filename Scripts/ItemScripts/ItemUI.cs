using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemUI : MonoBehaviour {
	/// <summary>
	/// Class that controls Item UI in inventories 
	/// </summary>


	public Text itemName,itemDescription;
	public Image icon;
	public Item toDisplay;
	public Button equip, use;
	public void setItem(Item i){
		
		toDisplay = i;
		if (toDisplay.gameObject.GetComponent<AmmoItem> () == false) {
			itemName.text = i.getItemName ();
		} else {
			AmmoItem ai = i.gameObject.GetComponent<AmmoItem> ();

			itemName.text = ai.getItemName ();;

		}
		icon.sprite = i.itemTex;
		itemDescription.text = i.getBriefDescription();

		if (equip == null ) {
			return;
		}
		if (i.itemTex.texture.width > 70) {
			icon.rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, i.itemTex.texture.width);
			icon.rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, i.itemTex.texture.height);
		} else {
			icon.rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, i.itemTex.texture.width*2);
			icon.rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, i.itemTex.texture.height*2);
		}

		if (InventoryUI.me.myUIs.Contains (this)) {
			
			equip.gameObject.SetActive (i.canEquip);	
			use.gameObject.SetActive (i.canConsume);
		}
	}

	public void clearItem()
	{
		toDisplay = null;
		icon.sprite = null;
		//this.gameObject.SetActive( false);
	}

	public void dropItem()
	{
		if (ItemMoniter.me.displayContainer == false) {
			icon.sprite = null;
			if (toDisplay.gameObject.GetComponent<ClothingItem> () == false) {
				Inventory.playerInventory.dropItem (toDisplay);
			} else {
				toDisplay.gameObject.GetComponent<ClothingItem> ().dropItem ();
			}
			ItemMoniter.me.refreshItems ();
		} else {


			if (toDisplay.gameObject.GetComponent<ClothingItem> () == false) {
				icon.sprite = null;

				//Inventory.playerInventory.dropItem (toDisplay);
				Inventory.playerInventory.unequipItem (toDisplay);
				Inventory.playerInventory.removeItemWithoutDrop (toDisplay);
				ItemMoniter.me.toDisplay.addItemToContainer (toDisplay);
				ItemMoniter.me.refreshItems ();
			} else {
				Inventory.playerInventory.removeItemWithoutDrop (toDisplay);
				toDisplay.gameObject.GetComponent<ClothingItem> ().dropItem ();
				ItemMoniter.me.toDisplay.addItemToContainer (toDisplay);
				ItemMoniter.me.refreshItems ();
			}
		}
	}

	public void equipItem()
	{
		
		toDisplay.equipItem ();
	}

	public void useItem()
	{
		toDisplay.useItem ();
	}

	public void addItemToInventory()
	{
		
		if (toDisplay.myContainer == null) {

		} else {
			toDisplay.myContainer.removeItemFromContainer (toDisplay);
			if (toDisplay.GetComponent<ClothingItem> () == true) {
				toDisplay.GetComponent<ClothingItem> ().unequipItem ();
			}
		}
		Inventory.playerInventory.addItemToInventory (toDisplay);
		ItemMoniter.me.refreshItems ();
	}

	public void examineItem()
	{
		ExamineItem.me.setItem (toDisplay);
	}
}


