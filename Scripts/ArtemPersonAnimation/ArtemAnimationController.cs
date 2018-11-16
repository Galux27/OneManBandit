using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtemAnimationController : MonoBehaviour {

	/// <summary>
	/// Controls animation state of NPCs and contains methods that they use e.g. setting sorting order of hands when holding items.
	/// </summary>

	public Animator myAnimator;
	public ArtemAestheticController myAesthetic;
	public PersonMovementController pmc;
	public PersonWeaponController pwc;
	public GameObject rightHand,leftHand;

	public bool thrownAnimDone=false;
	public bool shortThrown=false;
	void Awake()
	{
		myAnimator = this.GetComponent<Animator> ();
		myAesthetic = this.GetComponent<ArtemAestheticController> ();
		pmc = transform.root.gameObject.GetComponent<PersonMovementController> ();
		pwc = transform.root.gameObject.GetComponent<PersonWeaponController> ();

	}

	// Use this for initialization
	void Start () {

		myAnimator = this.GetComponent<Animator> ();
		myAesthetic = this.GetComponent<ArtemAestheticController> ();
		pmc = transform.root.gameObject.GetComponent<PersonMovementController> ();
		pwc = transform.root.gameObject.GetComponent<PersonWeaponController> ();

		rightHand = myAesthetic.rHandObj;
		leftHand = myAesthetic.lHandObj;
		setMovingForward ();
	}


	public void forceInitialise()
	{
		Awake ();
		Start ();
	}

	// Update is called once per frame
	void Update () {
		areWeMoving ();
		weaponSet ();
		shouldWeChangeGunState ();

		if (Application.isEditor == true) {
			if (Input.GetKeyDown (KeyCode.L)) {
				doRedFlash ();
			}
		}


		if (attackFlash == true) {
			redFlash ();
		}
	}

	void areWeMoving()
	{
		if (pmc.killVel()==true || pmc.movedThisFrame==false) {
			myAnimator.SetBool ("Moving",false);
		} else {
			myAnimator.SetBool ("Moving",true);
			setMovingForward ();

			//if (pmc.xMove.x > 0) {
			//	setMovingLeft ();
			//} else if (pmc.xMove.x < 0) {
			//	setMovingRight ();
			//} else {
			//	setMovingForward ();
			//}

		}
	}

	public void setMovingLeft()
	{
		myAnimator.SetInteger ("MovementDir", 2);
	}

	public void setMovingRight()
	{
		myAnimator.SetInteger ("MovementDir", 3);

	}

	public void setMovingForward()
	{
		myAnimator.SetInteger ("MovementDir", 1);
	}

	void weaponSet()
	{

		if (pwc.currentWeapon == null) {
			myAnimator.SetBool ("Armed", false);
			myAesthetic.setUnarmed ();
		} else {
			myAnimator.SetBool ("Armed", true);

			if (pwc.currentWeapon.melee == true) {
				myAnimator.SetBool ("Melee", true);
			} else {
				myAnimator.SetBool ("Melee", false);

				if (pwc.currentWeapon.oneHanded == false) {
					myAnimator.SetBool ("TwoHanded", true);
				} else {
					myAnimator.SetBool ("TwoHanded", false);
				}
				myAnimator.SetBool ("Reload", pwc.reloading);

				myAnimator.SetBool ("ADS", pwc.aimDownSight);
				myAnimator.SetInteger ("Weapon", pwc.currentWeapon.gunAnimValue);
				changeGunState ();
			}
		}
	}
	string lastWeaponName = "";
	bool lastFrameADS = false, lastFrameReload = false;
	void shouldWeChangeGunState()
	{
		bool hasSomethingChanged = false;

		//if (pwc.aimDownSight != lastFrameADS) {
		//	//Debug.Log ("Changing ADS State");
		//	hasSomethingChanged = true;
		//	changeGunState ();
		//	lastFrameADS = pwc.aimDownSight;
		//} 


		if (pwc.currentWeapon == null) {
			if (lastWeaponName != "") {
				lastWeaponName = "";

				changeGunState ();
				hasSomethingChanged = true;

			} 
		} else {
			if (lastWeaponName != pwc.currentWeapon.WeaponName) {
				changeGunState ();
				lastWeaponName = pwc.currentWeapon.WeaponName;
				hasSomethingChanged = true;

			}
		}

		//if (lastFrameReload != pwc.reloading) {

			//if (lastFrameReload == false) {
			//changeGunState ();
			//}
		//	lastFrameReload = pwc.reloading;
		//	hasSomethingChanged = true;
		//} 

		if (hasSomethingChanged == false) {
			gunReset ();
		}
	}

	public void changeGunState()
	{
		myAnimator.SetBool ("CheckWeaponAnimation",true);

	}

	public void gunReset()
	{
		myAnimator.SetBool ("CheckWeaponAnimation",false);
	}

	public void setMeleeAttack()
	{
		myAnimator.SetBool ("MeleeAttacking", true);
	}

	public void stopMeleeAttack()
	{
		myAnimator.SetBool ("MeleeAttacking", false);
		meleeDone ();
	}

	public void setHoldingThrowable()
	{
		thrownAnimDone = false;
		myAnimator.SetBool ("HoldingGrenade", true);
	}

	public void setThrowableThrown()
	{
		myAnimator.SetBool ("GrenadeThrown", true);
	}

	public void setThrowablePutAway()
	{
		myAnimator.SetBool ("HoldingGrenade", false);

	}

	public void switchThrow()
	{
		shortThrown = !shortThrown;
		myAnimator.SetBool ("ShortThrow", shortThrown);
	}

	public void throwableReset()
	{
		thrownAnimDone = true;
		myAnimator.SetBool ("GrenadeThrown", false);

		myAnimator.SetBool ("HoldingGrenade", false);
	}

	public void setTied()
	{
		myAnimator.SetBool ("AreWeTied", true);
		changeGunState ();
	}

	public void setUntied()
	{
		myAnimator.SetBool ("AreWeTied", false);
	}

	public void setHumanShield()
	{
		myAnimator.SetBool ("AreWeHostage", true);
		changeGunState ();

	}

	public void releaseHumanShield()
	{
		myAnimator.SetBool ("AreWeHostage", false);

	}

	public void block()
	{
		myAnimator.SetBool ("Blocking", true);
	}

	public void stopBlock()
	{
		myAnimator.SetBool ("Blocking", false);

	}

	public void disableLegs()
	{
		myAesthetic.lThighObj.SetActive (false);
		myAesthetic.lCalfObj.SetActive (false);
		myAesthetic.lFootObj.SetActive (false);

		myAesthetic.rThighObj.SetActive (false);
		myAesthetic.rCalfObj.SetActive (false);
		myAesthetic.rFootObj.SetActive (false);
	}

	public void enableLegs()
	{
		myAesthetic.lThighObj.SetActive (true);
		myAesthetic.lCalfObj.SetActive (true);
		myAesthetic.lFootObj.SetActive (true);

		myAesthetic.rThighObj.SetActive (true);
		myAesthetic.rCalfObj.SetActive (true);
		myAesthetic.rFootObj.SetActive (true);
	}

	public void disableBodyParts()
	{
		myAesthetic.disableBody ();
	}

	public void enableBodyParts()
	{
		myAesthetic.enableBody ();
	}

	public GameObject getHead()
	{
		return myAesthetic.headObj;
	}

	public void meleeAttackConnect()
	{
		pwc.animationConnected = true;
	}

	public void meleeDone()
	{
		pwc.setMeleeDone ();
	}

	public void setLeftHold()
	{
		myAesthetic.setLeftHold ();
	}

	public void setLeftTrigger()
	{
		myAesthetic.setLeftTrigger ();
	}

	public void setLeftFist()
	{
		myAesthetic.setLeftFist ();
	}

	public void setFallen()
	{
		myAnimator.SetBool ("Knocked", true);

		myAesthetic.setFall ();
	}

	public void setUp()
	{
		myAnimator.SetBool ("Knocked", false);
		myAesthetic.setUp ();
	}

	int leftSortingOrder = 0;

	public void setLeftAboveGun()
	{
		leftSortingOrder = myAesthetic.lHandObj.GetComponent<SpriteRenderer> ().sortingOrder;
		if (pwc.currentWeapon == null) {

		} else {
			myAesthetic.setLeftSortingOrder (pwc.currentWeapon.gameObject.GetComponent<SpriteRenderer> ().sortingOrder + 1);	
		}
	}

	public void setLeftBelowGun()
	{
		myAesthetic.setLeftSortingOrder (leftSortingOrder);
	}
	float alphaCountdown = 0.0f;
	bool attackFlash=false;
	public List<SpriteRenderer> myRenderers;

	public void doRedFlash()
	{
		if (myRenderers == null || myRenderers.Count==0) {
			Shadow shadow = this.transform.root.GetComponent<Shadow> ();
			myRenderers = new List<SpriteRenderer> ();

			SpriteRenderer[] srs = this.gameObject.GetComponentsInChildren<SpriteRenderer> ();

			foreach (SpriteRenderer sr in srs) {
				if (shadow.toCreateShadowsFor.Contains (sr.gameObject)) {
					myRenderers.Add (sr);
				}
			}
		}
		alphaCountdown = 0;
		attackFlash = true;
	}

	void redFlash()
	{
		//Debug.Log ("Doing red flash for " + this.gameObject.transform.root.name);
		alphaCountdown += Time.deltaTime;

		Color c = new Color (1, alphaCountdown, alphaCountdown, 1);
		foreach (SpriteRenderer sr in myRenderers) {
			sr.color = c;
		}

		if (alphaCountdown >= 1) {
			alphaCountdown = 0;
			attackFlash = false;
		}
	}
}
