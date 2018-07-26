using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerActionArrow : MonoBehaviour {
	public Image arrow;
	// Use this for initialization
	void Start () {
		arrow = this.GetComponent<Image> ();
	}
	
	// Update is called once per frame
	void Update () {
		setPosition ();
	}

	void setPosition()
	{
		if (NewPlayerActionUI.me.myActions.Count>0 && NewPlayerActionUI.me.hidden==false) {
			arrow.enabled = true;
			this.transform.position = CommonObjectsStore.me.mainCam.WorldToScreenPoint( NewPlayerActionUI.me.myActions [NewPlayerActionUI.me.index].gameObject.transform.position ) + new Vector3(0,60,0);
		} else {
			arrow.enabled = false;
		}

	}
}
