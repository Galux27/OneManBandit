using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PhoneMessageUI : MonoBehaviour {
    public static PhoneMessageUI me;
    List<PhoneMessage> messagesAdded;
    public GameObject menuParent, displayParent,parent;
    public Text[] messageDisplayText;
    public Text messageDisplaySender;
    public Text messageDisplayMessage;
    int messageIndex = 0;

    private void Awake()
    {
        me = this;
        parent.SetActive(false);
    }

    public Text above, below;
    public void setMessages(List<PhoneMessage> phoneMessages)
    {
        parent.SetActive(true);
        menuParent.SetActive(true);
        messagesAdded = phoneMessages;
        messageIndex = 0;
        resetDisplay();
    }

    public void increaseIndex()
    {
        if(messageIndex<messagesAdded.Count-1)
        {
            messageIndex++;
            resetDisplay();
        }
    }
	
    public void decreaseIndex()
    {
        if (messageIndex > 0)
        {
            messageIndex--;
            resetDisplay();
        }
    }

    public void backToMenu()
    {
        displayParent.SetActive(false);
        menuParent.SetActive(true);
        resetDisplay();
    }

    public void closePhone()
    {
        displayParent.SetActive(false);
        menuParent.SetActive(true);
        parent.SetActive(false);
    }

    public void readMessage()
    {
        PhoneMessage toDisp = messagesAdded[messageIndex];
        displayParent.SetActive(true);
        menuParent.SetActive(false);
        messageDisplaySender.text = toDisp.sender;
        messageDisplayMessage.text = toDisp.messageText;
    }

    void resetDisplay()
    {
        for(int x =0;x<4;x++)
        {
            if (messageIndex + x < messagesAdded.Count)
            {
                if(x==0)
                {
                    messageDisplayText[x].color = Color.white;
                }
                else
                {
                    messageDisplayText[x].color = Color.grey;
                }

                messageDisplayText[x].gameObject.SetActive(true);
                PhoneMessage toDisp = messagesAdded[messageIndex + x];
                messageDisplayText[x].text = toDisp.sender + ":" + toDisp.daySend + "/" + toDisp.monthSend + "/" + toDisp.yearSend;
            }
            else
            {
                messageDisplayText[x].gameObject.SetActive(false);

            }
        }

        if (shouldWeDisplayAboveIndicator())
        {
            above.gameObject.SetActive(true);
        }
        else
        {
            above.gameObject.SetActive(false);
        }

        if(shouldWeDisplayBelowIndicator())
        {
            below.gameObject.SetActive(true);
        }
        else
        {
            below.gameObject.SetActive(false);
        }
    }

    bool shouldWeDisplayBelowIndicator()
    {
        if(messageIndex+4 >= messagesAdded.Count)
        {
            return false;
        }
        return true;
    }

    bool shouldWeDisplayAboveIndicator()
    {
        if(messageIndex>=4)
        {
            return true;
        }
        return false;
    }
}
