using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that draws a progress bar based on values passed in 
/// </summary>
public class ProgressBar : MonoBehaviour {
	public RectTransform progressBar;
	public float maxWidth,maxValue,currentValue;//max value is the value we are working towards,current is what we are working to
	public GameObject myObjectToFollow;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		showProgress ();
		setPosition ();
		if (myObjectToFollow == null) {

		} else {
			destroyedObjectCheck ();
		}

	//	if (Application.isEditor == true) {
		//	debugProgress ();
	//	}
	}

	void destroyedObjectCheck()
	{
		if (myObjectToFollow.tag == "Dead/Knocked") {
			Destroy (this.gameObject);
		}
	}

	void debugProgress()
	{
		maxValue = 100;
		if (currentValue < maxValue) {
			currentValue += 15 * Time.deltaTime;
		} else {
			currentValue = 0;
		}
	}

	public void showProgress()
	{
		if (currentValue < maxValue) {
			progressBar.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, currentValue * ( maxWidth/maxValue));
		} else {
			progressBar.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, maxWidth);
		}
	}

	void setPosition()
	{
		this.transform.position = CommonObjectsStore.me.mainCam.WorldToScreenPoint( myObjectToFollow.transform.position);
	}
}
