using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBeingDragged : MonoBehaviour {
	Rigidbody2D rid;

	void Awake()
	{
		rid = this.gameObject.GetComponent<Rigidbody2D> ();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		float d = Vector2.Distance (this.transform.position, CommonObjectsStore.player.transform.position);
		if (d > 0.75f && d < 3.5f) {
			Vector3 dir = CommonObjectsStore.player.transform.position - transform.position;
			rid.velocity = new Vector2 (dir.x * 1.5f, dir.y * 1.5f);
			rid.mass = 1;
			rid.angularDrag = 0.25f;
			rid.drag = 0.25f;
		} else if (d > 3.5f) {

			this.transform.parent = null;
			PlayerAction.currentAction = null;
			Destroy (this);

		} else if (d < 0.75f) {
			rid.angularDrag = 111.25f;
			rid.drag = 111.25f;
			rid.velocity = new Vector2 (0, 0);

		}

	}

	void OnDestroy()
	{
		rid.velocity = new Vector2 (0, 0);
	}
}
