using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to control shops  
/// </summary>
public class Shop : MonoBehaviour {
	public static List<Shop> shopsInWorld;
	public List<string> itemsICouldSell,itemsIHave,itemsIBuy;
	public List<int> quantityIHave;
	int min,hour,day,month,year;//date of generation
	public bool shopAvailable = true;
	public GameObject myKeeper;
	NPCController keeperController;
	NPCBehaviourDecider keeperDecider;

	void Awake()
	{
		if (itemsIHave == null || itemsIHave.Count == 0) {
			generateInventory ();
		}

		if (shopsInWorld == null) {
			shopsInWorld = new List<Shop> ();
		}
		shopsInWorld.Add (this);
	}

	void Start()
	{
		myKeeper = (GameObject)Instantiate (CommonObjectsStore.me.civilian,new Vector3(this.transform.position.x,this.transform.position.y,0), this.transform.rotation);
		keeperController = myKeeper.GetComponent<NPCController> ();
		keeperDecider = myKeeper.GetComponent<NPCBehaviourDecider> ();
		keeperController.myType = AIType.shopkeeper;
		keeperDecider.myType = AIType.shopkeeper;
	}

	void Update()
	{
		if (keeperController.currentBehaviour == null) {
			shopAvailable = false;
		} else {
			if (keeperController.currentBehaviour.myType == behaviourType.shopkeeper) {
				if (Vector2.Distance (myKeeper.transform.position, this.transform.position) < 4.0f) {
					shopAvailable = true;
				} else {
					shopAvailable = false;
				}
			} else {
				shopAvailable = false;
			}
		}
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
