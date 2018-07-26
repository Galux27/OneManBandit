using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerDeathScript : MonoBehaviour {
	public static PlayerDeathScript me;
	public Image tex;
	public GameObject buttonMenu;
	bool isDead=false;

	public float timer=0.01f,alphaVal=0.0f;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (PersonHealth.playerHealth.healthValue <= 0) {
			fadeOut ();
		}
	}

	void fadeOut()
	{
		CameraController.me.bloodAlphaVal = 0.0f;
		CameraController.me.bloodTexDisp.enabled = false;
		if (alphaVal < 1.0f) {
			timer -= Time.deltaTime;
			if (timer <= 0) {
				alphaVal += 0.01f;
				timer = 0.01f;
				tex.color = new Color (1, 1, alphaVal);
			}


		} else {
			buttonMenu.SetActive (true);
		}
	}
}
