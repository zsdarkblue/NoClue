using UnityEngine;
using System.Collections;
#if !Steamworks_Off
using Steamworks;
#endif

namespace BeatsFever{
	public class AchievementUniqueLevelCompleteNumber : AchievementBase {
		public AchievementUniqueLevelCompleteNumber(string apiName,int _compareValue) : base(apiName){
			compareValue = _compareValue;
		}
//
//		public override bool CheckUnlockRequirement()
//		{
//			int count = 0;
//			for (int i = 0; i < SteamAPIMgr.Instance.TotalLevelCount; i++) {
//				if (SteamAPIMgr.Instance.GetStat (StatsName.BestRecordScoreLevel + i.ToString ()).value > 0) {
//					count++;
//				}
//			}
//
//			currentValue = count;
//				
//			if (currentValue >= compareValue) {
//				UnlockAchievement ();
//				return true;
//			}
//			else {
//				return false;
//			}
//		}
	}
}
