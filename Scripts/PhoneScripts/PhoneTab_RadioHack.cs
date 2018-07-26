using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhoneTab_RadioHack : PhoneTab {
	public static PhoneTab_RadioHack me;
	public List<RadioHackText> myDisplays;

	public List<string> messagesToDisplaySWAT,messagesToDisplayPrivate,messagesToDisplayCop;

	public Button b1, b2;
	public Image bg;
	public Text channelDisplay;

	public radioHackBand myBand;

	void Awake()
	{
		me = this;

	}

	// Use this for initialization
	void Start () {
		nextBand ();
		foreach (RadioHackText r in myDisplays) {
			r.stopDisplaying ();
		}
	}
	
	// Update is called once per frame
	public override void onUpdate(){
		
	}

	public void setNewText(string newText,radioHackBand type)
	{
		if (messagesToDisplaySWAT == null) {
			messagesToDisplaySWAT = new List<string> ();
		}

		if (messagesToDisplayPrivate == null) {
			messagesToDisplayPrivate = new List<string> ();
		}

		if (messagesToDisplayCop == null) {
			messagesToDisplayCop = new List<string> ();
		}

		if (type == radioHackBand.swat) {
			if (messagesToDisplaySWAT.Count >= 5) {
				messagesToDisplaySWAT.Remove (messagesToDisplaySWAT [0]);
			}

			messagesToDisplaySWAT.Add (newText);
		} else if (type == radioHackBand.buisness) {
			if (messagesToDisplayPrivate.Count >= 5) {
				messagesToDisplayPrivate.Remove (messagesToDisplayPrivate [0]);
			}
			messagesToDisplayPrivate.Add (newText);
		} else {
			if (messagesToDisplayCop.Count >= 5) {
				messagesToDisplayCop.Remove (messagesToDisplayCop [0]);
			}
			messagesToDisplayCop.Add (newText);
		}

		setText ();
	}

	public void setText()
	{
		if (PhoneController.me.currentTab == this) {
			if (myBand == radioHackBand.swat) {
				int messCount = messagesToDisplaySWAT.Count - 1;


				for (int x = 0; x < myDisplays.Count; x++) {
					if (x <= messCount) {
					myDisplays [x].gameObject.SetActive (true);

						myDisplays [x].setText (messagesToDisplaySWAT [x], true, Color.blue);
					} else {
						myDisplays [x].stopDisplaying ();
					}
				}


				/*for (int x = myDisplays.Count - 1; x >= 0; x--) {
					if (messCount >= 0) {
						if (x == myDisplays.Count - 1) {
							myDisplays [x].gameObject.SetActive (true);
							myDisplays [x].setText (messagesToDisplaySWAT [messCount], false, Color.red);
							//messCount--;
						} else {
							myDisplays [x].gameObject.SetActive (true);

							myDisplays [x].setText (messagesToDisplaySWAT [messCount], true, Color.blue);
							//messCount--;

						}
						messCount--;
					} else {
						myDisplays [x].stopDisplaying ();
					//}*/
				//}
			} else if (myBand == radioHackBand.buisness) {
				int messCount = messagesToDisplayPrivate.Count - 1;

				for (int x = 0; x < myDisplays.Count; x++) {
					if (x <= messCount) {
					myDisplays [x].gameObject.SetActive (true);

						myDisplays [x].setText (messagesToDisplayPrivate [x], true, Color.blue);
					} else {
						myDisplays [x].stopDisplaying ();
					}
				}
				/*for (int x = myDisplays.Count - 1; x >= 0; x--) {
					if (messCount >= 0) {
						if (x == myDisplays.Count - 1) {
							myDisplays [x].gameObject.SetActive (true);
							myDisplays [x].setText (messagesToDisplayPrivate [messCount], false, Color.red);
							//messCount--;
						} else {
							myDisplays [x].gameObject.SetActive (true);

							myDisplays [x].setText (messagesToDisplayPrivate [messCount], true, Color.blue);
							//messCount--;

						}
						messCount--;
					} else {
						myDisplays [x].stopDisplaying ();
					}
				}*/
			} else {
				int messCount = messagesToDisplayCop.Count - 1;

				for (int x = 0; x < myDisplays.Count; x++) {
					if (x <= messCount) {
					myDisplays [x].gameObject.SetActive (true);

						myDisplays [x].setText (messagesToDisplayCop [x], true, Color.blue);
					} else {
						myDisplays [x].stopDisplaying ();
					}
				}
				/*for (int x = myDisplays.Count - 1; x >= 0; x--) {
					if (messCount >= 0) {
						if (x == myDisplays.Count - 1) {
							myDisplays [x].gameObject.SetActive (true);
							myDisplays [x].setText (messagesToDisplayCop [messCount], false, Color.red);
							//messCount--;
						} else {
							myDisplays [x].gameObject.SetActive (true);

							myDisplays [x].setText (messagesToDisplayCop [messCount], true, Color.blue);
							//messCount--;

						}
						messCount--;
					} else {
						myDisplays [x].stopDisplaying ();
					}
				}*/
			}
		}
	}


	public void nextBand()
	{
		if (myBand == radioHackBand.buisness) {
			myBand = radioHackBand.cop;
			channelDisplay.text = "Police";

		} else if (myBand == radioHackBand.cop) {
			myBand = radioHackBand.swat;
			channelDisplay.text = "SWAT";

		} else {
			myBand = radioHackBand.buisness;
			channelDisplay.text = "Security";

		}
		setText ();
	}

	public void previousBand()
	{
		if (myBand == radioHackBand.buisness) {
			myBand = radioHackBand.swat;
			channelDisplay.text = "SWAT";
		} else if (myBand == radioHackBand.cop) {
			myBand = radioHackBand.buisness;
			channelDisplay.text = "Security";

		} else {
			myBand = radioHackBand.cop;
			channelDisplay.text = "Police";

		}
		setText ();
	}

	public override void disablePhoneTab()
	{
		//disable all the UI elements needed
		b1.gameObject.SetActive(false);
		b2.gameObject.SetActive (false);
		channelDisplay.gameObject.SetActive (false);
		bg.gameObject.SetActive (false);

		foreach (RadioHackText r in myDisplays) {
			r.stopDisplaying ();
		}
		active = false;
	}

	public override void enablePhoneTab()
	{
		//enable all ui elements
		b1.gameObject.SetActive(true);
		b2.gameObject.SetActive (true);
		channelDisplay.gameObject.SetActive (true);
		bg.gameObject.SetActive (true);


		setText();
		active = true;
	}


	void OnGUI()
	{
		if (active == true && PhoneController.me.displayPhoneController == true) {
			

			foreach (GameObject g in NPCManager.me.npcsInWorld) {
				if (g == null) {
					continue;
				}

				NPCController npc = g.GetComponent<NPCController> ();

				if (npc.myHealth.healthValue <= 0) {
					continue;
				}

				if (npc.npcB.myType == AIType.cop && myBand == radioHackBand.cop || npc.npcB.myType == AIType.guard && myBand == radioHackBand.buisness || npc.npcB.myType == AIType.swat && myBand == radioHackBand.swat) {
					Vector3 posInScreen = CommonObjectsStore.me.mainCam.WorldToScreenPoint (g.transform.position);
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
					GUI.DrawTexture (new Rect (posInScreen.x, posInScreen.y, 10, 10), CommonObjectsStore.me.enemy);
				}
			
			
			}
		}
	}

}



public enum radioHackBand{
	swat,
	cop,
	buisness
}
