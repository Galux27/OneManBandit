using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Base class for various phone tabs that are available
/// </summary>
public class PhoneTab : MonoBehaviour {

	public bool active=false;
	public string tabName;
	public virtual void disablePhoneTab()
	{
		//disable all the UI elements needed
	}

	public virtual void enablePhoneTab()
	{
		//enable all ui elements
	}

	public virtual void onUpdate()
	{
		//do anything that needs to be done every frame
	}

}
