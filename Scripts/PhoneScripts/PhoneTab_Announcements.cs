using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PhoneTab_Announcements :PhoneTab {

	public Image background;
	public Text myText,index;
	public Button next,previous;

	int counter=0;
	public List<ComputerAnnouncementPrefab> announcements;
	public DoorScript[] doorsInLevel;

	void setAnnouncements()
	{
		if (ComputerTab_Announcements.me == null) {

		} else {
			if (announcements == null || announcements.Count == 0) {
				announcements = new List<ComputerAnnouncementPrefab> ();
				foreach (ComputerAnnouncementPrefab ca in ComputerTab_Announcements.me.myAnnouncements) {
					if (ca.assigned == true) {
						announcements.Add (ca);
					}
				}
			}
		}
	}

	void setText()
	{
		ComputerAnnouncementPrefab ca = announcements [counter];
		myText.text = "Name : " + ca.name.text + "\n" + "Announcement : " + ca.announcement.text;
		index.text = (counter + 1).ToString () + "/" + announcements.Count; 
	}

	public void nextAnnouncement()
	{
		if (counter < announcements.Count-1) {
			counter++;
		}
		setText ();
	}

	public void previousAnnouncement()
	{
		if (counter > 0) {
			counter--;
		}
		setText ();
	}

	public override void disablePhoneTab()
	{
		//disable all the UI elements needed
		//background.gameObject.SetActive(false);
		myText.gameObject.SetActive (false);
		index.gameObject.SetActive (false);
		next.gameObject.SetActive (false);
		previous.gameObject.SetActive (false);
		background.enabled = false;
		active = false;
	}

	public override void enablePhoneTab()
	{
		//enable all ui elements
		background.enabled=true;
		myText.gameObject.SetActive (true);
		index.gameObject.SetActive (true);
		next.gameObject.SetActive (true);
		previous.gameObject.SetActive (true);
		//zoomOutB.gameObject.SetActive (true);
		active = true;
		setAnnouncements ();
		setText ();
	}


	void OnGUI()
	{
		if (active == true && PhoneController.me.displayPhoneController == true) {

			DoorScript ds = announcements [counter].attachedToAnnouncement.GetComponent<DoorScript> ();

			if (ds == null) {

			} else {
				if (ds.myKey == null && ds.myCode == null) {
					
				} else if (ds.myKey != null) {

					if (ds.myKey.transform.parent == null) {

					} else if (ds.myKey.transform.parent.gameObject.tag == "Player") {
					}

					Vector3 posInScreen = CommonObjectsStore.me.mainCam.WorldToScreenPoint (ds.myKey.transform.position);
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
					GUI.DrawTexture (new Rect (posInScreen.x, posInScreen.y, 25, 25), CommonObjectsStore.me.key);
					//if (ds.wayIAmLocked == lockedWith.key) {

					///} else {

					//}
				} else if (ds.myCode != null) {

					if (ds.myCode.transform.parent == null) {

					} else if (ds.myCode.transform.parent.gameObject.tag == "Player") {
					}

					Vector3 posInScreen = CommonObjectsStore.me.mainCam.WorldToScreenPoint (ds.myCode.transform.position);
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
					GUI.DrawTexture (new Rect (posInScreen.x, posInScreen.y, 25, 25), CommonObjectsStore.me.note);

				}
			}
		}
	}
}
