using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MissionObjective_KillPerson :MissionObjective {

	public GameObject toKillInstance, toKillPrefab;
	public Vector3 positionToSpawn;
	NPCController toKillController;
	public int toKillID=-1;
	public bool doWeSpawnInEnemy=false;

	void Start()
	{
		setupEnemy ();
	}

	void initialiseObjective()
	{
		setupEnemy ();
		isObjectiveStarted = true;
		objectiveStartedHour = TimeScript.me.hour;
		objectiveStartedDay = TimeScript.me.day;
		objectiveStartedMonth = TimeScript.me.month;


		initialised = true;
	}

	void setupEnemy()
	{
		if (shouldObjectiveBeActive ()) {
			if (toKillInstance == null) {
				toKillInstance = (GameObject)Instantiate (toKillPrefab, positionToSpawn, Quaternion.Euler (0, 0, 0));
				if (toKillInstance == null) {

				} else {
					toKillController = toKillInstance.GetComponent<NPCController> ();
				}


            }
            else
            {
                toKillController = toKillInstance.GetComponent<NPCController>();

            }
        }
	}

	public override bool objectiveComplete ()
	{
		if (isObjectiveDone == true) {//was objective already finished and has been loaded in that state
			return true;
		} else {

			if (initialised == false) {
				initialiseObjective ();
			}

			if (objectiveCanBeFailed == true) {
				if (hasObjectiveBeenFailed () == true) {
					isMissionFailed = true;
					return false;
				}
			}

			if (isMissionFailed == true) {
				return false;
			}

			if (shouldObjectiveBeActive () == false) {//are we in the right level
				return false;
			} else {

				if (toKillController.myHealth.healthValue > 0) {//is NPC dead or not
					return false;
				} else {
					isObjectiveDone = true;
					return true;
				}
			}
		}

		return false;
	}

	public override string getObjectiveText ()
	{
		return baseObjectiveText;
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

			setupEnemy ();

			initialised = true;
		}
	}
}
