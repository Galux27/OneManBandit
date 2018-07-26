using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneController : MonoBehaviour {
	public static PhoneController me;

	public GameObject phoneObject;
	public bool displayPhoneController;
	public RectTransform myTransform;

	Vector3Int closed = new Vector3Int(-304,132,20),open=new Vector3Int(-304,304,20);

	public PhoneTab currentTab;

	public List<PhoneTab> allPhoneTabs;
	public List<PhoneTab> activePhoneTabs;
	public int phoneTabCounter=0;

	public bool booted=false;

	void Awake()
	{
		me = this;
		myTransform = phoneObject.GetComponent<RectTransform> ();
	}

	// Use this for initialization
	void Start () {
		activePhoneTabs = new List<PhoneTab> ();
		openPhone ();
		//PhoneAlert.me.setMessageText ("Press the button to open & close the phone");
	}
	
	// Update is called once per frame
	void Update () {
		runTab ();
		tabMoniter ();

		if (Application.isEditor == true) {
			//if (Input.GetKeyDown (KeyCode.T)) {
				foreach (PhoneTab t in allPhoneTabs) {
					if (activePhoneTabs.Contains (t) == false) {
						activePhoneTabs.Add (t);
					}
				}
		//	}
		}
	}

	void tabMoniter()
	{
		foreach (PhoneTab t in activePhoneTabs) {
			if (t.active == true && currentTab != t) {
				t.disablePhoneTab ();
			}
		}
	}

	void runTab()
	{

		if (activePhoneTabs.Count > 0 && currentTab==null) {
			currentTab = activePhoneTabs [phoneTabCounter];
		}


		if (currentTab == null) {

		} else {
			if (currentTab != activePhoneTabs[phoneTabCounter]) {
				currentTab = activePhoneTabs [phoneTabCounter];
			}

			if (currentTab.active == false) {
				currentTab.gameObject.SetActive (true);
				currentTab.enablePhoneTab ();
			}

			currentTab.onUpdate ();
		}
	}

	public void openPhone()
	{
		if (displayPhoneController == false) {
			myTransform.anchoredPosition3D = new Vector3 (myTransform.anchoredPosition3D.x, myTransform.anchoredPosition3D.y + 300, 20);
			displayPhoneController = true;

			if (booted == false) {
				activePhoneTabs.Add (getPhoneTab ("Boot"));
				booted = true;
			}
		} else if(displayPhoneController==true){
			myTransform.anchoredPosition3D = new Vector3 (myTransform.anchoredPosition3D.x, myTransform.anchoredPosition3D.y - 300,20);
			displayPhoneController = false;
		}
	}

	public bool phoneOpen()
	{
		return displayPhoneController;
	}

	public PhoneTab getPhoneTab(string tabName)
	{
		foreach (PhoneTab pt in allPhoneTabs) {
			if (pt.tabName == tabName) {
				return pt;
			}
		}
		return null;
	}

	public void nextTab()
	{
		if (phoneTabCounter < activePhoneTabs.Count-1) {
			phoneTabCounter++;
		}
	}

	public void previousTab()
	{
		if (phoneTabCounter > 0) {
			phoneTabCounter--;
		}
	}
}
