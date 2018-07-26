using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXDatabase : MonoBehaviour {

	/// <summary>
	/// Store for various sound effects that will be used by more than 1 object.
	/// </summary>

	public static SFXDatabase me;

	public AudioClip smashGlass,bluntImpact,bladedImpact,punchImpact,explosion,flickSwitch,doorOpen,swing,windowInteract,explosiveBeep,fire,textAlert;

	void Awake()
	{
		me = this;
	}
}
