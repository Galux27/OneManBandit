using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction_UsePhone : PlayerAction
{
    public override void doAction()
    {
        PhoneMessage[] messages = this.gameObject.GetComponents<PhoneMessage>();
        List<PhoneMessage> m = new List<PhoneMessage>();
        foreach (PhoneMessage mess in messages)
        {
            if (mess.shouldWeAddMessage())
            {
                m.Add(mess);
            }
        }
        PhoneMessageUI.me.setMessages(m);
    }

    public override bool canDo()
    {
        if(Vector2.Distance(this.gameObject.transform.position,CommonObjectsStore.player.transform.position)<2.5f)
        {
            return true;
        }
        return false;
    }

    public override string getType()
    {
        return "Use Phone";
    }

    public override string getDescription()
    {
        return "Look through this phones messages.";
    }
}
