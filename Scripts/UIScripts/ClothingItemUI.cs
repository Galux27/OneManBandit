using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Class for displaying the UI for one item of clothing the player is wearing 
/// </summary>
public class ClothingItemUI : MonoBehaviour {

	void Awake()
	{
		resetGearSlot ();
	}

	public ClothingItem itemEquiped;
	public Image itemImage;
	public ClothingItemSlot mySlot;
	public void setItem(ClothingItem i)
	{
		//////Debug.Log ("Item set " + i.gameObject.name);

		if (itemEquiped == null) {

		} else {
			unequipItem ();
		}

		if (i.itemTex.texture.width > 70) {
			itemImage.rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, i.itemTex.texture.width);
			itemImage.rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, i.itemTex.texture.height);
		} else {
			itemImage.rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, i.itemTex.texture.width*2);
			itemImage.rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, i.itemTex.texture.height*2);
		}

		//this.gameObject.SetActive (true);
		itemEquiped = i;
		itemImage.color = Color.white;
		itemImage.sprite = i.itemTex;
	}

	public void hideItem()
	{
		itemEquiped = null;
		itemImage.sprite = null;
		itemImage.color = Color.clear;

		//this.gameObject.SetActive (false);
	}

	public void dropItem()
	{
		itemImage.sprite = null;
		itemImage.color = Color.clear;
		//Inventory.playerInventory.dropItem (itemEquiped);
		itemEquiped.dropItem();
		//itemEquiped.dropItem ();
		itemEquiped = null;
	}

	public void unequipItem()
	{
		if (itemEquiped == null) {

		} else {
			itemEquiped.unequipItem ();
			//Inventory.playerInventory.unequipItem (itemEquiped);
		}


		itemImage.sprite = null;
		itemImage.color = Color.clear;
		itemEquiped = null;
	}


	public void resetGearSlot()
	{
		itemImage.sprite = null;
		itemImage.color = Color.clear;
		itemEquiped = null;
	}
}