using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrimeRecordScript : MonoBehaviour {

	/*For this script I'll want to keep a record of what crimes have been commited, what time they were committed at & what level they were done in.
	 * Will also want to keep a record of whether the player was seen during the level, Have two tiers of this, one if they are just spotted by a security camera, and another worse one if they are reported by a civilian
	 * and a worse one still if they are seen. 
	 * These values then get combined on exiting the level and get recorded.
	 * 
	 * Things like the number of cops, if any patrol cops are there & their response time are bound by the number of crimes committed overall whether the player is found out or not.
	 * If the player is spotted committing these crimes then they are going to be investigated by the police (have undercover officers follow the player, spawn cars in the levels that contain officers observing the player etc)
	 * 
	 * The alert will gradually decrease at a rate that decreases the higher the overall number so the player can wait for things to cool down but will take a long time
	 * 
	 * may have to find some way to balance the undercover officers, maybe only have them in levels that the player has been seen committing a crime in when the crime levels get too high?
	 * 
	 * Crime Wave: 0) No patrol cops, just responders 1) chance of patrol officers being mixed in with civilians, they'll do civilian actions till the police are called or they see a crime, then they revert to police AI, if they see you commit a crime they will call the police automaticly
	 * 2) More patrol officers and patrol cars that instantly spawn cops when the alarm is raised. 3) Larger quantity of cops respond & at a faster speed 4) Have checkpoints & soldiers near the edges of levels for martial law
	 * 
	 * Investigation: 0) no investigation 1) Request for information (civilians may recognise you) 2) Wanted (civilians more likely to recognise you) 3) Active Investigation (Civilians very likely to recognise you, detective cars in level)
	 * 4) Most Wanted (Civilians always recognise you, undercover officers, detective cars)
	 * 
	 * 
	 * Firearms Incidents 0) none, cops have normal equipment 1) occasional firearms incident Have cops armed with tasers 2) have responder cops armed with glocks 3) have patrol cops armed with glocks 4) responders get MP5's 5) Swat get shotguns & assault rifles
	 * */

	public static CrimeRecordScript me;
	public int crimeLevel;
	public int investigationLevel;
	public int firearmsLevel;
	int dayOfLastChange;
	public bool valsSet=false;
	public bool playerSpottedByCCTV=false;
	//things that increase crime level
	/*Vandalism
	 * Car Theft
	 * Assault 
	 * Murder
	 * Theft
	 * Spotted with a weapon
	 * 
	 * */

	//Things that increase investigation level
	/* Being spotted by civilians *
	 * Being spotted by CCTV *
	 * Being spotted by police *
	 * leaving illigal equipment at the scene of the crime **
	 * leaving any item at the scene of the crime ***
	 * */
	//*only if not wearing some kind of face covering
	//**If not wearing gloves
	//***If not wearing gloves & investigation level 3 or more


	//Things that increase firearms level
	/* Firearms murders
	 * being spotted with firearms
	 * leaving firearms/ammo in crime scenes
	 * */

	void Awake()
	{
		me = this;

	}

	void Start(){
		if (valsSet == false) {
			crimeLevel = 0;
			investigationLevel = 0;
			firearmsLevel = 0;
			valsSet = true;
		}
	}

	void Update()
	{
		if (dayOfLastChange != TimeScript.me.day) {
			calculateCrimeDecay ();
			dayOfLastChange = TimeScript.me.day;
		}
	}

	public List<Crime> crimesFromLevel;

	public void addCrime(Crime toAdd)
	{
		if (doWeAlreadyHaveCrimeReported (toAdd.crime) == true) {
			return;
		}

		if (crimesFromLevel == null) {
			crimesFromLevel = new List<Crime> ();
		}

		crimesFromLevel.Add (toAdd);
	}

	void calculateCrimeIncrease()
	{
        if(crimesFromLevel==null)
        {
            return;
        }
		foreach (Crime c in crimesFromLevel) {
            if (c == null)
            {
                continue;
            }
			int increaseValue = 0;
			if (c.crime == crimeTypes.assault) {
				increaseValue = 250;
			} else if (c.crime == crimeTypes.vandalism) {
				increaseValue = 50;
			} else if (c.crime == crimeTypes.murder) {
				increaseValue = 500;
			} else if (c.crime == crimeTypes.theft) {
				increaseValue = 100;
			} else if (c.crime == crimeTypes.trespass) {
				increaseValue = 50;
			} else if (c.crime == crimeTypes.carTheft) {
				increaseValue = 150;
			} else if (c.crime == crimeTypes.illigalItem) {
				increaseValue = 300;
			} else if (c.crime == crimeTypes.weapon) {
				increaseValue = 300;
			} else if (c.crime == crimeTypes.gun) {
				increaseValue = 500;
			} else if (c.crime == crimeTypes.firearmsMurder) {
				increaseValue = 750;
			}

			crimeLevel += increaseValue;

			if (c.crime == crimeTypes.firearmsMurder || c.crime == crimeTypes.murder || c.crime == crimeTypes.gun) {
				firearmsLevel += increaseValue;
			}

			if (c.playerWasSpotted == true) {
				investigationLevel += increaseValue;
			}
			
		}

		if (crimesFromLevel.Count > 0) {
			if (playerSpottedByCCTV == true) {
				investigationLevel += 250;
			}
		}
	}

	public int getCrimeValue()
	{
		if (crimeLevel <= 1000) {
			return 1;
		} else if (crimeLevel <= 3000) {
			return 2;
		} else if (crimeLevel <= 5000) {
			return 3;
		} else if (crimeLevel <= 8000) {
			return 4;
		} else {
			return 5;
		}
	}

	public int getInvestigationValue()
	{
		if (investigationLevel <= 1000) {
			return 1;
		} else if (investigationLevel <= 3000) {
			return 2;
		} else if (investigationLevel <= 5000) {
			return 3;
		} else if (investigationLevel <= 8000) {
			return 4;
		} else {
			return 5;
		}
	}

	public int getFirearmsValue()
	{
		if (firearmsLevel <= 1000) {
			return 1;
		} else if (firearmsLevel <= 3000) {
			return 2;
		} else if (firearmsLevel <= 5000) {
			return 3;
		} else if (firearmsLevel <= 8000) {
			return 4;
		} else {
			return 5;
		}
	}

	public void calculateCrimeDecay()
	{
		int cl = getCrimeValue ();
		int iv = getInvestigationValue ();
		int fv = getFirearmsValue ();

		if (cl == 1) {
			crimeLevel -= 250;
		} else if (cl == 2) {
			crimeLevel -= 175;
		} else if (cl == 3) {
			crimeLevel -= 100;
		} else if (cl == 4) {
			crimeLevel -= 75;
		} else {
			crimeLevel -= 50;
		}

		if (iv == 1) {
			investigationLevel -= 250;
		} else if (iv == 2) {
			investigationLevel -= 175;
		} else if (iv == 3) {
			investigationLevel -= 100;
		} else if (iv == 4) {
			investigationLevel -= 75;
		} else {
			investigationLevel -= 50;
		}

		if (fv == 1) {
			firearmsLevel -= 250;
		} else if (fv == 2) {
			firearmsLevel -= 175;
		} else if (fv == 3) {
			firearmsLevel -= 100;
		} else if (fv == 4) {
			firearmsLevel -= 75;
		} else {
			firearmsLevel -= 50;
		}


		if (crimeLevel < 0) {
			crimeLevel = 0;
		}

		if (investigationLevel < 0) {
			investigationLevel = 0;
		}

		if (firearmsLevel < 0) {
			firearmsLevel = 0;
		}
	}

	public List<string> getDataToSerialize()
	{
		calculateCrimeIncrease ();
		List<string> retVal = new List<string> ();
		retVal.Add (crimeLevel.ToString ());
		retVal.Add (investigationLevel.ToString ());
		retVal.Add (firearmsLevel.ToString ());
		retVal.Add (dayOfLastChange.ToString ());
		return retVal;
	}

	public void setValues(string[] data)
	{
		crimeLevel = int.Parse (data [0]);
		investigationLevel = int.Parse (data [1]);
		firearmsLevel = int.Parse (data [2]);
		dayOfLastChange = int.Parse (data [3]);
		valsSet = true;
	}

	public bool doWeAlreadyHaveCrimeReported(crimeTypes c)
	{
		if (crimesFromLevel == null) {
			crimesFromLevel = new List<Crime> ();
		}

		foreach(Crime c2 in crimesFromLevel)
		{
			if (c2.crime == c) {
				return true;
			}
		}
		return false;
	}

	public int numberOfCopsToSpawn_Initial(){
		if (getCrimeValue () < 2) {
			return 4;
		} else if (getCrimeValue () < 3) {
			return 6;
		}
		else{
			return 8;
		}
	}

	public int numberOfCopsToSpawn_Backup(){

		if (getCrimeValue () < 2) {
			return 8;
		} else if (getCrimeValue () < 3) {
			return 10;
		}
		else{
			return 12;
		}
	}

	public int numberOfCopsToSpawn_Swat(){
		if (getCrimeValue () < 3) {
			return 8;

		} else {
			return 12;
		}

	}

	public int numberOfCopsToSpawn_Patrol()
	{
		if (getInvestigationValue () > 2) {
			return 0;
		} else {
			return getCrimeValue ();
		}
	}

	public int numberOfCopsToSpawn_Detective()
	{
		if (getInvestigationValue () < 2) {
			return 0;
		} else if (getInvestigationValue () < 3) {
			return 1;
		} else if (getInvestigationValue () < 4) {
			return 2;
		} else {
			return 3;
		}
	}

	public int numberOfCopsToSpawn_PatrolCars()
	{
		if (getCrimeValue () < 2) {
			return 0;
		} else if (getCrimeValue () < 4) {
			return 1;
		} else {
			return 2;
		}
	}

	public int numberOfSoldiersToSpawn()
	{
		return 12;
	}

	public bool shouldWeSpawnPatrolCops()
	{
		if (crimeLevel >= 1000) {
			return true;
		}
		return false;

	}

	public bool shouldWeSpawnDetectives()
	{
		if (investigationLevel >= 3000) {
			return true;
		}

		return false;
	}

	public bool shouldWeSpawnPatrolCars()
	{
		if (crimeLevel >= 3000) {
			return true;
		}

		return false;
	}

	public bool shouldWeSpawnSoldiers()
	{
		if (crimeLevel >= 8000) {
			return true;
		}
		return false;
	}

}

public class Crime{

	public Crime( crimeTypes type, bool spotted){
		crime = type;
		playerWasSpotted = spotted;
	}
	public crimeTypes crime;
	public bool playerWasSpotted;
}

public enum crimeTypes{
	vandalism,
	assault,
	murder,
	firearmsMurder,
	theft,
	trespass,
	carTheft,
	illigalItem,
	weapon,
	gun,
	firearmMurder
}
