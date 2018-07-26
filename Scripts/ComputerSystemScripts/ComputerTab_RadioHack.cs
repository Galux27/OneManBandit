using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerTab_RadioHack : ComputerTab {

	public override void downloadData ()
	{
		FindObjectOfType<PhoneTab_DownloadingHack>().setHack (CommonObjectsStore.player.transform.position, "RadioHack", 40000);
	}
}
