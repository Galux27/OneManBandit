using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhoneTab_Employees : PhoneTab {

	public Image background;
	public GameObject button1,button2;
	public Text myText,numberOfEmployees;
	public int counter = 0;

	public List<EmployeeData> data;
	public override void disablePhoneTab()
	{
		//disable all the UI elements needed
		//background.gameObject.SetActive(false);
		myText.gameObject.SetActive (false);
		background.enabled = false;
		button1.gameObject.SetActive (false);
		button2.gameObject.SetActive (false);
		numberOfEmployees.gameObject.SetActive (false);
		active = false;
	}

	public override void enablePhoneTab()
	{
		//enable all ui elements
		background.enabled=true;
		myText.gameObject.SetActive(true);
		//zoomOutB.gameObject.SetActive (true);
		background.enabled = true;
		button1.gameObject.SetActive (true);
		button2.gameObject.SetActive (true);
		numberOfEmployees.gameObject.SetActive (true);

		active = true;
	}

	void Update()
	{
		if (ComputerTab_EmployeeDatabase.me == null) {

		} else {
			if (data == null || data.Count == 0) {
				data = new List<EmployeeData> ();
				foreach (EmployeeData ed in ComputerTab_EmployeeDatabase.me.myData) {
					if (ed.gameObject.activeInHierarchy == false || ed.name.text=="Name1") {

					} else {
						data.Add (ed);
					}
				}
				setText ();
			}
		}
	}

	public void increaseCounter()
	{
		if (counter < data.Count-1) {
			counter++;
		}
		setText ();

	}

	public void decreaseCounter()
	{
		if (counter > 0) {
			counter--;
		}
		setText ();
	}

	void setText()
	{
		EmployeeData ed = data [counter];
		numberOfEmployees.text = (counter+1).ToString() + "/" + data.Count;
		myText.text = "Name: " + ed.name.text + "\n";
		myText.text += "Shift: " + ed.shift.text + "\n";
		myText.text += "Job: " + ed.doing.text + "\n";
		myText.text += "Details: " + ed.details.text;
	}

	void OnGUI()
	{
		if (active == true) {
			GameObject guard = data [counter].employee;

			Vector3 posInScreen = CommonObjectsStore.me.mainCam.WorldToScreenPoint (guard.transform.position);
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
			GUI.DrawTexture (new Rect (posInScreen.x, posInScreen.y, 25, 25), CommonObjectsStore.me.enemy);
			//if (ds.wayIAmLocked == lockedWith.key) {
		}
	}
}
