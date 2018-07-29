using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour_Shopkeeper: NPCBehaviour {
	Shop myShop;
	public override void Initialise ()
	{
		myType = behaviourType.shopkeeper;
		myController = this.gameObject.GetComponent<NPCController> ();

		float d = 9999999.0f;

		foreach (Shop s in Shop.shopsInWorld) {
			float d2 = Vector2.Distance (s.transform.position, this.transform.position);
			if (d2 < d) {
				d = d2;
				myShop = s;
			}
		}

		if (Vector2.Distance (this.transform.position, myShop.transform.position) > 2.0f) {
			myController.pf.getPath (this.gameObject,myShop.gameObject);
		}
		Debug.Log ("INITIALISED SHOPKEEPER BEHAVIOUR");
		isInitialised = true;
	}

	public override void OnUpdate ()
	{
		if (isInitialised == false) {
			Initialise ();
		}

		if (Vector2.Distance (this.transform.position, myShop.transform.position) > 2.0f) {
			moveToPosition ();
		} else {
			if (ShopUI.me.myShop == myShop) {
				myController.pmc.rotateToFacePosition (CommonObjectsStore.player.transform.position);

			} else {
				myController.pmc.rotateToFacePosition (myShop.transform.position);

			}
		}
	}
	public string posImGoingTo = "";

	bool areWeAtPosition()
	{
		return (Vector2.Distance (this.transform.position, myShop.transform.position) < 2.0f);
	}

	void moveToPosition(){
		myController.pmc.rotateToFacePosition (myController.pf.getCurrentPoint());
		if (areWeAtPosition()==false) {
			myController.pmc.moveToDirection (myController.pf.getCurrentPoint());
		}
	}

	public override void OnComplete ()
	{

	}

	void OnDestroy()
	{
		myShop.shopAvailable = false;
	}

}
