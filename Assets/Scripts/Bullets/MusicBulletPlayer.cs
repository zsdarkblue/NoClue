using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine.UI;
using BeatsFever;
using BeatsFever.GameState;
using BeatsFever.UI;

//In this "game", the controller uses data from the analyses is used to create some simple visuals.
namespace BeatsFever{
	public class MusicBulletPlayer
	{
		public enum EState
		{
			WaitForPlay,
			Playing,
			Finish,
		}

		AudioSource audioSource;
		AudioSource sloganAudioSource;
		AudioClip audioClip;
		EState state = EState.WaitForPlay;

		public void Init (AudioSource source,AudioSource sloganSource)
		{		
			App.InputMgr.OnUserInput += OnUserInput;
			audioSource = source;
			sloganAudioSource = sloganSource;
			state = EState.WaitForPlay;
		}	

		public void OnUserInput(InputType type)
		{
			if (type == InputType.Pause) {
				Time.timeScale = 0;
				audioSource.Pause ();
				sloganAudioSource.Pause ();
				App.Game.GUIFrameMgr.Active (GUIFrameID.PauseUI);
				DeviceContainer.Instance.isGamePause = true;
			}
			else if (type == InputType.Resume) {
				Time.timeScale = 1;
				audioSource.UnPause ();
				sloganAudioSource.UnPause ();
				App.Game.GUIFrameMgr.DeActive (GUIFrameID.PauseUI);
				DeviceContainer.Instance.isGamePause = false;
			}
		}

		public void PrepareMusic(string musicName)
		{
			audioClip = Resources.Load ("Audio/"+musicName) as AudioClip;
			if (null == audioClip) {
				Debug.LogError ("no music in path:" + musicName);
			}
			audioSource.clip = audioClip;
			App.Game.RhythmMgr.BuildRhyDataFromFile (musicName);
		}

		public void StartMusic()
		{
			if (audioSource.clip == null)
				Debug.LogWarning ("no songs configured");
			else {
				App.Game.RhythmMgr.ResetRhythmData();
				audioSource.Play ();
				sloganAudioSource.Play ();
			}
		}

		public void ForceEndMusic()
		{
			audioSource.Stop ();
			sloganAudioSource.Stop();
			state = EState.WaitForPlay;
		}

		// OnEndOfSong is called by RhythmTool if the song has ended
		void OnEndOfSong()
		{	
			//(App.Game.GameStateMgr.ActiveState as BeatState).FinishMusic ();
		}

		public void Update ()
		{
			if (DeviceContainer.Instance.isGamePause)
				return;

			if (state == EState.WaitForPlay) {
				if (audioSource.isPlaying) {
					state = EState.Playing;
				}
			}
			else if (state == EState.Playing) {
				if (audioSource.isPlaying) {
					UpdateRhythmMgr ();
				}
				else {
					state = EState.Finish;
					OnEndOfSong ();
				}
			}
		}

		private void UpdateRhythmMgr()
		{
			//BPM
			App.Game.RhythmMgr.RefreshAudioData (88);
			audioSource.GetSpectrumData (RhythmMgr.SpectrumData, 0, FFTWindow.Blackman);
			var readyRhythm = App.Game.RhythmMgr.GetReadyThythm (audioSource.time);
			if (null != readyRhythm) {
				readyRhythm.ReleaseBullet (audioSource.time);
			}
		}


		public void ShutDown()
		{
			App.InputMgr.OnUserInput -= OnUserInput;
		}
	}
}

