using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetProjectorColor : MonoBehaviour
{
   public Material m;
    private void Start()
    {
        m = this.gameObject.GetComponent<Projector>().material;
    }

    // Update is called once per frame
    void Update()
    {
        m.SetColor(0, TimeScript.me.getSunColor());
        m.SetColor("Tint", TimeScript.me.getSunColor());
    }
}
