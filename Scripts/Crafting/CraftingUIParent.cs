using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CraftingUIParent : MonoBehaviour {
	/// <summary>
	/// Class that controls the UI for the 3 CraftingUIItems and works out if there are any valid recipies from the components selected.
	/// </summary>

	public static CraftingUIParent me;
	public List<CraftingUIItem> items;

	public Image resultImage;
	public Text resultText;
	public Item toCraft;
	public List<CraftingRecipie> recipies;
	public bool displayCrafting = false;
	public GameObject craftObj;
	public CraftingRecipie itemWeAreCrafting;
	void Awake()
	{
		me = this;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (displayCrafting == false) {
			craftObj.SetActive (false);
		} else {
			craftObj.SetActive (true);
			canWeCraftAnything ();
		}
	}

	public bool isItemInUse(Item i)
	{
		foreach (CraftingUIItem cui in items) {
			if (cui.myItem == i) {
				return true;
			}
		}
		return false;
	}

	public void craftItem()
	{
		if (itemWeAreCrafting == null) {
			return;
		}

		foreach (CraftingUIItem ci in items) {
			if (ci.myItem == null) {
			} else {
				Inventory.playerInventory.removeItemWithoutDrop (ci.myItem);
			}
			ci.resetItem ();
		}

		foreach (string st in itemWeAreCrafting.results) {
			GameObject itemBase = ItemDatabase.me.getItem (st);
			if (itemBase == null) {

			} else {
				GameObject g = (GameObject)Instantiate (itemBase, CommonObjectsStore.player.transform.position, CommonObjectsStore.player.transform.rotation);
				Item i = g.GetComponent<Item> ();
				if (Inventory.playerInventory.canWeCarryItem (i) == true) {
					Inventory.playerInventory.addItemToInventory (i);
				}
			}
		}

		foreach (CraftingUIItem ci in items) {
			ci.resetItem ();
			ci.populateDropdown ();
		}
	

		//toCraft = null;
		resultText.text = "None";
		//resultImage.sprite = null;
	}

	void canWeCraftAnything()
	{
		CraftingRecipie c = null;
		foreach (CraftingRecipie ct in recipies) {
			List<string> recipieComponents = new List<string> ();
			if (ct.component1 != "None") {
				recipieComponents.Add (ct.component1);
			}

			if (ct.component2 != "None") {
				recipieComponents.Add (ct.component2);
			}

			if (ct.component3 != "None") {
				recipieComponents.Add (ct.component3);
			}

			bool anyNonNeededComponents = false;

			foreach (CraftingUIItem ci in items) {
				if (ci.myItem == null) {

				} else if (recipieComponents.Contains (ci.myItem.itemName) == false) {
					anyNonNeededComponents = true;
				}
			}

			foreach (CraftingUIItem ci in items) {
				if (ci.myItem == null) {
				} else {
					recipieComponents.Remove (ci.myItem.itemName);
				}
			}

			bool duplicate = false;

			foreach (CraftingUIItem ci in items) {
				foreach (CraftingUIItem ci2 in items) {
					if (ci.myItem == null || ci2.myItem == null) {
						
					} else {
						if (ci != ci2) {
							if (ci.myItem.itemName == ci2.myItem.itemName) {
								duplicate = true;
							}
						}
					}
				}
			}

			//Debug.Log ("Recipie " + ct.result + " has " + recipieComponents.Count + " components remaining");
			if (recipieComponents.Count == 0 && anyNonNeededComponents==false && duplicate==false) {
				c = ct;
				//setItemToCraft (ct);
			}
		}

		if (c == null) {
			resetItemToCraft ();
		} else {
			setItemToCraft (c);
		}
	}

	void setItemToCraft(CraftingRecipie ct)
	{
		//toCraft = ItemDatabase.me.getItem (st).GetComponent<Item> ();
		itemWeAreCrafting=ct;
		//resultImage.rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, toCraft.itemTex.texture.width*3);
		//resultImage.rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, toCraft.itemTex.texture.height*3);

		//resultImage.sprite = toCraft.itemTex;
		resultText.text = "Items Created:" + "\n";
		foreach (string st in ct.results) {
			resultText.text += st + "\n";
		}

		//resultText.text = toCraft.itemName;
	}

	public bool isItemBeingUsed(string name)
	{
		return false;

		foreach (CraftingUIItem ci in items) {
			if (ci.myItem == null) {
				
			}
			else{
				if (ci.myItem.itemName == name) {
					//Debug.LogError (ci.myItem.itemName + " Was not equal to " + name);
					return true;
				}
			}
		}
	}

	void resetItemToCraft()
	{
		itemWeAreCrafting=null;
		resultText.text = "";

	}

	public void resetAllUIButGiven(CraftingUIItem c)
	{
		foreach (CraftingUIItem ci in items) {
			if (ci != c && ci.myItem==null) {
				ci.populateDropdown ();
			}
		}
	}

}
