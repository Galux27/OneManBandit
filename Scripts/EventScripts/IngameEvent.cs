using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameEvent : MonoBehaviour {
   public int min = 0, hour = 0, day = 1, month = 1, year = 1970;//time that the event was triggered
    public int myID = -1;//unique event ID, assigned with editor tool and used to reference
    public string eventName="";
    public List<IngameEventCondition> conditionsToTrigger,conditionsToFinish;
    public List<GameObject> objectsToEnable;
    public bool eventTriggered = false, eventFinished = false;
    public bool Repeatable = false;
    public int hoursTillRepeat = 0;
    private void Start()
    {
        IngameEventManager.me.eventsInWorld.Add(this);
        tryToTrigger();
        if(eventTriggered==false)
        {
            foreach(GameObject g in objectsToEnable)
            {
                g.SetActive(false);
            }
        }
    }

    public void tryToTrigger()
    {
        if (eventTriggered == false)
        {
            if (shouldWeEnableEvent())
            {
                enableEvent();
            }
        }
        else
        {
            if (TimeScript.me.howManyHoursHavePassed(hour, day, month, year) >= hoursTillRepeat)
            {
                if (shouldWeEnableEvent())
                {
                    enableEvent();
                }
            }
        }
    }

    bool shouldWeEnableEvent()
    {
        foreach (IngameEventCondition e in conditionsToTrigger)
        {
            if (e.shouldEventBeEnabled() == false)
            {
                return false;
            }
        }
        return true;
    }

    void enableEvent()
    {
        foreach (GameObject g in objectsToEnable)
        {
            g.SetActive(true);
        }
        min = TimeScript.me.minute;
        hour = TimeScript.me.hour;
        day = TimeScript.me.day;
        month = TimeScript.me.month;
        year = TimeScript.me.year;
        eventTriggered = true;
    }

    bool deserailizeBool(string st)
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

    string serializeBool(bool ts)
    {
        if (ts == true)
        {
            return "1";
        }
        else
        {
            return "0";
        }
    }

    public string serializeEvent()
    {
        string st = myID + ":";
        st += serializeBool(eventTriggered) + ":";

        if (eventTriggered==false)
        {
        }
        else
        {
            st += min.ToString() + ":";
            st += hour.ToString() + ":";
            st += day.ToString() + ":";
            st += month.ToString() + ":";
            st += year.ToString() + ":";
        }

        return st;
    }

    public void deserializeEvent(string data)
    {
        string[] split = data.Split(':');
        bool triggered = deserailizeBool(split[1]);
        eventTriggered = triggered;
        if(triggered==false)
        {

        }
        else
        {
            min = int.Parse(split[2]);
            hour = int.Parse(split[3]);
            day = int.Parse(split[4]);
            month = int.Parse(split[5]);
            year = int.Parse(split[6]);
        }
    }
}
