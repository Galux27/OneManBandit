using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BleedingEffect : MonoBehaviour {
	/// <summary>
	/// A bleeding effect for the player and npcs, creates a blood trail behind the person whilst decreasing their health.
	/// </summary>
	PersonHealth ph;
	public GameObject currentBloodEffect;
	public bool bleeding = false;
	public RawImage bleedingOutImage;
	public GameObject p;
	void Awake()
	{
		ph = this.gameObject.GetComponent<PersonHealth> ();
		p = this.GetComponentInChildren<ParticleSystem> ().gameObject;
		p.SetActive( bleeding);
	}
	
	// Update is called once per frame
	void Update () {
		bleed ();

		if (Application.isEditor == true) {
			if (Input.GetKeyDown (KeyCode.B)) {
				if (bleeding==false) {
					setBleeding ();
				} else {
					stopBleeding ();
				}
			}
		}
	}

	bool doWeNeedNewBloodEffect()
	{
		if (currentBloodEffect == null) {
			return true;
		} else {
			return Vector3.Distance (this.transform.position, currentBloodEffect.transform.position) > 1;
		}
	}

	public void setBleeding()
	{
		bleeding = true;
		p.SetActive( true);
	}

	public void stopBleeding()
	{
		bleeding = false;
		p.SetActive(false);
	}


	public float increaseSizeTimer = 0.25f;
	public void bleed()
	{
		if (bleeding == true) {

			increaseSizeTimer -= Time.deltaTime;
			if (increaseSizeTimer <= 0) {
				if (doWeNeedNewBloodEffect () == true) {
					createBlood ();
				}


				if (currentBloodEffect.transform.localScale.x < 1) {
					currentBloodEffect.transform.localScale = new Vector3 (currentBloodEffect.transform.localScale.x + 0.01f, currentBloodEffect.transform.localScale.y + 0.01f, 1);
				}
				ph.healthValue -= Random.Range (1, 5);
				increaseSizeTimer = Random.Range(0.2f,0.3f);

			}
			setBleedingOutAlpha ();

		}
	}

	public void setBleedingOutAlpha()
	{
		if (ph == PersonHealth.playerHealth) {
			float alphaVal = 1 - (ph.healthValue / 5200);
			alphaVal /= 2;
			Color c = BleedOutTexStore.bleedOutDisplay.color;
			c.a = alphaVal;
			BleedOutTexStore.bleedOutDisplay.color = c;
		}
	}


	void createBlood()
	{
		currentBloodEffect = (GameObject) Instantiate (CommonObjectsStore.me.bloodEffects [Random.Range( 0, CommonObjectsStore.me.bloodEffects.Length)], this.transform.position, this.transform.rotation);
	}

	public void bloodImpact(Vector3 pos,Quaternion rot)
	{
		Instantiate (CommonObjectsStore.me.bloodSpurt, pos, rot);
	}
}
