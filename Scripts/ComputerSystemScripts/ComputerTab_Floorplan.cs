using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ComputerTab_Floorplan : ComputerTab {
	public RawImage mapDisplay;

	void Update()
	{
		mapDisplay.texture = PhoneTab_Map.me.mapBase; //will need to work out a way to allow for multiple maps of levels
	}

	public override void downloadData ()
	{
		PhoneTab_DownloadingHack.me.setHack (CommonObjectsStore.player.transform.position, "Map", 30000);
	}
}
