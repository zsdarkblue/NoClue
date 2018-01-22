using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BeatsFever;

public class SceneEventPlayer : MonoBehaviour {

	public int EventID;
	public List<int> LaunchTimers = new List<int>();
	private int currentEventIndex;

	private ParticleSystem[] effects;
	// Use this for initialization
	void Start () {
		App.Game.RhythmMgr.OnMusicReset += OnMusicReset;
		currentEventIndex = 0;
		effects = GetComponentsInChildren<ParticleSystem> ();
	}

	public void Init(List<int> times)
	{
		LaunchTimers.Clear ();
		LaunchTimers.AddRange (times);
	}

	public void Clear()
	{
		LaunchTimers.Clear ();
	}
	
	// Update is called once per frame
	void Update () {
		if (App.Game.RhythmMgr.AudioSource == null) 
		{
			return;
		}

		if (!App.Game.RhythmMgr.AudioSource.isPlaying)
		{
			return;
		}
		
		if (currentEventIndex >= LaunchTimers.Count)
			return;

		if (App.Game.RhythmMgr.AudioSource.time > LaunchTimers [currentEventIndex]) {
			LaunchEffects ();
			currentEventIndex++;
		}
	}

	void LaunchEffects()
	{
		if (null == effects)
			return;

		foreach (var effect in effects) {
			effect.Play ();
		}
	}

	public void OnMusicReset()
	{
		currentEventIndex = 0;
		if (null == effects)
			return;
		
		foreach (var effect in effects) {
			effect.Stop();
		}
	}

	void OnDestroy()
	{
		App.Game.RhythmMgr.OnMusicReset -= OnMusicReset;
	}
}
