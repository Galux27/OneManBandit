using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonClothesController : MonoBehaviour {
	public static PersonClothesController playerClothes;
	public ArtemAestheticController aac;
	public List<ClothingItem> clothesBeingWorn;
	public bool getRandomClothes=false;
	public List<string> clothesToWearAtStart;
	public bool clothesSetFromLoading=false;
	void Awake()
	{
		if (this.gameObject.tag == "Player") {
			playerClothes = this;
		}
		aac = this.GetComponentInChildren<ArtemAestheticController> ();
		clothesBeingWorn = new List<ClothingItem> ();
	}




	// Use this for initialization
	void Start () {
		if (getRandomClothes == true) {
			initialiseClothing ();
		} else {
			if (clothesToWearAtStart == null || clothesToWearAtStart.Count == 0) {
				setNaked ();
			} else {
				initialiseClothing ();
			}
		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void initialiseClothing()
	{
		if (getRandomClothes == true) {
			clothesToWearAtStart = this.GetComponent<RandomClothesGenerator> ().getRandomClothes ();
		}

		foreach (string st in clothesToWearAtStart) {
			ClothingItem ci = ItemDatabase.me.getItem (st).GetComponent<ClothingItem>();
			if (ci == null) {
				continue;
			}
			GameObject g = (GameObject) Instantiate (ci.gameObject, this.transform);
			Inventory i = this.gameObject.GetComponent<Inventory> ();

			i.addItemToInventory (g.GetComponent<ClothingItem>());
			g.GetComponent<ClothingItem>().equipItem ();
			g.transform.parent = this.transform;

		}
	}

	public void equipClothing(ClothingItem ci)
	{

		removeConflictingClothing (ci);

		if (ci.head == null) {

		} else {
			aac.head = ci.head;
		}

		if (ci.torso == null) {

		} else {
			aac.torso = ci.torso;
			aac.torFall = ci.torFall;
		}

		if (ci.lShoulder == null) {

		} else {
			aac.lShoulder = ci.lShoulder;
		}

		if (ci.rShoulder == null) {

		} else {
			aac.rShoulder = ci.rShoulder;
		}

		if (ci.lFore == null) {

		} else {
			aac.lFore = ci.lFore;
		}

		if (ci.rFore == null) {

		} else {
			aac.rFore = ci.rFore;
		}

		if (ci.lHandFist == null) {

		} else {
			aac.lHandFist= ci.lHandFist;
			aac.lHandHold = ci.lHandHold;
			aac.lHandTrigger = ci.lHandTrigger;
		}

		if (ci.rHandFist == null) {

		} else {
			aac.rHandFist= ci.rHandFist;
			aac.rHandHold = ci.rHandHold;
			aac.rHandTrigger = ci.rHandTrigger;
		}

		if (ci.lCalf == null) {

		} else {
			aac.lCalf = ci.lCalf;
			aac.lThigh = ci.lThigh;
		}

		if (ci.rCalf == null) {

		} else {
			aac.rCalf = ci.rCalf;
			aac.rThigh = ci.rThigh;
		}

		if (ci.lFoot == null) {

		} else {
			aac.lFoot = ci.lFoot;
		}

		if (ci.rFoot == null) {

		} else {
			aac.rFoot = ci.rFoot;
		}

		aac.setAesthetics ();
		ci.inUse = true;
		clothesBeingWorn.Add (ci);
	}

	public void removeClothing(ClothingItem ci)
	{
		if (clothesBeingWorn.Contains (ci) == false) {
			return;
		}


		if (ci.head == null) {

		} else {
			aac.head = transform.root.GetComponentInChildren<HeadController>().defaultHead;
		}

		if (ci.torso == null) {

		} else {
			aac.torso = CommonObjectsStore.me.defaultClothes.torso;
			aac.torFall = CommonObjectsStore.me.defaultClothes.torFall;
		}

		if (ci.lShoulder == null) {

		} else {
			aac.lShoulder =CommonObjectsStore.me.defaultClothes.lShoulder;
		}

		if (ci.rShoulder == null) {

		} else {
			aac.rShoulder = CommonObjectsStore.me.defaultClothes.rShoulder;
		}

		if (ci.lFore == null) {

		} else {
			aac.lFore = CommonObjectsStore.me.defaultClothes.lFore;
		}

		if (ci.rFore == null) {

		} else {
			aac.rFore = CommonObjectsStore.me.defaultClothes.rFore;
		}

		if (ci.lHandFist == null) {

		} else {
			aac.lHandFist= CommonObjectsStore.me.defaultClothes.lHandFist;
			aac.lHandHold = CommonObjectsStore.me.defaultClothes.lHandHold;
			aac.lHandTrigger = CommonObjectsStore.me.defaultClothes.lHandTrigger;
		}

		if (ci.rHandFist == null) {

		} else {
			aac.rHandFist= CommonObjectsStore.me.defaultClothes.rHandFist;
			aac.rHandHold = CommonObjectsStore.me.defaultClothes.rHandHold;
			aac.rHandTrigger = CommonObjectsStore.me.defaultClothes.rHandTrigger;
		}

		if (ci.lCalf == null) {

		} else {
			aac.lCalf = CommonObjectsStore.me.defaultClothes.lCalf;
			aac.lThigh = CommonObjectsStore.me.defaultClothes.lThigh;
		}

		if (ci.rCalf == null) {

		} else {
			aac.rCalf = CommonObjectsStore.me.defaultClothes.rCalf;
			aac.rThigh = CommonObjectsStore.me.defaultClothes.rThigh;
		}

		if (ci.lFoot == null) {

		} else {
			aac.lFoot = CommonObjectsStore.me.defaultClothes.lFoot;
		}

		if (ci.rFoot == null) {

		} else {
			aac.rFoot = CommonObjectsStore.me.defaultClothes.rFoot;
		}

		aac.setAesthetics ();
		ci.inUse = false;
		//ci.unequipItem ();
		clothesBeingWorn.Remove (ci);
	}

	void setNaked()
	{
		if (clothesSetFromLoading == true) {
			clothesSetFromLoading = false;
			return;
		}
			aac.head = transform.root.GetComponentInChildren<HeadController>().defaultHead;
	

	
			aac.torso = CommonObjectsStore.me.defaultClothes.torso;
			aac.torFall = CommonObjectsStore.me.defaultClothes.torFall;


			aac.lShoulder =CommonObjectsStore.me.defaultClothes.lShoulder;


			aac.rShoulder = CommonObjectsStore.me.defaultClothes.rShoulder;


			aac.lFore = CommonObjectsStore.me.defaultClothes.lFore;



			aac.rFore = CommonObjectsStore.me.defaultClothes.rFore;

			aac.lHandFist= CommonObjectsStore.me.defaultClothes.lHandFist;
			aac.lHandHold = CommonObjectsStore.me.defaultClothes.lHandHold;
			aac.lHandTrigger = CommonObjectsStore.me.defaultClothes.lHandTrigger;

		
			aac.rHandFist= CommonObjectsStore.me.defaultClothes.rHandFist;
			aac.rHandHold = CommonObjectsStore.me.defaultClothes.rHandHold;
			aac.rHandTrigger = CommonObjectsStore.me.defaultClothes.rHandTrigger;
		
			aac.lCalf = CommonObjectsStore.me.defaultClothes.lCalf;
			aac.lThigh = CommonObjectsStore.me.defaultClothes.lThigh;
		
			aac.rCalf = CommonObjectsStore.me.defaultClothes.rCalf;
			aac.rThigh = CommonObjectsStore.me.defaultClothes.rThigh;
		
			aac.lFoot = CommonObjectsStore.me.defaultClothes.lFoot;
		
			aac.rFoot = CommonObjectsStore.me.defaultClothes.rFoot;
		aac.setAesthetics ();

	}

	void removeConflictingClothing(ClothingItem ci)
	{
		List<ClothingItem> toRemove = new List<ClothingItem> ();
		foreach (ClothingItem cloths in clothesBeingWorn) {
			foreach (ClothingItemSlot cl in ci.slotsITakeUp) {
				if (cloths.slotsITakeUp.Contains (cl)) {
					toRemove.Add (cloths);
				}
			}
		}

		foreach (ClothingItem cl in toRemove) {
			removeClothing (cl);
		}
	}
}
