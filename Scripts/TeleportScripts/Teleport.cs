using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour {
    public Teleport toGoTo;
    public List<GameObject> objectsITeleported;
    List<float> timerResetValues;
   public WorldTile nearest;

    private void Awake()
    {
        objectsITeleported = new List<GameObject>();
        timerResetValues = new List<float>();

        nearest = WorldBuilder.me.getNearest(this.transform.position);
        if (toGoTo.nearest == null)
        {
            toGoTo.nearest = WorldBuilder.me.getNearest(toGoTo.transform.position);
        }

        toGoTo.nearest.myNeighbours.Add(nearest);
        nearest.myNeighbours.Add(toGoTo.nearest);
    }

  
    // Update is called once per frame
    void Update () {
        countdownTimer();
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if (collision.gameObject.GetComponent<NPCController>() == false && collision.gameObject!=CommonObjectsStore.player)
        //  {
        //     return;
        //  }
        if (collision.gameObject.transform.root.GetComponent<NPCController>() == true)
        {
            if (shouldWeTeleportGameObject(collision.gameObject))
            {
                //Debug.LogError("Teleport Collision with " + collision.transform.root.gameObject.name);

                PathFollower pf = collision.transform.root.GetComponent<PathFollower>();
              
                objectsITeleported.Add(collision.gameObject);
                timerResetValues.Add(3.0f);
                collision.transform.root.position = toGoTo.transform.position;
                //pf.workOutPositionOnPath();
                pf.getPath(pf.transform.position, pf.target.transform.position);
            }
        }
        else
        {
            if (shouldWeTeleportGameObject(collision.gameObject))
            {
                if (collision.gameObject.transform.root.gameObject == CommonObjectsStore.player)
                {
                    objectsITeleported.Add(collision.gameObject);
                    timerResetValues.Add(3.0f);
                    collision.transform.root.position = toGoTo.transform.position;

                }
            }
        }
    }

    bool shouldWeTeleportGameObject(GameObject g)
    {
        if (objectsITeleported.Contains(g)==false && toGoTo.objectsITeleported.Contains(g)==false)
        {
            return true;
        }
        return false;
    }

    void countdownTimer()
    {
        for(int x = 0; x < timerResetValues.Count; x++)
        {
            timerResetValues[x] -= Time.deltaTime;
            if(timerResetValues[x]<=0)
            {
                objectsITeleported.RemoveAt(x);
                timerResetValues.RemoveAt(x);
                return;
            }
        }
    }
}
