using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EmailController : MonoBehaviour {

    public static EmailController me;

    private void Awake()
    {
        me = this;
        wholeParent.SetActive(false);
    }

    public List<Email> emailsToDisplay;
    public EmailDisplay[] myDisplays;
    public int index = 0;
    public GameObject menuParent, displayParent, wholeParent;
    public void setEmails(List<Email> disp)
    {
        emailsToDisplay = disp;
        index = 0;
        wholeParent.SetActive(true);
        menuParent.SetActive(true);
        setDisplays();
    }

    public void openClose()
    {
        wholeParent.SetActive(!wholeParent.activeInHierarchy);
    }

    private void Update()
    {
        if (TimeScript.me == null)
        {
            return;
        }

            curDate.text = TimeScript.me.day + "/" + TimeScript.me.month + "/" + TimeScript.me.year + " " + TimeScript.me.hour + ":" + TimeScript.me.minute;
    }


    public Text dispSender, dispSubject, dispMessage,dispDate;
    public Text curDate, moreAbove, moreBelow;
    public void increaseIndex()
    {
        if (index + myDisplays.Length < emailsToDisplay.Count )
        {
            index++;
        }
        setDisplays();
    }

    public void decreaseIndex()
    {
        if(index>0)
        {
            index--;
        }
        setDisplays();
    }

    bool displayAbove()
    {
        if(index>0)
        {
            return true;
        }
        return false;
    }

    bool displayBelow()
    {
       if(index+myDisplays.Length<emailsToDisplay.Count)
       {
            return true;
       }
        return false;
    }

    public void readEmail(int indMod)
    {
        menuParent.SetActive(false);
        displayParent.SetActive(true);
        //Debug.LogError("Index = " + (indMod + index));
        Email toDisp = emailsToDisplay[indMod+index];

        dispSender.text = toDisp.sender;
        dispSubject.text = toDisp.subject;
        dispMessage.text = toDisp.contents;
        dispDate.text = toDisp.daySend + "/" + toDisp.monthSend + "/" + toDisp.yearSend;
    }

    public void back()
    {
        displayParent.SetActive(false);
        menuParent.SetActive(true);
    }

    public void close()
    {
        index = 0;
        displayParent.SetActive(false);
        menuParent.SetActive(false);
    }

    void setDisplays()
    {
        for(int x= 0;x<myDisplays.Length;x++)
        {
            if(index+x > emailsToDisplay.Count-1)
            {
                myDisplays[x].gameObject.SetActive(false);
            }
            else
            {
                Email toDisp = emailsToDisplay[x + index];
                myDisplays[x].gameObject.SetActive(true);
                myDisplays[x].emailDetails.text = toDisp.sender + " - " + toDisp.subject;
                myDisplays[x].emailDate.text = toDisp.daySend + "/" + toDisp.monthSend + "/" + toDisp.yearSend;
                if (x == 0)
                {
                    myDisplays[x].emailDate.color = Color.black;
                    myDisplays[x].emailDetails.color = Color.black;
                }
                else
                {
                    myDisplays[x].emailDate.color = Color.grey;
                    myDisplays[x].emailDetails.color = Color.grey;
                }
            }
        }
        moreAbove.gameObject.SetActive(displayAbove());
        moreBelow.gameObject.SetActive(displayBelow());
    }

}
