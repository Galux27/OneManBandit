using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission : MonoBehaviour {
	//should be able to serialize a mission once its started, have a list of true/false values to see which objectives are done

	public int currentObjectiveIndex=0;
	public bool missionStarted=false;
	public bool missionFinished=false;
	public bool missionFailed=false;
	public string missionName="";
	public List<MissionObjective> objectivesInMission;

	bool givenReward=false;
	public MissionReward myReward;
	bool doneAlert=false;

	public GameObject missionGiver;
	public NPCController missionGiverController;

	void Start()
	{
		if (missionStarted == true && MissionController.me.activeMissions.Contains (this) == false) {
			MissionController.me.activeMissions.Add (this);
		}

		if (missionGiver == null) {

		} else {
			missionGiverController = missionGiver.GetComponent<NPCController> ();
		}
	}

	public bool isMissionComplete()
	{
		if (missionFinished == false) {
			missionFinished = objectivesInMission [objectivesInMission.Count - 1].objectiveComplete ();

			return missionFinished;
		} else {
			return true;
		}
	}

	bool isMissionGiverDead()
	{
		if (missionGiver == null) {
			return false;
		} else {
			if (missionGiverController == null) {
				return false;
			} else {
				if (missionGiverController.myHealth.healthValue <= 0) {
					return true;
				} else {
					return false;
				}
			}
		}
		return false;

	}

	void Update()
	{
		/*if (missionStarted==true && missionFailed == false && missionFinished==false) {
			if(objectivesInMission [currentObjectiveIndex].isMissionFailed==true && missionFailed==false || isMissionGiverDead()==true && missionFailed==false){
				missionFailed = true;
				PhoneAlert.me.setMessageText (missionName + ": Failed");
			}else{
				if (objectivesInMission [currentObjectiveIndex].objectiveComplete () == true) {
					doneAlert = false;
					if (currentObjectiveIndex == objectivesInMission.Count - 1) {
						if (doneAlert == false) {
							PhoneAlert.me.setMessageText (missionName + ": Complete");
							doneAlert = true;
						}
						missionFinished = true;
						if (givenReward == false) {
							giveReward ();
						}
					} else {
						if (doneAlert == false) {
							PhoneAlert.me.setMessageText (objectivesInMission [currentObjectiveIndex + 1].getObjectiveText ());
							doneAlert = true;
						}
					}
					currentObjectiveIndex++;
				}
			}
		}*/
	}

	public string getFileName()
	{
		if (missionFinished == true) {
			return missionName + "Complete" + ".txt";
		} else if (missionFailed == true) {
			return missionName + "Failed" + ".txt";
		} else {
			return missionName + ".txt";
		}
	}

	public List<string> serializeMission()
	{
		List<string> retVal = new List<string> ();
		foreach (MissionObjective m in objectivesInMission) {
			retVal.Add (m.serializeObjective ());
		}
		return retVal;
	}

	public void deserializeMission(string[] data, string fileName)
	{
		missionStarted = true;
		if (fileName.Contains ("Complete")) {
			missionFinished = true;
			givenReward = true;
		} else if (fileName.Contains ("Failed") == true) {
			missionFailed = true;
		}

		for (int x = 0; x < objectivesInMission.Count; x++) {
			objectivesInMission [x].deserializeObjective (data [x]);
		}
	}

	public string getRewardAsString()
	{
		if (myReward == null) {
			return "None";
		}

		List<string> items = new List<string> ();
		List<int> quantity = new List<int> ();
		string retVal = "";
		foreach (string st in myReward.itemsToGive) {
			if (items.Contains (st) == false) {
				items.Add (st);
				quantity.Add (1);
			} else {
				quantity [items.IndexOf (st)]++;
			}
		}

		for (int x = 0; x < items.Count; x++) {
			retVal += items [x] + " X " + quantity [x].ToString () + ",";
		}

		if (myReward.moneyToGive > 0) {
			retVal += " £" + myReward.moneyToGive.ToString ();
		}
		return retVal;
	}

	void giveReward()
	{
		if (myReward == null) {
			return;
		}

		foreach (string st in myReward.itemsToGive) {
			GameObject g = ItemDatabase.me.getItem (st);
			if (g == null) {

			} else {
				GameObject instance = (GameObject) Instantiate (g, CommonObjectsStore.player.transform.position, CommonObjectsStore.player.transform.rotation);
				Item i = instance.GetComponent<Item> ();
				Inventory.playerInventory.addItemToInventory (i);
			}
		}
		int val = LoadingDataStore.me.getStashValue ();
		LoadingDataStore.me.setStashValue (val + myReward.moneyToGive);

		MissionController.me.startMission (myReward.missionToActivate);
		
	}
}
//will also need to have some kind of condition to work out whether the mission giver has been killed so that missions can be failed (Done)

/*TODO
 * Make it so that civilians have a chance of recognising the player if the investigation value is high enough
 * Write mission editor
 * Create a way for important NPCs to be created, saved and loaded
 * Write editor for important npcs
 * Change conversation so that it can have more than three options (DONE)
 * Add option for starting missions from conversations (DONE)
 * Add calls to serialize missions from loadingdatastore (DONE)
 * Rework bits of gui (conversation, inventory) so the arrows appear/disappear to indicate items above/below in list (DONE)
 * Make sure conversation creation UI can handle to two new options for the conversation triggers (money,missions) (DONE)
 * Make a way for unique NPCs to be recorded and their state monitered (work on NPCID system).
 * */
