using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour {
	public static EffectsManager me;

	public EffectBase[] effects;

	public List<EffectBase> effectsOnPlayer;
	public bool initialised=false;
	void Start()
	{
		me = this;

		if (initialised == false) {
			effectsOnPlayer = new List<EffectBase> ();
			effects = this.GetComponents<EffectBase> ();
			initialised = true;
		}
	}

	public void addEffectToPlayer(string effectName)
	{
		EffectBase eb = null;
		foreach (EffectBase effect in effects) {
			if (effect.effectName == effectName) {
				eb = effect;
				break;
			}
		}
		EffectBase onPlayer = CommonObjectsStore.player.AddComponent<EffectBase> ();;
		onPlayer.effectName = eb.effectName;
		onPlayer.length = eb.length;
		onPlayer.effectToAddOnRemoval = eb.effectToAddOnRemoval;
		onPlayer.effectToRemoveOnAdd = eb.effectToRemoveOnAdd;
		onPlayer.randomChanceForEffect = eb.randomChanceForEffect;
		onPlayer.effectChange = eb.effectChange;
		onPlayer.moveMod = eb.moveMod;
		onPlayer.damageMod = eb.damageMod;
		onPlayer.initialise ();
		onPlayer.onAdd ();
		onPlayer.instance = true;
		effectsOnPlayer.Add (onPlayer);
		//Debug.Log ("Added effect to player");
	}

	void reAddEffectToPlayer(string[] data)
	{
		if (effects == null || effects.Length==0) {
			effects = this.GetComponents<EffectBase> ();
		}

		string effectName = data [0];
		EffectBase eb = null;
		foreach (EffectBase effect in effects) {
			if (effect.effectName == effectName) {
				eb = effect;
				break;
			}
		}

		GameObject player = GameObject.FindGameObjectWithTag ("Player");

		EffectBase onPlayer = player.AddComponent<EffectBase> ();;
		onPlayer.effectName = eb.effectName;
		onPlayer.length = eb.length;
		onPlayer.effectToAddOnRemoval = eb.effectToAddOnRemoval;
		onPlayer.effectToRemoveOnAdd = eb.effectToRemoveOnAdd;
		onPlayer.randomChanceForEffect = eb.randomChanceForEffect;
		onPlayer.effectChange = eb.effectChange;
		onPlayer.moveMod = eb.moveMod;
		onPlayer.damageMod = eb.damageMod;
		onPlayer.initialise ();
		onPlayer.onAdd ();
		onPlayer.instance = true;
		onPlayer.day = int.Parse (data [1]);
		onPlayer.month = int.Parse (data [2]);
		onPlayer.year = int.Parse (data [3]);
		onPlayer.initialised = true;
		effectsOnPlayer.Add (onPlayer);
		//Debug.Log ("Added effect to player");
	}

	public void destroyEffectOnAdd(string st)
	{
		foreach (EffectBase eb in effectsOnPlayer) {
			if (eb.effectName == st) {
				eb.destroyEffect ();
				return;
			}
		}
	}

	void Update()
	{
		foreach (EffectBase eb in effectsOnPlayer) {
			if (eb.shouldWeGetRidOfEffect ()) {
				eb.destroyEffect ();
				//Debug.Log ("Removed effect from player");
				return;
			}
		}
	}

	public List<string> getEffectData(){
		List<string> retVal = new List<string> ();
		foreach (EffectBase eb in effectsOnPlayer) {
			string st = "";
			st += eb.effectName + ";";
			st += eb.day + ";";
			st += eb.month + ";";
			st += eb.year;
			retVal.Add (st);
		}
		return retVal;
	}


	public void deserialiseEffects(List<string> effectData){
		if (effects == null || effects.Length==0) {
			effects = this.GetComponents<EffectBase> ();
		}


		string[] split = new string[1];
		split [0] = ";";
		foreach (string st in effectData) {
			string[] data = st.Split (split, System.StringSplitOptions.RemoveEmptyEntries);
			reAddEffectToPlayer (data);
		}
		initialised = true;
	}
}
