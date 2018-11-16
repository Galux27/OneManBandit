using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimation : MonoBehaviour {
	/// <summary>
	/// Old animation system for people, no longer used, remove at some point.
	/// </summary>
	public Sprite[] spritesInAnimation;
	public float timePerFrame=0;
	public string ID,AnimName;//ID=Person the animation is for e.g. player, gangster, AnimName = what the animation contains e.g. 

	public bool animWereLookingFor(string _ID,string _AnimName)
	{
		if (_ID.CompareTo( ID)==0 && _AnimName.CompareTo( AnimName)==0) {
			//////////Debug.Log("Found Animation " + _ID + " || " + _AnimName);
			return true;
		}
		return false;
	}
}
