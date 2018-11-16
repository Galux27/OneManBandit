using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionObjective : MonoBehaviour {

	public string baseObjectiveText="";
	public bool activeInAllLevels=false;
	public string levelForObjective="";
	public bool isObjectiveStarted=false, isObjectiveDone = false,isMissionFailed=false;
	protected bool initialised=false;

	public bool objectiveCanBeFailed=false;
	public int objectiveStartedHour=0, objectiveStartedDay=0, objectiveStartedMonth=0;

	public bool limitedTime = false;
	public int timeLimitHours = 0;


	public virtual bool objectiveComplete()
	{
		return false;
	}

	public virtual string getObjectiveText()
	{
		return baseObjectiveText;
	}

	public bool shouldObjectiveBeActive()
	{
		if (activeInAllLevels == false) {
			return SceneManager.GetActiveScene ().name == levelForObjective;
		} else{
			return true;
		}
	}

	public virtual bool hasObjectiveBeenFailed()
	{
		if (objectiveCanBeFailed == false) {
			return false;
		}

		if (hasTooMuchTimePassed () == true) {
			return true;
		}

		return false;
	}

	public virtual string serializeObjective()
	{
		return isObjectiveStarted.ToString()+";"+ isObjectiveDone.ToString () + ";" + isMissionFailed.ToString () + ";" + objectiveStartedHour.ToString () + ";" + objectiveStartedDay.ToString () + ";"+objectiveStartedMonth.ToString();
	}

	public virtual void deserializeObjective(string st)
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

	public bool hasTooMuchTimePassed()
	{

		if (limitedTime == false) {
			return false;
		}


		System.DateTime curTime = new System.DateTime (TimeScript.me.year, TimeScript.me.month, TimeScript.me.day, TimeScript.me.hour, TimeScript.me.minute, 0);
		System.DateTime objStarted = new System.DateTime (TimeScript.me.year, objectiveStartedMonth, objectiveStartedDay, objectiveStartedHour, 0, 0);
		System.TimeSpan result = curTime.Subtract ( objStarted);
		if (result.TotalHours > timeLimitHours) {
			return true;
		} else {
			return false;
		}
	}
}

