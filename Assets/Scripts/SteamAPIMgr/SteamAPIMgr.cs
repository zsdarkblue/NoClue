using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if !Steamworks_Off
using Steamworks;
#endif

namespace BeatsFever{
	public class LeaderBoardName
	{  
		public const string StoryTopScoreLeaderboard = "StoryTopScoreLeaderboard";
		public const string ChallengeTopScoreLeaderboard = "ChallengeTopScoreLeaderboard2";
	}

	public class StatsName
	{  
		public const string BestScoreOnce = "BestScoreOnce";
		public const string BestComboOnce = "BestComboOnce";
		public const string TotalPlayedMins = "TotalPlayedMins";

		public const string BestStoryScore = "BestStoryScore";
		public const string BestChallengeScore = "BestChallengeScore";

		//故事模式下的没关最大分数细节的前缀
		public const string StoryTimeScorePrefix_Level = "StoryTimeScorePrefix_Level";
		public const string StoryComboScorePrefix_Level = "StoryComboScorePrefix_Level";
		public const string StoryGhostScorePrefix_Level = "StoryGhostScorePrefix_Level";

		//挑战模式下的最大分数一关的统计
		public const string ChallengeMaxLevel = "ChallengeMaxLevel";
		public const string ChallengeMaxCombo = "ChallengeMaxCombo";
		public const string ChallengeMaxGhost = "ChallengeMaxGhost";
	}

	public class AchievementName
	{  
		public const string ScoreBiggerThan_5 = "ScoreBiggerThan_5";
		public const string ScoreBiggerThan_10 = "ScoreBiggerThan_10";
		public const string ScoreBiggerThan_15 = "ScoreBiggerThan_15";
		public const string ScoreBiggerThan_20 = "ScoreBiggerThan_20";
		public const string ScoreBiggerThan_30 = "ScoreBiggerThan_30";
		public const string ScoreBiggerThan_40 = "ScoreBiggerThan_40";
		public const string ScoreBiggerThan_50 = "ScoreBiggerThan_50";
		public const string ScoreBiggerThan_100 = "ScoreBiggerThan_100";
		public const string ScoreBiggerThan_200 = "ScoreBiggerThan_200";
		public const string ScoreBiggerThan_500 = "ScoreBiggerThan_500";

		public const string ComboBiggerThan_10 = "ComboBiggerThan_10";
		public const string ComboBiggerThan_50 = "ComboBiggerThan_50";
		public const string ComboBiggerThan_100 = "ComboBiggerThan_100";

		public const string PlayedMinBiggerThan_10 = "PlayedMinBiggerThan_10";
		public const string PlayedMinBiggerThan_60 = "PlayedMinBiggerThan_60";
		public const string PlayedMinBiggerThan_120 = "PlayedMinBiggerThan_120";
		public const string PlayedMinBiggerThan_300 = "PlayedMinBiggerThan_300";
		public const string PlayedMinBiggerThan_720 = "PlayedMinBiggerThan_720";
		public const string PlayedMinBiggerThan_1440 = "PlayedMinBiggerThan_1440";
		public const string PlayedMinBiggerThan_6000 = "PlayedMinBiggerThan_6000";

		public const string LeaderBoardRank_1 = "LeaderBoardRank_1";
		public const string LeaderBoardRank_10 = "LeaderBoardRank_10";
		public const string LeaderBoardRank_100 = "LeaderBoardRank_100";
	}

	public class SteamAPIMgr {
		public Dictionary<string,AchievementBase> achievements = new Dictionary<string, AchievementBase>();
		public Dictionary<string,StatsBase> stats = new Dictionary<string, StatsBase>();
		public bool needStoreNewStats = false;
		public bool isStatsValid = false;
		public bool isInited = false;


		public static bool DisableSteamWorks = false;
		#if !Steamworks_Off
		private CGameID m_GameID;
		protected Callback<UserStatsReceived_t> m_UserStatsReceived;
		protected Callback<UserStatsStored_t> m_UserStatsStored;
		protected Callback<UserAchievementStored_t> m_UserAchievementStored;

		private SteamLeaderboard_t steamLeaderboard_StoryScore;
		private SteamLeaderboard_t steamLeaderboard_ChallengeScore;

		protected CallResult<LeaderboardFindResult_t> m_LeaderboardFindResult_Story;  
		protected CallResult<LeaderboardFindResult_t> m_LeaderboardFindResult_Challenge;
		protected CallResult<LeaderboardScoreUploaded_t> m_LeaderboardScoreUploaded;  
		#endif

		public bool RequestCurrentState()
		{
			if (DisableSteamWorks)
				return false;
			
			Debug.Log ("################################################################# RequestCurrentState");
			#if !Steamworks_Off
			bool bSuccess = SteamUserStats.RequestCurrentStats();
			if (!bSuccess)
				Debug.LogError ("SteamUserStats.RequestCurrentStats return false");
			return bSuccess;
			#else
			return false;
			#endif
		
		}

		public int TotalLevelCount = 20;
		public int PlayedMins = 0;

		#if !Steamworks_Off
		public SteamLeaderboard_t GetLeaderBoardHandle(GameMode mode)
		{
			if (mode == GameMode.Story)
				return steamLeaderboard_StoryScore;
			else
				return steamLeaderboard_ChallengeScore;
		}
		#endif
		public void InitMainLeaderBoard()
		{
			if (DisableSteamWorks)
				return;

			#if !Steamworks_Off
			m_LeaderboardFindResult_Story = CallResult<LeaderboardFindResult_t>.Create(OnLeaderboardFindResult_Story);
			m_LeaderboardFindResult_Challenge = CallResult<LeaderboardFindResult_t>.Create(OnLeaderboardFindResult_Challenge);

			m_LeaderboardScoreUploaded = CallResult<LeaderboardScoreUploaded_t>.Create(OnLeaerboardScoreUploaded);

			SteamAPICall_t handle = SteamUserStats.FindOrCreateLeaderboard(LeaderBoardName.StoryTopScoreLeaderboard, ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending, ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric);  
			m_LeaderboardFindResult_Story.Set(handle); 

			SteamAPICall_t handle2 = SteamUserStats.FindOrCreateLeaderboard(LeaderBoardName.ChallengeTopScoreLeaderboard, ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending, ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric);  
			m_LeaderboardFindResult_Challenge.Set(handle2); 
			#endif

		}

		public void Init()
		{
			if (DisableSteamWorks)
				return;
			
			TotalLevelCount = App.Game.ConfigureMgr.GetStoryConfig ().GetTotalLevelCount();
			#if !Steamworks_Off
			SteamAPI.RestartAppIfNecessary ((AppId_t)(625320));

			m_GameID = new CGameID(SteamUtils.GetAppID());

			InitMainLeaderBoard ();

			m_UserStatsReceived = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
			m_UserStatsStored = Callback<UserStatsStored_t>.Create(OnUserStatsStored);
			m_UserAchievementStored = Callback<UserAchievementStored_t>.Create(OnAchievementStored);
			#endif

			//统计信息
			stats.Add (StatsName.BestScoreOnce,new StatsBase(StatsName.BestScoreOnce));
			stats.Add (StatsName.TotalPlayedMins,new StatsBase(StatsName.TotalPlayedMins));
			stats.Add (StatsName.BestComboOnce,new StatsBase(StatsName.BestComboOnce));

			stats.Add (StatsName.BestStoryScore,new StatsBase(StatsName.BestStoryScore));
			stats.Add (StatsName.BestChallengeScore,new StatsBase(StatsName.BestChallengeScore));

			stats.Add (StatsName.ChallengeMaxLevel,new StatsBase(StatsName.ChallengeMaxLevel));
			stats.Add (StatsName.ChallengeMaxCombo,new StatsBase(StatsName.ChallengeMaxCombo));
			stats.Add (StatsName.ChallengeMaxGhost,new StatsBase(StatsName.ChallengeMaxGhost));

			for (int i = 0; i < 6; i++) {
				stats.Add (StatsName.StoryTimeScorePrefix_Level + i.ToString(),new StatsBase(StatsName.StoryTimeScorePrefix_Level + i.ToString()));
				stats.Add (StatsName.StoryComboScorePrefix_Level + i.ToString(),new StatsBase(StatsName.StoryComboScorePrefix_Level + i.ToString()));
				stats.Add (StatsName.StoryGhostScorePrefix_Level + i.ToString(),new StatsBase(StatsName.StoryGhostScorePrefix_Level + i.ToString()));
			}

			//单次分数大于
			achievements.Add (AchievementName.ScoreBiggerThan_5, new AchievementStatsNumberCompare (AchievementName.ScoreBiggerThan_5, StatsName.BestScoreOnce, 5));
			achievements.Add (AchievementName.ScoreBiggerThan_10, new AchievementStatsNumberCompare (AchievementName.ScoreBiggerThan_10, StatsName.BestScoreOnce, 10));
			achievements.Add (AchievementName.ScoreBiggerThan_15, new AchievementStatsNumberCompare (AchievementName.ScoreBiggerThan_15, StatsName.BestScoreOnce, 15));
			achievements.Add (AchievementName.ScoreBiggerThan_20, new AchievementStatsNumberCompare (AchievementName.ScoreBiggerThan_20, StatsName.BestScoreOnce, 20));
			achievements.Add (AchievementName.ScoreBiggerThan_30, new AchievementStatsNumberCompare (AchievementName.ScoreBiggerThan_30, StatsName.BestScoreOnce, 30));
			achievements.Add (AchievementName.ScoreBiggerThan_40, new AchievementStatsNumberCompare (AchievementName.ScoreBiggerThan_40, StatsName.BestScoreOnce, 40));
			achievements.Add (AchievementName.ScoreBiggerThan_50, new AchievementStatsNumberCompare (AchievementName.ScoreBiggerThan_50, StatsName.BestScoreOnce, 50));
			achievements.Add (AchievementName.ScoreBiggerThan_100, new AchievementStatsNumberCompare (AchievementName.ScoreBiggerThan_100, StatsName.BestScoreOnce, 100));
			achievements.Add (AchievementName.ScoreBiggerThan_200, new AchievementStatsNumberCompare (AchievementName.ScoreBiggerThan_200, StatsName.BestScoreOnce, 200));
			achievements.Add (AchievementName.ScoreBiggerThan_500, new AchievementStatsNumberCompare (AchievementName.ScoreBiggerThan_500, StatsName.BestScoreOnce, 500));

			//单次combo大于
			achievements.Add (AchievementName.ComboBiggerThan_10, new AchievementStatsNumberCompare (AchievementName.ComboBiggerThan_10, StatsName.BestComboOnce, 10));
			achievements.Add (AchievementName.ComboBiggerThan_50, new AchievementStatsNumberCompare (AchievementName.ComboBiggerThan_50, StatsName.BestComboOnce, 50));
			achievements.Add (AchievementName.ComboBiggerThan_100, new AchievementStatsNumberCompare (AchievementName.ComboBiggerThan_100, StatsName.BestComboOnce, 100));


			//累计分钟
			achievements.Add (AchievementName.PlayedMinBiggerThan_10, new AchievementStatsNumberCompare (AchievementName.PlayedMinBiggerThan_10, StatsName.TotalPlayedMins, 10));
			achievements.Add (AchievementName.PlayedMinBiggerThan_60, new AchievementStatsNumberCompare (AchievementName.PlayedMinBiggerThan_60, StatsName.TotalPlayedMins, 60));
			achievements.Add (AchievementName.PlayedMinBiggerThan_120, new AchievementStatsNumberCompare (AchievementName.PlayedMinBiggerThan_120, StatsName.TotalPlayedMins, 120));
			achievements.Add (AchievementName.PlayedMinBiggerThan_300, new AchievementStatsNumberCompare (AchievementName.PlayedMinBiggerThan_300, StatsName.TotalPlayedMins, 300));
			achievements.Add (AchievementName.PlayedMinBiggerThan_720, new AchievementStatsNumberCompare (AchievementName.PlayedMinBiggerThan_720, StatsName.TotalPlayedMins, 720));
			achievements.Add (AchievementName.PlayedMinBiggerThan_1440, new AchievementStatsNumberCompare (AchievementName.PlayedMinBiggerThan_1440, StatsName.TotalPlayedMins, 1440));
			achievements.Add (AchievementName.PlayedMinBiggerThan_6000, new AchievementStatsNumberCompare (AchievementName.PlayedMinBiggerThan_6000, StatsName.TotalPlayedMins, 6000));

			//其他类型成就 leaderboard
			achievements.Add (AchievementName.LeaderBoardRank_1, new AchievementBase (AchievementName.LeaderBoardRank_1));
			achievements.Add (AchievementName.LeaderBoardRank_10, new AchievementBase (AchievementName.LeaderBoardRank_10));
			achievements.Add (AchievementName.LeaderBoardRank_100, new AchievementBase (AchievementName.LeaderBoardRank_100));

			isInited = true;
		}

		public void StorePlayResult(GameMode mode,List<LevelScoreItem> levelScoreList)
		{
			if (DisableSteamWorks)
				return;

			int totalSeconds = 0;
			int totalScore = 0;
			int totalCombo = 0;
			int totalCatchGhost = 0;
			foreach (var scoreItem in levelScoreList) {
				totalSeconds += scoreItem.timeScore;
				totalCombo += scoreItem.comboScore;
				totalCatchGhost += scoreItem.ghostScore;

				if (mode == GameMode.Story) {
					totalScore += scoreItem.totalScore;
				} else if (mode == GameMode.Challenge) {
					totalScore += (ScoreMgr.ChallengeLevelRewardScore + scoreItem.comboScore + scoreItem.ghostScore);
				} 
			}

			if (mode == GameMode.Challenge) {
				totalSeconds = ScoreMgr.ChallengeStartSeconds + levelScoreList.Count * ScoreMgr.ChallengeRewardSeconds;
			}

			int playedMin = ((totalSeconds / 60) + 1);
			stats [StatsName.TotalPlayedMins].AddValueToStat (playedMin);
			stats [StatsName.BestScoreOnce].UpdateValueIfBigger (totalScore);
			stats [StatsName.BestComboOnce].UpdateValueIfBigger (totalCombo);

			if (mode == GameMode.Story) {
				var lastBestScore = GetStat (StatsName.BestStoryScore).value;
				if (lastBestScore < totalScore) {
					stats [StatsName.BestStoryScore].ForceUpdateValue (totalScore);

					foreach (var scoreItem in levelScoreList) {
						stats [StatsName.StoryTimeScorePrefix_Level + scoreItem.index.ToString()].ForceUpdateValue(scoreItem.timeScore);
						stats [StatsName.StoryGhostScorePrefix_Level + scoreItem.index.ToString()].ForceUpdateValue(scoreItem.ghostScore);
						stats [StatsName.StoryComboScorePrefix_Level + scoreItem.index.ToString()].ForceUpdateValue(scoreItem.comboScore);
					}
				}
			} else if (mode == GameMode.Challenge) {
				var lastBestScore = GetStat (StatsName.BestChallengeScore).value;
				if (lastBestScore < totalScore) {
					stats [StatsName.BestChallengeScore].ForceUpdateValue (totalScore);
					stats [StatsName.ChallengeMaxCombo].ForceUpdateValue(totalCombo);
					stats [StatsName.ChallengeMaxGhost].ForceUpdateValue(totalCatchGhost);
					stats [StatsName.ChallengeMaxLevel].ForceUpdateValue(levelScoreList.Count);
				}
			}

			needStoreNewStats = true;
			CheckStoreStats ();
			UpdateAchievements ();

			UpdateTotalLeaderBoard (mode);
		}

		void UpdateTotalLeaderBoard(GameMode mode)
		{
			#if !Steamworks_Off

			int bestScore = 0;
			if (mode == GameMode.Story) {
				bestScore = SteamAPIMgr.Instance.GetStat (StatsName.BestStoryScore).value;
				if (steamLeaderboard_StoryScore.m_SteamLeaderboard != 0)  
				{  
					var handle = SteamUserStats.UploadLeaderboardScore(steamLeaderboard_StoryScore, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest, bestScore, null, 0);  
					m_LeaderboardScoreUploaded.Set(handle);  
				} 
			} else {
				bestScore = SteamAPIMgr.Instance.GetStat (StatsName.BestChallengeScore).value;
				if (steamLeaderboard_ChallengeScore.m_SteamLeaderboard != 0)  
				{  
					var handle = SteamUserStats.UploadLeaderboardScore(steamLeaderboard_ChallengeScore, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest, bestScore, null, 0);  
					m_LeaderboardScoreUploaded.Set(handle);  
				} 
			}

			#endif
		}

		public void UpdateAchievements()
		{
			if (DisableSteamWorks)
				return;
			
			foreach (var achievement in achievements.Values) {
				if (!achievement.isAchieved) {
					achievement.CheckUnlockRequirement ();
				}
			}

			CheckStoreStats ();
		}

		public void CheckStoreStats()
		{
			if (DisableSteamWorks)
				return;
			
			if (needStoreNewStats) {

				#if !Steamworks_Off
				bool bSuccess = SteamUserStats.StoreStats();
				if (bSuccess) {
					RefreshAllDataFromSteam ();
				}
				else {
					Debug.LogError ("SteamUserStats.StoreStats return false");
				}
				needStoreNewStats = !bSuccess;
				#endif
			}
		}

		void RefreshAllDataFromSteam()
		{
			foreach (var stat in stats.Values) {
				stat.RefreshDataFromSteam ();
			}

			foreach (var achievement in achievements.Values) {
				achievement.RefreshDataFromSteam ();
			}
		}

		public StatsBase GetStat(string keyName)
		{
			if (stats.ContainsKey (keyName)) {
				return stats [keyName];
			}
			else {
				return null;
			}
		}

		public AchievementBase GetAchievement(string keyName)
		{
			if (achievements.ContainsKey (keyName)) {
				return achievements [keyName];
			}
			else {
				return null;
			}
		}

		public void CheckDateAchievementUnlock()
		{
			if (SteamAPIMgr.DisableSteamWorks)
				return;
			
		}


		public void CheckEspecialAchevementUnlockAfterMusicFinish(ScoreMgr scoreMgr)
		{
			if (SteamAPIMgr.DisableSteamWorks)
				return;

		}

		//-----------------------------------------------------------------------------
		// Purpose: We have stats data from Steam. It is authoritative, so update
		//			our data with those results now.
		//-----------------------------------------------------------------------------
		#if !Steamworks_Off
		void OnUserStatsReceived(UserStatsReceived_t pCallback) {
			
			if (!SteamManager.Initialized)
				return;
			// we may get callbacks for other games' stats arriving, ignore them
			if ((ulong)m_GameID == pCallback.m_nGameID) {
				if (EResult.k_EResultOK == pCallback.m_eResult) {

					SteamAPIMgr.Instance.isStatsValid = true;
					SteamAPIMgr.Instance.RefreshAllDataFromSteam ();
				} else {
					Debug.LogError (m_GameID + "  RequestStats - failed, " + pCallback.m_eResult);
				}
			}
		}

		//-----------------------------------------------------------------------------
		// Purpose: Our stats data was stored!
		//-----------------------------------------------------------------------------
		void OnUserStatsStored(UserStatsStored_t pCallback) {
			// we may get callbacks for other games' stats arriving, ignore them
			if ((ulong)m_GameID == pCallback.m_nGameID) {
				if (EResult.k_EResultOK == pCallback.m_eResult) {
					Debug.Log("StoreStats - success");
				}
				else if (EResult.k_EResultInvalidParam == pCallback.m_eResult) {
					// One or more stats we set broke a constraint. They've been reverted,
					// and we should re-iterate the values now to keep in sync.
					// Fake up a callback here so that we re-load the values.
					UserStatsReceived_t callback = new UserStatsReceived_t();
					callback.m_eResult = EResult.k_EResultOK;
					callback.m_nGameID = (ulong)m_GameID;
					OnUserStatsReceived(callback);
				}
				else {
					Debug.Log("StoreStats - failed, " + pCallback.m_eResult);
				}
			}
		}

		//-----------------------------------------------------------------------------
		// Purpose: An achievement was stored
		//-----------------------------------------------------------------------------
		void OnAchievementStored(UserAchievementStored_t pCallback) {
			// We may get callbacks for other games' stats arriving, ignore them
			if ((ulong)m_GameID == pCallback.m_nGameID) {
				if (0 == pCallback.m_nMaxProgress) {
					Debug.Log("Achievement '" + pCallback.m_rgchAchievementName + "' unlocked!");
				}
				else {
					Debug.Log("Achievement '" + pCallback.m_rgchAchievementName + "' progress callback, (" + pCallback.m_nCurProgress + "," + pCallback.m_nMaxProgress + ")");
				}
			}
		}


		//排行榜
		//请求下载最新排行榜数据 回调
		void OnLeaderboardScoresDownloaded(LeaderboardScoresDownloaded_t pCallback, bool bIOFailure)  
		{  
			//判断是不是同一个排行榜
			if (pCallback.m_hSteamLeaderboard == steamLeaderboard_StoryScore)  
			{
				if (pCallback.m_cEntryCount > 0)  
				{
					for (int i = 0; i < pCallback.m_cEntryCount; i++)  
					{
						LeaderboardEntry_t leaderboardEntry;  
						int[] details = new int[pCallback.m_cEntryCount];  
						//获取下载好的排行榜信息
						SteamUserStats.GetDownloadedLeaderboardEntry(pCallback.m_hSteamLeaderboardEntries, i, out leaderboardEntry, details, pCallback.m_cEntryCount);  
						Debug.Log("用户ID：" + leaderboardEntry.m_steamIDUser + "   分数" + leaderboardEntry.m_nScore + "   排名" + leaderboardEntry.m_nGlobalRank + "   用户名:" + SteamFriends.GetFriendPersonaName(leaderboardEntry.m_steamIDUser));  
					}
				}  
				else  
				{  
					Debug.LogError("排行榜数据为空！");  
				}  
			}  
		}  


		//上传排行榜数据回调
		void OnLeaerboardScoreUploaded(LeaderboardScoreUploaded_t pCallback, bool bIOFailure)  
		{  
			if (pCallback.m_bSuccess != 1)  
			{  
				Debug.LogError ("上传数据失败！！！");
			}  
			else  
			{  
				Debug.Log("成功上传价值数据：" + pCallback.m_nScore + "榜内数据是否需要变更：" + pCallback.m_bScoreChanged  
					+ "新的排名：" + pCallback.m_nGlobalRankNew + "上次排名：" + pCallback.m_nGlobalRankPrevious);  

				if (pCallback.m_nGlobalRankNew == 1) {
					var achievement = GetAchievement (AchievementName.LeaderBoardRank_1);
					if (!achievement.isAchieved) {
						achievement.UnlockAchievement ();
						CheckStoreStats ();
					}
				}
				if(pCallback.m_nGlobalRankNew <= 10)
				{
					var achievement = GetAchievement (AchievementName.LeaderBoardRank_10);
					if (!achievement.isAchieved) {
						achievement.UnlockAchievement ();
						CheckStoreStats ();
					}
				}
				if(pCallback.m_nGlobalRankNew <= 100)
				{
					var achievement = GetAchievement (AchievementName.LeaderBoardRank_100);
					if (!achievement.isAchieved) {
						achievement.UnlockAchievement ();
						CheckStoreStats ();
					}
				}


				if (pCallback.m_nGlobalRankPrevious == 0|| pCallback.m_bScoreChanged != 0)  
				{  

				}  
			}  
		}  

		//初始化排行榜
		void OnLeaderboardFindResult_Story(LeaderboardFindResult_t pCallback, bool bIOFailure)  
		{  
			if (!SteamManager.Initialized)  
				return;  
			
			if (pCallback.m_hSteamLeaderboard.m_SteamLeaderboard != 0 && pCallback.m_bLeaderboardFound != 0)  
			{  
				steamLeaderboard_StoryScore = pCallback.m_hSteamLeaderboard;  
			}
		}  

		void OnLeaderboardFindResult_Challenge(LeaderboardFindResult_t pCallback, bool bIOFailure)  
		{  
			if (!SteamManager.Initialized)  
				return;  

			if (pCallback.m_hSteamLeaderboard.m_SteamLeaderboard != 0 && pCallback.m_bLeaderboardFound != 0)  
			{  
				steamLeaderboard_ChallengeScore = pCallback.m_hSteamLeaderboard;  
			}
		}  

		void UploadNewTotalScore(int score)  
		{  
			if (steamLeaderboard_StoryScore == null)
				return;
			
			if (steamLeaderboard_StoryScore.m_SteamLeaderboard != 0)  
			{  
				Debug.LogError("上传分数");  

				var handle = SteamUserStats.UploadLeaderboardScore(steamLeaderboard_StoryScore, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest, score, null, 0);  
				m_LeaderboardScoreUploaded.Set(handle);  

			}  
		}  
		#endif
		private static SteamAPIMgr instance;
		public static SteamAPIMgr Instance
		{
			get
			{
				if (instance == null) {
					instance = new SteamAPIMgr ();
				}
				return instance;
			}
		}
	}
}