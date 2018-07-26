using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PersonTextDisplay : MonoBehaviour {
	public Text myText;
	public GameObject toFollow;
	public float textAddTimer = 0.1f;
	public int counter = 0;
	public Image myImage;
	public string displayedString="",fullString="";
	public bool fadeOutText=false;
	// Use this for initialization
	void Start () {
		myImage = this.gameObject.GetComponentInParent<Image> ();
		if (displayedString == "") {
			myImage.enabled = false;

		}
	}
	
	// Update is called once per frame
	void Update () {
		setPosition ();
	
		if (displayedString != fullString) {
			addMoreChars ();
		} else {
			fadeOutWait ();
		}
		
		if (fadeOutText == true) {
			fadeOut ();
		}
	}

	void fadeOut()
	{
		if (textAddTimer > 0) {
			textAddTimer -= Time.deltaTime;
		}
		else if (textAddTimer <= 0) {
			Color cl = myText.color;
			cl = new Color (cl.r, cl.g, cl.b, cl.a -= 0.01f);
			myText.color = cl;
			textAddTimer = 0.02f;

			if (cl.a <= 0) {
				myImage.enabled = false;
				//Destroy (this.gameObject);
				//setText("");
				//fadeOutText = false;
			}
		}

	}

	float waitTillFade = 3.0f;

	void fadeOutWait()
	{
		waitTillFade -= Time.deltaTime;
		if (waitTillFade <= 0) {
			fadeOutText = true;
			waitTillFade = 3.0f;
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
				myText.text = displayedString;
			}
		}
	}

	public void setText(string toSet)
	{
		Color cl = myText.color;
		cl = new Color (cl.r, cl.g, cl.b, 1);
		myText.color = cl;
		fullString = toSet;
		displayedString = "";
		counter = 0;
		textAddTimer = 0.1f;
		myImage.enabled = true;
	}

	void setPosition()
	{
		if (toFollow == null) {
			return;
		}
		Vector3 pos = new Vector3 (toFollow.transform.position.x, toFollow.transform.position.y + 0.7f, toFollow.transform.position.z);
		Vector3 toCamPos = CommonObjectsStore.me.mainCam.WorldToScreenPoint (pos);

		this.transform.parent.position = toCamPos;

	}
}
