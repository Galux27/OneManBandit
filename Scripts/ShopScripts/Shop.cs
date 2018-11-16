using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to control shops  
/// </summary>
public class Shop : MonoBehaviour {
	public static List<Shop> shopsInWorld;
	//public List<Shop> disp;
	public static bool shopsInitialised=false;
	public List<string> itemsICouldSell,itemsIHave,itemsIBuy;
	public List<int> quantityIHave;
	int min=1,hour=1,day=1,month=1,year=1980;//date of generation
	public bool shopAvailable = true;
    public bool linkedToExisting = false;
	public GameObject myKeeper;
	NPCController keeperController;
	NPCBehaviourDecider keeperDecider;
	GameObject shopIcon;
	public float chanceOfHavingItem=100.0f;
    public bool robbed = false;

    bool shopInitialised = false;

	void Awake()
	{
		
		if (shopsInitialised==false) {
            Shop.shopsInWorld = new List<Shop> ();
            Shop.shopsInitialised = true;
		}


		Shop.shopsInWorld.Add (this);
	}

    private void OnDestroy()
    {
        Shop.shopsInWorld.Clear();
        Shop.shopsInitialised = false;
    }
    bool doWeCreate = true;
    BuildingScript buildingIAmIn;
    void Start()
	{
        if(dead==true)
        {
            if (TimeScript.me.howManyHoursHavePassed(0, dayOfSpook, monthOfSpook, yearOfSpook) < 24*5)
            {
                doWeCreate = false;
            }
            else
            {
                dead = false;
            }
        }
        else
        {
            if(spooked==true)
            {
                if (TimeScript.me.howManyHoursHavePassed(0, dayOfSpook, monthOfSpook, yearOfSpook) < 48)
                {
                    doWeCreate = false;
                }
                else
                {
                    spooked = false;
                }
            }

          //  Debug.Log("Hours since shop generation = " + TimeScript.me.howManyHoursHavePassed(0, day, month, year).ToString());
            if (TimeScript.me.howManyHoursHavePassed(0, day, month, year) > 72 || shopInitialised==false)
            {
                generateInventory();
            }
        }

        buildingIAmIn = LevelController.me.getBuildingPosIsIn(this.transform.position);
        if (buildingIAmIn == null)
        {
            
        }
        else
        {
            if (buildingIAmIn.buildingClosed==true)
            {
                doWeCreate = false;
            }
        }
      

        if (doWeCreate == true)
        {
            if (linkedToExisting == false)
            {
                myKeeper = (GameObject)Instantiate(CommonObjectsStore.me.civilian, new Vector3(this.transform.position.x, this.transform.position.y, 0), this.transform.rotation);
            }
            keeperController = myKeeper.GetComponent<NPCController>();
            keeperDecider = myKeeper.GetComponent<NPCBehaviourDecider>();
            keeperController.myType = AIType.shopkeeper;
            keeperDecider.myType = AIType.shopkeeper;

            if (keeperCanIdentifyPlayer == true)
            {
                NPCMemory m = myKeeper.GetComponent<NPCMemory>();
                m.seenSuspect = true;
                m.objectThatMadeMeSuspisious = CommonObjectsStore.player;
            }

            shopIcon = (GameObject)Instantiate(CommonObjectsStore.me.shopIcon, this.transform.position, Quaternion.Euler(0, 0, 0));
            shopIcon.GetComponent<InteractIcon>().setToFollow(myKeeper);
            shopIcon.GetComponent<InteractIcon>().isShopkeeper = true;

            PlayerAction_RobShop rob = myKeeper.AddComponent<PlayerAction_RobShop>();
            rob.myController = keeperController;
            rob.myShop = this;
        }
	}

	void Update()
	{
        if (doWeCreate == true)
        {
            if (keeperController==null|| keeperController.currentBehaviour == null)
            {
                shopAvailable = false;
            }
            else
            {
                if (keeperController.currentBehaviour.myType == behaviourType.shopkeeper && keeperController.knockedDown == false && keeperController.stunTimer <= 0.0f && keeperController.npcB.alarmed == false)
                {
                    if (Vector2.Distance(myKeeper.transform.position, this.transform.position) < 4.0f)
                    {
                        //keeperController.pmc.rotateToFacePosition (this.transform.position);

                        shopAvailable = true;
                    }
                    else
                    {
                        shopAvailable = false;
                    }
                }
                else
                {
                    shopAvailable = false;
                }
            }
        }
        if (shopAvailable)
        {
            if (buildingIAmIn == null)
            {

            }
            else
            {
                if (myKeeper.activeInHierarchy)
                {
                    if (buildingIAmIn.buildingClosed || buildingIAmIn.closedFromIncident)
                    {
                        myKeeper.SetActive(false);
                    }

                }
                else
                {
                    if (buildingIAmIn.buildingClosed ==false && buildingIAmIn.closedFromIncident==false)
                    {
                        myKeeper.SetActive(true);
                    }
                }
            }
        }
	}

	void generateInventory()
	{
        if(itemsICouldSell==null)
        {
            itemsICouldSell = new List<string>();
            quantityIHave = new List<int>();
        }

        day = TimeScript.me.day;
        month = TimeScript.me.month;
        year = TimeScript.me.year;
		itemsIHave = new List<string> ();
		quantityIHave = new List<int> ();
		foreach (string st in itemsICouldSell) {
			int r = Random.Range (0, 100);

			if (r < chanceOfHavingItem) {
				itemsIHave.Add (st);
				quantityIHave.Add (Random.Range (1, 5));
			}
		}
        Debug.LogError("Generated shop inventory");
        shopInitialised = true;
	}

	public List<string> getItemsISell()
	{
		if (itemsICouldSell == null) {
			itemsICouldSell = new List<string> ();
		}
		return itemsICouldSell;
	}

	public List<string> getItemsIBuy()
	{
		if (itemsIBuy == null) {
			itemsIBuy = new List<string> ();
		}
		return itemsIBuy;
	}

	public void addItemToShop(string item)
	{
		if (itemsICouldSell == null) {
			itemsICouldSell = new List<string> ();
		}

		itemsICouldSell.Add (item);
	}

	public void addItemICouldBuy(string item)
	{
		if (itemsIBuy == null) {
			itemsIBuy = new List<string> ();
		}

		itemsIBuy.Add (item);
	}

	public bool canIBuyItem(string name)
	{
		return itemsIBuy.Contains (name);
	}

    bool spooked = false;
    bool dead = false;
    bool keeperCanIdentifyPlayer = false;
    int dayOfSpook = 0, monthOfSpook = 0, yearOfSpook = 0;
    void hasKeeperBeenSpooked()
    {
        if (doWeCreate == true)
        {

            if (keeperController.memory.beenAttacked == true && keeperController.memory.peopleThatHaveAttackedMe.Contains(CommonObjectsStore.player) || keeperController.memory.seenSuspect == true || robbed==true)
            {
                spooked = true;
                dayOfSpook = TimeScript.me.day;
                monthOfSpook = TimeScript.me.month;
                yearOfSpook = TimeScript.me.year;
            }

            if (keeperController.memory.seenSuspectsFace == true)
            {
                keeperCanIdentifyPlayer = true;
            }

            if (keeperController.myHealth.healthValue <= 0)
            {
                dead = true;
                dayOfSpook = TimeScript.me.day;
                monthOfSpook = TimeScript.me.month;
                yearOfSpook = TimeScript.me.year;
            }
        }
    }

    string getBoolAsString(bool val)
    {
        if(val==true)
        {
            return "1";
        }
        else
        {
            return "0";
        }
    }

    bool parseBool(string st)
    {
        if(st=="1")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public string serializeShop()
    {
        hasKeeperBeenSpooked();
        string retVal = "";
        retVal += getBoolAsString(spooked) + ":";
        retVal += getBoolAsString(dead) + ":";
        retVal += getBoolAsString(keeperCanIdentifyPlayer)+":";
        retVal += dayOfSpook.ToString() + ":";
        retVal += monthOfSpook.ToString() + ":";
        retVal += yearOfSpook.ToString() + ":";
        retVal += day.ToString() + ":";
        retVal += month.ToString() + ":";
        retVal += year.ToString() + ":";
        for(int x = 0;x<itemsIHave.Count;x++)
        {
            retVal += itemsIHave[x] + ":" + quantityIHave[x].ToString() + ":";

        }
        return retVal;
    }

    public void deserializeShop(string data)
    {
        string[] split = data.Split(':');
        spooked =parseBool(split[0]);
        dead = parseBool(split[1]);
        keeperCanIdentifyPlayer = parseBool(split[2]);
        if(spooked==true || dead == true)
        {
            dayOfSpook = int.Parse(split[3]);
            monthOfSpook = int.Parse(split[4]);
            yearOfSpook = int.Parse(split[5]);
        }
        
            day = int.Parse(split[6]);
            month = int.Parse(split[7]);
            year = int.Parse(split[8]);
        
        itemsIHave = new List<string>();
        for(int x = 9;x<split.Length;x+=2)
        {
            if (split[x] != "")
            {

                itemsIHave.Add(split[x]);
                quantityIHave.Add(int.Parse(split[x + 1]));
            }
        }
        shopInitialised = true;
        //have the location as the file name and whether 
    }


    public string getFileName()
    {
        Vector3 v = new Vector3(Mathf.Round(this.transform.position.x * 10), Mathf.Round(this.transform.position.y * 10), Mathf.Round(this.transform.position.z * 10));
        return v.ToString() + ".txt";
    }
}
