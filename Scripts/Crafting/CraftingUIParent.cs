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
		if (toCraft == null) {
			return;
		}
		GameObject g = (GameObject)Instantiate (toCraft.gameObject, CommonObjectsStore.player.transform.position, CommonObjectsStore.player.transform.rotation);
		Item i = g.GetComponent<Item> ();
		Inventory.playerInventory.addItemToInventory (i);
		foreach (CraftingUIItem ci in items) {
			if (ci.myItem == null) {
			} else {
				Inventory.playerInventory.removeItemWithoutDrop (ci.myItem);
			}
			ci.resetItem ();
		}
		toCraft = null;
		resultText.text = "None";
		resultImage.sprite = null;
	}

	void canWeCraftAnything()
	{
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

			foreach (CraftingUIItem ci in items) {
				if (ci.myItem == null) {
				} else {
					recipieComponents.Remove (ci.myItem.itemName);
				}
			}
			Debug.Log ("Recipie " + ct.result + " has " + recipieComponents.Count + " components remaining");
			if (recipieComponents.Count == 0) {
				setItemToCraft (ct.result);
			}
		}
	}

	void setItemToCraft(string st)
	{
		toCraft = ItemDatabase.me.getItem (st).GetComponent<Item> ();

		resultImage.rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, toCraft.itemTex.texture.width*3);
		resultImage.rectTransform.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, toCraft.itemTex.texture.height*3);

		resultImage.sprite = toCraft.itemTex;
		resultText.text = toCraft.itemName;
	}


}
