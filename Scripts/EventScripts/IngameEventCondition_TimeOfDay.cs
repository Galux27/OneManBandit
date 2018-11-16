using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameEventCondition_TimeOfDay : IngameEventCondition
{
    public int Smin = 0, Shour = 0,Emin=59,Ehour=23;
    public override bool shouldEventBeEnabled()
    {
        if(TimeScript.me.hour > Shour && TimeScript.me.hour < Ehour)
        {
            if(TimeScript.me.minute>Smin)
            {
                return true;
            }
        }else if(TimeScript.me.hour==Ehour)
        {
            if(TimeScript.me.minute<Emin)
            {
                return true;
            }
        }else if(TimeScript.me.hour==Shour)
        {
            if(TimeScript.me.minute>Smin)
            {
                return true;
            }
        }
        return false;
    }
}
