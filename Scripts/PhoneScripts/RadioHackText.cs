using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RadioHackText : MonoBehaviour {
	public Image background;
	public Text textDisplay;

	public float textAddTimer = 0.1f;
	public int counter = 0;

	public string displayedString="",fullString="";

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (displayedString != fullString) {
			addMoreChars ();
		}
	}


	void addMoreChars()
	{
		if (displayedString != fullString) {
			textAddTimer -= Time.deltaTime;
			if (textAddTimer <= 0) {
				displayedString = displayedString + fullString.ToCharArray () [counter].ToString ();
				if (counter < fullString.Length) {
					counter++;
				}
				textDisplay.text = displayedString;
				textAddTimer = 0.1f;
			}
		}
	}

	public void setText(string toSet,bool seenBefore,Color bgCol)
	{
		textDisplay.enabled = true;
		background.enabled = true;
		if (seenBefore == false) {
			fullString = toSet;
			displayedString = "";
			counter = 0;
			textAddTimer = 0.1f;
		} else {
			fullString = toSet;
			displayedString = toSet;
			counter = fullString.Length;
			textAddTimer = 0.1f;
		}
		textDisplay.text = displayedString;

		background.color = bgCol;
	}

	public void stopDisplaying()
	{
		textDisplay.enabled = false;
		background.enabled = false;
	}

}
