using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that controls the UI of a single item when a shop is being interacted wtih
/// </summary>
public class ShopItem : MonoBehaviour {
	public Item i;
	public Text itemName,itemPrice;
	public Image itemImage;
	public Button takeItemButton, returnItemButton;
	public int itemQuantity = 0;

	public void takeItem()
	{
		if (isThisShopItem () == true) {
			if (itemQuantity > 0) {
				itemQuantity--;
				ShopUI.me.reduceQuantityOfItem (i.itemName);
				itemName.text = i.getItemName () + "(" + itemQuantity+")";
				itemPrice.text = "£" + i.price.ToString ();
				itemImage.sprite = i.itemTex;
				if (i.itemTex.texture.width > 70) {
					itemImage.rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, i.itemTex.rect.width*1.5f);
					itemImage.rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, i.itemTex.rect.height*1.5f);
				} else {
					itemImage.rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, i.itemTex.rect.width*2.5f);
					itemImage.rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, i.itemTex.rect.height*2.5f);
				}
				ShopUI.me.addItemToBasket (i.itemName);

			}
		}
	}

	public void returnItem()
	{
		if (isThisBasketItem () == true) {
			ShopUI.me.increaseQuantityOfItem (i.itemName);
			ShopUI.me.removeItemFromBasket (i.itemName);

		}
	}

	bool isThisShopItem()
	{
		return ShopUI.me.shopUIs.Contains (this);
	}

	bool isThisBasketItem()
	{

		if (ShopUI.me.displayBasket == true) {
			return ShopUI.me.basketUIs.Contains (this);
		} else {
			return false;
		}
	}

	bool isThisInventoryItem()
	{
		if (ShopUI.me.displayBasket == false) {
			return ShopUI.me.basketUIs.Contains (this);
		} else {
			return false;
		}
	}

	public void setItem(Item item, int quantity)
	{
		i=item;
		itemName.text = i.getItemName () + "(" + quantity+")";
		itemPrice.text = "£" + i.price.ToString ();
		itemImage.sprite = i.itemTex;

		if (i.itemTex.texture.width > 70) {
			itemImage.rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, i.itemTex.rect.width*1.5f);
			itemImage.rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, i.itemTex.rect.height*1.5f);
		} else {
			itemImage.rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, i.itemTex.rect.width*2.5f);
			itemImage.rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, i.itemTex.rect.height*2.5f);
		}


		itemQuantity = quantity;


		if (isThisInventoryItem () == true) {
			if (ShopUI.me.myShop.canIBuyItem (i.itemName) == false) {
				itemPrice.text = "Does not buy.";
				returnItemButton.gameObject.SetActive (false);
				takeItemButton.gameObject.SetActive (false);
			} else {
				returnItemButton.gameObject.SetActive (true);
				takeItemButton.gameObject.SetActive (false);

				returnItemButton.gameObject.GetComponentInChildren<Text> ().text = "Sell";
			}
		} else if (isThisShopItem () == true) {
			returnItemButton.gameObject.SetActive (false);
			takeItemButton.gameObject.SetActive (true);
		} else if (isThisBasketItem () == true) {
			returnItemButton.gameObject.SetActive (true);
			returnItemButton.gameObject.GetComponentInChildren<Text> ().text = "Return";

			takeItemButton.gameObject.SetActive (false);
		}
	}

	public void setItem(string itemName,int quantity)
	{
		Item i = ItemDatabase.me.getItem (itemName).GetComponent<Item> ();
		setItem (i,quantity);
	}


}
