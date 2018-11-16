using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Email : MonoBehaviour {

    public string sender="", subject="", contents="";
    public int daySend=1, monthSend=1, yearSend = 1970;

    private void Start()
    {
        if(this.gameObject.GetComponent<PlayerAction_ReadEmail>()==false)
        {
            this.gameObject.AddComponent<PlayerAction_ReadEmail>();
        }

        if(this.gameObject.GetComponent<ItemHighlight>()==false)
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
        //Debug.Log("Email was sent " + TimeScript.me.howManyHoursHavePassed(0, daySend, monthSend, yearSend));
        if (TimeScript.me.howManyHoursHavePassed(0, daySend, monthSend, yearSend) > 0)
        {
            return true;
        }
        return false;
    }
}
