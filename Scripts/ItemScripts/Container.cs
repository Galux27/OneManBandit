using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour {
	public string containerName="";
	public List<Item> itemsInContainer;
	public float maxWeight = 10.0f;
	public SpriteAnimation openContainerAnim,closeContainerAnim,anim;
	public bool open = false;
	public bool animate = false;
	public bool locked = false;
	public bool initialised = false;
	public AudioClip openSfx;

    public List<string> itemsICouldAdd;
    public List<float> chanceOfItem;

	void Awake()
	{
		sr = this.GetComponent<SpriteRenderer> ();
		this.gameObject.AddComponent<AudioController> ();

	}

	void Start()
	{
		actionsToAdd ();
        if(initialised==false)
        {
            generateInventory();
            initialised = true;
        }
	}


	float timer = 0.0f;
	int counter = 0;
	SpriteRenderer sr;
	public void playAnimation()
	{
		if (anim == null) {
			return;
		}

		if (counter >= anim.spritesInAnimation.Length) {
			counter = anim.spritesInAnimation.Length - 1;
		}

		timer -= Time.deltaTime;
		sr.sprite = anim.spritesInAnimation [counter];

		if (timer <= 0) {
			if (counter < anim.spritesInAnimation.Length-1) {
				counter++;
				timer = anim.timePerFrame;

			} else {
				counter=0;
				timer = anim.timePerFrame;
				animate = false;
			}
		}
	}

	void Update()
	{
		if (animate == true) {
			playAnimation ();
		}

		if (open == true && Inventory.playerInventory.inventoryGUI.activeInHierarchy == false) {
			closeContainer ();
		}
	}

	public void actionOpenContainer()
	{
		if (open == false) {
			this.gameObject.GetComponent<AudioController> ().playSound (openSfx);
			openContainer ();
			if (Inventory.playerInventory.inventoryGUI.activeInHierarchy == false) {
				Inventory.playerInventory.enableInv ();
			} 
		} else {
			this.gameObject.GetComponent<AudioController> ().playSound (openSfx);

			closeContainer ();
			if (Inventory.playerInventory.inventoryGUI.activeInHierarchy == true) {
				Inventory.playerInventory.disableInv ();
			}
		}
	}

	void actionsToAdd()
	{
		if (this.gameObject.tag != "Player" && this.gameObject.tag != "NPC" && this.GetComponent<PlayerAction_SearchContainer>()==false) {
			this.gameObject.AddComponent<PlayerAction_SearchContainer> ();
		}
	}

	public void openContainer()
	{
		if (locked == true) {
			return;
		}


		//play opening animation
		anim = openContainerAnim;
		open = true;
		animate = true;
		ItemMoniter.me.setOpenContainer(this);
	}

	public void closeContainer()
	{
		anim = closeContainerAnim;
		open = false;
		animate = true;
	}

	public float getCurrentWeight()
	{
		float retVal = 0.0f;

		foreach (Item i in itemsInContainer) {
			retVal += i.itemWeight;
		}

		return retVal;
	}

	public bool canWeAddItemToContainer(Item i)
	{
		if (getCurrentWeight () + i.itemWeight <= maxWeight) {
			return true;
		} else {
			return false;
		}
	}

	public string getDisplay(){
		return getCurrentWeight ().ToString() + "/" + maxWeight + " KG";
	}

	public void addItemToContainer(Item i)
	{
		if (canWeAddItemToContainer (i)) {
			if (itemsInContainer == null) {
				itemsInContainer = new List<Item> ();
			}

			itemsInContainer.Add (i);
			i.gameObject.SetActive (false);
			i.myContainer = this;
			i.gameObject.transform.parent = this.transform;
			i.transform.localPosition = new Vector3 (0, 0, 0);
		}
	}



	public Item removeItemFromContainer(Item i)
	{

		if (i.gameObject.GetComponent<ClothingItem> () == true) {
			i.gameObject.GetComponent<ClothingItem> ().unequipItem ();
		}

		itemsInContainer.Remove (i);
		i.gameObject.SetActive (true);
		i.transform.parent = null;
		return i;
	}

    public bool shouldWeSerializeContainer()
    {
        if (transform.root.tag == "NPC" || transform.root.tag == "Player" || transform.root.tag == "Dead/Knocked" || transform.root.tag == "Car")
        {
            return false;
        }

        return false;
    }

    int day=1, month=1, year=1971;
    public string serializeContainer()
    {
        string retVal = "";
        retVal += day.ToString() + ":";
        retVal += month.ToString() + ":";
        retVal += year.ToString() + ":";
        List<string> itemsToNotKeep = new List<string>();
        itemsToNotKeep.Add("Key");
        itemsToNotKeep.Add("Note");
        itemsToNotKeep.Add("Keycard");

        foreach (Item i in itemsInContainer)
        {
            if (itemsToNotKeep.Contains(i.itemName) == false)
            {
                retVal += LoadingDataStore.me.serialiseItem(i) + ":";
            }
        }
        return retVal;
    }

    public void deserializeContainer(string data)
    {
        ////Debug.LogError(this.gameObject.name + " Is being deserialized");
        clearInventory();
        string[] split = data.Split(':');
        day = int.Parse(split[0]);
        month = int.Parse(split[1]);
        year = int.Parse(split[2]);
        if (TimeScript.me.howManyHoursHavePassed(0, day, month, year) > 72)
        {
            generateInventory();
        }
        else
        {
            for (int x = 3; x < split.Length; x++)
            {
                GameObject g = LoadingDataStore.me.deserialiseItem(split[x]);
                if (g != null)
                {
                    GameObject instance = (GameObject)Instantiate(g, this.transform.position, this.transform.rotation);
                    addItemToContainer(instance.GetComponent<Item>());
                }

            }
        }
        initialised = true;
    }
    void generateInventory()
    {


        if (ItemDatabase.me==null)
        {
            ItemDatabase id = FindObjectOfType<ItemDatabase>();

            ItemDatabase.me = id;
        }

        if(itemsICouldAdd==null)
        {
            return;
        }

        for(int x = 0;x<itemsICouldAdd.Count;x++)
        {
            float r = Random.Range(0.0f, 100.0f);

            if (r < chanceOfItem[x])
            {
                GameObject instance = ItemDatabase.me.getItem(itemsICouldAdd[x]);
                if(instance!=null)
                {
                    GameObject g = (GameObject)Instantiate(instance, this.transform.position, this.transform.rotation);
                    addItemToContainer(g.GetComponent<Item>());
                }
            }
        }
        day = TimeScript.me.day;
        month = TimeScript.me.month;
        year = TimeScript.me.year;
    }

    void clearInventory()
    {
        foreach(Item i in itemsInContainer)
        {
            Destroy(i.gameObject);

        }
        itemsInContainer.Clear();
    }

    public string getFileName()
    {
        Vector3 v = new Vector3(Mathf.Round(this.transform.position.x*10), Mathf.Round(this.transform.position.y * 10), Mathf.Round(this.transform.position.z * 10));
        return v.ToString() + ".txt";
    }
}
