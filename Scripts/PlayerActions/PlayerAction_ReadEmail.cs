using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction_ReadEmail : PlayerAction
{
    public override void doAction()
    {
        Email[] emails = this.gameObject.GetComponents<Email>();
        List<Email> filtered = new List<Email>();
        foreach (Email e in emails)
        {
            if(e.shouldWeAddMessage())
            {
                filtered.Add(e);
            }
        }

        EmailController.me.setEmails(filtered);
    }

    public override bool canDo()
    {
        if (Vector2.Distance(this.gameObject.transform.position, CommonObjectsStore.player.transform.position) < 2.5f)
        {
            return true;
        }
        return false;
    }

    public override string getType()
    {
        return "Read Emails";
    }

    public override string getDescription()
    {
        return "Read the emails on the computer.";
    }
}
