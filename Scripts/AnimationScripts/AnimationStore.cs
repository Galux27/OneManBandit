using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStore : MonoBehaviour {
	/// <summary>
	/// Old animation store, no longer used, remove at some point.
	/// </summary>


	public static AnimationStore me;

	void Awake()
	{
		me = this;
	}

	public List<SpriteAnimation> allAnimations;


	public SpriteAnimation getAnimation(string _ID,string _AnimName)
	{
		foreach (SpriteAnimation a in allAnimations) {
			if (a.animWereLookingFor (_ID, _AnimName)==true) {
				//////////Debug.Log (a.ID + "||" + a.AnimName); 
				return a;
			}
		}
		return null;
	}
}
