using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PhoneTab_Notes  : PhoneTab {
	public static PhoneTab_Notes me;
	public Image background;
	public GameObject button1,button2;
	public Text myText,numberOfNotes,title;
	public int counter = 0;
	public List<string> messages = new List<string> ();
	public List<PhoneMessageMarker> messagesToAdd = new List<PhoneMessageMarker>();

	void Awake()
	{
		me = this;
		messages.Add ("Any important information will be written here.");
	}

	void Update()
	{
		foreach (PhoneMessageMarker p in messagesToAdd) {
			if (p.addedMessage == false) {
				if (p.shouldWeAddMessage () == true) {
					PhoneAlert.me.setMessageText (p.getMessage ());
					messages.Add (p.getMessage());
					p.addedMessage = true;
					setMessage ();
				}
			}
		}
	}

	public override void disablePhoneTab()
	{
		//disable all the UI elements needed
		//background.gameObject.SetActive(false);
		myText.gameObject.SetActive (false);
		numberOfNotes.gameObject.SetActive (false);
		title.gameObject.SetActive (false);
		background.enabled = false;
		button1.gameObject.SetActive (false);
		button2.gameObject.SetActive (false);
		numberOfNotes.enabled = false;
		active = false;
	}

	public override void enablePhoneTab()
	{
		//enable all ui elements
		background.enabled=true;
		myText.gameObject.SetActive(true);
		numberOfNotes.gameObject.SetActive (true);
		title.gameObject.SetActive (true);
		//zoomOutB.gameObject.SetActive (true);
		button1.gameObject.SetActive (true);
		button2.gameObject.SetActive (true);
		numberOfNotes.enabled = true;

		active = true;
	}



	public void increaseCounter()
	{
		if (counter < messages.Count - 1) {
			counter++;
		}
		setMessage ();
	}

	public void decreaseCounter()
	{
		if (counter > 0) {
			counter--;
		}
		setMessage ();
	}

	void setMessage()
	{
		numberOfNotes.text = (counter + 1).ToString () + "/" + messages.Count;
		myText.text = messages [counter];
	}
}
