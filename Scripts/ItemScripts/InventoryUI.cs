using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InventoryUI : MonoBehaviour {
	public static InventoryUI me;
	public List<ItemUI> myUIs,inventoryUis;
	public List<Text> invText,nearText;
	public GearSlotUI head, torso, leftArm, rightArm, leftLeg, rightLeg, backpack;
	int startIndex = 0;
	public Text weightText,containerName,containerWeight;
	public Button nearUp,nearDown,invUp,invDown;
	void Awake()
	{
		me = this;
		this.gameObject.SetActive (false);
	}

	void Update()
	{
		drawItemUI ();
        playerInput();
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

		nearUp.gameObject.SetActive (displayNearbyUp ());
		nearDown.gameObject.SetActive (displayNearbyDown ());
		invUp.gameObject.SetActive (displayMyItemsUp ());
		invDown.gameObject.SetActive (displayMyItemsDown ());

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

    void playerInput()
    {
        Vector2 input = Input.mouseScrollDelta;
        if(input.y>0)
        {
            if (scrollingPlayerItems() == true)
            {
                decreaseIndex();
            }
            else
            {
                ItemMoniter.me.decrementCounter();
            }
        }
        else if(input.y<0)
        {
            if (scrollingPlayerItems() == true)
            {
                increaseIndex();
            }
            else
            {
                ItemMoniter.me.incrementCounter();
            }
        }
    }

    bool scrollingPlayerItems()
    {
       if(Input.mousePosition.x>Screen.width/3)
        {
            return true;
        }
        else
        {
            return false;
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

	bool displayMyItemsUp()
	{
		if (startIndex > 0) {
			return true;
		}
		return false;
	}

	bool displayMyItemsDown()
	{
		if (startIndex < Inventory.playerInventory.inventoryItems.Count-8 && Inventory.playerInventory.inventoryItems.Count>8) {
			return true;
		}
		return false;
	}

	bool displayNearbyUp()
	{

		if (ItemMoniter.me.nearIndex > 0) {
			return true;
		}
		return false;
	}

	bool displayNearbyDown()
	{
		if (ItemMoniter.me.nearIndex < ItemMoniter.me.nearbyItems.Count-8 && ItemMoniter.me.nearbyItems.Count > 8) {
			return true;
		}
		return false;
	}
}
