using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInWorld : MonoBehaviour {
    public static List<ItemInWorld> itemsInWorld;
    public bool respawn = false;
    public int respawnRate = 0;//hours before respawn

    public Item myItem,instance;

    bool createdItem=false;
    int dayCreated, monthCreated, yearCreated;
    private void Start()
    {
        if (ItemInWorld.itemsInWorld == null)
        {
            ItemInWorld.itemsInWorld = new List<ItemInWorld>();
        }

        ItemInWorld.itemsInWorld.Add(this);
        if(createdItem==false)
        {
            createItem();
        }
    }

    void createItem()
    {
        if(myItem==null)
        {
            return;
        }

        instance = Instantiate(myItem.gameObject, this.transform.position, this.transform.rotation).GetComponent<Item>() ;
        instance.GetComponent<SpriteRenderer>().sortingOrder = this.GetComponent<SpriteRenderer>().sortingOrder;
        dayCreated = TimeScript.me.day;
        monthCreated = TimeScript.me.month;
        yearCreated = TimeScript.me.year;
        createdItem = true;
        this.gameObject.SetActive(false);
    }

    bool hasItemBeenPickedUp()
    {
        if(instance==null)
        {
            dayCreated = TimeScript.me.day;
            monthCreated = TimeScript.me.month;
            yearCreated = TimeScript.me.year;
            return true;
        }
        else
        {
            if(instance.gameObject.activeInHierarchy==true || instance.gameObject.transform.parent == null)
            {
                return false;
            }
            else
            {
                dayCreated = TimeScript.me.day;
                monthCreated = TimeScript.me.month;
                yearCreated = TimeScript.me.year;
                return true;
            }
        }
    }

    string getBoolAsString(bool val)
    {
        if (val == true)
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
        if (st == "1")
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public string serializeItem()
    {
        //itemPickedUp,day,month,year
        string retVal="";
        retVal += getBoolAsString(hasItemBeenPickedUp()) + ":";
        retVal += dayCreated.ToString() + ":";
        retVal+=monthCreated.ToString()+":";
        retVal += yearCreated.ToString();
        return retVal;
    }

    public void deserializeItem(string data)
    {
        string[] d = data.Split(':');
        bool pickedUp = parseBool(d[0]);
        dayCreated = int.Parse(d[1]);
        monthCreated = int.Parse(d[2]);
        yearCreated = int.Parse(d[3]);
        if(pickedUp==true)
        {
            if(shouldWeRespawn()==true)
            {
                createItem();
            }
        }
        else
        {
            createItem();
        }
    }

    bool shouldWeRespawn()
    {
        if(respawn==false)
        {
            return false;
        }

        if (TimeScript.me.howManyHoursHavePassed(0, dayCreated, monthCreated, yearCreated) >= respawnRate)
        {
            return true;
        }
        return false;
    }

    public string getFileName()
    {
        Vector3 v = new Vector3(Mathf.Round(this.transform.position.x * 10), Mathf.Round(this.transform.position.y * 10), Mathf.Round(this.transform.position.z * 10));
        return v.ToString() + ".txt";
    }
}
