using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameEventManager : MonoBehaviour {
    public static IngameEventManager me;
    public List<IngameEvent> eventsInWorld;

    private void Awake()
    {
        me = this;
        eventsInWorld = new List<IngameEvent>();
    }

    private void Update()
    {
        foreach(IngameEvent ie in eventsInWorld)
        {
            if(ie.eventTriggered==false)
            {
                ie.tryToTrigger();
            }
        }
    }

    public List<string> serializeEventData()
    {
        List<string> retVal = new List<string>();

        foreach(IngameEvent e in eventsInWorld)
        {
            retVal.Add(e.serializeEvent());
        }
        return retVal;
    }

    public void deserializeEventData(string[] data)
    {
        IngameEvent[] events = FindObjectsOfType<IngameEvent>();

        foreach(IngameEvent e in events)
        {
            foreach(string st in data)
            {
                int id = int.Parse(st.Split(':')[0]);
                if(id==e.myID)
                {
                    e.deserializeEvent(st);
                    break;
                }
            }
        }
    }
}
