using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class IntroController : MonoBehaviour {
	public string levelToLoad="";
	public List<GameObject> objectsToDisplay;
	public AudioClip paperFlick, envelopeOpen,silencedPistol;
	public int counter=0;
	AudioSource au;
	public Text nextButtonText;
	public bool isEnding=false;
	void Awake()
	{
		au = this.gameObject.AddComponent<AudioSource> ();
		au.playOnAwake = false;
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.A)) {
			prev ();
		}

		if (Input.GetKeyDown (KeyCode.D)) {
			next ();
		}
		setObject ();
	}

	void setObject()
	{
		if (isEnding == false) {
			if (counter == objectsToDisplay.Count - 1) {
				nextButtonText.text = "Play";
			} else {
				nextButtonText.text = "Next";
			}
		} else {
			if (counter == objectsToDisplay.Count - 1) {
				nextButtonText.text = "Quit";
			} else {
				nextButtonText.text = "Next";
			}
		}

		for (int x = 0; x < objectsToDisplay.Count; x++) {
			if (x != counter) {
				objectsToDisplay [x].SetActive (false);
			} else {
				objectsToDisplay [x].SetActive (true);

			}
		}
	}

	public void next()
	{
		if (counter < objectsToDisplay.Count - 1) {
			counter++;
		} else {
			if (isEnding == false) {
				LoadScreen.loadScreen.loadGivenScene (levelToLoad);
			//	SceneManager.LoadScene (levelToLoad);
			} else {
				Application.Quit ();
			}
		}

		if (counter == 2) {
			au.clip = envelopeOpen;
		} else if (counter == 4) {
			au.clip = silencedPistol;
		}
		else {
			au.clip = paperFlick;
		}
		au.Stop ();
		au.Play ();
	}

	public void prev()
	{
		if (counter > 0) {
			counter--;
		}
	}

}
