using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class to control the UI for the player health. 
/// </summary>
public class HealthUI : MonoBehaviour {
	

	public Image healthBar;
	RectTransform health,armour;
	public Text bloodDisplay;
	public GameObject armourIcon,bleedingIcon;
	BleedingEffect playerBleed;
	void Awake()
	{
		health = healthBar.GetComponent<RectTransform> ();

	}

	// Use thris for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (playerBleed == null) {
			playerBleed = PersonHealth.playerHealth.gameObject.GetComponent<BleedingEffect> ();
		} else {
			if (playerBleed.bleeding==true) {
				bleedingIcon.SetActive (true);
			} else {
				bleedingIcon.SetActive (false);
			}
		}



		if (PersonHealth.playerHealth.armourValue > 0) {
			armourIcon.SetActive (true);
		} else {
			armourIcon.SetActive (false);
		}

		//Debug.Log (PersonHealth.playerHealth.healthValue);
		health.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, (240.0f / 5200.0f) * PersonHealth.playerHealth.healthValue);
		bloodDisplay.text = "Blood: " + PersonHealth.playerHealth.healthValue + " ml";
		//health.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, PersonHealth.playerHealth.healthValue * 5f);
		//armour.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, PersonHealth.playerHealth.armourValue * 5f);

	}
}
