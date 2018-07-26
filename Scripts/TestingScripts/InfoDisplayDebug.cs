using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InfoDisplayDebug : MonoBehaviour {
	public Text display;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		setDisplay ();	
	}

	void setDisplay()
	{
		display.text = "";
		string st = "";

		if (NPCBehaviourDecider.copAlarm == true) {
			st+= " Cops are alerted || ";
		}

		if (NPCBehaviourDecider.globalAlarm == true) {
			st += " Guards are alerted || ";
		}

		display.text = st;
	}
}
