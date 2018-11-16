using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DemoLevelMission : MonoBehaviour {
	/// <summary>
	/// Class to control early prototype, not used anymore.
	/// </summary>
	public static DemoLevelMission me;
	public GameObject briefcase,car;
	public bool hasCase=false;
	public bool foundCase = false;
	void Awake()
	{
		me = this;
	}

	// Use this for initialization
	void Start () {
		PhoneAlert.me.setMessageText ("Remember, the briefcase should be in the bank vault somewhere. There is equipment in the container accross the street");
	}
	
	// Update is called once per frame
	void Update () {
		if (async == null) {

		} else {
			//Debug.Log ("Async is done = " + async.isDone.ToString ());
		}

		if (hasCase == true || foundCase == true) {
			if (async == null) {
			//	StartCoroutine(LoadNewScene());

			}
		}

		if (foundCase == false) {
			if (Vector2.Distance (briefcase.transform.position, CommonObjectsStore.player.transform.position) < 2.0f) {
				if (foundCase == false) {
					PhoneAlert.me.setMessageText ("I've found the case, I should grab it and get back to the car");
					foundCase = true;
				}
			}
		} else {
			if (briefcase.transform.parent==null) {
				if (hasCase == true) {
					PhoneAlert.me.setMessageText ("I need to pick up the case.");
					hasCase = false;
				}


			} else {
				if (hasCase == false) {
					PhoneAlert.me.setMessageText ("I've found the case, I should grab it and get back to the car");
					hasCase = true;
					//if (async == null) {
					//	StartCoroutine(LoadNewScene());

					//}

				}
			}

			if (Vector2.Distance (CommonObjectsStore.player.transform.position, car.transform.position) < 3.0f && hasCase==true) {
				PhoneAlert.me.setMessageTextWithoutBeep ("Press 'e' to finish the level.");
				if (Input.GetKeyDown (KeyCode.E)) {
					//StartCoroutine(LoadNewScene());
					LoadScreen.loadScreen.loadGivenScene("ClosingScene");
					//async.allowSceneActivation = true;

					//SceneManager.LoadScene ("LoadScreenEnding");
				}
			}
		}
	}
	AsyncOperation async;
	bool loading = false;
	IEnumerator LoadNewScene()
	{
		//yield return new WaitForSeconds (3);
		async = SceneManager.LoadSceneAsync ("LoadScreenEnding");
		if (loading == false) {
			async.allowSceneActivation = false;
			loading = true;
		}
		while (!async.isDone) {
			//loadText.text = message + Mathf.Round(async.progress*100).ToString() + "%";

			yield return null;
		}
	}
}
