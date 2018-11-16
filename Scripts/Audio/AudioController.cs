using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {

	/// <summary>
	/// Script that controls the playing of Audio & sets the volume based on the sources based on the distance to the player.
	/// </summary>

	public List<AudioSource> myAudioSources;
	// Use this for initialization
	public float audioRange = 20.0f;
	void Awake()
	{
		myAudioSources = new List<AudioSource> ();
	}

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		moniterAudioLoops ();
	}

	void moniterAudioLoops(){
		foreach (AudioSource au in myAudioSources) {
			if (au.loop == true) {
				au.volume = calculateVolumeOfAudio ();
			}
		}
	}

	public void stopLoopingAudio(AudioClip sfx)
	{
		foreach (AudioSource au in myAudioSources) {
			if(au.clip==sfx && au.loop==true)
			{
				au.Stop();
				au.loop=false;
				au.clip=null;
			}
		}
	}

	public void playSoundOnLoop(AudioClip sfx)
	{
		AudioSource au = getFreeAudiosource ();
		au.volume = calculateVolumeOfAudio ();
	//	//Debug.Log ("Volume of " + au.gameObject.name + " is " + au.volume);
		au.clip = sfx;
		au.loop = true;
		au.Play ();
	}

	public void playSound(AudioClip sfx)
	{
		AudioSource au = getFreeAudiosource ();
		au.volume = calculateVolumeOfAudio ();
	//	//Debug.Log ("Volume of " + au.gameObject.name + " is " + au.volume);
		au.clip = sfx;
		au.loop = false;
		au.Play ();
	}

	/// <summary>
	/// Either gets a free audiosource or returns a new one, done like this to prevent sounds stopping when a new one is played e.g. when firing a machine gun the shots are in rapid succession but we don't want to stop the sfx of the previous bullet.
	/// </summary>
	/// <returns>The free audiosource.</returns>
	public AudioSource getFreeAudiosource()
	{
		AudioSource retVal = null;

		foreach (AudioSource aus in myAudioSources) {
			if (aus.isPlaying == false) {
				retVal = aus;
			}
		}
		if (retVal == null) {
			retVal = this.gameObject.AddComponent<AudioSource> ();
			myAudioSources.Add (retVal);
		}
		return retVal;
	}

	float calculateVolumeOfAudio()
	{
		if (this.gameObject.tag == "Player") {
			return 1.0f;
		} else {
			float dist = Vector3.Distance (this.transform.position, CommonObjectsStore.player.transform.position);
			if (dist > audioRange) {
				return 0.0f;
			} else {
				return (audioRange-dist)  /audioRange;
			}
		}
	}
}
