using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NewPlayerActionUI : MonoBehaviour {
	public static NewPlayerActionUI me;
	public List<PlayerAction> allActions,myActions;
	public Text description,action,count,prevAction,nextAction;
	public Image bigBG,smallBG;
	public int index=0;
	public bool hidden = true;
	void Awake()
	{
		me = this;
	}

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		decideActionsToDisplay ();
		decideDisplay ();
		inputDetect ();
	}



	void inputDetect()
	{
		if (Input.GetKeyDown (KeyCode.Q)) {
			hidden = !hidden;
		}


		if (hidden == false) {
			if (Input.mouseScrollDelta.y < 0) {
				index++;
			} else if (Input.mouseScrollDelta.y > 0) {
				index--;
			}

			if (index > myActions.Count - 1) {
				index = 0;
			} else if (index < 0) {
				index = myActions.Count - 1;
			}

			if (myActions.Count > 0) {
				if (Input.GetKeyDown (KeyCode.E) && myActions [index].canDo () == true) {
					myActions [index].doAction ();
				}
			}
		}
	}

	void decideDisplay()
	{
		if (hidden == false) {
			if (myActions.Count == 0) {
				description.enabled = false;
				action.enabled = false;
				count.enabled = false;
				prevAction.enabled = false;
				nextAction.enabled = false;
				bigBG.gameObject.SetActive (false);
				return;
			} else {
				this.transform.position = CommonObjectsStore.player.transform.position + new Vector3 (2, 0, 0);
				description.enabled = true;
				action.enabled = true;
				count.enabled = true;
				prevAction.enabled = true;
				nextAction.enabled = true;
				bigBG.gameObject.SetActive (true);
				smallBG.gameObject.SetActive (false);

			}

			if (index > myActions.Count - 1) {
				index = myActions.Count - 1;
			}

			count.text = (index + 1).ToString () + "/" + myActions.Count;

			action.text = myActions [index].getType ();
			prevAction.text = myActions [getPreviousValue()].getType ();
			nextAction.text = myActions [getNextIndexValue()].getType ();
			description.text = "(E) "+myActions [index].getDescription ();



			if (myActions [index].canDo () == true) {
				action.color = Color.white;
			} else {
				action.color = Color.red;
			}
		} else {
			description.enabled = false;
			action.enabled = false;
			prevAction.enabled = false;
			nextAction.enabled = false;

			if (myActions.Count == 0) {
				count.enabled = false;
				bigBG.gameObject.SetActive (false);

				smallBG.gameObject.SetActive (false);
				return;
			} else {
				this.transform.position = CommonObjectsStore.player.transform.position + new Vector3 (2, 0, 0);
				count.enabled = true;
				bigBG.gameObject.SetActive (false);

				smallBG.gameObject.SetActive (true);
				count.text = myActions.Count + " Actions (Q)";

			}

		}
	}

	int getNextIndexValue()
	{
		if (index + 1 <= myActions.Count - 1) {
			return index + 1;
		} else {
			return 0;
		}
	}

	int getPreviousValue()
	{
		if (index - 1 >= 0) {
			return index - 1;
		} else {
			return myActions.Count - 1;
		}
	}

	public static void AddAction(PlayerAction pa)
	{
		if (NewPlayerActionUI.me == null) {
			NewPlayerActionUI.me = FindObjectOfType<NewPlayerActionUI> ();
		}

		if (NewPlayerActionUI.me.allActions == null) {
			NewPlayerActionUI.me.allActions = new List<PlayerAction> ();
		}

		if (NewPlayerActionUI.me.myActions == null) {
			NewPlayerActionUI.me.myActions = new List<PlayerAction> ();
		}

		NewPlayerActionUI.me.allActions.Add (pa);
	}

	void decideActionsToDisplay()
	{
		foreach (PlayerAction a in allActions) {
			if (a == null) {
				return;
			}

			if (myActions.Contains (a) == false) {
				if (Vector2.Distance (a.transform.position, CommonObjectsStore.player.transform.position) < 2.0f && a.enabled==true && lineOfSightToObject(a.gameObject) || Vector3.Distance (a.transform.position, CommonObjectsStore.player.transform.position) < 2.0f && a.getType()=="Release Hostage" && a.gameObject.transform.root.tag=="Player") {
					myActions.Add (a);
				}
			} else {
				if (Vector2.Distance (a.transform.position, CommonObjectsStore.player.transform.position) > 2.0f  || a.enabled==false || lineOfSightToObject(a.gameObject)==false && a.getType()!="Release Hostage" ) {
					myActions.Remove (a);
				}
			}
		}
	}
	public LayerMask lm;
	bool lineOfSightToObject(GameObject g)
	{
		Vector3 dir = CommonObjectsStore.player.transform.position - g.transform.position;
		RaycastHit2D ray = Physics2D.Raycast (g.transform.position, dir, Vector2.Distance (g.transform.position, CommonObjectsStore.player.transform.position),lm);
		bool store = Physics2D.queriesHitTriggers;
		Physics2D.queriesHitTriggers = false;
		if (ray.collider == null) {

		} else {

			if (Vector2.Distance (ray.point, CommonObjectsStore.player.transform.position) < 5) {
				//Debug.Log (ray.collider.gameObject.name);
			}

			if (ray.collider.gameObject.tag == "Player") {
				Physics2D.queriesHitTriggers = store;
				return true;
			}
		}
		Physics2D.queriesHitTriggers = store;
		return false;
	}
}
