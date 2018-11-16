using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapIcon : MonoBehaviour {
	public Texture2D icon;
	public int x=0,y=0;
	public Color myColor;
	public Color getNextPixel()
	{
		return myColor;
		Color retVal = icon.GetPixel (x, y);
		if (x < icon.width) {
			x++;
		} else {
			x = 0;
			if (y < icon.height) {
				y++;
			} else {
				y = 0;
			}
		}
		return retVal;
	}

}
