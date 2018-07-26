using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InventoryUI : MonoBehaviour {
	public static InventoryUI me;
	public List<ItemUI> myUIs;
	public List<Text> invText,nearText;
	public GearSlotUI head, torso, leftArm, rightArm, leftLeg, rightLeg, backpack;
	int startIndex = 0;
	public Text weightText,containerName,containerWeight;

	void Awake()
	{
		me = this;
	}

	void Update()
	{
		drawItemUI ();
	}

	void drawItemUI()
	{
		int counter = startIndex;
		foreach (ItemUI i in myUIs) {
			if (counter < Inventory.playerInventory.inventoryItems.Count) {
				i.gameObject.SetActive (true);
				i.setItem (Inventory.playerInventory.inventoryItems [counter]);
				counter++;
			} else {
				i.gameObject.SetActive (false);
			}
		}


		for (int x = 0; x < invText.Count; x++) {
				if (myUIs [x].gameObject.activeInHierarchy == true) {
					invText [x].gameObject.SetActive (true);
					invText [x].text = (startIndex + x).ToString();
				} else {
					invText [x].gameObject.SetActive (false);
				}
		}


		weightText.text = Inventory.playerInventory.getInventoryWeightSum () + "/" + Inventory.playerInventory.getCapacity() + " KG";
		if (ItemMoniter.me.displayContainer == true) {
			containerWeight.gameObject.SetActive (true);
			containerWeight.text = ItemMoniter.me.toDisplay.getDisplay ();
		} else {
			containerWeight.gameObject.SetActive (false);
		}
	}

	public bool isItemEquiped(Item i)
	{
		if (head.itemEquiped == i || torso.itemEquiped == i || leftArm.itemEquiped == i || rightArm.itemEquiped == i || leftLeg.itemEquiped == i || rightLeg.itemEquiped == i || backpack.itemEquiped == i) {
			return true;
		} else {
			return false;
		}
	}

	public void unequipItem(Item i )
	{
		itemEquipSlot ies = i.slot;
		if (ies == itemEquipSlot.backpack) {
			//backpack.unequipItem ();
			head.hideItem();
		} else if (ies == itemEquipSlot.bothHands) {
			//leftArm.unequipItem ();
			//rightArm.unequipItem ();
			leftArm.hideItem();
			rightArm.hideItem();
		}
		else if (ies == itemEquipSlot.head) {
			//head.unequipItem ();
			head.hideItem();
		}
		else if (ies == itemEquipSlot.torso) {
			//torso.unequipItem ();
			torso.hideItem();
		}
		else if (ies == itemEquipSlot.leftHand) {
			//leftArm.unequipItem ();
			leftArm.hideItem();
		}
		else if (ies == itemEquipSlot.rightHand) {
			//rightArm.unequipItem ();
			rightArm.hideItem();	
		}
		else if (ies == itemEquipSlot.leftLeg) {
			//leftLeg.unequipItem ();
			leftLeg.hideItem();
		}
		else if (ies == itemEquipSlot.rightLeg) {
			//rightLeg.unequipItem ();
			rightLeg.hideItem();
		}


	}

	public void decreaseIndex()
	{
		if (startIndex > 0) {
			startIndex--;
		}
	}

	public void increaseIndex(){
		if (startIndex < Inventory.playerInventory.inventoryItems.Count) {
			startIndex++;
		}
	}
}
