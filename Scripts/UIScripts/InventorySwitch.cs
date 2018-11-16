using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that controls the switching between the 3 main UI components (inventory, crafting, map)
/// </summary>
public class InventorySwitch : MonoBehaviour {
	public static InventorySwitch me;
	public GameObject switchParent;
	public Button mapButton;
	Image butIm;
	void Awake()
	{
		me = this;
		butIm = mapButton.gameObject.GetComponent<Image> ();
	}

	void Update()
	{
		if (LevelController.me.canWeLeaveLevel () == true) {
			butIm.color = Color.white;
		} else {
			butIm.color = Color.black;
		}
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
		MissionUI.me.disableMissionUI ();

	}

	public void setCrafting()
	{
		MissionUI.me.disableMissionUI ();

		CraftingUIParent.me.displayCrafting = true;
		MapControlScript.me.displayMap = false;
		if (Inventory.playerInventory.inventoryGUI.activeInHierarchy == true) {
			Inventory.playerInventory.disableInv();
		}
		ItemMoniter.me.nearIndex = 0;

	}

	public void setMap()
	{		
		if (LevelController.me.canWeLeaveLevel () == true) {
			MissionUI.me.disableMissionUI ();
			MapControlScript.me.displayMap = true;
			CraftingUIParent.me.displayCrafting = false;
			if (Inventory.playerInventory.inventoryGUI.activeInHierarchy == true) {
				Inventory.playerInventory.disableInv();
			}
			ItemMoniter.me.nearIndex = 0;
		} else {
			PhoneAlert.me.setMessageText ("You need to be by a level exit to escape on foot.");

		}
	}

	public void setMissions()
	{
        return;
		MapControlScript.me.displayMap = false;
		CraftingUIParent.me.displayCrafting = false;
		MissionUI.me.enableMissionUI ();
		if (Inventory.playerInventory.inventoryGUI.activeInHierarchy == true) {
			Inventory.playerInventory.disableInv();
		}
		ItemMoniter.me.nearIndex = 0;
	}

	void disableBits()
	{
		MissionUI.me.disableMissionUI ();

		MapControlScript.me.displayMap = false;
		CraftingUIParent.me.displayCrafting = false;
		if (Inventory.playerInventory.inventoryGUI.activeInHierarchy == true) {
			Inventory.playerInventory.disableInv ();
		}
		ItemMoniter.me.nearIndex = 0;
	}


}
