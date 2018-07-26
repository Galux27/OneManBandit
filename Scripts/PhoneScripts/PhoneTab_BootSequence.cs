using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PhoneTab_BootSequence : PhoneTab {
	public Image background;
	public Text bootText;

	public string bootingText = "Booting phone..." + "\n" + "Starting hAXX.exe..." + "\n" + "Starting encryptionBreaker.exe" + "\n" + "Starting packetSniffer.exe" + "\n" + "Boot Successful..!";
	public string activeText = "";
	public float timer = 0.05f;
	public int counter=0;

	void Awake()
	{
		tabName = "Boot";
	}

	public override void enablePhoneTab ()
	{
		background.gameObject.SetActive (true);
		background.enabled = true;
		bootText.gameObject.SetActive (true);
		bootText.enabled = true;
		active = true;
	}

	public override void disablePhoneTab ()
	{
		background.gameObject.SetActive (false);
		background.enabled = false;
		bootText.gameObject.SetActive (false);
		bootText.enabled = false;
		active = false;
	}

	public override void onUpdate(){
		bootText.text = activeText;
		if (shouldWeAnimateBootText ()) {
			animateBootText ();
		} else {
			PhoneController.me.activePhoneTabs.Add(PhoneController.me.getPhoneTab("Sleeping"));

			PhoneController.me.activePhoneTabs.Add(PhoneController.me.getPhoneTab("Notes"));

			PhoneController.me.activePhoneTabs.Remove (this);
			disablePhoneTab ();
			PhoneController.me.currentTab = null;
		}
	}

	bool shouldWeAnimateBootText()
	{
		if (activeText != bootingText) {
			return true;
		} else {
			return false;
		}
	}

	void animateBootText()
	{
		timer -= Time.deltaTime;
		if (timer <= 0) {
			activeText += bootingText.ToCharArray () [counter];
			timer = 0.05f;
			counter++;
		}
	}

}
