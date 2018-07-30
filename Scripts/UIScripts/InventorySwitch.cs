using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class that controls the switching between the 3 main UI components (inventory, crafting, map)
/// </summary>
public class InventorySwitch : MonoBehaviour {
	public static InventorySwitch me;
	public GameObject switchParent;

	void Awake()
	{
		me = this;
	}

	public void enable()
	{
		switchParent.SetActive (true);
		setInventory ();
	}

	public void disable()
	{
		disableBits ();

		switchParent.SetActive (false);
	}
	public void setInventory()
	{
		if (Inventory.playerInventory.inventoryGUI.activeInHierarchy == false) {
			Inventory.playerInventory.enableInv ();
		}
		MapControlScript.me.displayMap = false;
		CraftingUIParent.me.displayCrafting = false;

	}

	public void setCrafting()
	{
		CraftingUIParent.me.displayCrafting = true;
		MapControlScript.me.displayMap = false;
		if (Inventory.playerInventory.inventoryGUI.activeInHierarchy == true) {
			Inventory.playerInventory.disableInv();
		}
		ItemMoniter.me.nearIndex = 0;

	}

	public void setMap()
	{
		MapControlScript.me.displayMap = true;
		CraftingUIParent.me.displayCrafting = false;
		if (Inventory.playerInventory.inventoryGUI.activeInHierarchy == true) {
			Inventory.playerInventory.disableInv();
		}
		ItemMoniter.me.nearIndex = 0;

	}

	void disableBits()
	{
		MapControlScript.me.displayMap = false;
		CraftingUIParent.me.displayCrafting = false;
		if (Inventory.playerInventory.inventoryGUI.activeInHierarchy == true) {
			Inventory.playerInventory.disableInv();
		}
		ItemMoniter.me.nearIndex = 0;

	}


}
