using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerTab_Announcements : ComputerTab {
	public static ComputerTab_Announcements me;
	public ComputerAnnouncementPrefab[] myAnnouncements;

	void Awake()
	{
		me = this;
		myAnnouncements = dataScrollObject.GetComponentsInChildren<ComputerAnnouncementPrefab> ();
		populateAnnouncements ();
	}

	void populateAnnouncements(){


		DoorScript[] doorsInLevel = FindObjectsOfType<DoorScript> ();
		int ind = 0;
		foreach (DoorScript ds in doorsInLevel) {
			if (ind >= myAnnouncements.Length) {
				//////Debug.LogError ("Too many keys to fit in announcements");
			} else {

				if (ds.myKey == null && ds.myCode == null) {
					continue;
				} else if (ds.myKey != null) {
					if (ds.wayIAmLocked == lockedWith.key) {
						RoomScript rs = LevelController.me.getRoomObjectIsIn (ds.myKey);
						myAnnouncements [ind].attachedToAnnouncement = ds.gameObject;
						if (rs == null) {
							myAnnouncements [ind].announcement.text = "Key is somewhere";
						} else {
							myAnnouncements [ind].announcement.text = "Key is in " + rs.roomName;
						}

					} else {
						RoomScript rs = LevelController.me.getRoomObjectIsIn (ds.myKey);
						myAnnouncements [ind].attachedToAnnouncement = ds.gameObject;

						if (rs == null) {
							myAnnouncements [ind].announcement.text = "Keycard is somewhere";
						} else {
							myAnnouncements [ind].announcement.text = "Keycard is in " + rs.roomName;
						}
					}
					myAnnouncements [ind].assigned = true;
					ind++;
				} else if (ds.myCode != null) {
					RoomScript rs = LevelController.me.getRoomObjectIsIn (ds.myCode);
					myAnnouncements [ind].attachedToAnnouncement = ds.gameObject;

					if (rs == null) {
						myAnnouncements [ind].announcement.text = "Keycode is somewhere";
					} else {
						myAnnouncements [ind].announcement.text = "Keycode is in " + rs.roomName;
					}
					myAnnouncements [ind].assigned = true;

					ind++;
				}
			}
		}
		foreach (ComputerAnnouncementPrefab ca in myAnnouncements) {
			if (ca.assigned == false) {
				ca.gameObject.SetActive (false);
			}
		}
	}

	public override void downloadData ()
	{
		PhoneTab_DownloadingHack.me.setHack (CommonObjectsStore.player.transform.position, "Announcements", 10000);

	}

}
