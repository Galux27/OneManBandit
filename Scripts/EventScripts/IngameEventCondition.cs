using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameEventCondition : MonoBehaviour {

	public virtual bool shouldEventBeEnabled()
    {
        return false;
    }
}
