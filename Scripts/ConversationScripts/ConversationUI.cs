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
	public Button button1,button2,button3,upButton,downButton;
	public ConversationChoice currentChoice;
	int index=0,lengthOfCurrentChoices=0;
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
		currentConvo = null;
		i = 0;
		currentChoice = null;
		textToDisplay.Clear ();
	}


	void setStartingOptions()
	{
		AddText (currentConvo.personSpeakingTo + " : " + currentConvo.openingLine);
		//setButtonOptions (currentConvo.initialOptions);

		index = 0;

		filteredChoices = new List<ConversationChoice> ();
		foreach(ConversationChoice c2 in currentConvo.initialOptions)
		{
			if (c2.done == false || c2.done == true && c2.repeatable == true) {
				filteredChoices.Add (c2);
			}
		}

		lengthOfCurrentChoices = filteredChoices.Count;
		setOptions ();
		upButton.gameObject.SetActive (shouldWeDisplayUpArrow ());
		downButton.gameObject.SetActive (shouldWeDisplayDownArrow ());
		/*if (currentConvo.initialOptions.Count > 0) {
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
		}*/
	}

	void setButtonOptions(List<ConversationChoice> choices)
	{
	/*	List<ConversationChoice> filteredChoices = new List<ConversationChoice> ();
		foreach (ConversationChoice c in choices) {
			if (c.done == false || c.done == true && c.repeatable == true) {
				filteredChoices.Add (c);
			}
		}

		if (filteredChoices.Count > 0) {
			button1.gameObject.SetActive (true);
			button1.gameObject.GetComponentInChildren<Text> ().text = currentConvo.initialOptions [0].playerText;
		} else {
			button1.gameObject.SetActive (false);
		}
		if (filteredChoices.Count > 1) {
			button2.gameObject.SetActive (true);
			button2.gameObject.GetComponentInChildren<Text> ().text = filteredChoices [1].playerText;
		} else {
			button2.gameObject.SetActive (false);
		}
		if (filteredChoices.Count > 2) {
			button3.gameObject.SetActive (true);
			button3.gameObject.GetComponentInChildren<Text> ().text = filteredChoices [2].playerText;
		} else {
			button3.gameObject.SetActive (false);
		}*/
	}



	void setOptionsFromConvo(ConversationChoice c)
	{
		AddText ("Player : " + c.playerText);
		AddText (currentConvo.personSpeakingTo + " : " + c.NPCResponse);
		c.done = true;
		//myText.rectTransform.anchoredPosition = new Vector2 (myText.rectTransform.anchoredPosition.x, Mathf.Lerp (((myText.rectTransform.rect.height / 1.9f) * -1)-50, i*10, 0.0f));

		if (c.myTrigger == null) {

		} else {
			c.myTrigger.OnOptionSelect ();
		}

		currentChoice = c;
		index = 0;

		filteredChoices = new List<ConversationChoice> ();
		foreach(ConversationChoice c2 in c.getChoices())
		{
			if (c2.done == false || c2.done == true && c2.repeatable == true) {
				filteredChoices.Add (c);
			}
		}

		lengthOfCurrentChoices = filteredChoices.Count;
		setOptions ();
		upButton.gameObject.SetActive (shouldWeDisplayUpArrow ());
		downButton.gameObject.SetActive (shouldWeDisplayDownArrow ());
		//setButtonOptions (c.getChoices ());
		/*if (c.nextChoices.Count > 0) {
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
		}*/
	}

	List<ConversationChoice> filteredChoices;

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
			setOptionsFromConvo (filteredChoices [index+0]);

		} else {
			setOptionsFromConvo (filteredChoices [index+0]);
			filteredChoices [index+0].setTrigger ();

		}
	}

	public void option2()
	{
		if (currentChoice == null) {
			setOptionsFromConvo (filteredChoices [index+1]);

		} else {
			setOptionsFromConvo (filteredChoices [index+1]);
			filteredChoices [index+1].setTrigger ();

		}
	}

	public void option3()
	{
		if (currentChoice == null) {
			setOptionsFromConvo (filteredChoices [index+2]);

		} else {
			setOptionsFromConvo (filteredChoices [index+2]);
			filteredChoices [index+2].setTrigger ();
		}
	}

	public void upArrow()
	{
		if (index > 0) {
			index--;
			setOptions ();
			upButton.gameObject.SetActive (shouldWeDisplayUpArrow ());
			downButton.gameObject.SetActive (shouldWeDisplayDownArrow ());
		}
	}

	public void downArrow()
	{
		if (index < lengthOfCurrentChoices - 3) {
			index++;
			setOptions ();
			upButton.gameObject.SetActive (shouldWeDisplayUpArrow ());
			downButton.gameObject.SetActive (shouldWeDisplayDownArrow ());
		}
	}

	void setOptions()
	{
		for (int x = 0; x < 3; x++) {
			if (x == 0) {
				if (index + x >= lengthOfCurrentChoices) {
					button1.gameObject.SetActive (false);
				} else {
					button1.gameObject.SetActive (true);
					button1.gameObject.GetComponentInChildren<Text> ().text =  filteredChoices [index + x].playerText;
				}
			} else if (x == 1) {

				if (index + x >= lengthOfCurrentChoices) {
					button2.gameObject.SetActive (false);
				} else {
					button2.gameObject.SetActive (true);
					button2.gameObject.GetComponentInChildren<Text> ().text =  filteredChoices [index + x].playerText;
				}
			} else {
				if (index + x >= lengthOfCurrentChoices) {
					button3.gameObject.SetActive (false);
				} else {
					button3.gameObject.SetActive (true);
					button3.gameObject.GetComponentInChildren<Text> ().text =  filteredChoices [index + x].playerText;
				}
			}
		}
	}

	bool shouldWeDisplayUpArrow()
	{

		if (index > 0) {
			return true;
		}
		return false;
	}

	bool shouldWeDisplayDownArrow()
	{

		if (index < lengthOfCurrentChoices - 3) {
			return true;
		}
		return false;
	}
}
