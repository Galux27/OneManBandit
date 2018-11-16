using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MissionController : MonoBehaviour {
	public static MissionController me;
	public List<Mission> missions,activeMissions;

	void Awake()
	{
		me = this;
	}

	public void startMission(string name){
		foreach (Mission m in missions) {
			if (m.missionName == name) {
				activeMissions.Add (m);
				m.missionStarted = true;
				return;
			}
		}
	}

	public bool isMissionStarted(string name)
	{
		foreach (Mission m in activeMissions) {
			if (m.missionName == name) {
				return true;
			}
		}
		return false;
	}

    public bool isMissionFinished(string name)
    {
        foreach (Mission m in activeMissions)
        {
            if (m.missionName == name && m.missionFinished==true)
            {
                return true;
            }
        }
        return false;
    }

    public bool isMissionFailed(string name)
    {
        foreach (Mission m in activeMissions)
        {
            if (m.missionName == name && m.missionFailed == true)
            {
                return true;
            }
        }
        return false;
    }
    
}
