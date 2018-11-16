using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class to control the UI for the player health. 
/// </summary>
public class HealthUI : MonoBehaviour {
	

	public Image healthBar;
	RectTransform health;
	public Text bloodDisplay;
	public Image armour,bleeding,weed,coke,heroin,heroinWith,morphine,morphineWith,adrenaline;
	BleedingEffect playerBleed;
	void Awake()
	{
		health = healthBar.GetComponent<RectTransform> ();

	}


	void Update () {

		if (playerBleed == null) {
			playerBleed = PersonHealth.playerHealth.gameObject.GetComponent<BleedingEffect> ();
		} else {
			if (playerBleed.bleeding==true) {
				bleeding.enabled = true;
			} else {
				bleeding.enabled = false;
			}
		}



		if (PersonHealth.playerHealth.armourValue > 0) {
			armour.enabled = true;
			armour.color = new Color (1, 1, 1, PersonHealth.playerHealth.armourValue / 100);
		} else {
			armour.enabled = false;
		}
		disableIcons ();
		foreach (EffectBase eb in EffectsManager.me.effectsOnPlayer) {
			if (eb.effectName == "Heroin High") {
				heroin.enabled = true;
				heroinWith.enabled = false;
			} else if (eb.effectName == "Heroin Withdrawral") {
				heroin.enabled = false;
				heroinWith.enabled = true;
			} else if (eb.effectName == "Cannabis High") {
				weed.enabled = true;
			} else if (eb.effectName == "Cocaine High") {
				coke.enabled = true;
			} else if (eb.effectName == "Morphine High") {
				morphine.enabled = true;
				morphineWith.enabled = false;
			} else if (eb.effectName == "Morphine Withdrawral") {
				morphine.enabled = true;
				morphineWith.enabled = false;
			} else if (eb.effectName == "Adrenaline") {
				adrenaline.enabled = true;
			}
		}

		////Debug.Log (PersonHealth.playerHealth.healthValue);
		health.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, (240.0f / 5200.0f) * PersonHealth.playerHealth.healthValue);
		//bloodDisplay.text = "Blood: " + PersonHealth.playerHealth.healthValue + " ml";
		//health.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, PersonHealth.playerHealth.healthValue * 5f);
		//armour.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, PersonHealth.playerHealth.armourValue * 5f);

	}

	void disableIcons()
	{
		heroin.enabled = false;
		heroinWith.enabled = false;
		weed.enabled = false;
		coke.enabled = false;
		morphine.enabled = false;
		morphineWith.enabled = false;
		adrenaline.enabled = false;
	}
}
