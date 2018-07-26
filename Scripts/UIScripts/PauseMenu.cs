using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour {
	public static PauseMenu me;
	public GameObject pauseGUIParent;
	public GameObject resumeObj,hintObj,controlObj,exitObj,hintsPar,controlPar; 

	void Awake()
	{
		me = this;
	}

	public void openPauseMenu()
	{
		if (Inventory.playerInventory.inventoryGUI.activeInHierarchy == true) {
			Inventory.playerInventory.inventoryGUI.SetActive (false);
		}
		pauseGUIParent.SetActive (true);
		resumeObj.SetActive (true);
		Time.timeScale = 0.0f;
	}

	public void resume()
	{
		pauseGUIParent.SetActive (false);
		Time.timeScale = 1.0f;
	}

	public void openHints()
	{
		hintObj.SetActive (true);
		resumeObj.SetActive (false);
	}

	public void openControls()
	{
		controlObj.SetActive (true);
		resumeObj.SetActive (false);
	}

	public void exitGame()
	{
		LoadScreen.loadScreen.loadGivenScene ("MainMenu");
		pauseGUIParent.SetActive (false);
		Time.timeScale = 1.0f;
	}

	public void closeHints()
	{
		hintObj.SetActive (false);
		resumeObj.SetActive (true);
	}

	public void closeControls()
	{
		controlObj.SetActive (false);
		resumeObj.SetActive (true);
	}

	public void restart()
	{
		LoadScreen.loadScreen.loadSceneWithoutStoringData(SceneManager.GetActiveScene ().name);
	}


}
