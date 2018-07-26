using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerActionUI : MonoBehaviour {
	public static PlayerActionUI me;
	public Dropdown myDropdown;
	public Image myButton;
	public List<PlayerAction> availableActions;
	public GameObject lastObjectSelected;
	public Text description;
	public float timer = 1.0f;
	bool setRed = false;
	public LayerMask mask;
	public PlayerAction curAction;
	public int val=0;
	void Awake(){
		me = this;
		//myDropdown = this.gameObject.GetComponent<Dropdown> ();
		myDropdown.gameObject.transform.parent.gameObject.SetActive(false);// = false;
	}

	// Use this for initialization
	void Start () {
		
	}

	public bool isMenuOpen()
	{
		return myDropdown.gameObject.transform.parent.gameObject.activeInHierarchy;
	}

	// Update is called once per frame
	void Update () {
		if (PlayerAction.currentAction == null) {
			curAction = null;
		} else {
			curAction = PlayerAction.currentAction;
		}
		val = myDropdown.value;
		//rayMouseCursor ();

		//if (isMenuOpen ()) {
		//	setActions (lastObjectSelected);
		//	if (availableActions [val].canDo () == false) {
		//		myButton.color = Color.red;
		//	} else {
		//		myButton.color = Color.white;
		//	}
			//////Debug.Log (myDropdown.value);
	//	} else {
		//	lastObjectSelected = null;
		//}
		if (PlayerAction.currentAction == null) {

		} else {
			PlayerAction.currentAction.doAction ();
		}


	}

	public void doAction()
	{
		if (availableActions.Count > 0) {
		//	availableActions [myDropdown.value].doAction ();

			if (availableActions[val].canDo () == true) {
				//////Debug.Log ("Doing action");
				PlayerAction.currentAction = availableActions[val];
				PlayerAction.currentAction.doAction ();
				myDropdown.gameObject.transform.parent.gameObject.SetActive (false);
				//myDropdown.value = 0;

			} else {
				//////Debug.Log ("Can't do action");

				//PlayerAction.currentAction = null;
			//	myButton.color = Color.red;
				//setRed = true;
			}
		}
	}

	public void fadeRed()
	{
		timer -= Time.deltaTime;
		myButton.color = new Color (timer, 0, 0, 1);
		if (timer <= 0) {
			myButton.color = Color.white;
			setRed = false;

		}
	}

	void rayMouseCursor()
	{
		if (Input.GetMouseButtonDown (1)) {
			Vector3 mousePos = CommonObjectsStore.me.mainCam.ScreenToWorldPoint (Input.mousePosition);

			Ray ray = CommonObjectsStore.me.mainCam.ScreenPointToRay (Input.mousePosition);
			Debug.DrawRay (CommonObjectsStore.me.mainCam.transform.position, mousePos - CommonObjectsStore.me.mainCam.transform.position,Color.green,10.0f);
			RaycastHit2D r = Physics2D.GetRayIntersection (ray,30,mask);



			if (r.collider == null) {
				setActions (null);
				Debug.LogError ("No object was hit with raycast");
			} else {
				if (r.collider.gameObject.GetComponent<PlayerAction> () == true) {
					setActions (r.collider.gameObject);
					Debug.LogError (r.collider.gameObject.name + " was hit with raycast");
				}
			}

		}
	}

	public void setActions(GameObject g){
		if (g == null) {
			myDropdown.value = 0;
			myDropdown.gameObject.transform.parent.gameObject.SetActive(false);
			return;
		} else {
			//

			if (g != lastObjectSelected) {
				myDropdown.value = 0;
			}

			lastObjectSelected = g;


			Vector3 pos = CommonObjectsStore.me.mainCam.WorldToScreenPoint( lastObjectSelected.transform.position);//CommonObjectsStore.me.mainCam.ScreenToWorldPoint (Input.mousePosition);
			myDropdown.gameObject.transform.parent.gameObject.transform.position = new Vector3 (pos.x, pos.y, 10);

			myDropdown.gameObject.transform.parent.gameObject.SetActive(true);
			PlayerAction[] actions = g.GetComponents<PlayerAction> ();
			PlayerAction[] actionsInParent = new PlayerAction[0];

			if (g.transform.parent == null) {

			} else {
				actionsInParent = g.transform.parent.gameObject.GetComponents<PlayerAction> ();
			}

			List<PlayerAction> combinedActions = new List<PlayerAction> ();
			foreach (PlayerAction a in actions) {
				combinedActions.Add (a);
			}

			if (actionsInParent == null) {

			} else {

				foreach (PlayerAction a in actionsInParent) {
					combinedActions.Add (a);
				}

			}


			if (combinedActions.Count == 0) {


				return;
			}

			//////Debug.Log ("Object has " + actions.Length + " actions");
			List<string> actionsStr = new List<string> ();
			//actionsStr.Add ("Actions:");
			availableActions = new List<PlayerAction> ();
			foreach (PlayerAction a in combinedActions) {
				if (a == null) {
					continue;
				}

				//if (a.canDo () == true) {
					actionsStr.Add (a.getType ());
					availableActions.Add (a);
				//}
			}

			myDropdown.ClearOptions ();
			myDropdown.AddOptions (actionsStr);
			description.text = availableActions [myDropdown.value].getDescription (); 
				
		}

		if (availableActions.Count == 0) {
			myDropdown.gameObject.transform.parent.gameObject.SetActive(false);
			lastObjectSelected = null;
		}
	}
}
