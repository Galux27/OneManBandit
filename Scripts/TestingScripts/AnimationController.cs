using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour {
	public Animator myAnimator;
	public GameObject torso,leftHand,rightHand,head,leftLeg,rightLeg;
	bool punching = false;
	float punchTimer = 1.0f;

	public PersonMovementController pmc;
	public PersonWeaponController pwc;
	public PersonHealth myHealth;
	public PersonAestheticStore myAesthetic;
	public NPCController myController;

	public SpriteRenderer[] myRenderers;
	public float alphaCountdown = 0.0f;
	public bool attackFlash = false;
	void Awake()
	{
		pmc = this.gameObject.GetComponentInParent<PersonMovementController> ();
		pwc = this.gameObject.GetComponentInParent<PersonWeaponController> ();
		myHealth = this.gameObject.GetComponentInParent<PersonHealth> ();
		myAesthetic = this.GetComponent<PersonAestheticStore> ();
		setBodySprites ();
		myRenderers = this.transform.root.gameObject.GetComponentsInChildren<SpriteRenderer> ();
	}

	public void setBodySprites()
	{
		head.GetComponent<SpriteRenderer> ().sprite = myAesthetic.head;
		leftHand.GetComponent<SpriteRenderer> ().sprite = myAesthetic.leftHand;
		rightHand.GetComponent<SpriteRenderer> ().sprite = myAesthetic.rightHand;
		torso.GetComponent<SpriteRenderer> ().sprite = myAesthetic.torso;

		head.GetComponent<SpriteRenderer> ().sortingOrder = 5;
		leftHand.GetComponent<SpriteRenderer> ().sortingOrder = 2;
		rightHand.GetComponent<SpriteRenderer> ().sortingOrder = 2;
		torso.GetComponent<SpriteRenderer> ().sortingOrder = 4;
	}



	// Use this for initialization
	void Start () {
		myAnimator = this.gameObject.GetComponent<Animator> ();
		myAnimator.SetInteger ("Blood", 5200);
	}

	void setLegAnimation()
	{
		if (myController == null) {
			myAnimator.SetBool ("IsMoving", pmc.movedThisFrame);

		} else {
			if (myController.pf.waitingForPath  == true || myController.npcB.myType == AIType.hostage) {
				myAnimator.SetBool ("IsMoving",false);

			} else {
				myAnimator.SetBool ("IsMoving", pmc.movedThisFrame);

			}
		}

	}

	public void disableLegs()
	{
		leftLeg.SetActive (false);
		rightLeg.SetActive (false);
	}

	public void enableLegs()
	{
		leftLeg.SetActive (true);
		rightLeg.SetActive (true);
	}

	// Update is called once per frame
	void Update () {
		if (myAnimator == null) {
			myAnimator = this.gameObject.GetComponent<Animator> ();
		}

		if (attackFlash == true) {
			redFlash ();
		}


		if (this.gameObject.transform.parent.tag == "NPC") {
			if (myController == null) {
				myController = this.gameObject.transform.parent.gameObject.GetComponent<NPCController> ();
			}

			if(myController.npcB.myType==AIType.hostage)
			{
				setWalking ();
				return;
			}
		}


		myAnimator.SetInteger ("Blood",(int) myHealth.healthValue);
		if (this.gameObject.transform.parent.gameObject.tag == "Dead/Knocked") {
			disableBodyParts ();
		} else {
			enableBodyParts ();
		}

		if (pwc.currentWeapon == null) {
			setWeaponUnequiped ();
			//if (pmc.movedThisFrame == true) {
			//	setWalking ();
			//} else {
			//	setStationary ();
			//}
			setLegAnimation ();

			myAnimator.SetBool ("HasMeleeWeapon", false);

		}else if(pwc.currentWeapon.melee==true){ 
			setLegAnimation ();

			setWeaponUnequiped ();
			//if (pmc.movedThisFrame == true) {
			//	setWalking ();
			//} else {
			//	setStationary ();
			//}


			myAnimator.SetBool ("HasMeleeWeapon", true);




		}else {
			setLegAnimation ();

			setWeaponEquiped ();
			myAnimator.SetBool ("TwoHanded",!pwc.currentWeapon.oneHanded);
			myAnimator.SetBool("AimDownSight", pwc.aimDownSight);


		}

//		//////Debug.Log("Is player running " +  myAnimator.GetBool("IsRunning"));
		////////Debug.Log ("Is player moving " + myAnimator.GetBool ("IsMoving"));
		////////Debug.Log ("Player blood " + myAnimator.GetFloat ("Blood"));
	}

	public void setStandingTied()
	{
		leftHand.SetActive (false);
		rightHand.SetActive (false);
	}

	public void setStandingUntied()
	{
		leftHand.SetActive (true);
		rightHand.SetActive (true);
	}

	public void punch()
	{
		int r = Random.Range (0, 300);
		////////Debug.Log ("Punch value was " + r);
		myAnimator.SetInteger ("UnarmedMeleeDecide", r);
		myAnimator.SetBool ("IsUnarmedAttacking", true);
		punching = true;
		//myAnimator.SetBool ("IsUnarmedAttacking", false);

	}

	public void punchReset()
	{
		punchTimer -= Time.deltaTime;
		if (punchTimer <= 0) {
			myAnimator.SetBool ("IsUnarmedAttacking", false);
			punching = false;
			punchTimer = 1.0f;
		}
	}

	public void setWalking()
	{

		if (myController == null) {
			myAnimator.SetBool ("IsMoving", true);

		} else {
			if (myController.pf.waitingForPath  == true|| myController.npcB.myType == AIType.hostage) {
				myAnimator.SetBool ("IsMoving",false);

			} else {
				myAnimator.SetBool ("IsMoving", true);

			}
		}

		//myAnimator.SetBool ("IsMoving", true);

	}

	public void setRunning()
	{
		myAnimator.SetBool ("IsRunning", true);

	}

	public void setNotRunning()
	{
		myAnimator.SetBool ("IsRunning", false);

	}

	public void setStationary()
	{
		if (myController == null) {
			myAnimator.SetBool ("IsMoving", false);

		} else {
			if (myController.pf.waitingForPath == true|| myController.npcB.myType == AIType.hostage) {
				myAnimator.SetBool ("IsMoving",false);

			} else {
				myAnimator.SetBool ("IsMoving", false);

			}
		}

	}

	public void setWeaponEquiped()
	{
		if (myAnimator.GetBool ("GunEquiped") == false) {
			myAnimator.SetBool ("GunEquiped", true);
		}
	}

	public void setIfOneHanded(bool h)
	{
		myAnimator.SetBool ("TwoHanded",!h);
	}

	public void setWeaponUnequiped()
	{
		myAnimator.SetBool ("GunEquiped", false);

	}

	public void setHipFire()
	{
		myAnimator.SetBool ("AimDownSight", false);

	}

	public void setADS()
	{
		myAnimator.SetBool ("AimDownSight", true);

	}

	public void meleeAttackEnded()
	{
		myAnimator.SetBool ("IsUnarmedAttacking", false);
		this.transform.parent.root.GetComponent<PersonWeaponController> ().setMeleeDone	 ();
	}

	public void setItemPickedUp(){
		myAnimator.SetBool ("PickedUp", false);
	}

	public void setPickUp()
	{
		myAnimator.SetBool ("PickedUp", true);

	}

	public void setReload()
	{
		myAnimator.SetBool ("Reloading", true);

	}

	public void setLeftHandAboveGun()
	{
		leftHand.GetComponent<SpriteRenderer> ().sortingOrder = 5;
	}

	public void setLeftHandBelowGun()
	{
		leftHand.GetComponent<SpriteRenderer> ().sortingOrder = 3;

	}

	public void reloadDone()
	{
		myAnimator.SetBool ("Reloading", false);

	}

	public void disableBodyParts()
	{
		head.SetActive (false);
		leftHand.SetActive (false);
		rightHand.SetActive (false);
		torso.SetActive (false);
	}

	public void enableBodyParts()
	{
		head.SetActive (true);
		leftHand.SetActive (true);
		rightHand.SetActive (true);
		torso.SetActive (true);
	}

	public void setThrow()
	{
		myAnimator.SetBool ("HoldingThrowable", true);
		myAnimator.SetBool ("ExplosivePutAway", false);

	}

	public void setPutAway()
	{
		myAnimator.SetBool ("ExplosivePutAway", true);
		myAnimator.SetBool ("HoldingThrowable", false);

	}

	public void thrown()
	{
		myAnimator.SetBool ("HoldingThrowable", false);
	}

	public void meleeAttackConnect()
	{
		transform.root.GetComponent<PersonWeaponController> ().animationConnected = true;
		//myAnimator.SetFloat ("UnarmedMeleeDecide", -1.0f);
	}

	public void OnAttack()
	{
		attackFlash = true;
		alphaCountdown = 0.0f;
	}

	void redFlash()
	{
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

	public void flipWeapon()
	{
		

		if (transform.root.GetComponent<PersonWeaponController>().currentWeapon == null) {

		} else {
			transform.root.GetComponent<PersonWeaponController>().currentWeapon.GetComponent<SpriteRenderer> ().flipX = true;
		}
	}

	public void unflipWeapon()
	{
		if (transform.root.GetComponent<PersonWeaponController>().currentWeapon == null) {

		} else {
			transform.root.GetComponent<PersonWeaponController>().currentWeapon.GetComponent<SpriteRenderer> ().flipX = false;
		}
	}

	public void startMeleeAnimation()
	{
		transform.root.GetComponent<PersonWeaponController> ().currentWeapon.playMeleeAnim = true;
	}
}
