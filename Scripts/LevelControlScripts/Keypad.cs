using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Keypad : MonoBehaviour {
	public static Keypad me;
	public GameObject guiObject;
	public DoorScript doorToOpen;

	public string displayString = "";
	public Text display;
	void Awake()
	{
		me = this;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		display.text = displayString;

		if (doorToOpen == null) {
			guiObject.SetActive (false);
		}
	}

	public bool isKeypadInUse()
	{
		return guiObject.activeInHierarchy;
	}

	public void enableGUI(DoorScript door)
	{
		doorToOpen = door;
		guiObject.SetActive (true);
	}

	public void disableGUI()
	{
		doorToOpen = null;
		PlayerAction.currentAction = null;
		displayString = "";
		guiObject.SetActive (false);
	}

	public void removeCharacter()
	{

		displayString = "";

		//if (displayString != "") {
		//	displayString.Remove (displayString.Length - 1);
		//}
	}

	public void addOne()
	{
		if (displayString.ToCharArray ().Length < 4) {
			displayString += "1";
		}
	}

	public void addTwo()
	{
		if (displayString.ToCharArray ().Length < 4) {
			displayString += "2";

		}
	}

	public void addThree()
	{
		if (displayString.ToCharArray ().Length < 4) {
			displayString += "3";

		}
	}

	public void addFour()
	{
		if (displayString.ToCharArray ().Length < 4) {
			displayString += "4";

		}
	}

	public void addFive()
	{
		if (displayString.ToCharArray ().Length < 4) {
			displayString += "5";

		}
	}

	public void addSix()
	{
		if (displayString.ToCharArray ().Length < 4) {
			displayString += "6";

		}
	}

	public void addSeven()
	{
		if (displayString.ToCharArray ().Length < 4) {
			displayString += "7";

		}
	}

	public void addEight()
	{
		if (displayString.ToCharArray ().Length < 4) {
			displayString += "8";

		}
	}

	public void addNine()
	{
		if (displayString.ToCharArray ().Length < 4) {
			displayString += "9";

		}
	}

	int getInputNumber()
	{
		try{
			return int.Parse(displayString);
		}
		catch{
			return 0;
		}
	}

	public void enterCode()
	{
		if (getInputNumber () == doorToOpen.keycodeNumber) {
			doorToOpen.locked = false;
			doorToOpen.interactWithDoor (CommonObjectsStore.player);
			PlayerAction.currentAction = null;
			disableGUI ();
		} else {
			//have a beep to indicate its wrong
		}
	}
}

