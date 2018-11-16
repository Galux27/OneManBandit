using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBase : MonoBehaviour {
	public bool instance = false,initialised=false;
	public string effectName = "";
	public int day, month, year;
	public int length;

	public string effectToAddOnRemoval;
	public bool randomChanceForEffect = false;
	[Range(0,99)]
	public int effectChange;

	public string effectToRemoveOnAdd;


	public float moveMod,damageMod;


	public void initialise()
	{
		day = TimeScript.me.day;
		month = TimeScript.me.month;
		year = TimeScript.me.year;
		initialised = true;
	}

	public virtual float getMoveMod()
	{
		return moveMod*2;
	}

	public virtual float getDamageMod()
	{
		return damageMod*2;
	}

	public bool shouldWeGetRidOfEffect()
	{
		if (workOutNumberOfDaysSince () > length) {
			return true;
		}
		return false;
	}

	public void onAdd()
	{
		EffectsManager.me.destroyEffectOnAdd (effectToRemoveOnAdd);
	}

	public void destroyEffect()
	{
		EffectsManager.me.effectsOnPlayer.Remove (this);
		if (effectToAddOnRemoval != "") {
			if (randomChanceForEffect == true) {
				int r = Random.Range (0, 100);
				if (r < effectChange) {
					EffectsManager.me.addEffectToPlayer (effectToAddOnRemoval);
				}
			} else {
				EffectsManager.me.addEffectToPlayer (effectToAddOnRemoval);

			}
		}
		Destroy (this);
	}

	int workOutNumberOfDaysSince()
	{
		DateTimeStore date = new DateTimeStore ();
		date.day = this.day;
		date.month = this.month;
		date.year = this.year;
		date.min = 0;
		date.hour = 0;
		int numberOfDays = 0;
		int month = date.month;
		int day = date.day;


		if (date.month > TimeScript.me.month) {
			if (date.year < TimeScript.me.year) {
				numberOfDays += (date.month+1)*30;
			}
		}

		if (date.month < TimeScript.me.month) {
			while (month != TimeScript.me.month) {
				numberOfDays += TimeScript.me.daysInMonth [month];
				day = 0;
				month++;
			}
		}


		if (date.month == TimeScript.me.month) {
			if (day < TimeScript.me.day) {
				while (day < TimeScript.me.day) {
					numberOfDays++;
					day++;
				}
			} 
		}
		//Debug.Log ("Number of days since effect started is" + numberOfDays);
		return numberOfDays;
	}


}
