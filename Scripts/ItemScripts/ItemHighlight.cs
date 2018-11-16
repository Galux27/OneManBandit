using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHighlight : MonoBehaviour
{
    SpriteRenderer sr;
    Color originalSRColor;
    // Start is called before the first frame update
    void Start()
    {
        sr = this.GetComponent<SpriteRenderer>();
        if(sr==null)
        {
            Destroy(this);
        }
        originalSRColor = sr.color;
    }

    // Update is called once per frame
    void Update()
    {
        sr.color = GetColor();
    }

    Color GetColor()
    {
        Color retVal = new Color(Mathf.PingPong(Time.time, 1.0f)+0.5f, Mathf.PingPong(Time.time, 1.0f) + 0.5f, Mathf.PingPong(Time.time, 1.0f) + 0.5f);
        return retVal;
    }
}
