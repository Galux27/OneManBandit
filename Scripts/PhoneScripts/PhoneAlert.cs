using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI for phone alert. 
/// </summary>
public class PhoneAlert : MonoBehaviour {
	public static PhoneAlert me;
	public bool alertOpen=false;
	public Text dispText;
	public RectTransform phoneBg,myRect;
	public Vector3 closedPos = Vector3.zero,openPos = Vector3.zero;
	AudioController au;
	float closeTimer = 5.0f;
	// Use this for initialization

	void Awake(){
		me = this;

	}

	void Start () {
		myRect = this.GetComponent<RectTransform> ();
		au = CommonObjectsStore.player.GetComponent<AudioController> ();
	}
	
	// Update is called once per frame
	void Update () {
		setPositions ();
		closeMessage ();
		//if (Input.GetKeyDown (KeyCode.H)) {
		//	alertOpen = !alertOpen;
		//}


		if (alertOpen == false && myRect.position != closedPos) {
			this.transform.Translate ((closedPos - myRect.position) * Time.deltaTime * 10);
		} else if (alertOpen == true && myRect.position != openPos) {
			this.transform.Translate ((openPos - myRect.position) * Time.deltaTime * 10);

		}
	}

	void setPositions()
	{
		closedPos = phoneBg.position;

		openPos = phoneBg.position;
		openPos = new Vector3 (openPos.x, openPos.y + (Screen.height * 0.19f), openPos.z);
	}

	public void setMessageText(string toDisp)
	{
		if (au == null) {
			au = CommonObjectsStore.player.GetComponent<AudioController> ();
		}

		dispText.text = toDisp;
		closeTimer = 5.0f;
		alertOpen = true;
		au.playSound (SFXDatabase.me.textAlert);
	}

	public void setMessageTextWithoutBeep(string toDisp)
	{
		//if (au == null) {
		//	au = CommonObjectsStore.player.GetComponent<AudioController> ();
		//}

		dispText.text = toDisp;
		closeTimer = 5.0f;
		alertOpen = true;
		//au.playSound (SFXDatabase.me.textAlert);
	}

	void closeMessage()
	{
		if (alertOpen == true) {
			closeTimer -= Time.deltaTime;
			if (closeTimer <= 0) {
				alertOpen = false;
				closeTimer = 5.0f;
			}
		}
	}
}
