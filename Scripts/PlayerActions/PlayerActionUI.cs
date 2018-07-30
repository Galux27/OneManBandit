using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Class that controls the UI for the available actions the player can do. 
/// </summary>
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
	
		if (PlayerAction.currentAction == null) {

		} else {
			PlayerAction.currentAction.doAction ();
		}


	}

	public void doAction()
	{
		if (availableActions.Count > 0) {

			if (availableActions[val].canDo () == true) {
				//////Debug.Log ("Doing action");
				PlayerAction.currentAction = availableActions[val];
				PlayerAction.currentAction.doAction ();
				myDropdown.gameObject.transform.parent.gameObject.SetActive (false);
				//myDropdown.value = 0;

			}
		}
	}

}
