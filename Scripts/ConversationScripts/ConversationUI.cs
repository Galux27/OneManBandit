using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ConversationUI : MonoBehaviour {

	/// <summary>
	/// UI for interacting with a conversation.
	/// </summary>

	public static ConversationUI me;
	public Scrollbar myScrollbar;
	public Text myText;
	public GameObject convParent;
	public List<string> textToDisplay;
	public ConversationManager currentConvo;
	public Button button1,button2,button3;
	public ConversationChoice currentChoice;
	void Awake()
	{
		me = this;
	}

	// Use this for initialization
	void Start () {
		if (currentConvo == null) {
			disableConvo ();
		}
	}

	public void enableConvo(ConversationManager conv)
	{
		convParent.SetActive (true);
		textToDisplay = new List<string> ();
		myText.text = "";
		i = 0;
		currentConvo = conv;
		setStartingOptions ();
		myText.rectTransform.anchoredPosition = new Vector2 (myText.rectTransform.anchoredPosition.x, Mathf.Lerp (((myText.rectTransform.rect.height / 1.9f) * -1)-50, i*10, 0.1f)-50);
	}

	public void disableConvo()
	{
		convParent.SetActive (false);
	}


	void setStartingOptions()
	{
		AddText (currentConvo.personSpeakingTo + " : " + currentConvo.openingLine);
		if (currentConvo.initialOptions.Count > 0) {
			button1.gameObject.SetActive (true);
			button1.gameObject.GetComponentInChildren<Text> ().text = currentConvo.initialOptions [0].playerText;
		} else {
			button1.gameObject.SetActive (false);
		}
		if (currentConvo.initialOptions.Count > 1) {
			button2.gameObject.SetActive (true);
			button2.gameObject.GetComponentInChildren<Text> ().text = currentConvo.initialOptions [1].playerText;
		} else {
			button2.gameObject.SetActive (false);
		}
		if (currentConvo.initialOptions.Count > 2) {
			button3.gameObject.SetActive (true);
			button3.gameObject.GetComponentInChildren<Text> ().text = currentConvo.initialOptions [2].playerText;
		} else {
			button3.gameObject.SetActive (false);
		}
	}

	void setOptionsFromConvo(ConversationChoice c)
	{
		AddText ("Player : " + c.playerText);
		AddText (currentConvo.personSpeakingTo + " : " + c.NPCResponse);
		//myText.rectTransform.anchoredPosition = new Vector2 (myText.rectTransform.anchoredPosition.x, Mathf.Lerp (((myText.rectTransform.rect.height / 1.9f) * -1)-50, i*10, 0.0f));

		if (c.myTrigger == null) {

		} else {
			c.myTrigger.OnOptionSelect ();
		}

		currentChoice = c;
		if (c.nextChoices.Count > 0) {
			button1.gameObject.SetActive (true);
			button1.gameObject.GetComponentInChildren<Text> ().text = c.nextChoices [0].playerText;
		} else {
			button1.gameObject.SetActive (false);
		}
		if (c.nextChoices.Count > 1) {
			button2.gameObject.SetActive (true);
			button2.gameObject.GetComponentInChildren<Text> ().text = c.nextChoices [1].playerText;
		} else {
			button2.gameObject.SetActive (false);
		}
		if (c.nextChoices.Count > 2) {
			button3.gameObject.SetActive (true);
			button3.gameObject.GetComponentInChildren<Text> ().text = c.nextChoices [2].playerText;
		} else {
			button3.gameObject.SetActive (false);
		}
	}


	int i = 0;
	// Update is called once per frame


	/// <summary>
	/// Text is stored in a scroll view and is replaced every 10 conversation interactions, its abit wonkey so we have to change the position aswell to make sure that it stays in view.
	/// </summary>
	public void onScroll()
	{
		myText.rectTransform.anchoredPosition = new Vector2 (myText.rectTransform.anchoredPosition.x, getYOfScroll()-50);

	}


	float getYOfScroll()
	{
		return Mathf.Lerp (((myText.rectTransform.rect.height / 1.9f) * -1)-50, i*10, myScrollbar.value);
		//return myText.rectTransform.rect.height - ((myText.rectTransform.rect.height) * (1.0f- myScrollbar.value));
	}

	public void AddText(string text)
	{
		//myText.text += text;
		if (textToDisplay.Count > 20) {
			textToDisplay.RemoveAt (0);
		}
		textToDisplay.Add (text);
		setText ();
		i++;
		onScroll ();
	}

	void setText()
	{
		myText.text = "";
		foreach (string st in textToDisplay) {
			myText.text += st;
			myText.text += "\n";
			myText.text += "\n";
		}
	}

	public void option1()
	{
		if (currentChoice == null) {
			setOptionsFromConvo (currentConvo.initialOptions [0]);

		} else {
			setOptionsFromConvo (currentChoice.nextChoices [0]);
		}
	}

	public void option2()
	{
		if (currentChoice == null) {
			setOptionsFromConvo (currentConvo.initialOptions [1]);

		} else {
			setOptionsFromConvo (currentChoice.nextChoices [1]);
		}
	}

	public void option3()
	{
		if (currentChoice == null) {
			setOptionsFromConvo (currentConvo.initialOptions [2]);

		} else {
			setOptionsFromConvo (currentChoice.nextChoices [2]);
		}
	}
}
