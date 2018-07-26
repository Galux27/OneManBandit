using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtemAestheticController : MonoBehaviour {

	/// <summary>
	/// Stores the aesthetics of a person and a reference to all body components on them. Used for setting the clothes that an NPC wears and enabling/disabling different components. 
	/// </summary>
	
	public Sprite head,torso,lShoulder,lFore,lHand,rShoulder,rFore,rHand,lThigh,lCalf,lFoot,rThigh,rCalf,rFoot,lbThigh,lbCalf,lbFoot,rbThigh,rbCalf,rbFoot,torFall;
	public Sprite lHandFist,lHandTrigger,lHandHold, rHandFist,rHandTrigger,rHandHold;
	public GameObject headObj, torsoObj, lShoulderObj, lForeObj, lHandObj, rShoulderObj, rForeObj, rHandObj, lThighObj, lCalfObj, lFootObj, rThighObj, rCalfObj, rFootObj;

	public Vector2 handgunOffset;
	public Vector2 twoHandedOffset;
	public Vector2 throwableOffset;
	public Sprite knocked,dead;

	public bool changePallete = false,randomPallete=true;
	public string palleteToGet;
	// Use this for initialization
	void Start () {
		throwableOffset = new Vector2 (-0.104f, 0.114f);
		setAesthetics ();
	//	setPallete ();
	}

	void setPallete()
	{
		if (changePallete == true) {
			if (randomPallete == true) {
				SpriteColorPallete p = PalleteManager.me.getRandomPallete ();
				p.setThisPalletOntoSprite (torsoObj.GetComponent<SpriteRenderer> ());
				p.setThisPalletOntoSprite (lShoulderObj.GetComponent<SpriteRenderer> ());
				p.setThisPalletOntoSprite (rShoulderObj.GetComponent<SpriteRenderer> ());
				p.setThisPalletOntoSprite (rForeObj.GetComponent<SpriteRenderer> ());
				p.setThisPalletOntoSprite (lForeObj.GetComponent<SpriteRenderer> ());
			} 
		}
	}

	public void setAesthetics()
	{
		headObj.GetComponent<SpriteRenderer> ().sprite = head;
		if (transform.root.gameObject.tag == "Dead/Knocked") {
			torsoObj.GetComponent<SpriteRenderer> ().sprite = torFall;

		} else {
			torsoObj.GetComponent<SpriteRenderer> ().sprite = torso;
		}
		lShoulderObj.GetComponent<SpriteRenderer> ().sprite = lShoulder;
		lForeObj.GetComponent<SpriteRenderer> ().sprite = lFore;
		lHandObj.GetComponent<SpriteRenderer> ().sprite = lHandFist;

		rShoulderObj.GetComponent<SpriteRenderer> ().sprite = rShoulder;
		rForeObj.GetComponent<SpriteRenderer> ().sprite = rFore;
		rHandObj.GetComponent<SpriteRenderer> ().sprite = rHandFist;

		lThighObj.GetComponent<SpriteRenderer> ().sprite = lThigh;
		lCalfObj.GetComponent<SpriteRenderer> ().sprite = lCalf;
		lFootObj.GetComponent<SpriteRenderer> ().sprite = lFoot;

		rThighObj.GetComponent<SpriteRenderer> ().sprite = rThigh;
		rCalfObj.GetComponent<SpriteRenderer> ().sprite = rCalf;
		rFootObj.GetComponent<SpriteRenderer> ().sprite = rFoot;


	}
	
	// Update is called once per frame
	//void Update () {
		
	//}

	public void invertLeftLeg()
	{
		return;

		if (lThighObj.GetComponent<SpriteRenderer> ().sprite == lThigh) {
			lThighObj.GetComponent<SpriteRenderer> ().sprite = lbThigh;
			lCalfObj.GetComponent<SpriteRenderer> ().sprite = lbCalf;
			lFootObj.GetComponent<SpriteRenderer> ().sprite = lbFoot;
		} else {
			lThighObj.GetComponent<SpriteRenderer> ().sprite = lThigh;
			lCalfObj.GetComponent<SpriteRenderer> ().sprite = lCalf;
			lFootObj.GetComponent<SpriteRenderer> ().sprite = lFoot;
		}
	}

	public void invertRightLeg()
	{
		return;

		if (rThighObj.GetComponent<SpriteRenderer> ().sprite == rThigh) {
			rThighObj.GetComponent<SpriteRenderer> ().sprite = rbThigh;
			rCalfObj.GetComponent<SpriteRenderer> ().sprite = rbCalf;
			rFootObj.GetComponent<SpriteRenderer> ().sprite = rbFoot;
		} else {
			rThighObj.GetComponent<SpriteRenderer> ().sprite = rThigh;
			rCalfObj.GetComponent<SpriteRenderer> ().sprite = rCalf;
			rFootObj.GetComponent<SpriteRenderer> ().sprite = rFoot;
		}
	}

	public void setHandgunHip()
	{
		rHandObj.GetComponent<SpriteRenderer> ().sprite = rHandTrigger;
		lHandObj.GetComponent<SpriteRenderer> ().sprite = lHandFist;
	}

	public void setHandgunADS()
	{
		rHandObj.GetComponent<SpriteRenderer> ().sprite = rHandTrigger;
		lHandObj.GetComponent<SpriteRenderer> ().sprite = lHandHold;
	}

	public void setTwoHandedHip()
	{
		rHandObj.GetComponent<SpriteRenderer> ().sprite = rHandTrigger;
		lHandObj.GetComponent<SpriteRenderer> ().sprite = lHandHold;
	}

	public void setTwoHandedADS()
	{
		rHandObj.GetComponent<SpriteRenderer> ().sprite = rHandTrigger;
		lHandObj.GetComponent<SpriteRenderer> ().sprite = lHandHold;
	}

	public void setUnarmed()
	{
		rHandObj.GetComponent<SpriteRenderer> ().sprite = rHandFist;
		lHandObj.GetComponent<SpriteRenderer> ().sprite = lHandFist;
	}

	public void setLeftFist()
	{
		lHandObj.GetComponent<SpriteRenderer> ().sprite = lHandFist;

	}

	public void setLeftHold()
	{		
		lHandObj.GetComponent<SpriteRenderer> ().sprite = lHandHold;
	}

	public void setLeftTrigger()
	{
		lHandObj.GetComponent<SpriteRenderer> ().sprite = lHandTrigger;

	}

	public void setLeftSortingOrder(int val)
	{
		lHandObj.GetComponent<SpriteRenderer> ().sortingOrder = val;

	}

	public void setFall()
	{
		torsoObj.GetComponent<SpriteRenderer> ().sprite = torFall;

	}

	public void setUp()
	{
		torsoObj.GetComponent<SpriteRenderer> ().sprite = torso;

	}

	public void disableBody()
	{
		headObj.SetActive (false);
		torsoObj.SetActive (false);

		lShoulderObj.SetActive (false);
		lForeObj.SetActive (false);
		lHandObj.SetActive (false);

		rShoulderObj.SetActive (false);
		rForeObj.SetActive (false);
		rHandObj.SetActive (false);

		lThighObj.SetActive (false);
		lCalfObj.SetActive (false);
		lFootObj.SetActive (false);

		rThighObj.SetActive (false);
		rCalfObj.SetActive (false);
		rFootObj.SetActive (false);
	}

	public void enableBody()
	{
		headObj.SetActive (true);
		torsoObj.SetActive (true);

		lShoulderObj.SetActive (true);
		lForeObj.SetActive (true);
		lHandObj.SetActive (true);

		rShoulderObj.SetActive (true);
		rForeObj.SetActive (true);
		rHandObj.SetActive (true);

		lThighObj.SetActive (true);
		lCalfObj.SetActive (true);
		lFootObj.SetActive (true);

		rThighObj.SetActive (true);
		rCalfObj.SetActive (true);
		rFootObj.SetActive (true);
	}
}
