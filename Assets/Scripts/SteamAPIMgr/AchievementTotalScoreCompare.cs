using UnityEngine;
using System.Collections;
#if !Steamworks_Off
using Steamworks;
#endif
namespace BeatsFever{
	public class AchievementTotalScoreCompare : AchievementBase {
		public AchievementTotalScoreCompare(string apiName,int _compareValue) : base(apiName){
			compareValue = _compareValue;
		}

//		public override bool CheckUnlockRequirement()
//		{
//			int totalScore = 0;
//			for (int i = 0; i < SteamAPIMgr.Instance.TotalLevelCount; i++) {
//				int levelScore = SteamAPIMgr.Instance.GetStat (StatsName.BestRecordScoreLevel + i.ToString ()).value;
//				totalScore += levelScore;
//			}
//			currentValue = totalScore;
//
//			if (currentValue >= compareValue) {
//				UnlockAchievement ();
//				return true;
//			}
//			else {
//				return false;
//			}
//		}

		private int GetRankByScore(int level,int score)
		{
			return Random.Range (0, 5);
		}
	}
}
