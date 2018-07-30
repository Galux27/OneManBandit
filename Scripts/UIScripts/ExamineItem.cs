using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class to control the examine item UI 
/// </summary>
public class ExamineItem : MonoBehaviour {
	public static ExamineItem me;
	public GameObject examineGameobject;
	public Item myItem;
	public Text name,illigal,weight,equip,consume,description;
	public Image itemImage;
	void Awake()
	{
		me = this;
	}

	public bool isExaminingItem()
	{
		return examineGameobject.activeInHierarchy;
	}

	void Update()
	{
		if (Inventory.playerInventory.inventoryGUI.activeInHierarchy == false) {
			stopExamining ();
		}
	}

	public void setItem(Item i)
	{
		myItem = i;
		name.text = "Name: " + i.getItemName();
		description.text = i.getDescription ();
		if (i.illigal == true) {
			illigal.text = "Item is illigal.";
		} else {
			illigal.text = "Item is not illigal.";
		}

		if (i.canEquip == true || i.slot == itemEquipSlot.none) {
			equip.text = "Item is equiped to " + i.slot.ToString ();
		} else {
			equip.text = "Cannot equip item.";
		}

		if (i.canConsume == true) {
			consume.text = "Item can be consumed";
		} else {
			consume.text = "Item cannot be consumed";
		}

		itemImage.sprite = i.itemTex;
		if (i.itemTex.texture.width > 70) {
			itemImage.rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, i.itemTex.texture.width*3);
			itemImage.rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, i.itemTex.texture.height*3);
		} else {
			itemImage.rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, i.itemTex.texture.width*4);
			itemImage.rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, i.itemTex.texture.height*4);
		}

		weight.text = i.itemWeight.ToString () + "kg";

		examineGameobject.SetActive (true);
	}

	public void stopExamining()
	{
		examineGameobject.SetActive (false);
	}
}
