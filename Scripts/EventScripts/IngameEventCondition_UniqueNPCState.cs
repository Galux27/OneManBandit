using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameEventCondition_UniqueNPCState : IngameEventCondition
{
    public bool deadState=false;
    public int IDToCheck = -1;
    public override bool shouldEventBeEnabled()
    {
        List<string> data = LoadingDataStore.me.doesNPCExist(IDToCheck);

        if (data == null)
        {
            return true;
        }
        else
        {
        string[] d = data[0].Split(':');
            if (d[0]=="dead")
            {
                if(deadState==true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (deadState == true)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }


        return false;
    }
}
