using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PhoneTab_DownloadingHack :PhoneTab{
	public static PhoneTab_DownloadingHack me;
	public Vector3 DownloadingFrom;
	public string hackDownloading;
	public int hackSize =0,hackProgress=0;
	public bool gettingHack = false;
	public Text display;
	public Image background;
	float signalStrength;
	void Awake()
	{
		tabName = "Hack Download";

		if (me == null) {
			me = this;
		}
	}



	void OnEnable()
	{
		tabName = "Hack Download";

		if (me == null) {
			me = this;
		}
	}
	public override void enablePhoneTab ()
	{
		display.enabled = true;
		background.enabled = true;
		active = true;
	}

	public override void disablePhoneTab ()
	{
		display.enabled = false;
		background.enabled = false;
		active = false;
	}



	public override void onUpdate(){
		if (gettingHack == true) {
			display.text = getDownloadingString ();
		} else {
			display.text = getCompleteDownload ();
		}

	}

	public void setHack(Vector3 position,string hack,int size)
	{
		if (gettingHack == false) {
			if (PhoneController.me.activePhoneTabs.Contains (this)==false) {
				PhoneController.me.activePhoneTabs.Add (this);
			}
			DownloadingFrom = position;
			hackDownloading = hack;
			hackSize = size;
			gettingHack = true;
		}
	}

	void Update()
	{
		if (gettingHack == true) {

			signalStrength = getDistanceDownloadModifier () * Time.deltaTime * 100;
			if (signalStrength < 0) {
				signalStrength = 0;
			}

			if (signalStrength > 100) {
				signalStrength = 100;
			}

//			////////Debug.Log(signalStrength);

			hackProgress += Mathf.RoundToInt(signalStrength);
			if (hackProgress >= hackSize) {
				hackProgress = hackSize;
				gettingHack = false;
				OnComplete ();
			}
		} else {

		}
	}

	public void OnComplete(){
		////////Debug.Log ("Hack downloaded");
		hackProgress=0;
		if (hackDownloading == "CCTVView") {

			if (PhoneController.me.activePhoneTabs.Contains (PhoneController.me.getPhoneTab ("CCTVView")) == true) {
				return;
			}
			PhoneAlert.me.setMessageText ("CCTV Hack is now installed.");
			PhoneController.me.activePhoneTabs.Add (PhoneController.me.getPhoneTab ("CCTVView"));
		} else if (hackDownloading == "RadioHack") {
			if (PhoneController.me.activePhoneTabs.Contains (PhoneController.me.getPhoneTab ("RadioHack")) == true) {
				return;
			}

			PhoneAlert.me.setMessageText ("Police radio listener is now installed.");
			PhoneController.me.activePhoneTabs.Add (PhoneController.me.getPhoneTab ("RadioHack"));
		} else if (hackDownloading == "Map") {
			if (PhoneController.me.activePhoneTabs.Contains (PhoneController.me.getPhoneTab ("Map")) == true) {
				return;
			}

			PhoneAlert.me.setMessageText ("Building blueprint viewer is now installed.");

			PhoneController.me.activePhoneTabs.Add (PhoneController.me.getPhoneTab ("Map"));
		} else if (hackDownloading == "Employees") {
			if (PhoneController.me.activePhoneTabs.Contains (PhoneController.me.getPhoneTab ("Employees")) == true) {
				return;
			}
			PhoneAlert.me.setMessageText ("Employee information viewer is now installed.");

			PhoneController.me.activePhoneTabs.Add (PhoneController.me.getPhoneTab ("Employees"));

		} else if (hackDownloading == "Announcements") {
			if (PhoneController.me.activePhoneTabs.Contains (PhoneController.me.getPhoneTab ("Announcements")) == true) {
				return;
			}
			PhoneAlert.me.setMessageText ("Announcements viewer is now installed.");

			PhoneController.me.activePhoneTabs.Add (PhoneController.me.getPhoneTab ("Announcements"));
		}
	}

	public string getDownloadingString()
	{
		return "Downloading data from connected device:" + "\n" + hackProgress + "/" + hackSize + "kb" + "\n" + "Signal Strength: " +Mathf.RoundToInt (signalStrength*10).ToString() + "%";
	}

	public string getCompleteDownload()
	{
		if (hackDownloading != "") {
			return "Download Complete, Hack " + hackDownloading + " installed.";
		} else {
			return "No download in progress...";
		}
	}



	public float getDistanceDownloadModifier()
	{
		return 8 / Vector3.Distance (DownloadingFrom, CommonObjectsStore.player.transform.position);

	}
		
}