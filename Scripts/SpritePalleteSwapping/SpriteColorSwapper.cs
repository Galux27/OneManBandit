using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteColorSwapper : MonoBehaviour {
	public Texture2D mColorSwapTex,newPallete;
	public Color[] mSpriteColors;
	public SpriteRenderer mSpriteRenderer;

	void Start()
	{
		mSpriteRenderer = this.GetComponent<SpriteRenderer> ();
		InitColorSwapTex ();
		setNewPallete ();
		PerformSwap ();
	}



	public void InitColorSwapTex()
	{
		Texture2D colorSwapTex = new Texture2D (256, 1, TextureFormat.RGBA32, false, false);
		colorSwapTex.filterMode = FilterMode.Point;

		for (int i = 0; i < colorSwapTex.width; ++i) {
			colorSwapTex.SetPixel (i, 0, new Color (0, 0, 0, 0));
		}
		colorSwapTex.Apply();
		mSpriteRenderer.material.SetTexture("_SwapTex",colorSwapTex);
		mSpriteColors = new Color[colorSwapTex.width];
		mColorSwapTex = colorSwapTex;
	}

	public void setNewPallete()
	{
		for (int i = 0; i < newPallete.width; ++i) {
			SwapColor ((SwapIndex)i, newPallete.GetPixel (i, 0));
		}
	}

	public void SwapColor(SwapIndex index, Color color)
	{
		mSpriteColors [(int)index] = color;
		mColorSwapTex.SetPixel ((int)index, 0, color);
		mColorSwapTex.Apply ();
	}

	public void PerformSwap()
	{
		
		mSpriteRenderer.material.SetTexture("_SwapTex",mColorSwapTex);

	}

}
public enum SwapIndex
{
	head1 = 0,
	head2 = 1,
	head3 = 2,
	head4 = 3,
	head5 = 4,
	head6 = 5,
	head7 = 6,
	head8 = 7,
	torso1 = 8,
	torso2 = 9,
	torso3 = 10,
	torso4 = 11,
	torso5 = 12,
	torso6 = 13,
	torso7 = 14,
	torso8 = 15,
	torso9 = 16,
	leg1=17,
	leg2=18,
	leg3=19,
	leg4=20,
	leg5=21,
	leg6=22,
	leg7=23,
	foot1=24,
	foot2=25,
	foot3=26,
	foot4=27,
	foot5=28,
	foot6=29,
	foot7=30,
	skin1=31,
	skin2=32,
	skin3=33,
	skin4=34,
	skin5=35,
	skin6=36,
	skin7=37,
	skin8=38,
	skin9=39,
	skin10=40,
	skin11=41,
}



