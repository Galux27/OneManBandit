using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightObjectWithPlayerActions : MonoBehaviour {
	public bool initialised = false;
	PlayerAction[] myActions;
	SpriteOutline so;
	public SpriteRenderer sr;

	public void Initialise()
	{
		myActions = this.GetComponents<PlayerAction> ();
		if (this.GetComponent<SpriteRenderer> () == true) {
			sr = this.GetComponent<SpriteRenderer> ();
		}

		if (sr == null) {
			sr = this.gameObject.GetComponentInChildren<SpriteRenderer> ();
		}
		if (sr == null) {
			Destroy (this);
		} else {
			sr.material = CommonObjectsStore.me.spriteOutline;
			so = this.gameObject.AddComponent<SpriteOutline> ();
			so.sr = sr;
			so.color = CommonObjectsStore.me.spriteOutlineColor;
			so.getChildRenderers ();
			so.outlineSize = 1;
			so.enabled = false;
			initialised = true;
		}
	}

	void Update()
	{
		so.enabled = shouldWeOutlineSprite ();
	}

	bool shouldWeOutlineSprite()
	{
		if (Vector3.Distance (CommonObjectsStore.player.transform.position, this.transform.position) > 5) {
			return false;
		} else {
			return true;
			//foreach (PlayerAction pa in myActions) {
			//	if (pa.canDo () == true) {
			//		return true;
			//	}
			//}
			//return false;
		}
	}


}
