using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NPCID : MonoBehaviour {
	public int myId=-1;

	public string firstName="",lastName="";

    //activate the NPC
    public List<string> started_missionsToActivateOn,finished_missionsToActivateOn;
    public List<int> idsToActivateOnIfDead;
    //deactivate the NPC
    public List<string> started_missionsToDeactivateOn, finished_missionsToDeactivateOn;
    public List<int> idsToDeactivateOnIfDead;
    //make NPC hostlie
    public List<string> started_missionsToMakeHostile, finished_missionsToMakeHostile;
    public List<int> idsThatMakeHostileOnDeath;

    //make NPC non hostile
    public List<string> started_missionsToMakeNotHostile, finished_missionsToMakeNotHostile;//TODO add a way to fail missions by killing this NPC
    public List<int> idsThatMakeNotHostileOnDeath;

    public List<string> missionsIFailOnDeath;
    //gonna leave missions for a while

    PersonHealth ph;
    public bool dead=false;

	public int hourOfMurder,dayOfMurder,monthOfMurder,yearOfMurder;
	public bool madeHostileToPlayer = false;
    public bool alwaysActive = false;

    private void Start()
    {
        
        if (myId != -1)
        {
            NPCIDManager.me.addID(this);
        }
        if (shouldIActivate())
        {
            if(shouldIDeactivate())
            {
                this.gameObject.SetActive(false);
            }
            else
            {
                if (dead == true)
                {
                    int timeSinceMurder = TimeScript.me.howManyHoursHavePassed(hourOfMurder, dayOfMurder, monthOfMurder, yearOfMurder);
                    if (timeSinceMurder > 24)
                    {
                        this.gameObject.SetActive(false);
                       // NPCIDManager.me.idsInWorld.Remove(this);
                       // Destroy(this.gameObject);
                    }
                    else
                    {
                        this.GetComponent<PersonHealth>().dealDamage(10000, false);
                    }
                }
                else if (madeHostileToPlayer == true)
                {
                    this.GetComponent<NPCBehaviourDecider>().myType = AIType.uniqueHostile;
                    NPCMemory m = this.GetComponent<NPCMemory>();
                    m.peopleThatHaveAttackedMe = new List<GameObject>();
                    m.peopleThatHaveAttackedMe.Add(CommonObjectsStore.player);
                    m.beenAttacked = true;
                    m.objectThatMadeMeSuspisious = CommonObjectsStore.player;
                }
                else
                {
                    this.GetComponent<NPCBehaviourDecider>().myType = AIType.uniqueNeutral;

                }
            }
            
        }
        else
        {
            this.gameObject.SetActive(false);
        }

        ph = this.GetComponent<PersonHealth>();

    }

    void amIDead()
    {
        if (ph.healthValue <= 0 && dead == false)
        {
            hourOfMurder = TimeScript.me.hour;
            dayOfMurder = TimeScript.me.day;
            monthOfMurder = TimeScript.me.month;
            yearOfMurder = TimeScript.me.year;
            dead = true;
        }
    }

    bool shouldIActivate()
    {
        /*foreach(string st in started_missionsToActivateOn)
        {
            bool b = MissionController.me.isMissionStarted(st);
            if (b == true)
            {
                return true;
            }
        }

        foreach(string st in finished_missionsToActivateOn)
        {
            bool b = MissionController.me.isMissionFinished(st);
            if(b==true)
            {
                return true;
            }
        }

        foreach(int i in idsToActivateOnIfDead)
        {
            List<string> data = LoadingDataStore.me.doesNPCExist(i);
            if(data!=null)
            {
                string[] vals = data[0].Split(';');
                if(vals[0]=="dead")
                {
                    return true;
                }
            }

            foreach(NPCID id in NPCIDManager.me.idsInWorld)
            {
                if(id.myId==i)
                {
                    if(id.dead==true)
                    {
                        return true;
                    }
                }
            }

        }*/

        if(alwaysActive==true)
        {
            return true;
        }

        if(NPCIDManager.me.idsThatShouldBeActive.Contains(myId))
        {
            return true;
        }
        

        return false;
    }

    bool shouldIDeactivate()
    {
      /*  foreach (string st in started_missionsToDeactivateOn)
        {
            bool b = MissionController.me.isMissionStarted(st);
            if (b == true)
            {
                return true;
            }
        }

        foreach (string st in finished_missionsToDeactivateOn)
        {
            bool b = MissionController.me.isMissionFinished(st);
            if (b == true)
            {
                return true;
            }
        }*/

        foreach (int i in idsToDeactivateOnIfDead)
        {
            List<string> data = LoadingDataStore.me.doesNPCExist(i);
            if (data != null)
            {
                string[] vals = data[0].Split(';');
                if (vals[0] == "dead")
                {
                    return true;
                }
            }

            foreach (NPCID id in NPCIDManager.me.idsInWorld)
            {
                if (id.myId == i)
                {
                    if (id.dead == true)
                    {
                        return true;
                    }
                }
            }

        }
        return false;
    }


    public string serializeNPC()
    {
        amIDead();
        //have the ID as the file name, then have the state as the first string
        //States = dead, hostile, non hostile
        //dead;hour;day;month;year;items
        //hostile;items
        //nonHostile;items
        string retval = "";
        if(dead==true)
        {
            retval += "dead:";
            retval += hourOfMurder;
            retval += ":";
            retval += dayOfMurder;
            retval += ":";
            retval += monthOfMurder;
            retval += ":";
            retval += yearOfMurder;
            retval += ":";
            retval += LoadingDataStore.me.vectorToString(this.transform.position);
            retval += ":";
        }
        else
        {
            if(madeHostileToPlayer==true)//don't have a semicolon after the last because serialise item adds it to the start rather than the end
            {
                retval += "hostile:";
            }
            else
            {
                retval += "notHostile:";
            }
        }

        if (dead == false)
        {
            foreach (Item i in this.gameObject.GetComponent<Inventory>().inventoryItems)
            {
                retval += LoadingDataStore.me.serialiseItem(i);
                retval += ":";
            }
        }
        else
        {
            foreach (Item i in this.gameObject.GetComponent<Container>().itemsInContainer)
            {
                retval += LoadingDataStore.me.serialiseItem(i);
                retval += ":";
            }
        }
        return retval;
    }

    public void deserializeNPC(string data)//need to add way of deserializing when the NPC is used, maybe when adding to the manager get it to check if they have existing data to read from.
    {
        string[] split = data.Split(':');
        NPCController myController = this.gameObject.GetComponent<NPCController>();
        NPCBehaviourDecider npcb = this.gameObject.GetComponent<NPCBehaviourDecider>();
        Inventory i = this.gameObject.GetComponent<Inventory>();
        PersonClothesController pcc = this.gameObject.GetComponent<PersonClothesController>();
        if (split[0] == "dead")
        {
            dead = true;
            hourOfMurder = int.Parse(split[1]);
            dayOfMurder = int.Parse(split[2]);
            monthOfMurder = int.Parse(split[3]);
            yearOfMurder = int.Parse(split[4]);
            this.transform.position = LoadingDataStore.me.parseVector(split[5]);
           
            for(int x = 6; x < split.Length; x++)
            {
                if (pcc.clothesToWearAtStart.Contains(split[x]) == false)
                {
                    GameObject g = LoadingDataStore.me.deserialiseItem(split[x]);
                    if(g==null)
                    {
                        continue;
                    }
                    g.transform.position = this.transform.position;
                    i.addItemToInventory(g.GetComponent<Item>());
                }
            }
        }
        else if (split[0] == "hostile")
        {
            madeHostileToPlayer = true;
            for(int x = 1;x<split.Length;x++)
            {
                if (pcc.clothesToWearAtStart.Contains(split[x]) == false)
                {
                    GameObject g = LoadingDataStore.me.deserialiseItem(split[x]);
                    if (g == null)
                    {
                        continue;
                    }
                    g.transform.position = this.transform.position;
                    i.addItemToInventory(g.GetComponent<Item>());
                }
            }
        }else if (split[0] == "nonHostile")
        {
            madeHostileToPlayer = false;
            for (int x = 1; x < split.Length; x++)
            {
                if (pcc.clothesToWearAtStart.Contains(split[x]) == false)
                {
                    GameObject g = LoadingDataStore.me.deserialiseItem(split[x]);
                    if (g == null)
                    {
                        continue;
                    }
                    g.transform.position = this.transform.position;
                    i.addItemToInventory(g.GetComponent<Item>());
                }
            }
        }
    }

   
    public string getFileName()
    {
        return myId.ToString() + ".txt";
    }

    public string serializeNPCForEditor()
    {
        return myId.ToString() + ";" + firstName + ";" + lastName;
    }
}
