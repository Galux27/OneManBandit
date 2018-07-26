using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemUI_2 : MonoBehaviour {

	public bool itemInInventory = false;
	public bool itemEquiped = false;

	public Text title;
	public Image icon;

	public Button examine,equip,drop,pickUp;
	public Item myItem;

	public void setItem(Item i){
		myItem = i;
		title.text = myItem.getItemName ();
		icon.sprite = myItem.itemTex;

		if (equip == null) {
			return;
		}

		equip.gameObject.SetActive (i.canEquip);	
		//use.gameObject.SetActive (i.canConsume);
	}

	public void clearItem()
	{
	//	toDisplay = null;
		//icon.sprite = null;
		//this.gameObject.SetActive( false);
	}
}
