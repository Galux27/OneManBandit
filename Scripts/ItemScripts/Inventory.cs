using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Inventory : MonoBehaviour {
	public static Inventory playerInventory;
	public List<Item> inventoryItems;
	public float inventoryCapacity = 10.0f;
	public float inventoryCapcaityAddition=0.0f;
	public Item head,leftArm,rightArm,torso,leftLeg,rightLeg,back; //itemSlots
	public GameObject inventoryGUI;
	void Awake(){
		if (this.gameObject.GetComponent<PlayerInputController> () == true) {
			playerInventory = this;
		}
	}

	void Update()
	{
		decideCapacityMod ();
	}

	public void decideCapacityMod()
	{
		float val = 0.0f;
		if (head == null) {

		} else {
			val += head.getCapacityIncrease ();
		}

		if (torso == null) {

		} else {
			val += torso.getCapacityIncrease ();
		}

		if (leftArm == null) {
		} else {
			val += leftArm.getCapacityIncrease ();
		}

		if (rightArm == null) {

		} else {
			val += rightArm.getCapacityIncrease ();
		}

		if (leftLeg == null) {
		} else {
			
			val += leftLeg.getCapacityIncrease ();
		}
		if (rightLeg == null) {
		} else {
			val += rightLeg.getCapacityIncrease ();
		}

		if (back == null) {
		} else {
			val += back.getCapacityIncrease ();

		}
		inventoryCapcaityAddition = val;
	}

	public float getArmourMod ()
	{
		float val = 0.0f;
		if (head == null) {

		} else {
			val += head.getArmourIncrease ();
		}

		if (torso == null) {

		} else {
			val += torso.getArmourIncrease ();
		}

		if (leftArm == null) {
		} else {
			val += leftArm.getArmourIncrease ();
		}

		if (rightArm == null) {

		} else {
			val += rightArm.getArmourIncrease ();
		}

		if (leftLeg == null) {
		} else {

			val += leftLeg.getArmourIncrease ();
		}
		if (rightLeg == null) {
		} else {
			val += rightLeg.getArmourIncrease ();
		}

		if (back == null) {
		} else {
			val += back.getArmourIncrease ();

		}
		return val;
	}

	public float getCapacity()
	{
		return inventoryCapacity + inventoryCapcaityAddition;
	}

	public void setVisual(Item i, itemEquipSlot ies){
		if (ies == itemEquipSlot.backpack) {

			if (this != playerInventory) {
				return;
			}

			InventoryUI.me.backpack.setItem (i);
		} else if (ies == itemEquipSlot.bothHands) {
			

			if (this != playerInventory) {
				return;
			}




			InventoryUI.me.leftArm.setItem (i);
			InventoryUI.me.rightArm.setItem (i);

		}
		else if (ies == itemEquipSlot.head) {

		
			if (this != playerInventory) {
				return;
			}

			InventoryUI.me.head.setItem (i);

		}
		else if (ies == itemEquipSlot.torso) {
			
			if (this != playerInventory) {
				return;
			}


			InventoryUI.me.torso.setItem (i);

		}
		else if (ies == itemEquipSlot.leftHand) {
			
			if (this != playerInventory) {
				return;
			}


			InventoryUI.me.leftArm.setItem (i);

		}
		else if (ies == itemEquipSlot.rightHand) {
			
			if (this != playerInventory) {
				return;
			}


			InventoryUI.me.rightArm.setItem (i);

		}
		else if (ies == itemEquipSlot.leftLeg) {

			if (this != playerInventory) {
				return;
			}

			InventoryUI.me.leftLeg.setItem (i);

		}
		else if (ies == itemEquipSlot.rightLeg) {
			
			if (this != playerInventory) {
				return;
			}



			InventoryUI.me.rightLeg.setItem (i);

		}

		if (i.gameObject.GetComponent<Weapon> () == true || i.gameObject.GetComponent<PortableContainerItem> () == true || i.gameObject.GetComponent<ThrowableItem>()==true) {
			i.gameObject.SetActive (true);
		}
	}


	public void equipItem(Item i, itemEquipSlot ies)
	{
		if (ies == itemEquipSlot.backpack) {


			/*if (back == null) {

			} else {
				if (back.mustEquip == true) {
					dropItem (i);
				}

			}*/
			if (back == null) {

			} else {
				//InventoryUI.me.backpack.unequipItem ();
				unequipItem (back);
			}
			back = i;
			if (this != playerInventory) {
				return;
			}

			InventoryUI.me.backpack.setItem (i);
		} else if (ies == itemEquipSlot.bothHands) {

			/*if (leftArm == null) {

			} else {
				if (leftArm.mustEquip == true) {
					dropItem (leftArm);
				}
			}

			if (rightArm == null) {

			} else {
				if (rightArm.mustEquip == true) {
					dropItem (rightArm);
				}
			}*/
			if (leftArm == null) {

			} else {
				//InventoryUI.me.leftArm.unequipItem ();

				unequipItem (leftArm);


			}

			if (rightArm == null) {

			} else {
				//InventoryUI.me.rightArm.unequipItem ();
				unequipItem (rightArm);

			}

			leftArm = i;
			rightArm = i;

			if (this != playerInventory) {
				return;
			}




			InventoryUI.me.leftArm.setItem (i);
			InventoryUI.me.rightArm.setItem (i);

		}
		else if (ies == itemEquipSlot.head) {

			if (head == null) {

			} else {
				//InventoryUI.me.head.unequipItem ();
				unequipItem (head);

			}

			head = i;

			/*if (head == null) {

			} else {
				if (head.mustEquip == true) {
					dropItem (head);
				}
			}*/

			if (this != playerInventory) {
				return;
			}

			InventoryUI.me.head.setItem (i);

		}
		else if (ies == itemEquipSlot.torso) {
		/*	if (torso == null) {

			} else {
				if (torso.mustEquip == true) {
					dropItem (torso);
				}
			}*/

			if (torso == null) {

			} else {
				//InventoryUI.me.torso.unequipItem ();
				unequipItem (torso);

			}
			torso = i;
			if (this != playerInventory) {
				return;
			}
		
		
			InventoryUI.me.torso.setItem (i);

		}
		else if (ies == itemEquipSlot.leftHand) {
			if (leftArm == null) {

			} else {
				//InventoryUI.me.leftArm.unequipItem ();
				unequipItem (leftArm);

			}
			/*if (leftArm == null) {

			} else {
				if (leftArm.mustEquip == true) {
					dropItem (leftArm);
				}
			}*/
			leftArm = i;
			if (this != playerInventory) {
				return;
			}


			InventoryUI.me.leftArm.setItem (i);

		}
		else if (ies == itemEquipSlot.rightHand) {
			/*if (rightArm == null) {

			} else {
				if (rightArm.mustEquip == true) {
					dropItem (rightArm);
				}
			}*/
			if (rightArm == null) {

			} else {
				//InventoryUI.me.rightArm.unequipItem ();
				unequipItem (rightArm);

			}
			rightArm = i;
			if (this != playerInventory) {
				return;
			}


			InventoryUI.me.rightArm.setItem (i);

		}
		else if (ies == itemEquipSlot.leftLeg) {
			if (leftLeg == null) {

			} else {
				//InventoryUI.me.leftLeg.unequipItem ();
				unequipItem (leftLeg);

			}


			/*if (leftLeg == null) {

			} else {
				if (leftLeg.mustEquip == true) {
					dropItem (leftLeg);
				}
			}*/
			leftLeg = i;
			if (this != playerInventory) {
				return;
			}

			InventoryUI.me.leftLeg.setItem (i);

		}
		else if (ies == itemEquipSlot.rightLeg) {
			if (rightLeg == null) {

			} else {
				//InventoryUI.me.rightLeg.unequipItem ();
				unequipItem (rightLeg);

			}
			/*if (rightLeg == null) {

			} else {
				if (rightLeg.mustEquip == true) {
					dropItem (rightLeg);
				}
			}*/

			rightLeg = i;

			if (this != playerInventory) {
				return;
			}



			InventoryUI.me.rightLeg.setItem (i);

		}

		if (i.gameObject.GetComponent<Weapon> () == true || i.gameObject.GetComponent<PortableContainerItem> () == true || i.gameObject.GetComponent<ThrowableItem>()==true) {
			i.gameObject.SetActive (true);
		}
	}



	public void addItemToInventory(Item i)
	{
		if (inventoryItems == null) {
			inventoryItems = new List<Item> ();
		}

		if (canWeCarryItem (i)) {
			i.inUse = true;
			inventoryItems.Add (i);
			i.gameObject.transform.parent = this.gameObject.transform;

			if (i.mustEquip == true) {
				equipItem (i, i.slot);
				i.equipItem ();

			}
			if (i.mustEquip == false) {

				i.gameObject.SetActive (false);
			}

		}
	}

	public bool canWeCarryItem(Item i)
	{
		if (i.itemWeight + getInventoryWeightSum () <= getCapacity()) {
			return true;
		} else {
			return false;
		}
	}

	public void unequipItem(Item i )
	{
		itemEquipSlot ies = i.slot;
		//i.inUse = false;
		if (ies == itemEquipSlot.backpack) {
			if (back == null) {
				return;
			}
			back.unequipItem ();
			back = null;
		} else if (ies == itemEquipSlot.bothHands) {

			if (leftArm == null) {

			} else {
				leftArm.unequipItem ();
				//rightArm.unequipItem ();
				leftArm = null;
				//rightArm = null;
			}


			if (rightArm == null) {
			} else {
				//leftArm.unequipItem ();
				rightArm.unequipItem ();
				//leftArm = null;
				rightArm = null;
			}

		}
		else if (ies == itemEquipSlot.head) {
			

			head.unequipItem ();
			head = null;
		}
		else if (ies == itemEquipSlot.torso) {
			if (torso == null) {
				return;
			}

			torso.unequipItem ();
			torso = null;
		}
		else if (ies == itemEquipSlot.leftHand) {

			if (leftArm == null) {

			} else {
				leftArm.unequipItem ();
				//rightArm.unequipItem ();
				leftArm = null;
				//rightArm = null;
			}
		}
		else if (ies == itemEquipSlot.rightHand) {
			if (rightArm == null) {
			} else {
				//leftArm.unequipItem ();
				rightArm.unequipItem ();
				//leftArm = null;
				rightArm = null;
			}

		}
		else if (ies == itemEquipSlot.leftLeg) {
			leftLeg.unequipItem ();
			leftLeg = null;
		}
		else if (ies == itemEquipSlot.rightLeg) {
			rightLeg.unequipItem ();
			rightLeg = null;
		}

		if (this == playerInventory) {
			InventoryUI.me.unequipItem (i);
		}

		if (i.mustEquip == true) {
			inventoryItems.Remove (i);
			i.dropItem ();
			i.gameObject.transform.position = this.transform.position;
		}
		i.inUse = false;
	}


	public void dropItem(Item i)
	{
		
		if (inventoryItems.Contains (i) == false) {
			return;
		}
		//////Debug.Log ("Dropping item " + i.itemName);
		if (this == playerInventory) {
			unequipItem (i);
		}



		inventoryItems.Remove (i);
		i.dropItem ();
		i.inUse = false;
		i.gameObject.transform.position = this.transform.position;
	}

	public float getInventoryWeightSum()
	{
		float retVal = 0.0f;
		foreach (Item i in inventoryItems) {
			retVal += i.itemWeight;
		}
		return retVal;
	}

	public void removeItemWithoutDrop(Item i)
	{
		inventoryItems.Remove (i);
	}

	public void enableInv()
	{
		inventoryGUI.SetActive (true);
	}

	public void disableInv()
	{
		inventoryGUI.SetActive (false);
	}

	public bool canWeReloadGun(string curWep)
	{
		foreach (Item i in inventoryItems) {
			AmmoItem ai = i.gameObject.GetComponent<AmmoItem> ();

			if (ai == null) {
				continue;
			} else {
				//////Debug.Log (ai.gunsAmmoFitsIn [0]);
				if (ai.canWeUseAmmoInWeapon (curWep)==true) {
					return true;
				}
			}
		}
		return false;
	}

	public AmmoItem getAmmoForGun(string curWep)
	{
		foreach (Item i in inventoryItems) {
			AmmoItem ai = i.gameObject.GetComponent<AmmoItem> ();

			if (ai == null) {
				continue;
			} else {
				if (ai.canWeUseAmmoInWeapon (curWep)==true) {
					return ai;
				}
			}
		}
		return null;
	}

	public void die()
	{
		List<Item> itemTemp = inventoryItems;
		foreach (Item i in itemTemp) {
			unequipItem (i);
			i.gameObject.SetActive (true);
			i.inUse = false;
			i.transform.parent = null;
		}
		inventoryItems = new List<Item> ();
	}

	public bool doWeHaveAWeaponWeCanUse()
	{
		foreach (Item i in inventoryItems) {
			Weapon w = i.gameObject.GetComponent<Weapon> ();

			if (w == null) {
				continue;
			} else {
				if (w.melee == true) {
					return true;
				} else {

					if (canWeReloadGun (w.WeaponName) == true) {
						return true;
					}
				}
			}
		}
		return false;
	}

	public Weapon getWeaponWeCanUse()
	{
		foreach (Item i in inventoryItems) {
			Weapon w = i.gameObject.GetComponent<Weapon> ();

			if (w == null) {
				continue;
			} else {
				if (w.melee == true) {
					return w;
				} else {
					if (canWeReloadGun (w.WeaponName) == true) {
						return w;
					}
				}
			}
		}
		return null;
	}


	public bool doWeHaveItem(string itemName)
	{
		foreach (Item i in inventoryItems) {
			if (i.itemName == itemName) {
				return true;
			}
		}
		return false;
	}

	public bool doWeHaveItem(GameObject item)
	{
		foreach (Item i in inventoryItems) {
			if (i.gameObject == item) {
				return true;
			}
		}
		return false;
	}

	public Item getItem(string name)
	{
		foreach (Item i in inventoryItems) {
			if (i.itemName == name) {
				return i;
			}
		}
		return null;
	}

	public bool doWeHaveKeycardOfClearance(int clearance)
	{
		foreach (Item i in inventoryItems) {
			KeycardItem k = i.gameObject.GetComponent<KeycardItem> ();

			if (k == null) {

			} else {
				if (k.securityClearance == clearance) {
					return true;
				}
			}
		}
		return false;
	}
}
