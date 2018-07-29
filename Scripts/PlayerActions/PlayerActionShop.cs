using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionShop  : PlayerAction {
	Shop myShop;
	BuildingScript myBuilding;
	NPCController myController;
	public override void doAction()
	{
		ShopUI.me.enableShopUI (myShop);
		//PlayerAction.currentAction = this;
	}

	public override bool canDo ()
	{
		if (myShop == null) {
			myShop = this.GetComponent<Shop> ();
		}

		if (myShop.shopAvailable == false) {
			return false;
		}

		if (myBuilding == null) {
			myBuilding = LevelController.me.getBuildingPosIsIn (this.gameObject.transform.position);
		}

		if (myController == null) {
			myController = this.GetComponent<NPCController> ();
		}

		if (myBuilding == null) {

		} else {
			if (myBuilding.traspassing == true || myBuilding.buildingClosed==true) {
				return false;
			}
		}

	

		if (myController == null) {

		} else {
			if (myController.myType == AIType.hostage || myController.knockedDown == true || myController.memory.beenAttacked == true || myController.memory.peopleThatHaveAttackedMe.Contains (CommonObjectsStore.player)) {
				return false;
			}
		}

		if (myShop == null) {
			return false;
		}

		return true;


	}

	public override void onComplete()
	{
		
	}

	public override float getMoveModForAction ()
	{
		return -0.0f;
	}

	public override float getRotationModForAction ()
	{
		return -0.0f;
	}

	public override string getType()
	{
		return "Shop";
	}

	public override string getDescription()
	{
		return "Buy items from the shop.";
	}
}