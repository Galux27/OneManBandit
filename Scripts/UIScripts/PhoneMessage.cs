using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneMessage : MonoBehaviour {
    public string sender="";
    public int daySend = 1, monthSend = 1, yearSend = 1970;
    public string messageText = "";

    void Awake()
    {
        if (this.gameObject.GetComponent<PlayerAction_UsePhone>() == false)
        {
            this.gameObject.AddComponent<PlayerAction_UsePhone>();
        }

        if (this.gameObject.GetComponent<ItemHighlight>() == false)
        {
            this.gameObject.AddComponent<ItemHighlight>();
        }
    }

    public bool shouldWeAddMessage()
    {
        /*if (TimeScript.me.year > yearSend)
        {
            return true;
        }else if(TimeScript.me.month>monthSend)
        {
            return true;
        }else if (TimeScript.me.day > daySend)
        {
            return true;
        }*/
        //Debug.Log("Time passed since sent = " + TimeScript.me.howManyHoursHavePassed(0, daySend, monthSend, yearSend));
        if(TimeScript.me.howManyHoursHavePassed(0,daySend,monthSend,yearSend)>0)
        {
            return true;
        }
        return false;
    }
	
}
