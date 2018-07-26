using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PhoneTab_Sleeping : PhoneTab {
	public Text myText,appCount,hint;
	public Image background;
	public override void disablePhoneTab()
	{
		//disable all the UI elements needed
		myText.enabled=false;
		hint.enabled = false;
		appCount.enabled = false;
		background.enabled = false;
		active = false;
	}

	public override void enablePhoneTab()
	{
		//enable all ui elements
		background.enabled=true;
		myText.enabled = true;
		appCount.enabled = true;
		hint.enabled = true;
		active = true;
	}

	public override void onUpdate()
	{
		//do any shit that needs to be done every frame
		myText.text = TimeScript.me.getTime();
		appCount.text = "Apps Running - " + PhoneController.me.activePhoneTabs.Count;
		if (PhoneController.me.activePhoneTabs.Count > 1) {
			//get rid of this and make the other one the active one
		} else {
			//PhoneController.me.activePhoneTabs.Add (PhoneController.me.getPhoneTab ("Map"));
			//PhoneController.me.activePhoneTabs.Add (PhoneController.me.getPhoneTab ("CCTVView"));
			//PhoneController.me.activePhoneTabs.Add (PhoneController.me.getPhoneTab ("RadioHack"));

			PhoneController.me.activePhoneTabs.Remove (this);
			PhoneController.me.currentTab = null;
			disablePhoneTab ();
		}
	}

}
