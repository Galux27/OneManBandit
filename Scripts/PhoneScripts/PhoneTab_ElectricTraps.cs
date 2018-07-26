using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PhoneTab_ElectricTraps : PhoneTab {
	public static PhoneTab_ElectricTraps me;


	public Text numTrapsDisplay,currentTrapDisplay,buttonInfo;
	public Button b1,b2,b3,b4;
	public Image bg;
	LaserTrapItem currentTrap;
	public int index=0;

	public List<LaserTrapItem> trapsInWorld;
	void Awake()
	{
		me = this;
		trapsInWorld = new List<LaserTrapItem> ();
	}

	// Use this for initialization
	void Start () {
	}

	public override void onUpdate(){
		

		if (trapsInWorld.Count > 0) {
			currentTrap = trapsInWorld [index];
		}
		setNumTrapsDisplay ();
		setCurrentTrapInfo ();
	}

	public override void enablePhoneTab ()
	{
		b1.gameObject.SetActive (true);
		b2.gameObject.SetActive (true);
		b3.gameObject.SetActive (true);
		b4.gameObject.SetActive (true);
		numTrapsDisplay.gameObject.SetActive( true);
		currentTrapDisplay.gameObject.SetActive( true);
		buttonInfo.gameObject.SetActive (true);
		active = true;
	}

	public override void disablePhoneTab ()
	{

		b1.gameObject.SetActive (false);
		b2.gameObject.SetActive (false);

		b3.gameObject.SetActive (false);
		b4.gameObject.SetActive (false);

		numTrapsDisplay.gameObject.SetActive(false);
		currentTrapDisplay.gameObject.SetActive(false);
		buttonInfo.gameObject.SetActive (false);
		active = false;
	}
	// Update is called once per frame
	void Update () {
		if (PhoneController.me.currentTab == this) {
			bg.enabled = true;
		} else {
			bg.enabled = false;
		}
	}

	public void setNumTrapsDisplay()
	{

		if (trapsInWorld.Count == 0) {
			numTrapsDisplay.text = "0/0";
		} else {
			numTrapsDisplay.text = (index + 1).ToString () + "/" + trapsInWorld.Count.ToString ();
		}
	}

	public void setCurrentTrapInfo()
	{
		if (currentTrap == null) {
			currentTrapDisplay.text = "No trap placed";
			return;
		}


		RoomScript r = LevelController.me.getRoomObjectIsIn (currentTrap.gameObject);
		string itemInfo = "";
		itemInfo += "Trap : " + currentTrap.itemName + "\n";

		if (r == null) {

		} else {
			itemInfo += "Location : " + r.roomName + "\n";
		}

		if (currentTrap.canDetect == true) {
			itemInfo += "Active : True" + "\n";
		} else {
			itemInfo += "Active : False " + "\n";
		}

		if (currentTrap.created == true) {
			itemInfo += "Triggered : True" + "\n";

		} else {
			itemInfo += "Triggered : False" + "\n";

		}

		currentTrapDisplay.text = itemInfo;
	}

	public void increaseIndex()
	{
		if (index < 0) {
			index = 0;
		}

		if (index >= trapsInWorld.Count) {
			index = trapsInWorld.Count - 1;
		}

		if (index <trapsInWorld.Count) {
			index++;
		} else {
			index = 0;
		}
	}

	public void decreaseIndex()
	{
		if (index < 0) {
			index = 0;
		}

		if (index >= trapsInWorld.Count) {
			index = trapsInWorld.Count - 1;
		}

		if (index > 0) {
			index--;
		} else {
			index = trapsInWorld.Count - 1;
		}
	}

	public void disableCurrentTrap(){
		if (currentTrap == null) {
			return;
		}

		currentTrap.canDetect = !currentTrap.canDetect;
	}

	public void detonateCurrentTrap()
	{
		if (currentTrap == null) {
			return;
		}

		currentTrap.detonate ();
	}
}
