using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BeatsFever.GameState;
using BeatsFever.UI;

namespace BeatsFever{

	public class LevelScoreItem
	{
		public LevelScoreItem(int idx)
		{
			index = idx;
		}
		public int index;
		public int timeScore;
		public int comboScore;
		public int ghostScore;
		public int totalScore;
	}

	public class ScoreMgr
	{
		public const int ChangeItemTotalCount = 5;
		public const int StoryStartSeconds = 120;
		public const int ChallengeStartSeconds = 300;
		public const int ChallengeRewardSeconds = 20;
		public const int ChallengeLevelRewardScore= 10;
		public const int WrongPickUseSeconds = 5;
		public const int StoryTotalLevelCount = 6;

		// slideIndex is 0 when it's dot
		public delegate void OnScoreUpdateHandler();
		public OnScoreUpdateHandler OnRightPickUpdate;
		public OnScoreUpdateHandler OnWrongPickUpdate;

		List<LevelScoreItem> levelScoreList = new List<LevelScoreItem>();

		LevelScoreItem currentLevelScore;
		public enum ScoreType
		{
			Null,
			Right,
			Wrong,
		}

		public int LevelIndex = 0;
		public int SceneNameIndex = 1;

		public int CurrentScore = 0;
		public int RightPickCount;
		public int WrongPickCount;
		public int Combo = 0;
		public int MaxCombo = 0;
		public int LeftSeconds = 0;
		public bool IsWin;

		public void StartNewLevel()
		{
			currentLevelScore = new LevelScoreItem (LevelIndex);
		}

		public void InitTotalSeconds(int totalSeconds)
		{
			LeftSeconds = totalSeconds;
		}

		public void AddSeconds(int addSeconds)
		{
			LeftSeconds += addSeconds;
		}

		public void OneSecondPassed()
		{
			LeftSeconds--;

			if (LeftSeconds <= 10) {
				DeviceContainer.Instance.SetCameraEffectActive (true);
			}

			if (LeftSeconds <= 0) {
				//mission failed
				LeftSeconds = 0;

				LoseGame ();
			}
		}

		private void WrongPick()
		{
			LeftSeconds-= WrongPickUseSeconds;
			if (LeftSeconds <= 0) {
				//mission failed
				LeftSeconds = 0;

				LoseGame ();
			}
		}

		public List<LevelScoreItem> GetScoreList()
		{
			return levelScoreList;
		}

		void CalculateScore()
		{
			currentLevelScore.timeScore = LeftSeconds;
			currentLevelScore.comboScore = MaxCombo;
			currentLevelScore.ghostScore = RightPickCount;
			currentLevelScore.totalScore = LeftSeconds + MaxCombo + RightPickCount;

			levelScoreList.Add (currentLevelScore);
		}

		public void LoseGame()
		{
			IsWin = false;
			App.SoundMgr.Play (AudioResources.lose);

			if(App.Game.GameStateMgr.ActiveState is BeatState)
				(App.Game.GameStateMgr.ActiveState as BeatState).TimeUp ();
		}

		public void WinGame()
		{
			IsWin = true;
			App.SoundMgr.Play (AudioResources.win);

			if(App.Game.GameStateMgr.ActiveState is BeatState)
				(App.Game.GameStateMgr.ActiveState as BeatState).ShowWinParticles ();

			if (DeviceContainer.Instance.gameMode == GameMode.Story) {
				if (IsWin && levelScoreList.Count < 5) {
					if(App.Game.GameStateMgr.ActiveState is BeatState)
						(App.Game.GameStateMgr.ActiveState as BeatState).Finish ();
				} else {
					if(App.Game.GameStateMgr.ActiveState is BeatState)
						(App.Game.GameStateMgr.ActiveState as BeatState).Finish ();
				}
			} else {
				if(App.Game.GameStateMgr.ActiveState is BeatState)
					(App.Game.GameStateMgr.ActiveState as BeatState).EnterNextRoom ();
			}

		}

		public void UpdateScore(ScoreType type)
		{
			if (DeviceContainer.Instance.gameMode == GameMode.Story) {

			} else if (DeviceContainer.Instance.gameMode == GameMode.Challenge) {

			}

			if (type == ScoreType.Wrong) {
				Combo = 0;
				WrongPickCount++;
				WrongPick ();
				App.SoundMgr.Play (AudioResources.bad_catch);
			}
			else if (type == ScoreType.Right) {
				Combo++;
				RightPickCount++;
				App.SoundMgr.Play (AudioResources.good_catch);
				if (RightPickCount == ChangeItemTotalCount) {
					WinGame ();
				}
			}


			if (Combo > 1)
				MaxCombo++;

			//send message after all calculate is done
			if (type == ScoreType.Wrong) {
				if (OnWrongPickUpdate != null) {
					OnWrongPickUpdate ();
				}
			}
			else if (type == ScoreType.Right) {
				if (OnRightPickUpdate != null) {
					OnRightPickUpdate();
				}
			}
		}

		public void ResetAll()
		{
			LevelIndex = 0;
			SceneNameIndex = 1;

			ClearLevelScoreData ();
			levelScoreList.Clear ();
		}

		void ClearLevelScoreData()
		{
			CurrentScore = 0;
			MaxCombo = 0;
			RightPickCount = 0;
			WrongPickCount = 0;
			MaxCombo = 0;
			Combo = 0;
		}

		public void CalculateLevelScore(bool success)
		{
			CalculateScore ();
			if (success) {
				LevelIndex += 1;
				ClearLevelScoreData ();
			} else {
				
			}
		}

		public int GetTotalScore(GameMode mode)
		{
			int score = 0;
			if (mode == GameMode.Story) {
				foreach (var levelScore in levelScoreList) {
					score += levelScore.totalScore;
				}
				return score;
			} else if (mode == GameMode.Challenge) {
				foreach (var levelScore in levelScoreList) {
					score += (ChallengeLevelRewardScore + levelScore.comboScore + levelScore.ghostScore);
				}
				return score;
			} else {
				return score;
			}
		}
	}
}

