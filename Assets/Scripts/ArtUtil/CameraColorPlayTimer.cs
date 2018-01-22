using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using BeatsFever;

public class CameraColorPlayTimer : MonoBehaviour {

	public float delayTime = 3f;
	public float[] LaunchTimers;
	private int currentEventIndex;

	public ColorCorrectionCurves CameraColorCurver;
	// Use this for initialization
	void Start () {
		App.Game.RhythmMgr.OnMusicReset += OnMusicReset;
		currentEventIndex = 0;
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

		if (currentEventIndex >= LaunchTimers.Length)
			return;

		if (App.Game.RhythmMgr.AudioSource.time > LaunchTimers [currentEventIndex]) {
			EnableCameraColorEffect ();
			currentEventIndex++;
		}
	}

	void EnableCameraColorEffect()
	{
		if (null == CameraColorCurver)
			return;

		CameraColorCurver.enabled = true;
	//	App.Game.MusicEventMgr.IsColorCorrectionExclusivity = true;
		Invoke ("DisableColor", delayTime);
	}

	void DisableColor()
	{
	//	App.Game.MusicEventMgr.IsColorCorrectionExclusivity = false;
		App.Game.MusicEventMgr.ResetCameraColor ();
		CameraColorCurver.enabled = false;
	}

	public void OnMusicReset()
	{
		currentEventIndex = 0;
		if (null == CameraColorCurver)
			return;

	//	App.Game.MusicEventMgr.IsColorCorrectionExclusivity = false;
		CameraColorCurver.enabled = false;
	}

	void OnDestroy()
	{
		App.Game.RhythmMgr.OnMusicReset -= OnMusicReset;
	}
}
