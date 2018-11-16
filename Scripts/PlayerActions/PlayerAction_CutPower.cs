using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction_CutPower : PlayerAction
{
    ProgressBar myProgress;
    float timer = 10.0f;
    bool doing = false;
    public override void doAction()
    {
        illigal = true;

        if (canDo() == true)
        {
            doing = true;
        }
        else
        {
            doing = false;
        }

    }

    void Update()
    {
        if (PlayerAction.currentAction == null)
        {
            //Debug.Log("Players current action is null");
        }
        else
        {
            if (PlayerAction.currentAction == this)
            {

            }
        }

        if (doing == true)
        {
            if (Vector3.Distance(this.transform.position, CommonObjectsStore.player.transform.position) <= 1.5f)
            {
                if (myProgress == null)
                {
                    createProgressBar();
                }

                if (timer > 0)
                {
                    timer -= Time.deltaTime;
                    myProgress.currentValue = (myProgress.maxValue - timer);
                    doingAction = true;
                    PlayerAction.currentAction = this;
                    foreach (NPCController npc in NPCManager.me.npcControllers)
                    {
                        if (npc == null)
                        {
                            continue;
                        }
                        npc.setHearedGunshot(this.transform.position, 5.0f);
                    }

                }
                else
                {
                    myProgress.currentValue = myProgress.maxValue;
                    Destroy(myProgress.gameObject);
                    onComplete();
                    doingAction = false;
                    PlayerAction.currentAction = null;
                  //  this.enabled = false;
                }
            }
            else
            {
                timer = 10.0f;
                doing = false;
                if (myProgress == null)
                {

                }
                else
                {
                    myProgress.currentValue = (myProgress.maxValue - timer);
                    Destroy(myProgress.gameObject);
                    doingAction = false;
                    PlayerAction.currentAction = null;
                }
            }
        }
        else
        {
            if (myProgress == null)
            {

            }
            else
            {
                myProgress.currentValue = (myProgress.maxValue - timer);
                Destroy(myProgress.gameObject);
                doingAction = false;
                PlayerAction.currentAction = null;
            }
        }

    }

    public override bool canDo()
    {

        if (this.gameObject.GetComponent<CircuitBreaker>() == false)
        {
            Destroy(this);
        }


        if(LightSource.powerCut==true)
        {
            return false;
        }

        if (Vector3.Distance(this.transform.position, CommonObjectsStore.player.transform.position) <= 1.5f)
        {
            return true;
        }
        else
        {
            if (myProgress == null)
            {

            }
            else
            {
                Destroy(myProgress.gameObject);
                timer =10.0f;
                if (PlayerAction.currentAction == this)
                {
                    PlayerAction.currentAction = null;

                }
            }

            return false;
        }
    }

    public void createProgressBar()
    {
        GameObject g = (GameObject)Instantiate(CommonObjectsStore.me.progressBar, Vector3.zero, Quaternion.Euler(0, 0, 0));
        g.transform.parent = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<Canvas>().gameObject.transform;
        myProgress = g.GetComponent<ProgressBar>();
        myProgress.maxValue = timer;
        myProgress.myObjectToFollow = this.gameObject;
        g.transform.localScale = new Vector3(1, 1, 1);
    }

    public override void onComplete()
    {
        //this.transform.parent = null;
        timer = 10.0f;
        this.gameObject.GetComponent<CircuitBreaker>().cutPower();

    }

    public override string getType()
    {
        return "Trip Power";
    }

    public override float getMoveModForAction()
    {
        return -0.0f;
    }

    public override float getRotationModForAction()
    {
        return -0.0f;
    }

    public override string getDescription()
    {
        return "Will disable the power to CCTV and lighting in an area for 20 seconds";
    }
}
