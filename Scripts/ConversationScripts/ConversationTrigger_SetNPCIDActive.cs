using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationTrigger_SetNPCIDActive : ConversationTrigger
{

    public List<int> idsToAdd;
    public override void OnOptionSelect()
    {
        foreach(int i in idsToAdd)
        {
            NPCIDManager.me.addIdToActivate(i);

        }

    }



}