using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using System.Collections;
using BeatsFever.UnityEx;
using BeatsFever.UI;

namespace BeatsFever.GameState
{
	public class LobbyState : GameState
    {
		public LobbyState(int id) : base(id){}
		DayNightSwitcher dayNightSwitcher;
		ScenePickItemContainer pickItemContainer;

		private bool isFirstEnterLobby = true;
		public static bool IsForceReturn = false;
		//0 normal, 1 viking
		public static int ArtIndex = 0;
		LobbyStateParam stateParam;
        public override void Enter(GameStateParam param)
        {
			Debug.Log("LobbyState Enter...");

			DeviceContainer.Instance.SetCameraEffectActive (false);

			SteamAPIMgr.Instance.UpdateAchievements ();
			stateParam = param as LobbyStateParam;
			dayNightSwitcher = GameObject.Find ("DayNightController").GetComponent<DayNightSwitcher>();
			pickItemContainer = GameObject.Find ("SceneRoot").GetComponent<ScenePickItemContainer>();

			DeviceContainer.Instance.DayNightSwitcher_Current = dayNightSwitcher;
			DeviceContainer.Instance.PickItemContainer_Current = pickItemContainer;

			DeviceContainer.Instance.SetHammerColorByCommboIndex (-1);
			DeviceContainer.Instance.PlayMusic (true);
			//App.SoundMgr.PlayBkMusic (MusicResources.lobbyMusic);

			if (isFirstEnterLobby) {
				if (App.Game.LocalDataBase.IsGuideOver ()) {
					ShowLobbyMenu ();
				} else {
					//ShowLobbyMenu ();
					App.Game.GUIFrameMgr.Active (GUIFrameID.GuideUI);
				}

				SteamAPIMgr.Instance.CheckDateAchievementUnlock ();
			}
			else {
				if (IsForceReturn) {
					ShowLobbyMenu ();
				} else {
					ShowResult ();
				}
			}

			IsForceReturn = false;
			isFirstEnterLobby = false;

			PickUIBehaviour.ResetEventTimer ();

			//GameObject hint = GameObject.Instantiate (Resources.Load ("hint")) as GameObject;
			//hint.transform.position = DeviceContainer.Instance.ShieldRoot.transform.position + new Vector3 (0, 0.8f, 1);
		}

		public void ShowResult()
		{
			ResultUI.currentLevelId = stateParam.LastLevelId;
			App.Game.GUIFrameMgr.Active (GUIFrameID.ResultUI);
		}

		public void ShowLobbyMenu()
		{
			if (App.Game.GUIFrameMgr.IsActive (GUIFrameID.ResultUI)) {
				App.Game.GUIFrameMgr.DeActive (GUIFrameID.ResultUI);
			}

			App.Game.GUIFrameMgr.Active(GUIFrameID.RotateMusicSelectUI);
		}

		public void HideUserStandIndicator()
		{
			
		}

		public void EnterCity(GameMode mode)
		{
			RenderPoolManager.Instance.Clear ();
			App.MainGo.StartCoroutineExt(SwitchToCity(mode));
		}
	
		private IEnumerator SwitchToCity(GameMode mode)
		{
			yield return new WaitForSeconds(0.2f);
			var beatParam = new BeatStateParam ();
			if (mode == GameMode.Story) {
				App.Game.ScoreMgr.InitTotalSeconds (ScoreMgr.StoryStartSeconds);
				beatParam.Scene = GameUtil.GetSceneNameByIndex (App.Game.ScoreMgr.SceneNameIndex);
			}
			else if(mode == GameMode.Challenge) {
				App.Game.ScoreMgr.InitTotalSeconds (120);

				App.Game.ScoreMgr.InitTotalSeconds (ScoreMgr.ChallengeStartSeconds);
				string sceneName = GameUtil.GetNextSceneName (GameMode.Challenge,App.Game.ScoreMgr.SceneNameIndex);

				string number = "1";
				if (ArtIndex == 0) {
					beatParam.Scene = sceneName;
					number = beatParam.Scene.Substring (6);
				}
				else if(ArtIndex == 1)
				{
					beatParam.Scene =  sceneName.Replace("level_","viking_");
					number = beatParam.Scene.Substring (7);
				}

				App.Game.ScoreMgr.SceneNameIndex = int.Parse(number);
			}
			App.Game.GameStateMgr.SwitchTo (GameStateID.ST_Beat, beatParam);

			LeaderBoardUI.ClearCachedData ();
		}

        public override void Exit()
        {
			DeviceContainer.Instance.DayNightSwitcher_Current = null;
			if (App.Game.GUIFrameMgr.IsActive (GUIFrameID.CreditsUI)) {
				App.Game.GUIFrameMgr.DeActive (GUIFrameID.CreditsUI);
			}

			DeviceContainer.Instance.SwitchFlashLight (false);

			Debug.Log("LobbyState Exit...");
        }
    }
}