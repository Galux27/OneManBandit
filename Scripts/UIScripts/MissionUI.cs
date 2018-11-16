using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MissionUI : MonoBehaviour {
	public static MissionUI me;
	public GameObject missionUiParent;

	public Dropdown missionListDropdown;
	public Text missionName,currentObjectives,doneObjectives,reward;
	void Awake()
	{
		me = this;
		missionUiParent.SetActive (false);
	}

	// Use this for initialization
	void Start () {
		//setMissionUIs ();
	}
	
	// Update is called once per frame
	void Update () {
		//setActiveUI ();
	}

	void setMissionUIs()
	{
		List<string> st = new List<string> ();
		foreach (Mission m in MissionController.me.activeMissions) {
			st.Add (m.missionName);
		}

		missionListDropdown.ClearOptions ();
		missionListDropdown.AddOptions (st);	
	}

	void setActiveUI(){

		Mission active = MissionController.me.activeMissions [missionListDropdown.value];

		string addition = "";
		if (active.missionFinished == true) {
			addition = " - Finished";
		} else if (active.missionFailed == true) {
			addition = " - Failed";
		}

		missionName.text = "Current Mission : "+  active.missionName + addition;
		currentObjectives.text = "Current Objective : " +active.objectivesInMission [active.currentObjectiveIndex].getObjectiveText ();
		string doneObjs = "Done Objectives: " + "\n";
		for (int x = 0; x < active.currentObjectiveIndex; x++) {
			doneObjs += "-"+active.objectivesInMission [x].getObjectiveText ()+ "\n";;
		}
		doneObjectives.text = doneObjs;
		reward.text = "Reward : " + active.getRewardAsString();

	}

	public void enableMissionUI()
	{
        return;
		missionUiParent.SetActive (true);
	}

	public void disableMissionUI()
	{
		missionUiParent.SetActive (false);
	}

	public bool isMissionUIAvtive()
	{
		return missionUiParent.activeInHierarchy;
	}
}
