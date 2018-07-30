using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

/// <summary>
/// Class that controls loading scenes
/// </summary>
public class LoadScreen : MonoBehaviour {
	public static LoadScreen loadScreen;
	public bool loadScene = false;
	public string scene;
	public Text loadText;
	public Image myBackground;
	public float loading = 0.0f,fadeInTimer=0.0f;
	public string message = "";
	public bool startLoading = false,loadOnAwake=false,coroutineStarted=false;
	// Use this for initialization

	void Awake()
	{
		loadScreen = this;
		resetLoadScreen ();
		if (IDManager.me == null) {
			GameObject g = new GameObject ();
			g.name = "IDManager";
			g.AddComponent<IDManager> ();
		}
	}

	void Start () {

		if (loadOnAwake == true) {
			startLoading = true;
		}
	}

	void fadeInColor()
	{
		fadeInTimer += Time.deltaTime;
		myBackground.color = new Color (0, 0, 0, fadeInTimer);
		loadText.color = new Color (1, 1, 1, fadeInTimer);
	}

	// Update is called once per frame
	void Update () {
		
		if (startLoading == true) {
			if (fadeInTimer < 1.0f) {
				fadeInColor ();
			} else {
				loadText.color = new Color (1, 1, 1, Mathf.PingPong (Time.time, 1));
				if (coroutineStarted == false) {
					StartCoroutine (LoadNewScene ());
				}
			}
		} else {
			myBackground.color = new Color (0, 0, 0, fadeInTimer);
			loadText.color = new Color (1, 1, 1, fadeInTimer);
		}
	}

	IEnumerator LoadNewScene()
	{
		coroutineStarted = true;
		yield return new WaitForSeconds (0.5f);
		AsyncOperation unload = SceneManager.UnloadSceneAsync (SceneManager.GetActiveScene ().name);
		if (unload == null) {

		} else {
			while (!unload.isDone) {
				loadText.text = "Unloading Level " + Mathf.Round (unload.progress * 100).ToString () + "%";

				yield return null;
			}
		}

		if (Application.isEditor == false) {
			AsyncOperation async = SceneManager.LoadSceneAsync (scene, LoadSceneMode.Single);
			while (!async.isDone) {
				loadText.text = message + Mathf.Round (async.progress * 100).ToString () + "%";

				yield return null;
			}
		} else {
			#if UNITY_EDITOR

			AsyncOperation async = EditorSceneManager.LoadSceneAsync (scene, LoadSceneMode.Single);
			while (!async.isDone) {
				loadText.text = "EDITOR: "+message + Mathf.Round (async.progress * 100).ToString () + "%";

				yield return null;
			}
			#endif
		}

	}

	void resetLoadScreen(){
		startLoading = false;
		fadeInTimer = 0.0f;
	}

	public void loadGivenScene(string sceneName)
	{
		if (InventorySwitch.me == null) {

		} else {
			InventorySwitch.me.disable ();
		}
		scene = sceneName;
		startLoading = true;
		fadeInTimer = 0.0f;
		LoadingDataStore.me.storePlayerData ();
	}

	/// <summary>
	/// Sometimes we don't need to save data e.g. going from the main menu to the first level, restarting after dying etc...
	/// </summary>
	/// <param name="sceneName">Scene name.</param>
	public void loadSceneWithoutStoringData(string sceneName)
	{
		if (InventorySwitch.me == null) {

		} else {
			InventorySwitch.me.disable ();
		}
		scene = sceneName;
		startLoading = true;
		fadeInTimer = 0.0f;
	}
}
