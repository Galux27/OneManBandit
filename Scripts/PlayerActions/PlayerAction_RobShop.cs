using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction_RobShop : PlayerAction {
    public Shop myShop;
    public NPCController myController;
    public override void doAction()
    {
        myShop.shopAvailable = false;
        myController.memory.seenSuspect = true;
        myController.memory.objectThatMadeMeSuspisious = CommonObjectsStore.player;
        onComplete();

      

    }

    public override bool canDo()
    {
        if (myShop == null || myShop.robbed==true)
        {
            //Debug.LogError("Shop Inaccessable");
            return false;
        }

        if(myController==null || myController.myHealth.healthValue<=0 ||Vector2.Distance(myController.transform.position,myShop.transform.position)>10)
        {
            //Debug.LogError("Keeper Inaccessable");
            return false;
        }

        return true;
    }

    public override void onComplete()
    {
        
        if (CommonObjectsStore.pwc.currentWeapon != null || myController.myHealth.healthValue<5200)
        {
            myController.myText.setText("Okay, okay don't hurt me");
            dropLoot();
        }
        else
        {
            myController.myText.setText("Fuck you, you're not getting anything!");
        }
        LevelIncidentController.me.addIncident("Robbery", myShop.transform.position);
    }

    void dropLoot()
    {
        int valOfMoney = 0;
        for(int x = 0;x<myShop.itemsIHave.Count;x++)
        {
            for(int y = 0; y < myShop.quantityIHave[x]; y++)
            {
                GameObject g = ItemDatabase.me.getItem(myShop.itemsIHave[x]);
                if(g==null)
                {

                }
                else
                {
                    GameObject instance = (GameObject)Instantiate(g, this.transform.position + new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0.0f), this.transform.rotation);
                    valOfMoney += instance.GetComponent<Item>().price;
                }
            }
            myShop.quantityIHave[x] = 0;
            myShop.robbed = true;
        }

        while(valOfMoney>50)
        {
            if(valOfMoney>100)
            {
                GameObject g = ItemDatabase.me.getItem("£100");
                GameObject instance = (GameObject)Instantiate(g, this.transform.position + new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0.0f), this.transform.rotation);
                valOfMoney -= 100;
            }
            else if(valOfMoney>50)
            {
                GameObject g = ItemDatabase.me.getItem("£50");
                GameObject instance = (GameObject)Instantiate(g, this.transform.position + new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0.0f), this.transform.rotation);
                valOfMoney -= 100;
            }
            else
            {
                int r = Random.Range(0, 10);
                for (int x = 0; x < r; x++)
                {
                    GameObject g = ItemDatabase.me.getItem("Change");
                    GameObject instance = (GameObject)Instantiate(g, this.transform.position + new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0.0f), this.transform.rotation);
                }
            }
        }
    }

    public override float getMoveModForAction()
    {
        return -0.0f;
    }

    public override float getRotationModForAction()
    {
        return -0.0f;
    }

    public override string getType()
    {
        return "Rob Shop";
    }

    public override string getDescription()
    {
        return "Rob the shops money and goods.";
    }
}