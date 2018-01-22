using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using System.Collections;
using BeatsFever.UnityEx;
using System.Text.RegularExpressions;
using System;
using BeatsFever.UI;

namespace BeatsFever.GameState
{
	public class BeatState : GameState
    {
		public BeatState(int id) : base(id){}
	
		BeatStateParam stateParam;
		DayNightSwitcher dayNightSwitcher;
		ScenePickItemContainer pickItemContainer;

		ParticleSystem[] particles;

		public bool IsRetureToLobbyING = true;
		public static bool FinishSceneSuccess;

		float timer = 0;
        public override void Enter(GameStateParam param)
        {
			DeviceContainer.Instance.SetCameraEffectActive (false);

			App.Game.ScoreMgr.StartNewLevel ();

			FinishSceneSuccess = true;
			Debug.Log("BeatState Enter...");
			IsRetureToLobbyING = false;
			stateParam = param as BeatStateParam;

			dayNightSwitcher = GameObject.Find ("DayNightController").GetComponent<DayNightSwitcher>();
			pickItemContainer = GameObject.Find ("SceneRoot").GetComponent<ScenePickItemContainer>();

			particles = GameObject.Find ("FX_Group").GetComponentsInChildren<ParticleSystem>();
			foreach (var part in particles) {
				part.Stop ();
				part.Clear ();
			}

			DeviceContainer.Instance.DayNightSwitcher_Current = dayNightSwitcher;
			DeviceContainer.Instance.PickItemContainer_Current = pickItemContainer;

			DeviceContainer.Instance.PlayMusic (false);
			DeviceContainer.Instance.IsDuringFinding = true;

			DeviceContainer.Instance.HintObject.SetActive (true);

			DeviceContainer.Instance.HintCount.text = DeviceContainer.Instance.CurrentLeftHintCount.ToString() + " / 5";
        }

		public void EnterNextRoom()
		{
			DeviceContainer.Instance.IsDuringFinding = false;
			FinishSceneSuccess = true;
			App.MainGo.StartCoroutineExt(SwitchToNextRoom(DeviceContainer.Instance.gameMode));
		}

		private IEnumerator SwitchToNextRoom(GameMode mode)
		{
			yield return new WaitForSeconds(2f);
			var middleParam = new RoomMiddleStateParam ();
			if (mode == GameMode.Story) {
				App.Game.ScoreMgr.InitTotalSeconds (ScoreMgr.StoryStartSeconds);
				middleParam.NextLevelName = GameUtil.GetNextSceneName (GameMode.Story,App.Game.ScoreMgr.SceneNameIndex);
			}	
			else if(mode == GameMode.Challenge) {
				App.Game.ScoreMgr.AddSeconds (ScoreMgr.ChallengeRewardSeconds);
				//middleParam.NextLevelName = GameUtil.GetNextSceneName (GameMode.Challenge,App.Game.ScoreMgr.SceneNameIndex);
				string sceneName = GameUtil.GetNextSceneName (GameMode.Challenge,App.Game.ScoreMgr.SceneNameIndex);

				string number = "1";
				if (LobbyState.ArtIndex == 0) {
					middleParam.NextLevelName = sceneName;
					number = middleParam.NextLevelName.Substring (6);
				}
				else if(LobbyState.ArtIndex == 1)
				{
					middleParam.NextLevelName =  sceneName.Replace("level_","viking_");
					number = middleParam.NextLevelName.Substring (7);
				}
				App.Game.ScoreMgr.SceneNameIndex = int.Parse(number);
			}
			middleParam.Scene = null;
			App.Game.GameStateMgr.SwitchTo (GameStateID.ST_RoomMiddle, middleParam);
		}

		public void UserForceEnd()
		{
			FinishSceneSuccess = false;

			if (App.Game.GUIFrameMgr.IsActive (GUIFrameID.ResultUI)) {
				App.Game.GUIFrameMgr.DeActive (GUIFrameID.ResultUI);
			}
	
			LobbyState.IsForceReturn = true;
			App.MainGo.StartCoroutineExt(DoReturnToLobby());
		}

		public void TimeUp()
		{
			FinishSceneSuccess = false;
			DeviceContainer.Instance.IsDuringFinding = false;
			Finish();
		}

		public IEnumerator DoReturnToLobby()
		{
			IsRetureToLobbyING = true;
			yield return new WaitForSeconds(0.2f);
			var lobbyParam = new LobbyStateParam ();
			lobbyParam.Scene = "lobby";

			App.Game.GameStateMgr.SwitchTo (GameStateID.ST_Lobby, lobbyParam);
		}

		public override void Update ()
		{
			if (DeviceContainer.Instance.IsDuringFinding) {
				timer += Time.deltaTime;
				if (timer > 1) {
					App.Game.ScoreMgr.OneSecondPassed ();
					timer -= 1;
				}
			}
		}

		public void Finish()
		{
			DeviceContainer.Instance.IsDuringFinding = false;
			App.MainGo.StartCoroutineExt(SwitchToLobby());
		}

		private IEnumerator SwitchToLobby()
		{
			yield return new WaitForSeconds(2f);

			App.MainGo.StartCoroutineExt(DoReturnToLobby());
		}

		public void ShowWinParticles()
		{
			foreach (var part in particles) {
				part.Play ();
			}
		}
			
        public override void Exit()
        {
			if (DeviceContainer.Instance.gameMode == GameMode.Story) {
				App.Game.ScoreMgr.SceneNameIndex = App.Game.ScoreMgr.SceneNameIndex + 1;
			}

			App.Game.ScoreMgr.CalculateLevelScore (FinishSceneSuccess);

			DeviceContainer.Instance.SetCameraEffectActive (false);
			DeviceContainer.Instance.DayNightSwitcher_Current = null;
			App.Game.RhythmMgr.ResetMusicEffectsAndEvents ();
			RenderPoolManager.Instance.Clear ();

			DeviceContainer.Instance.SwitchFlashLight (false);

			DeviceContainer.Instance.HintObject.SetActive (false);
			Debug.Log("BeatState Exit...");
        }
    }
}