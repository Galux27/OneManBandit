using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour {
	/// <summary>
	/// Controls the music in the game based on events occuring e.g. noticed by civilians, police here...
	/// </summary>


	public static MusicController me;

	public AudioClip normal,civilianNoticed,guardNoticed,policeOnWay,policeHere;
	public AudioSource norm,civ,guard,onWay,here;

	public List<AudioSource> sources;
	public float maxVolume = 0.5f;

	void Awake()
	{
		me = this;
		norm = this.gameObject.AddComponent<AudioSource> ();
		civ = this.gameObject.AddComponent<AudioSource> ();
		guard = this.gameObject.AddComponent<AudioSource> ();
		onWay = this.gameObject.AddComponent<AudioSource> ();
		here = this.gameObject.AddComponent<AudioSource> ();
		norm.clip = normal;
		norm.volume = 0;
		civ.clip = civilianNoticed;
		civ.volume = 0;
		guard.clip = guardNoticed;
		guard.volume = 0;
		onWay.clip = policeOnWay;
		onWay.volume = 0;
		here.clip = policeHere;
		here.volume = 0;
		sources = new List<AudioSource> ();
		sources.Add (norm);
		sources.Add (civ);

		sources.Add (here);
		sources.Add (onWay);
		sources.Add (guard);

	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		decideAudioToPlay ();
	}

	public bool civilianSeenTrigger=false,guardSeenTrigger=false;

	void decideAudioToPlay()
	{
		civilianSeenTrigger = areThereAnyCiviliansRaisingAlarm ();
		guardSeenTrigger = areThereAnyGuardsRaisingAlarm ();

		if (PoliceController.me.swatHere == true || PoliceController.me.copsHere == true || PoliceController.me.backupHere == true) {
			fadeInAudio (here);
			fadeOutAllButGiven (here);
		} else if (PoliceController.me.swatCalled == true || PoliceController.me.copsCalled == true || PoliceController.me.backupCalled == true) {
			fadeInAudio (onWay);
			fadeOutAllButGiven (onWay);
		} else if (guardSeenTrigger == true) {
			fadeInAudio (guard);
			fadeOutAllButGiven (guard);
		} else if (civilianSeenTrigger == true) {
			fadeInAudio (civ);
			fadeOutAllButGiven (civ);
		} else {
			fadeInAudio (norm);
			fadeOutAllButGiven (norm);
		}
	}


	float volumeIncreaseTimer = 0.1f, volumeDecreaseTimer = 0.1f;
	public void fadeInAudio(AudioSource au)
	{
		if (au.isPlaying == false) {
			au.Play ();
		}

		volumeIncreaseTimer -= Time.deltaTime;
		if (volumeIncreaseTimer <= 0) {
			if (au.volume < maxVolume) {
				au.volume += 0.01f;
			}
			volumeIncreaseTimer = 0.1f;
		}
	}

	public void fadeOutAudio(AudioSource au)
	{
		if (au.volume==0) {
			au.Stop ();
		}

		volumeDecreaseTimer-= Time.deltaTime;
		if (volumeDecreaseTimer <=0) {
			if (au.volume > 0) {
				au.volume -= 0.02f;
			}
			volumeDecreaseTimer = 0.1f;
		}
	}

	public void fadeOutAllButGiven(AudioSource a)
	{

		volumeDecreaseTimer-= Time.deltaTime;
		if (volumeDecreaseTimer <=0) {
			foreach (AudioSource au in sources) {
				if (au == a) {
					continue;
				}
				if (au.volume > 0) {
					au.volume -= 0.02f;
				}
			}
			volumeDecreaseTimer = 0.1f;
		}
	}

	bool areThereAnyGuardsRaisingAlarm()
	{
		foreach (NPCController npc in NPCManager.me.npcControllers) {
			if (npc == null || npc.currentBehaviour == null) {
				continue;
			}


			if (npc.npcB.myType == AIType.guard && npc.currentBehaviour.myType == behaviourType.raiseAlarm) {
				return true;
			}
		}
		return false;
	}

	bool areThereAnyCiviliansRaisingAlarm()
	{
		foreach (NPCController npc in NPCManager.me.npcControllers) {

			if (npc == null || npc.currentBehaviour == null) {
				continue;
			}

			if (npc.npcB.myType == AIType.civilian && npc.currentBehaviour.myType == behaviourType.raiseAlarm) {
				return true;
			}
		}
		return false;
	}
}
