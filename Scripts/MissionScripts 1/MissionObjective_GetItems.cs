using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionObjective_GetItems :MissionObjective {
	public List<string> itemsToGet;
	public bool doWeGoToSpecificPoint=false;
	public Vector3 positionToGoTo;
	public override bool hasObjectiveBeenFailed ()
	{
		if (hasTooMuchTimePassed () == true) {
			return true;
		}
		return false;
	}

	public override bool objectiveComplete ()
	{

		if (SceneManager.GetActiveScene ().name == levelForObjective) {
			if (doWeGoToSpecificPoint == true) {
				if (Vector2.Distance (positionToGoTo, CommonObjectsStore.player.transform.position) < 4) {
					if (doWeHaveTheItems () == true) {
						return true;
					}
				}
			} else {
				if (doWeHaveTheItems () == true) {
					return true;
				}
			}
		}

		return false;
	}

	bool doWeHaveTheItems()//removing the string item removes all instances not just the one so need to work around that
	{
		List<string> itemNames = itemsToGet;
		List<Item> toHandOver = new List<Item> ();
		foreach (Item i in Inventory.playerInventory.inventoryItems) {
			if (itemNames.Contains (i.itemName)) {
				itemNames.Remove (i.itemName);
				toHandOver.Add (i);
			}
		}


		if (itemNames.Count == 0) {
			foreach (Item i in toHandOver) {
                if(i.myContainer==null)
                {
                    Inventory.playerInventory.removeItemWithoutDrop(i);
                }
                else
                {
                    i.myContainer.removeItemFromContainer(i);

                }
                Destroy (i.gameObject);
			}
			return true;
		}

		foreach (GameObject g in CarSpawner.me.carsInWorld) {
			PlayerCarController pcc = g.GetComponent<PlayerCarController> ();
			if (pcc == null) {

			} else {
				if (pcc.playerCar == true || pcc.stolen == true) {
					foreach (Item i in pcc.carContainer.itemsInContainer) {
						if (itemNames.Contains (i.itemName)) {
							itemNames.Remove (i.itemName);
							toHandOver.Add (i);
						}
					}
				}
			}
		}
		if (itemNames.Count == 0) {
			foreach (Item i in toHandOver) {
                if (i.myContainer == null)
                {
                    Inventory.playerInventory.removeItemWithoutDrop(i);
                }
                else
                {
                    i.myContainer.removeItemFromContainer(i);

                }
                Destroy (i.gameObject);
			}
			return true;
		}
		return false;


	}

	string getItemsFormetted()
	{
		List<string> itemsList = new List<string> ();
		List<int> quantity = new List<int> ();

		foreach (string st in itemsToGet) {
			if (itemsList.Contains (st) == false) {
				itemsList.Add (st);
				quantity.Add (1);

			} else {
				int ind = itemsList.IndexOf (st);
				////Debug.LogError ("Item index was "+  ind+ " Total list length was " + quantity.Count);

				quantity [ind]++;
			}
		}
		string retVal = "";
		for (int x = 0; x < itemsList.Count; x++) {
			retVal += itemsList [x] + " X " + quantity [x].ToString () + ",";
		}
		return retVal;
	}

	public override string getObjectiveText ()
	{
		return baseObjectiveText + getItemsFormetted ();
	}

	public override string serializeObjective ()
	{
		return isObjectiveDone.ToString () + ";" + isMissionFailed.ToString () + ";" + objectiveStartedHour.ToString () + ";" + objectiveStartedDay.ToString () + ";"+objectiveStartedMonth.ToString();
	}

	public override void  deserializeObjective(string st)
	{
		string[] data = st.Split (new char[]{';'}, 0);

		if (data [0] == "True") {
			isObjectiveStarted = true;
		} else {
			isObjectiveStarted = false;
		}

		if (isObjectiveStarted == true) {
			if (data [1] == "True") {
				isObjectiveDone = true;
			} else {
				isObjectiveDone = false;
			}

			if (data [2] == "True") {
				isMissionFailed = true;
			} else {
				isMissionFailed = false;
			}

			objectiveStartedHour = int.Parse (data [3]);
			objectiveStartedDay = int.Parse (data [4]);
			objectiveStartedMonth = int.Parse (data [5]);


			initialised = true;
		}
	}
}
