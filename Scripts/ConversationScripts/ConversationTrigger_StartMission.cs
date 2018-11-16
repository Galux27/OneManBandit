using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationTrigger_StartMission  : ConversationTrigger {

	public string missionToStart = "";
	public override void OnOptionSelect()
	{
		if (MissionController.me.isMissionStarted (missionToStart) == false) {
			MissionController.me.startMission (missionToStart);
		}
	}



}