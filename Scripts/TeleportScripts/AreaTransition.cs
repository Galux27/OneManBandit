using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTransition : MonoBehaviour {
    public string sceneToGoTo = "",sceneName="";
    public int startID = 0;
    bool canTransition = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.root.gameObject == CommonObjectsStore.player)
        {
            canTransition = true;
            PhoneAlert.me.setMessageText("Press Q to travel to " + sceneName);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.transform.root.gameObject==CommonObjectsStore.player)
        {
            canTransition = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.root.gameObject == CommonObjectsStore.player)
        {
            canTransition = false;
        }
    }

    private void Update()
    {
        if (canTransition && Input.GetKeyDown(KeyCode.Q))
        {
            transition();
        }
    }

    void transition()
    {
        PlayerPrefs.SetInt("StartPos", startID);
        LoadScreen.loadScreen.loadGivenScene(sceneToGoTo);
    }
}
