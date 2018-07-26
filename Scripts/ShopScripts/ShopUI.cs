using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShopUI : MonoBehaviour {
	public static ShopUI me;
	public List<ShopItem> shopUIs,basketUIs;
	public bool displayBasket=true;
	public GameObject shopParent;
	Shop myShop;
	public Text moneyDisp,itemsInBasketDisp,totalWeight,totalCost,basketTitle;
	public List<string> itemsInBasket;

	void Awake()
	{
		me = this;
		shopParent.SetActive (false);

	}

	public void buyItems()
	{
		if (canWeBuy () == true) {
			Debug.Log ("BUYING ITEMS");
			int stashValue = LoadingDataStore.me.getStashValue ();
			foreach (string st in itemsInBasket) {
				GameObject g = ItemDatabase.me.getItem (st);
				if (g == null) {

				} else {
					GameObject itemInWorld = (GameObject)Instantiate (g, CommonObjectsStore.player.transform.position, Quaternion.Euler (0, 0, 0));
					Item i = itemInWorld.GetComponent<Item> ();
					Inventory.playerInventory.addItemToInventory (i);
					stashValue -= i.price;
				}
			}
			LoadingDataStore.me.setStashValue (stashValue);
			itemsInBasket.Clear ();
			setUIs ();
		}
	}

	bool canWeBuy()
	{
		int value = 0;
		foreach (string st in itemsInBasket) {
			Item i = ItemDatabase.me.getItem (st).GetComponent<Item> ();
			if (i == null) {

			} else {
				value += i.price;
			}
		}
		int stash = LoadingDataStore.me.getStashValue ();

		if (stash >= value) {
			return true;
		} else {
			Debug.Log ("CANNOT BUY ITEMS, NOT ENOUGH MONEY");
			return false;
		}
	}

	public void enableShopUI(Shop s)
	{
		myShop = s;
		getEndPoints ();

		setUIs ();
		shopParent.SetActive (true);
	}

	public void disableShopUI()
	{
		shopParent.SetActive (false);
	}

	public void switchToPlayerInventory()
	{
		if (displayBasket == true) {
			displayBasket = false;
			basketTitle.text = "Inventory";
		} else {
			basketTitle.text = "Basket";
			displayBasket = true;
		}
		getEndPoints ();
		setUIs ();
	}

	public int shopEndPoint = 0,playerInvEndPoint=0,basketEndPoint=0;

	void setUIs()
	{
		int shopInd = shopEndPoint - shopUIs.Count;
		if (shopInd < 0) {
			shopInd = 0;
		}
		foreach(ShopItem si in shopUIs){
			if (shopInd <= shopEndPoint) {
				si.gameObject.SetActive (true);
				si.setItem (myShop.itemsIHave [shopInd],myShop.quantityIHave[shopInd]);
				shopInd++;
			} else {
				si.gameObject.SetActive (false);
			}
			//shopUIs [x].setItem (myShop.itemsIHave [x]);
		}

		if(displayBasket==true){
			int basketInd = basketEndPoint - basketUIs.Count;
			if (basketInd < 0) {
				basketInd = 0;
			}
			foreach (ShopItem si in basketUIs) {
				if (basketInd <= basketEndPoint) {
					si.gameObject.SetActive (true);
					si.setItem (itemsInBasket [basketInd],1);
					basketInd++;
				} else {
					si.gameObject.SetActive (false);
				}
			
			}


		}
		else{
			int basketInd = playerInvEndPoint - basketUIs.Count;
			if (basketInd < 0) {
				basketInd = 0;
			}
			foreach (ShopItem si in basketUIs) {
				if (basketInd <= playerInvEndPoint) {
					si.gameObject.SetActive (true);
					si.setItem (Inventory.playerInventory.inventoryItems [basketInd],1);
					basketInd++;
				} else {
					si.gameObject.SetActive (false);
				}

			}
		}
		setInfo ();
	}

	void getEndPoints()
	{
		if (myShop.itemsIHave.Count < shopUIs.Count) {
			shopEndPoint = myShop.itemsIHave.Count - 1;
		} else {
			shopEndPoint = shopUIs.Count - 1;
		}

		if (itemsInBasket.Count < basketUIs.Count) {
			basketEndPoint = itemsInBasket.Count - 1;
		} else {
			basketEndPoint = basketUIs.Count - 1;
		}

		if (Inventory.playerInventory.inventoryItems.Count < basketUIs.Count) {
			playerInvEndPoint = Inventory.playerInventory.inventoryItems.Count-1;
		} else{
			playerInvEndPoint = basketUIs.Count - 1;
		}
	}


	public void IncrementBasket()
	{
		
		if (displayBasket == true) {
			incrementBasketEndPoint ();
		} else {
			incrementInvEndPoint ();
		}
		setUIs ();
	}

	public void DecrementBasket()
	{
		if (displayBasket == true) {
			decrementBasketEndPoint ();
		} else {
			decrementInvEndPoint ();
		}
		setUIs ();

	}

	public void IncrementShop()
	{
		incrementShopEndPoint ();
		setUIs ();

	}

	public void DecrementShop()
	{
		decrementShopEndPoint ();
		setUIs ();

	}

	void incrementInvEndPoint()
	{
		Debug.Log (Inventory.playerInventory.inventoryItems.Count);
		if (playerInvEndPoint + 1 < Inventory.playerInventory.inventoryItems.Count) {
			playerInvEndPoint++;
		}
	}

	void decrementInvEndPoint()
	{
		if (playerInvEndPoint - 1 >= basketUIs.Count-1) {
			playerInvEndPoint--;
		}
	}

	void incrementShopEndPoint()
	{
		if (shopEndPoint + 1 < myShop.itemsIHave.Count) {
			shopEndPoint++;
		}
	}

	void decrementShopEndPoint()
	{
		if (shopEndPoint - 1 >= basketUIs.Count-1) {
			shopEndPoint--;
		}
	}

	void incrementBasketEndPoint()
	{
		if (basketEndPoint + 1 <itemsInBasket.Count) {
			basketEndPoint++;
		}
	}

	void decrementBasketEndPoint()
	{
		if (basketEndPoint - 1 >= 0) {
			basketEndPoint--;
		}
	}

	public void addItemToBasket(string item)
	{
		if (itemsInBasket == null) {
			itemsInBasket = new List<string> ();
		}
		itemsInBasket.Add (item);
		setUIs ();
	}

	public void removeItemFromBasket(string item)
	{
		itemsInBasket.Remove (item);
		setUIs ();
	}

	void setInfo()
	{
		float weight = 0;
		int price = 0;
		int quantity = 0;
		int myMoney = LoadingDataStore.me.getStashValue ();
		foreach (string st in itemsInBasket) {
			Item i = ItemDatabase.me.getItem (st).GetComponent<Item> ();
			if (i == null) {

			} else {
				weight += i.itemWeight;
				price += i.price;

			}
			quantity++;
		}

		moneyDisp.text = "£" + myMoney.ToString ();
		totalCost.text = "£" + price.ToString ();
		totalWeight.text = weight.ToString () + "KG";
		itemsInBasketDisp.text = quantity.ToString ();
	}

	public void reduceQuantityOfItem(string itemName){
		if (myShop.itemsIHave.Contains (itemName)) {
			int ind = myShop.itemsIHave.IndexOf (itemName);
			myShop.quantityIHave [ind]--;
		}
	}

	public void increaseQuantityOfItem(string itemName)
	{
		if (myShop.itemsIHave.Contains (itemName)) {
			int ind = myShop.itemsIHave.IndexOf (itemName);
			myShop.quantityIHave [ind]++;
		}
	}

	public void closeUI()
	{
		shopParent.SetActive (false);
	}
}
