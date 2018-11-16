using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GearSlotUI : MonoBehaviour {

	void Awake()
	{
		if (itemEquiped == null) {
			resetGearSlot ();
		}
	}

	public Item itemEquiped;
	public Image itemImage;



	public void setItem(Item i)
	{
		////////Debug.Log ("Item set " + i.gameObject.name);

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
		Inventory.playerInventory.dropItem (itemEquiped);
		//itemEquiped.dropItem ();
		itemEquiped = null;
	}

	public void unequipItem()
	{
		if (itemEquiped == null) {

		} else {
			Inventory.playerInventory.unequipItem (itemEquiped);
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
