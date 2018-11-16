using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CraftingUIItem : MonoBehaviour {

	/// <summary>
	/// Class that controls the UI of a crafting component in a recipie 
	/// </summary>

	public Item myItem;
	public Text itemName;
	public Dropdown potentialItems;
	public Image itemImage;


	
	// Update is called once per frame
	void Update () {
		try{
			
			if(potentialItems.value==0 || potentialItems.value == potentialItems.options.Count-1)
			{
				resetItem();
			}else{
				setItem (Inventory.playerInventory.getItem(potentialItems.options[potentialItems.value].text));
			}
		}catch{
			resetItem ();
		}
	}

	int getVal()
	{
		int retVal = potentialItems.value;
		if (retVal < 0 ) {
			resetItem ();
			return 0;
		}
		return retVal;
	}

	void OnEnable()
	{
		populateDropdown ();
	}

	public void populateDropdown()
	{
		if (Inventory.playerInventory == null) {
			return;
		}
		List<string> items = new List<string> ();
		items.Add ("None");
		foreach (Item i in Inventory.playerInventory.inventoryItems) {
			bool addItem = true;


			if (items.Contains(i.itemName)==true || CraftingUIParent.me.isItemBeingUsed(i.itemName)) {
				addItem = false;
			} 

			if (addItem == true) {
				items.Add (i.itemName);
			}



			/*if (myItem == null) {
				items.Add (i.itemName);
			} else if (myItem == i || CraftingUIParent.me.isItemInUse(i)==true) {

			} else {
				items.Add (i.itemName);
			}*/
		}
		//items.Add ("");
		items.Add ("None");
		potentialItems.ClearOptions ();
		potentialItems.AddOptions (items);

	}

	bool isItemInDropdown(string st)
	{
		for (int x = 0; x < potentialItems.options.Count; x++) {
			if (potentialItems.options [x].text == st) {
				return true;
			}
		}
		return false;
	}

	public void setItem(Item i)
	{
		myItem = i;
		itemName.text = i.itemName;


		itemImage.rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, i.itemTex.texture.width*3);
		itemImage.rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, i.itemTex.texture.height*3);


		itemImage.sprite = i.itemTex;
		//populateDropdown ();
		//CraftingUIParent.me.resetAllUIButGiven(this);
	}

	public void resetItem()
	{
		myItem = null;
		itemName.text ="None";
		itemImage.sprite = null;
		potentialItems.value = 0;
		//populateDropdown ();
	}
}
