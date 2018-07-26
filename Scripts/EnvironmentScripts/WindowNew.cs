using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowNew : MonoBehaviour {
	/// <summary>
	/// Script to control 3d windows. 
	/// </summary>
	public bool windowOpen = false,windowDestroyed = false,initilised=false;
	public GameObject window,windowOpenCol,windowClosedCol,destroyed;
	void Start()
	{
		if (initilised == false) {
			if (this.GetComponent<AudioController> () == false) {
				this.gameObject.AddComponent<AudioController> ();
			}
			closeWindow ();
			window.GetComponent<MeshRenderer> ().sortingOrder = 11;

			if (transform.rotation.eulerAngles.z >= 89 && transform.rotation.eulerAngles.z <= 91 || transform.rotation.eulerAngles.z >= 269 && transform.rotation.eulerAngles.z <= 271) {

			} else {
				window.transform.rotation = Quaternion.Euler (window.transform.eulerAngles.x, window.transform.eulerAngles.y, window.transform.eulerAngles.z - 180);

			}

			toRotateTo = window.transform.rotation;
			initilised = true;
		}
	}

	public void callStart()
	{
		Start ();
	}
	void Update()
	{
		if (windowDestroyed == false) {
			if (Vector3.Distance (this.transform.position, CommonObjectsStore.player.transform.position) < 1.5f) {
				if (Input.GetKeyDown (KeyCode.E)) {
					if (windowOpen == true) {
						this.gameObject.GetComponent<AudioController> ().playSound (SFXDatabase.me.windowInteract);

						closeWindow ();
					} else {
						this.gameObject.GetComponent<AudioController> ().playSound (SFXDatabase.me.windowInteract);

						openWindow ();
					}	
				}
			}
			rotate ();
		}
	}

	public void openWindow()
	{
		windowOpenCol.gameObject.SetActive (true);
		windowClosedCol.gameObject.SetActive (false);
		if (transform.rotation.eulerAngles.z >= 89 && transform.rotation.eulerAngles.z <= 91 || transform.rotation.eulerAngles.z >= 269 && transform.rotation.eulerAngles.z <= 271) {
			toRotateTo = Quaternion.Euler (0, 90, this.transform.rotation.eulerAngles.z);

		} else {
			toRotateTo = Quaternion.Euler (90, 0, this.transform.rotation.eulerAngles.z);

		}
		windowOpen = true;
	}

	public void closeWindow()
	{
		windowOpenCol.gameObject.SetActive (false);
		windowClosedCol.gameObject.SetActive (true);
		toRotateTo = Quaternion.Euler (0, 0, this.transform.rotation.eulerAngles.z);

		windowOpen = false;
	}



	public Quaternion toRotateTo;
	public void rotate()
	{
		window.transform.rotation = Quaternion.Slerp (window.transform.rotation, toRotateTo, 5 * Time.deltaTime);
	}

	public void destroyWindow()
	{
		this.gameObject.GetComponent<AudioController> ().playSound (SFXDatabase.me.smashGlass);

		windowOpenCol.gameObject.SetActive (true);
		windowClosedCol.SetActive (false);
		window.SetActive (false);
		destroyed.SetActive (true);
		destroyed.transform.position = new Vector3 (destroyed.transform.position.x, destroyed.transform.position.y, destroyed.transform.position.z - 0.1f);
		windowDestroyed = true;

		LevelIncidentController.me.addIncident ("Window", this.transform.position);
	}

	public void setDestroyed()
	{
		windowOpenCol.gameObject.SetActive (true);
		windowClosedCol.SetActive (false);
		window.SetActive (false);
		destroyed.SetActive (true);
		destroyed.transform.position = new Vector3 (destroyed.transform.position.x, destroyed.transform.position.y, destroyed.transform.position.z - 0.1f);
		windowDestroyed = true;
		initilised = true;
	}
}
