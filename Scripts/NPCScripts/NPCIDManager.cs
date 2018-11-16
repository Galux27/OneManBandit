using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class NPCIDManager : MonoBehaviour {

	public static NPCIDManager me;
	public List<NPCID> idsInWorld;
    public List<int> idsThatShouldBeActive;
	void Awake()
	{
		me = this;
        idsInWorld = new List<NPCID>();
		//NPCID[] id = FindObjectsOfType<NPCID> ();
		//if (idsInWorld == null) {
		//	idsInWorld = new List<NPCID> ();
		//}
		//foreach (NPCID ni in id) {
		//	idsInWorld.Add (ni);
		//}
	}

	public void addID(NPCID toAdd)//add checks for serialization and load from data if so
	{
        if (idsInWorld == null)
        {
            idsInWorld = new List<NPCID>();
        }
		idsInWorld.Add (toAdd);
        List<string> data = LoadingDataStore.me.doesNPCExist(toAdd);
        if(data==null)
        {

        }
        else
        {
            toAdd.deserializeNPC(data[0]);
        }
	}


    public void addIdToActivate(int id)
    {
        if (idsThatShouldBeActive.Contains(id) == false)
        {
            idsThatShouldBeActive.Add(id);
        }
    }

    public List<string>serializeActiveIDs()
    {
        List<string> retVal = new List<string>();

        foreach(int i in idsThatShouldBeActive)
        {
            retVal.Add(i.ToString());
        }

        return retVal;
    }

    public void setIDs(string[] data)
    {
        idsThatShouldBeActive = new List<int>();
        foreach(string st in data)
        {
            idsThatShouldBeActive.Add(int.Parse(st));
        }
    }

    public GameObject getByID(int num)
	{
		foreach (NPCID i in idsInWorld) {
			if (num == i.myId) {
				return i.gameObject;
			}
		}
		return null;
	}

    public void serializeNPCs(string filePath)
    {
        foreach(NPCID n in idsInWorld)
        {
            if(n==null)
            {
                continue;
            }
            List<string> val = new List<string>();
            val.Add(n.serializeNPC());
            LoadingDataStore.me.writeListToFile(filePath, n.getFileName(), val.ToArray());
        }
    }

 
    
}

