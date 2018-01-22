using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BeatsFever;

namespace BeatsFever.ResMgr
{
	public class LevelData
	{
		public int id = 0;
		public int timeScore = 0;
		public int comboScore = 0;
		public int ghostScore = 0;

		public LevelData(int levelId)
		{
			id = levelId;
			InitFromLocalFileData ();
		}

		public bool IsValid()
		{
			return ghostScore > 0;
		}

		public void RefreshRecordDataByScoreMgr(ScoreMgr scoreMgr)
		{
			SaveLevelRecord ();
		}

		public void SaveLevelRecord()
		{
			string storeNameKey = LocalDataBase.LevelRecordHanderName + id.ToString ();
			string localString = id.ToString() + "," + timeScore.ToString() + "," + comboScore.ToString() + "," + ghostScore.ToString();

			PlayerPrefs.SetString(storeNameKey,localString);
		}

		public void InitFromLocalFileData()
		{
			string storeNameKey = LocalDataBase.LevelRecordHanderName + id.ToString ();
			string localString = PlayerPrefs.GetString(storeNameKey,"");
			if (!string.IsNullOrEmpty (localString)) {
				string[] segments = localString.Split (',');
				for (int i = 0; i < segments.Length; ++i) {
					int intValue;
					if (0 == i) {
						if (int.TryParse (segments [i],out intValue))
							id = intValue;
					}
					if (1 == i) {
						if (int.TryParse (segments [i],out intValue))
							timeScore = intValue;
					}
					else if (2 == i) {
						if (int.TryParse (segments [i],out intValue))
							comboScore = intValue;
					}
					else if (3 == i) {
						if (int.TryParse (segments [i], out intValue)) {
							ghostScore = intValue;
						}
					}
				}
			}
			else {
				ghostScore = 0;
			}
		}
	}
	public class LocalDataBase
	{
		const string GuideOverName = "GuideOver";
		const string UserShieldHeightName = "UserShieldHeight";
		const string UserPassedLevelsName = "UserPassedLevels";

		const string PlayedMinsName = "UserPlayedMins";
		//	const string PlayedMinsName = "UserPlayedMins";

		public const string LevelRecordHanderName = "LevelRecordHander";

		public const string BestChallengeScore = "BestChallengeScore";

		private Dictionary<int,LevelData> LevelRecordDatas = new Dictionary<int, LevelData>();

		public void Start(){

			string clearKey = "hjgfuyuuyashduyy";
			if (!PlayerPrefs.HasKey (clearKey)) {
				PlayerPrefs.DeleteAll ();
				PlayerPrefs.SetInt (clearKey,1);
			}
		}

		public void InitLevelRecordData(){
			for (int i = 0; i < ScoreMgr.StoryTotalLevelCount; i++) {
				LevelRecordDatas.Add (i,new LevelData(i));
			}
		}

		public LevelData GetLevelRecordData(int levelId)
		{
			return LevelRecordDatas [levelId];
		}

		public void SetGuideOver(bool over)
		{
			PlayerPrefs.SetInt (GuideOverName, over ? 1 : 0);
		}
		public bool IsGuideOver()
		{
			if (App.Game.GuideMgr.ForceGuide) {
				return false;			
			}
			else {
				return PlayerPrefs.GetInt (GuideOverName,0) == 0 ? false : true;
			}
		}

		public bool IsUserShieldHeightSet()
		{
			if (App.Game.GuideMgr.ForceGuide) {
				return false;			
			}
			else {
				return PlayerPrefs.HasKey (UserShieldHeightName);
			}
		}

		public void SetUserShieldHeight(float height)
		{
			PlayerPrefs.SetFloat(UserShieldHeightName,height);
		}

		public float GetUserShieldHeight()
		{
			return PlayerPrefs.GetFloat (UserShieldHeightName,0);
		}

		public void SetBestChallengeScore(int score)
		{
			PlayerPrefs.SetInt(BestChallengeScore,score);
		}

		public int GetBestChallengeScore()
		{
			return PlayerPrefs.GetInt (BestChallengeScore,0);
		}

		public void AddPlayedTotalMin(int min)
		{
			int playedMin = GetPlayedTotalMin();
			PlayerPrefs.SetInt(PlayedMinsName,min + playedMin);
		}

		public int GetPlayedTotalMin()
		{
			return PlayerPrefs.GetInt (PlayedMinsName,0);
		}

		public void SaveNewUserPassedLevel(int id)
		{
			string localString = PlayerPrefs.GetString(UserPassedLevelsName,"");
			localString = localString + "," + id.ToString ();
			PlayerPrefs.SetString (UserPassedLevelsName,localString);
		}

		public List<int> LoadUserPassedLevels()
		{
			List<int> allPassedLevels = new List<int> ();
			string localString = PlayerPrefs.GetString(UserPassedLevelsName,"");
			Debug.Log ("GetUserPassedLevels:" + allPassedLevels);

			if (string.IsNullOrEmpty (localString))
				return allPassedLevels;

			string[] segments = localString.Split (',');
			for (int i = 0; i < segments.Length; ++i) {
				int levelId;
				if (int.TryParse (segments [i],out levelId)) {
					allPassedLevels.Add (levelId);
				}
				else {
					Debug.LogError ("can't get passed level ids:"+ segments [i]);
				}
			}

			return allPassedLevels;
		}

		public void ShutDown()
		{
			AddPlayedTotalMin ((int)(Time.realtimeSinceStartup / 60));
		}
	}
}
