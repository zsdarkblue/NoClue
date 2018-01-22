using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using BeatsFever;

public class MusicEventMgr {

	private ColorCorrectionCurves[] cameraColorCurvers;

//	public float[] LaunchTimers;
//	public MusicEvent[] Events;

	//private int currentEventIndex;
	private AudioSource audioSource;
	private int lastColorIndex = -1;

	//zero means not to update.
	private float lastComboTime = 0;
	public void Start()
	{
		audioSource = DeviceContainer.Instance.MainAudioSource;
//		if(DeviceContainer.Instance.colorCorrectionCurveContainer != null)
//			cameraColorCurvers = DeviceContainer.Instance.colorCorrectionCurveContainer.CameraColorCurvers;
	}

	public void Update()
	{
		CheckComboColorPassedTime ();

//		if (audioSource == null)
//			return;
//
//		if (!audioSource.isPlaying)
//			return;
//
//		if (currentEventIndex >= LaunchTimers.Length)
//			return;
//
//		if (audioSource.time > LaunchTimers [currentEventIndex]) {
//			Events [currentEventIndex].LaunchEvent ();
//			currentEventIndex++;
//		}
	}

	public void CheckComboColorPassedTime()
	{
		if (0 == lastComboTime)
			return;
		
		if (Time.time - lastComboTime > RhyThmDefine.CameraFerverLastTime) {
			ResetCameraColor ();
			lastComboTime = 0;
		}
	}

	public void SetAudioSource(AudioSource _audioSource)
	{
		audioSource = _audioSource;
	}

	public void Reset()
	{
	//	currentEventIndex = 0;
		ResetCameraColor ();
	}
		

	public void SetCameraColorByCombo(int combo)
	{
		int colorIndex = -1;
		if (combo >= RhyThmDefine.CameraFeverLevel3Score) {
			colorIndex = 2;
		}
		else if(combo >= RhyThmDefine.CameraFeverLevel2Score)
		{
			colorIndex = 1;
		}
		else if(combo >= RhyThmDefine.CameraFeverLevel1Score)
		{
			colorIndex = 0;
		}


		if (colorIndex == -1) {
			lastColorIndex = colorIndex;
			return;
		}

		lastComboTime = Time.time;

		for (int i = 0; i < cameraColorCurvers.Length; ++i) {
			if (i == colorIndex) {
				if (!cameraColorCurvers[i].enabled) {
					cameraColorCurvers[i].enabled = true;		
					if (colorIndex != lastColorIndex) {
						App.SoundMgr.Play (AudioResources.se_fever_on);
					}
				}
			}
			else {
				if (cameraColorCurvers[i].enabled) {
					cameraColorCurvers[i].enabled = false;		
				}
			}
		}

		lastColorIndex = colorIndex;
		DeviceContainer.Instance.SetHammerColorByCommboIndex (colorIndex);
	}

	public void ResetCameraColor()
	{
		lastComboTime = 0;
		for (int i = 0; i < cameraColorCurvers.Length; ++i) {
			if (cameraColorCurvers[i].enabled) {
				cameraColorCurvers[i].enabled = false;		
				//App.SoundMgr.Play (AudioResources.se_fever_off);
			}
		}

		DeviceContainer.Instance.SetHammerColorByCommboIndex (-1);
	}
}
