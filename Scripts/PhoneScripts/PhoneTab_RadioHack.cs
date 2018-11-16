using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhoneTab_RadioHack : PhoneTab {
	public static PhoneTab_RadioHack me;
	public Text infoDisplay,channelDisplay;
	public radioHackBand myBand;
	public Button b1, b2;
	void Awake()
	{
		me = this;
	}
	// Update is called once per frame
	public override void onUpdate(){
		setText ();
	}

	public void setNewText(string newText,radioHackBand type)
	{
		if (me == null) {
			me = this;
		}

		return;

		//setText ();
	}

	public void setText()
	{
		string text = "";


		text += "Civilians: ";

		if (areCiviliansAlarmed () == true) {
			text += "Someone is calling the police";
		} else {
			text += "No civilians are alerted.";
		}

		text += "\n";

		text += "Police: ";

		if (PoliceController.me.copsCalled == true && PoliceController.me.copsHere == false) {
			text += "First wave on the way, arriving in " + Mathf.RoundToInt (PoliceController.me.policeTimer).ToString ();
		}
		else if(PoliceController.me.copsCalled == true && PoliceController.me.copsHere == true)
		{
			text += "First wave have arrived,";

			if (LevelController.me.suspects.Contains (CommonObjectsStore.player) == true) {
				text += "They are searching for you.";
			} else {
				text += "They are searching the area for trouble.";
			}
		}
		else {
			text += "No police are alerted.";
		}
		text += "\n";

		if (PoliceController.me.backupCalled == false) {

		} else if (PoliceController.me.backupCalled == true && PoliceController.me.backupHere == false) {
			text += "Police have called for backup, arriving in " + Mathf.RoundToInt (PoliceController.me.policeBackup).ToString ();
		} else if (PoliceController.me.backupHere == true) {
			text += "Police backup has arrived";
		}

		text += "\n";
		text += "SFCO: ";

		if (PoliceController.me.swatCalled == false) {

		} else if (PoliceController.me.swatCalled == true && PoliceController.me.swatHere == false) {
			text += "Armed police have been called, arriving in " + Mathf.RoundToInt (PoliceController.me.swatTimer).ToString ();

		} else if (PoliceController.me.swatHere == true) {
			text += "Armed police have arrived";

		} else {
			text += "No Armed police are here.";
		}

		infoDisplay.text = text;
	}


	bool areCiviliansAlarmed()
	{
		foreach (NPCController npc in NPCManager.me.npcControllers) {

			if (npc.npcB.myType == AIType.civilian) {
				if (npc.currentBehaviour == null) {
					continue;
				}

				if (npc.currentBehaviour.myType == behaviourType.raiseAlarm || npc.npcB.alarmed == true) {
					return true;
				}
			}
		}
		return false;
	}

	public void nextBand()
	{
		if (myBand == radioHackBand.buisness) {
			myBand = radioHackBand.cop;
			channelDisplay.text = "Police";

		} else if (myBand == radioHackBand.cop) {
			myBand = radioHackBand.swat;
			channelDisplay.text = "SWAT";

		} else {
			myBand = radioHackBand.buisness;
			channelDisplay.text = "Security";

		}
		setText ();
	}

	public void previousBand()
	{
		if (myBand == radioHackBand.buisness) {
			myBand = radioHackBand.swat;
			channelDisplay.text = "SWAT";
		} else if (myBand == radioHackBand.cop) {
			myBand = radioHackBand.buisness;
			channelDisplay.text = "Security";

		} else {
			myBand = radioHackBand.cop;
			channelDisplay.text = "Police";

		}
		setText ();
	}

	public override void disablePhoneTab()
	{
		channelDisplay.gameObject.SetActive (false);
		b1.gameObject.SetActive (false);
		b2.gameObject.SetActive (false);
		infoDisplay.gameObject.SetActive (false);
		active = false;
	}

	public override void enablePhoneTab()
	{
		channelDisplay.gameObject.SetActive (true);

		b1.gameObject.SetActive (true);
		b2.gameObject.SetActive (true);
		infoDisplay.gameObject.SetActive (true);

		setText();
		active = true;
	}


	void OnGUI()
	{
		if (active == true && PhoneController.me.displayPhoneController == true) {
			

			foreach (GameObject g in NPCManager.me.npcsInWorld) {
				if (g == null) {
					continue;
				}

				NPCController npc = g.GetComponent<NPCController> ();

				if (npc.myHealth.healthValue <= 0) {
					continue;
				}

				if (npc.npcB.myType == AIType.cop && myBand == radioHackBand.cop || npc.npcB.myType == AIType.guard && myBand == radioHackBand.buisness || npc.npcB.myType == AIType.swat && myBand == radioHackBand.swat) {
					Vector3 posInScreen = CommonObjectsStore.me.mainCam.WorldToScreenPoint (g.transform.position);
					posInScreen = new Vector3 (posInScreen.x, Screen.height - posInScreen.y, 0);

					if (posInScreen.x > Screen.width - 50) {
						posInScreen = new Vector3 (Screen.width - 50, posInScreen.y, 0);
					} else if (posInScreen.x < 50) {
						posInScreen = new Vector3 (50, posInScreen.y, 0);
					}

					if (posInScreen.y > Screen.height - 50) {
						posInScreen = new Vector3 (posInScreen.x, Screen.height - 50, 0);
					} else if (posInScreen.y < 50) {
						posInScreen = new Vector3 (posInScreen.x, 50, 0);
					}

					//GUI.Box (new Rect (posInScreen.x, posInScreen.y, 10, 10), "");
					GUI.DrawTexture (new Rect (posInScreen.x, posInScreen.y, 10, 10), CommonObjectsStore.me.enemy);
				}
			
			
			}
		}
	}

}



public enum radioHackBand{
	swat,
	cop,
	buisness
}
