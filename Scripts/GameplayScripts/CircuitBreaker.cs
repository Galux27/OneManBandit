using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitBreaker : MonoBehaviour
{
   public LightSource[] sourcesInWorld;

    private void Start()
    {
       // if (CircuitBreaker.sourcesInWorld == null)
     //   {
            sourcesInWorld = FindObjectsOfType<LightSource>();
      //  }
    }

    List<LightSource> lightsITurnedOff;
    float cutLength = 20.0f;
    public void cutPower()
    {

        foreach(CCTVCamera c in CCTVController.me.camerasInWorld)
        {
            c.cameraActive = false;

        }
        lightsITurnedOff = new List<LightSource>();
        foreach(LightSource l in sourcesInWorld)
        {
            if (l == LightSource.sun)
            {
                continue;
            }

            if(l.lightOn==true)
            {
                lightsITurnedOff.Add(l);
            }
            l.lightOn = false;
        }


        
        LightSource.UpdateLightMeshes();
        LightSource.powerCut = true;


    }

    private void Update()
    {
        if (LightSource.powerCut)
        {
            timerCount();
        }
    }

    void timerCount()
    {
        cutLength -= Time.deltaTime;

        if(cutLength<=0)
        {
            LightSource.powerCut = false;
            foreach(LightSource l in lightsITurnedOff)
            {
                l.lightOn = true;
            }
            foreach (CCTVCamera c in CCTVController.me.camerasInWorld)
            {
                c.cameraActive = true;

            }

            LightSource.UpdateLightMeshes();
            cutLength = 20.0f;
            
        }

        
    }


}
