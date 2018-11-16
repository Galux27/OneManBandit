using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameEventConditionRandomChance : IngameEventCondition
{
    public int maxValue = 100,threshold=0,generated=-1;
    public override bool shouldEventBeEnabled()
    {
       if(generated==-1)
        {
            generated = Random.Range(0,maxValue);
        }

        if (generated < threshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
