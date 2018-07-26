using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour {

	public List<string> itemsICouldSell,itemsIHave,itemsIBuy;
	public List<int> quantityIHave;
	int min,hour,day,month,year;//date of generation

	void Awake()
	{
		if (itemsIHave == null || itemsIHave.Count == 0) {
			generateInventory ();
		}
	}

	void Start()
	{
		//ShopUI.me.enableShopUI (this);
	}

	void generateInventory()
	{
		itemsIHave = new List<string> ();
		quantityIHave = new List<int> ();
		foreach (string st in itemsICouldSell) {
			int r = Random.Range (0, 100);

			if (r < 40) {
				itemsIHave.Add (st);
				quantityIHave.Add (Random.Range (1, 5));
			}
		}
	}

}
