using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationTrigger_AddItem : ConversationTrigger {

	public List<string> itemsToAdd;

	public override void OnOptionSelect()
	{
		foreach (string st in itemsToAdd) {
			GameObject g = ItemDatabase.me.getItem (st);
			if (g == null) {
				continue;
			}

			GameObject item = (GameObject)Instantiate(g,CommonObjectsStore.player.transform.position,Quaternion.Euler(Vector3.zero));
			Item itemScript = item.GetComponent<Item> ();
			Inventory i = CommonObjectsStore.player.GetComponent<Inventory> ();
			i.addItemToInventory (itemScript);
		}
	}


	public void addItem(string item)
	{
		if (itemsToAdd == null) {
			itemsToAdd = new List<string> ();
		}

		itemsToAdd.Add (item);
	}

	public List<string> getItemsToAdd()
	{
		if (itemsToAdd == null) {
			itemsToAdd = new List<string> ();
		}
		return itemsToAdd ;
	}
}
