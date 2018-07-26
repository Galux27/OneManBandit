using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerUIControl : MonoBehaviour {
	public static ComputerUIControl me;
	public GameObject myObject;
	public bool computerBeingUsed = false;

	public GameObject staffTab,cameraTab,radioTab,notesTab,floorplanTab,activeTab;

	void Awake()
	{
		if (me == null) {
			me = this;
		}
	}

	public void minimiseActiveTab()
	{
		if (activeTab == null) {

		} else {
			activeTab.SetActive (false);
			activeTab = null;
		}
	}

	public void setStaff()
	{
		if (activeTab == null) {

		} else {
			activeTab.SetActive (false);
			activeTab = null;
		}
		activeTab = staffTab;
		activeTab.SetActive (true);
	}

	public void setRadio()
	{
		if (activeTab == null) {

		} else {
			activeTab.SetActive (false);
			activeTab = null;
		}
		activeTab = radioTab;
		activeTab.SetActive (true);
	}

	public void setCamera()
	{
		if (activeTab == null) {

		} else {
			activeTab.SetActive (false);
			activeTab = null;
		}
		activeTab = cameraTab;
		activeTab.SetActive (true);
	}

	public void setNotes()
	{
		if (activeTab == null) {

		} else {
			activeTab.SetActive (false);
			activeTab = null;
		}
		activeTab = notesTab;
		activeTab.SetActive (true);
	}

	public void setFloorplan()
	{
		if (activeTab == null) {

		} else {
			activeTab.SetActive (false);
			activeTab = null;
		}
		activeTab = floorplanTab;
		activeTab.SetActive (true);
	}

	public void interactWithComputer()
	{
		if (computerBeingUsed == false) {
			//////Debug.Log ("Setting computer on");

			myObject.SetActive (true);
			computerBeingUsed = true;
		} else {
			//////Debug.Log ("Setting computer off");
			if (PlayerAction.currentAction == null) {

			} else {
				PlayerAction.currentAction.onComplete ();
			}

			computerBeingUsed = false;
			myObject.SetActive (false);
		}
	}

	public void downloadHack(string hack)
	{
		//PhoneTab_DownloadingHack.me.setHack (PlayerAction.currentAction.gameObject, hack, 20000);
	}
}
