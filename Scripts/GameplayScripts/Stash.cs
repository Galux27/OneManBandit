using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stash : MonoBehaviour {
	/// <summary>
	/// Controller for the stash, an object in the players home where the player stores important items & puts cash to progress through the game. 
	/// </summary>

	public static Stash me;
	public Container myContainer;
	public int stashValue = 0;
	void Awake()
	{
		me = this;
	}
	// Use this for initialization
	void Start () {
		myContainer = this.gameObject.GetComponent<Container> ();
	}

	void Update(){
		checkForItemsThatHaveValue ();
		myContainer.containerName = "Stash: £" + stashValue.ToString ();
	}

	void checkForItemsThatHaveValue()
	{
		List<HighValueItem> items = new List<HighValueItem> ();
		foreach (Item i in myContainer.itemsInContainer) {
			HighValueItem hi = i.GetComponent<HighValueItem> ();
			if (hi == null) {

			} else {
				items.Add (hi);
			}
		}

		foreach (HighValueItem hi in items) {
			stashValue += hi.value;
			myContainer.removeItemFromContainer (hi);
			Destroy (hi.gameObject);
		}

	}

}
