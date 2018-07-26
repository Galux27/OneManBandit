using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerTab_EmployeeDatabase : ComputerTab {
	public static ComputerTab_EmployeeDatabase me;
	public EmployeeData[] myData;
	void Awake(){
		me = this;
		lastUiCreated = dataScrollObject.GetComponentInChildren<EmployeeData> ().gameObject;
		myData = dataScrollObject.GetComponentsInChildren<EmployeeData> ();
	}

	void Start()
	{
		//foreach (EmployeeData ed in myData) {
		//	ed.name.text = Random.Range (0, 1000).ToString ();
		//}


	}

	bool setValues = false;

	void Update()
	{
		if (setValues == false) {
			int counter = 0;
			if (counter < myData.Length) {
				foreach (NPCController npc in NPCManager.me.npcControllers) {
					if (npc.npcB.myType == AIType.guard) {
						EmployeeData d = myData [counter];
						d.employee = npc.gameObject;
						if (npc.npcB.patrol == true) {
							List<RoomScript> roomsRouteGoesThrough = new List<RoomScript> ();
							PatrolRoute p = npc.memory.myRoute;
							foreach (GameObject g in p.pointsInRoute) {
								RoomScript r = LevelController.me.getRoomObjectIsIn (g);
								if (r == null) {

								} else {
									if (roomsRouteGoesThrough.Contains (r) == false) {
										roomsRouteGoesThrough.Add (r);
									}
								}
							}
							string routeRooms = "";
							if (roomsRouteGoesThrough.Count == 0) {
								routeRooms = "Outdoors";
							} else {
								foreach (RoomScript r in roomsRouteGoesThrough) {
									routeRooms += r.roomName + ", ";
								}
							}
							d.name.text = npc.gameObject.name;
							d.doing.text = "Patroling " + routeRooms;
							d.details.text = "123 Alba Street";
						} else {
							RoomScript r = LevelController.me.getRoomObjectIsIn (npc.gameObject);
							if (r == null) {
								d.name.text = npc.gameObject.name;
								d.doing.text = "Could not get room NPC was in";
								d.details.text = "123 Alba Street";
							} else {
								d.name.text = npc.gameObject.name;
								d.doing.text = "Guarding " + r.roomName;
								d.details.text = "123 Alba Street";
							}
						}
					}
					counter++;
				}
			}

			foreach (EmployeeData ed in myData) {
				if (ed.name.text == "Name1") {
					ed.gameObject.SetActive (false);
				}
			}
			setValues = true;
		}

	}

	public override void downloadData ()
	{
		PhoneTab_DownloadingHack.me.setHack (CommonObjectsStore.player.transform.position, "Employees", 10000);
	}

	public EmployeeData[] getData()
	{
		if (myData == null) {
			myData = dataScrollObject.GetComponentsInChildren<EmployeeData> ();
		}
		return myData;
	}

}
